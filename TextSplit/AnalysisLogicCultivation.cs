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
        int FindTextPartNumber(string currentParagraph, string stringMarkBegin, int totalDigitsQuantity);        
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
        
        public int FindTextPartNumber(string currentParagraph, string symbolsMarkBegin, int totalDigitsQuantity)
        {
            //найти и выделить номер главы
            int currentPartNumber = -1;//чтобы не спутать с нулевым индексом на выходе, -1 - ничего нет (совсем ничего)            
            int symbolsMarkBeginLength = symbolsMarkBegin.Length;
            bool partNumberFound = Int32.TryParse(currentParagraph.Substring(symbolsMarkBeginLength, totalDigitsQuantity), out currentPartNumber);//вместо 3 взять totalDigitsQuantity для главы
            if (partNumberFound)
            {
                return currentPartNumber;
            }
            else
            {
                //что-то пошло не так, остановиться - System.Diagnostics.Debug.Assert(partNumberFound, "Stop here - partNumberFound did not find!");
                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "STOP HERE - currentPartNumber did not find in currentParagraph!" + strCRLF +
                    "currentParagraph = " + currentParagraph + " -->" + strCRLF +
                    "symbolsMarkBegin --> " + symbolsMarkBegin + strCRLF +                                
                    "totalDigitsQuantity = " + totalDigitsQuantity.ToString(), CurrentClassName, 3);                
                return (int)MethodFindResult.NothingFound;
            }            
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
