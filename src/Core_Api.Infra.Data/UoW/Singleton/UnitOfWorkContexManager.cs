using System;
using System.Collections;
using System.Threading;

namespace Core_Api.Infra.Data.UoW.Singleton
{
    public class UnitOfWorkContexManager
    {

        /// <summary>
        ///  Class Singleton. Garantee only one context by request
        ///  and only one instance of the class.
        /// </summary>

        private static UnitOfWorkContexManager objUoW;
        private static bool Init;
        static object syncRoot = new object();
        private static object syncAddContext = new object();
        private static object syncRemoveContext = new object();
        private static object syncIinit = new object();
        private Hashtable Context;

        internal UnitOfWorkContexManager()
        {
            Context = new Hashtable();
        }

        /// <summary>
        /// Initialize the Singleton, creating its own instance of the class
        /// 
        /// </summary>
        /// <returns></returns>
        private static bool Initialize()
        {
            try
            {
                if (!Init)
                    lock (syncIinit)
                        if (!Init)
                        {
                            objUoW = new UnitOfWorkContexManager();
                            Init = true;
                        }
                return Init;
            }
            catch (Exception ex)
            {
                Init = false;
                throw new Exception("Couldn't initialize the context." + ex.Message);
            }
        }

        /// <summary>
        /// Verifies if already exists a Context for the Thread
        /// </summary>
        /// <param name="ThreadId"></param>
        /// <returns></returns>
        internal bool InContextOpen(int ThreadId)
        {
            return Context.Contains(ThreadId);
        }

        /// <summary>
        /// Returns the Context stored for the Thread 
        /// </summary>
        /// <param name="ThreadId"></param>
        /// <returns></returns>
        internal object Get(int ThreadId)
        {
            return Context[ThreadId];
        }

        /// <summary>
        /// Inserts the Context for the Thread 
        /// </summary>
        /// <param name="ThreadId"></param>
        /// <returns></returns>
        public static void AddContext(object context, int ThreadId)
        {
            if (!Init)
                lock (syncRoot)
                    if (!Init)
                        Initialize();

            if (objUoW.InContextOpen(ThreadId))
                throw new Exception("Already exists a context for this thread.");
            else
                lock (syncAddContext)
                    if (!objUoW.InContextOpen(ThreadId))
                        objUoW.AddFirstContext(context);
        }

        internal void AddFirstContext(object context)
        {
            int ThreadId = Thread.CurrentThread.ManagedThreadId;
            Context.Add(ThreadId, context);
        }

        public static bool ContextOpen(int ThreadId)
        {
            if (Init)
                return objUoW.InContextOpen(ThreadId);
            else
                return false;
        }

        public static object GetContext(int ThreadId)
        {
            if (Init)
                return objUoW.Get(ThreadId);
            return null;
        }

        /// <summary>
        /// Removes a context of a thread.
        /// </summary>
        /// <param name="ThreadId">Thread ID number to be used.</param>
        public static void RemoveContext(int ThreadId)
        {
            if (Init)
            {
                lock (syncRemoveContext)
                {
                    if (Init)
                        objUoW.Remove(ThreadId);
                }
            }
        }

        internal void Remove(int ThreadId)
        {
            Context.Remove(ThreadId);
        }
    }
}
