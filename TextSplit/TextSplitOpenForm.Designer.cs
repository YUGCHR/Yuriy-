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
            this.butOpenAllFiles = new System.Windows.Forms.Button();
            this.butSelectEnglishFile = new System.Windows.Forms.Button();
            this.fldEnglishFilePath = new System.Windows.Forms.TextBox();
            this.butSelectRussianFile = new System.Windows.Forms.Button();
            this.fldRussianFilePath = new System.Windows.Forms.TextBox();
            this.butSelectResultFile = new System.Windows.Forms.Button();
            this.fldResultFilePath = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // butOpenAllFiles
            // 
            this.butOpenAllFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.butOpenAllFiles.Location = new System.Drawing.Point(633, 90);
            this.butOpenAllFiles.Name = "butOpenAllFiles";
            this.butOpenAllFiles.Size = new System.Drawing.Size(185, 23);
            this.butOpenAllFiles.TabIndex = 6;
            this.butOpenAllFiles.Text = "Open All Files and Return";
            this.butOpenAllFiles.UseVisualStyleBackColor = true;
            // 
            // butSelectEnglishFile
            // 
            this.butSelectEnglishFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.butSelectEnglishFile.Location = new System.Drawing.Point(633, 9);
            this.butSelectEnglishFile.Name = "butSelectEnglishFile";
            this.butSelectEnglishFile.Size = new System.Drawing.Size(185, 23);
            this.butSelectEnglishFile.TabIndex = 5;
            this.butSelectEnglishFile.Text = "Select English File";
            this.butSelectEnglishFile.UseVisualStyleBackColor = true;
            // 
            // fldEnglishFilePath
            // 
            this.fldEnglishFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fldEnglishFilePath.Location = new System.Drawing.Point(31, 12);
            this.fldEnglishFilePath.Name = "fldEnglishFilePath";
            this.fldEnglishFilePath.Size = new System.Drawing.Size(596, 20);
            this.fldEnglishFilePath.TabIndex = 4;
            // 
            // butSelectRussianFile
            // 
            this.butSelectRussianFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.butSelectRussianFile.Location = new System.Drawing.Point(633, 35);
            this.butSelectRussianFile.Name = "butSelectRussianFile";
            this.butSelectRussianFile.Size = new System.Drawing.Size(185, 23);
            this.butSelectRussianFile.TabIndex = 8;
            this.butSelectRussianFile.Text = "Select Russian File";
            this.butSelectRussianFile.UseVisualStyleBackColor = true;
            // 
            // fldRussianFilePath
            // 
            this.fldRussianFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fldRussianFilePath.Location = new System.Drawing.Point(31, 38);
            this.fldRussianFilePath.Name = "fldRussianFilePath";
            this.fldRussianFilePath.Size = new System.Drawing.Size(596, 20);
            this.fldRussianFilePath.TabIndex = 7;
            // 
            // butSelectResultFile
            // 
            this.butSelectResultFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.butSelectResultFile.Location = new System.Drawing.Point(633, 61);
            this.butSelectResultFile.Name = "butSelectResultFile";
            this.butSelectResultFile.Size = new System.Drawing.Size(185, 23);
            this.butSelectResultFile.TabIndex = 10;
            this.butSelectResultFile.Text = "Select Result File";
            this.butSelectResultFile.UseVisualStyleBackColor = true;
            // 
            // fldResultFilePath
            // 
            this.fldResultFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fldResultFilePath.Location = new System.Drawing.Point(31, 64);
            this.fldResultFilePath.Name = "fldResultFilePath";
            this.fldResultFilePath.Size = new System.Drawing.Size(596, 20);
            this.fldResultFilePath.TabIndex = 9;
            // 
            // TextSplitOpenForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(830, 187);
            this.Controls.Add(this.butSelectResultFile);
            this.Controls.Add(this.fldResultFilePath);
            this.Controls.Add(this.butSelectRussianFile);
            this.Controls.Add(this.fldRussianFilePath);
            this.Controls.Add(this.butOpenAllFiles);
            this.Controls.Add(this.butSelectEnglishFile);
            this.Controls.Add(this.fldEnglishFilePath);
            this.Name = "TextSplitOpenForm";
            this.Text = "TextSplitOpenForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button butSelectEnglishFile;
        private System.Windows.Forms.TextBox fldEnglishFilePath;
        private System.Windows.Forms.Button butSelectRussianFile;
        private System.Windows.Forms.TextBox fldRussianFilePath;
        private System.Windows.Forms.Button butSelectResultFile;
        private System.Windows.Forms.TextBox fldResultFilePath;
        private System.Windows.Forms.Button butOpenAllFiles;
    }
}