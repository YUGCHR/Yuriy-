using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using TextSplitLibrary;

namespace TextSplit
{
    public interface ILoadTextToDataBase
    {
        int PortionTextForDataBase(int ID_Language);
        //int FindChapterNumber(string currentParagraph, int ID_Language, int id_Chapter, bool isPreviousLineBlank);
        //int PortionParagraphOnSentences(string[] currentParagraphSentences, int ID_Language, int id_Chapter, int id_Paragraph, int id_Sentenses);
    }

    class LoadTextToDataBase : ILoadTextToDataBase
    {
        private readonly IAllBookData _book;
        private readonly IMessageService _messageService;
        private readonly IDataBaseAccessor _data;

        private string[] dataBaseTableNames;
        private int[] dataBaseTableToDo;
        readonly private int dataBaseTableQuantuty;
        readonly private int showMessagesLevel;
        readonly private int filesQuantity;
        readonly private string strCRLF;

        public LoadTextToDataBase(IAllBookData book, IDataBaseAccessor data, IMessageService service)
        {
            strCRLF = DConst.StrCRLF;
            showMessagesLevel = DConst.ShowMessagesLevel;
            filesQuantity = DConst.FilesQuantity;

            _book = book;
            _messageService = service;
            _data = data;

            dataBaseTableNames = new string[] { "Languages", "Chapters", "Paragraphs", "Sentences" };
            //0 - Languages - cannot insert records
            //1 - Chapters - Columns - ID, ID_Language, int Chapter, nvchar10 Chapter_name
            //2 - Paragraphs - Columns - ID, ID_Language, ID_Chapter, int Paragraph, nvchar10 Paragraph_name
            //3 - Sentences - Columns - ID, ID_Language, ID_Chapter, ID_Paragraph, int Sentence, ntext Sentence_name
            dataBaseTableQuantuty = dataBaseTableNames.Length;
            dataBaseTableToDo = new int[dataBaseTableQuantuty];
            //enum WhatNeedDoWithTables => PassThrough = 0, ReadRecord = 1, ReadAllRecord = 2, Reserved3 = 3, InsertRecord = 4, DeleteRecord = 5, ClearTable = 7, ContinueProcessing = 8, StopProcessing = 9

        }

        public int PortionTextForDataBase(int ID_Language)
        {
            char[] charsParagraphSeparator = new char[] { '\r', '\n' };            

            string currentText = _book.GetFileContent((int)TableLanguagesContent.English);
            string[] currentTextParagraphsPortioned = currentText.Split(charsParagraphSeparator);//portioned English content in the ParagraphsArray
            
            _data.OpenConnection();
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "\r\n dB connection OPENED successfully", CurrentClassName, 3);
            int clearAllTablesResult = ClearAllTables();
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " ClearAllTables returned ==> " + clearAllTablesResult.ToString(), CurrentClassName, 3);

            if (ID_Language < 0)//value -1 in "ID_Language" means - to insert records in Table Languages was allowed
            {
                dataBaseTableToDo[0] = (int)WhatNeedDoWithTables.InsertRecord;
                int i = _data.InsertTableLanguagesRecords(filesQuantity, dataBaseTableToDo);
            }
            else //usual 
            {
                int i = CurrentParagraphsPortioned(currentTextParagraphsPortioned, ID_Language);
            }

