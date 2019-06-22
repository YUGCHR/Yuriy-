using TextSplitLibrary;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting; // using for tests
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

namespace TextSplit.Tests
{
    [TestClass]
    public class AnalysisLogicChapterUnitTests
    {
        public AnalysisLogicChapterUnitTests()//тесты всех методов класса
        { }

        //[TestMethod]
        //[DataRow("Chapter 00", 2)]
        //[DataRow("-Chapter-00-", 2)]
        //[DataRow("Chapter$00", 2)]
        //[DataRow("-!-!-Chapter 1", 3)]
        //[DataRow("[Chapter-2]", 2)]
        //[DataRow("Chapter$3", 2)]
        //[DataRow("---Chapter---0---", 3)]
        //[DataRow("", -1)]
        //[DataRow("was going to be at precisely 3 p.m.?", 9)]
        //[DataRow("168     The Master and Margarita", 5)]
        //[DataRow("On Friday afternoon Maximilian Andreyevich walked into the office of the housing committee of No. 302B Sadovaya Street in Moscow.", 10)]
        //public void Test01_WordsOfParagraphSearch(string currentParagraph, int numberOfWords)
        //{
        //    IAllBookData bookData = new AllBookDataArrays();
        //    IFileManager manager = new FileManager(bookData);

        //    //IMessageService msgService = Mock.Of<IMessageService>();// - вывод на печать отключить
        //    IMessageService msgService = new MessageService(manager);// - вывод на печать включить (+ в самом методе включить)

        //    Mock<IAllBookData> bookDataMock = new Mock<IAllBookData>();
        //    IAnalysisConstantArrays arrayAnalysis = new AnalysisConstantArrays(bookData, msgService);
        //    IAnalysisLogicCultivation analysisLogic = new AnalysisLogicCultivation(bookData, msgService, arrayAnalysis);
        //    Trace.WriteLine("Input: " + currentParagraph);

        //    var target = new AnalysisLogicChapter(bookDataMock.Object, msgService, analysisLogic, arrayAnalysis);
        //    var words = target.WordsOfParagraphSearch(currentParagraph);
        //    Assert.AreEqual(numberOfWords, words);
        //}

        //[TestMethod]
        //[DataRow("Chapter 00", 0, 1)]
        //[DataRow("-Chapter-11-", 0, 12)]
        //[DataRow("", -1, 0)]
        //[DataRow("was going to Part be at precisely 3 p.m.?", 4, 4)]
        //[DataRow("168  Paragraph   The Master and Margarita", 1, 169)]
        //[DataRow("On Friday Chapter walked into housing committee No. 302B Sadovaya Street in Moscow.", 0, 0)]
        //public void Test02_FirstTenGroupsChecked(string currentParagraph, int countNumber, int wordNumber)
        //{
        //    IAllBookData bookData = new AllBookDataArrays();
        //    IFileManager manager = new FileManager(bookData);

        //    //IMessageService msgService = Mock.Of<IMessageService>();// - вывод на печать отключить
        //    IMessageService msgService = new MessageService(manager);// - вывод на печать включить (+ в самом методе включить)            
        //    IAnalysisConstantArrays arrayAnalysis = new AnalysisConstantArrays(bookData, msgService);
        //    IAnalysisLogicCultivation analysisLogic = new AnalysisLogicCultivation(bookData, msgService, arrayAnalysis);
        //    Mock<IAllBookData> bookDataMock = new Mock<IAllBookData>();

        //    var target = new AnalysisLogicChapter(bookDataMock.Object, msgService, analysisLogic, arrayAnalysis);
        //    int iParagraphNumber = 0;//номер проверяемого абзаца
        //    int desiredTextLanguage = 0;//english language
        //    int[] chapterNameIsDigitsOnly = new int[10];

        //    Trace.WriteLine("Input: " + currentParagraph + "   countNumber = " + countNumber.ToString() + "   wordNumber = " + wordNumber.ToString());

        //    target.FirstTenGroupsChecked(currentParagraph, chapterNameIsDigitsOnly, iParagraphNumber, desiredTextLanguage);

