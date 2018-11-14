using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BiRefS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Miernikx = new Metex();
            Mierniky = new Metex();
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            foreach (string s in ports)
            {
                xcombo.Items.Add(s);
                ycombo.Items.Add(s);
            }
            try
            {
                string config = System.IO.File.ReadAllText("ports.cfg");
                var result = System.Text.RegularExpressions.Regex.Split(config, "\r\n|\r|\n");
                xcombo.Text = result[0];
                ycombo.Text = result[1];
                
            }
            catch { };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Miernikx.InitPort(xcombo.Text);
            Mierniky.InitPort(ycombo.Text);
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            double x, y;
            timer1.Enabled = false;
            Miernikx.RequestData();
            Mierniky.RequestData();
            x = Miernikx.Voltage();
            y = Mierniky.Voltage();
            x = x / 0.04;
            if ((x>-1000)&&(y>-1000)) chart1.Series[0].Points.AddXY(x, y);
            timer1.Enabled = true;
            //Console.WriteLine(Miernikx.Voltage());
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string config;
            config = xcombo.Text + "\r\n" + ycombo.Text;
            System.IO.File.WriteAllText("ports.cfg", config);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string dane = "";
                for (int i = 0; i < chart1.Series[0].Points.Count; i++) { dane += chart1.Series[0].Points[i].XValue + " " + chart1.Series[0].Points[i].YValues[0] + "\r\n"; };
                System.IO.File.WriteAllText(saveFileDialog1.FileName, dane);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
        }
    }
}
