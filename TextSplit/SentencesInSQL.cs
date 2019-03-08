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
        void OpenConnection();
        int ExecuteWriter();
        void ExecuteReader();
        void ClearAllTables();
        void CloseConnection();
        int InsertRecordInTable(string dataBaseTableName, int[] dataBaseTableToDo, int id, int id_Language, int chapter, string chapter_name);//Insert Record in Table Chapters
        int InsertRecordInTable(string dataBaseTableName, int[] dataBaseTableToDo, int id, int id_Language, int id_Chapter, int paragraph, string paragraph_name);//Insert Record in Table Paragraphs
        int InsertRecordInTable(string dataBaseTableName, int[] dataBaseTableToDo, int id, int id_Language, int id_Chapter, int id_Paragraph, int sentence, string sentence_name);//Insert Record in Table Sentences
    }

    public class DataAccessor : IDataAccessor//abstract
    {
        private readonly IMessageService _messageService;

        private const string connectionStringSource = "Data Source=.;";
        private const string connectionStringDataBase = "Initial Catalog = TextSplitSentences;";
        private const string connectionStringSecurity = "Integrated Security=true";
        private const string connectionString = connectionStringSource + connectionStringDataBase + connectionStringSecurity;
        private SqlConnection connect = null;

        private string[] dataBaseTableNames;
        private int[] dataBaseTableToDo;
        private int dataBaseTableQuantuty;

        public DataAccessor(IMessageService service)
        {
            _messageService = service;
            dataBaseTableNames = new string[] { "Languages", "Chapters", "Paragraphs", "Sentences" };
            dataBaseTableQuantuty = dataBaseTableNames.Length;
            dataBaseTableToDo = new int[dataBaseTableQuantuty];
            //enum WhatNeedDoWithTables => PassThrough = 0, ReadRecord = 1, ReadAllRecord = 2, Reserved3 = 3, InsertRecord = 4, DeleteRecord = 5, ClearTable = 7, ContinueProcessing = 8, StopProcessing = 9


            connect = new SqlConnection(connectionString);
            //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " SqlConnection(connectionString) CREATED ==> " + connect.ToString(), CurrentClassName, 3);
        }

        public void OpenConnection()
        {            
            connect.Open();//need to add diagnostics in return
        }

        public void CloseConnection()
        {
            connect.Close();
        }

        public void ClearTable(int dataBaseTableName)//или получать весь dataBaseTableToDo?
        {
            //если 0 - сказать, что запрещено стирать
        }

        public void ClearAllTables()
        {            
            string sqlExpressionCommand = "DELETE FROM ";
            string sqlExpressionTable;
            string sqlExpression;

            for (int i = dataBaseTableQuantuty-1; i > 0; i--)//Table 0 (Languages) does not need to clear
            {
                sqlExpressionTable = dataBaseTableNames[i];
                sqlExpression = sqlExpressionCommand + sqlExpressionTable;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " sqlExpression ==> " + sqlExpression.ToString(), CurrentClassName, 3);
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    int iSqlCommandReturn = command.ExecuteNonQuery();
                    //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " ClearAllTables iSqlCommandReturn ==> " + iSqlCommandReturn.ToString(), CurrentClassName, 3);
                }
            }
        }
        //string dataBaseTableName = dataBaseTableNames[i];
        //0 - Languages - cannot insert records
        //1 - Chapters - Columns - ID, ID_Language, int Chapter, nvchar10 Chapter_name
        //2 - Paragraphs - Columns - ID, ID_Language, ID_Chapter, int Paragraph, nvchar10 Paragraph_name
        //3 - Sentences - Columns - ID, ID_Language, ID_Chapter, ID_Paragraph, int Sentence, ntext Sentence_name

        public int InsertRecordInTable(string dataBaseTableName, int[] dataBaseTableToDo, int id, int id_Language, int chapter, string chapter_name)//Insert Record in Table Chapters
        {
            if (dataBaseTableName == dataBaseTableNames[(int)TablesNamesNumbers.Chapters])
            {
                if (dataBaseTableToDo[(int)TablesNamesNumbers.Chapters] == (int)WhatNeedDoWithTables.InsertRecord)
                {
                    string sql = string.Format("Insert Into Chapters" + "(ID, ID_Language, Chapter, Chapter_name) Values(@ID, @ID_Language, @Chapter, @Chapter_name)");
                    
                    //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), 
                    //    "\r\n id = " + id.ToString() + 
                    //    "\r\n id_Language = " + id_Language.ToString() + 
                    //    "\r\n chapter = " + chapter.ToString() + 
                    //    "\r\n chapter_name = " + chapter_name, CurrentClassName, 3);
                    using (SqlCommand cmd = new SqlCommand(sql, this.connect))
                    {                        
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.Parameters.AddWithValue("@ID_Language", id_Language);
                        cmd.Parameters.AddWithValue("@Chapter", chapter);
                        cmd.Parameters.AddWithValue("@Chapter_name", chapter_name);                        
                        int iSqlCommandReturn = cmd.ExecuteNonQuery();
                        //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " iSqlCommandReturn Records added ==> " + iSqlCommandReturn.ToString(), CurrentClassName, 3);
                    }
                    return (int)ResultDidWithTables.Successfully;
                }
                else return (int)ResultDidWithTables.CannotInsert;
            }
            else return (int)ResultDidWithTables.UsingWrongTableName;
        }

        public int InsertRecordInTable(string dataBaseTableName, int[] dataBaseTableToDo, int id, int id_Language, int id_Chapter, int paragraph, string paragraph_name)//Insert Record in Table Paragraphs
        {
            if (dataBaseTableName == dataBaseTableNames[(int)TablesNamesNumbers.Paragraphs])
            {
                if (dataBaseTableToDo[(int)TablesNamesNumbers.Paragraphs] == (int)WhatNeedDoWithTables.InsertRecord)
                {
                    string sql = string.Format("Insert Into Paragraphs" + "(ID, ID_Language, ID_Chapter, Paragraph, Paragraphs_name) Values(@ID, @ID_Language, @ID_Chapter, @Paragraph, @Paragraphs_name)");
                    
                    //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), 
                    //    "\r\n id = " + id.ToString() + 
                    //    "\r\n id_Language = " + id_Language.ToString() + 
                    //    "\r\n id_Chapter = " + id_Chapter.ToString() +
                    //    "\r\n paragraph = " + paragraph.ToString() +
                    //    "\r\n paragraph_name = " + paragraph_name, CurrentClassName, 3);
                    using (SqlCommand cmd = new SqlCommand(sql, this.connect))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.Parameters.AddWithValue("@ID_Language", id_Language);
                        cmd.Parameters.AddWithValue("@ID_Chapter", id_Chapter);
                        cmd.Parameters.AddWithValue("@Paragraph", paragraph);
                        cmd.Parameters.AddWithValue("@Paragraphs_name", paragraph_name);
                        int iSqlCommandReturn = cmd.ExecuteNonQuery();
                        //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " iSqlCommandReturn Records added ==> " + iSqlCommandReturn.ToString(), CurrentClassName, 3);
                    }
                    return (int)ResultDidWithTables.Successfully;
                }
                else return (int)ResultDidWithTables.CannotInsert;
            }
            else return (int)ResultDidWithTables.UsingWrongTableName;
        }

        public int InsertRecordInTable(string dataBaseTableName, int[] dataBaseTableToDo, int id, int id_Language, int id_Chapter, int id_Paragraph, int sentence, string sentence_name)//Insert Record in Table Sentences
        {
            if (dataBaseTableName == dataBaseTableNames[(int)TablesNamesNumbers.Sentences])
            {
                if (dataBaseTableToDo[(int)TablesNamesNumbers.Sentences] == (int)WhatNeedDoWithTables.InsertRecord)
                {
                    string sql = string.Format("Insert Into Sentences" + "(ID, ID_Language, ID_Chapter, ID_Paragraph, Sentence, Sentence_name) Values(@ID, @ID_Language, @ID_Chapter, @ID_Paragraph, @Sentence, @Sentence_name)");
                  
                    //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(),
                    //    "\r\n id = " + id.ToString() +
                    //    "\r\n id_Language = " + id_Language.ToString() +
                    //    "\r\n id_Chapter = " + id_Chapter.ToString() +
                    //    "\r\n id_Paragraph = " + id_Paragraph.ToString() +
                    //    "\r\n sentence = " + sentence.ToString() +
                    //    "\r\n sentence_name = " + sentence_name, CurrentClassName, 3);
                    using (SqlCommand cmd = new SqlCommand(sql, this.connect))
                    {                        
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.Parameters.AddWithValue("@ID_Language", id_Language);
                        cmd.Parameters.AddWithValue("@ID_Chapter", id_Chapter);
                        cmd.Parameters.AddWithValue("@ID_Paragraph", id_Paragraph);
                        cmd.Parameters.AddWithValue("@Sentence", sentence);
                        cmd.Parameters.AddWithValue("@Sentence_name", sentence_name);                        
                        int iSqlCommandReturn = cmd.ExecuteNonQuery();
                        //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " iSqlCommandReturn Records added ==> " + iSqlCommandReturn.ToString(), CurrentClassName, 3);
                    }
                    return (int)ResultDidWithTables.Successfully;
                }
                else return (int)ResultDidWithTables.CannotInsert;
            }
            else return (int)ResultDidWithTables.UsingWrongTableName;            
        }


        public int ExecuteWriter()
        {

            return 0;
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

