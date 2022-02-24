using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace NoPorn
{
    public partial class passwordOpen : Form
    {
        bool firstTime;
        public passwordOpen()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!firstTime)
            {
                if (protection.Decrypt(Properties.Settings.Default.password, "@a#efCk") == textBox1.Text)
                {
                    this.Hide();
                    Form1 frm1 = new Form1();
                    frm1.Show();
                }
                else
                {

                    textBox1.Text = "";
                    MessageBox.Show("Bạn nhập sai password, hãy nhập lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {

                Properties.Settings.Default.password = protection.Encrypt(textBox1.Text, "@a#efCk");
                Properties.Settings.Default.Save();
                this.Hide();
                Form1 frm1 = new Form1();
                frm1.Show();
            }
        }
        private void passwordOpen_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    // Simulate clicks on button1.
                    button1.PerformClick();
                    break;

                default:
                    break;
            }
        }
        private void passwordOpen_Load(object sender, EventArgs e)
        {
            textBox1.PasswordChar = '*';
            firstTime = (Properties.Settings.Default.firstTime == "c#a") ? true : false;
            if (firstTime)
            {
                label1.Text = "Hãy đặt password để dùng ứng dụng";
            }
        }
    }
}
