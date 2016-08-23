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

namespace MinitureSixDegreesOfFreedomForDMC2183
{
    public partial class MainFragment : Form
    {
        private gclib gclib = null;
        private bool pauseFlag = false;
        private bool stand_by_data = false;
        private bool stand_by_position = false;
        private bool exec = false;
        public MainFragment()
        {
            InitializeComponent();
            try
            {
                gclib = new gclib();
                gclib.GOpen("192.168.11.219 --direct --subscribe ALL"); //Set an appropriate IP address here
                System.Diagnostics.Debug.WriteLine("GInfo: " + gclib.GInfo());
                System.Diagnostics.Debug.WriteLine(gclib.GCommand("MG tP[0];", false));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: " + ex.Message);
                MessageBox.Show("ERROR: " + ex.Message);
                MessageBox.Show("接続を確認してください");
                this.Enabled = false;
            }
            finally
            {
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {


        }

        private void label24_Click(object sender, EventArgs e)
        {

        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            
            if (!pauseFlag && !exec && stand_by_data && stand_by_position)
            {
                try
                {
                    gclib.GCommand("XQ#Run;", false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:" + ex.Message);
                }
                exec = true;
            }
            else if(pauseFlag && exec)
            {
                try {
                    gclib.GCommand("Run=1;");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:" + ex.Message);
                }
                pauseFlag = false;
            }
            else if (!pauseFlag && exec)
            {
                MessageBox.Show("動作中です");
            }
            else 
            {
                MessageBox.Show("準備を完了させてください");
            }
            stand_by_position = false;
            update_pre();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (exec)
            {
                try {
                    gclib.GCommand("Run=0;");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:" + ex.Message);
                }
                pauseFlag = true;
                update_pre();
            }
            else
            {
                MessageBox.Show("停止中です");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            pauseFlag = false;
            exec = false;
            stand_by_position = false;
            update_pre();
            this.Enabled = false;
            try
            {
                gclib.GCommand("Init=0;ST;XQ#Init");
                for (int i = 0; i < 50; i++)
                {
                    System.Threading.Thread.Sleep(200);
                    System.Windows.Forms.Application.DoEvents();
                    if (gclib.GCommand("MG Init;", false).ToCharArray()[1] == '1') break;
                }
                this.Enabled = true;
                stand_by_position = true;
                update_pre();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = @"C:\data\";
            openFileDialog1.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            if (!System.IO.Path.GetExtension(openFileDialog1.FileName).Equals(".csv"))
                            {
                                MessageBox.Show("不正なファイルです");
                                return;
                            }
                            stand_by_data = false;
                            update_pre();
                            // Insert code to read the stream here.
                            gclib.GCommand("ST;AB;", false);
                            exec = false;
                            pauseFlag = false;
                            update_pre();

                            string[][] tes = loadCSV(openFileDialog1.FileName);
                            sending bar = new sending();
                            bar.progressBar1.Minimum = 0;
                            bar.progressBar1.Maximum = 6000;
                            bar.progressBar1.Value = 0;
                            bar.Show();
                            this.Enabled = false;
                            for (int i = 0; i < tes.Length; i++)
                            {
                                //System.Diagnostics.Debug.WriteLine("i : "+i);
                                for (int j = 0; j < tes[i].Length; j++)
                                {
                                    if (i == 0) continue;

                                    //System.Diagnostics.Debug.WriteLine("tes : "+tes[i][j]);



                                    //System.Diagnostics.Debug.WriteLine("tP[" + (i - 1) + "]=" + tes[i][j] + "");
                                    gclib.GCommand("tP[" + (i - 1) + "]=" + tes[i][j], false);
                                   // System.Threading.Thread.Sleep(1);
                                }
                                if(i % 100 == 0)
                                {
                                    bar.progressBar1.Value = i;
                                    System.Windows.Forms.Application.DoEvents();
                                }
                            }
                            //System.Diagnostics.Debug.WriteLine("GInfo: " + gclib.GInfo());
                            bar.un();
                            this.Enabled = true;

                            label21.Text = Path.GetFileName(openFileDialog1.FileName);
                            stand_by_data = true;
                            update_pre();

                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:" + ex.Message);
                }
            }
        }
        private string[][] loadCSV(string filePath)
        {
            var list = new List<string[]>();
            StreamReader reader =
            new StreamReader(filePath, System.Text.Encoding.GetEncoding("Shift_JIS"));
            while (reader.Peek() >= 0) list.Add(reader.ReadLine().Split(','));
            reader.Close();
            return list.ToArray();
        }

        private void MainFragment_FormClosing(object sender, FormClosingEventArgs e)
        {
            try {
                gclib.GCommand("ST;AB;", false);
                gclib.GClose();
                gclib = null;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                gclib.GCommand("ST;XQ#Ori;");
                for (int i = 0; i < 50; i++)
                {
                    System.Threading.Thread.Sleep(200);
                    System.Windows.Forms.Application.DoEvents();
                    if (gclib.GCommand("MG Init;", false).ToCharArray()[1] == '2') break;
                }
                gclib.GCommand("AB;MO;");
                exec = false;
                pauseFlag = false;
                stand_by_position = false;
                update_pre();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message);
            }
            pauseFlag = true;
        }
        private void update_pre()
        {
            if (stand_by_data && stand_by_position)
            {
                this.panel1.BackColor = System.Drawing.Color.FromArgb(139, 195, 74);
                this.label24.ForeColor = System.Drawing.Color.Black;
            }
            else
            {
                this.label24.ForeColor = System.Drawing.Color.Gainsboro;
                this.panel1.BackColor = System.Drawing.Color.Gainsboro;
            }
            if (exec)
            {
                if (pauseFlag)
                {
                    this.panel2.BackColor = System.Drawing.Color.FromArgb(211,47,47);
                    this.label1.ForeColor = System.Drawing.Color.Black;
                    this.label2.Visible = true;
                    this.label1.Visible = false;

                }
                else
                {
                    this.panel2.BackColor = System.Drawing.Color.FromArgb(139, 195, 74);
                    this.label1.ForeColor = System.Drawing.Color.Black;
                    this.label2.Visible = false;
                    this.label1.Visible = true;
                }
            }
            else 
            {
                this.panel2.BackColor = System.Drawing.Color.Gainsboro;
                this.label1.ForeColor = System.Drawing.Color.Gainsboro;
                this.label2.Visible = false;
                this.label1.Visible = false;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
