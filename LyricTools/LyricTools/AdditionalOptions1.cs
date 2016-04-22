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
    public partial class AdditionalOptions1 : Form
    {
        public AdditionalOptions1()
        {
            Program.additionalOptions1 = this;
            InitializeComponent();

#if DEBUG
            Program.form1.SongVersesFileLocation = @"C:\Users\ICD\Desktop\new\LyricTools\data\SongsWithVerses.xml";
#endif
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Multiselect = false;
            openFileDialog1.Title = "Choose input file";
            openFileDialog1.ReadOnlyChecked = true;
            openFileDialog1.Filter = "Exported Lyrix Songs file (*.xml) | *.xml; *.*";
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                if (openFileDialog1.FileNames.Length < 1)
                {
                    MessageBox.Show(this, "Please select the input file", "No Input File Given", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    return;
                }
                else if (openFileDialog1.FileNames.Length == 1)
                    inputTextbox.Text = openFileDialog1.FileName;

                Program.form1.SongVersesFileLocation = openFileDialog1.FileName;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
