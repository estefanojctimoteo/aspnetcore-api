using System.ComponentModel.DataAnnotations;

namespace Core_Api.Services.Api.ViewModels
{
    public class UpdatePasswordViewModel
    {
        [Required] // AspNetUserId (Guid)
        public string Id { get; set; }

        [Required(ErrorMessage = "O e-mail é requerido")]
        [EmailAddress(ErrorMessage = "E-mail em formato inválido")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string SenhaAtual { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "NovaSenha")]
        public string NovaSenha { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirme a senha")]
        [Compare("NovaSenha", ErrorMessage = "A nova senha e a confirmação estão diferentes...")]
        public string NovaSenhaConfirmacao { get; set; }
    }
}
