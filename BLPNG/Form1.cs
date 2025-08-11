using System;
using System.Windows.Forms;
using System.IO;
using FileLoader;
using System.Diagnostics;

namespace BLPNG
{
    public partial class Form1 : Form
    {
        public string folderPath;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Text = "Browse";
            button2.Text = "Convert BLP to PNG";
            this.Text = "BLPNG";
            checkBox1.Text = "Delete original files (.blp)";


            if (File.Exists(Environment.CurrentDirectory + "\\settings\\" + "last_session.txt"))
            {
                try
                {
                    folderPath = File.ReadAllText(Environment.CurrentDirectory + "\\settings\\" + "last_session.txt");
                }
                catch
                {
                    MessageBox.Show("File: " + Environment.CurrentDirectory + "\\settings\\" + "last_session.txt" + " is damaged", "Error", MessageBoxButtons.OK);
                    File.Delete(Environment.CurrentDirectory + "\\settings\\" + " last_session.txt");
                }
            }
            else
            {
                folderPath = null;
            }

            textBox1.Text = folderPath;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            BrowseForFolder();
        }

        private void ProcessFolder(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            FileInfo[] BLPs = directory.GetFiles("*.blp", SearchOption.AllDirectories);

            if (BLPs.Length == 0)
            {
                MessageBox.Show("The selected folder doesn't contain any BLP files.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Confirmation popup
            DialogResult dialogResult = MessageBox.Show($"You are about to convert {BLPs.Length} files.\n\nContinue?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.No)
            {
                MessageBox.Show("The operation has been canceled.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();

            // Progressbar thingy
            progressBar1.Value = 0;
            progressBar1.Maximum = BLPs.Length;

            BLPLoader reader = new BLPLoader();

            foreach (FileInfo BLPFile in BLPs)
            {
                //Console.WriteLine(BLPFile);
                if (!File.Exists(BLPFile.FullName.ToLower().Replace(".blp", ".png")))
                {
                    reader.LoadBLP(BLPFile.FullName);
                    reader.bmp.Save(BLPFile.FullName.ToLower().Replace(".blp", ".png"));
                    if (checkBox1.Checked)
                        File.Delete(BLPFile.FullName);

                    progressBar1.Value += 1;
                }
            }

            sw.Stop();

            MessageBox.Show($"Done converting {BLPs.Length} files in {sw.Elapsed.ToString("mm\\:ss")}.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            progressBar1.Value = 0;
        }

        private void BrowseForFolder()
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                folderPath = folderBrowserDialog1.SelectedPath;
                textBox1.Text = folderPath;
                SaveSession();
            }
            else
            {
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (folderPath == null)
            {
                BrowseForFolder();
            }
            else
            {
                if (Directory.Exists(folderPath))
                {
                    SaveSession();
                    ProcessFolder(folderPath);
                }
                else
                {
                    MessageBox.Show("Folder " + folderPath + " has not been found", "Error", MessageBoxButtons.OK);
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            folderPath = textBox1.Text;
        }

        private void SaveSession()
        {
            if (!Directory.Exists(Environment.CurrentDirectory + "\\settings"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\settings");
            }

            if (!File.Exists(Environment.CurrentDirectory + "\\settings\\" + "last_session.txt"))
            {
                try
                {
                    File.WriteAllText(Environment.CurrentDirectory + "\\settings\\" + "last_session.txt", textBox1.Text);
                }
                catch
                {
                    throw new Exception("Could not create file: " + Environment.CurrentDirectory + "\\settings\\" + "last_session.txt");
                }
            }
            else
            {
                if (File.ReadAllText(Environment.CurrentDirectory + "\\settings\\" + "last_session.txt") != textBox1.Text)
                {
                    File.Delete(Environment.CurrentDirectory + "\\settings\\" + "last_session.txt");
                    File.WriteAllText(Environment.CurrentDirectory + "\\settings\\" + "last_session.txt", textBox1.Text);
                }
            }
        }
    }
}
