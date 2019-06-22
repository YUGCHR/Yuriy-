using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Reflection;
using TextSplitLibrary;

namespace TextSplitLibrary
{
	
	// 
	// IAnalysisDataConstant<int> test1 = new AnalysisDataConstant<int>();
	// Console.Write(test1.ReturnType);
	
	// IAnalysisDataConstant<string> test2 = new AnalysisDataConstant<string>();
	// Console.Write(test2.ReturnType);
	
	
    public interface IAnalysisDataConstant<T>
    {
        int TestTotalDigitsQuantity(string totalDigitsQuantity);
		
		T[] ReturnType;
	}

    //AnalysisDataConstantUnitTests

    public class AnalysisDataConstant<T> : IAnalysisDataConstant<T>
    {

        //private readonly IAllBookData _bookData;
        //private readonly IMessageService _msgService;

        public T[] ReturnType { get; private set; }
        public string WordToGetValue { get; private set; }
        public int ConstantLength { get; private set; }
		
        public AnalysisDataConstant()
        {	
			if (typeof(T) == typeof(int)) {
				
				ReturnType = new string[] { ".", "…", "!", "?", ";" };
				ConstantLength = SentenceSeparators.ReturnType.Length;
				WordToGetValue = "SentenceSeparators";
			}
			
			if (typeof(T) == typeof(string)) {
				
				ReturnType = new int[] { 1, 3, 5, 7 };
				ConstantLength = totalDigitsQuantity.ReturnType.Length;
				WordToGetValue = "totalDigitsQuantity";
			}
			
			throw new NotImplementedException("this T is not supported.");
		}
		
		public int TestTotalDigitsQuantity(string allDigitString)
        {
            int allDigitStringLength = allDigitString.Length;            
            int allresult = 0;

            for(int i = 0; i < allDigitStringLength; i++)
            {
                string tryChar = allDigitString[i].ToString();
                int charNumber;
                bool result = Int32.TryParse(tryChar, out charNumber);
                allresult += charNumber * -1;
            }

            return allresult;
        }

    }
}





