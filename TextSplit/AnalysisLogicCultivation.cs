using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using TextSplitLibrary;

namespace TextSplit
{
    public interface IAnalysisLogicCultivation
    {
        int GetDesiredTextLanguage();        
        //int FindTextPartNumber(string currentParagraph, string stringMarkBegin, int totalDigitsQuantity);        
        string AddSome00ToIntNumber(string currentNumberToFind, int totalDigitsQuantity);       
    }

    public class AnalysisLogicCultivation : IAnalysisLogicCultivation
    {
        private readonly IAllBookData _bookData;
        private readonly IMessageService _msgService;
        
        readonly private int filesQuantity;
        readonly private int showMessagesLevel;
        readonly private string strCRLF;
        

        public AnalysisLogicCultivation(IAllBookData bookData, IMessageService msgService)
        {
            _bookData = bookData;
            _msgService = msgService;            

            filesQuantity = DConst.FilesQuantity;
            showMessagesLevel = DConst.ShowMessagesLevel;
            strCRLF = DConst.StrCRLF;            
        }
        
        public int GetDesiredTextLanguage()
        {
            int desiredTextLanguage = (int)MethodFindResult.NothingFound;

            for (int i = 0; i < filesQuantity; i++)//пока остается цикл - все же один вместо двух, если вызывать специальный метод 2 раза
            {
                int iDesiredTextLanguage = _bookData.GetFileToDo(i);
                if (iDesiredTextLanguage == (int)WhatNeedDoWithFiles.AnalyseText) desiredTextLanguage = i;
                if (iDesiredTextLanguage == (int)WhatNeedDoWithFiles.AnalyseChapterName) desiredTextLanguage = i;
            }
            return desiredTextLanguage;
        }

        public string AddSome00ToIntNumber(string currentNumberToFind, int totalDigitsQuantity)
        {
            int currentChapterNumberLength = currentNumberToFind.Length;
            int add00Digits = totalDigitsQuantity - currentChapterNumberLength;
            if(add00Digits <= 0)
            {
                return null;
            }
            for(int i = 0; i < add00Digits; i++)
            {
                currentNumberToFind = "0" + currentNumberToFind;
            }            
            return currentNumberToFind;
        }

        public string SaveTextToFile(int desiredTextLanguage)
        {
            string hashFile = "";

            return hashFile;
        }

        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }
    }
}
