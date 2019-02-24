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
            this.label2 = new System.Windows.Forms.Label();
            this.numFont = new System.Windows.Forms.NumericUpDown();
            this.fldEnglishContent = new System.Windows.Forms.TextBox();
            this.butSaveFiles = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblSymbolCountOld = new System.Windows.Forms.ToolStripStatusLabel();
            this.butFilesOpen = new System.Windows.Forms.Button();
            this.butSelectEnglishFile = new System.Windows.Forms.Button();
            this.lblSymbolCount1 = new System.Windows.Forms.Label();
            this.lblSymbolCount2 = new System.Windows.Forms.Label();
            this.lblSymbolCount3 = new System.Windows.Forms.Label();
            this.fldResultContent = new System.Windows.Forms.TextBox();
            this.fldRussianContent = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numFont)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Select Font Size";
            // 
            // numFont
            // 
            this.numFont.Location = new System.Drawing.Point(106, 14);
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
            this.fldEnglishContent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.fldEnglishContent.Location = new System.Drawing.Point(19, 52);
            this.fldEnglishContent.Multiline = true;
            this.fldEnglishContent.Name = "fldEnglishContent";
            this.fldEnglishContent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.fldEnglishContent.Size = new System.Drawing.Size(367, 325);
            this.fldEnglishContent.TabIndex = 6;
            // 
            // butSaveFiles
            // 
            this.butSaveFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butSaveFiles.Location = new System.Drawing.Point(951, 473);
            this.butSaveFiles.Name = "butSaveFiles";
            this.butSaveFiles.Size = new System.Drawing.Size(182, 23);
            this.butSaveFiles.TabIndex = 7;
            this.butSaveFiles.Text = "Save All Files and Exit";
            this.butSaveFiles.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.lblSymbolCountOld});
            this.statusStrip1.Location = new System.Drawing.Point(0, 499);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1145, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(101, 17);
            this.toolStripStatusLabel1.Text = "Symbols Quantity";
            // 
            // lblSymbolCountOld
            // 
            this.lblSymbolCountOld.Name = "lblSymbolCountOld";
            this.lblSymbolCountOld.Size = new System.Drawing.Size(0, 17);
            // 
            // butFilesOpen
            // 
            this.butFilesOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.butFilesOpen.Location = new System.Drawing.Point(951, 14);
            this.butFilesOpen.Name = "butFilesOpen";
            this.butFilesOpen.Size = new System.Drawing.Size(182, 23);
            this.butFilesOpen.TabIndex = 11;
            this.butFilesOpen.Text = "Open All Files";
            this.butFilesOpen.UseVisualStyleBackColor = true;
            // 
            // butSelectEnglishFile
            // 
            this.butSelectEnglishFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.butSelectEnglishFile.Location = new System.Drawing.Point(240, 14);
            this.butSelectEnglishFile.Name = "butSelectEnglishFile";
            this.butSelectEnglishFile.Size = new System.Drawing.Size(75, 23);
            this.butSelectEnglishFile.TabIndex = 10;
            this.butSelectEnglishFile.Text = "Select File";
            this.butSelectEnglishFile.UseVisualStyleBackColor = true;
            // 
            // lblSymbolCount1
            // 
            this.lblSymbolCount1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSymbolCount1.AutoSize = true;
            this.lblSymbolCount1.Location = new System.Drawing.Point(345, 380);
            this.lblSymbolCount1.Name = "lblSymbolCount1";
            this.lblSymbolCount1.Size = new System.Drawing.Size(41, 13);
            this.lblSymbolCount1.TabIndex = 12;
            this.lblSymbolCount1.Text = "English";
            // 
            // lblSymbolCount2
            // 
            this.lblSymbolCount2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSymbolCount2.AutoSize = true;
            this.lblSymbolCount2.Location = new System.Drawing.Point(1088, 380);
            this.lblSymbolCount2.Name = "lblSymbolCount2";
            this.lblSymbolCount2.Size = new System.Drawing.Size(45, 13);
            this.lblSymbolCount2.TabIndex = 12;
            this.lblSymbolCount2.Text = "Russian";
            // 
            // lblSymbolCount3
            // 
            this.lblSymbolCount3.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lblSymbolCount3.AutoSize = true;
            this.lblSymbolCount3.Location = new System.Drawing.Point(731, 380);
            this.lblSymbolCount3.Name = "lblSymbolCount3";
            this.lblSymbolCount3.Size = new System.Drawing.Size(27, 13);
            this.lblSymbolCount3.TabIndex = 12;
            this.lblSymbolCount3.Text = "Split";
            // 
            // fldResultContent
            // 
            this.fldResultContent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.fldResultContent.Location = new System.Drawing.Point(392, 52);
            this.fldResultContent.Multiline = true;
            this.fldResultContent.Name = "fldResultContent";
            this.fldResultContent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.fldResultContent.Size = new System.Drawing.Size(367, 325);
            this.fldResultContent.TabIndex = 13;
            // 
            // fldRussianContent
            // 
            this.fldRussianContent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fldRussianContent.Location = new System.Drawing.Point(766, 52);
            this.fldRussianContent.Multiline = true;
            this.fldRussianContent.Name = "fldRussianContent";
            this.fldRussianContent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.fldRussianContent.Size = new System.Drawing.Size(367, 325);
            this.fldRussianContent.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 380);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "English text";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(237, 380);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Symbols quantity:";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(627, 380);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Symbols quantity:";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(993, 380);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "Symbols quantity:";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(391, 380);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "Split text";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(764, 380);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "Russian text";
            // 
            // TextSplitForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1145, 521);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.fldRussianContent);
            this.Controls.Add(this.fldResultContent);
            this.Controls.Add(this.lblSymbolCount3);
            this.Controls.Add(this.lblSymbolCount2);
            this.Controls.Add(this.lblSymbolCount1);
            this.Controls.Add(this.butFilesOpen);
            this.Controls.Add(this.butSelectEnglishFile);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.butSaveFiles);
            this.Controls.Add(this.fldEnglishContent);
            this.Controls.Add(this.numFont);
            this.Controls.Add(this.label2);
            this.Name = "TextSplitForm";
            this.Text = "Texts Splitter";
            ((System.ComponentModel.ISupportInitialize)(this.numFont)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numFont;
        private System.Windows.Forms.TextBox fldEnglishContent;
        private System.Windows.Forms.Button butSaveFiles;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel lblSymbolCountOld;
        private System.Windows.Forms.Button butFilesOpen;
        private System.Windows.Forms.Button butSelectEnglishFile;
        private System.Windows.Forms.Label lblSymbolCount1;
        private System.Windows.Forms.Label lblSymbolCount2;
        private System.Windows.Forms.Label lblSymbolCount3;
        private System.Windows.Forms.TextBox fldResultContent;
        private System.Windows.Forms.TextBox fldRussianContent;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
    }
}

