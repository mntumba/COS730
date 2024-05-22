using COS730.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using AuthenticationService = COS730.UserService.AuthenticationService;

namespace COS730.RestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        public UserController(ILogger<UserController> logger) : base(logger)
        {
        }

        [HttpPost("createAccount")]
        public ActionResult CreateAccount([FromBody] AccountRequest request)
        {
            try
            {
                var _authService = new AuthenticationService(DBConnection, this.Logger);

                var response = _authService.CreateAccount(request);

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

                var response = _authService.Authenticate(request);

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

                var response = _authService.VerifyAccount(request);

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
