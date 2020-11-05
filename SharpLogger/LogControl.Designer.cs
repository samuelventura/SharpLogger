namespace SharpLogger
{
    partial class LogControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.controlPanel = new System.Windows.Forms.TableLayoutPanel();
            this.copyButton = new System.Windows.Forms.Button();
            this.freezeViewCheckBox = new System.Windows.Forms.CheckBox();
            this.showDebugCheckBox = new System.Windows.Forms.CheckBox();
            this.clearButton = new System.Windows.Forms.Button();
            this.logPanel = new SharpLogger.LogPanel();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.controlPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // controlPanel
            // 
            this.controlPanel.AutoSize = true;
            this.controlPanel.ColumnCount = 5;
            this.controlPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.controlPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.controlPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.controlPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.controlPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.controlPanel.Controls.Add(this.copyButton, 4, 0);
            this.controlPanel.Controls.Add(this.freezeViewCheckBox, 2, 0);
            this.controlPanel.Controls.Add(this.showDebugCheckBox, 1, 0);
            this.controlPanel.Controls.Add(this.clearButton, 0, 0);
            this.controlPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.controlPanel.Location = new System.Drawing.Point(0, 0);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.RowCount = 1;
            this.controlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.controlPanel.Size = new System.Drawing.Size(654, 34);
            this.controlPanel.TabIndex = 18;
            // 
            // copyButton
            // 
            this.copyButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.copyButton.Location = new System.Drawing.Point(576, 3);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(75, 28);
            this.copyButton.TabIndex = 8;
            this.copyButton.Text = "Copy";
            this.copyButton.UseVisualStyleBackColor = true;
            this.copyButton.Click += new System.EventHandler(this.CopyButton_Click);
            // 
            // freezeViewCheckBox
            // 
            this.freezeViewCheckBox.AutoSize = true;
            this.freezeViewCheckBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.freezeViewCheckBox.Location = new System.Drawing.Point(181, 3);
            this.freezeViewCheckBox.Name = "freezeViewCheckBox";
            this.freezeViewCheckBox.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.freezeViewCheckBox.Size = new System.Drawing.Size(87, 28);
            this.freezeViewCheckBox.TabIndex = 5;
            this.freezeViewCheckBox.Text = "Freeze View";
            this.freezeViewCheckBox.UseVisualStyleBackColor = true;
            this.freezeViewCheckBox.CheckedChanged += new System.EventHandler(this.FreezeViewCheckBox_CheckedChanged);
            // 
            // showDebugCheckBox
            // 
            this.showDebugCheckBox.AutoSize = true;
            this.showDebugCheckBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.showDebugCheckBox.Location = new System.Drawing.Point(84, 3);
            this.showDebugCheckBox.Name = "showDebugCheckBox";
            this.showDebugCheckBox.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.showDebugCheckBox.Size = new System.Drawing.Size(91, 28);
            this.showDebugCheckBox.TabIndex = 4;
            this.showDebugCheckBox.Text = "Show Debug";
            this.showDebugCheckBox.UseVisualStyleBackColor = true;
            this.showDebugCheckBox.CheckedChanged += new System.EventHandler(this.ShowDebugCheckBox_CheckedChanged);
            // 
            // clearButton
            // 
            this.clearButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.clearButton.Location = new System.Drawing.Point(3, 3);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 28);
            this.clearButton.TabIndex = 2;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // logPanel
            // 
            this.logPanel.AutoScroll = true;
            this.logPanel.BackColor = System.Drawing.Color.Black;
            this.logPanel.DebugColor = System.Drawing.Color.LightGray;
            this.logPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logPanel.ErrorColor = System.Drawing.Color.Tomato;
            this.logPanel.InfoColor = System.Drawing.Color.White;
            this.logPanel.Location = new System.Drawing.Point(0, 34);
            this.logPanel.Name = "logPanel";
            this.logPanel.SelectionBack = System.Drawing.Color.DodgerBlue;
            this.logPanel.SelectionFront = System.Drawing.Color.White;
            this.logPanel.Size = new System.Drawing.Size(654, 380);
            this.logPanel.TabIndex = 19;
            this.logPanel.WarnColor = System.Drawing.Color.Yellow;
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // LogControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.logPanel);
            this.Controls.Add(this.controlPanel);
            this.DoubleBuffered = true;
            this.Name = "LogControl";
            this.Size = new System.Drawing.Size(654, 414);
            this.controlPanel.ResumeLayout(false);
            this.controlPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel controlPanel;
        private System.Windows.Forms.Button copyButton;
        private System.Windows.Forms.CheckBox freezeViewCheckBox;
        private System.Windows.Forms.CheckBox showDebugCheckBox;
        private System.Windows.Forms.Button clearButton;
        private LogPanel logPanel;
        private System.Windows.Forms.Timer timer;
    }
}
