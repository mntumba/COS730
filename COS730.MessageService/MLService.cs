using COS730.Dapper;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Google.Cloud.Language.V1;

namespace COS730.MessageService
{
    public class MLService : MainService
    {
        private readonly LanguageServiceClient _client;

        private List<string>  _spamKeywords = new()
        {
            "free", "winner", "congratulations", "prize", "money", "cash", "earn", "income", "win",
            "offer", "click", "deal", "discount", "guarantee", "buy now", "limited time", "act now",
            "urgent", "important", "call now", "risk-free", "investment", "opportunity", "credit",
            "loan", "debt", "lottery", "gift", "amazing", "cheap", "sale", "bargain",
            "special promotion", "save", "instant", "miracle", "no cost", "order now"
        };

        public MLService(DapperConnection connection, ILogger logger) : base(connection, logger)
        {
            _client = LanguageServiceClient.Create();
        }

        public string VerifySpam(string message)
        {
            message = message.ToLowerInvariant();
            message = Regex.Replace(message, "[^a-zA-Z0-9\\s]", "");

            var score = AnalyzeSentiment(message);

            var entitiesScore = AnalyzeEntitySentiment(message);

            var spamScore = score + entitiesScore;

            if (spamScore > 2.25)
            {
                return "The message is considered spam.";
            }

            return "The message is not considered spam.";
        }

        private double AnalyzeSentiment(string message)
        {
            var document = new Document
            {
                Content = message,
                Type = Document.Types.Type.PlainText
            };

            var response = _client.AnalyzeSentimentAsync(new AnalyzeSentimentRequest { Document = document });

            var score = response.Result.DocumentSentiment.Score +
                (response.Result.DocumentSentiment.Magnitude > 0.5 ? 0.5 : 0);

            return score;
        }

        private double AnalyzeEntitySentiment(string message)
        {
            var document = new Document
            {
                Content = message,
                Type = Document.Types.Type.PlainText
            };
            var response = _client.AnalyzeEntitySentimentAsync(document);

            var score = 0.0;

            foreach (var entity in response.Result.Entities)
            {
                if (entity.Type == Entity.Types.Type.Person || entity.Type == Entity.Types.Type.Organization)
                {
                    continue;
                }
                else
                {
                    var se = entity.Sentiment.Score;

                    var lowerCaseEntity = entity.ToString().ToLower();
                    var hasSpamKeywords = _spamKeywords.Any(keyword => lowerCaseEntity.Contains(keyword));

                    score += se + (hasSpamKeywords ? 0.5 : 0);
                }
            }

            return score;
        }
    }
}
