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
        private List<string> textParagraphs;

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
            textParagraphs = new List<string>();
            
            ID_Language = 0;
            ID_Chapter = 0;
            ID_Paragraph = 0;
            ID_Sentenses = 0;
            Sentence = 0;


        //isFilesExist = new bool[filesQuantity];
        //_view.ManageFilesContent(filesPath, filesContent, filesToDo);//move?

        _view.OpenTextSplitOpenForm += new EventHandler(_view_OpenTextSplitOpenForm);
            //_open.AllOpenFilesClick += new EventHandler(_open_FilesOpenClick);
            
            _view.FilesSaveClick += new EventHandler(_view_FilesSaveClick);
            _view.TextSplitFormClosing += new EventHandler<FormClosingEventArgs>(_view_TextSplitFormClosing);
        }

        private void _open_LoadEnglishToDataBase(object sender, EventArgs e)
        {
            _data.OpenConnection();
            char[] charsParagraphSeparator = new char[] { '\r', '\n' };
            char[] charsSentenceSeparator = new char[] { '.', '!', '?' };
            //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "filesToDo = " + filesToDo[(int)TableLanguagesContent.English].ToString(), CurrentClassName, 3);

            if (filesToDo[(int)TableLanguagesContent.English] == (int)WhatNeedDoWithFiles.CountSymbols)
            {
                string currentText = filesContent[(int)TableLanguagesContent.English];
                string[] currentTextSentences = currentText.Split(charsParagraphSeparator);
                int iSent = 0;//current Paragraph index
                //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " textParagraphes ", textParagraphes.ToString(), CurrentClassName, 3);
                foreach (string s in currentTextSentences)
                {
                    
                    int lengthS = s.Length;
                    textParagraphs.Add(s);

                    //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " Sentence No Lengh ==> ", iSent.ToString() + " = " + s + " ==> " + lengthS.ToString(), CurrentClassName, 3);

                    if (iSent > 0)
                        if (lengthS > 0)
                            if (textParagraphs[iSent - 1].Length == 0)//if not first step and if not blank line and if previous line was blank - we found the chapter begining - now we check is it start from digit
                            {
                                char[] isStringChapterNumber = s.ToCharArray(0, 1);
                                bool isDigitAtSentence = Char.IsDigit(isStringChapterNumber[0]);

                                if (isDigitAtSentence)
                                {
                                    //we found the chapter begining
                                    //need to define chapter number here
                                    //by the way - maybe it is need to add to the tables quantity of chapters and others
                                    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " isDigitAtSentence =  ", isDigitAtSentence.ToString() + "textParagraphs[iSent] " + textParagraphs[iSent], CurrentClassName, 3);

                                    //we will portion current paragraph on sentences

                                    string[] currentParagraphSentences = textParagraphs[iSent].Split(charsSentenceSeparator);
                                    Sentence = 0;
                                    ID_Paragraph = iSent;
                                    foreach (string sentence_name in currentParagraphSentences)
                                    {
                                        //string dataBaseTableName = dataBaseTableNames[i];
                                        //0 - Languages - cannot insert records
                                        //1 - Chapters - Columns - ID, ID_Language, int Chapter, nvchar10 Chapter_name
                                        //2 - Paragraphs - Columns - ID, ID_Language, ID_Chapter, int Paragraph, nvchar10 Paragraph_name
                                        //3 - Sentences - Columns - ID, ID_Language, ID_Chapter, ID_Paragraph, int Sentence, ntext Sentence_name
                                        _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " (int)TablesNamesNumbers.Sentences =  ", ((int)TablesNamesNumbers.Sentences).ToString() + "(int)WhatNeedDoWithTables.InsertRecord " + ((int)WhatNeedDoWithTables.InsertRecord).ToString(), CurrentClassName, 3);
                                        dataBaseTableToDo[(int)TablesNamesNumbers.Sentences] = (int)WhatNeedDoWithTables.InsertRecord;
                                        _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() +
                                                                  "\r\n" +
                                                                  " ID_Sentenses =  " + ID_Sentenses.ToString() + "\r\n" +
                                                                  " ID_Language =  " + ID_Language.ToString() + "\r\n" +
                                                                  " ID_Chapter =  " + ID_Chapter.ToString() + "\r\n" +
                                                                  " ID_Paragraph =  " + ID_Paragraph.ToString() + "\r\n" +
                                                                  " Sentence =  " + Sentence.ToString() + "\r\n",
                                                                  " Sentence_name =  " + sentence_name, CurrentClassName, 3);
                                        int insertResult = _data.InsertRecordInTable(dataBaseTableNames[(int)TablesNamesNumbers.Sentences], dataBaseTableToDo, ID_Sentenses, ID_Language, ID_Chapter, ID_Paragraph, Sentence, sentence_name);//Insert Record in Table Sentences
                                        ID_Sentenses++;
                                        Sentence++;
                                    }
                                    ID_Chapter++;//quantity of chapters
                                }
                            }                    
                    iSent++;//quantity of paragraphs
                }

                int textParagraphesCount = textParagraphs.Count;
                string[] textParagraphesArray = new string[textParagraphesCount];
                textParagraphesArray = textParagraphs.ToArray();
                _manager.WriteToFilePathPlus(textParagraphesArray, filesPath[(int)TableLanguagesContent.English], "001");
            }
            _data.CloseConnection();
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