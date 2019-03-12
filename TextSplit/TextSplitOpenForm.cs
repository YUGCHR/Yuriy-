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
        void SetFileContent(string[] filesPath, string[] filesContent, int[] filesToDo, int i);
        void SetSymbolCount(int[] counts, int[] filesToDo);

        event EventHandler ContentChanged;
        event EventHandler OpenFileClick;
        event EventHandler LoadEnglishToDataBase;
        //event EventHandler FormOpenClick;
        //event EventHandler<FormClosingEventArgs> TextSplitOpenFormClosing;
    }

    public partial class TextSplitOpenForm : Form, ITextSplitOpenForm
    {
        private readonly IMessageService _messageService;        

        public string[] FilesPath;// { get; set; }        
        public int[] FilesToDo;
        public string[] FilesContent;

        public Label[] lblSymbolsCount;
        public TextBox[] txtboxFilesPath;
        public string[,] butMfOpenSaveLanguage;

        readonly private int filesQuantity;
        readonly private int filesQuantityPlus;
        readonly private int textFieldsQuantity;
        readonly private int showMessagesLevel;
        readonly private string strCRLF;
        readonly private int iBreakpointManager;

        public event EventHandler ContentChanged;
        public event EventHandler OpenFileClick;
        public event EventHandler LoadEnglishToDataBase;
        //public event EventHandler FormOpenClick;
        //public event EventHandler<FormClosingEventArgs> TextSplitOpenFormClosing;

        private int T1 = 0;      

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
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "InitializeComponent", CurrentClassName, showMessagesLevel);

            FilesToDo = new int[filesQuantityPlus];
            FilesPath = new string[filesQuantity];
            FilesContent = new string[filesQuantity];

            lblSymbolsCount = new Label[] { lblSymbolCount1, lblSymbolCount2 };//, lblSymbolCount3 };
            txtboxFilesPath = new TextBox[] { fld0EnglishFilePath, fld1RussianFilePath };//, fld2ResultFilePath  };
            butMfOpenSaveLanguage = new string[,] { { "Open English File", "Save English File" }, { "Open Russian File", "Save Russian File" } };
            //English - butMfEnglishBox, Russian - butMfRussianBox

            butAllFilesOpen.Click += new EventHandler (butAllFilesOpen_Click);
            butMfLeftBox.Click += butMfBox_Click;
            butMfRightBox.Click += butMfBox_Click;
            butEnglishToDataBase.Click += ButEnglishToDataBase_Click;
            //butSelectResultFile.Click += butSelectFile_Click;

            fld0EnglishContent.TextChanged += fldContent_TextChanged;
            fld1RussianContent.TextChanged += fldContent_TextChanged;
            //fld2ResultContent.TextChanged += fldContent_TextChanged;

            numEnglishFont.SelectedIndexChanged += numEnglishFont_SelectedIndexChanged;

            //butCreateResultFile.Click += butSelectEnglishFile_Click; it needs to clear up
            //fld2CreateResultFileName - result file name field

            FormClosing += TextSplitOpenForm_FormClosing;

            butMfLeftBox.Text = butMfOpenSaveLanguage[0, 0];
            butMfRightBox.Text = butMfOpenSaveLanguage[1, 0];
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "butMfEnglishBox ==> " + butMfLeftBox.Text +
                strCRLF + "butMfRussianBox ==> " + butMfRightBox.Text, CurrentClassName, showMessagesLevel);
            


        }

        private void ButEnglishToDataBase_Click(object sender, EventArgs e)
        {
            //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " LoadEnglishToDataBase = ", LoadEnglishToDataBase.ToString(), CurrentClassName, 3);
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

        private void butMfBox_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string ButtonName = button.Name;
            string ButtonText = button.Text; 

            ButClickcedWhatIsLabelNow(ButtonName, ButtonText);
        }

        private void ButClickcedWhatIsLabelNow(string buttonName, string buttonText)
        {
            string currentOpenFormButtonPlace = "";
            string currentOpenFormButtonName = "";

            var list = new List<Action>();
            Action currentMethod = Method1;
            list.Add(currentMethod);
            currentMethod = Method2;
            list.Add(currentMethod);
            currentMethod = Method3;
            list.Add(currentMethod);

            for (int i = 0; i < textFieldsQuantity; i++)//0 - Left (English), 1 - Right (Russian)
            {
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Cycle i started, i = " + i.ToString(), CurrentClassName, 3);
                currentOpenFormButtonPlace = Enum.GetNames(typeof(OpenFormButtonNames))[i];
                if (buttonName == currentOpenFormButtonPlace)
                { 
                    for (int j = 0; j < textFieldsQuantity; j++)
                    {
                        _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Cycle j started, j = " + j.ToString(), CurrentClassName, 3);
                        currentOpenFormButtonName = butMfOpenSaveLanguage[i, j];

                        if (buttonText == currentOpenFormButtonName)
                        {
                            if (i == 0 & j == 0) list[0]();
                        }
                    }
                }
            }
        }

        private void Method1()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Text files|*.txt|All files|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                FilesPath[0] = dlg.FileName;
                txtboxFilesPath[0].Text = FilesPath[0];
                FilesToDo[0] = (int)WhatNeedDoWithFiles.ReadFirst;
            }
            statusBottomLabel.Text = Enum.GetNames(typeof(OpenFormProgressStatusMessages))[0] + " - " + FilesPath[0];//Set the short type of current action in the status bar                    
            //Array.Clear(FilesToDo, 0, FilesToDo.Length);
            if (OpenFileClick != null) OpenFileClick(this, EventArgs.Empty);
        }

        private void Method2()
        { }
        private void Method3()
        { }
        

        public void SetFileContent(string[] filesPath, string[] filesContent, int[] filesToDo, int i)
        {            
            FilesContent = filesContent;
            if (i == 0)
            {
                fld0EnglishContent.Text = FilesContent[0];//lblSymbolsCount[i].Text = count[i].ToString(); 
                numEnglishFont_FirstSet();
            }
                
            if (i == 1) fld1RussianContent.Text = FilesContent[1];
            
        }

        private void fldContent_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string TextBoxName = textBox.Name;
            //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " TextBoxName - ", TextBoxName, CurrentClassName, 3);
            

            for (int i = 0; i < textFieldsQuantity; i++)
            {
                string currentFormFieldName = Enum.GetNames(typeof(FormFieldsNames))[i];
                if (TextBoxName == currentFormFieldName)
                {
                    FilesToDo[i] = (int)WhatNeedDoWithFiles.ContentChanged;
                    //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " FilesToDo[i] - ", FilesToDo[i].ToString(), CurrentClassName, 3);
                    FilesContent[i] = textBox.Text;
                }
            }
            //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " ContentChanged - ", ContentChanged.ToString(), CurrentClassName, 3);
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
