using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace CSVToJSON
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog(this);
            txtPath.Text = openFileDialog1.FileName;
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            try
            {
                var fileName = txtPath.Text;
                if (fileName != string.Empty)
                {
                    string[] FileExt = fileName.Split('.');
                    string FileEx = FileExt[FileExt.Length - 1];
                    if (FileEx.ToLower() == "csv")
                    {
                        saveFileDialog1.ShowDialog(this);
                        var destPath = saveFileDialog1.FileName;
                        
                        StreamWriter sw = new StreamWriter(destPath);
                        var csv = new List<string[]>();
                        var lines = System.IO.File.ReadAllLines(fileName);

                        var txt = System.IO.File.ReadAllText(fileName);
                        var value = CsvToJson(txt);
                      
                        var json = string.Empty;
                        foreach (var s in value)
                        {
                            json += s;
                            json += ",";
                        }
                        if (json.Length > 0)
                        {
                            json = json.Substring(0, json.Length - 1);
                        }

                        json = "{ \"data\":[" + json + "]}";
                        sw.Write(json);
                        sw.Close();
                        MessageBox.Show("File is converted to json.");
                    }
                    else
                    {
                        MessageBox.Show("Invalid File");
                    }

                }
                else
                {
                    MessageBox.Show("File Not Found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            } 
        }

        public List<string> CsvToJson(string value)
        {
            // Get lines.
            if (value == null) return null;
            string[] lines = value.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2) throw new InvalidDataException("Must have header line.");

            // Get headers.
            string[] headers = lines[0].Split(',');
            for (int h = 0; h < headers.Length; h++)
            {
                if (headers[h].Trim().ToLower() == "req qty")
                    headers[h] = "req_qty";
                else if (headers[h].Trim().ToLower() == "item name")
                    headers[h] = "item_name";
                else if (headers[h].Trim().ToLower() == "item number")
                    headers[h] = "item_number";
            }
            // Build JSON array.
            var csv = new List<string>();
           
            for (int i = 1; i < lines.Length; i++)
            {
                //string[] fields = lines[i].Split(',');
                string lineText = lines[i];
                lineText = lineText.Replace("\"\"", ";;");

                try
                {
                    int start = 0;
                    while (true)
                    {
                        if (i == 9671)
                        {
                            i = i;
                        }
                        start = lineText.IndexOf("\"", start);
                        if (start == -1) break;
                        string first = lineText.Substring(0, start);
                        int end = lineText.IndexOf("\"", start + 1);
                        if (end == -1)
                        {
                            //first = lines[i];
                            break;
                        }
                        string last = lineText.Substring(end);
                        lineText = lineText.Substring(start, end - start);
                        int count = lineText.Split(',').Length - 1;
                        lineText = lineText.Replace(",", "##");
                        lineText = first + lineText + last;

                        start = end + 2 + count;
                    }
                }
                catch (Exception e)
                {

                }
                
                string[] fields = lineText.Split(',');
                if (i == 21000) {
                    i = i;
                }
                if (fields.Length != headers.Length) throw new InvalidDataException("Field count must match header count."+i);
                var jsonElements = string.Empty;
                for (int x = 0; x < fields.Length; x++)
                {
                    fields[x] = fields[x].Replace(";;", "\"\"");
                    //jsonElements += "" + headers[x] + "" + ":" + "" + fields[x] + "";
                    jsonElements += "\"" + headers[x] + "\"" + ":" + "\"" + fields[x] + "\"";
                    jsonElements += ",";
                }
                if (jsonElements.Length > 0)
                {
                    jsonElements = jsonElements.Replace("##", ",");
                    jsonElements = jsonElements.Substring(0, jsonElements.Length - 1);
                }
                csv.Add("{" + jsonElements + "}");
            }
           
            return csv;
        }
        

    }
}
