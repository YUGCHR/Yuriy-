using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using TextSplitLibrary;
using System.Windows.Forms;

namespace TextSplit
{
    public class MainPresentor
    {
        private readonly ITextSplitForm _view;
        private ITextSplitOpenForm _open;
        private readonly IFileManager _manager;
        private readonly IMessageService _messageService;
        private readonly ILogFileMessages _logs;
        private readonly IDataAccessor _data;

        private bool wasEnglishContentChange = false;
        readonly private string strCRLF;
        readonly private int filesQuantity;
        readonly private int filesQuantityPlus;
        readonly private int iBreakpointManager;
        readonly private int resultFileNumber;
        private int showMessagesLevel;
        private string resultFileName;

        private int[] filesToDo;
        private int[] counts;
        private string[] filesPath;
        private string[] filesContent;

        private string[] dataBaseTableNames;
        private int[] dataBaseTableToDo;
        private int dataBaseTableQuantuty;        

        private int ID_Language;
        private int ID_Chapter;
        private int ID_Paragraph;
        private int ID_Sentenses;
        private int Sentence;

        //bool[] isFilesExist;

        public MainPresentor(ITextSplitForm view, ITextSplitOpenForm open, IFileManager manager, IMessageService service, ILogFileMessages logs, IDataAccessor data)
        {
            _view = view;
            _open = open;
            _manager = manager;
            _messageService = service;
            _logs = logs;
            _data = data;

            strCRLF = Declaration.StrCRLF;
            showMessagesLevel = Declaration.ShowMessagesLevel;
            filesQuantity = Declaration.FilesQuantity;
            filesQuantityPlus = Declaration.ToDoQuantity;
            iBreakpointManager = filesQuantityPlus - 1;
            resultFileNumber = Declaration.ResultFileNumber;//index of the Resalt File
            resultFileName = Declaration.ResultFileName;

            string mainStart = "******************************************************************************************************************************************* \r\n";//Log-file separator
            _messageService.ShowTrace(mainStart + MethodBase.GetCurrentMethod().ToString(), " Started", CurrentClassName, showMessagesLevel);

            filesToDo = new int[filesQuantityPlus];
            counts = new int[filesQuantity];
            _open.SetSymbolCount(counts, filesToDo);//move?

            filesPath = new string[filesQuantity];
            filesContent = new string[filesQuantity];

            dataBaseTableNames = new string[] { "Languages", "Chapters", "Paragraphs", "Sentences" };
            dataBaseTableQuantuty = dataBaseTableNames.Length;
            dataBaseTableToDo = new int[dataBaseTableQuantuty];
            //enum WhatNeedDoWithTables => PassThrough = 0, ReadRecord = 1, ReadAllRecord = 2, Reserved3 = 3, InsertRecord = 4, DeleteRecord = 5, ClearTable = 7, ContinueProcessing = 8, StopProcessing = 9
           
            ID_Language = 0;
            ID_Chapter = -1;
            ID_Paragraph = 0;
            ID_Sentenses = 0;
            Sentence = 0;


        //isFilesExist = new bool[filesQuantity];
        //_view.ManageFilesContent(filesPath, filesContent, filesToDo);//move?

        _view.OpenTextSplitOpenForm += new EventHandler(_view_OpenTextSplitOpenForm);
        _view.FilesSaveClick += new EventHandler(_view_FilesSaveClick);
        _view.TextSplitFormClosing += new EventHandler<FormClosingEventArgs>(_view_TextSplitFormClosing);
        }

        //dataBase Tables Names
        //0 - Languages - cannot insert records
        //1 - Chapters - Columns - ID, ID_Language, int Chapter, nvchar10 Chapter_name
        //2 - Paragraphs - Columns - ID, ID_Language, ID_Chapter, int Paragraph, nvchar10 Paragraph_name
        //3 - Sentences - Columns - ID, ID_Language, ID_Chapter, ID_Paragraph, int Sentence, ntext Sentence_name

