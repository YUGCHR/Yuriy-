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
    public interface IDataBaseAccessor
    {
        void OpenConnection();        
        void ExecuteReader();
        int ClearAllTables();
        void CloseConnection();
        int InsertRecordInTable(string[] dataBaseTableNames, int[] dataBaseTableToDo, int id, int id_Language, int chapter, string chapter_name);//Insert Record in Table Chapters
        int InsertRecordInTable(string[] dataBaseTableName, int[] dataBaseTableToDo, int id, int id_Language, int id_Chapter, int paragraph, string paragraph_name);//Insert Record in Table Paragraphs
        int InsertRecordInTable(string[] dataBaseTableName, int[] dataBaseTableToDo, int id, int id_Language, int id_Chapter, int id_Paragraph, int sentence, string sentence_name);//Insert Record in Table Sentences
        int InsertTableLanguagesRecords(int filesQuantity, int[] dataBaseTableToDo);
    }

    public class DataBaseAccessor : IDataBaseAccessor//abstract
    {
        private readonly IMessageService _messageService;

        private const string connectionStringSource = "Data Source=.;";
        private const string connectionStringDataBase = "Initial Catalog = TextSplitSentences;";
        private const string connectionStringSecurity = "Integrated Security=true";
        private const string connectionString = connectionStringSource + connectionStringDataBase + connectionStringSecurity;

        private SqlConnection connect = null;
        private readonly string strCRLF;
        private readonly int showMessagesLevel;        

        private readonly string[] dataBaseTableNames;
        private int[] dataBaseTableToDo;
        private readonly int dataBaseTableQuantuty;        

        public DataBaseAccessor(IMessageService service)
        {
            _messageService = service;

            strCRLF = Declaration.StrCRLF;
            showMessagesLevel = Declaration.ShowMessagesLevel;
            dataBaseTableNames = Declaration.DataBaseTableNames;// - Languages, Chapters, Paragraphs, Sentences

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
            string sqlExpressionDelete = "DELETE FROM ";
            string tableNameForSqlExpression;
            string sqlExpressionCommandDelete;
            string sqlExpressionCommandCheckident;
            int iSqlCommandReturn = -1;

            for (int i = dataBaseTableQuantuty-1; i > 0; i--)//Table 0 (Languages) does not need to clear
            {
                tableNameForSqlExpression = dataBaseTableNames[i];// 0 -> Languages, 1 -> Chapters, 2 -> Paragraphs, 3 -> Sentences
                sqlExpressionCommandDelete = sqlExpressionDelete + tableNameForSqlExpression;
                sqlExpressionCommandCheckident = "DBCC CHECKIDENT (" + tableNameForSqlExpression + ", reseed, -1)";//сброс значения автоинкремента и начало его с нуля

                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " sqlExpressionCommandDelete ==> " + sqlExpressionCommandDelete, CurrentClassName, showMessagesLevel);
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " sqlExpressionCommandCheckident ==> " + sqlExpressionCommandCheckident, CurrentClassName, showMessagesLevel);

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " sqlExpression ==> " + sqlExpression.ToString(), CurrentClassName, showMessagesLevel);
                    SqlCommand commandDelete = new SqlCommand(sqlExpressionCommandDelete, connection);
                    SqlCommand commandCheckident = new SqlCommand(sqlExpressionCommandCheckident, connection);
                    try
                    {                        
                        iSqlCommandReturn = commandDelete.ExecuteNonQuery();
                        _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " SqlCommand DELETE FROM " + tableNameForSqlExpression + strCRLF + " returned ==> " + iSqlCommandReturn.ToString(), CurrentClassName, showMessagesLevel);                        
                        
                        iSqlCommandReturn = commandCheckident.ExecuteNonQuery();
                        _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " SqlCommand DBCC CHECKIDENT " + tableNameForSqlExpression + strCRLF + " returned ==> " + iSqlCommandReturn.ToString(), CurrentClassName, showMessagesLevel);
                    }
                    catch (SqlException ex)
                    {
                        _messageService.ShowError(ex.Message);
                        return -1;
                    }
                    connection.Close();
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
            return InsertRecordInTable(dataBaseTableNames, dataBaseTableToDo, id, id_Language, -1, -1, chapter, chapter_name);//overload Insert Record in Table Sentences
        }

        public int InsertRecordInTable(string[] dataBaseTableNames, int[] dataBaseTableToDo, int id, int id_Language, int id_Chapter, int paragraph, string paragraph_name)//Insert Record in Table Paragraphs
        {
            return InsertRecordInTable(dataBaseTableNames, dataBaseTableToDo, id, id_Language, id_Chapter, -1, paragraph, paragraph_name);//overload Insert Record in Table Sentences
        }

        public int InsertRecordInTable(string[] dataBaseTableNames, int[] dataBaseTableToDo, int id, int id_Language, int id_Chapter, int id_Paragraph, int sentence, string sentence_name)//Insert Record in Table Sentences
        {            
            string sql = "";
            int intValue = sentence;
            string intValueName = sentence_name;            

            for (int i = 1; i < dataBaseTableQuantuty; i++)//0 - Languages - cannot insert records
            {                
                if (dataBaseTableToDo[i] == (int)WhatNeedDoWithTables.InsertRecord)//Define the table to be inserted in
                {
                    string dataBaseTableName = dataBaseTableNames[i];

                    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(),
                        strCRLF + "dataBaseTableName ==> " + dataBaseTableName +
                        strCRLF + "id = " + id.ToString() +
                        strCRLF + "id_Language = " + id_Language.ToString() +
                        strCRLF + "id_Chapter = " + id_Chapter.ToString() + " - (if -1 - Chapter)" +
                        strCRLF + "id_Paragraph = " + id_Paragraph.ToString() + " - (if -1 - Paragraph)" +
                        strCRLF + "sentence = " + intValue.ToString() +
                        strCRLF + "sentence_name = " + intValueName, CurrentClassName, showMessagesLevel);

                    if (id_Chapter < 0) //prepare format Chapter
                    {
                        sql = string.Format("Insert Into " + dataBaseTableName + "(ID_Language, Chapter, Chapter_name) Values(@ID_Language, @Chapter, @Chapter_name)");//ID - identity autoincrement
                        int ii = CmdAssembly(sql, id, id_Language, -1, -1, intValue, intValueName);
                        return (int)TablesProcessingResult.Successfully;
                    }
                    else
                    {
                        if (id_Paragraph < 0) //prepare format Paragraph
                        {
                            sql = string.Format("Insert Into " + dataBaseTableName + "(ID_Language, ID_Chapter, Paragraph, Paragraph_name) Values(@ID_Language, @ID_Chapter, @Paragraph, @Paragraph_name)");//ID - identity autoincrement
                            int ii = CmdAssembly(sql, id, id_Language, id_Chapter, -1, intValue, intValueName);
                            return (int)TablesProcessingResult.Successfully;
                        }
                        else //prepare format Sentence
                        {
                            sql = string.Format("Insert Into " + dataBaseTableName + "(ID_Language, ID_Chapter, ID_Paragraph, Sentence, Sentence_name) Values(@ID_Language, @ID_Chapter, @ID_Paragraph, @Sentence, @Sentence_name)");//ID - identity autoincrement
                            int ii = CmdAssembly(sql, id, id_Language, id_Chapter, id_Paragraph, intValue, intValueName);
                            return (int)TablesProcessingResult.Successfully;
                        }
                    }                    
                }                
            }
            return (int)TablesProcessingResult.CannotInsert;
        }
        
        private int CmdAssembly(string sql, int id, int id_Language, int id_Chapter, int id_Paragraph, int intValue, string intValueName)
        {
            using (SqlCommand cmd = new SqlCommand(sql, this.connect))
            {
                //cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@ID_Language", id_Language);
                if (id_Chapter < 0)//id_Chapter в вызове = -1 - вставляем Chapter
                {
                    cmd.Parameters.AddWithValue("@Chapter", intValue);
                    cmd.Parameters.AddWithValue("@Chapter_name", intValueName);
                    int ii = ExecuteWriter(cmd);
                    return 0;//проверить все коды возврата
                }
                else //не глава - вставляем ссылку на главу и проверяем - Paragraph или Sentence
                {
                    cmd.Parameters.AddWithValue("@ID_Chapter", id_Chapter);
                    if (id_Paragraph < 0)//id_Paragraph в вызове = -1 - вставляем Paragraphs
                    {                        
                        cmd.Parameters.AddWithValue("@Paragraph", intValue);
                        cmd.Parameters.AddWithValue("@Paragraph_name", intValueName);
                        int ii = ExecuteWriter(cmd);
                        return 0;
                    }
                    else//id_Paragraph в вызове > 0 - вставляем Sentences
                    {                        
                        cmd.Parameters.AddWithValue("@ID_Paragraph", id_Paragraph);
                        cmd.Parameters.AddWithValue("@Sentence", intValue);
                        cmd.Parameters.AddWithValue("@Sentence_name", intValueName);                        
                        int ii = ExecuteWriter(cmd);
                        return 0;
                    }
                }                
            }            
        }

        public int InsertTableLanguagesRecords(int filesQuantity, int[] dataBaseTableToDo)//Insert Records in Table Languages
        {
            int iCmdResult = -1;
            int iCmdResults = 0;
            string[] dataBaseTableNames = new string[] { "English", "Russian", "Result" };
            int id_Language = -1;
            string language_name = "";

            if (dataBaseTableToDo[0] == (int)WhatNeedDoWithTables.InsertRecord)//value "InsertRecord" in index 0 means - to insert records in Table Languages was allowed
            {                             
                int tableLanguagesRecordsCount = dataBaseTableNames.Length;
                if (filesQuantity != tableLanguagesRecordsCount) return (int)TablesProcessingResult.CannotInsert;

                for (int i = 0; i < filesQuantity; i++)//insert records in Languages
                {
                    id_Language = i;
                    language_name = dataBaseTableNames[i];

                    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(),                                                
                        strCRLF + "id_Language = " + id_Language.ToString() +                                                
                        strCRLF + "Language_name = " + language_name, CurrentClassName, 3);

                    string sql = string.Format("Insert Into Languages(ID_Language, Language_name) Values(@ID_Language, @Language_name)");
                    using (SqlCommand cmd = new SqlCommand(sql, this.connect))
                    {
                        cmd.Parameters.AddWithValue("@ID_Language", id_Language);
                        cmd.Parameters.AddWithValue("@Language_name", language_name);
                        iCmdResult = ExecuteWriter(cmd);
                        iCmdResults = iCmdResults + iCmdResult;
                    }
                }
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " Records to table Languages inserted Successfully " + strCRLF + "Records inserted ==> " + iCmdResults.ToString(), CurrentClassName, 3);
                return (int)TablesProcessingResult.Successfully;
            }
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " Parameter dataBaseTableToDo[0] does not allow to insert records ==> " + dataBaseTableToDo[0].ToString(), CurrentClassName, 3);
            return (int)TablesProcessingResult.CannotInsert;
        }

        private int ExecuteWriter(SqlCommand cmd)
        {
            try
            {
                int iSqlCommandReturn = cmd.ExecuteNonQuery();
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " SqlCommand (Records inserted) returned ==> " + iSqlCommandReturn.ToString(), CurrentClassName, showMessagesLevel);
                return iSqlCommandReturn;
            }
            catch (SqlException ex)
            {
                _messageService.ShowError(ex.Message);
                return -1;
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

