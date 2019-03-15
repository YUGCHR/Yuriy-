using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using TextSplitLibrary;
using System.Windows.Forms;

namespace TextSplit
{
    public interface ITextSplitOpenForm
    {
        int SetFileContent(int theAffectedElementNumber);
        int SetSymbolCount(int i);
        string GetbutMfOpenSaveLanguageNames(int mfButtonPlaceTexts, int mfButtonNameTexts);
        int ChangeOnButtonText(int mfButtonPlace, int mfButtonText, bool mfButtonEnableFlag);

        event EventHandler ContentChanged;
        event EventHandler OpenFileClick;
        event EventHandler SaveFileClick;
        event EventHandler LoadEnglishToDataBase;
        
        //event EventHandler<FormClosingEventArgs> TextSplitOpenFormClosing;
    }

    public partial class TextSplitOpenForm : Form, ITextSplitOpenForm
    {
        private readonly IAllBookData _book;
        private readonly IMessageService _messageService;

        private Label[] lblSymbolsCount;
        private TextBox[] txtBoxFilesPath;
        private TextBox[] txtBoxFilesContent;
        private Button[] butMF;
        private string[,] butMfOpenSaveLanguageNames;

        readonly private int filesQuantity;
        readonly private int filesQuantityPlus;
        readonly private int textFieldsQuantity;
        readonly private int showMessagesLevel;
        readonly private string strCRLF;
        readonly private int iBreakpointManager;        

        public event EventHandler ContentChanged;
        public event EventHandler OpenFileClick;
        public event EventHandler SaveFileClick;
        public event EventHandler LoadEnglishToDataBase;
        
        //public event EventHandler<FormClosingEventArgs> TextSplitOpenFormClosing;

        public enum FormFieldsNames : int { fld0EnglishContent = 0, fld1RussianContent = 1, fld2ResultContent = 2 };
        public enum OpenFormFieldNames : int { fld0EnglishFilePath = 0, fld1RussianFilePath = 1, fld2ResultFilePath = 2, fld2CreateResultFileName = 3 };
        public enum OpenFormButtonNames : int { butMfLeftTextBox = 0, butMfRightTextBox = 1, butOpenResultFile = 2 };

        public TextSplitOpenForm(IMessageService service, IAllBookData book)
        {
            _book = book;
            _messageService = service;
            
            filesQuantity = Declaration.FilesQuantity;
            filesQuantityPlus = Declaration.FilesQuantityPlus;
            textFieldsQuantity = Declaration.TextFieldsQuantity;
            iBreakpointManager = Declaration.IBreakpointManager;
            showMessagesLevel = Declaration.ShowMessagesLevel;
            strCRLF = Declaration.StrCRLF;

            InitializeComponent();
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "InitializeComponent in TextSplitOpenForm", CurrentClassName, showMessagesLevel);

            lblSymbolsCount = new Label[] { lblSymbolCount1, lblSymbolCount2 };//, lblSymbolCount3 };
            txtBoxFilesPath = new TextBox[] { fld0EnglishFilePath, fld1RussianFilePath };//, fld2ResultFilePath  };
            txtBoxFilesContent = new TextBox[] { fld0EnglishContent, fld1RussianContent };//, fld2ResultFileContent  };
            butMF = new Button[] { butMfLeftTextBox, butMfRightTextBox };

            butMfOpenSaveLanguageNames = new string[,] { { "Open English File", "Save English File" }, { "Open Russian File", "Save Russian File" } };
            // English - butMfLeftTextBox, Russian - butMfRightTextBox
            //butMfOpenSaveLanguageNames[0, 0] = Open English File [(int)MfButtonPlaceTexts.EnglishFile, (int)MfButtonNameTexts.OpenFile]
            //butMfOpenSaveLanguageNames[0, 1] = Save English File [(int)MfButtonPlaceTexts.EnglishFile, (int)MfButtonNameTexts.SaveFile]
            //butMfOpenSaveLanguageNames[1, 0] = Open Russian File [(int)MfButtonPlaceTexts.RussianFile, (int)MfButtonNameTexts.OpenFile]
            //butMfOpenSaveLanguageNames[1, 1] = Save Russian File [(int)MfButtonPlaceTexts.RussianFile, (int)MfButtonNameTexts.SaveFile]
            
            butMF[(int)MfButtonPlaceTexts.EnglishFile].Click += butMfBox_Click;
            butMF[(int)MfButtonPlaceTexts.RussianFile].Click += butMfBox_Click;
            butEnglishToDataBase.Click += ButEnglishToDataBase_Click;
            //butSelectResultFile.Click += butSelectFile_Click;

