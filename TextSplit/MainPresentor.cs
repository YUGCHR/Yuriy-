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
        private readonly ILogFileMessages _logs;        
        private readonly ILoadTextToDataBase _load;

        private bool wasEnglishContentChange = false;
        readonly private string strCRLF;
        readonly private int filesQuantity;
        readonly private int filesQuantityPlus;
        readonly private int textFieldsQuantity;
        readonly private int iBreakpointManager;
        readonly private int resultFileNumber;
        readonly private int showMessagesLevel;

        private string resultFileName;        

        private int[] filesToDo;
        private int[] counts;
        private string[] filesPath;
        private string[] filesContent;        

        public MainPresentor(ITextSplitForm view, ITextSplitOpenForm open, IFileManager manager, IMessageService service, ILogFileMessages logs, ILoadTextToDataBase load)
        {
            _view = view;
            //_open = open;//потом - для работы с формами отдельный класс и там все события нажатия кнопок, а текстовые поля - в отдельном классе?
            _manager = manager;
            _messageService = service;
            _logs = logs;
            _load = load;

            resultFileNumber = Declaration.ResultFileNumber;
            filesQuantity = Declaration.FilesQuantity;
            filesQuantityPlus = Declaration.FilesQuantityPlus;
            textFieldsQuantity = Declaration.TextFieldsQuantity;
            iBreakpointManager = Declaration.IBreakpointManager;
            showMessagesLevel = Declaration.ShowMessagesLevel;
            strCRLF = Declaration.StrCRLF;

            string mainStart = "******************************************************************************************************************************************* \r\n";//Log-file separator
            _messageService.ShowTrace(mainStart + MethodBase.GetCurrentMethod().ToString(), " Started", CurrentClassName, showMessagesLevel);

            filesToDo = new int[filesQuantityPlus];
            counts = new int[filesQuantity];
            //_open.SetSymbolCount(counts, filesToDo);//Саид, зачем ты здесь?

            filesPath = new string[filesQuantity];
            filesContent = new string[filesQuantity];

            _view.OpenTextSplitOpenForm += new EventHandler(_view_OpenTextSplitOpenForm);            
            _view.TextSplitFormClosing += new EventHandler<FormClosingEventArgs>(_view_TextSplitFormClosing);
            // All EventHandlers for _open (TextSplitOpenForm) are located inside _view_OpenTextSplitOpenForm method

        }

        private void _open_LoadEnglishToDataBase(object sender, EventArgs e)
        {   // обобщить метод для любого языка
            // разрешать запускать только после сохранения текста - гасить кнопку или менять название - Save на dB Load
            int ID_Language = 0;//работа с английской таблицей базы - может, надо убрать, по ToDo ясно, с кем работаем (но это не точно)
            filesToDo = _open.GetFilesToDo();
            filesContent = _open.GetFilesContent();
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + strCRLF + "filesToDo[English] must be 4 here ==> ", filesToDo[(int)TableLanguagesContent.English].ToString(), CurrentClassName, showMessagesLevel);
            if (filesToDo[(int)TableLanguagesContent.English] == (int)WhatNeedDoWithFiles.CountSymbols)//check is English content filled in the filesContent - CountSymbols was done
            {
                //ID_Language = -1; //временный костыль для заполнения таблицы, потом перевесить в настройки главной формы, для обычной работы пока что просто закомментировать
                int insertPortionResult = _load.PortionTextForDataBase(filesContent, filesToDo, ID_Language); // - возможно, вызывается 2 раза, проверить и найти почему
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + strCRLF + " insertPortionResult ==> ", insertPortionResult.ToString(), CurrentClassName, showMessagesLevel);
            }            
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
            _open.OpenFileClick += new EventHandler(_open_OpenFileClick);
            _open.ContentChanged += new EventHandler(_open_ContentChanged);
            _open.SaveFileClick += new EventHandler(_open_SaveFileClick);
            _open.LoadEnglishToDataBase += _open_LoadEnglishToDataBase;

            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "openForm will start now", CurrentClassName, showMessagesLevel);
            openForm.Show();
        }        
        
        private void _open_OpenFileClick(object sender, EventArgs e)
        {
            try
            {
                filesPath = _open.GetFilesPath();
                filesToDo = _open.GetFilesToDo();
                
                int numberOfPerformedActionElement = isFilesExistCheckAndOpen();//получили номер элемента, с которым произведено действие
                
                if (filesToDo[numberOfPerformedActionElement] == (int)WhatNeedDoWithFiles.ContinueProcessing)//добавить else WittingIncomplete)//если в элементе сказано, что все хорошо, отправляем данные в форму
                {

                    _open.SetFileContent(filesContent, numberOfPerformedActionElement);
                    //подходящий момент поменять название на кнопках - поставить Save вместо Open
                    //передать номер названия кнопки 0,0 - Open English File, 1, 0 - Save English File, 1, 0 - Open Russian File, 1, 1 - Save Russian File
                    //butMfOpenSaveLanguageNames[0, 0] = Open English File [(int)MfButtonPlaceTexts.EnglishFile, (int)MfButtonNameTexts.OpenFile]
                    //butMfOpenSaveLanguageNames[0, 1] = Save English File [(int)MfButtonPlaceTexts.EnglishFile, (int)MfButtonNameTexts.SaveFile]
                    //butMfOpenSaveLanguageNames[1, 0] = Open Russian File [(int)MfButtonPlaceTexts.RussianFile, (int)MfButtonNameTexts.OpenFile]
                    //butMfOpenSaveLanguageNames[1, 1] = Save Russian File [(int)MfButtonPlaceTexts.RussianFile, (int)MfButtonNameTexts.SaveFile]
                    int mfButtonPlace = numberOfPerformedActionElement; // Place of the button which was pressed (English or Russian)
                    int mfButtonText = 1; //change ButtonName from Open to Save
                    bool mfButtonEnableFlag = false; //кнопка серая - текст только загружен и не изменялся

                    string butMfOpenSaveLanguageName = _open.GetbutMfOpenSaveLanguageNames(mfButtonPlace, mfButtonText);//непонятно зачем это делается, но может потом пригодится
                    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "fetched pressed button place and name ==> " + butMfOpenSaveLanguageName, CurrentClassName, showMessagesLevel);

                    //вызывается метод смены названия на кнопке и меняется на нужное
                    //и еще (сразу в первый раз)/(а потом как?) сделать кнопку Save серой - скорее всего в ChangeContent
                    int checkOnButtonTextResult = mfButtonPlace + mfButtonText;
                    int changeOnButtonTextResult = _open.ChangeOnButtonText(mfButtonPlace, mfButtonText, mfButtonEnableFlag);
                    //if (changeOnButtonTextResult == checkOnButtonTextResult) - allright
                    
                    _open.SetFileContent(filesContent, numberOfPerformedActionElement);
                    filesToDo[numberOfPerformedActionElement] = (int)WhatNeedDoWithFiles.CountSymbols;//передаем указание посчитать символы и это является подтверждением, что файл успешно открыли и записали в форму
                    counts[numberOfPerformedActionElement] = _manager.GetSymbolCounts(filesContent, numberOfPerformedActionElement);
                    _open.SetFilesToDo(filesToDo);//передали значение "посчитать символы" в управляющий массив, как указание, что открытие и запись в textbox прошла успешно
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
            for (int i = 0; i < textFieldsQuantity; i++)
            {
                int theAffectedElementNumber = filesToDo[i];
                
                if (theAffectedElementNumber == (int)WhatNeedDoWithFiles.ReadFileFirst)
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
        
        void _open_ContentChanged(object sender, EventArgs e)
        {//где-то тут сделать кнопку Save активной
            filesToDo = _open.GetFilesToDo();
            filesContent = _open.GetFilesContent();            
            //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " filesContent - ", filesContent, CurrentClassName, 3);
            for (int i = 0; i < textFieldsQuantity; i++)
            {
                if (filesToDo[i] == (int)WhatNeedDoWithFiles.ContentChanged)
                {
                    //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " filesToDo[i] - ", filesToDo[i].ToString(), CurrentClassName, 3); 
                    int mfButtonPlace = i; // Place of the button the same as changed field (English or Russian)
                    int mfButtonText = 1; // ButtonName leave Save
                    bool mfButtonEnableFlag = true; //активировать кнопку - текст изменился и его можно сохранить                    
                    int checkOnButtonTextResult = mfButtonPlace + mfButtonText; // типа, контроль
                    int changeOnButtonTextResult = _open.ChangeOnButtonText(mfButtonPlace, mfButtonText, mfButtonEnableFlag);
                    //if (changeOnButtonTextResult == checkOnButtonTextResult) - allright

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

        private void _open_SaveFileClick(object sender, EventArgs e)//сделать return с кодом возврата при удачном сохранении, поставить после цикла "сохранить не удалось"
        {//где-то там, где вызывают, определить, первый раз сохраняют или нет - по признаку FileWasSavedDontAsk и поставить указание - либо SaveFileFirst, либо SaveFile
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Start", CurrentClassName, showMessagesLevel);
            try
            {
                filesToDo = _open.GetFilesToDo();
                filesPath = _open.GetFilesPath();
                filesContent = _open.GetFilesContent();

                for (int i = 0; i < textFieldsQuantity; i++)
                {
                    if (filesToDo[i] == (int)WhatNeedDoWithFiles.SaveFileFirst)
                    {
                        //сообщить, что файл будет перезаписан и если не копия, то пусть идет и делает сам копию, то шо нефиг (после первого сохранения больше не спрашивать)
                        
                        _manager.SaveContents(filesContent, filesPath, filesToDo);
                        _messageService.ShowMessage("File saved sucessfully!");
                        //тут сделать кнопку Save серой, код ниже допилить
                        int mfButtonPlace = i; // Place of the button the same as changed field (English or Russian)
                        int mfButtonText = 1; // ButtonName leave Save
                        bool mfButtonEnableFlag = true; //активировать кнопку - текст изменился и его можно сохранить                    
                        int checkOnButtonTextResult = mfButtonPlace + mfButtonText; // типа, контроль
                        int changeOnButtonTextResult = _open.ChangeOnButtonText(mfButtonPlace, mfButtonText, mfButtonEnableFlag);
                        //if (changeOnButtonTextResult == checkOnButtonTextResult) - allright
                        //присвоить filesToDo[i] == (int)WhatNeedDoWithFiles.FileWasSavedDontAsk
                        _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "чего-то напечатать", CurrentClassName, showMessagesLevel);
                    }

                    //добавить if с проверкой SaveFile, вынести весь блок сохранения в один метод и вызывать его отсюда и из SaveFileFirst
                }
            }
            catch (Exception ex)
            {
                _messageService.ShowError(ex.Message);
            }
        }

        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }
    }
}