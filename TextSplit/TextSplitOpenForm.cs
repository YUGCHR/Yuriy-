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

    public partial class TextSplitOpenForm : Form
    {
        private TextSplitForm parentForm;
        //public event EventHandler AllFilesOpenClick;

        public TextSplitOpenForm(TextSplitForm f)
        {
            parentForm = f;
            InitializeComponent();
            MessageBox.Show("InitializeComponent", "InitializeComponent", MessageBoxButtons.OK, MessageBoxIcon.Information);
            butOpenAllFiles.Click += new EventHandler(butOpenAllFiles_Click);
            butSelectEnglishFile.Click += butSelectEnglishFile_Click;
            //butSelectRussianFile.Click += ButSelectRussianFile_Click;
            //butSelectResultFile.Click += ButSelectResultFile_Click;
            
        }

        //EnglishFilePath        

        void butOpenAllFiles_Click(object sender, EventArgs e)
        {
            MessageBox.Show("butOpenAllFiles_Click - Started", "butOpenAllFiles_Click", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //if (AllFilesOpenClick != null)
            //{
            //    AllFilesOpenClick(this, EventArgs.Empty);
            //}
            MessageBox.Show("AllFilesOpenClick", "AllFilesOpenClick", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MessageBox.Show(fldEnglishFilePath.Text, "fldEnglishFilePath", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MessageBox.Show(parentForm.FilePathArray[0], " - this.parentForm.FilePath[0] before", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.parentForm.FilePathArray[0] = fldEnglishFilePath.Text;
            MessageBox.Show(this.parentForm.FilePathArray[0], " - this.parentForm.FilePath[0] after", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
            
            MessageBox.Show("this.Closed", "this.Closed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //this.parentForm.FilePath[1] = fldRussianFilePath.Text;
            //this.parentForm.FilePath[2] = fldResultFilePath.Text;
        }

        private void butSelectEnglishFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Text files|*.txt|All files|*.*";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                fldEnglishFilePath.Text = dlg.FileName;
            }
        }

        //private void ButSelectRussianFile_Click(object sender, EventArgs e)
        //{
        //    OpenFileDialog dlg = new OpenFileDialog();
        //    dlg.Filter = "Text files|*.txt|All files|*.*";

        //    if (dlg.ShowDialog() == DialogResult.OK)
        //    {
        //        fldRussianFilePath.Text = dlg.FileName;
        //        MessageBox.Show("butSelectRussianFile_Click - ", "We here", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    }

        //}

        //private void ButSelectResultFile_Click(object sender, EventArgs e)
        //{
        //    OpenFileDialog dlg = new OpenFileDialog();
        //    dlg.Filter = "Text files|*.txt|All files|*.*";

        //    if (dlg.ShowDialog() == DialogResult.OK)
        //    {
        //        fldResultFilePath.Text = dlg.FileName;
        //        MessageBox.Show("butSelectResultFile_Click - ", "We here", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    }
        //}
    }
}
