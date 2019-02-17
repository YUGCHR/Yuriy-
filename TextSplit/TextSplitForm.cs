using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextSplit
{
    public interface ITextSplitForm
    {
        string FilePath { get; }
        string EnglishContent { get; set; }
        //bool WasEnglishContentChange { set; }
        void SetSymbolCount(int count);
          
        event EventHandler FilesOpenClick;
        event EventHandler FilesSaveClick;
        event EventHandler EnglishContentChanged;
        //event EventHandler TextSplitFormClosing;
        event EventHandler <FormClosingEventArgs> TextSplitFormClosing;
    }

    public partial class TextSplitForm : Form, ITextSplitForm
    {
        //private bool wasEnglishContentChange;
        public string EnglishFilePath;

        public TextSplitForm()
        {
            InitializeComponent();
            butOpenFiles.Click += new EventHandler(butOpenFiles_Click);
            butSaveFiles.Click += butSaveFiles_Click;
            fldEnglishContent.TextChanged += fldEnglishContent_TextChanged;
            butSelectEnglishFile.Click += butSelectEnglishFile_Click;
            numFont.ValueChanged += numFont_ValueChanged;
            FormClosing += TextSplitForm_FormClosing;
            
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
            MessageBox.Show(messageBoxCS.ToString(), "FormClosing Event");
        }
                
        void butOpenFiles_Click(object sender, EventArgs e)
        {
            TextSplitOpenForm openForm = new TextSplitOpenForm(this);
            openForm.Show();
            //MessageBox.Show(EnglishFilePath, "EnglishFilePath received", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //if (FilesOpenClick != null)
            //{
            //    FilesOpenClick(this, EventArgs.Empty);
            //}
            //EnglishFilePath = 
        }

        private void butSaveFiles_Click(object sender, EventArgs e)
        {
            if (FilesSaveClick != null) FilesSaveClick(this, EventArgs.Empty);
        }

        private void fldEnglishContent_TextChanged(object sender, EventArgs e)
        {
            if (EnglishContentChanged != null) EnglishContentChanged(this, EventArgs.Empty);
        }
        #endregion

        #region Interface ITextSplitForm
        public string FilePath
        { 
            get { return fldEnglishFilePath.Text; } 
        }

        public string EnglishContent
        {
            get { return fldEnglishContent.Text; }
            set { fldEnglishContent.Text = value; }
        }

        public void SetSymbolCount(int count)
        {
            lblSymbolCount.Text = count.ToString();            
        }

        //public bool WasEnglishContentChange
        //{
        //    set { wasEnglishContentChange = value; }
        //}


        public event EventHandler FilesOpenClick;
        public event EventHandler FilesSaveClick;
        public event EventHandler EnglishContentChanged;
        //public event EventHandler TextSplitFormClosing;
        public event EventHandler<FormClosingEventArgs> TextSplitFormClosing;
        #endregion

        private void butSelectEnglishFile_Click(object sender, EventArgs e)
        {
            //OpenFileDialog dlg = new OpenFileDialog();
            //dlg.Filter = "Text files|*.txt|All files|*.*";

            //if (dlg.ShowDialog() == DialogResult.OK)
            //{
            fldEnglishFilePath.Text = EnglishFilePath;
            MessageBox.Show(EnglishFilePath, "EnglishFilePath received", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (FilesOpenClick != null) FilesOpenClick(this, EventArgs.Empty);
            
        }

        private void numFont_ValueChanged(object sender, EventArgs e)
        {            
            {
                fldEnglishContent.Font = new Font("Calibri", (float)numFont.Value);
            }
        }
    }
}
