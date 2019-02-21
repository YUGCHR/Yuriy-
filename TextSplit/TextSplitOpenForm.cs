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
    public interface ITextSplitOpenForm
    {
        string[] FilesPath { get; set; }
        int FilesQuantity { get; set; }
        event EventHandler<FormClosingEventArgs> TextSplitOpenFormClosing;
    }

    public partial class TextSplitOpenForm : Form, ITextSplitOpenForm
    {
        public string[] FilesPath { get; set; }
        public int FilesQuantity { get; set; }
        //private TextSplitForm parentForm;
        public int filesQuantity;

        public TextSplitOpenForm()
        {
            //parentForm = f;

            InitializeComponent();
            MessageBox.Show("InitializeComponent", "InitializeComponent", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            filesQuantity = FilesQuantity;            
            MessageBox.Show(filesQuantity.ToString(), "filesQuantity", MessageBoxButtons.OK, MessageBoxIcon.Information);

            FilesPath = new string[filesQuantity];

            MessageBox.Show(FilesPath[0], " - FilePath[0] Open", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MessageBox.Show(FilesPath[1], " - FilePath[2] Open", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MessageBox.Show(FilesPath[2], " - FilePath[1] Open", MessageBoxButtons.OK, MessageBoxIcon.Information);

            butOpenAllFiles.Click += new EventHandler(butOpenAllFiles_Click);
            butSelectEnglishFile.Click += butSelectEnglishFile_Click;
            butSelectRussianFile.Click += ButSelectRussianFile_Click;
            butSelectResultFile.Click += ButSelectResultFile_Click;
            FormClosing += TextSplitOpenForm_FormClosing;

            MessageBox.Show("FilesPath[0]", " - FilePath[0] after", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            MessageBox.Show("butOpenAllFiles_Click - Started", "butOpenAllFiles_Click", MessageBoxButtons.OK, MessageBoxIcon.Information);                       
            MessageBox.Show(fldEnglishFilePath.Text, "fldEnglishFilePath", MessageBoxButtons.OK, MessageBoxIcon.Information);
            FilesPath[0] = fldEnglishFilePath.Text;
            MessageBox.Show(FilesPath[0], " - FilePath[0] after", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MessageBox.Show(fldRussianFilePath.Text, "fldRussianFilePath", MessageBoxButtons.OK, MessageBoxIcon.Information);
            FilesPath[1] = fldRussianFilePath.Text;
            MessageBox.Show(FilesPath[1], " - FilePath[1] after", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MessageBox.Show(fldResultFilePath.Text, "fldResultFilePath", MessageBoxButtons.OK, MessageBoxIcon.Information);
            FilesPath[2] = fldResultFilePath.Text;
            MessageBox.Show(FilesPath[2], " - FilePath[2] after", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
            MessageBox.Show("taki.Closed", "this.Closed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //this.parentForm.FilePath[1] = fldRussianFilePath.Text;
            //this.parentForm.FilePath[2] = fldResultFilePath.Text;
        }

        private void butSelectEnglishFile_Click(object sender, EventArgs e)
        {
            MessageBox.Show(FilesPath[0], " - FilePath[0] form", MessageBoxButtons.OK, MessageBoxIcon.Information);            
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
