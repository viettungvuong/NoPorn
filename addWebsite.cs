using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NoPorn
{
    public partial class addWebsite : Form
    {
        public addWebsite()
        {
            InitializeComponent();
        }
        private void addWebsite_KeyUp(object sender, KeyEventArgs e)
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
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
            {
                Form1.instance.listView1.Items.Add(textBox1.Text);
                writeToHosts(textBox1.Text);
                Form1.instance.blockedsites.Add(textBox1.Text);
                fileInOut.saveSites();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Không thể thêm được", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void writeToHosts(string url)
        {
            StreamWriter w = File.AppendText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers/etc/hosts"));

            if (url.Contains("www."))
                w.WriteLine("127.0.0.1 " + url + "\n");
            else
                w.WriteLine("127.0.0.1 www." + url + "\n");
            Form1.runCmd("ipconfig /flushdns");
            w.Close();
            copyHosts();
        }

        void copyHosts()
        {
            File.Copy(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers/etc/hosts"), "writtenhosts", true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "File text|*.txt";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string fileName = ofd.FileName;
                var lines = File.ReadAllLines(fileName);
                foreach (string line in lines)
                {
                    Form1.instance.listView1.Items.Add(line);
                    writeToHosts(line);
                    Form1.instance.blockedsites.Add(line);
                }
            }
            fileInOut.saveSites();
            this.Hide();
        }
    }
}
