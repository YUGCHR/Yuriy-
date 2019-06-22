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
    public interface IAnalysisDataConstant<T>
    {
        int[] TestTotalDigitsQuantity(string Quantity);

        T[] ReturnType { get; set; }

    }
    
    public class AnalysisDataConstant<T> : IAnalysisDataConstant<T>
    {
        //private readonly IAllBookData _bookData;
        //private readonly IMessageService _msgService;

        public T[] ReturnType { get; set; }
        public string WordToGetValue { get; private set; }
        public int ConstantLength { get; private set; }
        
        public int[] intArr1;

        public AnalysisDataConstant()
        {
            intArr1 = new int[] { 1, 3, 5, 7 };
        }
        
        public int[] TestTotalDigitsQuantity(string Quantity)
        {
            AnalysisDataConstant<int> totalDigitsQuantity = new AnalysisDataConstant<int>();
            totalDigitsQuantity.ReturnType = intArr1;
            totalDigitsQuantity.ConstantLength = 1; // totalDigitsQuantity.ReturnType.Length;
            totalDigitsQuantity.WordToGetValue = "totalDigitsQuantity";
            int[] totalDigits = totalDigitsQuantity.ReturnType;
            int result = totalDigitsQuantity.ConstantLength;
            if(Quantity == totalDigitsQuantity.WordToGetValue)
            {
                return totalDigits;
            }
            return null;
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            //var cs = new ConstantStorage();
            //cs._storage.Add("num", 1);
            //cs._storage.Add("ss", "Stroka");

            //string s = cs.GetConstant<string>("ss");

            //int n = cs.GetConstant<int>("num");

            //Console.WriteLine("{0} {1}", s, n);

            var cs = new ConstantStorage();
            cs._storage.Add("num", 1);
            cs._storage.Add("ss", "Stroka");
            cs._storage.Add("ss1", new string[] { "Stroka1", "Stroka2" });

            string s = cs.GetConstant<string>("ss");

            int n = cs.GetConstant<int>("num");
            string[] sss = cs.GetConstant<string[]>("ss1");

            Console.WriteLine("{0} {1} {2}", s, n, string.Join(",", sss));

        }
    }


    public class ConstantStorage
    {
        public Dictionary<string, object> _storage = new Dictionary<string, object>();

        public T GetConstant<T>(string constantName)
        {
            var o = _storage[constantName];

            return (T)o;
        }
    }
}

//if (typeof(T) == typeof(int))
//{
//    int[] ReturnType = new int[] { 1, 3, 5, 7 };
//    //ConstantLength = SentenceSeparators.ReturnType.Length;
//    //WordToGetValue = "SentenceSeparators";
//    return;
//}

//if (typeof(T) == typeof(string))
//{
//    string[] ReturnType = new string[] { ".", "…", "!", "?", ";" };

//    //ConstantLength = totalDigitsQuantity.ReturnType.Length;
//    //WordToGetValue = "totalDigitsQuantity";
//}

//throw new NotImplementedException("this T is not supported.");



