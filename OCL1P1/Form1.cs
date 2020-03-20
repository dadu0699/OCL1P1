﻿using OCL1P1.analyzer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OCL1P1
{
    public partial class Form1 : Form
    {
        private LexicalAnalyzer lexicalAnalyzer;
        private SyntacticAnalyzer syntacticAnalyzer;
        private Interpreter interpreter;
        private int countTab;
        private int indexImage;
        private List<string> images;

        public Form1()
        {
            InitializeComponent();
            countTab = 1;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                RestoreDirectory = true,
                FileName = "",
                DefaultExt = "ER",
                Filter = "Archivos ER (*.er)|*.er"
            };

            RichTextBox richTextBox = tabControl1.SelectedTab.Controls.Cast<RichTextBox>().FirstOrDefault(x => x is RichTextBox);
            string line = "";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                richTextBox.Clear();
                StreamReader streamReader = new StreamReader(openFileDialog.FileName);
                while (line != null)
                {
                    line = streamReader.ReadLine();
                    if (line != null)
                    {
                        richTextBox.AppendText(line);
                        richTextBox.AppendText(Environment.NewLine);
                    }
                }
                streamReader.Close();

                tabControl1.SelectedTab.Text = openFileDialog.FileName;
            }
        }

        private void saveAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = @"C:\",
                RestoreDirectory = true,
                FileName = "",
                DefaultExt = "ER",
                Filter = "Archivos ER (*.er)|*.er"
            };

            RichTextBox richTextBox = tabControl1.SelectedTab.Controls.Cast<RichTextBox>().FirstOrDefault(x => x is RichTextBox);

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream fileStream = saveFileDialog.OpenFile();
                StreamWriter streamWriter = new StreamWriter(fileStream);
                streamWriter.Write(richTextBox.Text);
                streamWriter.Close();
                fileStream.Close();

                tabControl1.SelectedTab.Text = saveFileDialog.FileName;
                Console.WriteLine("Archivo " + saveFileDialog.FileName + " guardado con exito");
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(tabControl1.SelectedTab.Text))
            {
                RichTextBox richTextBox = tabControl1.SelectedTab.Controls.Cast<RichTextBox>().FirstOrDefault(x => x is RichTextBox);

                StreamWriter streamWriter = new StreamWriter(tabControl1.SelectedTab.Text);
                streamWriter.Write(richTextBox.Text);
                streamWriter.Close();
            }
            else
            {
                saveAs();
            }
        }

        private void newTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            countTab++;
            TabPage tabPage = new TabPage("Tab" + countTab);

            RichTextBox richTextBox = new RichTextBox
            {
                BorderStyle = BorderStyle.None,
                Font = new Font("Microsoft Sans Serif", 12),
                Dock = DockStyle.Fill,
                Multiline = true,
                AcceptsTab = true
            };

            tabPage.Controls.Add(richTextBox);
            tabControl1.TabPages.Add(tabPage);
        }

        private void tabControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                for (int ix = 0; ix < tabControl1.TabCount; ++ix)
                {
                    if (tabControl1.GetTabRect(ix).Contains(e.Location))
                    {
                        tabControl1.TabPages.RemoveAt(ix);
                        break;
                    }
                }
            }
        }

        private void saveTokensToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lexicalAnalyzer != null)
            {
                lexicalAnalyzer.GenerateReportToken();
            }
        }

        private void saveErrorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lexicalAnalyzer != null)
            {
                lexicalAnalyzer.GenerateReportLexicalErrors();
            }
        }

        private void ThompsonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox richTextBox = tabControl1.SelectedTab.Controls.Cast<RichTextBox>().FirstOrDefault(x => x is RichTextBox);
            string content = richTextBox.Text;


            indexImage = 0;
            images = new List<string>();
            if (automataImage.Image != null)
            {
                automataImage.Image.Dispose();

            }
            automataImage.Image = null;

            lexicalAnalyzer = new LexicalAnalyzer();
            lexicalAnalyzer.Scanner(content);

            if (lexicalAnalyzer.ListError.Count() == 0)
            {
                syntacticAnalyzer = new SyntacticAnalyzer(lexicalAnalyzer.ListToken);
                if (syntacticAnalyzer.ListError.Count() == 0)
                {
                    interpreter = new Interpreter(lexicalAnalyzer.ListToken);

                    images.AddRange(interpreter.RoutesNFA);
                    if (images.Count > 0)
                    {
                        LoadImage(0);
                    }
                }
                else
                {
                    syntacticAnalyzer.GenerateReports();
                }
            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            if (images != null && images.Count > 0)
            {
                indexImage++;

                if (indexImage > images.Count - 1)
                {
                    indexImage = 0;
                }

                LoadImage(indexImage);
            }
        }

        private void prevButton_Click(object sender, EventArgs e)
        {
            if (images != null && images.Count > 0)
            {
                indexImage--;

                if (indexImage < 0)
                {
                    indexImage = images.Count - 1;
                }

                LoadImage(indexImage);
            }
        }

        private void LoadImage(int index)
        {
            if (automataImage.Image != null)
            {
                automataImage.Image.Dispose();

            }
            automataImage.Image = null;

            Image image = Image.FromFile(images[index]);
            automataImage.Image = image;
        }

        private void automataImage_DoubleClick(object sender, EventArgs e)
        {
            if (automataImage.Image != null)
            {
                Process.Start(images[indexImage]);
            }
        }

        private void SymbolTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (interpreter != null)
            {
                interpreter.GenerateReports();
            }
        }
    }
}
