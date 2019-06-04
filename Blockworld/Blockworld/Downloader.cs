using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
//using System.Drawing;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using Microsoft.VisualBasic;
using System.Xml;
using System.Reflection;
using System.Diagnostics;
//using System.Threading;

namespace Update {

    class Downloader {
        public static string Last(string str) {
            char[] sep = { '.' };
            string[] txt = str.Split(sep);
            return txt[txt.Length-1];
        }
        public static bool Checkupdates() {
            try
            {
                string updatefile = Look("http://www.542407.leerlingsites.nl/blockworld/lastupdate");
/*                string version = Assembly.GetEntryAssembly().GetName().Version.ToString();
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                string version = fvi.FileVersion;//string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                //string assemblyVersion = Assembly.LoadFile('your assembly file').GetName().Version.ToString();
                string fileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
                string productVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;*/
                return updatefile.IndexOf("login") == -1 && Convert.ToInt16(updatefile) > 2;
            }

            catch { return false;
            }
        }
        static string Look(string pad)
        {
            // Create a request for the URL. 		
            WebRequest request = WebRequest.Create(pad);
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            // Display the status.
            Console.Write(response.StatusDescription);
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Cleanup the streams and the response.
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }


        void Download(string[] from, string to)
        {
            //            BackgroundWorker backgroundWorker1;
            //            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            //            backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            //            backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            int ctr = 0;
            using (var client = new WebClient())
            {
                foreach (string File in from)
                {
                    if (File.Length > 2)
                    {
                        //client.UploadData("https://www.542407.leerlingsites.nl/test2.txt", new byte[4]);
                        client.DownloadFileAsync(new Uri(File), to + "File" + ctr + ".LaZ");
                        do
                        {
                            Application.DoEvents();
                        } while (client.IsBusy);
                    }
                    }
            }
        }

    }
}