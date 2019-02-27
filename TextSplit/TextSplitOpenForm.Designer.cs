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
            this.butSelectRussianFile = new System.Windows.Forms.Button();
            this.butSelectResultFile = new System.Windows.Forms.Button();
            this.butCreateResultFile = new System.Windows.Forms.Button();
            this.fld2CreateResultFileName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Cancel = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusBottomLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // butAllFilesOpen
            // 
            this.butAllFilesOpen.Location = new System.Drawing.Point(271, 278);
            this.butAllFilesOpen.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.butAllFilesOpen.Name = "butAllFilesOpen";
            this.butAllFilesOpen.Size = new System.Drawing.Size(216, 24);
            this.butAllFilesOpen.TabIndex = 6;
            this.butAllFilesOpen.Text = "Open All selected Files";
            this.butAllFilesOpen.UseVisualStyleBackColor = true;
            // 
            // butSelectEnglishFile
            // 
            this.butSelectEnglishFile.Location = new System.Drawing.Point(40, 44);
            this.butSelectEnglishFile.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.butSelectEnglishFile.Name = "butSelectEnglishFile";
            this.butSelectEnglishFile.Size = new System.Drawing.Size(216, 24);
            this.butSelectEnglishFile.TabIndex = 5;
            this.butSelectEnglishFile.Text = "Select English File";
            this.butSelectEnglishFile.UseVisualStyleBackColor = true;
            // 
            // butSelectRussianFile
            // 
            this.butSelectRussianFile.Location = new System.Drawing.Point(271, 44);
            this.butSelectRussianFile.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.butSelectRussianFile.Name = "butSelectRussianFile";
            this.butSelectRussianFile.Size = new System.Drawing.Size(216, 24);
            this.butSelectRussianFile.TabIndex = 8;
            this.butSelectRussianFile.Text = "Select Russian File";
            this.butSelectRussianFile.UseVisualStyleBackColor = true;
            // 
            // butSelectResultFile
            // 
            this.butSelectResultFile.Location = new System.Drawing.Point(40, 119);
            this.butSelectResultFile.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.butSelectResultFile.Name = "butSelectResultFile";
            this.butSelectResultFile.Size = new System.Drawing.Size(216, 24);
            this.butSelectResultFile.TabIndex = 10;
            this.butSelectResultFile.Text = "Select Result File";
            this.butSelectResultFile.UseVisualStyleBackColor = true;
            // 
            // butCreateResultFile
            // 
            this.butCreateResultFile.Location = new System.Drawing.Point(40, 195);
            this.butCreateResultFile.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.butCreateResultFile.Name = "butCreateResultFile";
            this.butCreateResultFile.Size = new System.Drawing.Size(216, 24);
            this.butCreateResultFile.TabIndex = 11;
            this.butCreateResultFile.Text = "Create New Result File";
            this.butCreateResultFile.UseVisualStyleBackColor = true;
            // 
            // fld2CreateResultFileName
            // 
            this.fld2CreateResultFileName.Location = new System.Drawing.Point(40, 165);
            this.fld2CreateResultFileName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.fld2CreateResultFileName.Name = "fld2CreateResultFileName";
            this.fld2CreateResultFileName.Size = new System.Drawing.Size(216, 22);
            this.fld2CreateResultFileName.TabIndex = 12;
            this.fld2CreateResultFileName.Text = "sampleResultTextDoc.txt";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(268, 167);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(223, 17);
            this.label6.TabIndex = 18;
            this.label6.Text = "Name to create file for Splitting text";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(35, 238);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(377, 17);
            this.label7.TabIndex = 19;
            this.label7.Text = "3. Please, open all selected files and start text splitting";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(35, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(362, 17);
            this.label4.TabIndex = 16;
            this.label4.Text = "2. Please, select or create result file for splitting text";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(36, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(451, 17);
            this.label1.TabIndex = 13;
            this.label1.Text = "1. Please, select files with synchronous English and Russian texts";
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(40, 278);
            this.Cancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(216, 24);
            this.Cancel.TabIndex = 20;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.statusBottomLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 390);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(539, 24);
            this.statusStrip1.TabIndex = 21;
            this.statusStrip1.Text = "Implementation progress status";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Font = new System.Drawing.Font("Verdana", 10F);
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 19);
            // 
            // statusBottomLabel
            // 
            this.statusBottomLabel.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Italic);
            this.statusBottomLabel.Name = "statusBottomLabel";
            this.statusBottomLabel.Size = new System.Drawing.Size(158, 19);
            this.statusBottomLabel.Text = "toolStripStatusLabel2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(37, 367);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(276, 19);
            this.label2.TabIndex = 22;
            this.label2.Text = "Implementation progress status:";
            // 
            // TextSplitOpenForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(539, 414);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.fld2CreateResultFileName);
            this.Controls.Add(this.butCreateResultFile);
            this.Controls.Add(this.butSelectResultFile);
            this.Controls.Add(this.butSelectRussianFile);
            this.Controls.Add(this.butAllFilesOpen);
            this.Controls.Add(this.butSelectEnglishFile);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "TextSplitOpenForm";
            this.Text = "TextSplitOpenForm";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button butSelectEnglishFile;
        private System.Windows.Forms.Button butSelectRussianFile;
        private System.Windows.Forms.Button butSelectResultFile;
        private System.Windows.Forms.Button butAllFilesOpen;
        private System.Windows.Forms.Button butCreateResultFile;
        private System.Windows.Forms.TextBox fld2CreateResultFileName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel statusBottomLabel;
        private System.Windows.Forms.Label label2;
    }
}