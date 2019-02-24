﻿using System;
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
            string mainStart = "******************************************************************************************************************************************* \r\n";
            _messageService.ShowTrace(mainStart + MethodBase.GetCurrentMethod().ToString(), " Started", CurrentClassName, showMessagesLevel);

            filesToDo = new int[filesQuantity];
            counts = new int[filesQuantity];
            _view.SetSymbolCount(counts, filesToDo);//move?

            filesPath = new string[filesQuantity];
            filesContent = new string[filesQuantity];
            //_view.ManageFilesContent(filesPath, filesContent, filesToDo);//move?

            _view.OpenTextSplitOpenForm += new EventHandler(_view_OpenTextSplitOpenForm);
            //_open.AllOpenFilesClick += new EventHandler(_open_FilesOpenClick);            
            _view.EnglishContentChanged += new EventHandler (_view_EnglishContentChanged);            
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
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Start", CurrentClassName, 3);//showMessagesLevel);
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
                //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Start", CurrentClassName, 3);//showMessagesLevel);                
                filesPath = _open.GetFilesPath();
                //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " filesPath value = ", filesPath, CurrentClassName, 3);//showMessagesLevel);
                int[] counts = new int[filesQuantity];
                wasEnglishContentChange = false;
                bool[] isFilesExist = new bool[filesQuantity];
                isFilesExist = _manager.IsFilesExist(filesPath);
                
                for (int i = 0; i < filesQuantity; i++)
                {
                    //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " isFilesExist[i] = ", isFilesExist[i].ToString(), CurrentClassName, 3);// showMessagesLevel);

                    if (isFilesExist[i])
                    {
                        filesToDo[i] = 1;
                        //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " filesToDo[i] = ", filesToDo[i].ToString(), CurrentClassName, 3);// showMessagesLevel);
                    } 
                    else
                    {
                        filesToDo[i] = 0;
                        //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " ELSE filesToDo[i] = ", filesToDo[i].ToString(), CurrentClassName, 3);// showMessagesLevel);
                        if (i<2) _messageService.ShowExclamation("Selected file does not exist!");//If the result file is not exist we need to create it here!
                    }
                }

                filesContent = _manager.GetContents(filesPath, filesToDo);
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " filesContent[] value = ", filesContent, CurrentClassName, 3);//showMessagesLevel);

                //counts = _manager.GetSymbolCounts(_manager.GetContents(filesPath, filesToDo));

                //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " counts[] value = ", counts.ToString(), CurrentClassName, 3);//showMessagesLevel);
                _view.SetSymbolCount(counts, filesToDo);
                
            }            
            catch (Exception ex)
            {
                _messageService.ShowError(ex.Message);
            }
        }

        void _view_EnglishContentChanged(object sender, EventArgs e)
        {
            //string[] contents = _view.FilesContent;                        
            //int[] counts = _manager.GetSymbolCounts(contents);
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + "wasEnglishContentChange", wasEnglishContentChange.ToString(), CurrentClassName, showMessagesLevel);
            //_view.SetSymbolCount(counts, _view.FilesToDo);
            wasEnglishContentChange = true;//we need also the array here
        }
        
        public static string CurrentClassName
            {
                get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
            }
    }
}
