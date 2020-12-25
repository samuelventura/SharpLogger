namespace SharpLogger.Tryout
{
    partial class ControlTryoutForm
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
            this.panelTop = new System.Windows.Forms.Panel();
            this.button10 = new System.Windows.Forms.Button();
            this.buttonException = new System.Windows.Forms.Button();
            this.buttonRefresh100 = new System.Windows.Forms.Button();
            this.buttonRefresh10 = new System.Windows.Forms.Button();
            this.button1000 = new System.Windows.Forms.Button();
            this.checkBoxUseLogger = new System.Windows.Forms.CheckBox();
            this.checkBoxAsyncFeed = new System.Windows.Forms.CheckBox();
            this.button100 = new System.Windows.Forms.Button();
            this.logControl = new SharpLogger.LogControl();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.button10);
            this.panelTop.Controls.Add(this.buttonException);
            this.panelTop.Controls.Add(this.buttonRefresh100);
            this.panelTop.Controls.Add(this.buttonRefresh10);
            this.panelTop.Controls.Add(this.button1000);
            this.panelTop.Controls.Add(this.checkBoxUseLogger);
            this.panelTop.Controls.Add(this.checkBoxAsyncFeed);
            this.panelTop.Controls.Add(this.button100);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(800, 63);
            this.panelTop.TabIndex = 0;
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(177, 12);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(79, 39);
            this.button10.TabIndex = 2;
            this.button10.Text = "10 Lines";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // buttonException
            // 
            this.buttonException.Location = new System.Drawing.Point(680, 12);
            this.buttonException.Name = "buttonException";
            this.buttonException.Size = new System.Drawing.Size(108, 39);
            this.buttonException.TabIndex = 7;
            this.buttonException.Text = "Exception";
            this.buttonException.UseVisualStyleBackColor = true;
            this.buttonException.Click += new System.EventHandler(this.buttonException_Click);
            // 
            // buttonRefresh100
            // 
            this.buttonRefresh100.Location = new System.Drawing.Point(566, 12);
            this.buttonRefresh100.Name = "buttonRefresh100";
            this.buttonRefresh100.Size = new System.Drawing.Size(108, 39);
            this.buttonRefresh100.TabIndex = 6;
            this.buttonRefresh100.Text = "Refresh 100ms";
            this.buttonRefresh100.UseVisualStyleBackColor = true;
            this.buttonRefresh100.Click += new System.EventHandler(this.buttonRefresh100_Click);
            // 
            // buttonRefresh10
            // 
            this.buttonRefresh10.Location = new System.Drawing.Point(452, 12);
            this.buttonRefresh10.Name = "buttonRefresh10";
            this.buttonRefresh10.Size = new System.Drawing.Size(108, 39);
            this.buttonRefresh10.TabIndex = 5;
            this.buttonRefresh10.Text = "Refresh 10ms";
            this.buttonRefresh10.UseVisualStyleBackColor = true;
            this.buttonRefresh10.Click += new System.EventHandler(this.buttonRefresh10_Click);
            // 
            // button1000
            // 
            this.button1000.Location = new System.Drawing.Point(12, 12);
            this.button1000.Name = "button1000";
            this.button1000.Size = new System.Drawing.Size(74, 39);
            this.button1000.TabIndex = 0;
            this.button1000.Text = "1000 Lines";
            this.button1000.UseVisualStyleBackColor = true;
            this.button1000.Click += new System.EventHandler(this.button1000_Click);
            // 
            // checkBoxUseLogger
            // 
            this.checkBoxUseLogger.AutoSize = true;
            this.checkBoxUseLogger.Location = new System.Drawing.Point(365, 24);
            this.checkBoxUseLogger.Name = "checkBoxUseLogger";
            this.checkBoxUseLogger.Size = new System.Drawing.Size(81, 17);
            this.checkBoxUseLogger.TabIndex = 4;
            this.checkBoxUseLogger.Text = "Use Logger";
            this.checkBoxUseLogger.UseVisualStyleBackColor = true;
            this.checkBoxUseLogger.CheckedChanged += new System.EventHandler(this.checkBoxUseLogger_CheckedChanged);
            // 
            // checkBoxAsyncFeed
            // 
            this.checkBoxAsyncFeed.AutoSize = true;
            this.checkBoxAsyncFeed.Location = new System.Drawing.Point(277, 24);
            this.checkBoxAsyncFeed.Name = "checkBoxAsyncFeed";
            this.checkBoxAsyncFeed.Size = new System.Drawing.Size(82, 17);
            this.checkBoxAsyncFeed.TabIndex = 3;
            this.checkBoxAsyncFeed.Text = "Async Feed";
            this.checkBoxAsyncFeed.UseVisualStyleBackColor = true;
            this.checkBoxAsyncFeed.CheckedChanged += new System.EventHandler(this.checkBoxAsyncFeed_CheckedChanged);
            // 
            // button100
            // 
            this.button100.Location = new System.Drawing.Point(92, 12);
            this.button100.Name = "button100";
            this.button100.Size = new System.Drawing.Size(79, 39);
            this.button100.TabIndex = 1;
            this.button100.Text = "100 Lines";
            this.button100.UseVisualStyleBackColor = true;
            this.button100.Click += new System.EventHandler(this.button100_Click);
            // 
            // logControl
            // 
            this.logControl.AutoScroll = true;
            this.logControl.DebugColor = System.Drawing.Color.Gray;
            this.logControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logControl.ErrorColor = System.Drawing.Color.Tomato;
            this.logControl.FontSize = 12F;
            this.logControl.FreezeView = false;
            this.logControl.InfoColor = System.Drawing.Color.White;
            this.logControl.LineLimit = 1000;
            this.logControl.Location = new System.Drawing.Point(0, 63);
            this.logControl.LogBackColor = System.Drawing.Color.Black;
            this.logControl.LogFormat = "{TS:HH:mm:ss.fff} {MESSAGE}";
            this.logControl.Name = "logControl";
            this.logControl.PollPeriod = 100;
            this.logControl.SelectingBack = System.Drawing.Color.LightSteelBlue;
            this.logControl.SelectionBack = System.Drawing.Color.DodgerBlue;
            this.logControl.SelectionFront = System.Drawing.Color.White;
            this.logControl.ShowDebug = false;
            this.logControl.Size = new System.Drawing.Size(800, 387);
            this.logControl.SuccessColor = System.Drawing.Color.PaleGreen;
            this.logControl.TabIndex = 1;
            this.logControl.WarnColor = System.Drawing.Color.Yellow;
            // 
            // ControlTryoutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.logControl);
            this.Controls.Add(this.panelTop);
            this.Name = "ControlTryoutForm";
            this.Text = "Sharp Logger Control Tryout";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TryoutForm_FormClosing);
            this.Load += new System.EventHandler(this.TryoutForm_Load);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button button100;
        private System.Windows.Forms.CheckBox checkBoxAsyncFeed;
        private System.Windows.Forms.CheckBox checkBoxUseLogger;
        private System.Windows.Forms.Button button1000;
        private System.Windows.Forms.Button buttonRefresh100;
        private System.Windows.Forms.Button buttonRefresh10;
        private System.Windows.Forms.Button buttonException;
        private System.Windows.Forms.Button button10;
        private LogControl logControl;
    }
}

