using TextSplitLibrary;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting; // using for tests
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace TextSplit.Tests
{
    [TestClass]
    public class AnalysisDataConstantUnitTests
    {
        public AnalysisDataConstantUnitTests()//тесты всех методов класса
        { }

        [TestMethod]
        [DataRow(7, "totalDigitsQuantity")]        

        public void Test01_Base(int expResult, string Quantity)
        {
            //IAllBookData bookData = new AllBookDataArrays();
            //IFileManager manager = new FileManager(bookData);

            //IMessageService msgService = Mock.Of<IMessageService>();// - вывод на печать отключить
            //IMessageService msgService = new MessageService(manager);// - вывод на печать включить (+ в самом методе включить)

            //Mock<IAllBookData> bookDataMock = new Mock<IAllBookData>();

            //IAnalysisDataConstant constantAnalysis = new AnalysisDataConstant<int[]>();

            //AnalysisDataConstant<int[]> totalDigitsQuantity = new AnalysisDataConstant<int[]>();
            //AnalysisDataConstant<string[]> SentenceSeparators = new AnalysisDataConstant<string[]>();

            IAnalysisDataConstant<int> test1 = new AnalysisDataConstant<int>();

            //msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "expResult" + expResult.ToString(), "CurrentClassName", 3);

            //Trace.WriteLine("Input: " + expResult);

            //var target = new AnalysisDataConstant<int[]>(); 
            int result;

            int[] allresult = test1.TestTotalDigitsQuantity(Quantity);
            if (allresult == null)
            {
                result = 0;
            }
            else
            {
                result = allresult[3];

            }
            Assert.AreEqual(expResult, result);
        }        
    }
}


