namespace SharpLogger.Tryout
{
    partial class PanelTryoutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PanelTryoutForm));
            this.panelTop = new System.Windows.Forms.Panel();
            this.button10 = new System.Windows.Forms.Button();
            this.button1000 = new System.Windows.Forms.Button();
            this.button100 = new System.Windows.Forms.Button();
            this.logPanel = new SharpLogger.LogPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.button1);
            this.panelTop.Controls.Add(this.button10);
            this.panelTop.Controls.Add(this.button1000);
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
            // logPanel
            // 
            this.logPanel.AutoScroll = true;
            this.logPanel.AutoScrollMinSize = new System.Drawing.Size(0, 15);
            this.logPanel.BackColor = System.Drawing.Color.Black;
            this.logPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logPanel.FontSize = 12F;
            this.logPanel.Location = new System.Drawing.Point(0, 63);
            this.logPanel.Name = "logPanel";
            this.logPanel.SelectingBack = System.Drawing.Color.LightSteelBlue;
            this.logPanel.SelectionBack = System.Drawing.Color.SteelBlue;
            this.logPanel.SelectionFront = System.Drawing.Color.White;
            this.logPanel.Size = new System.Drawing.Size(800, 387);
            this.logPanel.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(262, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(79, 39);
            this.button1.TabIndex = 3;
            this.button1.Text = "0 Lines";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // PanelTryoutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.logPanel);
            this.Controls.Add(this.panelTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PanelTryoutForm";
            this.Text = "Sharp Logger Panel Tryout";
            this.panelTop.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button button100;
        private System.Windows.Forms.Button button1000;
        private System.Windows.Forms.Button button10;
        private LogPanel logPanel;
        private System.Windows.Forms.Button button1;
    }
}

