namespace TextSplit
{
    partial class TextSplitOpenForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.butOpenEnglishFile = new System.Windows.Forms.Button();
            this.butSelectEnglishFile = new System.Windows.Forms.Button();
            this.fldEnglishFilePath = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // butOpenEnglishFile
            // 
            this.butOpenEnglishFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.butOpenEnglishFile.Location = new System.Drawing.Point(684, 9);
            this.butOpenEnglishFile.Name = "butOpenEnglishFile";
            this.butOpenEnglishFile.Size = new System.Drawing.Size(75, 23);
            this.butOpenEnglishFile.TabIndex = 6;
            this.butOpenEnglishFile.Text = "Open File";
            this.butOpenEnglishFile.UseVisualStyleBackColor = true;
            // 
            // butSelectEnglishFile
            // 
            this.butSelectEnglishFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.butSelectEnglishFile.Location = new System.Drawing.Point(603, 9);
            this.butSelectEnglishFile.Name = "butSelectEnglishFile";
            this.butSelectEnglishFile.Size = new System.Drawing.Size(75, 23);
            this.butSelectEnglishFile.TabIndex = 5;
            this.butSelectEnglishFile.Text = "Select File";
            this.butSelectEnglishFile.UseVisualStyleBackColor = true;
            // 
            // fldEnglishFilePath
            // 
            this.fldEnglishFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fldEnglishFilePath.Location = new System.Drawing.Point(31, 12);
            this.fldEnglishFilePath.Name = "fldEnglishFilePath";
            this.fldEnglishFilePath.Size = new System.Drawing.Size(566, 20);
            this.fldEnglishFilePath.TabIndex = 4;
            // 
            // TextSplitOpenForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.butOpenEnglishFile);
            this.Controls.Add(this.butSelectEnglishFile);
            this.Controls.Add(this.fldEnglishFilePath);
            this.Name = "TextSplitOpenForm";
            this.Text = "TextSplitOpenForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button butOpenEnglishFile;
        private System.Windows.Forms.Button butSelectEnglishFile;
        private System.Windows.Forms.TextBox fldEnglishFilePath;
    }
}