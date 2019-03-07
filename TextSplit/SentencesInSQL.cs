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
        void ClearAllTables();
    }

    public class DataAccessor : IDataAccessor//abstract
    {
        private readonly IMessageService _messageService;

        public const string connectionStringSource = "Data Source=.;";
        public const string connectionStringDataBase = "Initial Catalog = TextSplitSentences;";
        public const string connectionStringSecurity = "Integrated Security=true";
        public const string connectionString = connectionStringSource + connectionStringDataBase + connectionStringSecurity;
        public string[] dataBaseTableNames;
        public int dataBaseTableQuantuty;

        public DataAccessor(IMessageService service)
        {
            _messageService = service;
            //InitializeComponent();
            dataBaseTableNames = new string[] { "Languages", "Chapters", "Paragraphs", "Sentences" };
            dataBaseTableQuantuty = dataBaseTableNames.Length;
        }

        public void ClearAllTables()
        {            
            string sqlExpressionCommand = "DELETE FROM ";
            string sqlExpressionTable;
            string sqlExpression;

            for (int i = 1; i < dataBaseTableQuantuty; i++)//Table 0 (Languages) does not need to clear
            {
                sqlExpressionTable = dataBaseTableNames[i];
                sqlExpression = sqlExpressionCommand + sqlExpressionTable;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    int iSqlCommandReturn = command.ExecuteNonQuery();
                    //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " iSqlCommandReturn ==> " + iSqlCommandReturn.ToString(), CurrentClassName, 3);
                }
            }
        }

        public void ExecuteReader()
        {
            
            string sql = "select * from Languages";
            using (var connection = new SqlConnection(connectionString))
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

