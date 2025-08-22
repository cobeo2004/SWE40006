using System;
using System.Drawing;
using System.Windows.Forms;

namespace Assignment1
{
    public partial class Form1 : Form
    {
        private Button btn;

        public Form1()
        {
            InitializeComponent();
            InitializeButton();
        }

        private void InitializeButton()
        {
            btn = new Button();
            btn.Text = "Click Me For Hello World...";
            btn.Size = new Size(200, 50);
            // Middle of the screen
            btn.Location = new Point(200, 150);
            btn.Click += OnBtnClicked;

            Controls.Add(btn);
        }

        private void OnBtnClicked(object sender, EventArgs e)
        {
            MessageBox.Show("Button clicked! HELOOOOOOOOOOO");
        }
    }
}
