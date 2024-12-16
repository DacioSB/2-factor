
namespace Authenticator.Models
{
    public class UserConnectionDTO
    {
        public string EncryptedURL { get; set; } = string.Empty;
        public string EncryptedIv { get; set; } = string.Empty;
    }
}