using System.IO;
using TextSplitLibrary;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;

namespace TextSplit.Tests
{   // from menu Test -> run -> All tests
    // or Test -> Windows -> TestExplorer

    [TestClass]
    public class AnalysisLogicChapterIntegrationTest
    {
        [TestMethod] // - marks method as a test
        [DataRow(".//testBooks//testEndlishTexts_03.txt", "a18db99a01a8f7a08085ea28968edd31", (int)WhatNeedDoWithFiles.AnalyseText, 0, "9ea145f9d9a7702f48c64eba6a5e8407")]
        //(int)WhatNeedDoWithFiles.AnalyseText - включить анализ текста - аналог нажатия кнопки Analysis в OpenForm
        //1a228cf53b80ba5e24499b8d83a44df0 - в исходный текст поставлены градусы вместо пробела после разделителей - для контроля переноса пробела
        //e7ef272232c4b704f557db114ac7815f - в исходный текст добавлено много пустых строк в конце
        //a18db99a01a8f7a08085ea28968edd31 - в исходный текст добавлены скобки для всех [Maximilian Andreyevich]
        //i = 0  - this will be desiredTextLanguage for AnalyseTextBook
        //Actual Hash with Paragraph numbers only (without sentences): <02a6c1080c08c87c95cf95005fb701e7>
        //Actual Hash with Paragraph and Sentences numbers): <97509ac7bb342a814e59684113b74997>
        //Actual Hash with Paragraph and Sentences numbers (градусы вместо пробела и точку на место кавычек, а не +1 позицию): da07ef8ec9674dec7cea0bc9af3772a2
        //Actual Hash with deleted last Paragraph number - before empty line: 14759d56a7875bb6f6b5648457d7303c
        //Actual Hash with []: 9ea145f9d9a7702f48c64eba6a5e8407 - должно быть 544 предложения, Excel проверил
        public void TestMain_AnalyseTextBook(string _filePath, string expectedHash, int fileToDo, int desiredTextLanguage, string saveTextFileResult)
        {
            bool truePath = File.Exists(_filePath);
            Assert.IsTrue(truePath, "test file not found");            

            IAllBookData bookData = new AllBookDataArrays();
            IFileManager manager = new FileManager(bookData);

            //IMessageService msgService = Mock.Of<IMessageService>();// - вывод на печать отключить
            IMessageService msgService = new MessageService(manager);// - вывод на печать включить (+ в самом методе включить)           
            IAnalysisLogicDataArrays arrayAnalysis = new AnalysisLogicDataArrays(bookData, msgService);
            IAnalysisLogicCultivation analysisLogic = new AnalysisLogicCultivation(bookData, msgService, arrayAnalysis);
            IAnalysisLogicSentences sentenceAnalyser = new AnalysisLogicSentences(bookData, msgService, analysisLogic, arrayAnalysis);
            IAnalysisLogicParagraph paragraphAnalysis = new AnalysisLogicParagraph(bookData, msgService, analysisLogic, arrayAnalysis);
            IAnalysisLogicChapter chapterAnalyser = new AnalysisLogicChapter(bookData, msgService, analysisLogic, arrayAnalysis);            
            IAllBookAnalysis bookAnalysis = new AllBookAnalysis(bookData, msgService, analysisLogic, chapterAnalyser, paragraphAnalysis, sentenceAnalyser);

            bookData.SetFileToDo(fileToDo, desiredTextLanguage);//создание нужной инструкции ToDo
            bookData.SetFilePath(_filePath, desiredTextLanguage);
            string fileContent = manager.GetContent(desiredTextLanguage);
            CheckMd5Hash(fileContent, expectedHash);//проверка неизменности тестового текстового файла
            bookData.SetFileContent(fileContent, desiredTextLanguage);            

            var result = bookAnalysis.AnalyseTextBook();
            Assert.AreEqual(saveTextFileResult, result);
        }

