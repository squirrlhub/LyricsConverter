namespace LyricTools
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.inputTextbox = new System.Windows.Forms.TextBox();
            this.outputTextbox = new System.Windows.Forms.TextBox();
            this.logTextbox = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.sourceFileTypeCombo = new System.Windows.Forms.ComboBox();
            this.destFileTypeCombo = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.button1.Location = new System.Drawing.Point(103, 81);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(79, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Convert";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Input";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Output";
            // 
            // inputTextbox
            // 
            this.inputTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inputTextbox.Location = new System.Drawing.Point(71, 30);
            this.inputTextbox.Name = "inputTextbox";
            this.inputTextbox.ReadOnly = true;
            this.inputTextbox.Size = new System.Drawing.Size(381, 20);
            this.inputTextbox.TabIndex = 99;
            this.inputTextbox.Text = "[Choose input files]";
            this.inputTextbox.TextChanged += new System.EventHandler(this.inputTextbox_TextChanged);
            // 
            // outputTextbox
            // 
            this.outputTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputTextbox.Location = new System.Drawing.Point(71, 56);
            this.outputTextbox.Name = "outputTextbox";
            this.outputTextbox.ReadOnly = true;
            this.outputTextbox.Size = new System.Drawing.Size(381, 20);
            this.outputTextbox.TabIndex = 99;
            this.outputTextbox.Text = "[Choose output folder]";
            // 
            // logTextbox
            // 
            this.logTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logTextbox.Location = new System.Drawing.Point(12, 110);
            this.logTextbox.Multiline = true;
            this.logTextbox.Name = "logTextbox";
            this.logTextbox.ReadOnly = true;
            this.logTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logTextbox.Size = new System.Drawing.Size(545, 315);
            this.logTextbox.TabIndex = 5;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(458, 28);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(99, 23);
            this.button2.TabIndex = 10;
            this.button2.Text = "Choose Files...";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(458, 54);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(99, 23);
            this.button3.TabIndex = 11;
            this.button3.Text = "Choose Folder...";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(188, 86);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "from";
            // 
            // sourceFileTypeCombo
            // 
            this.sourceFileTypeCombo.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.sourceFileTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sourceFileTypeCombo.FormattingEnabled = true;
            this.sourceFileTypeCombo.Items.AddRange(new object[] {
            "Lyrix (TXT)"});
            this.sourceFileTypeCombo.Location = new System.Drawing.Point(221, 82);
            this.sourceFileTypeCombo.Name = "sourceFileTypeCombo";
            this.sourceFileTypeCombo.Size = new System.Drawing.Size(117, 21);
            this.sourceFileTypeCombo.TabIndex = 1;
            // 
            // destFileTypeCombo
            // 
            this.destFileTypeCombo.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.destFileTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.destFileTypeCombo.FormattingEnabled = true;
            this.destFileTypeCombo.Items.AddRange(new object[] {
            "OpenLyrics (XML)"});
            this.destFileTypeCombo.Location = new System.Drawing.Point(366, 82);
            this.destFileTypeCombo.Name = "destFileTypeCombo";
            this.destFileTypeCombo.Size = new System.Drawing.Size(117, 21);
            this.destFileTypeCombo.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(344, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(16, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "to";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(569, 437);
            this.Controls.Add(this.destFileTypeCombo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.sourceFileTypeCombo);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.logTextbox);
            this.Controls.Add(this.outputTextbox);
            this.Controls.Add(this.inputTextbox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Lyrix converter";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox inputTextbox;
        private System.Windows.Forms.TextBox outputTextbox;
        private System.Windows.Forms.TextBox logTextbox;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox sourceFileTypeCombo;
        private System.Windows.Forms.ComboBox destFileTypeCombo;
        private System.Windows.Forms.Label label4;
    }
}

