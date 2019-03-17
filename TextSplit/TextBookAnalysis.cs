using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using TextSplitLibrary;

namespace TextSplit
{
    public interface ITextBookAnalysis
    {
        int AnalyseTextBook();
    }

    class TextBookAnalysis : ITextBookAnalysis
    {
        private readonly IAllBookData _book;
        private readonly IMessageService _messageService;

        readonly private int filesQuantity;
        readonly private int showMessagesLevel;
        readonly private string strCRLF;

        public TextBookAnalysis(IAllBookData book, IMessageService service)
        {
            _book = book;
            _messageService = service;

            filesQuantity = Declaration.FilesQuantity;
            showMessagesLevel = Declaration.ShowMessagesLevel;
            strCRLF = Declaration.StrCRLF;
        }
        


        public int AnalyseTextBook()
        {
            int desiredTextLanguage = GetDesiredTextLanguage();
            if (desiredTextLanguage < 0) return desiredTextLanguage;            
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF + "Start desiredTextLanguage = " + desiredTextLanguage.ToString(), CurrentClassName, 3);
            string textToAnalyse = _book.GetFileContent(desiredTextLanguage);





            return 0;
        }

        private int GetDesiredTextLanguage()
        {
            int desiredTextLanguage = -1;

            for (int i = 0; i < filesQuantity; i++)
            {
                int iDesiredTextLanguage = _book.GetFileToDo(i);
                if (iDesiredTextLanguage == (int)WhatNeedDoWithFiles.AnalyseText) desiredTextLanguage = i;
            }
            return desiredTextLanguage;
        }


        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }

    }
}
