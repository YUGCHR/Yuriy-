namespace TextSplit
{
    partial class TextSplitForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numFont = new System.Windows.Forms.NumericUpDown();
            this.fldEnglishContent = new System.Windows.Forms.TextBox();
            this.butSaveFiles = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblSymbolCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.butOpenFiles = new System.Windows.Forms.Button();
            this.butSelectEnglishFile = new System.Windows.Forms.Button();
            this.fldEnglishFilePath = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numFont)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select English file";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Select Font Size";
            // 
            // numFont
            // 
            this.numFont.Location = new System.Drawing.Point(106, 63);
            this.numFont.Maximum = new decimal(new int[] {
            72,
            0,
            0,
            0});
            this.numFont.Minimum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.numFont.Name = "numFont";
            this.numFont.Size = new System.Drawing.Size(120, 20);
            this.numFont.TabIndex = 5;
            this.numFont.Value = new decimal(new int[] {
            11,
            0,
            0,
            0});
            // 
            // fldEnglishContent
            // 
            this.fldEnglishContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fldEnglishContent.Location = new System.Drawing.Point(19, 101);
            this.fldEnglishContent.Multiline = true;
            this.fldEnglishContent.Name = "fldEnglishContent";
            this.fldEnglishContent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.fldEnglishContent.Size = new System.Drawing.Size(725, 297);
            this.fldEnglishContent.TabIndex = 6;
            // 
            // butSaveFiles
            // 
            this.butSaveFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butSaveFiles.Location = new System.Drawing.Point(669, 415);
            this.butSaveFiles.Name = "butSaveFiles";
            this.butSaveFiles.Size = new System.Drawing.Size(75, 23);
            this.butSaveFiles.TabIndex = 7;
            this.butSaveFiles.Text = "Save Files";
            this.butSaveFiles.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.lblSymbolCount});
            this.statusStrip1.Location = new System.Drawing.Point(0, 471);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(756, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(101, 17);
            this.toolStripStatusLabel1.Text = "Symbols Quantity";
            // 
            // lblSymbolCount
            // 
            this.lblSymbolCount.Name = "lblSymbolCount";
            this.lblSymbolCount.Size = new System.Drawing.Size(0, 17);
            // 
            // butOpenFiles
            // 
            this.butOpenFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.butOpenFiles.Location = new System.Drawing.Point(669, 26);
            this.butOpenFiles.Name = "butOpenFiles";
            this.butOpenFiles.Size = new System.Drawing.Size(75, 23);
            this.butOpenFiles.TabIndex = 11;
            this.butOpenFiles.Text = "Open Files";
            this.butOpenFiles.UseVisualStyleBackColor = true;
            // 
            // butSelectEnglishFile
            // 
            this.butSelectEnglishFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.butSelectEnglishFile.Location = new System.Drawing.Point(588, 26);
            this.butSelectEnglishFile.Name = "butSelectEnglishFile";
            this.butSelectEnglishFile.Size = new System.Drawing.Size(75, 23);
            this.butSelectEnglishFile.TabIndex = 10;
            this.butSelectEnglishFile.Text = "Select File";
            this.butSelectEnglishFile.UseVisualStyleBackColor = true;
            // 
            // fldEnglishFilePath
            // 
            this.fldEnglishFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fldEnglishFilePath.Location = new System.Drawing.Point(16, 29);
            this.fldEnglishFilePath.Name = "fldEnglishFilePath";
            this.fldEnglishFilePath.Size = new System.Drawing.Size(566, 20);
            this.fldEnglishFilePath.TabIndex = 9;
            // 
            // TextSplitForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(756, 493);
            this.Controls.Add(this.butOpenFiles);
            this.Controls.Add(this.butSelectEnglishFile);
            this.Controls.Add(this.fldEnglishFilePath);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.butSaveFiles);
            this.Controls.Add(this.fldEnglishContent);
            this.Controls.Add(this.numFont);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "TextSplitForm";
            this.Text = "Texts Splitter";
            ((System.ComponentModel.ISupportInitialize)(this.numFont)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numFont;
        private System.Windows.Forms.TextBox fldEnglishContent;
        private System.Windows.Forms.Button butSaveFiles;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel lblSymbolCount;
        private System.Windows.Forms.Button butOpenFiles;
        private System.Windows.Forms.Button butSelectEnglishFile;
        private System.Windows.Forms.TextBox fldEnglishFilePath;
    }
}

