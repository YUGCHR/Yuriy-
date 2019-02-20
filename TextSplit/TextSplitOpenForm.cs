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

        public TextSplitOpenForm(TextSplitForm f)
        {
            parentForm = f;

            InitializeComponent();
            MessageBox.Show("InitializeComponent", "InitializeComponent", MessageBoxButtons.OK, MessageBoxIcon.Information);

            butOpenAllFiles.Click += new EventHandler(butOpenAllFiles_Click);
            butSelectEnglishFile.Click += butSelectEnglishFile_Click;
            butSelectRussianFile.Click += ButSelectRussianFile_Click;
            butSelectResultFile.Click += ButSelectResultFile_Click;
        }
        
        void butOpenAllFiles_Click(object sender, EventArgs e)
        {
            MessageBox.Show("butOpenAllFiles_Click - Started", "butOpenAllFiles_Click", MessageBoxButtons.OK, MessageBoxIcon.Information);                       
            MessageBox.Show(fldEnglishFilePath.Text, "fldEnglishFilePath", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MessageBox.Show(fldRussianFilePath.Text, "fldRussianFilePath", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MessageBox.Show(fldResultFilePath.Text, "fldResultFilePath", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.parentForm.FilesPath[0] = fldEnglishFilePath.Text;
            this.parentForm.FilesPath[1] = fldResultFilePath.Text;
            this.parentForm.FilesPath[2] = fldResultFilePath.Text;
            MessageBox.Show(this.parentForm.FilesPath[0], " - FilePath[0] after", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MessageBox.Show(this.parentForm.FilesPath[1], " - FilePath[1] after", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MessageBox.Show(this.parentForm.FilesPath[2], " - FilePath[2] after", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
            MessageBox.Show("taki.Closed", "this.Closed", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show(fldEnglishFilePath.Text, "EnglishFilePath", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ButSelectRussianFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Text files|*.txt|All files|*.*";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                fldRussianFilePath.Text = dlg.FileName;
                MessageBox.Show(fldRussianFilePath.Text, "RussianFilePath", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void ButSelectResultFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Text files|*.txt|All files|*.*";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                fldResultFilePath.Text = dlg.FileName;
                MessageBox.Show(fldResultFilePath.Text, "ResultFilePath", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
