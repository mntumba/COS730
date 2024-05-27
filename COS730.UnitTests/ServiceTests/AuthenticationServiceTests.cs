using COS730.Dapper;
using COS730.DBContext.Dapper;
using COS730.Helpers.Interfaces;
using COS730.Models.DBModels;
using COS730.Models.Requests;
using COS730.UserService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace COS730.UnitTests.ServiceTests
{
    public class AuthenticationServiceTests
    {
        private Mock<DapperConnection> _mockConnection;
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<IEncryptionHelper> _mockEncryptionHelper;
        private AuthenticationService _authService;
        private readonly DbContextOptions<ApiDBContext> _dbContextOptions;
        private ApiDBContext _dbContext;

        public AuthenticationServiceTests()
        {
            _mockLogger = new Mock<ILogger>();
            _mockEncryptionHelper = new Mock<IEncryptionHelper>();

            _dbContextOptions = new DbContextOptionsBuilder<ApiDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase2")
                .Options;

            _dbContext = new ApiDBContext(_dbContextOptions);
            _mockConnection = new Mock<DapperConnection>(_dbContext);

            _authService = new AuthenticationService(_mockConnection.Object, _mockLogger.Object);
        }

        private void ResetDatabase()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext = new ApiDBContext(_dbContextOptions);
            _mockConnection = new Mock<DapperConnection>(_dbContext);
            _authService = new AuthenticationService(_mockConnection.Object, _mockLogger.Object);
        }

        [Fact]
        public void Authenticate_UserNotFound_ReturnsErrorMessage()
        {
            // Arrange
            var request = new AuthRequest { Email = "test@example.com", Password = "password" };

            // Act
            var response = _authService.Authenticate(request, _mockEncryptionHelper.Object);

            // Assert
            Assert.Equal("We couldn't find an account associated with this email.", response.ErrorMessage);
        }

        [Fact]
        public void Authenticate_InvalidPassword_ReturnsErrorMessage()
        {
            // Arrange
            ResetDatabase();
            var user = new User { Email = "test@example.com", Password = "hashedpassword", IsVerified = true };
            _dbContext.User!.Add(user);
            _dbContext.SaveChanges();

            var request = new AuthRequest { Email = "test@example.com", Password = "wrongpassword" };
            _mockEncryptionHelper.Setup(e => e.VerifyCode(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            // Act
            var response = _authService.Authenticate(request, _mockEncryptionHelper.Object);

            // Assert
            Assert.Equal("Invalid email or password.", response.ErrorMessage);
        }

        [Fact]
        public void Authenticate_UserNotVerified_ReturnsErrorMessage()
        {
            // Arrange
            ResetDatabase();
            var user = new User { Email = "test@example.com", Password = "hashedpassword", IsVerified = false };
            _dbContext.User!.Add(user);
            _dbContext.SaveChanges();

            var request = new AuthRequest { Email = "test@example.com", Password = "password" };
            _mockEncryptionHelper.Setup(e => e.VerifyCode(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            // Act
            var response = _authService.Authenticate(request, _mockEncryptionHelper.Object);

            // Assert
            Assert.Equal("Account not verified! Please use the code sent to your email to verify your account.", response.ErrorMessage);
        }

        [Fact]
        public void Authenticate_ValidCredentials_ReturnsAuthResponse()
        {
            // Arrange
            ResetDatabase();
            var user = new User { Name = "Test User", Email = "test@example.com", Password = "hashedpassword", IsVerified = true };
            _dbContext.User!.Add(user);
            _dbContext.SaveChanges();

            var request = new AuthRequest { Email = "test@example.com", Password = "password" };
            _mockEncryptionHelper.Setup(e => e.VerifyCode(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            // Act
            var response = _authService.Authenticate(request, _mockEncryptionHelper.Object);

            // Assert
            Assert.Null(response.ErrorMessage);
            Assert.Equal(user.Name, response.Name);
            Assert.Equal(user.Email, response.Email);
            Assert.True(response.IsVerified);
        }

        [Fact]
        public void CreateAccount_SuccessfulCreation_ReturnsSuccessMessage()
        {
            // Arrange
            ResetDatabase();
            var request = new AccountRequest { Name = "Test User", Email = "test@example.com", Password = "password", PreferedLanguage = "en" };
            var otp = "123456";

            _mockEncryptionHelper.Setup(e => e.GenerateOtp()).Returns(otp);
            _mockEncryptionHelper.Setup(e => e.EncryptCode(It.IsAny<string>())).Returns((string s) => s);
            var emailHelperMock = new Mock<IEmailHelper>();
            emailHelperMock.Setup(e => e.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            // Act
            var result = _authService.CreateAccount(request, _mockEncryptionHelper.Object, emailHelperMock.Object);

            // Assert
            Assert.Equal("Account successfully created!", result);
            emailHelperMock.Verify(e => e.SendEmail(request.Email, "Verificattion code", otp), Times.Once);
            var user = _dbContext.User!.SingleOrDefault(u => u.Email == request.Email);
            Assert.NotNull(user);
            Assert.Equal(request.Name, user.Name);
            Assert.Equal(request.Email, user.Email);
            Assert.Equal(request.Password, user.Password);
            Assert.Equal(request.PreferedLanguage, user.PreferedLanguage);
            Assert.False(user.IsVerified);
        }

        [Fact]
        public void VerifyAccount_InvalidOtp_ReturnsErrorMessage()
        {
            //Arrange
                ResetDatabase();
           var user = new User { Email = "test@example.com", OTP = "encryptedotp", IsVerified = false };
            _dbContext.User!.Add(user);
            _dbContext.SaveChanges();

            var request = new OTPRequest { Email = "test@example.com", OTP = "wrongotp" };
            _mockEncryptionHelper.Setup(e => e.VerifyCode(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            // Act
            var response = _authService.VerifyAccount(request, _mockEncryptionHelper.Object);

            // Assert
            Assert.Equal("Wrong Otp!", response.ErrorMessage);
            Assert.False(response.IsVerified);
        }

        [Fact]
        public void VerifyAccount_ValidOtp_ReturnsAuthResponse()
        {
            // Arrange
            ResetDatabase();
            var user = new User { Email = "test@example.com", OTP = "encryptedotp", IsVerified = false };
            _dbContext.User!.Add(user);
            _dbContext.SaveChanges();

            var request = new OTPRequest { Email = "test@example.com", OTP = "correctotp" };
            _mockEncryptionHelper.Setup(e => e.VerifyCode(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            // Act
            var response = _authService.VerifyAccount(request, _mockEncryptionHelper.Object);

            // Assert
            Assert.Null(response.ErrorMessage);
            Assert.True(response.IsVerified);
        }

    }
}
