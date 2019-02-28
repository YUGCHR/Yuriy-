using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSplitLibrary
{
    public interface ILogFileMessages
    {
        //string[] LogFileMessagesTexts { get; set; }
        string GetLogFileMessages(int i);        
    }

    public class LogFileMessages : ILogFileMessages
    {
        public string[] LogFileMessagesTexts;

        public LogFileMessages()
        {
            int OpenFormTextBoxImplementationMessagesCount = Enum.GetNames(typeof(OpenFormTextBoxImplementationMessages)).Length;
            LogFileMessagesTexts = new string[OpenFormTextBoxImplementationMessagesCount];

            LogFileMessagesTexts[(int)OpenFormTextBoxImplementationMessages.EnglishFilePathSelected] = "Following path of English file has been selected - ";
            LogFileMessagesTexts[(int)OpenFormTextBoxImplementationMessages.RussianFilePathSelected] = "Following path of Russian file has been selected - ";
            LogFileMessagesTexts[(int)OpenFormTextBoxImplementationMessages.ResultFilePathSelected] = "Following path of result file has been selected - ";
            LogFileMessagesTexts[(int)OpenFormTextBoxImplementationMessages.ResultFileCreated] = "Following result file has been created sucessfully - ";
        }

        public string GetLogFileMessages(int i)
        {
            return LogFileMessagesTexts[i];
        }
    }
}

