using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using TextSplitLibrary;

namespace TextSplit
{
    public interface IAllBookAnalysis
    {
        string AnalyseTextBook();        

        event EventHandler AnalyseInvokeTheMain;
    }

    public class AllBookAnalysis : IAllBookAnalysis
    {
        private readonly IAllBookData _bookData;
        private readonly IMessageService _msgService;
        private readonly IAnalysisLogicCultivation _analysisLogic;
        private readonly IAnalysisLogicChapter _chapterAnalysis;
        private readonly IAnalysisLogicParagraph _paragraphAnalyser;
        private readonly IAnalysisLogicSentences _sentenceAnalyser;

        int GetParagraphTextLength(int desiredTextLanguage) => _bookData.GetParagraphTextLength(desiredTextLanguage);
        string GetParagraphText(int paragraphCount, int desiredTextLanguage) => _bookData.GetParagraphText(paragraphCount, desiredTextLanguage);

        readonly private int showMessagesLevel;
        readonly private string strCRLF;        

        public event EventHandler AnalyseInvokeTheMain;

        public AllBookAnalysis(IAllBookData bookData, IMessageService msgService, IAnalysisLogicCultivation analysisLogic, IAnalysisLogicChapter chapterAnalysis, IAnalysisLogicParagraph paragraphAnalysis, IAnalysisLogicSentences sentenceAnalyser)
        {
            _bookData = bookData;
            _msgService = msgService;
            _analysisLogic = analysisLogic;//общая логика
            _chapterAnalysis = chapterAnalysis;//главы
            _paragraphAnalyser = paragraphAnalysis;//абзацы
            _sentenceAnalyser = sentenceAnalyser;//предложения
            
            showMessagesLevel = DeclarationConstants.ShowMessagesLevel;
            strCRLF = DeclarationConstants.StrCRLF;   
        }

        public string AnalyseTextBook() // типа Main в логике анализа текста
        {
            string lastFoundChapterNumberInMarkFormat = "";
            int chapterCountNumber = 0;
            
            int desiredTextLanguage = _analysisLogic.GetDesiredTextLanguage();//возвращает номер языка, если на нем есть AnalyseText или AnalyseChapterName
            if (desiredTextLanguage == (int)MethodFindResult.NothingFound) return desiredTextLanguage.ToString();//типа, нечего анализировать
            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF + "Start desiredTextLanguage = " + desiredTextLanguage.ToString(), CurrentClassName, showMessagesLevel);

            if (_bookData.GetFileToDo(desiredTextLanguage) == (int)WhatNeedDoWithFiles.AnalyseText)//если первоначальный анализ текста, без подсказки пользователя о названии глав, ищем главы самостоятельно
            {
                int portionBookTextResult = _paragraphAnalyser.PortionBookTextOnParagraphs(desiredTextLanguage);

                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF + "portionBookTextResult = " + portionBookTextResult.ToString(), CurrentClassName, showMessagesLevel);

                int normalizeEmptyParagraphsResult = _paragraphAnalyser.normalizeEmptyParagraphs(desiredTextLanguage);//удаляем лишние пустые строки, оставляя только одну, результат - количество удаленных пустых строк                

                lastFoundChapterNumberInMarkFormat = _chapterAnalysis.ChapterNameAnalysis(desiredTextLanguage);//находим название и номера, расставляем метки глав в тексте

                if (lastFoundChapterNumberInMarkFormat == null)
                {
                    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), lastFoundChapterNumberInMarkFormat + " - Stop here - ChapterNameAnalysis cannon do analysis!", CurrentClassName, 3);
                }

                int enumerateParagraphsCount = _paragraphAnalyser.markAndEnumerateParagraphs(lastFoundChapterNumberInMarkFormat, desiredTextLanguage);//тут раставляем метки и номера абзацев - lastFoundChapterNumberInMarkFormat - не особо нужен
                //затем - предложений

                //тут писать вызов разделения на предложения - посмотреть минимум параметров для вызова - desiredTextLanguage
                int countSentencesNumber = _sentenceAnalyser.DividePagagraphToSentencesAndEnumerate(desiredTextLanguage);

                StringBuilder appendFileContent = new StringBuilder();//тут сохраняем весь текст в файл для контрольной печати - убрать метод в дополнения
                int paragraphTextLength = GetParagraphTextLength(desiredTextLanguage);
                for (int i = 0; i < paragraphTextLength; i++)
                {
                    string currentParagraph = GetParagraphText(i, desiredTextLanguage);
                    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "CurrentParagraph [" + i.ToString() + "] -- > " + currentParagraph, CurrentClassName, showMessagesLevel);
                    appendFileContent = appendFileContent.AppendLine(currentParagraph);                    
                }
                string tracedFileContent = appendFileContent.ToString();
                string tracedFileNameAddition = ".//testBooks//testEndlishTexts_03R.txt";//путь только для тестов, для полного запуска надо брать путь, указанный пользователем
                string hashSavedFile = _msgService.SaveTracedToFile(tracedFileNameAddition, tracedFileContent);
                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "hash of the saved file - " + strCRLF + hashSavedFile, CurrentClassName, 3);

                //и вообще, сделать запись в файл по строкам и там проверять результат - этот же файл потом можно сделать контрольным (посчитать хэш)
                //затем загрузить первую главу (предысловие?) в текстовое окно и показать пользователю

                //следующий блок - в отдельный метод - это потом что-то решить, что делать, если не удалось найти номера глав - сначала найти такой пример
                _bookData.SetFileToDo((int)WhatNeedDoWithFiles.SelectChapterName, desiredTextLanguage);//сообщаем Main, что надо поменять название на кнопке на Select
                    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF +
                        "AnalyseInvokeTheMain with desiredTextLanguage = " + desiredTextLanguage.ToString(), CurrentClassName, showMessagesLevel);
                    if (AnalyseInvokeTheMain != null) AnalyseInvokeTheMain(this, EventArgs.Empty);//пошли узнавать у пользователя, как маркируются главы
                
                return hashSavedFile;
            }
            //if (_bookData.GetFileToDo(desiredTextLanguage) == (int)WhatNeedDoWithFiles.AnalyseChapterName) //анализ названия главы с полученной подсказкой от пользователя
            //{//если получена подсказка от пользователя - текст названия главы, то изучаем полученный выбранный текст
            //    chapterCountNumber = _chapterAnalyser.UserSelectedChapterNameAnalysis(desiredTextLanguage);
            //    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF + "Found Chapters count = " + lastFoundChapterNumberInMarkFormat, CurrentClassName, 3);
            //    return chapterCountNumber;
            //}
            return desiredTextLanguage.ToString();//типа, нечего анализировать
        }        

        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }
    }
}
