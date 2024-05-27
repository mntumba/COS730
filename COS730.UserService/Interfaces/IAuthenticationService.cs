using COS730.Helpers.Interfaces;
using COS730.Models.Requests;
using COS730.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COS730.UserService.Interfaces
{
    public interface IAuthenticationService
    {
        AuthResponse Authenticate(AuthRequest request, IEncryptionHelper encryptionHelper);
        string CreateAccount(AccountRequest request, IEncryptionHelper encryptionHelper, IEmailHelper emailHelper);
        AuthResponse VerifyAccount(OTPRequest request, IEncryptionHelper encryptionHelper);
    }
}
