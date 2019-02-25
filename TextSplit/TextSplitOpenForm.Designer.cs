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
            this.butAllFilesOpen = new System.Windows.Forms.Button();
            this.butSelectEnglishFile = new System.Windows.Forms.Button();
            this.fld0EnglishFilePath = new System.Windows.Forms.TextBox();
            this.butSelectRussianFile = new System.Windows.Forms.Button();
            this.fld1RussianFilePath = new System.Windows.Forms.TextBox();
            this.butSelectResultFile = new System.Windows.Forms.Button();
            this.fld2ResultFilePath = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // butAllFilesOpen
            // 
            this.butAllFilesOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.butAllFilesOpen.Location = new System.Drawing.Point(633, 90);
            this.butAllFilesOpen.Name = "butAllFilesOpen";
            this.butAllFilesOpen.Size = new System.Drawing.Size(185, 23);
            this.butAllFilesOpen.TabIndex = 6;
            this.butAllFilesOpen.Text = "Open All Files";
            this.butAllFilesOpen.UseVisualStyleBackColor = true;
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
            // fld0EnglishFilePath
            // 
            this.fld0EnglishFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fld0EnglishFilePath.Location = new System.Drawing.Point(31, 12);
            this.fld0EnglishFilePath.Name = "fld0EnglishFilePath";
            this.fld0EnglishFilePath.Size = new System.Drawing.Size(596, 20);
            this.fld0EnglishFilePath.TabIndex = 4;
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
            // fld1RussianFilePath
            // 
            this.fld1RussianFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fld1RussianFilePath.Location = new System.Drawing.Point(31, 38);
            this.fld1RussianFilePath.Name = "fld1RussianFilePath";
            this.fld1RussianFilePath.Size = new System.Drawing.Size(596, 20);
            this.fld1RussianFilePath.TabIndex = 7;
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
            // fld2ResultFilePath
            // 
            this.fld2ResultFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fld2ResultFilePath.Location = new System.Drawing.Point(31, 64);
            this.fld2ResultFilePath.Name = "fld2ResultFilePath";
            this.fld2ResultFilePath.Size = new System.Drawing.Size(596, 20);
            this.fld2ResultFilePath.TabIndex = 9;
            // 
            // TextSplitOpenForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(830, 139);
            this.Controls.Add(this.butSelectResultFile);
            this.Controls.Add(this.fld2ResultFilePath);
            this.Controls.Add(this.butSelectRussianFile);
            this.Controls.Add(this.fld1RussianFilePath);
            this.Controls.Add(this.butAllFilesOpen);
            this.Controls.Add(this.butSelectEnglishFile);
            this.Controls.Add(this.fld0EnglishFilePath);
            this.Name = "TextSplitOpenForm";
            this.Text = "TextSplitOpenForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button butSelectEnglishFile;
        private System.Windows.Forms.TextBox fld0EnglishFilePath;
        private System.Windows.Forms.Button butSelectRussianFile;
        private System.Windows.Forms.TextBox fld1RussianFilePath;
        private System.Windows.Forms.Button butSelectResultFile;
        private System.Windows.Forms.TextBox fld2ResultFilePath;
        private System.Windows.Forms.Button butAllFilesOpen;
    }
}