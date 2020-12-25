using System;
using System.Windows.Forms;

namespace SharpLogger.Tryout
{
    public partial class MainForm : Form
    {
        public int Tryout;

        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonPanel_Click(object sender, EventArgs e)
        {
            Tryout = 1;
            Hide();
        }

        private void buttonControl_Click(object sender, EventArgs e)
        {
            Tryout = 2;
            Hide();
        }
    }
}
