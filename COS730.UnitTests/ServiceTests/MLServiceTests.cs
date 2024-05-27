using Google.Cloud.Dialogflow.V2;
using Google.Cloud.Language.V1;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using COS730.Dapper;
using COS730.MessageService;
using Sentiment = Google.Cloud.Language.V1.Sentiment;
using Microsoft.EntityFrameworkCore;
using COS730.DBContext.Dapper;

namespace COS730.UnitTests.ServiceTests
{
    public class MLServiceTests
    {
        private readonly Mock<LanguageServiceClient> _languageServiceClientMock;
        private readonly Mock<SessionsClient> _sessionsClientMock;
        private readonly Mock<DapperConnection> _connectionMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly MLService _mlService;

        public MLServiceTests()
        {
            _languageServiceClientMock = new Mock<LanguageServiceClient>();
            _sessionsClientMock = new Mock<SessionsClient>();
            _connectionMock = new Mock<DapperConnection>();
            _loggerMock = new Mock<ILogger>();

            var dbContextOptions = new DbContextOptionsBuilder<ApiDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase3")
                .Options;

            var dbContext = new ApiDBContext(dbContextOptions);
            _connectionMock = new Mock<DapperConnection>(dbContext);

            _mlService = new MLService(_connectionMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void VerifySpam_MessageContainsSpamKeywords_ReturnsSpamMessage()
        {
            // Arrange
            var message = "Congratulations! You have won a free prize.";

            var sentimentResponse = new AnalyzeSentimentResponse
            {
                DocumentSentiment = new Sentiment { Score = 1.0f, Magnitude = 0.5f }
            };

            _languageServiceClientMock
                .Setup(c => c.AnalyzeSentimentAsync(It.IsAny<AnalyzeSentimentRequest>(), null))
                .ReturnsAsync(sentimentResponse);

            var entitySentimentResponse = new AnalyzeEntitySentimentResponse
            {
                Entities = { new Entity { Sentiment = new Sentiment { Score = 0.5f }, Name = "prize" } }
            };

            _languageServiceClientMock
                .Setup(c => c.AnalyzeEntitySentimentAsync(It.IsAny<Google.Cloud.Language.V1.Document>(), null))
                .ReturnsAsync(entitySentimentResponse);

            // Act
            var result = _mlService.VerifySpam(message);

            // Assert
            Assert.Equal("The message is considered spam.", result);
        }

        [Fact]
        public async Task SuggestRepliesAsync_ValidMessage_ReturnsSuggestedReplies()
        {
            // Arrange
            var message = "Hello";
            var sessionId = Guid.NewGuid().ToString();

            var queryResult = new QueryResult
            {
                FulfillmentMessages =
            {
                new Intent.Types.Message
                {
                    Text = new Intent.Types.Message.Types.Text
                    {
                        Text_ = { "I'm good, thank you!" }
                    }
                }
            }
            };

            var detectIntentResponse = new DetectIntentResponse
            {
                QueryResult = queryResult
            };

            _sessionsClientMock
                .Setup(c => c.DetectIntentAsync(It.IsAny<SessionName>(), It.IsAny<QueryInput>(), null))
                .ReturnsAsync(detectIntentResponse);

            // Act
            var result = await _mlService.SuggestRepliesAsync(message);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    }
}
