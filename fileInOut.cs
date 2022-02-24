using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NoPorn
{
    public static class fileInOut
    {
        public static void loadSites()
        {
            const Int32 BufferSize = 128;
            using (var fileStream = File.OpenRead("sitelist.dat"))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                String line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.SubItems[0].Text = protection.Decrypt(line, "zzier834@@#");
                    Form1.instance.listView1.Items.Add(lvi);
                }
            }
        }

        public static void loadApps()
        {
            const Int32 BufferSize = 128;
            using (var fileStream = File.OpenRead("applist.dat"))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                String line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.SubItems[0].Text = protection.Decrypt(line, "c348e!#");
                    Form1.instance.listView2.Items.Add(lvi);
                }
            }
        }
        public static void saveSites()
        {
            using (StreamWriter save = new StreamWriter("sitelist.dat"))
            {
                foreach (ListViewItem Item in Form1.instance.listView1.Items)
                {
                    save.WriteLine(protection.Encrypt(Item.SubItems[0].Text, "zzier834@@#"));
                }

            }
        }

        public static void saveApps()
        {
            using (StreamWriter save = new StreamWriter("applist.dat"))
            {
                foreach (ListViewItem Item in Form1.instance.listView2.Items)
                {
                    save.WriteLine(protection.Encrypt(Item.SubItems[0].Text, "c348e!#"));
                }

            }
        }
    }
};