        //    int baseKeyWordForms = arrayAnalysis.GetBaseKeyWordFormsQuantity();
        //    int resultWordCount = -1;
        //    int resultNumber = chapterNameIsDigitsOnly[iParagraphNumber];

        //    Assert.AreEqual(wordNumber, resultNumber, "wordNumber is not Equal");

        //    for (int n = 0; n < arrayAnalysis.GetChapterNamesSamplesLength(desiredTextLanguage); n++)//тут неправильная логика поиска индекса ключевого слова, она годится только для теста из одной строки
        //    {
        //        for (int m = 0; m < baseKeyWordForms; m++)
        //        {
        //            if (arrayAnalysis.GetChapterNamesVersionsCount(m, n) > 0)
        //            {
        //                resultWordCount = n;
        //            }
        //        }
        //    }
        //    Assert.AreEqual(countNumber, resultWordCount, "countNumber is not Equal");
        //}

        //[TestMethod]
        //[DataRow("", -1)]
        //[DataRow("Chapter ", 0)]
        //[DataRow("CHAPTER ", 0)]
        //[DataRow("chapter ", 0)]
        //[DataRow("chapterAte ", 0)]
        //[DataRow("chapteR ", -1)]
        //[DataRow("ChapteR ", -1)]
        //[DataRow("SUBHEAD ", 3)]
        //[DataRow("Part ", 4)]
        //[DataRow("PART ", 4)]
        //[DataRow("Word", -1)]
        //public void Test03_CheckWordOfParagraphCompare(string currentParagraph, int numberOfWords)
        //{
        //    IAllBookData bookData = new AllBookDataArrays();
        //    IFileManager manager = new FileManager(bookData);

        //    //IMessageService msgService = Mock.Of<IMessageService>();// - вывод на печать отключить
        //    IMessageService msgService = new MessageService(manager);// - вывод на печать включить (+ в самом методе включить)

        //    Mock<IAllBookData> bookDataMock = new Mock<IAllBookData>();
        //    IAnalysisConstantArrays arrayAnalysis = new AnalysisConstantArrays(bookData, msgService);
        //    IAnalysisLogicCultivation analysisLogic = new AnalysisLogicCultivation(bookData, msgService, arrayAnalysis);
        //    Trace.WriteLine("Input: " + currentParagraph);

        //    var target = new AnalysisLogicChapter(bookDataMock.Object, msgService, analysisLogic, arrayAnalysis);
        //    int desiredTextLanguage = 0;//english language
        //    int result = target.CheckWordOfParagraphCompare(currentParagraph, 0, desiredTextLanguage);

        //    Assert.AreEqual(numberOfWords, result);
        //}

        //[TestMethod]
        //[DataRow("Chapter   00", "Chapter 00")]
        //[DataRow("-Chapter-00  -", "-Chapter-00 -")]
        //[DataRow("Chapter$00  ", "Chapter$00 ")]
        //[DataRow("- !  - ! -Chapter 1 ", "- ! - ! -Chapter 1 ")]
        //public void Test04_RemoveMoreThenOneBlank(string currentParagraph, string lineResult)
        //{
        //    IAllBookData bookData = new AllBookDataArrays();
        //    IFileManager manager = new FileManager(bookData);
        //    IMessageService msgService = new MessageService(manager);// - вывод на печать включить (+ в самом методе включить)
        //    //IMessageService msgService = Mock.Of<IMessageService>();// - вывод на печать отключить

        //    Mock<IAllBookData> bookDataMock = new Mock<IAllBookData>();
        //    IAnalysisConstantArrays arrayAnalysis = new AnalysisConstantArrays(bookData, msgService);
        //    IAnalysisLogicCultivation analysisLogic = new AnalysisLogicCultivation(bookData, msgService, arrayAnalysis);
        //    Trace.WriteLine("Input: " + currentParagraph);

        //    var target = new AnalysisLogicChapter(bookDataMock.Object, msgService, analysisLogic, arrayAnalysis);
        //    var words = target.RemoveMoreThenOneBlank(currentParagraph);
        //    Assert.AreEqual(lineResult, words);
        //}

