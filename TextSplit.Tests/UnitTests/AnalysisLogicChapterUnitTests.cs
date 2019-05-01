using System;
using TextSplit;
using TextSplitLibrary;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting; // using for tests

namespace TextSplit.Tests
{ // from menu Test -> run -> All tests
    // or Test -> Windows -> TestExplorer
    [TestClass]
    public class AnalysisLogicChapterUnitTests
    {        
        public string[] test10WordsText;
        private string[] foundWordsOfParagraph;
        private int test10WordsTextLengh;
        private int[] expectedResult;

        public AnalysisLogicChapterUnitTests()//тесты всех методов класса
        {//сделать массив с голыми номерами, с другим ключевым словом, с разными ключевыми словами

            test10WordsText = new string[]
            {
                "",                                        // 0 слов (перехватить -1 на выдаче)
                "Chapter 00",                              // 1 слово - один пробел обрабатывается неверно
                "Chapter  00",                             // 2 слова 
                "$$$ $$$ Chapter 00 ",                     // 3 слова
                "$$$  $$$  Chapter  00 Chapter",           // 4 слова - две группы символов обрабатываются как одна - но наверное, так и задумано
                "$$$ Found Following Chapter 00 ",         // 5 слов - еще понять, обрабатываются ли символы после номера, и что там с пробелом после номера (с ним все плохо)
            };//добавить длинный пример - больше 10-ти слов, чтобы выйти за пределы рабочего массива
            test10WordsTextLengh = test10WordsText.Length;
            expectedResult = new int[test10WordsTextLengh];
            foundWordsOfParagraph = new string[10];//временное хранение найденных первых десяти слов абзаца
        }

        [TestMethod]
        public void WordsOfParagraphSearch_10WordsArray()
        {//метод выделяет из строки (абзаца текста) первые десять (или больше - по размерности передаваемого массива) слов или чисел (и, возможно, перечисляет все разделители)
            // ARRANGE
            int actualResult = 0;
            expectedResult = new int[]
                {
                -1,
                1,
                1,//должно быть 2 слова!
                3,
                3,//должно быть 4 слова!
                5
                };
            int expectedResultAssertLengh = expectedResult.Length;
            if (expectedResultAssertLengh != test10WordsTextLengh)
            {
                Assert.Fail("The test arrays are not equal");
            }
            //IMessageService msgService = Mock.Of<IMessageService>();// - вывод на печать отключить

            IAllBookData book = new AllBookData();
            IFileManager manager = new FileManager(book);
            IMessageService msgService = new MessageService(manager);// - вывод на печать включить (+ в самом методе включить)

            Mock<IAllBookData> bookDataMock = new Mock<IAllBookData>();

            var target = new AnalysisLogicChapter(bookDataMock.Object, msgService);
            for (int i = 0; i < test10WordsTextLengh; i++)
            {
                actualResult = target.WordsOfParagraphSearch(test10WordsText[i], foundWordsOfParagraph);
                
                Assert.AreEqual(expectedResult[i], actualResult, "There are must be " + expectedResult[i].ToString() + " words.");
            }
        }

        [TestMethod]
        public void RemoveMoreThenOneBlank_10WordsArray()
        {//метод выделяет из строки (абзаца текста) первые десять (или больше - по размерности передаваемого массива) слов или чисел (и, возможно, перечисляет все разделители)
         // ARRANGE
            int actualResult = 0;
            expectedResult = new int[]
                {
                0,
                0,
                -1,
                0,
                -1,
                0
                };
            int expectedResultAssertLengh = expectedResult.Length;
            if(expectedResultAssertLengh != test10WordsTextLengh)
            {
                Assert.Fail("The test arrays are not equal");
            }
            IMessageService msgService = Mock.Of<IMessageService>();// - вывод на печать отключить
            //IAllBookData book = new AllBookData();
            //IFileManager manager = new FileManager(book);
            //IMessageService msgService = new MessageService(manager);// - вывод на печать включить (+ в самом методе включить)

            Mock<IAllBookData> bookDataMock = new Mock<IAllBookData>();

            var target = new AnalysisLogicChapter(bookDataMock.Object, msgService);
            for (int i = 0; i < test10WordsTextLengh; i++)
            {
                string result = target.RemoveMoreThenOneBlank(test10WordsText[i]);
                if (result == test10WordsText[i])
                {
                    actualResult = 0;//если строки одинаковые (разница между ними нулевая)
                }
                else
                {
                    actualResult = -1;//если были двойные пробелы и строки отличаются

                }
                Assert.AreEqual(expectedResult[i], actualResult, "There are must be " + expectedResult[i].ToString() + " result.");
            }
        }
    }
}
