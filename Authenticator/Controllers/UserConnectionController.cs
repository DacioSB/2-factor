using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Authenticator.Models;
using Authenticator.Services;
using Authenticator.Utils;

namespace Authenticator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserConnectionController : ControllerBase
    {
        private readonly IUserConnectionService _userConnectionService;

        public UserConnectionController(IUserConnectionService userService)
        {
            _userConnectionService = userService;
        }

        [HttpGet("{id:length(24)}", Name = "GetUserConnection")]
        [ProducesResponseType(200, Type = typeof(UserConnectionGetByIdDTO))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(string id)
        {
            var userConnection = await _userConnectionService.Get(id);
            if (userConnection == null)
            {
                return NotFound();
            }
            var userConnectionResponseDTO = new UserConnectionGetByIdDTO
            {
                Username = userConnection.Username,
                Issuer = userConnection.Issuer,
                totp = userConnection.totp,
                remainingSeconds = userConnection.remainingSeconds,
                futureTotp = userConnection.futureTotp
            };


            return Ok(userConnectionResponseDTO);
        }

        //GETALL
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<UserConnectionGetAllDTO>))]
        public async Task<IActionResult> GetAll()
        {
            var userConnections = await _userConnectionService.GetAll();
            var userConnectionsResponse = userConnections.Select(userConnection => new UserConnectionGetAllDTO
            {
                Id = userConnection.Id,
                Username = userConnection.Username,
                Issuer = userConnection.Issuer,
                CreatedAt = userConnection.CreatedAt,
                UpdatedAt = userConnection.UpdatedAt
            });
            return Ok(userConnectionsResponse);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(UserConnectionResponseDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(UserConnectionDTO userdto)
        {
            UserConnection userConnection = await Decrypt(userdto);
            if (userConnection == null)
            {
                return BadRequest();
            }

            await _userConnectionService.Add(userConnection);
            var userConnectionResponseDTO = new UserConnectionResponseDTO
            {
                Id = userConnection.Id
            };

            return CreatedAtRoute("GetUserConnection", new { id = userConnection.Id }, userConnectionResponseDTO);
        }

        private async Task<UserConnection> Decrypt(UserConnectionDTO userdto)
        {
            string encryptedUrl = userdto.EncryptedURL;
            string encryptedIv = userdto.EncryptedIv;
            if (
                encryptedUrl == null ||
                encryptedIv == null
            )
            {
                throw new ArgumentNullException();
            }
            string decryptedUrl = await CryptoService.Decrypt(encryptedUrl, encryptedIv);

            decryptedUrl = decryptedUrl.Replace("otpauth://totp/", "");
            string[] split = decryptedUrl.Split(":");
            string issuer = split[0];
            string[] split2 = split[1].Split("?");
            string username = split2[0];
            string[] split3 = split2[1].Split("=");
            string secret = split3[1];
            UserConnection userConnection = new UserConnection
            {
                Username = username,
                Issuer = issuer,
                Secret = secret,
                CreatedAt = DateTime.Now.ToString(),
                UpdatedAt = DateTime.Now.ToString()
            };
            userConnection.Id = ObjectId.GenerateNewId().ToString();
            return userConnection;

        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, UserConnection userIn)
        {
            var userConnection = await _userConnectionService.Get(id);

            if (userConnection == null)
            {
                return NotFound();
            }

            userConnection.Username = userIn.Username;
            userConnection.Secret = userIn.Secret;
            userConnection.Issuer = userIn.Issuer;
            userConnection.UpdatedAt = DateTime.Now.ToString();

            await _userConnectionService.Update(userConnection);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var userConnection = await _userConnectionService.Get(id);

            if (userConnection == null)
            {
                return NotFound();
            }

            await _userConnectionService.Delete(id);

            return NoContent();
        }
    }
}

