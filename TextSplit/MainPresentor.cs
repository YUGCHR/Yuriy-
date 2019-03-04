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
        //bool[] isFilesExist;

        public MainPresentor(ITextSplitForm view, ITextSplitOpenForm open, IFileManager manager, IMessageService service, ILogFileMessages logs)
        {
            _view = view;
            _open = open;
            _manager = manager;
            _messageService = service;
            _logs = logs;

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
            //isFilesExist = new bool[filesQuantity];
            //_view.ManageFilesContent(filesPath, filesContent, filesToDo);//move?

            _view.OpenTextSplitOpenForm += new EventHandler(_view_OpenTextSplitOpenForm);
            //_open.AllOpenFilesClick += new EventHandler(_open_FilesOpenClick);            
            _open.ContentChanged += new EventHandler(_view_ContentChanged);
            _view.FilesSaveClick += new EventHandler(_view_FilesSaveClick);
            _view.TextSplitFormClosing += new EventHandler<FormClosingEventArgs>(_view_TextSplitFormClosing);
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
            TextSplitOpenForm openForm = new TextSplitOpenForm(_messageService);
            _open = openForm;
            _open.OpenFileClick += new EventHandler(_open_FilesOpenClick);
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
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " iBreakpointManager = " + iBreakpointManager.ToString(), CurrentClassName, 3);
                if (filesToDo[iBreakpointManager] == (int)WhatNeedDoWithFiles.ContinueProcessing)//WittingIncomplete)
                    {
                        _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " filesToDo[iBreakpointManager] = " + filesToDo[iBreakpointManager].ToString(), CurrentClassName, 3);
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

        int isFilesExistCheckAndOpen() // check files pathes and prepare filesToDo array
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
        
        void _view_ContentChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < filesQuantity; i++)
            {
                if (_view.FilesToDo[i] != 0)
                {
                    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " _view.FilesToDo[i] - ", _view.FilesToDo[i].ToString(), CurrentClassName, showMessagesLevel);
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