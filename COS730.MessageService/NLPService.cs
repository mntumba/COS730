using COS730.Dapper;
using Google.Cloud.Translation.V2;
using Microsoft.Extensions.Logging;

namespace COS730.MessageService
{
    public class NLPService : MainService
    {
        private readonly TranslationClient _client;

        public NLPService(DapperConnection connection, ILogger logger) : base(connection, logger)
        {
            _client = TranslationClient.Create();
        }

        public string TranslateMessage(string message, string targetLanguage)
        {
            var response = _client.TranslateText(message, targetLanguage);

            return response.TranslatedText;
        }
    }
}
