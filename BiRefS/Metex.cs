using System;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;

namespace BiRefS
{
    class Metex
    {
        private SerialPort SP;
        public double Voltage()
        {
            Console.WriteLine("Voltage subroutine entered ");
            Stopwatch sw;
            sw = new Stopwatch();
            sw.Reset();
            sw.Start();
            Console.WriteLine(this.ToString() + dataready + " " + sw.ElapsedMilliseconds);
            Console.WriteLine(this.ToString() + dataready + " " + sw.ElapsedMilliseconds);
            while ((!dataready) & (sw.ElapsedMilliseconds < 250))
            {
                //Console.WriteLine(sw.ElapsedMilliseconds);
                // if (sw.ElapsedMilliseconds > 500) break;
            };
            Console.WriteLine(sw.ElapsedMilliseconds);
            if (dataready) return (voltages[0] + voltages[1] + voltages[2] + voltages[3]) / 4.0;
            else return -1.0e11;

        }
        public bool dataready;
        private int metexg;
        private double[] voltages;

        public void RequestData()
        {
            if (this.SP.IsOpen) SP.WriteLine("D");
            this.dataready = false;
        }
        public void InitPort(string PortName)
        {
            if (!this.SP.IsOpen) this.SP.PortName = PortName;
            else
            {
                this.SP.Close();
                this.SP.PortName = PortName;
            }
            this.SP.Open();
        }
        public Metex()
        {
            SP = new SerialPort();
            SP.BaudRate = 9600;
            SP.DataBits = 7;
            SP.StopBits = System.IO.Ports.StopBits.Two;
            SP.RtsEnable = false;
            SP.DtrEnable = true;
            SP.NewLine = "\r";
            SP.PortName = "Com1";
            SP.DataReceived += new SerialDataReceivedEventHandler(ReceiveEventHandler);
            dataready = new bool();
            dataready = false;
            voltages = new double[4];
        }
        private void ReceiveEventHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata;
            indata = "";
            //System.Threading.Thread.Sleep(100);
            indata = sp.ReadLine();
            if (indata.Substring(0, 1) != " " || metexg > 3) metexg = 0;
            double value;
            double.TryParse(indata.Substring(2, 8), NumberStyles.Any, new CultureInfo("en-US"), out value);
            //Console.WriteLine("value "+value);
            voltages[metexg] = value;
            metexg++;
            //            Console.WriteLine("Data Received:");
            //Console.WriteLine(metexg +" "+ indata);

            if (metexg > 3) this.dataready = true;
        }
    }
}
