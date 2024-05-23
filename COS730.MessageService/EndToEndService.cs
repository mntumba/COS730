using COS730.Dapper;
using COS730.Helpers.Interfaces;
using COS730.Models.DBModels;
using COS730.Models.Requests;
using COS730.Models.Responses;
using Microsoft.Extensions.Logging;

namespace COS730.MessageService
{
    public class EndToEndService : MainService
    {
        private readonly DapperConnection _connection;
        public EndToEndService(DapperConnection connection, ILogger logger) : base(connection, logger)
        {
            _connection = connection;
        }

        public string SendMessage(MessageRequest request, IEncryptionHelper encryptionHelper)
        {
            try
            {
                var user = 
                    (from u in DBContext.User
                     where u.Email == request.RecipientEmail
                     select u
                     ).SingleOrDefault();

                var _nlpService = new NLPService(_connection, this.Logger);

                var translatedMessage = _nlpService.TranslateMessage(request.Message!, user!.PreferedLanguage!);

                var (EncryptedMessage, EncryptedAesKey, IV) = encryptionHelper.EncryptMessage(translatedMessage);

                DBContext.Message!.Add(new Message
                {
                    SenderEmail = request.SenderEmail,
                    RecipientEmail = request.RecipientEmail,
                    MessageData = EncryptedMessage,
                    MessageKey = EncryptedAesKey,
                    MessageIV = IV,
                });

                DBContext.SaveChanges();

                return "Message successfully sent!";
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