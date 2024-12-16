using System.Text;
using OtpNet;

namespace Util
{
    public class TOTPGenerator
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

        public static IEnumerable<KeyValuePair<string, string>> GenerateTOTP(string key){
            byte[] generatedKey = GenerateKey(key);
            //1970 utc epoch
            var totp = new Totp(generatedKey, step: TimeStepSeconds, totpSize: DigitLength);
            string totpCode = totp.ComputeTotp();
            string remainingSeconds = totp.RemainingSeconds().ToString();
            //computetotp 30 seconds after now
            DateTime futureTime = DateTime.UtcNow.AddSeconds(30);
            string totpCodeFuture = totp.ComputeTotp(futureTime);
            return new Dictionary<string, string> { { "totp", totpCode }, { "remainingSeconds", remainingSeconds }, { "future_totp", totpCodeFuture } };
        }

    }
}