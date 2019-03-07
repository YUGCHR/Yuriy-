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

        readonly private int filesQuantity;
        readonly private int filesQuantityPlus;
        readonly private int showMessagesLevel;
        readonly private string strCRLF;
        readonly private int iBreakpointManager;

        public event EventHandler ContentChanged;
        public event EventHandler OpenFileClick;
        public event EventHandler LoadEnglishToDataBase;
        //public event EventHandler FormOpenClick;
        //public event EventHandler<FormClosingEventArgs> TextSplitOpenFormClosing;

        public TextSplitOpenForm(IMessageService service)
        {            
            _messageService = service;
            
            filesQuantity = Declaration.FilesQuantity; //the length of the all Files___ arrays (except FilesToDo)
            filesQuantityPlus = Declaration.ToDoQuantity; //the length of the FilesToDo array (+1 for BreakpointManager)
            iBreakpointManager = filesQuantityPlus-1; //index of BreakpointManager in the FilesToDo array
            showMessagesLevel = Declaration.ShowMessagesLevel;
            strCRLF = Declaration.StrCRLF;

            InitializeComponent();
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "InitializeComponent", CurrentClassName, showMessagesLevel);

            FilesToDo = new int[filesQuantityPlus];
            FilesPath = new string[filesQuantity];
            FilesContent = new string[filesQuantity];

            lblSymbolsCount = new Label[] { lblSymbolCount1, lblSymbolCount2 };//, lblSymbolCount3 };
            txtboxFilesPath = new TextBox[] { fld0EnglishFilePath, fld1RussianFilePath };//, fld2ResultFilePath  };

            butAllFilesOpen.Click += new EventHandler (butAllFilesOpen_Click);
            butOpenEnglishFile.Click += butOpenFile_Click;
            butOpenRussianFile.Click += butOpenFile_Click;
            butEnglishToDataBase.Click += ButEnglishToDataBase_Click;
            //butSelectResultFile.Click += butSelectFile_Click;

            fld0EnglishContent.TextChanged += fldContent_TextChanged;
            fld1RussianContent.TextChanged += fldContent_TextChanged;
            //fld2ResultContent.TextChanged += fldContent_TextChanged;

            numEnglishFont.SelectedIndexChanged += numEnglishFont_SelectedIndexChanged;

            //butCreateResultFile.Click += butSelectEnglishFile_Click; it needs to clear up
            //fld2CreateResultFileName - result file name field

            FormClosing += TextSplitOpenForm_FormClosing;            
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

        private void butOpenFile_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string ButtonName = button.Name;

            int textFieldsQuantity = filesQuantity - 1;//TEMP

            for (int i = 0; i < textFieldsQuantity; i++)
            {
                string currentOpenFormButtonName = Enum.GetNames(typeof(OpenFormButtonNames))[i];//select current (i) button name from buttons list
                if (ButtonName == currentOpenFormButtonName)
                {                    
                    OpenFileDialog dlg = new OpenFileDialog();
                    dlg.Filter = "Text files|*.txt|All files|*.*";

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {                        
                        FilesPath[i] = dlg.FileName;
                        txtboxFilesPath[i].Text = FilesPath[i];
                        FilesToDo[i] = (int)WhatNeedDoWithFiles.ReadFirst;                        
                    }
                    statusBottomLabel.Text = Enum.GetNames(typeof(OpenFormProgressStatusMessages))[i] + " - " + FilesPath[i];//Set the short type of current action in the status bar
                    //Array.Clear(FilesToDo, 0, FilesToDo.Length);
                    if (OpenFileClick != null) OpenFileClick(this, EventArgs.Empty);
                }
            }            
        }

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
            int textFieldsQuantity = filesQuantity - 1;//TEMP

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
            int textFieldsQuantity = filesQuantity - 1;//TEMP

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
