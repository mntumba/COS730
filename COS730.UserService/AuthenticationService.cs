using COS730.Dapper;
using COS730.Helpers;
using COS730.Models.DBModels;
using COS730.Models.Requests;
using COS730.Models.Responses;
using Microsoft.Extensions.Logging;

namespace COS730.UserService
{
    public class AuthenticationService : BaseService
    {
        public AuthenticationService(DapperConnection connection, ILogger logger) : base(connection, logger)
        {
        }

        public AuthResponse Authenticate(AuthRequest request)
        {
            AuthResponse response = new();

            var user =
                (from u in DBContext.User
                 where u.Email == request.Email
                 select u
                ).SingleOrDefault();

            if (user == null)
            {
                response.ErrorMessage = "We couldn't find an account associated with this email.";
                return response;
            }

            if (!EncryptionHelper.IsValid(request.Password!, user.Password!))
            {
                response.ErrorMessage = "Invalid email or password.";
                return response;
            }

            if (!user.IsVerified)
            {
                response.ErrorMessage = "Account not verified! Please use the code sent to your email to verify your account.";
                return response;
            }

            return new AuthResponse
            {
                Name = user.Name,
                Email = user.Email,
                IsVerified = user.IsVerified,
            };
        }

        public string CreateAccount(AccountRequest request)
        {
            try
            {
                var otp = EncryptionHelper.GenerateOtp();

                var password = EncryptionHelper.EncryptCode(request.Password!);

                DBContext.User!.Add(new User
                {
                    Name = request.Name,
                    Email = request.Email,
                    Password = password,
                    OTP = EncryptionHelper.EncryptCode(otp),
                    IsVerified = false,

                });

                DBContext.SaveChanges();

                EmailHelper.SendEmail(request.Email!, "Verificattion code", otp);

                return "Account successfully created!";
            }
            catch (Exception)
            {
                return "Account could not be created. Please check your details and try again";
            }
        }

        public AuthResponse VerifyAccount(OTPRequest request)
        {
            AuthResponse response = new();

            var user =
                (from u in DBContext.User
                 where u.Email == request.Email
                 select u
                ).SingleOrDefault()!;

            bool isOTPValid = EncryptionHelper.IsValid(request.OTP!, user.OTP!);

            if (isOTPValid)
            {
                user.IsVerified = true;
                DBContext.SaveChanges();
                return response;
            }

            response.ErrorMessage = "Wrong Otp!";
            return response;
        }
    }
}