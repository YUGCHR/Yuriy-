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
        int ClearAllTables();
        void CloseConnection();
        int InsertRecordInTable(string[] dataBaseTableNames, int[] dataBaseTableToDo, int id, int id_Language, int chapter, string chapter_name);//Insert Record in Table Chapters
        int InsertRecordInTable(string[] dataBaseTableName, int[] dataBaseTableToDo, int id, int id_Language, int id_Chapter, int paragraph, string paragraph_name);//Insert Record in Table Paragraphs
        int InsertRecordInTable(string[] dataBaseTableName, int[] dataBaseTableToDo, int id, int id_Language, int id_Chapter, int id_Paragraph, int sentence, string sentence_name);//Insert Record in Table Sentences
    }

    public class DataAccessor : IDataAccessor//abstract
    {
        private readonly IMessageService _messageService;

        private const string connectionStringSource = "Data Source=.;";
        private const string connectionStringDataBase = "Initial Catalog = TextSplitSentences;";
        private const string connectionStringSecurity = "Integrated Security=true";
        private const string connectionString = connectionStringSource + connectionStringDataBase + connectionStringSecurity;

        private SqlConnection connect = null;
        readonly private string strCRLF;
        private int showMessagesLevel;        

        private string[] dataBaseTableNames;
        private int[] dataBaseTableToDo;
        private int dataBaseTableQuantuty;

        public DataAccessor(IMessageService service)
        {
            strCRLF = Declaration.StrCRLF;
            showMessagesLevel = Declaration.ShowMessagesLevel;

            _messageService = service;

            dataBaseTableNames = new string[] { "Languages", "Chapters", "Paragraphs", "Sentences" };
            //0 - Languages - cannot insert records
            //1 - Chapters - Columns - ID, ID_Language, int Chapter, nvchar10 Chapter_name
            //2 - Paragraphs - Columns - ID, ID_Language, ID_Chapter, int Paragraph, nvchar10 Paragraph_name
            //3 - Sentences - Columns - ID, ID_Language, ID_Chapter, ID_Paragraph, int Sentence, ntext Sentence_name

            dataBaseTableQuantuty = dataBaseTableNames.Length;
            dataBaseTableToDo = new int[dataBaseTableQuantuty];
            //enum WhatNeedDoWithTables => PassThrough = 0, ReadRecord = 1, ReadAllRecord = 2, Reserved3 = 3, InsertRecord = 4, DeleteRecord = 5, ClearTable = 7, ContinueProcessing = 8, StopProcessing = 9

            connect = new SqlConnection(connectionString);
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " SqlConnection(connectionString) CREATED ==> " + connect.ToString(), CurrentClassName, showMessagesLevel);
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

        public int ClearAllTables()
        {            
            string sqlExpressionCommand = "DELETE FROM ";
            string sqlExpressionTable;
            string sqlExpression;
            int iSqlCommandReturn = -1;

            for (int i = dataBaseTableQuantuty-1; i > 0; i--)//Table 0 (Languages) does not need to clear
            {
                sqlExpressionTable = dataBaseTableNames[i];// 0 -> Languages, 1 -> Chapters, 2 -> Paragraphs, 3 -> Sentences
                sqlExpression = sqlExpressionCommand + sqlExpressionTable;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " sqlExpression ==> " + sqlExpression.ToString(), CurrentClassName, 3);
                    SqlCommand command = new SqlCommand(sqlExpression, connection);                    
                    try
                    {
                        iSqlCommandReturn = command.ExecuteNonQuery();
                        _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " SqlCommand (Records inserted) returned ==> " + iSqlCommandReturn.ToString(), CurrentClassName, showMessagesLevel);                        
                    }
                    catch (SqlException ex)
                    {
                        _messageService.ShowError(ex.Message);
                        return -1;
                    }                   
                }
            }
            return iSqlCommandReturn;
        }
        //string dataBaseTableName = dataBaseTableNames[i];
        //0 - Languages - cannot insert records
        //1 - Chapters - Columns - ID, ID_Language, int Chapter, nvchar10 Chapter_name
        //2 - Paragraphs - Columns - ID, ID_Language, ID_Chapter, int Paragraph, nvchar10 Paragraph_name
        //3 - Sentences - Columns - ID, ID_Language, ID_Chapter, ID_Paragraph, int Sentence, ntext Sentence_name

        public int InsertRecordInTable(string[] dataBaseTableNames, int[] dataBaseTableToDo, int id, int id_Language, int chapter, string chapter_name)//Overload to insert record in Table Chapters
        {
            for (int i = 1; i < dataBaseTableQuantuty; i++)//0 - Languages - cannot insert records
            {
                if (dataBaseTableToDo[i] == (int)WhatNeedDoWithTables.InsertRecord)//must found at i = 1 -> (int)TablesNamesNumbers.Chapters
                {
                    string dataBaseTableName = dataBaseTableNames[i];
                    string sql = string.Format("Insert Into " + dataBaseTableName + "(ID, ID_Language, Chapter, Chapter_name) Values(@ID, @ID_Language, @Chapter, @Chapter_name)");
                    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(),
                        strCRLF + "dataBaseTableName ==> " + dataBaseTableName +
                        strCRLF + "id = " + id.ToString() +                            
                        strCRLF + "id_Language = " + id_Language.ToString() +                            
                        strCRLF + "chapter = " + chapter.ToString() +                            
                        strCRLF + "chapter_name = " + chapter_name, CurrentClassName, showMessagesLevel);

                    using (SqlCommand cmd = new SqlCommand(sql, this.connect))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.Parameters.AddWithValue("@ID_Language", id_Language);
                        cmd.Parameters.AddWithValue("@Chapter", chapter);
                        cmd.Parameters.AddWithValue("@Chapter_name", chapter_name);
                        try
                        {
                            int iSqlCommandReturn = cmd.ExecuteNonQuery();
                            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " SqlCommand (Records inserted) returned ==> " + iSqlCommandReturn.ToString(), CurrentClassName, showMessagesLevel);
                        }
                        catch (SqlException ex)
                        {
                            _messageService.ShowError(ex.Message);
                        }
                    }
                    return (int)ResultDidWithTables.Successfully;
                }
            }
            return (int)ResultDidWithTables.CannotInsert;// - не нашлась таблица, в которюу надо вставлять записи
        }

        public int InsertRecordInTable(string[] dataBaseTableNames, int[] dataBaseTableToDo, int id, int id_Language, int id_Chapter, int paragraph, string paragraph_name)//Insert Record in Table Paragraphs
        {

            return InsertRecordInTable(dataBaseTableNames, dataBaseTableToDo, id, id_Language, id_Chapter, -1, paragraph, paragraph_name);//overload Insert Record in Table Sentences

            //for (int i = 1; i < dataBaseTableQuantuty; i++)//0 - Languages - cannot insert records
            //{
            //    if (dataBaseTableToDo[i] == (int)WhatNeedDoWithTables.InsertRecord)//must found at i = 2 -> (int)TablesNamesNumbers.Paragraphs
            //    {
            //        string dataBaseTableName = dataBaseTableNames[i];
            //        string sql = string.Format("Insert Into " + dataBaseTableName + "(ID, ID_Language, ID_Chapter, Paragraph, Paragraphs_name) Values(@ID, @ID_Language, @ID_Chapter, @Paragraph, @Paragraphs_name)");

            //        _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(),
            //            strCRLF + "dataBaseTableName ==> " + dataBaseTableName +
            //            strCRLF + "id = " + id.ToString() +
            //            strCRLF + "id_Language = " + id_Language.ToString() +
            //            strCRLF + "id_Chapter = " + id_Chapter.ToString() +
            //            strCRLF + "paragraph = " + paragraph.ToString() +
            //            strCRLF + "paragraph_name = " + paragraph_name, CurrentClassName, showMessagesLevel);                    

            //        using (SqlCommand cmd = new SqlCommand(sql, this.connect))
            //        {
            //            cmd.Parameters.AddWithValue("@ID", id);
            //            cmd.Parameters.AddWithValue("@ID_Language", id_Language);
            //            cmd.Parameters.AddWithValue("@ID_Chapter", id_Chapter);
            //            cmd.Parameters.AddWithValue("@Paragraph", paragraph);
            //            cmd.Parameters.AddWithValue("@Paragraphs_name", paragraph_name);
            //            try
            //            {
            //                int iSqlCommandReturn = cmd.ExecuteNonQuery();
            //                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " SqlCommand (Records inserted) returned ==> " + iSqlCommandReturn.ToString(), CurrentClassName, showMessagesLevel);
            //            }
            //            catch (SqlException ex)
            //            {
            //                _messageService.ShowError(ex.Message);
            //            }                       
            //        }
            //        return (int)ResultDidWithTables.Successfully;
            //    }                
            //}
            //return (int)ResultDidWithTables.CannotInsert;
        }

        public int InsertRecordInTable(string[] dataBaseTableNames, int[] dataBaseTableToDo, int id, int id_Language, int id_Chapter, int id_Paragraph, int sentence, string sentence_name)//Insert Record in Table Sentences
        {
            for (int i = 1; i < dataBaseTableQuantuty; i++)//0 - Languages - cannot insert records
            {
                if (dataBaseTableToDo[i] == (int)WhatNeedDoWithTables.InsertRecord)//define what table will insert in
                {
                    string dataBaseTableName = dataBaseTableNames[i];
                    string sql = "";
                    if (id_Paragraph < 0) sql = string.Format("Insert Into " + dataBaseTableName + "(ID, ID_Language, ID_Chapter, Paragraph, Paragraphs_name) Values(@ID, @ID_Language, @ID_Chapter, @Paragraph, @Paragraphs_name)");
                    else sql = string.Format("Insert Into " + dataBaseTableName + "(ID, ID_Language, ID_Chapter, ID_Paragraph, Sentence, Sentence_name) Values(@ID, @ID_Language, @ID_Chapter, @ID_Paragraph, @Sentence, @Sentence_name)");

                    int paragraph = sentence;
                    string paragraph_name = sentence_name;

                    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(),
                        strCRLF + "dataBaseTableName ==> " + dataBaseTableName +
                        strCRLF + "id = " + id.ToString() +
                        strCRLF + "id_Language = " + id_Language.ToString() +
                        strCRLF + "id_Chapter = " + id_Chapter.ToString() +
                        strCRLF + "paragraph = " + paragraph.ToString() +
                        strCRLF + "paragraph_name = " + paragraph_name, CurrentClassName, showMessagesLevel);

                    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(),
                        strCRLF + "dataBaseTableName ==> " + dataBaseTableName +
                        strCRLF + "id = " + id.ToString() +
                        strCRLF + "id_Language = " + id_Language.ToString() +
                        strCRLF + "id_Chapter = " + id_Chapter.ToString() +
                        strCRLF + "id_Paragraph = " + id_Paragraph.ToString() +
                        strCRLF + "sentence = " + sentence.ToString() +
                        strCRLF + "sentence_name = " + sentence_name, CurrentClassName, showMessagesLevel);

                    using (SqlCommand cmd = new SqlCommand(sql, this.connect))
                    {                        
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.Parameters.AddWithValue("@ID_Language", id_Language);
                        cmd.Parameters.AddWithValue("@ID_Chapter", id_Chapter);
                        if (id_Paragraph < 0)//id_Paragraph в вызове = -1 - вставляем Paragraphs
                        {
                            cmd.Parameters.AddWithValue("@Paragraph", paragraph);
                            cmd.Parameters.AddWithValue("@Paragraphs_name", paragraph_name);
                        }
                        else//id_Paragraph в вызове > 0 - вставляем Sentences
                        {
                            cmd.Parameters.AddWithValue("@ID_Paragraph", id_Paragraph);
                            cmd.Parameters.AddWithValue("@Sentence", sentence);
                            cmd.Parameters.AddWithValue("@Sentence_name", sentence_name);
                        }
                        
                        try
                        {
                            int iSqlCommandReturn = cmd.ExecuteNonQuery();
                            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " SqlCommand (Records inserted) returned ==> " + iSqlCommandReturn.ToString(), CurrentClassName, showMessagesLevel);
                        }
                        catch (SqlException ex)
                        {
                            _messageService.ShowError(ex.Message);
                        }
                    }
                    return (int)ResultDidWithTables.Successfully;
                }                
            }
            return (int)ResultDidWithTables.CannotInsert;
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

