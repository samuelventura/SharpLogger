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
            this.button100 = new System.Windows.Forms.Button();
            this.button100k = new System.Windows.Forms.Button();
            this.logPanel = new SharpLogger.LogPanel();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.button100);
            this.panelTop.Controls.Add(this.button100k);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(800, 63);
            this.panelTop.TabIndex = 0;
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
            // button100k
            // 
            this.button100k.Location = new System.Drawing.Point(126, 12);
            this.button100k.Name = "button100k";
            this.button100k.Size = new System.Drawing.Size(108, 39);
            this.button100k.TabIndex = 0;
            this.button100k.Text = "100k Logs";
            this.button100k.UseVisualStyleBackColor = true;
            this.button100k.Click += new System.EventHandler(this.button100k_Click);
            // 
            // logPanel
            // 
            this.logPanel.AutoScroll = true;
            this.logPanel.BackColor = System.Drawing.Color.Black;
            this.logPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logPanel.Location = new System.Drawing.Point(0, 63);
            this.logPanel.Name = "logPanel";
            this.logPanel.Size = new System.Drawing.Size(800, 387);
            this.logPanel.TabIndex = 1;
            // 
            // TryoutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.logPanel);
            this.Controls.Add(this.panelTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TryoutForm";
            this.Text = "Sharp Logger Control Show Case";
            this.panelTop.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private SharpLogger.LogPanel logPanel;
        private System.Windows.Forms.Button button100k;
        private System.Windows.Forms.Button button100;
    }
}

