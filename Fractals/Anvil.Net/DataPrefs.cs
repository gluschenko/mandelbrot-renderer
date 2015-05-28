//5.05.2014 (класс никто не трогал и не тестировал, но так даже больше безвыходных проблем, которые всегда веселят своим присутствием)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace Anvil.Net
{
    public class DataPrefs
    {
        public static void Start()
        {
            Data = GetData();
            System.Windows.Forms.Application.ApplicationExit += delegate { Save(); };
        }

        public static List<string[]> Data = new List<string[]>();
        public static string PrefsPath = System.Windows.Forms.Application.StartupPath + @"\prefs.anvil";

        public static void SetString(string KeyName, string KeyValue)
        {
            if (HasKey(KeyName))
            {
                foreach (string[] sArray in Data)
                {
                    if (sArray[0] == KeyName) sArray[1] = KeyValue;
                }
            }
            else 
            {
                string[] s = new string[] { KeyName, KeyValue };
                Data.Add(s);
            }
        }

        public static string GetString(string KeyName, string KeyValue = "")
        {
            if (HasKey(KeyName) && KeyName != "")
            {
                foreach (string[] sArray in Data)
                {
                    if (sArray[0] == KeyName) return sArray[1];
                }
            }
            return KeyValue;
        }

        public static void DeleteAll() 
        {
            Data.Clear();
        }

        public static void DeleteKey(string KeyName) 
        {
            foreach (string[] sArray in Data)
            {
                if (sArray[0] == KeyName) sArray[0] = "";
            }
        }

        public static bool HasKey(string KeyName) 
        {
            foreach(string[] sArray in Data)
            {
                if (sArray[0] == KeyName && sArray[0] != "") return true;
            }

            return false;
        }

        private static List<string[]> GetData()
        {
            string filedata = "";

            if (File.Exists(PrefsPath))
            {
                using (StreamReader streamr = File.OpenText(PrefsPath))
                {
                    string l = "";
                    string data = "";
                    while ((l = streamr.ReadLine()) != null)
                    {
                        data += l + "\n";
                    }

                    filedata = data;
                }
            }
            else 
            {
                using (FileStream fstream = File.Create(PrefsPath))
                {
                    byte[] data = new UTF8Encoding(true).GetBytes("");
                    fstream.Write(data, 0, data.Length);
                }

                filedata = "";
            }

            List<string[]> rdata = new List<string[]>();

            if(filedata != "")
            {
                string[] filedataArray = filedata.Split(new char[] { '¿' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in filedataArray)
                {
                    string[] sArray = s.Split(new char[] { '¡' });
                    rdata.Add(sArray);
                }
            }

            return rdata;
        }

        public static void Save()
        {
            string wdata = "";

            foreach(string[] sArray in Data)
            {
                if (sArray[0] != "" && sArray.Length > 1) wdata += string.Format("{0}¡{1}¿", sArray[0].Replace("¿", "?").Replace("¡", "!"), sArray[1].Replace("¿", "?").Replace("¡", "!"));
            }

            using (FileStream fstream = File.Create(PrefsPath))
            {
                byte[] data = new UTF8Encoding(true).GetBytes(wdata);
                fstream.Write(data, 0, data.Length);
            }
        }
    }
}
