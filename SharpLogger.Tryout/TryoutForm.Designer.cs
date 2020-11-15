namespace SharpLogger.Tryout
{
    partial class TryoutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TryoutForm));
            this.panelTop = new System.Windows.Forms.Panel();
            this.checkBoxAsyncFeed = new System.Windows.Forms.CheckBox();
            this.button100 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.logPanel = new SharpLogger.LogPanel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.logControl = new SharpLogger.LogControl();
            this.checkBoxUseLogger = new System.Windows.Forms.CheckBox();
            this.panelTop.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.checkBoxUseLogger);
            this.panelTop.Controls.Add(this.checkBoxAsyncFeed);
            this.panelTop.Controls.Add(this.button100);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(800, 63);
            this.panelTop.TabIndex = 0;
            // 
            // checkBoxAsyncFeed
            // 
            this.checkBoxAsyncFeed.AutoSize = true;
            this.checkBoxAsyncFeed.Location = new System.Drawing.Point(126, 24);
            this.checkBoxAsyncFeed.Name = "checkBoxAsyncFeed";
            this.checkBoxAsyncFeed.Size = new System.Drawing.Size(82, 17);
            this.checkBoxAsyncFeed.TabIndex = 2;
            this.checkBoxAsyncFeed.Text = "Async Feed";
            this.checkBoxAsyncFeed.UseVisualStyleBackColor = true;
            this.checkBoxAsyncFeed.CheckedChanged += new System.EventHandler(this.checkBoxAsyncFeed_CheckedChanged);
            // 
            // button100
            // 
            this.button100.Location = new System.Drawing.Point(12, 12);
            this.button100.Name = "button100";
            this.button100.Size = new System.Drawing.Size(108, 39);
            this.button100.TabIndex = 1;
            this.button100.Text = "100 Logs";
            this.button100.UseVisualStyleBackColor = true;
            this.button100.Click += new System.EventHandler(this.button100_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 63);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 387);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.logPanel);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(792, 361);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "LogPanel";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // logPanel
            // 
            this.logPanel.AutoScroll = true;
            this.logPanel.BackColor = System.Drawing.Color.Black;
            this.logPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logPanel.Location = new System.Drawing.Point(3, 3);
            this.logPanel.Name = "logPanel";
            this.logPanel.SelectionBack = System.Drawing.Color.DodgerBlue;
            this.logPanel.SelectionFront = System.Drawing.Color.White;
            this.logPanel.Size = new System.Drawing.Size(786, 355);
            this.logPanel.TabIndex = 2;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.logControl);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(792, 361);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "LogControl";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // logControl
            // 
            this.logControl.AutoScroll = true;
            this.logControl.DebugColor = System.Drawing.Color.Gray;
            this.logControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logControl.ErrorColor = System.Drawing.Color.Tomato;
            this.logControl.FreezeView = false;
            this.logControl.InfoColor = System.Drawing.Color.White;
            this.logControl.LineLimit = 100;
            this.logControl.Location = new System.Drawing.Point(3, 3);
            this.logControl.LogBackColor = System.Drawing.Color.Black;
            this.logControl.LogFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logControl.LogFormat = "{0:HH:mm:ss.fff} {5}";
            this.logControl.Name = "logControl";
            this.logControl.SelectionBack = System.Drawing.Color.DodgerBlue;
            this.logControl.SelectionFront = System.Drawing.Color.White;
            this.logControl.ShowDebug = false;
            this.logControl.Size = new System.Drawing.Size(786, 355);
            this.logControl.SuccessColor = System.Drawing.Color.PaleGreen;
            this.logControl.TabIndex = 1;
            this.logControl.WarnColor = System.Drawing.Color.Yellow;
            // 
            // checkBoxUseLogger
            // 
            this.checkBoxUseLogger.AutoSize = true;
            this.checkBoxUseLogger.Location = new System.Drawing.Point(214, 24);
            this.checkBoxUseLogger.Name = "checkBoxUseLogger";
            this.checkBoxUseLogger.Size = new System.Drawing.Size(81, 17);
            this.checkBoxUseLogger.TabIndex = 3;
            this.checkBoxUseLogger.Text = "Use Logger";
            this.checkBoxUseLogger.UseVisualStyleBackColor = true;
            this.checkBoxUseLogger.CheckedChanged += new System.EventHandler(this.checkBoxUseLogger_CheckedChanged);
            // 
            // TryoutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panelTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TryoutForm";
            this.Text = "Sharp Logger Tryout";
            this.Load += new System.EventHandler(this.TryoutForm_Load);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button button100;
        private System.Windows.Forms.CheckBox checkBoxAsyncFeed;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private LogPanel logPanel;
        private System.Windows.Forms.TabPage tabPage2;
        private LogControl logControl;
        private System.Windows.Forms.CheckBox checkBoxUseLogger;
    }
}