        //[TestMethod]
        //[DataRow(22, new int[] { 1, 2, 3, 808, 4, 5, 6, 34, 7, 8, 9, 10, 11, 112, 12, 75, 13, 14, 15, 16, 17, 18, 19, 33, 4, 5, 6, 20, 21, 22 })]
        //public void Test05_IsChaptersNumbersIncreased(int countNumber, int[] allDigitsInParagraphs)
        //{
        //    int paragraphTextLength = allDigitsInParagraphs.Length;
        //    IAllBookData bookData = new AllBookDataArrays();
        //    IFileManager manager = new FileManager(bookData);

        //    //IMessageService msgService = Mock.Of<IMessageService>();// - вывод на печать отключить
        //    IMessageService msgService = new MessageService(manager);// - вывод на печать включить (+ в самом методе включить)            
        //    IAnalysisConstantArrays arrayAnalysis = new AnalysisConstantArrays(bookData, msgService);
        //    IAnalysisLogicCultivation analysisLogic = new AnalysisLogicCultivation(bookData, msgService, arrayAnalysis);
        //    Mock<IAllBookData> bookDataMock = new Mock<IAllBookData>();
        //    bookDataMock.Setup(x => x.GetParagraphTextLength(It.IsAny<int>())).Returns(paragraphTextLength);

        //    Trace.WriteLine("paragraphTextLength = " + paragraphTextLength.ToString());
        //    int desiredTextLanguage = 0;//english language            

        //    var target = new AnalysisLogicChapter(bookDataMock.Object, msgService, analysisLogic, arrayAnalysis);

        //    //Trace.WriteLine("Input: " + currentParagraph + "   countNumber = " + countNumber.ToString() + "   wordNumber = " + wordNumber.ToString());

        //    int[] chapterNameIsDigitsOnly = target.IsChaptersNumbersIncreased(allDigitsInParagraphs);

        //    int lastValueIndex = chapterNameIsDigitsOnly.Length - 1;
        //    int resultCountNumber = chapterNameIsDigitsOnly[lastValueIndex];

        //    Assert.AreEqual(countNumber, resultCountNumber, "countNumber is not Equal");
        //}

        //[TestMethod]
        //[DataRow("Chapter", new int[] { 52, 12, 0, 10, 2 }, new int[] { 11, 12, 3, 10, 2 }, new int[] { 8, 12, 10, 0, 2 })]//{ "Chapter", "Paragraph", "Section", "Subhead", "Part" }, {all UPPER}, {all LOWER}
        //[DataRow("Subhead", new int[] { 1, 12, 3, 52, 22 }, new int[] { 11, 12, 3, 10, 2 }, new int[] { 8, 12, 10, 0, 2 })]
        //[DataRow("PARAGRAPH", new int[] { 0, 32, 3, 12, 10 }, new int[] { 11, 52, 3, 10, 2 }, new int[] { 8, 12, 10, 0, 2 })]
        //[DataRow("Part", new int[] { 51, 1, 10, 7, 52 }, new int[] { 11, 12, 3, 10, 2 }, new int[] { 8, 12, 10, 0, 2 })]
        //public void Test06_KeyWordIndexFound(string expectedChapterName, int[] chapterNamesVersionsCount0, int[] chapterNamesVersionsCount1, int[] chapterNamesVersionsCount2)//метод выбирает наибольшое значение и возвращает ключевое слово в нужной форме
        //{
        //    int chapterNamesVersionsCountTextLength = chapterNamesVersionsCount0.Length;
        //    int formCount = 3;
        //    int[,] chapterNamesVersionsCount = new int[formCount, chapterNamesVersionsCountTextLength];
        //    IAllBookData bookData = new AllBookDataArrays();
        //    IFileManager manager = new FileManager(bookData);
        //    //IMessageService msgService = Mock.Of<IMessageService>();// - вывод на печать отключить
        //    IMessageService msgService = new MessageService(manager);// - вывод на печать включить (+ в самом методе включить)
        //    IAnalysisLogicDataArrays arrayAnalysis = new AnalysisLogicDataArrays(bookData, msgService);
        //    IAnalysisLogicCultivation analysisLogic = new AnalysisLogicCultivation(bookData, msgService, arrayAnalysis);
        //    Mock<IAllBookData> bookDataMock = new Mock<IAllBookData>();
        //    AnalysisLogicChapter chapterAnalyser = new AnalysisLogicChapter(bookData, msgService, analysisLogic, arrayAnalysis);

