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
        [DataRow(".//testBooks//testEndlishTexts_03.txt", "e97f0953ea502433e5bc53607a89f6e8", (int)WhatNeedDoWithFiles.AnalyseText, 0, 1507)]
        //(int)WhatNeedDoWithFiles.AnalyseText - включить анализ текста - аналог нажатия кнопки Analysis в OpenForm
        //i = 0  - this will be desiredTextLanguage for AnalyseTextBook
        public void TestMain_AnalyseTextBook(string _filePath, string expectedHash, int fileToDo, int desiredTextLanguage, int analyseTextBookResult)
        {
            bool truePath = File.Exists(_filePath);
            Assert.IsTrue(truePath, "test file not found");            

            IAllBookData bookData = new AllBookDataArrays();
            IFileManager manager = new FileManager(bookData);

            //IMessageService msgService = Mock.Of<IMessageService>();// - вывод на печать отключить
            IMessageService msgService = new MessageService(manager);// - вывод на печать включить (+ в самом методе включить)
            IAnalysisLogicCultivation analysisLogic = new AnalysisLogicCultivation(bookData, msgService);
            IAnalysisLogicDataArrays arrayAnalysis = new AnalysisLogicDataArrays(bookData, msgService);
            IAnalysisLogicParagraph paragraphAnalysis = new AnalysisLogicParagraph(bookData, msgService, analysisLogic, arrayAnalysis);
            IAnalysisLogicChapter chapterAnalyser = new AnalysisLogicChapter(bookData, msgService, analysisLogic, arrayAnalysis);            
            IAnalysisLogicSentences sentenceAnalyser = new AnalysisLogicSentences(bookData, msgService, analysisLogic, arrayAnalysis);
            IAllBookAnalysis bookAnalysis = new AllBookAnalysis(bookData, msgService, analysisLogic, chapterAnalyser, paragraphAnalysis, sentenceAnalyser);

            bookData.SetFileToDo(fileToDo, desiredTextLanguage);//создание нужной инструкции ToDo
            bookData.SetFilePath(_filePath, desiredTextLanguage);
            string fileContent = manager.GetContent(desiredTextLanguage);
            GetMd5Hash(fileContent, expectedHash);//проверка неизменности тестового текстового файла
            bookData.SetFileContent(fileContent, desiredTextLanguage);            

            var result = bookAnalysis.AnalyseTextBook();
            Assert.AreEqual(analyseTextBookResult, result);
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
            IAnalysisLogicCultivation analysisLogic = new AnalysisLogicCultivation(bookData, msgService);
            IAnalysisLogicDataArrays arrayAnalysis = new AnalysisLogicDataArrays(bookData, msgService);
            IAnalysisLogicParagraph paragraphAnalyser = new AnalysisLogicParagraph(bookData, msgService, analysisLogic, arrayAnalysis);
            IAnalysisLogicChapter chapterAnalyser = new AnalysisLogicChapter(bookData, msgService, analysisLogic, arrayAnalysis);

            bookData.SetFilePath(_filePath, desiredTextLanguage);
            string fileContent = manager.GetContent(desiredTextLanguage);

            GetMd5Hash(fileContent, expectedHash);//проверка неизменности тестового текстового файла
            
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
            IAnalysisLogicCultivation analysisLogic = new AnalysisLogicCultivation(bookData, msgService);
            IAnalysisLogicDataArrays arrayAnalysis = new AnalysisLogicDataArrays(bookData, msgService);
            IAnalysisLogicParagraph paragraphAnalyser = new AnalysisLogicParagraph(bookData, msgService, analysisLogic, arrayAnalysis);
            AnalysisLogicChapter chapterAnalyser = new AnalysisLogicChapter(bookData, msgService, analysisLogic, arrayAnalysis);//используется класс, а не интерфейс - часть методов внутренние

            bookData.SetFilePath(_filePath, desiredTextLanguage);
            string fileContent = manager.GetContent(desiredTextLanguage);

            GetMd5Hash(fileContent, expectedHash);//проверка неизменности тестового текстового файла

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

        public static void GetMd5Hash(string fileContent, string expectedHash)
        {
            MD5 md5Hasher = MD5.Create(); //создаем объект класса MD5 - он создается не через new, а вызовом метода Create            
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(fileContent));//преобразуем входную строку в массив байт и вычисляем хэш
            StringBuilder sBuilder = new StringBuilder();//создаем новый Stringbuilder (изменяемую строку) для набора байт
            for (int i = 0; i < data.Length; i++)// Преобразуем каждый байт хэша в шестнадцатеричную строку
            {
                sBuilder.Append(data[i].ToString("x2"));//указывает, что нужно преобразовать элемент в шестнадцатиричную строку длиной в два символа
            }
            string pasHash = sBuilder.ToString();
            Trace.WriteLine("Hash = " + pasHash);//печать хэша файла для изменения на правильный после корректировки файла
            bool trueHash = pasHash == expectedHash;
            Assert.IsTrue(trueHash, "test file has been changed");
        }
    }
}
