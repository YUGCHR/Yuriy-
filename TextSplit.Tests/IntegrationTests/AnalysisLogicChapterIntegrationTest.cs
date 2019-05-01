using System;
using System.IO;
using TextSplit;
using TextSplitLibrary;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TextSplit.Tests
{   // from menu Test -> run -> All tests
    // or Test -> Windows -> TestExplorer
    [TestClass]
    public class AnalysisLogicChapterIntegrationTest
    {
        private const int desiredTextLanguage = 0; //0 - англ, 1 - русский
        private const string _filePath = "D://OneDrive//Gonchar//C#2005//TextSplit//TextSplit.Tests//testBooks//testEndlishTexts_03.txt";

        [TestMethod] // - marks method as a test
        public void Test1()
        {
            var t = File.Exists(_filePath);
            Assert.IsTrue(t, "test file not found");

            var bookData = new AllBookData();
            var fileManager = new FileManager(bookData);
            var msgService = Mock.Of<IMessageService>();

            AnalysisLogicParagraph paragraphAnalyser = new AnalysisLogicParagraph(bookData, msgService);
            AnalysisLogicChapter chapterAnalyser = new AnalysisLogicChapter(bookData, msgService);

            bookData.SetFilePath(_filePath, desiredTextLanguage);
            fileManager.GetContent(desiredTextLanguage);
            bookData.SetFileContent(fileManager.GetContent(desiredTextLanguage), desiredTextLanguage);
            paragraphAnalyser.PortionBookTextOnParagraphs(desiredTextLanguage);


            var result = chapterAnalyser.ChapterNameAnalysis(desiredTextLanguage);
            Assert.AreEqual(52, result);
        }
    }
}
