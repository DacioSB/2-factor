using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Models;
using OtpNet;
using Service;

namespace Controller
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {

        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        //get user by ID
        [HttpGet("{id}")]
        public ActionResult<User> Get([FromRoute] long id)
        {
            var user = userService.FindByID(id);
            if (user == null) return NotFound();
            return user;
        }

        //sign in user
        [HttpPost("signin")]
        public ActionResult<string> SignIn([FromBody] UserSignInDTO signInData)
        {
            if (string.IsNullOrEmpty(signInData.Email))
            {
                return BadRequest("Email is required");
            }

            if (string.IsNullOrEmpty(signInData.Password))
            {
                return BadRequest("Password is required");
            }

            bool signedin = userService.Signin(signInData.Email, signInData.Password);
            if (signedin)
            {
                return Ok("Successfully signed in!");
            }

            return Unauthorized("Invalid email or password");
        }
        //sign in user
        [HttpPost("code/{id}")]
        public ActionResult<string> VerifyCode([FromRoute] long id, [FromBody] string code)
        {
            
            User? user = userService.FindByID(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Email is required");
            }

            if ((user.Active == 1) && (user.SignedIn == 1))
            {
                bool result = this.userService.SignedInValidated(id, code);
                if (result)
                {
                    return Ok("Code verified Successfully!");
                }else{
                    return Unauthorized("Wrong 6-digit code");
                }
            }else{
                return Unauthorized("User is not active or not signedin");
            }
        }

        [HttpPut("signout/{id}")]
        public ActionResult<object> signout([FromRoute] long id){
            User? user = userService.FindByID(id);
            if (user == null)
            {
                return NotFound("User not found");
            }
            user.SignedIn = 0;
            user.SignedInValidated = 0;
            userService.UpdateUser(user);
            return Ok("Signed out Successfully");

        }      

        //create user
        [HttpPost]
        public ActionResult<UserDTO> Create([FromBody] UserDTOIn user)
        {
            //transform DTO to User
            var realUser = new User
            {
                Email = user.Email,
                Password = user.Password,
                Name = user.Name,
                ActivationKey = Base32Encoding.ToString(KeyGeneration.GenerateRandomKey(20)),
                Active = 1,
                SignedIn = 0,
                SignedInValidated = 0
            };
            var createdUser = userService.Create(realUser);
            string url = $"otpauth://totp/Docitu:{createdUser?.Name}?secret={createdUser?.ActivationKey}";
            byte[] data = Encoding.UTF8.GetBytes(url);
            byte[] key = new byte[32]; // generate a random key or use a shared secret key
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                byte[] iv = aes.IV; // save the IV for later use
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                    }
                    byte[] encryptedData = ms.ToArray();
                    string encryptedUrl = Convert.ToBase64String(encryptedData);
                    string encryptedIv = Convert.ToBase64String(iv);
                    // pass the encryptedUrl and encryptedIv as parameters in the GET request
                    var responseUser = new UserDTO
                    {
                        Email = createdUser?.Email,
                        Name = createdUser?.Name,
                        //fake qr code is an object formed with encryptedUrl and encryptedIv
                        FakeQRCode = new { encryptedUrl, encryptedIv }
                    };

                    return CreatedAtAction(nameof(Get), new { id = createdUser?.Id }, responseUser);
                }
            }
            //transform User to DTO
        }
    }
}