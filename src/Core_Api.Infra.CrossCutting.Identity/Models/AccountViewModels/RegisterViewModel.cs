using System.ComponentModel.DataAnnotations;

namespace Core_Api.Infra.CrossCutting.Identity.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        //[Required(ErrorMessage = "O nome é requerido")]
        //public string Nome { get; set; }

        //[Required(ErrorMessage = "O CPF é requerido")]
        //[StringLength(11)]
        //public string CPF { get; set; }

        [Required(ErrorMessage = "O e-mail é requerido")]
        [EmailAddress(ErrorMessage = "E-mail em formato inválido")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Senha { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirme a senha")]
        [Compare("Senha", ErrorMessage = "The password and confirmation password do not match.")]
        public string SenhaConfirmacao { get; set; }

        [Required(ErrorMessage = "O ID do funcionário é requerido. Pode ser [-1].")]
        public long FuncionarioId { get; set; }

        [Required(ErrorMessage = "O ID do profissional é requerido. Pode ser [-1].")]
        public long ProfissionalId { get; set; }

        //[Required(ErrorMessage = "O ID da empresa é requerido")]
        //public long EmpresaId { get; set; }
    }
}
