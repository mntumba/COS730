using COS730.Helpers.Interfaces;
using COS730.Models.Requests;
using COS730.Models.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Reflection;
using AuthenticationService = COS730.UserService.AuthenticationService;

namespace COS730.RestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IEmailHelper _emailHelper;
        private readonly IEncryptionHelper _encryptionHelper;
        public UserController(ILogger<UserController> logger, IEmailHelper emailHelper, IEncryptionHelper encryptionHelper, IOptions<SQLConnectionSettings> sQLConnectionSettings) 
            : base(logger, sQLConnectionSettings)
        {
            _emailHelper = emailHelper;
            _encryptionHelper = encryptionHelper;
        }

        [HttpPost("createAccount")]
        public ActionResult CreateAccount([FromBody] AccountRequest request)
        {
            try
            {
                var _authService = new AuthenticationService(DBConnection, this.Logger);

                var response = _authService.CreateAccount(request, _encryptionHelper, _emailHelper);

                return Ok(response);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Error occurred in [{MethodBase.GetCurrentMethod()!.Name}]");
                return BadRequest("An error occurred.");
            }
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthRequest request)
        {
            try
            {
                var _authService = new AuthenticationService(DBConnection, this.Logger);

                var response = _authService.Authenticate(request, _encryptionHelper);

                if (string.IsNullOrEmpty(response.ErrorMessage))
                {
                    return Ok(response);
                }

                return BadRequest(response.ErrorMessage);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Error occurred in [{MethodBase.GetCurrentMethod()!.Name}]");
                return BadRequest("An error occurred.");
            }
        }

        [HttpPost("verifyAccount")]
        public IActionResult VerifyAccount([FromBody] OTPRequest request)
        {
            try
            {
                var _authService = new AuthenticationService(DBConnection, this.Logger);

                var response = _authService.VerifyAccount(request, _encryptionHelper);

                if (string.IsNullOrEmpty(response.ErrorMessage))
                {
                    return Ok("Account successfully verified.");
                }

                return BadRequest(response.ErrorMessage);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Error occurred in [{MethodBase.GetCurrentMethod()!.Name}]");
                return BadRequest("An error occurred.");
            }
        }
    }
}
