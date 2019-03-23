using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Core_Api.Infra.CrossCutting.Identity.Authorization
{
    public class SigningCredentialsConfiguration
    {
        private const string SecretKey = "thesecretkeygoeshere";
        public static readonly SymmetricSecurityKey Key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
        public SigningCredentials SigningCredentials { get; }

        public SigningCredentialsConfiguration()
        {
            SigningCredentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);
        }
    }
}