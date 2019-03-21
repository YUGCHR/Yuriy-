using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using TextSplitLibrary;

namespace TextSplit
{
    public interface IAnalysisLogicSentences
    {
        

        //event EventHandler AnalyseInvokeTheMain;
    }

    class AnalysisLogicSentences : IAnalysisLogicSentences
    {
        private readonly IAllBookData _book;
        private readonly IMessageService _messageService;

        readonly private int filesQuantity;
        readonly private int showMessagesLevel;
        readonly private string strCRLF;

        private string[,] chapterNamesSamples;
        private readonly char[] charsParagraphSeparator;
        private readonly char[] charsSentenceSeparator;

        //public event EventHandler AnalyseInvokeTheMain;

        public AnalysisLogicSentences(IAllBookData book, IMessageService service)
        {
            _book = book;
            _messageService = service;

            filesQuantity = Declaration.FilesQuantity;
            showMessagesLevel = Declaration.ShowMessagesLevel;
            strCRLF = Declaration.StrCRLF;

            charsParagraphSeparator = new char[] { '\r', '\n' };
            charsSentenceSeparator = new char[] { '.', '!', '?' };

            //проверить типовые названия глав (для разных языков свои) - сделать метод универсальным и для частей тоже?
            chapterNamesSamples = new string[,]
            { { "Chapter ", "Paragraph ", "Section ", "Subhead ", "Part " },
                { "Глава ", "Параграф " , "Раздел ", "Подраздел ", "Часть " }, };//а номера глав бывают буквами!
        }

        










        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }
    }
}

