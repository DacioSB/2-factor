using System.Text;
using OtpNet;

namespace Utils
{
    
    public class TOTPUtils
    {
        private const int DigitLength = 6;
        private const int TimeStepSeconds = 30;
        private const int WindowSize = 1;

        public static byte[] GenerateKey(string key)
        {
            //KeyGeneration.GenerateRandomKey(20); // 20 bytes = 160 bits
            // I want to make the reverse of this: I have a string and I want to convert it to a byte array using OtpNet
            byte[] secretKeyBytes = Encoding.UTF8.GetBytes(key);

            return secretKeyBytes;
        }
        public static Boolean VerifyCode(string? key, string code){
            if (key == null)
            {
                return false;
            }
            byte[] generatedKey = GenerateKey(key);
            var totp = new Totp(generatedKey, step: TimeStepSeconds, totpSize: DigitLength);
            return totp.VerifyTotp(code, out long timeWindowUsed);
        }
    }
}