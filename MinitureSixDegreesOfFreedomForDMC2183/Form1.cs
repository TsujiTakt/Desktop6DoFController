using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinitureSixDegreesOfFreedomForDMC2183
{
    public partial class Form1 : Form
    {
        gclib gclib = null;
        public Form1()
        {
            InitializeComponent();
            try
            {
                gclib = new gclib();
                gclib.GOpen("192.168.11.254 --direct --subscribe ALL"); //Set an appropriate IP address here
                gclib.GArrayDownloadFile(@"C:\Users\tsuji\C\beta.csv");
                //gclib.GArrayUploadFile(@"C:\Users\tsuji\C\ates.csv", "tP");
                System.Diagnostics.Debug.WriteLine("GInfo: " + gclib.GInfo());
                gclib.GClose();
                gclib = null;
                this.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: " + ex.Message);
                MessageBox.Show("ERROR: " + ex.Message);
            }
            finally
            {
                
            }
        }
    }
}
