using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSplitLibrary;
using System.Windows.Forms;

namespace TextSplit
{
    public interface ITextSplitOpenForm
    {
        void ManageFilesContent(string[] filesPath, string[] filesContent, int[] filesToDo);        
        event EventHandler<FormClosingEventArgs> TextSplitOpenFormClosing;
    }

    public partial class TextSplitOpenForm : Form, ITextSplitOpenForm
    {
        private readonly IMessageService _messageService;
        public string[] FilesPath { get; set; }
        public string[] FilesContent { get; set; }
        public int[] FilesToDo { get; set; }        
        public int filesQuantity = Declaration.LanguagesQuantity;        

        public TextSplitOpenForm(IMessageService service)
        {
            _messageService = service;

            InitializeComponent();
            MessageBox.Show("InitializeComponent", "TextSplitOpenForm", MessageBoxButtons.OK, MessageBoxIcon.Information);            
            
            FilesToDo = new int[filesQuantity];
            FilesPath = new string[filesQuantity];
            FilesContent = new string[filesQuantity];

            butOpenAllFiles.Click += new EventHandler(butOpenAllFiles_Click);
            butSelectEnglishFile.Click += butSelectEnglishFile_Click;
            butSelectRussianFile.Click += ButSelectRussianFile_Click;
            butSelectResultFile.Click += ButSelectResultFile_Click;
            FormClosing += TextSplitOpenForm_FormClosing;            
        }

        private void TextSplitOpenForm_FormClosing(object sender, FormClosingEventArgs e)
        {            
            MessageBox.Show("TextSplitOpenForm_FormClosing", "We are here", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //TextSplitOpenFormClosing(this, e);
            //MessageBox.Show(wasEnglishContentChange.ToString(), "We here again - last step", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //e.Cancel = wasEnglishContentChange;
            System.Text.StringBuilder messageBoxCS = new System.Text.StringBuilder();
            messageBoxCS.AppendFormat("{0} = {1}", "CloseReason", e.CloseReason);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "Cancel", e.Cancel);
            messageBoxCS.AppendLine();
            MessageBox.Show(messageBoxCS.ToString(), "FormClosing Event");
        }

        void butOpenAllFiles_Click(object sender, EventArgs e)
        {           
            MessageBox.Show("butOpenAllFiles_Click - Started", "TextSplitOpenForm", MessageBoxButtons.OK, MessageBoxIcon.Information);                       
            MessageBox.Show(fldEnglishFilePath.Text, "TextSplitOpenForm-butOpenAllFiles_Click - fldEnglishFilePath", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //MessageBox.Show(_filesPath[0], "TextSplitOpenForm-butOpenAllFiles_Click - _filesPath[0] before", MessageBoxButtons.OK, MessageBoxIcon.Information);
            FilesPath[0] = fldEnglishFilePath.Text;
            //MessageBox.Show(_filesPath[0], "TextSplitOpenForm-butOpenAllFiles_Click - _filesPath[0] after", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //MessageBox.Show(fldRussianFilePath.Text, "fldRussianFilePath", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //MessageBox.Show(_filesPath[1], "TextSplitOpenForm-butOpenAllFiles_Click - _filesPath[1] before", MessageBoxButtons.OK, MessageBoxIcon.Information);
            FilesPath[1] = fldRussianFilePath.Text;
            //MessageBox.Show(_filesPath[1], "TextSplitOpenForm-butOpenAllFiles_Click - _filesPath[1] after", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //MessageBox.Show(fldResultFilePath.Text, "fldResultFilePath", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //MessageBox.Show(_filesPath[2], "TextSplitOpenForm-butOpenAllFiles_Click - _filesPath[2] before", MessageBoxButtons.OK, MessageBoxIcon.Information);
            FilesPath[2] = fldResultFilePath.Text;
            //MessageBox.Show(_filesPath[2], "TextSplitOpenForm-butOpenAllFiles_Click - _filesPath[2] after", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
            MessageBox.Show("taki.Closed", "TextSplitOpenForm", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //this.parentForm.FilePath[1] = fldRussianFilePath.Text;
            //this.parentForm.FilePath[2] = fldResultFilePath.Text;
        }

        public void ManageFilesContent(string[] filesPath, string[] filesContent, int[] filesToDo)
        {
            _messageService.ShowTrace("Received filePath", filesPath[0], "Form-ManageFilesContent", 2); //traced
            FilesPath = filesPath;
            _messageService.ShowTrace("FilePath set", FilesPath[0], "Form-ManageFilesContent", 2); //traced
            FilesContent = filesContent;
            FilesToDo = filesToDo;
        }

        private void butSelectEnglishFile_Click(object sender, EventArgs e)
        {
            MessageBox.Show(FilesPath[0], "TextSplitOpenForm-butSelectEnglishFile_Click - _filePath[0] form", MessageBoxButtons.OK, MessageBoxIcon.Information);            
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Text files|*.txt|All files|*.*";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                fldEnglishFilePath.Text = dlg.FileName;
                //MessageBox.Show(fldEnglishFilePath.Text, "EnglishFilePath", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ButSelectRussianFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Text files|*.txt|All files|*.*";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                fldRussianFilePath.Text = dlg.FileName;
                //MessageBox.Show(fldRussianFilePath.Text, "RussianFilePath", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void ButSelectResultFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Text files|*.txt|All files|*.*";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                fldResultFilePath.Text = dlg.FileName;
                //MessageBox.Show(fldResultFilePath.Text, "ResultFilePath", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public event EventHandler <FormClosingEventArgs> TextSplitOpenFormClosing;      
    }
}