        //    for (int i = 0; i < chapterNamesVersionsCountTextLength; i++)
        //    {
        //        chapterNamesVersionsCount[0, i] = chapterNamesVersionsCount0[i];
        //        chapterNamesVersionsCount[1, i] = chapterNamesVersionsCount1[i];
        //        chapterNamesVersionsCount[2, i] = chapterNamesVersionsCount2[i];
        //        for (int m = 0; m < formCount; m++)
        //        {
        //            int b = arrayAnalysis.SetChapterNamesVersionsCount(m, i, chapterNamesVersionsCount[m, i]);
        //        }
        //    }
        //    int possibleKeyWordIndex = arrayAnalysis.GetChapterNamesVersionsCount(0, 0);
        //    Trace.WriteLine("possibleKeyWordIndex = " + possibleKeyWordIndex.ToString());//проверка заполнения удаленного массива данными из DataRow
        //    int desiredTextLanguage = 0;//english language            

        //    var target = new AnalysisLogicChapter(bookDataMock.Object, msgService, analysisLogic, arrayAnalysis);

        //    int paragraphTextLength = bookData.GetParagraphTextLength(desiredTextLanguage);
        //    int[] allDigitsInParagraphs = new int[paragraphTextLength];

        //    for (int i = 0; i < paragraphTextLength; i++)
        //    {
        //        string currentParagraph = bookData.GetParagraphText(i, desiredTextLanguage);
        //        chapterAnalyser.FirstTenGroupsChecked(currentParagraph, allDigitsInParagraphs, i, desiredTextLanguage);
        //    }

        //    int[] chapterNameIsDigitsOnly = chapterAnalyser.IsChaptersNumbersIncreased(allDigitsInParagraphs);

        //    string resultChapterName = target.KeyWordFormFound(desiredTextLanguage, chapterNameIsDigitsOnly);

        //    Assert.AreEqual(expectedChapterName, resultChapterName, "ChapterName is not Equal");
        //}

        //[TestMethod]
        //[DataRow("Chapter", new int[] { 52, 12, 0, 10, 33 }, new int[] { 11, 12, 3, 10, 34 }, new int[] { 8, 12, 10, 0, 35 })]//{ "Chapter", "Paragraph", "Section", "Subhead", "Part" }, {all UPPER}, {all LOWER}
        //[DataRow("Subhead", new int[] { 1, 12, 3, 52, 23 }, new int[] { 11, 12, 3, 10, 24 }, new int[] { 8, 12, 10, 0, 25 })]
        //[DataRow("PARAGRAPH", new int[] { 0, 32, 3, 12, 43 }, new int[] { 11, 52, 3, 10, 44 }, new int[] { 8, 12, 10, 0, 45 })]
        //[DataRow("Part", new int[] { 51, 1, 10, 7, 52 }, new int[] { 11, 12, 3, 10, 37 }, new int[] { 8, 12, 10, 0, 47 })]
        //public void Test06Mock_KeyWordIndexFound(string expectedChapterName, int[] chapterNamesVersionsCount0, int[] chapterNamesVersionsCount1, int[] chapterNamesVersionsCount2)//такой же тест, но через Mock
        //{
        //    int chapterNamesVersionsCountTextLength = chapterNamesVersionsCount0.Length;
        //    int formCount = 3;
        //    int[,] chapterNamesVersionsCount = new int[formCount, chapterNamesVersionsCountTextLength];