            _data.CloseConnection();
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "\r\n dB connection CLOSED successfully", CurrentClassName, 3);
            return 0;
        }

        private int CurrentParagraphsPortioned(string[] currentTextParagraphsPortioned, int ID_Language)
        {
            int ID_Chapter = -1;// - gimp stick?
            int ID_Paragraph = 0;
            int ID_Sentenses = 0;
            char[] charsSentenceSeparator = new char[] { '.', '!', '?' };
            int iSentStep = 0;
            
            int previousParagraphLength = -1;//start flag value - before Paragraph exists

            foreach (string currentParagraph in currentTextParagraphsPortioned)//place in currentParagraph each Paragraph
            {
                int lengthOfCurrentParagraph = currentParagraph.Length;//count symbols quantity in the current Paragraph - придумать более осмысленый параметр
                string paragraph_name = lengthOfCurrentParagraph.ToString();//(for test only)

                #region Chapters 
                //ID_Chapter = -1; on the first pass of the foreach
                //find Chapters and set Chapters ID and name
                bool isPreviousLineBlank = previousParagraphLength == 0;//check if previous line was blank
                if (iSentStep > 0)//if not first step
                {
                    ID_Chapter = FindChapterNumber(currentParagraph, ID_Language, ID_Chapter, isPreviousLineBlank);
                }
                #endregion
                #region Paragraphs & Sentences
                if (ID_Chapter >= 0)//if the first Chapter has not inserted in dB yet, we cannot insert the Paragraph
                {
                    Array.Clear(dataBaseTableToDo, 0, dataBaseTableToDo.Length);
                    dataBaseTableToDo[(int)TablesNamesNumbers.Paragraphs] = (int)WhatNeedDoWithTables.InsertRecord;// - массив объявлен вне метода и не передан в метод
                    //dataBaseTableNames[(int)TablesNamesNumbers.Paragraphs] - искать имя таблицы по индексу dataBaseTableToDo который равен InsertRecord
                    int insertResultParagraphs = _data.InsertRecordInTable(dataBaseTableNames, dataBaseTableToDo, ID_Paragraph, ID_Language, ID_Chapter, lengthOfCurrentParagraph, paragraph_name);//Insert Record in Table Paragraphs
                    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " insertResult after Paragraphs ==> ", insertResultParagraphs.ToString(), CurrentClassName, showMessagesLevel);
                    string[] currentParagraphSentences = currentParagraph.Split(charsSentenceSeparator);//portioned current paragraph on sentences - съел все точки и прочие окончания предложений!
                                                                                                        // - написать свое разделение на предложния с учетом кавычек и всяких p.m. 
                                                                                                        // - показать пользователю проблемные места в поле редактора формы - точки без последующих пробелов и т.д. 
                                                                                                        // - чтобы пользователь мог написать маски для обработки
                    ID_Sentenses = PortionParagraphOnSentences(currentParagraphSentences, ID_Language, ID_Chapter, ID_Paragraph, ID_Sentenses);
                    ID_Paragraph++;
                }
                #endregion

                iSentStep++;//count of paragraphs
                previousParagraphLength = currentParagraph.Length;//count symbols quantity in the current Paragraph - will be in previous Paragraph on the next pass

                // to print in file
                //int textParagraphesCount = textParagraphs.Count;
                //string[] textParagraphesArray = new string[textParagraphesCount];
                //textParagraphesArray = textParagraphs.ToArray();
                //_manager.WriteToFilePathPlus(textParagraphesArray, filesPath[(int)TableLanguagesContent.English], "001");
            }
            return 0;
        }

        private int FindChapterNumber(string currentParagraph, int ID_Language, int id_Chapter, bool isPreviousLineBlank)
        {
            int lengthOfCurrentParagraph = currentParagraph.Length;
            if (lengthOfCurrentParagraph > 0)//and if not blank line
                if (isPreviousLineBlank)//and if previous line was blank
                {//we found the chapter begining
                    char[] isStringChapterNumber = currentParagraph.ToCharArray(0, 1);
                    bool isDigitAtSentence = Char.IsDigit(isStringChapterNumber[0]);//check is it start from digit - добавить распознавание ключевых слов от пользователя (глава т.д.)

                    if (isDigitAtSentence)//if yes - the chapter begining found
                    {//need to define chapter number here
                        id_Chapter++;//quantity of chapters and reset -1 to 0 on the first pass of sucessful chapter search
                        //from obsolete - if (id_Chapter < 0) id_Chapter = 0;//reset -1 to 0 on the first pass of sucessful chapter search
                        int chapter = isStringChapterNumber.Length;// - поставить какую-то осмысленную цифру - количество абзацев и/или предложений
                        string chapter_name = new string(isStringChapterNumber);//instead of - string Chapter_name = isStringChapterNumber.ToString(); - поставить присвоение названия главы
                        Array.Clear(dataBaseTableToDo, 0, dataBaseTableToDo.Length);
                        dataBaseTableToDo[(int)TablesNamesNumbers.Chapters] = (int)WhatNeedDoWithTables.InsertRecord;

                        _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(),
                            strCRLF + "Chapter =  " + chapter.ToString() +
                            strCRLF + "Chapter_name - " + chapter_name, CurrentClassName, showMessagesLevel);

                        int insertRecordResult = _data.InsertRecordInTable(dataBaseTableNames, dataBaseTableToDo, id_Chapter, ID_Language, chapter, chapter_name);//Insert Record in Table Chapters
                        _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " insertResult after Chapters ==> ", insertRecordResult.ToString(), CurrentClassName, showMessagesLevel);
                    }
                }
            return id_Chapter;
        }


        private int PortionParagraphOnSentences(string[] currentParagraphSentences, int ID_Language, int id_Chapter, int id_Paragraph, int id_Sentenses)
        {
            int Sentence = 0;
            foreach (string sentence_name in currentParagraphSentences)//place in sentence_name each Sentence
            {
                Array.Clear(dataBaseTableToDo, 0, dataBaseTableToDo.Length);
                dataBaseTableToDo[(int)TablesNamesNumbers.Sentences] = (int)WhatNeedDoWithTables.InsertRecord;

                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + strCRLF +
                                                              " ID_Sentenses =  " + id_Sentenses.ToString() + strCRLF +
                                                              " ID_Language =  " + ID_Language.ToString() + strCRLF +
                                                              " ID_Chapter =  " + id_Chapter.ToString() + strCRLF +
                                                              " ID_Paragraph =  " + id_Paragraph.ToString() + strCRLF +
                                                              " Sentence =  " + Sentence.ToString() + strCRLF,
                                                              " Sentence_name =  " + sentence_name, CurrentClassName, showMessagesLevel);

                int insertRecordResult = _data.InsertRecordInTable(dataBaseTableNames, dataBaseTableToDo, id_Sentenses, ID_Language, id_Chapter, id_Paragraph, Sentence, sentence_name);//Insert Record in Table Sentences
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " insertResult after Sentences ==> ", insertRecordResult.ToString(), CurrentClassName, showMessagesLevel);
                id_Sentenses++;
                Sentence++;
            }
            return id_Sentenses;
        }

        private int ClearAllTables()
        {
            int clearAllTablesResult = _data.ClearAllTables();
            return clearAllTablesResult;
        }



        private static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }
    }
}
