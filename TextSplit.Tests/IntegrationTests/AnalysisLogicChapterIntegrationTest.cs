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
        private const int desiredTextLanguage = 0; //0 - англ, 1 - русский
        private const string _filePath = ".//testBooks//testEndlishTexts_03.txt";

        [TestMethod] // - marks method as a test
        [DataRow("¤¤¤¤¤Chapter-051-¤¤¤")]
        public void TestMain_AnalysisLogicChapter(string lastFoundChapterNumberInMarkFormat)
        {
            bool truePath = File.Exists(_filePath);
            Assert.IsTrue(truePath, "test file not found");

            var book = new AllBookDataArrays();
            IFileManager manager = new FileManager(book);

            //IMessageService msgService = Mock.Of<IMessageService>();// - вывод на печать отключить
            IMessageService msgService = new MessageService(manager);// - вывод на печать включить (+ в самом методе включить)

            AnalysisLogicChapterDataArrays arrayChapter = new AnalysisLogicChapterDataArrays(book, msgService);
            AnalysisLogicParagraph paragraphAnalyser = new AnalysisLogicParagraph(book, msgService);
            AnalysisLogicChapter chapterAnalyser = new AnalysisLogicChapter(book, msgService, arrayChapter);

            book.SetFilePath(_filePath, desiredTextLanguage);
            string fileContent = manager.GetContent(desiredTextLanguage);

            string pasHash = GetMd5Hash(fileContent);
            Trace.WriteLine("Hash = " + pasHash);
            bool trueHash = pasHash == "e97f0953ea502433e5bc53607a89f6e8";
            Assert.IsTrue(trueHash, "test file has been changed");
            
            book.SetFileContent(fileContent, desiredTextLanguage);
            int portionBookTextResult = paragraphAnalyser.PortionBookTextOnParagraphs(desiredTextLanguage);
            int normalizeEmptyParagraphsResult = paragraphAnalyser.normalizeEmptyParagraphs(desiredTextLanguage);

            var result = chapterAnalyser.ChapterNameAnalysis(desiredTextLanguage);
            Assert.AreEqual(lastFoundChapterNumberInMarkFormat, result);
        }

        public static string GetMd5Hash(string fileContent)
        {
            MD5 md5Hasher = MD5.Create(); //создаем объект класса MD5 - он создается не через new, а вызовом метода Create            
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(fileContent));//преобразуем входную строку в массив байт и вычисляем хэш
            StringBuilder sBuilder = new StringBuilder();//создаем новый Stringbuilder (изменяемую строку) для набора байт
            for (int i = 0; i < data.Length; i++)// Преобразуем каждый байт хэша в шестнадцатеричную строку
            {
                sBuilder.Append(data[i].ToString("x2"));//указывает, что нужно преобразовать элемент в шестнадцатиричную строку длиной в два символа
            }
            return sBuilder.ToString();
        }

        [TestMethod] // - marks method as a test
        [DataRow("¤¤¤¤¤Chapter-051-¤¤¤")]
        public void TestUnit_TextBookDivideOnChapter(string lastFoundChapterNumberInMarkFormat)
        {            
            bool truePath = File.Exists(_filePath);
            Assert.IsTrue(truePath, "test file not found");

            var book = new AllBookDataArrays();
            IFileManager manager = new FileManager(book);

            //IMessageService msgService = Mock.Of<IMessageService>();// - вывод на печать отключить
            IMessageService msgService = new MessageService(manager);// - вывод на печать включить (+ в самом методе включить)

            AnalysisLogicChapterDataArrays arrayChapter = new AnalysisLogicChapterDataArrays(book, msgService);
            AnalysisLogicParagraph paragraphAnalyser = new AnalysisLogicParagraph(book, msgService);
            AnalysisLogicChapter chapterAnalyser = new AnalysisLogicChapter(book, msgService, arrayChapter);

            book.SetFilePath(_filePath, desiredTextLanguage);
            string fileContent = manager.GetContent(desiredTextLanguage);

            string pasHash = GetMd5Hash(fileContent);
            Trace.WriteLine("Hash = " + pasHash);
            bool trueHash = pasHash == "e97f0953ea502433e5bc53607a89f6e8"; //в отдельный метод
            Assert.IsTrue(trueHash, "test file has been changed");            

            book.SetFileContent(fileContent, desiredTextLanguage);
            int portionBookTextResult = paragraphAnalyser.PortionBookTextOnParagraphs(desiredTextLanguage);
            int normalizeEmptyParagraphsResult = paragraphAnalyser.normalizeEmptyParagraphs(desiredTextLanguage);
            int paragraphTextLength = book.GetParagraphTextLength(desiredTextLanguage);
            //Trace.WriteLine("paragraphTextLength = " + paragraphTextLength.ToString());
            int[] chapterNameIsDigitsOnly = new int[paragraphTextLength];

            for (int i = 0; i < paragraphTextLength; i++)
            {
                string currentParagraph = book.GetParagraphText(i, desiredTextLanguage);                
                chapterAnalyser.FirstTenGroupsChecked(currentParagraph, chapterNameIsDigitsOnly, i, desiredTextLanguage);
            }
            //int increasedChapterNumbers = expectedChapterNumberCount;
            int increasedChapterNumbers = chapterAnalyser.IsChapterNumbersIncreased(chapterNameIsDigitsOnly, desiredTextLanguage);
            //string keyWordFounfForm = "Chapter";
            string keyWordFounfForm = chapterAnalyser.KeyWordFormFound(desiredTextLanguage);

            var result = chapterAnalyser.TextBookDivideOnChapter(chapterNameIsDigitsOnly, increasedChapterNumbers, keyWordFounfForm, desiredTextLanguage);
            Assert.AreEqual(lastFoundChapterNumberInMarkFormat, result);
        }
    }
}
