using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.Linq.Mapping;
using System.Configuration;
using System.Reflection;
using TextSplitLibrary;

namespace TextSplit
{
    public interface IDataAccessor
    {
        void ExecuteReader();
    }

    public class DataAccessor : IDataAccessor//abstract
    {
        private readonly IMessageService _messageService;

        public const string connStr = "Data Source=.;Initial Catalog = TextSplitSentences;Integrated Security=true";

        public DataAccessor(IMessageService service)
        {
            _messageService = service;
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " DataAccessor Started ", CurrentClassName, 3);
        }

        public void ExecuteReader()
        {
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " ExecuteReader Started ", CurrentClassName, 3);
            string sql = "select * from Sentences";
            using (var connection = new SqlConnection(connStr))
            {
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " sql ==> " + sql.ToString(), CurrentClassName, 3);
                connection.Open();
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " connection ==> " + connection.ToString(), CurrentClassName, 3);
                using (var cmd = new SqlCommand(sql, connection))
                {
                    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " cmd ==> " + cmd.ToString(), CurrentClassName, 3);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //your logic goes here;
                            //example
                           var id = reader.GetInt32(0);// -тут номер колонки таблицы
                            string text = reader.GetSqlString(1).ToString();
                        }
                    }
                }
            }
        }

        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }
    }    
}

//class SentencesInSQL
//{

//    void SqlConnection()
//    {
//        SqlConnection myConnection = new 
//            SqlConnection("user id=username;" +
//            "password=password;server=serverurl;" +
//            "Trusted_Connection=yes;" +
//            "database=database; " +
//            "connection timeout=30");

//        try
//        {
//            myConnection.Open();
//        }
//        catch (Exception e)
//        {
//            Console.WriteLine(e.ToString());
//        }
//    }
//}
//[Table(Name = "Users")]
//public class User
//{
//    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
//    public int Id { get; set; }
//    [Column(Name = "Name")]
//    public string FirstName { get; set; }
//    [Column]
//    public int Age { get; set; }
//}
//Sentences
//Paragraphs
//Chapters
//Languages
//