        //    IAllBookData bookData = new AllBookDataArrays();
        //    IFileManager manager = new FileManager(bookData);
        //    //IMessageService msgService = Mock.Of<IMessageService>();// - вывод на печать отключить
        //    IMessageService msgService = new MessageService(manager);// - вывод на печать включить (+ в самом методе включить)
        //    IAnalysisLogicDataArrays arrayAnalysis = new AnalysisLogicDataArrays(bookData, msgService);
        //    IAnalysisLogicCultivation analysisLogic = new AnalysisLogicCultivation(bookData, msgService, arrayAnalysis);
        //    Mock<IAllBookData> bookDataMock = new Mock<IAllBookData>();
        //    AnalysisLogicChapter chapterAnalyser = new AnalysisLogicChapter(bookData, msgService, analysisLogic, arrayAnalysis);

        //    //for(int language = 0; language <= 1, language++)
        //    //{
        //    //    for (int words = 0; words <5; words++)
        //    //    {
        //    //надо достать массив ключевых слов, сохранить его во временный и потом ниже залить его в мок так же, как массив с индексами, но немного не так (в своих циклах)
        //    //    }
        //    //}

        //    Mock<IAnalysisLogicDataArrays> arrayChapterMock = new Mock<IAnalysisLogicDataArrays>();
        //    arrayChapterMock.Setup(x => x.GetChapterNamesSamplesLength(It.IsAny<int>())).Returns(chapterNamesVersionsCountTextLength);
        //    //нужен массив GetChapterNamesSamples 
        //    arrayChapterMock.Setup(x => x.GetBaseKeyWordFormsQuantity()).Returns(formCount);

        //    for (int i = 0; i < chapterNamesVersionsCountTextLength; i++)
        //    {
        //        chapterNamesVersionsCount[0, i] = chapterNamesVersionsCount0[i];
        //        chapterNamesVersionsCount[1, i] = chapterNamesVersionsCount1[i];
        //        chapterNamesVersionsCount[2, i] = chapterNamesVersionsCount2[i];
        //        for (int m = 0; m < formCount; m++)
        //        {
        //            arrayChapterMock.Setup(x => x.GetChapterNamesVersionsCount(m, i)).Returns(chapterNamesVersionsCount[m, i]);//похоже, массив так не заполнить, какждый раз просто переопределяется переменная
        //        }
        //    }
        //    int possibleKeyWordIndex = arrayChapterMock.Object.GetChapterNamesVersionsCount(2, 4);
        //    Trace.WriteLine("formCount = " + formCount.ToString());//проверка заполнения удаленного массива данными из DataRow
        //    int desiredTextLanguage = 0;//english language

        //    var target = new AnalysisLogicChapter(bookDataMock.Object, msgService, analysisLogic, arrayChapterMock.Object);

        //    int paragraphTextLength = bookData.GetParagraphTextLength(desiredTextLanguage);
        //    int[] allDigitsInParagraphs = new int[paragraphTextLength];

        //    for (int i = 0; i < paragraphTextLength; i++)
        //    {
        //        string currentParagraph = bookData.GetParagraphText(i, desiredTextLanguage);
        //        chapterAnalyser.FirstTenGroupsChecked(currentParagraph, allDigitsInParagraphs, i, desiredTextLanguage);
        //    }

        //    int[] chapterNameIsDigitsOnly = chapterAnalyser.IsChaptersNumbersIncreased(allDigitsInParagraphs);

        //    string resultChapterName = target.KeyWordFormFound(desiredTextLanguage, chapterNameIsDigitsOnly);

        //    Assert.AreEqual(expectedChapterName, resultChapterName, "countNumber is not Equal");
        //}

