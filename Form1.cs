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
    public partial class Form1 : Form
    {
        public static Form1 instance;
        public List<string> blockedsites, blockedprograms;
        RegistryKey currentUser = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.CurrentUser, RegistryView.Registry64);
        string key = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\DisallowRun";
        string key2 = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer";
        public Form1()
        {
            InitializeComponent();
            instance = this;
            listView1.MultiSelect = false;
            listView2.MultiSelect = false;
            blockedsites = new List<string>();
            blockedprograms = new List<string>();
        }
        
        public static void runCmd(string command)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = command;
            process.StartInfo = startInfo;
            process.Start();
        }

        void importReg(string file)
        {
            Process regeditProcess = Process.Start("regedit.exe", "/s " + file);
            regeditProcess.WaitForExit();
        }

        void deleteLineHosts(string text)
        {
            File.Copy(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers/etc/hosts"), "temp", true);
            string file = File.ReadAllText("temp");
            if (text.Contains("www."))
                file = file.Replace("127.0.0.1 " + text, "");
           else
                file = file.Replace("127.0.0.1 www." + text, "");
            File.WriteAllText("temp", file);
            File.Copy("temp", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers/etc/hosts"), true);
            runCmd("ipconfig /flushdns");
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems[0].Index > -1)
            {
                deleteLineHosts(listView1.SelectedItems[0].Text);

                blockedsites.Remove(listView1.SelectedItems[0].Text);
                listView1.Items.RemoveAt(listView1.SelectedItems[0].Index);
                MessageBox.Show("Đã xóa trang web khỏi danh sách.", "ĐÃ XÓA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                fileInOut.saveSites();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.firstTime == "c#a")//true
            {
                string refpath= "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer";
                System.IO.File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers/etc/hosts"), string.Empty);
                RegistryKey regkey = currentUser.OpenSubKey(key2, true);
                if (Registry.GetValue(key2, "DisallowRun", null) != null)
                    Registry.CurrentUser.DeleteSubKey(refpath + "\\DisallowRun", true);
                regkey = currentUser.CreateSubKey("DisallowRun");
                regkey = currentUser.OpenSubKey(key2, true);
                if (Registry.GetValue(key2, "RestrictRun", null) != null)
                    Registry.CurrentUser.DeleteSubKey(refpath + "\\RestrictRun", true);
                regkey = currentUser.CreateSubKey("RestrictRun"); //de ti tim cacho doi qua dang bien cho de nhin hon
                regkey.Close();
                /*importReg("config.reg");
                importReg("disallowRun.reg");*/
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "DisallowRun", 1,
            RegistryValueKind.QWord);
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "RestrictRun", 0,
            RegistryValueKind.QWord);
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\RestrictRun", "1", "Blocker.exe",
            RegistryValueKind.String);
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\RestrictRun", "2", "regedit.exe",
            RegistryValueKind.String);
                //MessageBox.Show("Đã nạp cấu hình sử dụng lần đầu");
                Properties.Settings.Default.firstTime = "@t!23e"; //false

                Properties.Settings.Default.blockSites = false;
                Properties.Settings.Default.blockApps = false;
                Properties.Settings.Default.restrictRunApps = false;
                Properties.Settings.Default.Save();
            }
            else
            {
                fileInOut.loadApps();
                fileInOut.loadSites();

            }
            if (Properties.Settings.Default.blockSites)
                radioButton1.Checked = true;
            else
            {
                radioButton1.Checked = false;
                radioButton3.Checked = true;
                listView1.Enabled = false;
                button1.Enabled = false;
                button2.Enabled = false;
            }
            if (Properties.Settings.Default.blockApps)
            {
                button6.Enabled = false;
                label8.Text = button6.Text;
            }
            else if (Properties.Settings.Default.restrictRunApps)
            {
                button7.Enabled = false;
                label8.Text = button7.Text;
            }
            else
            {
                button8.Enabled = false;
                label8.Text = button8.Text;
                listView2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedItems[0].Index > -1)
            {
                var item = listView2.SelectedItems[0];
                int index = listView2.Items.IndexOf(item) + 1;
                removeProgramAt(index);
                blockedprograms.Remove(item.Text);
                listView2.Items.RemoveAt(item.Index);
                MessageBox.Show("Đã xóa ứng dụng khỏi danh sách!");
                fileInOut.saveApps();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {//bật block sites
            if (radioButton1.Checked)
            {
                turnOnBlockSites();
                Properties.Settings.Default.blockSites = true;
                radioButton3.Checked = false;
                listView1.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
            }
            else
            {
                turnOffBlockSites();
                radioButton3.Checked = true;
                Properties.Settings.Default.blockSites = false;
                listView1.Enabled = false;
                button1.Enabled = false;
                button2.Enabled = false;
            }
            Properties.Settings.Default.Save();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {//tắt block sites
            if (radioButton3.Checked)
            {
                turnOffBlockSites();
                Properties.Settings.Default.blockSites = false;
                radioButton1.Checked = false;
                listView1.Enabled = false;
                button1.Enabled = false;
                button2.Enabled = false;
            }
            else
            {
                turnOnBlockSites();
                radioButton1.Checked = true;
                Properties.Settings.Default.blockSites = true;
                listView1.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
            }
            Properties.Settings.Default.Save();
        }
        void removeProgramAt(int index)
        {
            for (int i = listView2.Items.Count; i > index; i--)
            {
                string s = (string)Registry.GetValue(key, i.ToString(), ""); //lấy giá trị key
                Registry.SetValue(key, (i - 1).ToString(), s,
            RegistryValueKind.String); //lùi xuống các ứng dụng kh bị xóa (giống như thuật toán xóa một phần tử trong một list)
                s = (string)Registry.GetValue(key2 + @"\RestrictRun", (i + 2).ToString(), "");
                Registry.SetValue(key2 + @"\RestrictRun", (i + 1).ToString(), s,
            RegistryValueKind.String);
            }
            Registry.SetValue(key2 + @"\RestrictRun", (listView2.Items.Count + 2).ToString(), String.Empty,
            RegistryValueKind.String);
            Registry.SetValue(key, (listView2.Items.Count).ToString(), String.Empty,
            RegistryValueKind.String); //xóa phần tử cuối

        }

        void turnOnBlockApps()
        {
            Registry.SetValue(key2, "DisallowRun", 1,
           RegistryValueKind.DWord);
            Registry.SetValue(key2, "RestrictRun", 0,
           RegistryValueKind.DWord);
            // dat disallowRun thanh 1
        }

        void turnOffBlockSites()
        {
            File.Copy("emptyhosts", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers/etc/hosts"), true);
            // load file hosts trống
        }
        void turnOnBlockSites()
        {
            File.Copy("writtenhosts", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers/etc/hosts"), true);
            // load file hosts có lưu tên site
        }

        private void button5_Click(object sender, EventArgs e)
        {
            password pw = new password();
            pw.ShowDialog();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.facebook.com/tungvuong.2003/");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            addWebsite aw = new addWebsite();
            aw.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            addProgram ap = new addProgram();
            ap.ShowDialog();
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count == 0)
                return;
        }



        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;
        }
        void turnOnRestrictRun()
        {
            Registry.SetValue(key2, "DisallowRun", 0,
               RegistryValueKind.DWord);
            Registry.SetValue(key2, "RestrictRun", 1,
           RegistryValueKind.DWord);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            turnOff();
            button7.Enabled = false;
            label8.Text = button6.Text;
            button6.Enabled = true;
            button8.Enabled = true;
            listView2.Enabled = true;
            button4.Enabled = true;
            button3.Enabled = true;
            turnOnRestrictRun();
            Properties.Settings.Default.blockApps = false;
            Properties.Settings.Default.restrictRunApps = true;
            Properties.Settings.Default.Save();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            turnOff();
            button6.Enabled = false;
            label8.Text = button7.Text;
            button7.Enabled = true;
            button8.Enabled = true;
            listView2.Enabled = true;
            button4.Enabled = true;
            button3.Enabled = true;
            turnOnBlockApps();
            Properties.Settings.Default.blockApps = true;
            Properties.Settings.Default.restrictRunApps = false;
            Properties.Settings.Default.Save();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            turnOff();
            button8.Enabled = false;
            label8.Text = button8.Text;
            button7.Enabled = true;
            button8.Enabled = true;
            listView2.Enabled = false;
            button4.Enabled = false;
            button3.Enabled = false;
            Properties.Settings.Default.blockApps = false;
            Properties.Settings.Default.restrictRunApps = false;
            Properties.Settings.Default.Save();
        }


        void turnOff() //tat ca 2 che do
        {
            Registry.SetValue(key2, "DisallowRun", 0,
               RegistryValueKind.DWord);
            Registry.SetValue(key2, "RestrictRun", 0,
           RegistryValueKind.DWord);
        }


    }


}
