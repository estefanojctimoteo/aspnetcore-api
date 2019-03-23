using FluentValidation;
using Core_Api.Domain.Core.Models;

namespace Core_Api.Domain.Core.Models
{
    public abstract class EntityEmpresa<T> : Entity<T> where T : Entity<T>
    {
        public long EmpresaId { get; protected set; }
        public long FuncionarioEmpresaId { get; protected set; }
    }
}
