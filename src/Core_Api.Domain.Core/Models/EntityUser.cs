using System;
using FluentValidation;
using FluentValidation.Results;

namespace Core_Api.Domain.Core.Models
{
    public abstract class EntityUser<T> : AbstractValidator<T> where T : EntityUser<T>
    {
        protected EntityUser()
        {
            ValidationResult = new ValidationResult();
        }
                
        public Guid Id { get; protected set; }
        public bool Deleted { get; protected set; }
        public void ExecuteDeletion()
        {
            // TODO: should we consider other rules? If yes, insert them here.
            Deleted = true;
        }

        public abstract bool IsValid();
        public ValidationResult ValidationResult { get; protected set; }

        public override bool Equals(object obj)
        {
            var compareTo = obj as EntityUser<T>;

            if (ReferenceEquals(this, compareTo)) return true;
            if (ReferenceEquals(null, compareTo)) return false;

            return Id.Equals(compareTo.Id);
        }

        public static bool operator ==(EntityUser<T> a, EntityUser<T> b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(EntityUser<T> a, EntityUser<T> b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (GetType().GetHashCode() * 907) + Id.GetHashCode();
        }

        public override string ToString()
        {
            return GetType().Name + "[Id = " + Id + "]";
        }
    }
}