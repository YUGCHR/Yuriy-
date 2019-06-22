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

        public event EventHandler AnalyseInvokeTheMain;


        public AllBookAnalysis(IAllBookData bookData, IMessageService msgService, IAnalysisLogicCultivation analysisLogic, IAnalysisLogicChapter chapterAnalysis, IAnalysisLogicParagraph paragraphAnalysis, IAnalysisLogicSentences sentenceAnalyser)
        {
            _bookData = bookData;
            _msgService = msgService;
            _analysisLogic = analysisLogic;//общая логика
            _chapterAnalysis = chapterAnalysis;//главы
            _paragraphAnalyser = paragraphAnalysis;//абзацы
            _sentenceAnalyser = sentenceAnalyser;//предложения
        }

        public string AnalyseTextBook() // типа Main в логике анализа текста
        {
            int desiredTextLanguage = _analysisLogic.GetDesiredTextLanguage();//возвращает номер языка, если на нем есть AnalyseText или AnalyseChapterName
            if (desiredTextLanguage == (int)MethodFindResult.NothingFound)
            {
                return desiredTextLanguage.ToString();//типа, нечего анализировать
            }
            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), DConst.StrCRLF + "Start desiredTextLanguage = " + desiredTextLanguage.ToString(), CurrentClassName, DConst.ShowMessagesLevel);

            if (_bookData.GetFileToDo(desiredTextLanguage) == (int)WhatNeedDoWithFiles.AnalyseText)//если первоначальный анализ текста, без подсказки пользователя о названии глав, ищем главы самостоятельно
            {
                int portionBookTextResult = _paragraphAnalyser.PortionBookTextOnParagraphs(desiredTextLanguage);//возвращает количество строк с текстом
                int paragraphTextLength = _bookData.GetIntContent(desiredTextLanguage, "GetParagraphTextLength");
                int allEmptyParagraphsCount = paragraphTextLength - portionBookTextResult;

                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), DConst.StrCRLF + "Total TEXTs Paragraphs count = " + portionBookTextResult.ToString() + DConst.StrCRLF +
                    "Total ANY paragraphs count = " + paragraphTextLength.ToString() + DConst.StrCRLF +
                    "Total EMPTY paragraphs count = " + allEmptyParagraphsCount.ToString(), CurrentClassName, DConst.ShowMessagesLevel);

                int[] ChapterNumberParagraphsIndexes = _chapterAnalysis.ChapterNameAnalysis(desiredTextLanguage);//находим название и номера, расставляем метки глав в тексте

                int enumerateParagraphsCount = _paragraphAnalyser.MarkAndEnumerateParagraphs(desiredTextLanguage, ChapterNumberParagraphsIndexes);//тут раставляем метки и номера абзацев - lastFoundChapterNumberInMarkFormat - не особо нужен
                
                int countSentencesNumber = _sentenceAnalyser.DividePagagraphToSentencesAndEnumerate(desiredTextLanguage);

                StringBuilder appendFileContent = new StringBuilder();//тут сохраняем весь текст в файл для контрольной печати - убрать метод в дополнения
                //int paragraphTextLength = GetParagraphTextLength(desiredTextLanguage);
                for (int i = 0; i < paragraphTextLength; i++)
                {
                    string currentParagraph = _bookData.GetParagraphText(i, desiredTextLanguage);
                    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "CurrentParagraph [" + i.ToString() + "] -- > " + currentParagraph, CurrentClassName, DConst.ShowMessagesLevel);
                    appendFileContent = appendFileContent.AppendLine(currentParagraph); // was + DConst.StrCRLF);                    
                }
                string tracedFileContent = appendFileContent.ToString();
                string tracedFileNameAddition = ".//testBooks//testEndlishTexts_03R.txt";//путь только для тестов, для полного запуска надо брать путь, указанный пользователем
                string hashSavedFile = _msgService.SaveTracedToFile(tracedFileNameAddition, tracedFileContent);
                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "hash of the saved file - " + DConst.StrCRLF + hashSavedFile, CurrentClassName, 3);// DConst.ShowMessagesLevel);

                //и вообще, сделать запись в файл по строкам и там проверять результат - этот же файл потом можно сделать контрольным (посчитать хэш)
                //затем загрузить первую главу (предысловие?) в текстовое окно и показать пользователю

                //следующий блок - в отдельный метод - это потом что-то решить, что делать, если не удалось найти номера глав - сначала найти такой пример
                _bookData.SetFileToDo((int)WhatNeedDoWithFiles.SelectChapterName, desiredTextLanguage);//сообщаем Main, что надо поменять название на кнопке на Select
                    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), DConst.StrCRLF +
                        "AnalyseInvokeTheMain with desiredTextLanguage = " + desiredTextLanguage.ToString(), CurrentClassName, DConst.ShowMessagesLevel);

                    if (AnalyseInvokeTheMain != null) AnalyseInvokeTheMain(this, EventArgs.Empty);//пошли узнавать у пользователя, как маркируются главы
                
                return hashSavedFile;
            }
            
            return desiredTextLanguage.ToString();//типа, нечего анализировать
        }        

        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }
    }
}
