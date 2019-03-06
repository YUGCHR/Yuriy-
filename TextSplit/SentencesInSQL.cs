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
            
        }

        public void ExecuteReader()
        {
            
            string sql = "select * from Languages";
            using (var connection = new SqlConnection(connStr))
            {
                
                connection.Open();
                string sqlExpression = "INSERT INTO Languages (ID, Language, Language_name) VALUES (1, 1, 'Russian')";
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                int iSqlCommandReturn = command.ExecuteNonQuery();
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " iSqlCommandReturn ==> " + iSqlCommandReturn.ToString(), CurrentClassName, 3);

                using (var cmd = new SqlCommand(sql, connection))
                {                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //your logic goes here;
                            //example
                            var id = reader.GetInt32(0);// - тут номер колонки таблицы
                            var Language = reader.GetInt32(1);// - тут номер колонки таблицы
                            string Language_name = reader.GetSqlString(2).ToString();
                            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " id - Language - Language_name ==> " + id.ToString() + Language.ToString() + Language_name, CurrentClassName, 3);
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
//
//_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " DataAccessor Started ", CurrentClassName, 3);
//_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " ExecuteReader Started ", CurrentClassName, 3);
//_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " sql ==> " + sql.ToString(), CurrentClassName, 3);
//
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