        [TestMethod]
        [DataRow("0", "000", 3)]
        [DataRow("1", "001", 3)]
        [DataRow("10", "010", 3)]
        [DataRow("777", null, 3)]
        [DataRow("99", "099", 3)]
        [DataRow("9779", null, 3)]
        [DataRow("99", "00099", 5)]
        [DataRow("55555", null, 5)]
        [DataRow("55", "00000055", 8)]
        public void Test07_AddSome00ToIntNumber(string currentNumberToFind, string currentNumberWith00, int totalDigitsQuantity)
        {
            IAllBookData bookData = new AllBookDataArrays();
            IFileManager manager = new FileManager(bookData);

            //IMessageService msgService = Mock.Of<IMessageService>();// - вывод на печать отключить
            IMessageService msgService = new MessageService(manager);// - вывод на печать включить (+ в самом методе включить)

            //Mock<IAllBookData> bookDataMock = new Mock<IAllBookData>();
            IAnalysisConstantArrays arrayAnalysis = new AnalysisConstantArrays(bookData, msgService);
            IAnalysisLogicCultivation analysisLogic = new AnalysisLogicCultivation(bookData, msgService);
            //IAnalysisLogicDataArrays arrayAnalysis = new AnalysisLogicDataArrays(bookData, msgService);
            Trace.WriteLine("currentNumberToFind: " + currentNumberToFind);

            //var target = new AnalysisLogicChapter(bookDataMock.Object, msgService, analysisLogic, arrayAnalysis);

            string result = analysisLogic.AddSome00ToIntNumber(currentNumberToFind, totalDigitsQuantity);// totalDigitsQuantity);

            Assert.AreEqual(currentNumberWith00, result);
        }

        //[TestMethod]
        //[DataRow(2, "\"Yes,\" the maid was saying into the phone, \"Who is it? Baron Maigel? Hullo. Yes! The artiste is at home today. Yes, he'll be happy to see you. Yes, there'll be guests... Tails or a black dinner jacket What? Before midnight. \" After finishing her conversation, the maid put back the receiver and turned to the bartender, \"What can I do for you?\"")]
        //[DataRow(2, "\"May I see the chairman of the housing committee?\"$The economic planner inquired politely, taking off his hat and putting his suitcase on the chair by the doorway.")]
        //[DataRow(5, "True, it would be difficult, very difficult.$But the difficulties had to be overcome, no matter what.$As an experienced man of the world.$Maximilian Andreyevich knew the first thing he had to do to accomplbh this goal was to get registered.$In his late nephew's three-room apartment.")]
        //[DataRow(5, "AT the same time as bookkeeper was in the taxj enroute to his encounter with the suit, man was getting off the  car of the No.$9 train from Kiev.$This passenger was Poplavsky, the uncle of the late Berlioz, who lived in Kiev.$The reason for his trip to Moscow was a telegram.$It said,$\"I have just been cut in half by a streetcar.$Funeral Friday 3 PM.$Come.$Berlioz.\"")]

        //public void Test08_DividePagagraphToSentences(int foundRealSentencesCount, string textParagraph)
        //{
        //    IAllBookData bookData = new AllBookDataArrays();
        //    IFileManager manager = new FileManager(bookData);
        //    Mock<IAllBookData> bookDataMock = new Mock<IAllBookData>();
        //    //IMessageService msgService = Mock.Of<IMessageService>();// - вывод на печать отключить
        //    IMessageService msgService = new MessageService(manager);// - вывод на печать включить (+ в самом методе включить)            
        //    IAnalysisLogicDataArrays arrayAnalysis = new AnalysisLogicDataArrays(bookData, msgService);
        //    IAnalysisLogicCultivation analysisLogic = new AnalysisLogicCultivation(bookData, msgService, arrayAnalysis);
        //    IAnalysisLogicSentences sentenceAnalyser = new AnalysisLogicSentences(bookData, msgService, analysisLogic, arrayAnalysis);
        //    var target = new AnalysisLogicSentences(bookDataMock.Object, msgService, analysisLogic, arrayAnalysis);
        //    List<List<char>> charsAllDelimiters = new List<List<char>> { new List<char>(), new List<char>(), new List<char>() };//временный массив для хранения всех групп разделителей в виде char[] для IndexOfAny           
        //    int sGroupCount = ConstanstListFillCharsDelimiters(charsAllDelimiters);//вернули количество групп разделителей (предложений, кавычки, скобки)

        //    Trace.WriteLine("textParagraph: " + textParagraph);

        //    int desiredTextLanguage = 0;
        //    string sentenceTextMarksWithOtherNumbers = "-Paragraph-3-of-Chapter-3";
        //    string[] paragraphSentences = FSM1(nextParagraph, charsAllDelimiters, sGroupCount);
        //    string[] resultArray = target.DividePagagraphToSentences(sentenceTextMarksWithOtherNumbers, paragraphSentences);
        //    int result = resultArray.Length;
        //    Assert.AreEqual(foundRealSentencesCount, result);
        //}

