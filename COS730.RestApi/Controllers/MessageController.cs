﻿using COS730.Helpers.Interfaces;
using COS730.MessageService;
using COS730.Models.Requests;
using COS730.Models.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace COS730.RestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : BaseController
    {
        private readonly IEncryptionHelper _encryptionHelper;

        public MessageController(ILogger<MessageController> logger, IEncryptionHelper encryptionHelper, IOptions<SQLConnectionSettings> sQLConnectionSettings) 
            : base(logger, sQLConnectionSettings)
        {
            _encryptionHelper = encryptionHelper;
        }

        [HttpPost("sendMessage")]
        public ActionResult GetChatHistory([FromBody] MessageRequest request)
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
    }
}
