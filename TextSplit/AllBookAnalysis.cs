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

        event EventHandler AnalyseInvokeTheMain;
    }

    class AllBookAnalysis : ITextBookAnalysis
    {
        private readonly IAllBookData _bookData;
        private readonly IMessageService _msgService;
        private readonly IAnalysisLogicCultivation _analysisLogic;
        private readonly IAnalysisLogicChapter _chapterLogic;
        private readonly IAnalysisLogicParagraph _paragraphLogic;
        private readonly IAnalysisLogicSentences _sentenceLogic;
        
        readonly private int showMessagesLevel;
        readonly private string strCRLF;        

        public event EventHandler AnalyseInvokeTheMain;

        public AllBookAnalysis(IAllBookData bookData, IMessageService msgService, IAnalysisLogicCultivation analysisLogic, IAnalysisLogicChapter chapterLogic, IAnalysisLogicParagraph paragraphLogic, IAnalysisLogicSentences sentenceLogic)
        {
            _bookData = bookData;
            _msgService = msgService;
            _analysisLogic = analysisLogic;//общая логика
            _chapterLogic = chapterLogic;//главы
            _paragraphLogic = paragraphLogic;//абзацы
            _sentenceLogic = sentenceLogic;//предложения
            
            showMessagesLevel = Declaration.ShowMessagesLevel;
            strCRLF = Declaration.StrCRLF;   
        }

        public int AnalyseTextBook() // типа Main в логике анализа текста
        {
            string lastFoundChapterNumberInMarkFormat = "";
            int chapterCountNumber = 0;
            int desiredTextLanguage = _analysisLogic.GetDesiredTextLanguage();//возвращает номер языка, если на нем есть AnalyseText или AnalyseChapterName
            if (desiredTextLanguage == (int)MethodFindResult.NothingFound) return desiredTextLanguage;//типа, нечего анализировать
            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF + "Start desiredTextLanguage = " + desiredTextLanguage.ToString(), CurrentClassName, showMessagesLevel);

            if (_bookData.GetFileToDo(desiredTextLanguage) == (int)WhatNeedDoWithFiles.AnalyseText)
            {   //если первоначальный анализ текста, то делим текст на абзацы, создем служебный массив с нумерацией пустых строк
                int portionBookTextResult = _paragraphLogic.PortionBookTextOnParagraphs(desiredTextLanguage);
                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF + "portionBookTextResult = " + portionBookTextResult.ToString(), CurrentClassName, 3);
                int normalizeEmptyParagraphsResult = _paragraphLogic.normalizeEmptyParagraphs(desiredTextLanguage);//удаляем лишние пустые строки, оставляя только одну, результат - количество удаленных пустых строк                
                lastFoundChapterNumberInMarkFormat = _chapterLogic.ChapterNameAnalysis(desiredTextLanguage);//находим название и номера, расставляем метки глав в тексте
                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), lastFoundChapterNumberInMarkFormat + " <-> and Stop here ", CurrentClassName, 3);//досюда работает!
                //теперь поставить метки и номера абзацев

                //затем - предложений

                //затем загрузить первую главу (предысловие?) в текстовое окно и показать пользователю

                //следующий блок - в отдельный метод - это потом что-то решить, что делать, если не удалось найти номера глав - сначала найти такой пример
                _bookData.SetFileToDo((int)WhatNeedDoWithFiles.SelectChapterName, desiredTextLanguage);//сообщаем Main, что надо поменять название на кнопке на Select
                    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF +
                        "AnalyseInvokeTheMain with desiredTextLanguage = " + desiredTextLanguage.ToString(), CurrentClassName, showMessagesLevel);
                    if (AnalyseInvokeTheMain != null) AnalyseInvokeTheMain(this, EventArgs.Empty);//пошли узнавать у пользователя, как маркируются главы
                
                return portionBookTextResult;
            }
            if (_bookData.GetFileToDo(desiredTextLanguage) == (int)WhatNeedDoWithFiles.AnalyseChapterName) //анализ названия главы с полученной подсказкой от пользователя
            {//если получена подсказка от пользователя - текст названия главы, то изучаем полученный выбранный текст
                chapterCountNumber = _chapterLogic.UserSelectedChapterNameAnalysis(desiredTextLanguage);
                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF + "Found Chapters count = " + lastFoundChapterNumberInMarkFormat, CurrentClassName, 3);
                return chapterCountNumber;
            }
            return desiredTextLanguage;//типа, нечего анализировать
        }        

        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }
    }
}
