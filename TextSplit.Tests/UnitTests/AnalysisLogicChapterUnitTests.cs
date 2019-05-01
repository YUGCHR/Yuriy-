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

        public AnalysisLogicChapterUnitTests()//тесты всех методов класса
        {//сделать массив с голыми номерами, с другим ключевым словом, с разными ключевыми словами

            test10WordsText = new string[]
            {
                "",                                        // 0 слов
                "Chapter 00",                              // 1 слово - один пробел обрабатывается неверно
                "Chapter  00",                             // 2 слова 
                "$$$ $$$ Chapter 00 ",                     // 3 слова
                "$$$  $$$  Chapter  00 Chapter",           // 4 слова - две группы символов обрабатываются как одна - но наверное, так и задумано
                "$$$ Found Following Chapter 00 ",         // 5 слов - еще понять, обрабатываются ли символы после номера, и что там с пробелом после номера (с ним все плохо)
            };
            test10WordsTextLengh = test10WordsText.Length;

            foundWordsOfParagraph = new string[10];//временное хранение найденных первых десяти слов абзаца
        }

        [TestMethod]
        public void WordsOfParagraphSearch_10WordsArray()
        {//метод выделяет из строки (абзаца текста) первые десять (или больше - по размерности передаваемого массива) слов или чисел (и, возможно, перечисляет все разделители)
            // ARRANGE
            
            IMessageService msgService = Mock.Of<IMessageService>();// - вывод на печать отключить
            
            //IAllBookData book = new AllBookData();
            //IFileManager manager = new FileManager(book);
            //IMessageService msgService = new MessageService(manager);// - вывод на печать включить (+ в самом методе включить)

            Mock<IAllBookData> bookDataMock = new Mock<IAllBookData>();

            var target = new AnalysisLogicChapter(bookDataMock.Object, msgService);
            for (int i = 0; i < test10WordsTextLengh; i++)
            {
                int result = target.WordsOfParagraphSearch(test10WordsText[i], foundWordsOfParagraph);
                if (result == -1) result = 0;//если слов не нашли, то их количество равно 0, а не -1
                Assert.AreEqual(i, result, "There are must be " + i.ToString() + " words.");
            }
        }       
    }
}
