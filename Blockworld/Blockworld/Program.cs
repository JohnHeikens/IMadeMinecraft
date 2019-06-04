using System;
//using Microsoft.VisualBasic;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.IO;
using System.Net;
using System.Windows.Forms;
namespace Wander {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String[] args){
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (Update.Downloader.Checkupdates()) {
                if (MessageBox.Show("do you want to download the updates?") == DialogResult.OK) {
                    using (var client = new WebClient())
                    {
                        FolderBrowserDialog fd = new FolderBrowserDialog();
                        fd.ShowDialog();
                        string File = "http://www.542407.leerlingsites.nl/blockworld/blokkenwereld.zip", Fileto=fd.SelectedPath + "\\blokkenwereld.zip";

                        if (File.Length > 2)
                            {
                                client.DownloadFileAsync(new Uri(File), Fileto);
                                do
                                {
                                    Application.DoEvents();
                                } while (client.IsBusy);
                            }
                        MessageBox.Show("De downloads staan in " + fd.SelectedPath + "\\blokkenwereld.zip");
                    }

                }
            }
            BlockWorld.User U = new BlockWorld.User(AppDomain.CurrentDomain.BaseDirectory + "Sectors\\", AppDomain.CurrentDomain.BaseDirectory);
            U.Run();
        }
    }
}