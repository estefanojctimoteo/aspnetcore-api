using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Core_Api.Infra.Data.Context;

namespace Core_Api.Infra.Data.UoW
{
    public class ContextManager
    {
        public const string _ContextKey = "ContextManager.Context.EF";
        private Dictionary<string, object> _dicConext = new Dictionary<string, object>();
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContextManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public FirstContext Context
        {
            get
            {
                /// VERIFICAR SE O CONTEXT JÁ FOI INICIADO
                if (_httpContextAccessor.HttpContext.Items[_ContextKey] == null)
                {
                    _dicConext = new Dictionary<string, object>();
                    var ctx = new FirstContext();
                    ctx.Database.BeginTransaction();
                    _dicConext.Add(_ContextKey, ctx);
                    _httpContextAccessor.HttpContext.Items[_ContextKey] = _dicConext;
                }
                else
                {
                    /// SEMPRE QUE VIR UMA NOVA INSTÂNCIA DENTRO DE UMA MESMA REQUISIÇÃO
                    /// ADICIONA NO ARRAY PARA QUE SOMENTE O PRIMEIRO  POSSA EXECUTAR O SAVECHANGES
                    _dicConext = (Dictionary<string, object>)_httpContextAccessor.HttpContext.Items[_ContextKey];
                    _dicConext.Add(_ContextKey + _dicConext.Count, "");


                    /// RETORNAR O HttpContext COM DICTIONARY (SOMENTE A POSIÇÃO 1 ARMAZENA O DataBaseContext)
                    _httpContextAccessor.HttpContext.Items[_ContextKey] = _dicConext;

                }

                var httpContext = (Dictionary<string, object>)_httpContextAccessor.HttpContext.Items[_ContextKey];

                /// RETORNAR SEMPRE O CONTEXTO, O PRIMEIRO DO ARRAY
                return (FirstContext)httpContext[_ContextKey];

            }
        }

        public bool IsFirst
        {
            get
            {

                var _dicConext = (Dictionary<string, object>)_httpContextAccessor.HttpContext.Items[_ContextKey];
                return _dicConext.Count == 1;
            }
        }

        public int RemoveContex
        {
            get
            {
                /// VERIFICAR QUANTAS VEZES O MESMO CONTEXTO FOI REQUISITADO, E SEMPRE QUE TERMINAR DE USAR
                /// REMOVER UM ITEM DO Dictionary dentro do  HttpContext. 
                /// QUANDO RESTAR SOMENTE 1 ITEM, É QUE A REQUISIÇÃO SERÁ "COMITADA"
                var _dicConext = (Dictionary<string, object>)_httpContextAccessor.HttpContext.Items[_ContextKey];
                if (_dicConext.Count > 1)
                {
                    _dicConext.Remove(_ContextKey + (_dicConext.Count - 1));
                    _httpContextAccessor.HttpContext.Items[_ContextKey] = _dicConext;
                }

                return 1;
            }
        }

    }
}
