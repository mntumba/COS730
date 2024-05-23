using COS730.Dapper;
using Google.Cloud.Translation.V2;
using Google.Cloud.Speech.V1;
using Microsoft.Extensions.Logging;

namespace COS730.MessageService
{
    public class NLPService : MainService
    {
        private readonly TranslationClient _translationclient;
        private readonly SpeechClient _speechClient;

        public NLPService(DapperConnection connection, ILogger logger) : base(connection, logger)
        {
            _translationclient = TranslationClient.Create();
            _speechClient = SpeechClient.Create();
        }

        public string TranslateMessage(string message, string targetLanguage)
        {
            var response = _translationclient.TranslateText(message, targetLanguage);

            return response.TranslatedText;
        }

        public string ConvertVoiceToText(string audioPath)
        {
            var response = _speechClient.RecognizeAsync(new RecognitionConfig
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.OggOpus,
                SampleRateHertz = 16000,
                LanguageCode = "en"
            }, RecognitionAudio.FromFile(audioPath));

            var transcriptions = response.Result.Results;

            string convertedMessage = String.Empty;

            foreach (var transcription in transcriptions)
            {
                var transcript = transcription.Alternatives.FirstOrDefault();

                convertedMessage += transcript!.Transcript;
            }

            return convertedMessage;
        }
    }
}
