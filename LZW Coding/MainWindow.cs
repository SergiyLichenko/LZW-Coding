using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace LZW_Coding
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private char Arhieve_getChar(int byteIndex)
        {
            return GetChar(byteIndex);
        }

        private Dictionary<string, int> GetTable()
        {
            Dictionary<string, int> table = new Dictionary<string, int>();
            for (int i = 0; i < 256; i++)
                table.Add(GetChar(i).ToString(), i);
            return table;
        }



        private char GetChar(int index)
        {
            byte[] b = new byte[] { Convert.ToByte(index) };
            return Encoding.Default.GetString(b)[0];
        }



        private void pictureBox4_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            if (sf.ShowDialog() == DialogResult.OK)
                this.textBox1.Text = sf.FileName.Trim();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            Dictionary<string, int> table = GetTable();
            UnArhieve unArhieve = new UnArhieve(table);
            unArhieve.DeArhieve(this.textBox1.Text);
            string time = (DateTime.Now - now).ToString();

            double entropy1 = Statistics(File.OpenRead(Environment.CurrentDirectory + @"\coded.bin"));
            double entropy2 = Statistics(File.OpenRead(this.textBox1.Text.Trim()));
            this.richTextBox1.Text = "Час роботи: " + time + "\nЕнтропія вхідного файлу: " + entropy1 + "\nЕнтропія вихідного файлу: " + entropy2;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            ArhieveE arhieve = new ArhieveE(GetTable());
            arhieve.getChar += Arhieve_getChar;
            arhieve.Arhieve(File.OpenRead(this.textBox_Arhive_From.Text));
            string time = (DateTime.Now - now).ToString();

            double entropy1 = Statistics(File.OpenRead(this.textBox_Arhive_From.Text));
            double entropy2 = Statistics(File.OpenRead(Environment.CurrentDirectory + @"\coded.bin"));
            this.richTextBox1.Text = "Час роботи: " + time + "\nЕнтропія вхідного файлу: " + entropy1 + "\nЕнтропія вихідного файлу: " + entropy2;
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            if (openDialog.ShowDialog() == DialogResult.OK)
                this.textBox_Arhive_From.Text = openDialog.FileName.Trim();
        }
        public double Statistics(FileStream instream)
        {
            int[] mass = new int[256];
            long legth = instream.Length;
            double entropy = 0;
            while (true)            {
                try  {  mass[instream.ReadByte()]++;  }
                catch (Exception){  break;   }            }
            for (int i = 0; i < mass.Length; i++)
            {
                if (mass[i] == 0)
                    continue;
                double chance = (double)mass[i] / (double)legth;
                entropy += -chance * Math.Log(chance, 2);
            }
            return entropy;
        }
    }
}
