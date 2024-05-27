using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COS730.MessageService.Interfaces
{
    public interface INLPService
    {
        string TranslateMessage(string message, string targetLanguage);
        string ConvertVoiceToText(string audioPath);
    }
}
