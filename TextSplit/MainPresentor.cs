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

        private bool wasEnglishContentChange = false;
        private int filesQuantity;
        private int showMessagesLevel;
        private int resultFileNumber;
        private string resultFileName;
        private int iBreakpoint; //Index value in the cycle breakpoint

        private int[] filesToDo;
        private int[] counts;
        private string[] filesPath;
        private string[] filesContent;
        bool[] isFilesExist;        

        public MainPresentor(ITextSplitForm view, ITextSplitOpenForm open, IFileManager manager, IMessageService service)
        {            
            _view = view;
            _open = open;            
            _manager = manager;
            _messageService = service;

            showMessagesLevel = Declaration.ShowMessagesLevel;
            filesQuantity = Declaration.LanguagesQuantity;
            resultFileNumber = Declaration.ResultFileNumber;//index of the Resalt File
            resultFileName = Declaration.ResultFileName;

            

            string mainStart = "******************************************************************************************************************************************* \r\n";//Log-file separator
            _messageService.ShowTrace(mainStart + MethodBase.GetCurrentMethod().ToString(), " Started", CurrentClassName, showMessagesLevel);

            filesToDo = new int[filesQuantity];
            counts = new int[filesQuantity];
            _view.SetSymbolCount(counts, filesToDo);//move?
            
            filesPath = new string[filesQuantity];
            filesContent = new string[filesQuantity];
            isFilesExist = new bool[filesQuantity];
            //_view.ManageFilesContent(filesPath, filesContent, filesToDo);//move?

            _view.OpenTextSplitOpenForm += new EventHandler(_view_OpenTextSplitOpenForm);
            //_open.AllOpenFilesClick += new EventHandler(_open_FilesOpenClick);            
            _view.ContentChanged += new EventHandler (_view_ContentChanged);            
            _view.FilesSaveClick += new EventHandler (_view_FilesSaveClick);            
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
            _open.AllOpenFilesClick += new EventHandler(_open_FilesOpenClick);
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
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Start", CurrentClassName, showMessagesLevel);                
                filesPath = _open.GetFilesPath();
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " filesPath value = ", filesPath, CurrentClassName, 3);// showMessagesLevel);
                isFilesExist = _manager.IsFilesExist(filesPath);
                iBreakpoint = isFilesExistCheck(); // check all files and prepare filesToDo array (-1 - OK)
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " isFilesExistCheck finished, iBreakpoint = ", iBreakpoint.ToString(), CurrentClassName, 3);// showMessagesLevel);
                if (iBreakpoint < 0) //check key files
                {//key files exist                    
                    int isResultFileCreated = toCreateResultFile();
                    if (isResultFileCreated < 0)
                    {
                        _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " If ended, check the iBreakpoint = ", iBreakpoint.ToString(), CurrentClassName, 3);// showMessagesLevel);
                        filesContent = _manager.GetContents(filesPath, filesToDo);
                        _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " filesContent[] value = ", filesContent, CurrentClassName, showMessagesLevel);
                        counts = _manager.GetSymbolCounts(filesContent);
                        _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " counts[] value = ", counts[0].ToString(), CurrentClassName, showMessagesLevel);
                        _view.SetFilesContent(filesPath, filesContent, filesToDo);
                        _view.SetSymbolCount(counts, filesToDo);
                    }
                    //the Result file already exist we need to select or to delete it
                }
                //key files does not exist - That's not the way to do things - you must select both source files                
            }            
            catch (Exception ex)
            {
                _messageService.ShowError(ex.Message);
            }
        }

        int isFilesExistCheck() // check files pathes and prepare filesToDo array
        {
            for (int i = 0; i < filesQuantity; i++) 
            {
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " isFilesExist[i] value = ", isFilesExist[i].ToString(), CurrentClassName, 3);// showMessagesLevel);
                if (isFilesExist[i]) filesToDo[i] = (int)WhatNeedDoWithFiles.ReadFirst;//if file exist we will read (open) it
                else
                {//file does not exist but we check if resultFile does not exist, then we will try to create it
                    filesToDo[i] = (int)WhatNeedDoWithFiles.StopProcessing;
                    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " file does not exist we would like to return to OpenForm, but if resultFile does not exist, we will create it ==> filesToDo[i] value = ", filesToDo[i].ToString(), CurrentClassName, 3);// showMessagesLevel);
                    if (i == resultFileNumber)
                    {//result file is in short supply
                        _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " resultFile exist ==> iBreakpoint = ", iBreakpoint.ToString(), CurrentClassName, 3);// showMessagesLevel);
                        return -1;
                    }
                    else
                    {//some of key file is in short supply
                        _messageService.ShowExclamation("The source file does not exist, please select it!");//return to OpenForm for all necessary file selection                        
                        return i; //key file(s) does not exist
                    }                                        
                }
            }            
            return -1;//all files exist
        }
        
        int toCreateResultFile() //we check the result file existing and try to create it
        {
            if (isFilesExist[resultFileNumber]) //if resultFile does not exist, we will create it in the path of the first selected file (with 0 index)
            {//result file exist
                filesToDo[resultFileNumber] = (int)WhatNeedDoWithFiles.ReadFirst;//the selected result file will be processing
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " isFilesExist[resultFileNumber] = ", isFilesExist[resultFileNumber].ToString(), CurrentClassName, 3);// showMessagesLevel);
                return -1;//all files exist
            }
            else
            {
                _messageService.ShowExclamation("The Result file does not exist, it will be created now!");
                filesPath[resultFileNumber] = _manager.CreateFile(filesPath[0], resultFileName);//we had tried to create result file
                if (filesPath[resultFileNumber] != null)
                {//we created result file successfully
                    filesToDo[resultFileNumber] = (int)WhatNeedDoWithFiles.ReadFirst;//the created result file needs in processing
                    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " Creation of resultFile ended, filesToDo[resultFileNumber] = ", filesToDo[resultFileNumber].ToString(), CurrentClassName, 3);// showMessagesLevel);
                    return -1; //all files exist
                }
                else //we cannot create result file
                {
                    _messageService.ShowExclamation("The Result file already exist, please select or delete it!");
                    return 2; //we need to decide what to do with existing result file
                }
            }
        }

        void _view_ContentChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < filesQuantity; i++)
            {
                if (_view.FilesToDo[i] != 0)
                {
                    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " _view.FilesToDo[i] - ", _view.FilesToDo[i].ToString(), CurrentClassName, 3);// showMessagesLevel);
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