        public int ConstanstListFillCharsDelimiters(List<List<char>> charsAllDelimiters)
        {
            IAllBookData bookData = new AllBookDataArrays();
            IFileManager manager = new FileManager(bookData);
            IMessageService msgService = new MessageService(manager);
            IAnalysisConstantArrays arrayAnalysis = new AnalysisConstantArrays(bookData, msgService);

            int sGroupCount = arrayAnalysis.GetIntConstant("Groups");
            string[] numbersOfGroupsNames = arrayAnalysis.GetStringArrConstant("Groups");            
            int allDelimitersCount = 0;
            for (int g = 0; g < sGroupCount; g++)
            {                
                string stringCurrentDelimiters = numbersOfGroupsNames[g];
                charsAllDelimiters[g].AddRange(stringCurrentDelimiters.ToCharArray());
                allDelimitersCount = allDelimitersCount + charsAllDelimiters[g].Count;
            }            
            return sGroupCount;
        }

        //[TestMethod]
        //[DataRow(7, "\"Yes, \" the maid was saying into the phone, \"Who is it ? Baron Maigel ? Hullo.Yes!The artiste is at home today.Yes, he'll be happy to see you. Yes, there'll be guests...Tails or a black dinner jacket What ? Before midnight.\" After finishing her conversation, the maid put back the receiver and turned to the bartender, \"What can I do for you ? \"")]
        //[DataRow(20, "\"Yes, \" the maid was saying into the phone, \"Who is it ? Baron Maigel ? «Hullo».Yes!The artiste is at home today.Yes, he'll be happy to see you. Yes, there'll be guests...Tails or a black dinner jacket What ? Before midnight.\" After finishing her conversation, the maid put back the receiver and turned to the bartender, \"What can I do for you ? \"")]

        //public void Test09_CollectTextInQuotes(int quotesCount, string textParagraph)
        //{
        //    IAllBookData bookData = new AllBookDataArrays();
        //    IFileManager manager = new FileManager(bookData);
        //    Mock<IAllBookData> bookDataMock = new Mock<IAllBookData>();
        //    //IMessageService msgService = Mock.Of<IMessageService>();// - вывод на печать отключить
        //    IMessageService msgService = new MessageService(manager);// - вывод на печать включить (+ в самом методе включить)
        //    IAnalysisLogicCultivation analysisLogic = new AnalysisLogicCultivation(bookData, msgService);
        //    IAnalysisLogicDataArrays arrayAnalysis = new AnalysisLogicDataArrays(bookData, msgService);
        //    IAnalysisLogicSentences sentenceAnalyser = new AnalysisLogicSentences(bookData, msgService, analysisLogic, arrayAnalysis);
        //    var target = new AnalysisLogicSentences(bookDataMock.Object, msgService, analysisLogic, arrayAnalysis);
        //    Trace.WriteLine("textParagraph: " + textParagraph);

        //    int charsSentenceSeparatorLength = arrayAnalysis.GetConstantWhatNotLength("Sentence");
        //    string[] charsSentencesSeparators = new string[charsSentenceSeparatorLength];
        //    charsSentencesSeparators = arrayAnalysis.GetConstantWhatNot("Sentence");

        //    Dictionary<int, string> searchResultQuotes = new Dictionary<int, string>(10);//начальная емкость списка - скажем 10, типа 5 пар кавычек
        //    int quotesQuantity = searchResultQuotes.Count;//просто инициализовали allIndexesQuotesLength, ничего личного    
        //    int quotesTypesQuantity = target.SelectAllQuotesOrSeparatorsInText(textParagraph, searchResultQuotes, charsSentencesSeparators);

        //    int result = target.CollectTextInQuotes(textParagraph, searchResultQuotes, quotesTypesQuantity, charsSentencesSeparators, charsSentenceSeparatorLength);

        //    Assert.AreEqual(quotesCount, result);
        //}
    }
}


