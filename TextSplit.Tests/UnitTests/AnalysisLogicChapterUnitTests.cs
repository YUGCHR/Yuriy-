using System;
using TextSplit;
using TextSplitLibrary;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting; // using for tests
using System.Diagnostics;

namespace TextSplit.Tests
{
    [TestClass]
    public class AnalysisLogicChapterUnitTests
    {
        public AnalysisLogicChapterUnitTests()//тесты всех методов класса
        { }

        [TestMethod] 
        [DataRow("Chapter 00", 2)]
        [DataRow("-Chapter-00-", 2)]
        [DataRow("Chapter$00", 2)]
        [DataRow("-!-!-Chapter 1", 3)]
        [DataRow("[Chapter-2]", 2)]
        [DataRow("Chapter$3", 2)]
        [DataRow("---Chapter---0---", 3)]
        [DataRow("", -1)]
        [DataRow("was going to be at precisely 3 p.m.?", 9)]
        [DataRow("168     The Master and Margarita", 5)]
        [DataRow("On Friday afternoon Maximilian Andreyevich walked into the office of the housing committee of No. 302B Sadovaya Street in Moscow.", 10)]
        public void Test_WordsOfParagraphSearch(string line, int numberOfWords)
        {
            IAllBookData book = new AllBookData();
            IFileManager manager = new FileManager(book);

            //IMessageService msgService = Mock.Of<IMessageService>();// - вывод на печать отключить
            IMessageService message = new MessageService(manager);// - вывод на печать включить (+ в самом методе включить)

            Mock<IAllBookData> bookDataMock = new Mock<IAllBookData>();
            AnalysisLogicChapterDataArrays adata = new AnalysisLogicChapterDataArrays(book, message);
            Trace.WriteLine("Input: " + line);

            var target = new AnalysisLogicChapter(bookDataMock.Object, message, adata);
            var arr = new string[10];
            var words = target.WordsOfParagraphSearch(line);
            Assert.AreEqual(numberOfWords, words);
        }

        [TestMethod]
        [DataRow("Chapter   00", "Chapter 00")]
        [DataRow("-Chapter-00  -", "-Chapter-00 -")]
        [DataRow("Chapter$00  ", "Chapter$00 ")]
        [DataRow("- !  - ! -Chapter 1 ", "- ! - ! -Chapter 1 ")]        
        public void Test_RemoveMoreThenOneBlank(string line, string lineResult)
        {         
            //IMessageService msgService = Mock.Of<IMessageService>();// - вывод на печать отключить

            IAllBookData book = new AllBookData();
            IFileManager manager = new FileManager(book);
            IMessageService message = new MessageService(manager);// - вывод на печать включить (+ в самом методе включить)

            Mock<IAllBookData> bookDataMock = new Mock<IAllBookData>();
            AnalysisLogicChapterDataArrays adata = new AnalysisLogicChapterDataArrays(book, message);
            Trace.WriteLine("Input: " + line);

            var target = new AnalysisLogicChapter(bookDataMock.Object, message, adata);
            var words = target.RemoveMoreThenOneBlank(line);
            Assert.AreEqual(lineResult, words);
        }
    }
}