            txtBoxFilesContent[(int)MfButtonPlaceTexts.EnglishFile].TextChanged += fldContent_TextChanged;//потом поменять English/Russian на Left/Right
            txtBoxFilesContent[(int)MfButtonPlaceTexts.RussianFile].TextChanged += fldContent_TextChanged;
            //fld2ResultContent.TextChanged += fldContent_TextChanged;

            numEnglishFont.SelectedIndexChanged += numEnglishFont_SelectedIndexChanged;

            //butCreateResultFile.Click += butSelectEnglishFile_Click; it needs to clear up
            //fld2CreateResultFileName - result file name field

            FormClosing += TextSplitOpenForm_FormClosing;

            butMF[(int)MfButtonPlaceTexts.EnglishFile].Text = butMfOpenSaveLanguageNames[0, 0];//первоначальная установка названий кнопок на старте
            butMF[(int)MfButtonPlaceTexts.RussianFile].Text = butMfOpenSaveLanguageNames[1, 0];//потом заменить вызовом метода с нужными параметрами
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), 
                "butMfEnglishBox ==> " + butMfLeftTextBox.Text + strCRLF + 
                "butMfRussianBox ==> " + butMfRightTextBox.Text, CurrentClassName, showMessagesLevel);
            
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), 
                " butMfOpenSaveLanguageNames[0,0] = " + butMfOpenSaveLanguageNames[(int)MfButtonPlaceTexts.EnglishFile, (int)MfButtonNameTexts.OpenFile] + strCRLF +
                " butMfOpenSaveLanguageNames[0, 1] = " + butMfOpenSaveLanguageNames[(int)MfButtonPlaceTexts.EnglishFile, (int)MfButtonNameTexts.SaveFile] + strCRLF +
                " butMfOpenSaveLanguageNames[1, 0] = " + butMfOpenSaveLanguageNames[(int)MfButtonPlaceTexts.RussianFile, (int)MfButtonNameTexts.OpenFile] + strCRLF +
                " butMfOpenSaveLanguageNames[1, 1] = " + butMfOpenSaveLanguageNames[(int)MfButtonPlaceTexts.RussianFile, (int)MfButtonNameTexts.SaveFile], 
                CurrentClassName, showMessagesLevel);
        }

        private void ButEnglishToDataBase_Click(object sender, EventArgs e)//выбирать кнопки из массива
        {            
            if (LoadEnglishToDataBase != null) LoadEnglishToDataBase(this, EventArgs.Empty);
        }

        public string GetbutMfOpenSaveLanguageNames(int mfButtonPlaceTexts, int mfButtonNameTexts)//передавать i и j нужного элемента и получать один нужный, а не весь массив (или еще как)
        {
            //butMfOpenSaveLanguageNames[0, 0] = Open English File [(int)MfButtonPlaceTexts.EnglishFile, (int)MfButtonNameTexts.OpenFile]
            //butMfOpenSaveLanguageNames[0, 1] = Save English File [(int)MfButtonPlaceTexts.EnglishFile, (int)MfButtonNameTexts.SaveFile]
            //butMfOpenSaveLanguageNames[1, 0] = Open Russian File [(int)MfButtonPlaceTexts.RussianFile, (int)MfButtonNameTexts.OpenFile]
            //butMfOpenSaveLanguageNames[1, 1] = Save Russian File [(int)MfButtonPlaceTexts.RussianFile, (int)MfButtonNameTexts.SaveFile]
            return butMfOpenSaveLanguageNames[mfButtonPlaceTexts, mfButtonNameTexts];
        }

        public int ChangeOnButtonText(int mfButtonPlace, int mfButtonText, bool mfButtonEnableFlag)
        {
            if (mfButtonPlace < textFieldsQuantity && mfButtonText < textFieldsQuantity)
            {
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(),
                    "on the button " + butMF[mfButtonPlace].Name + strCRLF +
                    "the text is " + butMF[mfButtonPlace].Text + strCRLF +
                    " set name " + butMfOpenSaveLanguageNames[mfButtonPlace, mfButtonText] + strCRLF +
                    " enabled = " + mfButtonEnableFlag, CurrentClassName, showMessagesLevel);

                if (butMF[mfButtonPlace].Text != butMfOpenSaveLanguageNames[mfButtonPlace, mfButtonText])//если название надо менять, то меняем
                {
                    butMF[mfButtonPlace].Text = butMfOpenSaveLanguageNames[mfButtonPlace, mfButtonText];//установили требуемое название на нужной кнопке
                }
                butMF[mfButtonPlace].Enabled = mfButtonEnableFlag;//сделали кнопку Enabled или нет

                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(),
                    "on the button " + butMF[mfButtonPlace].Name + strCRLF +
                    " set name " + butMfOpenSaveLanguageNames[mfButtonPlace, mfButtonText] + strCRLF +
                    " enabled = " + mfButtonEnableFlag, CurrentClassName, showMessagesLevel);

                return mfButtonPlace + mfButtonText;
            }
            else
            {
                _messageService.ShowExclamation("Your requests exceed reasonable limits!");                
                return mfButtonPlace + mfButtonText;
            }
        }

        private void TextSplitOpenForm_FormClosing(object sender, FormClosingEventArgs e)//it needs to clear up
        {            
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Start", CurrentClassName, showMessagesLevel);
            //TextSplitOpenFormClosing(this, e);            
            //e.Cancel = wasEnglishContentChange;
            System.Text.StringBuilder messageBoxCS = new System.Text.StringBuilder();
            messageBoxCS.AppendFormat("{0} = {1}", "CloseReason", e.CloseReason);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "Cancel", e.Cancel);
            messageBoxCS.AppendLine();
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + "FormClosing Event", messageBoxCS.ToString(), CurrentClassName, showMessagesLevel);            
        }

        private void butMfBox_Click(object sender, EventArgs e)     // алгоритм выбора действия по нажатым кнопкам
        {                                                           // 
            Button button = sender as Button;                       // кнопка нажата
            string ButtonName = button.Name;                        // получили место нажатой кнопки
            string ButtonText = button.Text;                        // получили имя нажатой кнопки 

            ButClickLabelNow(ButtonName, ButtonText);      // вызвали обработчик
        }

        private void ButClickLabelNow(string buttonName, string buttonText)
        {                                                           // алгоритм выбора действия по нажатым кнопкам
            string currentOpenFormButtonPlace = "";                 // 
            string currentOpenFormButtonName = "";                  // 
            
            var listOpenFormButtonMethods = new List<Action<int>>();
            Action<int> currentButtonMethod;
            currentButtonMethod = PathForOpenFile;// listOpenFormButtonMethods[0]
            listOpenFormButtonMethods.Add(currentButtonMethod);
            currentButtonMethod = SaveFileFromTextBox;// listOpenFormButtonMethods[1]
            listOpenFormButtonMethods.Add(currentButtonMethod);
            currentButtonMethod = Method3;
            listOpenFormButtonMethods.Add(currentButtonMethod);

            for (int i = 0; i < textFieldsQuantity; i++)//0 - Left (English), 1 - Right (Russian), last field is not used yet
            {                                                         // цикл 1 начат                
                currentOpenFormButtonPlace = Enum.GetNames(typeof(OpenFormButtonNames))[i];// проход 1 цикла 1 - выбрали левая для места кнопки

                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " i = " + i.ToString() +
                            strCRLF + " currentOpenFormButtonPlace ==> " + currentOpenFormButtonPlace, CurrentClassName, showMessagesLevel);

                if (buttonName == currentOpenFormButtonPlace)         // если совпало с местом нажатой кнопки - начали цикл 2, если нет - проход 2 цикла 1 - правая (все аналогично)
                { 
                    for (int j = 0; j < textFieldsQuantity; j++)//нужно другое значение вместо textFieldsQuantity - количество значений имени кнопки и поставить =<
                    {                                                 // цикл 2 начат                
                        currentOpenFormButtonName = butMfOpenSaveLanguageNames[i, j]; // проход 1 цикла 2 - выбрали имя Open для проверки имени кнопки 
                                                                                 // проход 2 цикла 2 - выбрали имя Save для проверки имени кнопки 
                                                                                 // если совпало с именем нажатой кнопки - вызвали метод сохранения файла, если нет - беда... 
                        _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " i = " + i.ToString() + " / j = " + j.ToString() + 
                            strCRLF + " currentOpenFormButtonName ==> " + currentOpenFormButtonName, CurrentClassName, showMessagesLevel);

                        if (buttonText == currentOpenFormButtonName)  // если совпало с именем нажатой кнопки - вызвали метод открытия файла, если нет - проход 2 цикла 2 - похоже, надо делать Save
                        {
                            listOpenFormButtonMethods[j](i);//только тут мы знаем i - чтобы не передавать его в Main обрабатываем его тут и заносим в FilesToDo[i] или в FilesToSave[i]
                        }
                    }
                }
            }
        }

        private void PathForOpenFile(int i) // listOpenFormButtonMethods[0] - нажата одна из кнопок и на ней было написано Open, i хранит место кнопки (0 - левая)
        {
            string currentFilePath = "";
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " PathForOpenFile has fetched i = " + i.ToString(), CurrentClassName, showMessagesLevel);

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Text files|*.txt|All files|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                currentFilePath = dlg.FileName;
                _book.SetFilePath(currentFilePath, i);
                txtBoxFilesPath[i].Text = currentFilePath; //записали путь и имя файла в поле над текстом файла
                _book.SetFileToDo((int)WhatNeedDoWithFiles.ReadFileFirst, i); // записали в нужный элемент ToDo значение, что надо открывать файл
            }
            statusBottomLabel.Text = Enum.GetNames(typeof(OpenFormProgressStatusMessages))[i] + " - " + currentFilePath;//Set the short type of current action in the status bar            
            if (OpenFileClick != null) OpenFileClick(this, EventArgs.Empty);//вызываем Main (сказать поменять надписи на кнопках - показать погасший Save)
        }

        private void SaveFileFromTextBox(int i) // listOpenFormButtonMethods[1] - нажата одна из кнопок и на ней было написано Save, i хранит место кнопки (0 - левая)
        {//определить, первый раз сохраняют или нет - по признаку FileWasSavedSuccessfully и поставить указание - либо SaveFileFirst, либо SaveFile
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " SaveFileFromTextBox has fetched i = " + i.ToString(), CurrentClassName, showMessagesLevel);            
            if (_book.GetFileToSave(i) == (int)WhatNeedSaveFiles.FileSavedSuccessfully)
            {
                int test = _book.SetFileToSave((int)WhatNeedSaveFiles.SaveFile, i);//молча сохраняем файл
            }
            else
            {
                int test = _book.SetFileToSave((int)WhatNeedSaveFiles.SaveFileFirst, i);//спрашиваем о перезаписи, но в Main - хотя можно и тут                
            }
            statusBottomLabel.Text = Enum.GetNames(typeof(OpenFormProgressStatusMessages))[i] + " - " + _book.GetFilePath(i);//Set the short type of current action in the status bar
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " FilesToSave[i] = " + _book.GetFileToSave(i).ToString(), CurrentClassName, showMessagesLevel);
            if (SaveFileClick != null) SaveFileClick(this, EventArgs.Empty);
        }
        
        private void Method3(int i)
        { }   

        private void fldContent_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string TextBoxName = textBox.Name;
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Text Changed in TextBox - " + TextBoxName, CurrentClassName, showMessagesLevel);            

            for (int i = 0; i < textFieldsQuantity; i++)
            {
                string currentFormFieldName = Enum.GetNames(typeof(FormFieldsNames))[i];
                if (TextBoxName == currentFormFieldName)
                {
                    int setToDoResult = _book.SetFileToDo((int)WhatNeedDoWithFiles.ContentChanged, i); 
                    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " FilesToDo[i] - " + _book.GetFileToDo(i).ToString(), CurrentClassName, showMessagesLevel);
                    int setContentResult = _book.SetFileContent(textBox.Text, i);
                }
            }
            
            if (ContentChanged != null) ContentChanged(this, EventArgs.Empty);
        }

        public int SetFileContent(int theAffectedElementNumber)// сделать присваивание массиву в цикле и добавить коды возврата
        {
            txtBoxFilesContent[theAffectedElementNumber].Text = _book.GetFileContent(theAffectedElementNumber);
            numEnglishFont_FirstSet(theAffectedElementNumber);
            return (int)WhatDoSaveResults.Successfully;
        }

        public int SetSymbolCount(int i)
        {
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), 
                " SetSymbolCount fetched i = " + i.ToString() + strCRLF +
                "_book.GetFileToDo(i) (must be 4) = " + _book.GetFileToDo(i).ToString(), CurrentClassName, showMessagesLevel);

            if (_book.GetFileToDo(i) == (int)WhatNeedDoWithFiles.CountSymbols)
            {
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), 
                    "lblSymbolsCount[i] ==> " + lblSymbolsCount[i].Name + strCRLF +
                    "SymbolsCount(i) = " + _book.GetSymbolsCount(i).ToString(), CurrentClassName, showMessagesLevel);
                lblSymbolsCount[i].Text = _book.GetSymbolsCount(i);
                return 0;
            }
            return -1;
        }

        private void numEnglishFont_FirstSet(int theAffectedElementNumber)
        {
            var font = txtBoxFilesContent[theAffectedElementNumber].Font;            
            numEnglishFont.SelectedIndex = Convert.ToInt32(font.Size);
        }

        private void numEnglishFont_SelectedIndexChanged(object sender, EventArgs e)//переделать в поля из массива
        {
            {
                fld0EnglishContent.Font = new Font("Tahoma", numEnglishFont.SelectedIndex);
            }
        }        

        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
		
	}
}
