using COS730.Helpers.Interfaces;
using COS730.MessageService;
using COS730.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace COS730.RestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : BaseController
    {
        private readonly IEncryptionHelper _encryptionHelper;

        public MessageController(ILogger<MessageController> logger, IEncryptionHelper encryptionHelper) 
            : base(logger)
        {
            _encryptionHelper = encryptionHelper;
        }

        [HttpPost("sendMessage")]
        public ActionResult SendMessage([FromBody] MessageRequest request)
        {
            try
            {
                var _e2eService = new EndToEndService(DBConnection, this.Logger);

                var response = _e2eService.SendMessage(request, _encryptionHelper);

                return Ok(response);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Error occurred in [{MethodBase.GetCurrentMethod()!.Name}]");
                return BadRequest("An error occurred.");
            }
        }

        [HttpPost("getChatHistory")]
        public ActionResult GetChatHistory([FromBody] ChatHistoryRequest request)
        {
            try
            {
                var _e2eService = new EndToEndService(DBConnection, this.Logger);

                var response = _e2eService.GetChatHistory(request, _encryptionHelper);

                return Ok(response);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Error occurred in [{MethodBase.GetCurrentMethod()!.Name}]");
                return BadRequest("An error occurred.");
            }
        }

        [HttpPost("readVoiceMsgAsText/{audioPath}")]
        public ActionResult ReadVoiceMsgAsText(string audioPath)
        {
            try
            {
                var _nlpService = new NLPService(DBConnection, this.Logger);

                var response = _nlpService.ConvertVoiceToText(audioPath);

                return Ok(response);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Error occurred in [{MethodBase.GetCurrentMethod()!.Name}]");
                return BadRequest("An error occurred.");
            }
        }

        [HttpPost("detectSpam/{message}")]
        public ActionResult DetectSpam(string message)
        {
            try
            {
                var _mlService = new MLService(DBConnection, this.Logger);

                var response = _mlService.VerifySpam(message);

                return Ok(response);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Error occurred in [{MethodBase.GetCurrentMethod()!.Name}]");
                return BadRequest("An error occurred.");
            }
        }
    }
}
