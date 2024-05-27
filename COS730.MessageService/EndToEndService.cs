using COS730.Dapper;
using COS730.Helpers.Interfaces;
using COS730.MessageService.Interfaces;
using COS730.Models.DBModels;
using COS730.Models.Requests;
using COS730.Models.Responses;
using Microsoft.Extensions.Logging;

namespace COS730.MessageService
{
    public class EndToEndService : MainService
    {
        private readonly DapperConnection _connection;
        private readonly INLPService _nlpService;

        public EndToEndService(DapperConnection connection, ILogger logger, INLPService nlpService) : base(connection, logger)
        {
            _connection = connection;
            _nlpService = nlpService;
        }

        public (Message response, string message) SendMessage(MessageRequest request, IEncryptionHelper encryptionHelper)
        {
            try
            {
                var user = 
                    (from u in DBContext.User
                     where u.Email == request.RecipientEmail
                     select u
                     ).SingleOrDefault();


                var translatedMessage = _nlpService.TranslateMessage(request.Message!, user!.PreferedLanguage!);

                var (EncryptedMessage, EncryptedAesKey, IV) = encryptionHelper.EncryptMessage(translatedMessage);

                var message = new Message
                {
                    SenderEmail = request.SenderEmail,
                    RecipientEmail = request.RecipientEmail,
                    MessageData = EncryptedMessage,
                    MessageKey = EncryptedAesKey,
                    MessageIV = IV,
                };

                DBContext.Message!.Add(message);

                DBContext.SaveChanges();

                return (message, "Message successfully sent!");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ChatHistoryResponse> GetChatHistory(ChatHistoryRequest request, IEncryptionHelper encryptionHelper)
        {
            try
            {
                var chats = 
                    (from m in DBContext.Message
                     where m.SenderEmail == request.SenderEmail && m.RecipientEmail == request.RecipientEmail
                     select new ChatHistoryResponse
                     {
                         Id = m.Id,
                         SenderEmail = m.SenderEmail,
                         RecipientEmail = m.RecipientEmail,
                         Message = encryptionHelper.DecryptMessage(m.MessageData!, m.MessageKey!, m.MessageIV!)
                     }).ToList();

                return chats;
                
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}