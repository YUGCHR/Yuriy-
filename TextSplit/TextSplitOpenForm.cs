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
        string[] GetFilesContent();
        string[] GetFilesPath();
        int[] GetFilesToDo();
        void SetFilesToDo(int[] filesToDo);
        void SetFileContent(string[] filesContent, int theAffectedElementNumber);
        void SetSymbolCount(int[] counts, int[] filesToDo);
        string GetbutMfOpenSaveLanguageNames(int mfButtonPlaceTexts, int mfButtonNameTexts);
        int ChangeOnButtonText(int mfButtonPlace, int mfButtonText, bool mfButtonEnableFlag);

        event EventHandler ContentChanged;
        event EventHandler OpenFileClick;
        event EventHandler SaveFileClick;
        event EventHandler LoadEnglishToDataBase;
        //event EventHandler FormOpenClick;
        //event EventHandler<FormClosingEventArgs> TextSplitOpenFormClosing;
    }

    public partial class TextSplitOpenForm : Form, ITextSplitOpenForm
    {
        private readonly IMessageService _messageService;

        private string[] FilesPath;// { get; set; }        
        private int[] FilesToDo;
        private string[] FilesContent;

        private Label[] lblSymbolsCount;
        private TextBox[] txtboxFilesPath;
        private Button[] butMF;
        private string[,] butMfOpenSaveLanguageNames;

        readonly private int filesQuantity;
        readonly private int filesQuantityPlus;
        readonly private int textFieldsQuantity;
        readonly private int showMessagesLevel;
        readonly private string strCRLF;
        readonly private int iBreakpointManager;

        public enum MfButtonPlaceTexts : int { EnglishFile = 0, RussianFile = 1, Reserved = 2 };
        public enum MfButtonNameTexts : int { OpenFile = 0, SaveFile = 1, Reserved = 2 };
        

        public event EventHandler ContentChanged;
        public event EventHandler OpenFileClick;
        public event EventHandler SaveFileClick;
        public event EventHandler LoadEnglishToDataBase;
        //public event EventHandler FormOpenClick;
        //public event EventHandler<FormClosingEventArgs> TextSplitOpenFormClosing;
        
        public TextSplitOpenForm(IMessageService service)
        {            
            _messageService = service;
            
            filesQuantity = Declaration.FilesQuantity;
            filesQuantityPlus = Declaration.FilesQuantityPlus;
            textFieldsQuantity = Declaration.TextFieldsQuantity;
            iBreakpointManager = Declaration.IBreakpointManager;
            showMessagesLevel = Declaration.ShowMessagesLevel;
            strCRLF = Declaration.StrCRLF;

            InitializeComponent();
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "InitializeComponent in TextSplitOpenForm", CurrentClassName, showMessagesLevel);

            FilesToDo = new int[filesQuantityPlus];
            FilesPath = new string[filesQuantity];
            FilesContent = new string[filesQuantity];

            lblSymbolsCount = new Label[] { lblSymbolCount1, lblSymbolCount2 };//, lblSymbolCount3 };
            txtboxFilesPath = new TextBox[] { fld0EnglishFilePath, fld1RussianFilePath };//, fld2ResultFilePath  };
            butMF = new Button[] { butMfLeftTextBox, butMfRightTextBox };

            butMfOpenSaveLanguageNames = new string[,] { { "Open English File", "Save English File" }, { "Open Russian File", "Save Russian File" } };
            // English - butMfLeftTextBox, Russian - butMfRightTextBox
            //butMfOpenSaveLanguageNames[0, 0] = Open English File [(int)MfButtonPlaceTexts.EnglishFile, (int)MfButtonNameTexts.OpenFile]
            //butMfOpenSaveLanguageNames[0, 1] = Save English File [(int)MfButtonPlaceTexts.EnglishFile, (int)MfButtonNameTexts.SaveFile]
            //butMfOpenSaveLanguageNames[1, 0] = Open Russian File [(int)MfButtonPlaceTexts.RussianFile, (int)MfButtonNameTexts.OpenFile]
            //butMfOpenSaveLanguageNames[1, 1] = Save Russian File [(int)MfButtonPlaceTexts.RussianFile, (int)MfButtonNameTexts.SaveFile]

            

            butAllFilesOpen.Click += new EventHandler (butAllFilesOpen_Click);
            butMF[(int)MfButtonPlaceTexts.EnglishFile].Click += butMfBox_Click;
            butMF[(int)MfButtonPlaceTexts.RussianFile].Click += butMfBox_Click;
            butEnglishToDataBase.Click += ButEnglishToDataBase_Click;
            //butSelectResultFile.Click += butSelectFile_Click;

            fld0EnglishContent.TextChanged += fldContent_TextChanged;
            fld1RussianContent.TextChanged += fldContent_TextChanged;
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

        private void ButEnglishToDataBase_Click(object sender, EventArgs e)
        {
            //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " LoadEnglishToDataBase = " + LoadEnglishToDataBase.ToString(), CurrentClassName, showMessagesLevel);
            if (LoadEnglishToDataBase != null) LoadEnglishToDataBase(this, EventArgs.Empty);
        }

        public string[] GetFilesContent()
        {
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " FilesPath[] = ", FilesPath, CurrentClassName, showMessagesLevel);
            return FilesContent;
        }

        public string[] GetFilesPath()
        {            
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " FilesPath[] = ", FilesPath, CurrentClassName, showMessagesLevel);
            return FilesPath;
        }

        public int[] GetFilesToDo()
        {
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " FilesToDo[] = ", FilesToDo.ToString(), CurrentClassName, showMessagesLevel);
            return FilesToDo;
        }

        public void SetFilesToDo(int[] filesToDo)
        {
            FilesToDo = filesToDo;
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

        void butAllFilesOpen_Click(object sender, EventArgs e)
        {
            int BreakpointManager = FilesToDo[iBreakpointManager];            
            if (BreakpointManager != (int)WhatNeedDoWithFiles.WittingIncomplete) this.Close();//if we have received all data from OpenForm we can close it
        }        

        private void butMfBox_Click(object sender, EventArgs e)     // алгоритм выбора действия по нажатым кнопкам
        {                                                           // 
            Button button = sender as Button;                       // кнопка нажата
            string ButtonName = button.Name;                        // получили место нажатой кнопки
            string ButtonText = button.Text;                        // получили имя нажатой кнопки 

            ButClickcedWhatIsLabelNow(ButtonName, ButtonText);      // вызвали обработчик
        }

        private void ButClickcedWhatIsLabelNow(string buttonName, string buttonText)
        {                                                           // алгоритм выбора действия по нажатым кнопкам
            string currentOpenFormButtonPlace = "";                 // 
            string currentOpenFormButtonName = "";                  // 
            
            var listOpenFormButtonMethods = new List<Action<int>>();
            Action<int> currentButtonMethod;
            currentButtonMethod = PathForOpenFile;
            listOpenFormButtonMethods.Add(currentButtonMethod);
            currentButtonMethod = SaveFileFromTextBox;
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
                            listOpenFormButtonMethods[j](i);
                        }
                    }
                }
            }
        }

        private void PathForOpenFile(int i)
        {
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " PathForOpenFile has fetched i = " + i.ToString(), CurrentClassName, showMessagesLevel);

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Text files|*.txt|All files|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                FilesPath[i] = dlg.FileName;
                txtboxFilesPath[i].Text = FilesPath[0];
                FilesToDo[i] = (int)WhatNeedDoWithFiles.ReadFileFirst;
            }
            statusBottomLabel.Text = Enum.GetNames(typeof(OpenFormProgressStatusMessages))[i] + " - " + FilesPath[i];//Set the short type of current action in the status bar
            //Array.Clear(FilesToDo, 0, FilesToDo.Length);
            if (OpenFileClick != null) OpenFileClick(this, EventArgs.Empty);//сказать мейну, что тут надо поменять надписи на кнопках - показать погасший Save
        }

        private void SaveFileFromTextBox(int i)
        {
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " SaveFileFromTextBox has fetched i = " + i.ToString(), CurrentClassName, showMessagesLevel);

            FilesToDo[i] = (int)WhatNeedDoWithFiles.SaveFile;

            statusBottomLabel.Text = Enum.GetNames(typeof(OpenFormProgressStatusMessages))[i] + " - " + FilesPath[i];//Set the short type of current action in the status bar

            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " FilesSaveClick = " + SaveFileClick.ToString(), CurrentClassName, showMessagesLevel);
            if (SaveFileClick != null) SaveFileClick(this, EventArgs.Empty);
        }
        
        private void Method3(int i)
        { }

        //public int ChangeOnButtonText(string[,] butMfOpenSaveLanguageNames, int theAffectedElementNumber)//меняет текст на кнопке (Open - Save)
        //{
        //    int i = 0;

        //    return i;
        //}

        public void SetFileContent(string[] filesContent, int theAffectedElementNumber)// сделать присваивание массиву в цикле и добавить коды возврата
        {            
            FilesContent = filesContent;
            if (theAffectedElementNumber == 0)
            {
                fld0EnglishContent.Text = FilesContent[0];//lblSymbolsCount[i].Text = count[i].ToString(); 
                numEnglishFont_FirstSet();
            }
                
            if (theAffectedElementNumber == 1) fld1RussianContent.Text = FilesContent[1];            
        }

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
                    FilesToDo[i] = (int)WhatNeedDoWithFiles.ContentChanged;
                    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " FilesToDo[i] - ", FilesToDo[i].ToString(), CurrentClassName, showMessagesLevel);
                    FilesContent[i] = textBox.Text;
                }
            }
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " ContentChanged - ", ContentChanged.ToString(), CurrentClassName, showMessagesLevel);
            if (ContentChanged != null) ContentChanged(this, EventArgs.Empty);
        }

        public void SetSymbolCount(int[] count, int[] filesToDo)
        {
            for (int i = 0; i < textFieldsQuantity; i++)
            {
                if (filesToDo[i] == (int)WhatNeedDoWithFiles.CountSymbols)
                {                    
                    lblSymbolsCount[i].Text = count[i].ToString();
                }
            }
        }

        private void numEnglishFont_FirstSet()
        {
            var font = fld0EnglishContent.Font;
            //ContentBox.SelectedItem = font.Name;
            numEnglishFont.SelectedIndex = Convert.ToInt32(font.Size);
        }

        private void numEnglishFont_SelectedIndexChanged(object sender, EventArgs e)
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

		private void fld1RussianContent_TextChanged(object sender, EventArgs e)
		{


















		}
	}
}
