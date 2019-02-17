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
    //    private TextSplitForm parentForm;
    //    public TextSplitOpenForm(TextSplitForm f)
    //    {
    //        parentForm = f;
    //        InitializeComponent();        

    //        butOpenEnglishFile.Click += new EventHandler(butOpenEnglishFile_Click);            
    //        butSelectEnglishFile.Click += butSelectEnglishFile_Click;


    //        //this.parentForm.EnglishFilePath = 
    //    }

    //    void butOpenEnglishFile_Click(object sender, EventArgs e)
    //    {
    //        if (EnglishFileOpenClick != null)
    //        {
    //            EnglishFileOpenClick(this, EventArgs.Empty);
    //        }

    //    }

    //    public string EnglishFilePath
    //    { 
    //        get { return fldEnglishFilePath.Text; }
    //    }
    //}        

    //    public event EventHandler EnglishFileOpenClick;
    //    public event EventHandler SelectEnglishFileClick;

    //private void butSelectEnglishFile_Click(object sender, EventArgs e)
    //    {
    //        OpenFileDialog dlg = new OpenFileDialog();
    //        dlg.Filter = "Text files|*.txt|All files|*.*";

    //        if (dlg.ShowDialog() == DialogResult.OK)
    //        {
    //            fldEnglishFilePath.Text = dlg.FileName;

    //            if (EnglishFileOpenClick != null) EnglishFileOpenClick(this, EventArgs.Empty);
    //        }
    //    }
    }
}
