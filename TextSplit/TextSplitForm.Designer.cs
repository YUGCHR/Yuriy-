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
            this.butOpenForm = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblSymbolCountOld = new System.Windows.Forms.ToolStripStatusLabel();
            this.butSaveFiles = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.textBoxImplementation = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // butOpenForm
            // 
            this.butOpenForm.Location = new System.Drawing.Point(468, 49);
            this.butOpenForm.Name = "butOpenForm";
            this.butOpenForm.Size = new System.Drawing.Size(212, 25);
            this.butOpenForm.TabIndex = 32;
            this.butOpenForm.Text = "Open All Files";
            this.butOpenForm.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.lblSymbolCountOld});
            this.statusStrip1.Location = new System.Drawing.Point(0, 570);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStrip1.Size = new System.Drawing.Size(695, 22);
            this.statusStrip1.TabIndex = 30;
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
            // butSaveFiles
            // 
            this.butSaveFiles.Location = new System.Drawing.Point(468, 97);
            this.butSaveFiles.Name = "butSaveFiles";
            this.butSaveFiles.Size = new System.Drawing.Size(212, 25);
            this.butSaveFiles.TabIndex = 39;
            this.butSaveFiles.Text = "Save All Files and Exit";
            this.butSaveFiles.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label16.Location = new System.Drawing.Point(261, 29);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(619, 17);
            this.label16.TabIndex = 61;
            this.label16.Text = "Please, look no further than the same quantity of sentences in English and Russia" +
    "n texts!";
            // 
            // textBoxImplementation
            // 
            this.textBoxImplementation.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textBoxImplementation.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxImplementation.Location = new System.Drawing.Point(12, 49);
            this.textBoxImplementation.Multiline = true;
            this.textBoxImplementation.Name = "textBoxImplementation";
            this.textBoxImplementation.ReadOnly = true;
            this.textBoxImplementation.Size = new System.Drawing.Size(450, 509);
            this.textBoxImplementation.TabIndex = 62;
            this.textBoxImplementation.Text = "Implementation progress status:";
            // 
            // label18
            // 
            this.label18.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label18.Location = new System.Drawing.Point(261, 11);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(716, 17);
            this.label18.TabIndex = 64;
            this.label18.Text = "You need to have files with synchronous English and Russian texts to make split t" +
    "ext with this program";
            // 
            // TextSplitForm
            // 
            this.AcceptButton = this.butOpenForm;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(695, 592);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.textBoxImplementation);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.butOpenForm);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.butSaveFiles);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Name = "TextSplitForm";
            this.Text = "Texts Splitter";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button butOpenForm;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel lblSymbolCountOld;
        private System.Windows.Forms.Button butSaveFiles;
        private System.Windows.Forms.Label label16;
		private System.Windows.Forms.TextBox textBoxImplementation;
        private System.Windows.Forms.Label label18;
    }
}

