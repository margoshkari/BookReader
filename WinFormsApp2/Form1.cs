using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Synthesis;

namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        List<string> fullPath = new List<string>();
        bool isPaused = false;
        Task task;
        public Form1()
        {
            InitializeComponent();
            if (File.Exists("books.txt"))
            {
                fullPath = File.ReadAllLines("books.txt").ToList();
                fullPath.ForEach(item =>
                {
                    if (File.Exists(item))
                        this.bookList.Items.Add(Path.GetFileName(item));
                });
            }
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFile = new OpenFileDialog())
            {
                openFile.Filter = "txt files (*.txt)|*.txt";
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    fullPath.Add(openFile.FileName);
                    this.bookList.Items.Add(Path.GetFileName(openFile.FileName));
                }
            }
        }

        private void playBtn_Click(object sender, EventArgs e)
        {
            if (!isPaused)
            {
                task = new Task(() =>
                {
                    synthesizer.Volume = 100;  // 0...100
                    synthesizer.Rate = -2;     // -10...10// Synchronous
                    synthesizer.Speak(this.textBox1.Text);

                });
                task.Start();
            }
            else
                synthesizer.Resume();
        }

        private void pauseBtn_Click(object sender, EventArgs e)
        {
            synthesizer.Pause();
            isPaused = true;
        }

        private void bookList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((sender as ListBox).SelectedItem != null)
            {
                int index = fullPath.FindIndex(item => item.Contains((sender as ListBox).SelectedItem.ToString()));
                this.textBox1.Text = File.ReadAllText(fullPath[index]);
                isPaused = false;
                synthesizer.Pause();
                synthesizer = new SpeechSynthesizer();
            }
        }

        private void Form1_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            File.WriteAllLines("books.txt", fullPath);
        }
    }
}
