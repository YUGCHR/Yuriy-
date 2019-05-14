using TextSplitLibrary;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting; // using for tests

namespace TextSplit.Tests
{ // from menu Test -> run -> All tests
    // or Test -> Windows -> TestExplorer
    [TestClass]
    public class AnalysisLogicChapterMainTest
    {
        public string[] testOneChapterText;
        public string[] testThreeChapterText;
        private int[] expectedResult;

        public AnalysisLogicChapterMainTest()//тест главного метода класса
        {//сделать массив с голыми номерами, с другим ключевым словом, с разными ключевыми словами
            testOneChapterText = new string[]//тестовый текст для нахождения одной главы (номера глав совпадают)
            {
                "",
                "Chapter 00",
                "",
                " no need to setup return values for a method of interface.",
                "dfkgl df gdgdfkl",
                "",
                "Chapter 00",
                "",
                " no need to setup return values for a method of interface.",
                "dfkgl df gdgdfkl",
                "",
                "Chapter 00",
                "",
                " no need to setup return values for a method of interface.",
                "dfkgl df gdgdfkl",
                "",
                "Chapter 00",
                ""
            };

            testThreeChapterText = new string[]//тестовый текст для нахождения трех глав
                {
                "",
                "<< - -  Chapter 00  - - >>",
                "",
                " no need to setup return values for a method of interface.",
                "dfkgl df gdgdfkl",
                "",
                "<< - -  Chapter 01  - - >>",
                "",
                "dfkgl df gdgdfkl",
                "",
                "<< - -  Chapter 02  - - >>",
                "",
                };            
        }

        [TestMethod] // - marks method as a test
        public void ChapterNameAnalysis_NoChapters()
        {
            // ARRANGE
            IAllBookData book = new AllBookData();
            IMessageService message = Mock.Of<IMessageService>(); // - use this if there is no need to setup return values for a method of interface.
            Mock<IAllBookData> bookDataMock = new Mock<IAllBookData>();
            AnalysisLogicChapterDataArrays adata = new AnalysisLogicChapterDataArrays(book, message);
            bookDataMock.Setup(x => x.GetChapterNameLength(It.IsAny<int>())).Returns(5 /* - you can setup what evere data */);
            bookDataMock.Setup(x => x.GetParagraphTextLength(It.IsAny<int>())).Returns(1);
            bookDataMock.Setup(x => x.GetParagraphText(It.IsAny<int>(), It.IsAny<int>()))
                .Returns("use this if there is no need to setup return values for a method of interface.");

            // create instance of a class we want to test
            var target = new AnalysisLogicChapter(bookDataMock.Object, message, adata);

            // ACT
            int result = target.ChapterNameAnalysis(0);//0 - Eng, 1 - Rus

            // ASSERT
            Assert.AreEqual(0, result, "there must be no chapters.");
        }

        [DataTestMethod] // - marks method as a test
        [DataRow("Chapter 00")]
        //[DataRow("Chapter #00")]
        //[DataRow("<-- Chapter 00 -->")]//варианты написания заголовка главы
        public void ChapterNameAnalysis_ShouldFindChapters(string chapterString)
        {
            // ARRANGE
            IAllBookData book = new AllBookData();
            IMessageService message = Mock.Of<IMessageService>(); // - use this if there is no need to setup return values for a method of interface.
            AnalysisLogicChapterDataArrays adata = new AnalysisLogicChapterDataArrays(book, message);
            Mock<IAllBookData> bookDataMock = new Mock<IAllBookData>();

            var addedText = new string[] {
                "  11  ",
                "",
                chapterString,
                "",
                " no need to setup return values for a method of interface."};

            bookDataMock.Setup(x => x.GetChapterNameLength(It.IsAny<int>())).Returns(5 /* - you can setup what evere data */);
            bookDataMock.Setup(x => x.GetParagraphTextLength(It.IsAny<int>())).Returns(addedText.Length);

            for (var i = 0; i < addedText.Length; i++)
            {
                bookDataMock.Setup(x => x.GetParagraphText(i, It.IsAny<int>())).Returns(addedText[i]);
            }
            // create instance of a class we want to test
            var target = new AnalysisLogicChapter(bookDataMock.Object, message, adata);

            // ACT
            int result = target.ChapterNameAnalysis(0);//0 - Eng, 1 - Rus

            // ASSERT
            Assert.AreEqual(1, result, "There is must be 1 chapter.");
        }


        [TestMethod]
        public void ChapterNameAnalysis_OneChapters()
        {
            // ARRANGE
            int result = assumptionData(testOneChapterText);            
            // ASSERT
            Assert.AreEqual(1, result, "There must 1 chapter.");
        }

        [TestMethod]
        public void ChapterNameAnalysis_ThreeChapters()
        {
            // ARRANGE
            int result = assumptionData(testThreeChapterText);            
            // ASSERT
            Assert.AreEqual(3, result, "There must 3 chapter.");
        }

        public int assumptionData(string[] testText)
        {   // ARRANGE
            IAllBookData book = new AllBookData();
            IMessageService message = Mock.Of<IMessageService>(); // - use this if there is no need to setup return values for a method of interface.
            AnalysisLogicChapterDataArrays adata = new AnalysisLogicChapterDataArrays(book, message);
            Mock<IAllBookData> bookDataMock = new Mock<IAllBookData>();
            int testTextLength = testText.Length;
            bookDataMock.Setup(x => x.GetChapterNameLength(It.IsAny<int>())).Returns(5 /* - you can setup what evere data */);
            bookDataMock.Setup(x => x.GetParagraphTextLength(It.IsAny<int>())).Returns(testTextLength);

            for (var i = 0; i <testTextLength; i++)
            {
                bookDataMock.Setup(x => x.GetParagraphText(i, It.IsAny<int>())).Returns(testText[i]);
            }
            // create instance of a class we want to test
            var target = new AnalysisLogicChapter(bookDataMock.Object, message, adata);
            // ACT
            int result = target.ChapterNameAnalysis(0);//0 - Eng, 1 - Rus
            return result;
        }
    }    
}
