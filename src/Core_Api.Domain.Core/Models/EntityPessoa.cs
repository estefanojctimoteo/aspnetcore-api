
namespace Core_Api.Domain.Core.Models
{
    public abstract class EntityPessoa<T> : Entity<T> where T : Entity<T>
    {
        public string Nome { get; protected set; }
        public string CpfCnpj { get; protected set; }
    }
}
