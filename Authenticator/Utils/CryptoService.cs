using System.Security.Cryptography;
using System.Text;

namespace Authenticator.Utils
{
    public static class CryptoService
    {
        public static async Task<string> Decrypt(string encryptedUrl, string encryptedIv)
        {

            byte[] encryptedData = Convert.FromBase64String(encryptedUrl);
            byte[] iv = Convert.FromBase64String(encryptedIv);
            byte[] key = new byte[32]; // use the same key as in the first application
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.IV = iv;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        await cs.WriteAsync(encryptedData, 0, encryptedData.Length);
                    }
                    byte[] decryptedData = ms.ToArray();
                    string decryptedUrl = Encoding.UTF8.GetString(decryptedData);
                    return decryptedUrl;
                }
            }
        }
    }
}