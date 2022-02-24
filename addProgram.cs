using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NoPorn
{
    public partial class addProgram : Form
    {
        string key = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\DisallowRun";
        string key2 = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\RestrictRun";
        public addProgram()
        {
            InitializeComponent();
        }
        private void addProgram_KeyUp(object sender, KeyEventArgs e)
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

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "File ứng dụng | *.exe";
            if (fd.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = fd.FileName;
            }
        }

        string fileNameFromFullPath(string fullPath)
        {
            if (!fullPath.Contains(@"\"))
                return fullPath;
            string ans = "";
            int i = fullPath.Length - 1;
            while (fullPath[i] != '\\')
            {
                ans += fullPath[i];
                i--;
            }
            ans = new String(ans.ToCharArray().Reverse().ToArray()); //reverse string
            return ans;
        }

        void blockApp(string program)
        {
            Form1.instance.listView2.Items.Add(program);
            Registry.SetValue(key, Form1.instance.listView2.Items.Count.ToString(), program,
            RegistryValueKind.String);
            Registry.SetValue(key2, (Form1.instance.listView2.Items.Count + 2).ToString(), program,
            RegistryValueKind.String);
            Form1.instance.blockedprograms.Add(program);
            MessageBox.Show("Đã thêm ứng dụng vào danh sách");
            MessageBox.Show("Vui lòng khởi động lại máy để thay đổi có hiệu lực!");
            fileInOut.saveApps();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
            {
                blockApp(fileNameFromFullPath(textBox1.Text));
                this.Hide();
            }
        }


        void exportBlockedRegFile()
        {
            string path = "blocked.reg";
            Process regeditProcess = Process.Start("regedit.exe", "/e " + path + " " + key);
            regeditProcess.WaitForExit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "File text|*.txt";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string fileName = ofd.FileName;
                var lines = File.ReadAllLines(fileName);
                foreach (string line in lines)
                {
                    blockApp(fileNameFromFullPath(line));
                }
            }
            this.Hide();
        }
    }
}