        [TestMethod] // - marks method as a test
        [DataRow(".//testBooks//testEndlishTexts_03.txt", "e97f0953ea502433e5bc53607a89f6e8", 0, "¤¤¤¤¤051¤¤¤-Chapter-")]
        public void TestMain_AnalysisLogicChapter(string _filePath, string expectedHash, int desiredTextLanguage, string lastFoundChapterNumberInMarkFormat)
        {
            bool truePath = File.Exists(_filePath);
            Assert.IsTrue(truePath, "test file not found");

            IAllBookData bookData = new AllBookDataArrays();
            IFileManager manager = new FileManager(bookData);

            //IMessageService msgService = Mock.Of<IMessageService>();// - вывод на печать отключить
            IMessageService msgService = new MessageService(manager);// - вывод на печать включить (+ в самом методе включить)            
            IAnalysisLogicDataArrays arrayAnalysis = new AnalysisLogicDataArrays(bookData, msgService);
            IAnalysisLogicCultivation analysisLogic = new AnalysisLogicCultivation(bookData, msgService, arrayAnalysis);
            IAnalysisLogicSentences sentenceAnalyser = new AnalysisLogicSentences(bookData, msgService, analysisLogic, arrayAnalysis);
            IAnalysisLogicParagraph paragraphAnalyser = new AnalysisLogicParagraph(bookData, msgService, analysisLogic, arrayAnalysis);
            IAnalysisLogicChapter chapterAnalyser = new AnalysisLogicChapter(bookData, msgService, analysisLogic, arrayAnalysis);

            bookData.SetFilePath(_filePath, desiredTextLanguage);
            string fileContent = manager.GetContent(desiredTextLanguage);

            CheckMd5Hash(fileContent, expectedHash);//проверка неизменности тестового текстового файла
            
            bookData.SetFileContent(fileContent, desiredTextLanguage);
            int portionBookTextResult = paragraphAnalyser.PortionBookTextOnParagraphs(desiredTextLanguage);
            int normalizeEmptyParagraphsResult = paragraphAnalyser.normalizeEmptyParagraphs(desiredTextLanguage);

            var result = chapterAnalyser.ChapterNameAnalysis(desiredTextLanguage);
            Assert.AreEqual(lastFoundChapterNumberInMarkFormat, result);
        }

        [TestMethod] // - marks method as a test
        [DataRow(".//testBooks//testEndlishTexts_03.txt", "e97f0953ea502433e5bc53607a89f6e8", 0, "¤¤¤¤¤051¤¤¤-Chapter-")]
        public void TestUnit_TextBookDivideOnChapter(string _filePath, string expectedHash, int desiredTextLanguage, string lastFoundChapterNumberInMarkFormat)
        {            
            bool truePath = File.Exists(_filePath);
            Assert.IsTrue(truePath, "test file not found");

            IAllBookData bookData = new AllBookDataArrays();
            IFileManager manager = new FileManager(bookData);

            //IMessageService msgService = Mock.Of<IMessageService>();// - вывод на печать отключить
            IMessageService msgService = new MessageService(manager);// - вывод на печать включить (+ в самом методе включить)            
            IAnalysisLogicDataArrays arrayAnalysis = new AnalysisLogicDataArrays(bookData, msgService);
            IAnalysisLogicCultivation analysisLogic = new AnalysisLogicCultivation(bookData, msgService, arrayAnalysis);
            IAnalysisLogicSentences sentenceAnalyser = new AnalysisLogicSentences(bookData, msgService, analysisLogic, arrayAnalysis);
            IAnalysisLogicParagraph paragraphAnalyser = new AnalysisLogicParagraph(bookData, msgService, analysisLogic, arrayAnalysis);
            AnalysisLogicChapter chapterAnalyser = new AnalysisLogicChapter(bookData, msgService, analysisLogic, arrayAnalysis);//используется класс, а не интерфейс - часть методов внутренние

            bookData.SetFilePath(_filePath, desiredTextLanguage);
            string fileContent = manager.GetContent(desiredTextLanguage);

            CheckMd5Hash(fileContent, expectedHash);//проверка неизменности тестового текстового файла

            bookData.SetFileContent(fileContent, desiredTextLanguage);
            int portionBookTextResult = paragraphAnalyser.PortionBookTextOnParagraphs(desiredTextLanguage);
            int normalizeEmptyParagraphsResult = paragraphAnalyser.normalizeEmptyParagraphs(desiredTextLanguage);
            int paragraphTextLength = bookData.GetParagraphTextLength(desiredTextLanguage);
            
            int[] chapterNameIsDigitsOnly = new int[paragraphTextLength];

            for (int i = 0; i < paragraphTextLength; i++)
            {
                string currentParagraph = bookData.GetParagraphText(i, desiredTextLanguage);                
                chapterAnalyser.FirstTenGroupsChecked(currentParagraph, chapterNameIsDigitsOnly, i, desiredTextLanguage);
            }
            
            int increasedChapterNumbers = chapterAnalyser.IsChapterNumbersIncreased(chapterNameIsDigitsOnly, desiredTextLanguage);
            
            string keyWordFounfForm = chapterAnalyser.KeyWordFormFound(desiredTextLanguage);

            var result = chapterAnalyser.TextBookDivideOnChapter(chapterNameIsDigitsOnly, increasedChapterNumbers, keyWordFounfForm, desiredTextLanguage);
            Assert.AreEqual(lastFoundChapterNumberInMarkFormat, result);
        }

        public static void CheckMd5Hash(string fileContent, string expectedHash)
        {
            IAllBookData bookData = new AllBookDataArrays();
            IFileManager manager = new FileManager(bookData);

            string pasHash = manager.GetMd5Hash(fileContent);

            Trace.WriteLine("Hash = " + pasHash);//печать хэша файла для изменения на правильный после корректировки файла
            bool trueHash = pasHash == expectedHash;
            Assert.IsTrue(trueHash, "test file has been changed");
        }
    }
}
