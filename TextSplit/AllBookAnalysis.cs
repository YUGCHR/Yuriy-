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

        //новые методы из _bookData
        int GetIntContent(int desiredTextLanguage, string needOperationName) => _bookData.GetIntContent(desiredTextLanguage, needOperationName);//перегрузка для получения длины двуязычных динамических массивов
        int GetIntContent(string needOperationName, string stringToSet, int indexCount) => _bookData.GetIntContent(needOperationName, stringToSet, indexCount);//перегрузка для записи обычных массивов
        int GetIntContent(int desiredTextLanguage, string needOperationName, string stringToSet, int indexCount) => _bookData.GetIntContent(desiredTextLanguage, needOperationName, stringToSet, indexCount);

        string GetStringContent(string nameOfStringNeed, int indexCount) => _bookData.GetStringContent(nameOfStringNeed, indexCount);
        string GetStringContent(int desiredTextLanguage, string nameOfStringNeed, int indexCount) => _bookData.GetStringContent(desiredTextLanguage, nameOfStringNeed, indexCount);


        //старые методы из _bookData

        int GetParagraphTextLength(int desiredTextLanguage) => _bookData.GetParagraphTextLength(desiredTextLanguage);
        string GetParagraphText(int paragraphCount, int desiredTextLanguage) => _bookData.GetParagraphText(paragraphCount, desiredTextLanguage);            

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
            //string lastFoundChapterNumberInMarkFormat = "";
            //int chapterCountNumber = 0;
            
            int desiredTextLanguage = _analysisLogic.GetDesiredTextLanguage();//возвращает номер языка, если на нем есть AnalyseText или AnalyseChapterName
            if (desiredTextLanguage == (int)MethodFindResult.NothingFound)
            {
                return desiredTextLanguage.ToString();//типа, нечего анализировать
            }
            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), DConst.StrCRLF + "Start desiredTextLanguage = " + desiredTextLanguage.ToString(), CurrentClassName, DConst.ShowMessagesLevel);

            if (_bookData.GetFileToDo(desiredTextLanguage) == (int)WhatNeedDoWithFiles.AnalyseText)//если первоначальный анализ текста, без подсказки пользователя о названии глав, ищем главы самостоятельно
            {
                int portionBookTextResult = _paragraphAnalyser.PortionBookTextOnParagraphs(desiredTextLanguage);//возвращает количество строк с текстом
                int paragraphTextLength = GetIntContent(desiredTextLanguage, "GetParagraphTextLength");
                int allEmptyParagraphsCount = paragraphTextLength - portionBookTextResult;

                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), DConst.StrCRLF + "Total TEXTs Paragraphs count = " + portionBookTextResult.ToString() + DConst.StrCRLF +
                    "Total ANY paragraphs count = " + paragraphTextLength.ToString() + DConst.StrCRLF +
                    "Total EMPTY paragraphs count = " + allEmptyParagraphsCount.ToString(), CurrentClassName, DConst.ShowMessagesLevel);

                //int normalizeEmptyParagraphsResult = _paragraphAnalyser.NormalizeEmptyParagraphs(desiredTextLanguage);//удаляем лишние пустые строки, оставляя только одну, результат - количество удаленных пустых строк                

                int[] ChapterNumberParagraphsIndexes = _chapterAnalysis.ChapterNameAnalysis(desiredTextLanguage);//находим название и номера, расставляем метки глав в тексте

                //string firstParagraph = GetStringContent(desiredTextLanguage, "GetParagraphText", 0);//достаем самый первый абзац
                //int ChapterNumberParagraphsIndexesLength = ChapterNumberParagraphsIndexes.Length;
                //int lastChapterNumberParagraphsIndex = ChapterNumberParagraphsIndexes[ChapterNumberParagraphsIndexesLength - 1];
                //int chapterNumber1ParagraphsIndex = ChapterNumberParagraphsIndexes[0];
                //string lastChapterNameParagraph = GetStringContent(desiredTextLanguage, "GetParagraphText", lastChapterNumberParagraphsIndex);//достаем абзац с последним номером главы
                //string chapterName1Paragraph = GetStringContent(desiredTextLanguage, "GetParagraphText", chapterNumber1ParagraphsIndex);//достаем абзац с последним номером главы
                //_msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Paragraph 0 -- > " + firstParagraph + DConst.StrCRLF +
                //    "Paragraph 1 -- > " + chapterName1Paragraph + DConst.StrCRLF +
                //    "Paragraph with last chapter -- > " + lastChapterNameParagraph, CurrentClassName, 3);






                int enumerateParagraphsCount = _paragraphAnalyser.MarkAndEnumerateParagraphs(desiredTextLanguage, ChapterNumberParagraphsIndexes);//тут раставляем метки и номера абзацев - lastFoundChapterNumberInMarkFormat - не особо нужен
                //затем - предложений

                //тут писать вызов разделения на предложения - посмотреть минимум параметров для вызова - desiredTextLanguage
                int countSentencesNumber = _sentenceAnalyser.DividePagagraphToSentencesAndEnumerate(desiredTextLanguage);



                paragraphTextLength = GetIntContent(desiredTextLanguage, "GetParagraphTextLength");
                for (int i = 0; i < paragraphTextLength; i++)//перебираем все абзацы текста
                {
                    string currentParagraph = GetParagraphText(i, desiredTextLanguage);
                    
                        _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Paragraph[" + i.ToString() + "] -> " + currentParagraph, CurrentClassName, DConst.ShowMessagesLevel);
                    
                }





                StringBuilder appendFileContent = new StringBuilder();//тут сохраняем весь текст в файл для контрольной печати - убрать метод в дополнения
                //int paragraphTextLength = GetParagraphTextLength(desiredTextLanguage);
                for (int i = 0; i < paragraphTextLength; i++)
                {
                    string currentParagraph = GetParagraphText(i, desiredTextLanguage);
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
            //if (_bookData.GetFileToDo(desiredTextLanguage) == (int)WhatNeedDoWithFiles.AnalyseChapterName) //анализ названия главы с полученной подсказкой от пользователя
            //{//если получена подсказка от пользователя - текст названия главы, то изучаем полученный выбранный текст
            //    chapterCountNumber = _chapterAnalyser.UserSelectedChapterNameAnalysis(desiredTextLanguage);
            //    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), DConst.StrCRLF + "Found Chapters count = " + lastFoundChapterNumberInMarkFormat, CurrentClassName, 3);
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
