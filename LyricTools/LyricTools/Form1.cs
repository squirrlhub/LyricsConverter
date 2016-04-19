using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LyricTools
{
    public partial class Form1 : Form
    {
        string[] inputFiles = { "" };
        string outputFolder = "";
        public string SongVersesFileLocation = "";

        public Form1()
        {
            Program.form1 = this;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (inputFiles == null || String.IsNullOrEmpty(inputFiles[0]))
                {
                    MessageBox.Show(this, "Please select the input file(s)", "No Input File(s) Given", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    return;
                }
                if (String.IsNullOrEmpty(outputFolder))
                {
                    MessageBox.Show(this, "Please specify an output folder", "No Output Folder Given", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    return;
                }

                Program.conversionTools = new ConversionTools();

                //(if from Lyrix)
                Program.additionalOptions1 = new AdditionalOptions1();
                Program.additionalOptions1.ShowDialog(); // wait for user response

                if (!string.IsNullOrWhiteSpace(SongVersesFileLocation))
                {
                    Program.conversionTools.SetSongVersesFile(SongVersesFileLocation);
                }

                foreach (string file in inputFiles)
                {

                    string result = Program.conversionTools.StartConversion(file, outputFolder);
                    Console.WriteLine(result);
                    logTextbox.AppendText(result + "\r\n");
                }
            }
            catch (Exception e2)
            {
                MessageBox.Show(this, "An error has occurred: " + e2.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// InputFile select
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Multiselect = true;
            openFileDialog1.Title = "Choose input file(s)";
            openFileDialog1.ReadOnlyChecked = true;
            openFileDialog1.Filter = "Exported Lyrix text files (*.txt) | *.txt; *.*";
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                if (openFileDialog1.FileNames.Length < 1)
                {
                    MessageBox.Show(this, "Please select the input file(s)", "No Input File(s) Given", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    return;
                }
                else if (openFileDialog1.FileNames.Length == 1)
                    inputTextbox.Text = openFileDialog1.FileNames[0];
                else
                    inputTextbox.Text = "[Multiple files selected]";

                inputFiles = openFileDialog1.FileNames;
            }
        }

        /// <summary>
        /// OutputFile select
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            folderBrowserDialog1.Description = "Choose output folder";
            folderBrowserDialog1.ShowNewFolderButton = true;
            folderBrowserDialog1.ShowDialog();
            outputFolder = folderBrowserDialog1.SelectedPath;
            outputTextbox.Text = outputFolder;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sourceFileTypeCombo.SelectedIndex = 0;
            destFileTypeCombo.SelectedIndex = 0;

#if DEBUG
            inputFiles = new[] { @"C:\Users\visse\Desktop\ZEH002.TXT" };
            outputFolder = @"C:\Users\visse\Desktop";
#endif
        }

        private void inputTextbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
