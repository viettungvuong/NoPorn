using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NoPorn
{
    public partial class password : Form
    {
        public password()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (protection.Decrypt(Properties.Settings.Default.password, "@a#efCk") == textBox1.Text)
            {
                textBox2.Enabled = true;
                button2.Enabled = true;
                button1.Enabled = false;
                textBox1.Enabled = false;
            }
            else
            {
                textBox1.Text = "";
                MessageBox.Show("Bạn nhập sai password, hãy nhập lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
        private void password_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    // Simulate clicks on button1.
                    if (button1.Enabled)
                        button1.PerformClick();
                    else if (button2.Enabled)
                        button2.PerformClick();
                    break;

                default:
                    break;
            }
        }
        private void password_Load(object sender, EventArgs e)
        {
            textBox2.Enabled = false;
            button2.Enabled = false;
            textBox1.PasswordChar = '*';
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.password = protection.Encrypt(textBox2.Text, "@a#efCk");
            Properties.Settings.Default.Save();
            MessageBox.Show("Đã đổi password thành công!");
            this.Hide();
        }
    }
}
