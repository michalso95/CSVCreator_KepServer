using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;
using System.Windows;


namespace AddTagsIOM
{
    class ReadWrite
    {
        string pathstr, namestr;
        public int ammount;
        public List<string> tagList = new List<string>();
        //private string filepath;

        public ReadWrite(string name)
        {
            pathstr = name;
            namestr = pathstr.Substring(0, (pathstr.IndexOf(".")));
            Read(pathstr);
        }

        private void Read(string path) 
        {
            ammount = 0;
            StringBuilder sb = new StringBuilder();
            StreamReader sr = new StreamReader(path);
            string s;
            do
            {
                s = sr.ReadLine();
                sb.AppendLine(s);
                try
                {
                    tagList.Add(s);
                }
                catch (Exception)
                {
                   // 
                }

                ammount++;
            } while (s != null);
            sr.Close();
        }

        public void WriteCSV(List<string> lista, string typ) 
        {
            string[] text1;
            string outPath = string.Format("{0}{1}CSV.csv", namestr, typ);
            text1 = lista.ToArray();
            text1 = text1.Take(text1.Count() - 1).ToArray();
            System.IO.File.WriteAllLines(outPath, text1);
            //filepath = outPath;
        }

        public void AddTag(string item, string type) 
        {
            string pathAdd;
            if(type.Contains("REST"))
                pathAdd = string.Format("{0}{1}CSV.csv", namestr, type);
            else if (type.Contains("Device"))
                pathAdd = string.Format("{0}{1}CSV.csv", namestr, type);
            else
            {
                MessageBox.Show("Err with file");
                return;
            }
            if (!System.IO.File.Exists(pathAdd))
            {
                MessageBox.Show("File doesn't exist");
                return;
            }
            else 
            {
                System.IO.File.AppendAllText(pathAdd, item);
            }
        }
    }
}
