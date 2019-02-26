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
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " filesPath value = ", filesPath, CurrentClassName, showMessagesLevel);
                int[] counts = new int[filesQuantity];
                wasEnglishContentChange = false;
                bool[] isFilesExist = new bool[filesQuantity];
                isFilesExist = _manager.IsFilesExist(filesPath);
                Array.Clear(filesToDo, 0, filesToDo.Length);//Array must be still clear after Act of Creation
                iBreakpoint = -1;//Breakpoint reset - everithing allright

                for (int i = 0; i < filesQuantity; i++) //all files pathes to check and prepare filesToDo to open them
                {
                    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " isFilesExist[i] value = ", isFilesExist[i].ToString(), CurrentClassName, 3);// showMessagesLevel);
                    
                    if (isFilesExist[i]) filesToDo[i] = (int)WhatNeedDoWithFiles.ReadFirst;//if file exist we will read (open) it
                    else
                    {
                        filesToDo[i] = (int)WhatNeedDoWithFiles.StopProcessing;//if file does not exist we would like to return to OpenForm, but if resultFile does not exist, we will create it
                        _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " file does not exist we would like to return to OpenForm, but if resultFile does not exist, we will create it ==> filesToDo[i] value = ", filesToDo[i].ToString(), CurrentClassName, 3);// showMessagesLevel);
                        iBreakpoint = (i == resultFileNumber) ? -1 : i;
                        _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " resultFile exist ==> iBreakpoint = ", iBreakpoint.ToString(), CurrentClassName, 3);// showMessagesLevel);
                        break;
                    } 
                }

                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " Cycle for ended, check the iBreakpoint = ", iBreakpoint.ToString(), CurrentClassName, 3);// showMessagesLevel);

                if (iBreakpoint >= 0) //That's not the way to do things - you must select both source files
                {
                    _messageService.ShowExclamation("The source file does not exist, please select it!");//return to OpenForm for all necessary file selection
                    
                }
                else //all files are existed and we are ready to open them
                {
                    if (isFilesExist[resultFileNumber]) //if resultFile does not exist, we will create it in the path of the first selected file (with 0 index)
                    {
                        filesToDo[resultFileNumber] = (int)WhatNeedDoWithFiles.ReadFirst;//the selected result file needs in processing
                        _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " resultFile exist ==> iBreakpoint = ", iBreakpoint.ToString(), CurrentClassName, 3);// showMessagesLevel);
                    }
                    else
                    {
                        _messageService.ShowExclamation("The Result file does not exist, it will be created now!");
                        //!!! - If the result file is not exist we need to create it here               
                        
                        filesPath[resultFileNumber] = _manager.CreateFile(filesPath[0], resultFileName);
                        //вернуть ситуацию, что файл уже есть и принять решение, что с этим делать - перезаписать или идти открывать существующий

                        if (isFilesExist[resultFileNumber])
                        {
                             
                            iBreakpoint = -1;//Breakpoint reset - the result file created
                            filesToDo[resultFileNumber] = (int)WhatNeedDoWithFiles.ReadFirst;//the created result file needs in processing
                            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " Creation of resultFile ended, WhatNeedDoWithFiles.ReadFirst = ", WhatNeedDoWithFiles.ReadFirst.ToString(), CurrentClassName, 3);// showMessagesLevel);
                        }
                    }
                    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " If ended, check the iBreakpoint = ", iBreakpoint.ToString(), CurrentClassName, 3);// showMessagesLevel);
                    filesContent = _manager.GetContents(filesPath, filesToDo);
                    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " filesContent[] value = ", filesContent, CurrentClassName, showMessagesLevel);
                    counts = _manager.GetSymbolCounts(filesContent);
                    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " counts[] value = ", counts[0].ToString(), CurrentClassName, showMessagesLevel);
                    _view.SetFilesContent(filesPath, filesContent, filesToDo);
                    _view.SetSymbolCount(counts, filesToDo);
                }

                


                        
            }            
            catch (Exception ex)
            {
                _messageService.ShowError(ex.Message);
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
