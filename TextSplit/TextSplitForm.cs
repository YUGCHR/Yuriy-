using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using TextSplitLibrary;
using System.Windows.Forms;


namespace TextSplit
{
    public interface ITextSplitForm
    {
        void ManageFilesContent(string[] filesPath, string[] filesContent, int[] filesToDo);
        void SetSymbolCount(int[] counts, int[] filesToDo);
          
        //event EventHandler FormOpenClick;
        event EventHandler FilesSaveClick;
        event EventHandler OpenTextSplitOpenForm;
        event EventHandler EnglishContentChanged;
        
        event EventHandler <FormClosingEventArgs> TextSplitFormClosing;
        //event EventHandler <FormClosingEventArgs> TextSplitOpenFormClosing;
    }

    public partial class TextSplitForm : Form, ITextSplitForm
    {
        private readonly IMessageService _messageService;
        public string[] FilesPath { get; set; }
        public string[] FilesContent { get; set; }        
        public int[] FilesToDo { get; set; }
        public int[] counts;
        public Label[] lblSymbolsCount;        
        public int filesQuantity = Declaration.LanguagesQuantity;
        public int showMessagesLevel = Declaration.ShowMessagesLevel;

        //public event EventHandler FormOpenClick;
        public event EventHandler FilesSaveClick;
        public event EventHandler OpenTextSplitOpenForm;
        public event EventHandler EnglishContentChanged;
        //public event EventHandler TextSplitFormClosing;
        public event EventHandler<FormClosingEventArgs> TextSplitFormClosing;
        //public event EventHandler <FormClosingEventArgs> TextSplitOpenFormClosing;

        public TextSplitForm(IMessageService service)
        {
            _messageService = service;
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " Started", CurrentClassName, showMessagesLevel);            
            InitializeComponent();

            //_open = open;
            butFilesOpen.Click += new EventHandler(butFilesOpen_Click);
            butSaveFiles.Click += butSaveFiles_Click;
            fldEnglishContent.TextChanged += fldContent_TextChanged;
            fldRussianContent.TextChanged += fldContent_TextChanged;
            fldResultContent.TextChanged += fldContent_TextChanged;

            //butSelectEnglishFile.Click += butSelectEnglishFile_Click;
            numFont.ValueChanged += numFont_ValueChanged;
            FormClosing += TextSplitForm_FormClosing;            
            
            FilesToDo = new int[filesQuantity];
            FilesPath = new string[filesQuantity];
            FilesContent = new string[filesQuantity];
            
            lblSymbolsCount = new Label[] { lblSymbolCount1, lblSymbolCount2, lblSymbolCount3 };                        
        }

        #region Events forwarding
        private void TextSplitForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //MessageBox.Show(wasEnglishContentChange.ToString(), "It caught there, not here", MessageBoxButtons.OK, MessageBoxIcon.Information);
            TextSplitFormClosing(this, e);
            //MessageBox.Show(wasEnglishContentChange.ToString(), "We here again - last step", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //e.Cancel = wasEnglishContentChange;
            System.Text.StringBuilder messageBoxCS = new System.Text.StringBuilder();
            messageBoxCS.AppendFormat("{0} = {1}", "CloseReason", e.CloseReason);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "Cancel", e.Cancel);
            messageBoxCS.AppendLine();
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " messageBoxCS = ", messageBoxCS.ToString(), CurrentClassName, showMessagesLevel);            
        }

        //private void _open_TextSplitOpenFormClosing(object sender, FormClosingEventArgs e)
        //{            
        //    _messageService.ShowTrace("after TextSplitOpenFormClosing ", "(Closed)", CurrentClassName, showMessagesLevel);
        //    //e.Cancel = wasEnglishContentChange;
        //}

        void butFilesOpen_Click(object sender, EventArgs e)//обрабатываем нажатие кнопки Open, которое означает открытие вспомогательной формы
        {
            //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " Started", CurrentClassName, showMessagesLevel);
            //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " FormOpenClick Called", CurrentClassName, showMessagesLevel);            
            //if (FormOpenClick != null) FormOpenClick(this, EventArgs.Empty);//Received 3 arrays from Main            
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " OpenTextSplitOpenForm will call now - " + OpenTextSplitOpenForm.ToString(), CurrentClassName, showMessagesLevel);
            if (OpenTextSplitOpenForm != null) OpenTextSplitOpenForm(this, EventArgs.Empty);                      
        }

        private void butSaveFiles_Click(object sender, EventArgs e)
        {
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " FilesSaveClick = ", FilesSaveClick.ToString(), CurrentClassName, showMessagesLevel);
            if (FilesSaveClick != null) FilesSaveClick(this, EventArgs.Empty);
        }

        private void fldContent_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string textBoxName = textBox.Name;
                        
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " chName - " + textBoxName, CurrentClassName, 3);// showMessagesLevel);
            if (EnglishContentChanged != null) EnglishContentChanged(this, EventArgs.Empty);
        }
        #endregion

        #region Interface ITextSplitForm
        //public string EnglishContent - source version get/set without arrays
        //{
        //    get { return fldEnglishContent.Text; }
        //    set { fldEnglishContent.Text = value; }
        //}
        public void ManageFilesContent(string[] filesPath, string[] filesContent, int[] filesToDo)
        {
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " Received filesPath - ", filesPath, CurrentClassName, showMessagesLevel);
            FilesPath = filesPath;
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " FilePath set - ", FilesPath, CurrentClassName, showMessagesLevel);
            FilesContent = filesContent;
            FilesToDo = filesToDo;
            if (FilesToDo[0] != 0) fldEnglishContent.Text = FilesContent[0];
            if (FilesToDo[1] != 0) fldRussianContent.Text = FilesContent[0];
            if (FilesToDo[2] != 0) fldResultContent.Text = FilesContent[0];
        }

        public void SetSymbolCount(int[] count, int[] filesToDo)
        {
            for (int i = 0; i < filesQuantity; i++)
            {
                if (filesToDo[i] != 0)
                {                    
                    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), count[i].ToString(), CurrentClassName, showMessagesLevel);
                    lblSymbolsCount[i].Text = count[i].ToString();
                }                
            }            
        }        
        #endregion

        

        //private void butSelectEnglishFile_Click(object sender, EventArgs e)//сразу открыть файлы?
        //{
        //    //OpenFileDialog dlg = new OpenFileDialog();
        //    //dlg.Filter = "Text files|*.txt|All files|*.*";

        //    //if (dlg.ShowDialog() == DialogResult.OK)
        //    //{
        //    MessageBox.Show(FilesPath[0], "FilePathArray received", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    fldEnglishFilePath.Text = FilesPath[0];
        //    MessageBox.Show(fldEnglishFilePath.Text, "EnglishFilePath received", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    //if (FilesOpenClick != null)
        //    FilesOpenClick(this, EventArgs.Empty);            
        //}

        private void numFont_ValueChanged(object sender, EventArgs e)
        {            
            {
                fldEnglishContent.Font = new Font("Calibri", (float)numFont.Value);
            }
        }

        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }        
    }
}
