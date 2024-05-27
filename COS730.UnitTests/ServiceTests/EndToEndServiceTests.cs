using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using COS730.Helpers.Interfaces;
using COS730.MessageService;
using Microsoft.Extensions.Logging;
using COS730.Dapper;
using COS730.DBContext.Dapper;
using COS730.Models.Requests;
using COS730.Models.DBModels;
using COS730.MessageService.Interfaces;

namespace COS730.UnitTests.ServiceTests
{
    public class EndToEndServiceTests
    {
        private readonly Mock<IEncryptionHelper> _mockEncryptionHelper;
        private EndToEndService _endToEndService;
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<INLPService> _mockNlpService;
        private Mock<DapperConnection> _mockConnection;
        private readonly DbContextOptions<ApiDBContext> _dbContextOptions;
        private ApiDBContext _dbContext;

        public EndToEndServiceTests()
        {
            _mockEncryptionHelper = new Mock<IEncryptionHelper>();
            _mockLogger = new Mock<ILogger>();
            _mockNlpService = new Mock<INLPService>();

            _dbContextOptions = new DbContextOptionsBuilder<ApiDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase1")
                .Options;

            _dbContext = new ApiDBContext(_dbContextOptions);
            _mockConnection = new Mock<DapperConnection>(_dbContext);

            _endToEndService = new EndToEndService(_mockConnection.Object, _mockLogger.Object, _mockNlpService.Object);
        }

        [Fact]
        public void SendMessage_UserNotFound_ThrowsException()
        {
            // Arrange
            _dbContext.Database.EnsureDeleted();
            _dbContext = new ApiDBContext(_dbContextOptions);
            _mockConnection = new Mock<DapperConnection>(_dbContext);
            _endToEndService = new EndToEndService(_mockConnection.Object, _mockLogger.Object, _mockNlpService.Object); 
            var request = new MessageRequest { SenderEmail = "sender@example.com", RecipientEmail = "recipient@example.com", Message = "Hello" };

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => _endToEndService.SendMessage(request, _mockEncryptionHelper.Object));
        }

        [Fact]
        public void SendMessage_Successful_ReturnsSuccessMessage()
        {
            // Arrange
            var user = new User { Email = "recipient@example.com", PreferedLanguage = "en" };
            _dbContext.User!.Add(user);
            _dbContext.SaveChanges();

            var request = new MessageRequest { SenderEmail = "sender@example.com", RecipientEmail = "recipient@example.com", Message = "Hello" };
            var translatedMessage = "Translated Hello";
            var encryptedMessage = (Convert.FromBase64String("U29tZUVuY3J5cHRlZE1lc3NhZ2U="), Convert.FromBase64String("U29tZUVuY3J5cHRlZEtleQ=="), Convert.FromBase64String("U29tZUlW"));

            var nlpServiceMock = new Mock<NLPService>(_mockConnection.Object, _mockLogger.Object);
            _mockNlpService.Setup(n => n.TranslateMessage(It.IsAny<string>(), It.IsAny<string>())).Returns(translatedMessage);

            _mockEncryptionHelper.Setup(e => e.EncryptMessage(It.IsAny<string>())).Returns(encryptedMessage);

            // Act
            var (response, message) = _endToEndService.SendMessage(request, _mockEncryptionHelper.Object);

            // Assert
            Assert.Equal("Message successfully sent!", message);
            Assert.NotNull(response);
            Assert.Equal(encryptedMessage.Item1, response.MessageData);
            Assert.Equal(encryptedMessage.Item2, response.MessageKey);
            Assert.Equal(encryptedMessage.Item3, response.MessageIV);
        }

        [Fact]
        public void GetChatHistory_ReturnsChatHistory()
        {
            // Arrange
            var senderEmail = "sender@example.com";
            var recipientEmail = "recipient@example.com";
            var encryptedMessage = Convert.FromBase64String("U29tZUVuY3J5cHRlZE1lc3NhZ2U=");
            var messageKey = Convert.FromBase64String("U29tZUVuY3J5cHRlZEtleQ==");
            var messageIV = Convert.FromBase64String("U29tZUlW");
            var decryptedMessage = "Decrypted Message";

            var message = new Message
            {
                SenderEmail = senderEmail,
                RecipientEmail = recipientEmail,
                MessageData = encryptedMessage,
                MessageKey = messageKey,
                MessageIV = messageIV
            };
            _dbContext.Message!.Add(message);
            _dbContext.SaveChanges();

            var request = new ChatHistoryRequest { SenderEmail = senderEmail, RecipientEmail = recipientEmail };

            _mockEncryptionHelper.Setup(e => e.DecryptMessage(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()))
                .Returns(decryptedMessage);

            // Act
            var chatHistory = _endToEndService.GetChatHistory(request, _mockEncryptionHelper.Object);

            // Assert
            Assert.Equal(senderEmail, chatHistory[0].SenderEmail);
            Assert.Equal(recipientEmail, chatHistory[0].RecipientEmail);
            Assert.Equal(decryptedMessage, chatHistory[0].Message);
        }
    }
}