        private void _open_LoadEnglishToDataBase(object sender, EventArgs e)//by the way - maybe it is need to add to the tables quantity of chapters and others
        {
            _data.OpenConnection();
            //_data.ClearAllTables();
            char[] charsParagraphSeparator = new char[] { '\r', '\n' };
            char[] charsSentenceSeparator = new char[] { '.', '!', '?' };            

            if (filesToDo[(int)TableLanguagesContent.English] == (int)WhatNeedDoWithFiles.CountSymbols)//check is English content filled in the filesContent - CountSymbols was done
            {
                string currentText = filesContent[(int)TableLanguagesContent.English];
                string[] currentTextParagraphsPortioned = currentText.Split(charsParagraphSeparator);//portioned English content in the ParagraphsArray
                int iSent = 0;
                ID_Paragraph = 0;
                int previousParagraphLength = -1;//start flag value - before Paragraph exists
                
                foreach (string currentParagraph in currentTextParagraphsPortioned)//place in - s - each Paragraph
                {
                    int lengthOfCurrentParagraph = currentParagraph.Length;//count symbols quantity in the current Paragraph                    
                    string paragraph_name = lengthOfCurrentParagraph.ToString();//(for test only)

                    #region Chapters 
                    //ID_Chapter = -1; on the first pass of the foreach
                    //find Chapters and set Chapters ID and name
                    bool isPreviousLineBlank = previousParagraphLength == 0;//check if previous line was blank
                    if (iSent > 0)//if not first step
                    {
                        ID_Chapter = FindChapterNumber(currentParagraph, ID_Chapter, isPreviousLineBlank);                        
                    }
                    #endregion

                    #region Paragraphs & Sentences
                    if (ID_Chapter >= 0)//if the first Chapter has not sent we cannot insert the Paragraph
                    {
                        dataBaseTableToDo[(int)TablesNamesNumbers.Paragraphs] = (int)WhatNeedDoWithTables.InsertRecord;
                        int insertResultParagraphs = _data.InsertRecordInTable(dataBaseTableNames[(int)TablesNamesNumbers.Paragraphs], dataBaseTableToDo, ID_Paragraph, ID_Language, ID_Chapter, lengthOfCurrentParagraph, paragraph_name);//Insert Record in Table Paragraphs
                        _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " insertResult after Paragraphs ==> ", insertResultParagraphs.ToString(), CurrentClassName, showMessagesLevel);
                        string[] currentParagraphSentences = currentParagraph.Split(charsSentenceSeparator);//portioned current paragraph on sentences                        
                        ID_Sentenses = CurrentParagraphInsertInSentences(currentParagraphSentences, ID_Sentenses);
                        ID_Paragraph++;
                    }
                    #endregion

                    iSent++;//count of paragraphs
                    previousParagraphLength = currentParagraph.Length;//count symbols quantity in the current Paragraph - will be in previous Paragraph on the next pass
                }
                // to print in file
                //int textParagraphesCount = textParagraphs.Count;
                //string[] textParagraphesArray = new string[textParagraphesCount];
                //textParagraphesArray = textParagraphs.ToArray();
                //_manager.WriteToFilePathPlus(textParagraphesArray, filesPath[(int)TableLanguagesContent.English], "001");
            }
            _data.CloseConnection();
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " dB CloseConnection Successfully", CurrentClassName, 3);
        }
        private int FindChapterNumber(string currentParagraph, int id_Chapter, bool isPreviousLineBlank)
        {
            int lengthOfCurrentParagraph = currentParagraph.Length;
            if (lengthOfCurrentParagraph > 0)//and if not blank line
                if (isPreviousLineBlank)//and if previous line was blank
                {//we found the chapter begining
                    char[] isStringChapterNumber = currentParagraph.ToCharArray(0, 1);
                    bool isDigitAtSentence = Char.IsDigit(isStringChapterNumber[0]);//check is it start from digit

                    if (isDigitAtSentence)//if yes - the chapter begining found
                    {//need to define chapter number here
                        id_Chapter++;//quantity of chapters and reset -1 to 0 on the first pass of sucessful chapter search
                        //if (id_Chapter < 0) id_Chapter = 0;//reset -1 to 0 on the first pass of sucessful chapter search
                        int chapter = isStringChapterNumber.Length;                        
                        string chapter_name = new string(isStringChapterNumber);//instead of - string Chapter_name = isStringChapterNumber.ToString();
                        dataBaseTableToDo[(int)TablesNamesNumbers.Chapters] = (int)WhatNeedDoWithTables.InsertRecord;                        
                        _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), 
                            " Chapter =  " + 
                            chapter.ToString() + 
                            "\r\nChapter_name - " + 
                            chapter_name, CurrentClassName, showMessagesLevel);
                        int insertResultChapters = _data.InsertRecordInTable(dataBaseTableNames[(int)TablesNamesNumbers.Chapters], dataBaseTableToDo, id_Chapter, ID_Language, chapter, chapter_name);//Insert Record in Table Chapters
                        _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " insertResult after Chapters ==> ", insertResultChapters.ToString(), CurrentClassName, showMessagesLevel);                        
                    }
                }
            return id_Chapter;
        }
               

        private int CurrentParagraphInsertInSentences(string[] currentParagraphSentences, int id_Sentenses)
        {
            Sentence = 0;
            foreach (string sentence_name in currentParagraphSentences)//place in sentence_name each Sentence
            {
                //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " (int)TablesNamesNumbers.Sentences =  ", ((int)TablesNamesNumbers.Sentences).ToString() + "(int)WhatNeedDoWithTables.InsertRecord " + ((int)WhatNeedDoWithTables.InsertRecord).ToString(), CurrentClassName, 3);
                dataBaseTableToDo[(int)TablesNamesNumbers.Sentences] = (int)WhatNeedDoWithTables.InsertRecord;
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() +
                                                              "\r\n" +
                                                              " ID_Sentenses =  " + ID_Sentenses.ToString() + "\r\n" +
                                                              " ID_Language =  " + ID_Language.ToString() + "\r\n" +
                                                              " ID_Chapter =  " + ID_Chapter.ToString() + "\r\n" +
                                                              " ID_Paragraph =  " + ID_Paragraph.ToString() + "\r\n" +
                                                              " Sentence =  " + Sentence.ToString() + "\r\n",
                                                              " Sentence_name =  " + sentence_name, CurrentClassName, showMessagesLevel);
                int insertResultSentences = _data.InsertRecordInTable(dataBaseTableNames[(int)TablesNamesNumbers.Sentences], dataBaseTableToDo, id_Sentenses, ID_Language, ID_Chapter, ID_Paragraph, Sentence, sentence_name);//Insert Record in Table Sentences
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " insertResult after Sentences ==> ", insertResultSentences.ToString(), CurrentClassName, showMessagesLevel);
                id_Sentenses++;
                Sentence++;
            }
            return id_Sentenses;
        }
                        


    private void _view_TextSplitFormClosing(object sender, FormClosingEventArgs e)
        {
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " Closing attempt catched", CurrentClassName, showMessagesLevel);
            //var formArgs = (FormClosingEventArgs)e;
            e.Cancel = wasEnglishContentChange;
            //_view.WasEnglishContentChange = wasEnglishContentChange;
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " wasEnglishContentChange = ", wasEnglishContentChange.ToString(), CurrentClassName, showMessagesLevel);
        }

        void _view_OpenTextSplitOpenForm(object sender, EventArgs e)//обрабатываем нажатие кнопки Open, которое означает открытие вспомогательной формы
        {
            _data.ClearAllTables();
            TextSplitOpenForm openForm = new TextSplitOpenForm(_messageService);
            _open = openForm;
            _open.OpenFileClick += new EventHandler(_open_FilesOpenClick);
            _open.ContentChanged += new EventHandler(_open_ContentChanged);
            _open.LoadEnglishToDataBase += _open_LoadEnglishToDataBase;
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "openForm will start now", CurrentClassName, showMessagesLevel);
            openForm.Show();
        }

        private void _view_FilesSaveClick(object sender, EventArgs e)
        {
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Start", CurrentClassName, showMessagesLevel);
            try
            {
                //string content = _view.FileContent;
                //_manager.SaveContent(content, _currentFilePath);
                _messageService.ShowMessage("File saved sucessfully!");
                wasEnglishContentChange = false;
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " wasEnglishContentChange = ", wasEnglishContentChange.ToString(), CurrentClassName, showMessagesLevel);
            }
            catch (Exception ex)
            {
                _messageService.ShowError(ex.Message);
            }
        }
        
        private void _open_FilesOpenClick(object sender, EventArgs e)
        {
            try
            {
                filesPath = _open.GetFilesPath();
                filesToDo = _open.GetFilesToDo();

                int iBreakpointManager = isFilesExistCheckAndOpen();
                
                if (filesToDo[iBreakpointManager] == (int)WhatNeedDoWithFiles.ContinueProcessing)//WittingIncomplete)
                    {                        
                        _open.SetFileContent(filesPath, filesContent, filesToDo, iBreakpointManager);
                        filesToDo[iBreakpointManager] = (int)WhatNeedDoWithFiles.CountSymbols;
                        counts[iBreakpointManager] = _manager.GetSymbolCounts(filesContent, iBreakpointManager);
                        _open.SetFilesToDo(filesToDo);
                        _open.SetSymbolCount(counts, filesToDo);                        
                    }
            }
            catch (Exception ex)
            {
                _messageService.ShowError(ex.Message);
            }
        }

        int isFilesExistCheckAndOpen()
        {
            int textFieldsQuantity = filesQuantity - 1;//TEMP

            for (int i = 0; i < textFieldsQuantity; i++)
            {
                int BreakpointManager = filesToDo[i];
                
                if (BreakpointManager == (int)WhatNeedDoWithFiles.ReadFirst)
                {                    
                    if (_manager.IsFilesExist(filesPath[i]))
                    {                        
                        filesContent = _manager.GetContents(filesPath, filesToDo);
                        filesToDo[i] = (int)WhatNeedDoWithFiles.ContinueProcessing;
                        return i;
                    }
                    else
                    {
                        _messageService.ShowExclamation("The source file does not exist, please select it!");
                        return i; //some file does not exist
                    }
                }
            }
            _messageService.ShowExclamation("Something was wrong!");
            return -1; //some file does not exist
        }

        void LoadEnglishToDataBase()
        {

        }

        //int toCreateResultFile() //we check the result file existing and try to create it
        //{
        //    if (isFilesExist[resultFileNumber]) //if resultFile does not exist, we will create it in the path of the first selected file (with 0 index)
        //    {//result file exist
        //        filesToDo[resultFileNumber] = (int)WhatNeedDoWithFiles.ReadFirst;//the selected result file will be processing                
        //        return -1;//all files exist
        //    }
        //    else
        //    {
        //        _messageService.ShowExclamation("The Result file does not exist, it will be created now!");
        //        filesPath[resultFileNumber] = _manager.CreateFile(filesPath[0], resultFileName);//we had tried to create result file
        //        if (filesPath[resultFileNumber] != null)
        //        {//we created result file successfully
        //            filesToDo[resultFileNumber] = (int)WhatNeedDoWithFiles.ReadFirst;//the created result file needs in processing                    
        //            return -1; //all files exist
        //        }
        //        else //we cannot create result file
        //        {
        //            _messageService.ShowExclamation("The Result file already exist, please select or delete it!");
        //            return 2; //we need to decide what to do with existing result file
        //        }
        //    }
        //}

        void _open_ContentChanged(object sender, EventArgs e)
        {
            filesToDo = _open.GetFilesToDo();
            filesContent = _open.GetFilesContent();
            int textFieldsQuantity = filesQuantity - 1;//TEMP
            //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " filesContent - ", filesContent, CurrentClassName, 3);
            for (int i = 0; i < textFieldsQuantity; i++)
            {
                if (filesToDo[i] == (int)WhatNeedDoWithFiles.ContentChanged)
                {
                    //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " filesToDo[i] - ", filesToDo[i].ToString(), CurrentClassName, 3); 
                    filesToDo[i] = (int)WhatNeedDoWithFiles.CountSymbols;
                    counts[i] = _manager.GetSymbolCounts(filesContent, i);
                    _open.SetFilesToDo(filesToDo);
                    _open.SetSymbolCount(counts, filesToDo);
                }


                //string[] contents = _view.FilesContent;                        
                //int[] counts = _manager.GetSymbolCounts(contents);
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + "wasEnglishContentChange", wasEnglishContentChange.ToString(), CurrentClassName, showMessagesLevel);
                //_view.SetSymbolCount(counts, _view.FilesToDo);
                wasEnglishContentChange = true;//we need also the array here
            }
        }

        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }
    }
}