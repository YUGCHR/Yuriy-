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
        string[] FilePath { get; set; }
        string[] FileContent { get; set; }
        int FilesQuantity { get; set; }
        //bool WasEnglishContentChange { set; }
        void SetSymbolCount(int[] count);
          
        event EventHandler FilesOpenClick;
        event EventHandler FilesSaveClick;
        event EventHandler EnglishContentChanged;
        //event EventHandler TextSplitFormClosing;
        event EventHandler <FormClosingEventArgs> TextSplitFormClosing;
    }

    public partial class TextSplitForm : Form, ITextSplitForm
    {
        //private bool wasEnglishContentChange;        
        public string[] FilePath { get; set; }
        public string[] FileContent { get; set; }
        public Label[] lblSymbolCount;
        public int FilesQuantity { get; set; }
        public int[] count;
        public int filesQuantity;


        public TextSplitForm()
        {            
            InitializeComponent();
            butOpenFiles.Click += new EventHandler(butOpenFiles_Click);
            butSaveFiles.Click += butSaveFiles_Click;
            fldEnglishContent.TextChanged += fldEnglishContent_TextChanged;
            butSelectEnglishFile.Click += butSelectEnglishFile_Click;
            numFont.ValueChanged += numFont_ValueChanged;
            FormClosing += TextSplitForm_FormClosing;
            filesQuantity = FilesQuantity;
            FilePath = new string[filesQuantity];
            FileContent = new string[filesQuantity];            
            count = new int[filesQuantity];
            lblSymbolCount = new Label[] { lblSymbolCount1, lblSymbolCount2, lblSymbolCount3 };
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
            MessageBox.Show("butOpenFiles_Click - Started", "butOpenFiles_Click", MessageBoxButtons.OK, MessageBoxIcon.Information);
            TextSplitOpenForm openForm = new TextSplitOpenForm(this);
            MessageBox.Show("openForm.Show will start now", "openForm.Show", MessageBoxButtons.OK, MessageBoxIcon.Information);
            openForm.Show();
            MessageBox.Show("after openForm.Show now", "openForm.Show", MessageBoxButtons.OK, MessageBoxIcon.Information);            
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
        //public string EnglishContent - source version get/set without arrays
        //{
        //    get { return fldEnglishContent.Text; }
        //    set { fldEnglishContent.Text = value; }
        //}

        public void SetSymbolCount(int[] count)
        {
            for (int i = 0; i < filesQuantity; i++)
            {
                //count[i] = i;
                MessageBox.Show(i.ToString(), "SetSymbolCount", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblSymbolCount[i].Text = i.ToString();
                    //count[i].ToString();                
            }            
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

        private void butSelectEnglishFile_Click(object sender, EventArgs e)//сразу открыть файлы?
        {
            //OpenFileDialog dlg = new OpenFileDialog();
            //dlg.Filter = "Text files|*.txt|All files|*.*";

            //if (dlg.ShowDialog() == DialogResult.OK)
            //{
            MessageBox.Show(FilePath[0], "FilePathArray received", MessageBoxButtons.OK, MessageBoxIcon.Information);
            fldEnglishFilePath.Text = FilePath[0];
            MessageBox.Show(fldEnglishFilePath.Text, "EnglishFilePath received", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //if (FilesOpenClick != null)
            FilesOpenClick(this, EventArgs.Empty);            
        }

        private void numFont_ValueChanged(object sender, EventArgs e)
        {            
            {
                fldEnglishContent.Font = new Font("Calibri", (float)numFont.Value);
            }
        }
    }
}
