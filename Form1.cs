using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using NI = NationalInstruments.NI4882;

namespace sleep_app
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
            for (int i = 0; i < 12; i++)
            {
                this.dataGridView1.Rows.Add();
            }
        }

        DSO dso1 = new DSO(2);
        /* Start Button */
        private void button1_Click(object sender, EventArgs e)
        {
            string privateTime, si = textBox1.Text;
            float dsoTime, xScale = Convert.ToSingle(si); 
            dso1.clearOffset();
            dso1.setScale(1.0F);
            //dso1.setCoupling(DSO.Coupling.DC);
            dso1.setTrigerLevel(1.5F);
            dso1.setTrigerSlopePos();
            //dso1.sweepNormal();
            dso1.clearMeasure();

            if ((string)listBox1.SelectedItem == "usleep")
            {
                xScale = xScale / 1000000;
            }

            var scaleText = Convert.ToSingle(xScale).ToString("E1");
            dso1.setTimeScale(scaleText);

            serialPort1.Open();

            Thread.Sleep(1000);

            for (int j = 0; j < 12; j++)
            {
                serialPort1.WriteLine(si);
                dsoTime = getHighTime(Convert.ToInt32(si));
								// dsoTime = getHighTime(1);
                privateTime = serialPort1.ReadLine();
                this.dataGridView1.Rows[j].Cells[0].Value = Convert.ToDouble(privateTime) / 325000000;
                this.dataGridView1.Rows[j].Cells[1].Value = dsoTime;
                Thread.Sleep(2000);
                dso1.clearMeasure();
            }   
                
            serialPort1.Close();  
        }

        private float getHighTime(int waitTime)
        {
            dso1.clearMeasure();
            Thread.Sleep(waitTime*5000);
            var data = dso1.getdata();
            var xinc = dso1.getXInc();
            var time = 0;

            foreach (var i in data)
            {
                if (i > 3)
                {
                    time++;
                }
            }

            return (time*xinc);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}

class DSO
{
    public enum Coupling
    {
        DC,
        AC,
        GND
    }
    NI.Device device;
    //Initialize the DSO with an address
    public DSO(byte addr)
    {
        device = new NI.Device(0, addr);        //dso address and board number 0
    }

    public void clearOffset()
    {
        device.Write(":CHAN1:OFFS 0");      //writes 0 to chan1 of the dso
    }

    public void setTimeScale(string scale)
    {
        device.Write(":TIM:SCAL " + scale);           //sends command to set time scale of dso
        this.isDone();
    }

    public void setTimeScale(float scale)
    {
        device.Write(":TIM:SCAL " + scale.ToString("E"));           //sends command to set time scale of dso
        this.isDone();
    }


    public void setScale(float scale)
    {
        device.Write(":CHAN1:SCAL " + scale.ToString("E"));     //sends command to set voltage scale of dso
    }

    //Not really sure what this is for...
    //the documentation said to use it every once in awhile
    //I guess it tells when the last command was finished or something.
    public int isDone()
    {
        device.Write("*OPC?");              //asks if operation is complete in query
        var data = device.ReadString();        //reads the string sent from
        if (data != "\n")
        {
            return int.Parse(data);
        }
        else
        {
            return 0;
        }
    }

    public void setTrigerSlopePos()
    {
        device.Write(":TRIG:SLOP: POS");
    }

    public void autoScale()
    {
        device.Write(":KEY:AUTO_SCALE");
        this.isDone();
    }

    public void sweepNormal()
    {
        device.Write(":TRIGger:EDGE:SWE NORM");
        this.isDone();
    }

    public void setTrigerLevel(float level)
    {
        device.Write(":TRIG:EDGE:LEV " + level.ToString("f"));
    }

    public void setCoupling(Coupling coupling)
    {
        string coup;
        switch (coupling)
        {
            case Coupling.AC:
                coup = "AC";
                break;
            case Coupling.DC:
                coup = "DC";
                break;
            default:
                coup = "GND";
                break;
        }
        device.Write(":CHAN1:COUP " + coup);
        this.isDone();
    }

    public void clearMeasure()
    {
        device.Write(":MEAS:CLE");      //clears the measure input of the device
        this.isDone();                   //dso is done
    }

    public void displayClear()
    {
        device.Write(":DISP:CLE");      //clears the display of the device
        this.isDone();                      //dso is done
    }

    public void single()
    {
        device.Write(":KEY:SINGLE");        //command virtually presses single button on the dso
        this.isDone();                        //dso is done      
    }

    public float getYInc()
    {
        device.Write(":WAV:YINC?");     //get time between y increments
        string YINC = device.ReadString();
        return float.Parse(YINC);
    }

    public float getXInc()
    {
        device.Write(":WAV:XINC?");    //get time between x increments
        string XINC = "";
        try
        {
            XINC = device.ReadString();
        }
        catch
        {
            return this.getXInc();
        }
        return float.Parse(XINC);
    }

    public float getYor()
    {
        device.Write(":WAV:YOR?");     //y orgin
        var YOR = device.ReadString();
        return float.Parse(YOR);
    }

    public float[] getdata()
    {
        string DATA = "";
        try
        {
            device.Write(":WAV:SCREENDATA?");
            do
            {
                DATA += device.ReadString();
            } while (!Regex.IsMatch(DATA, @"\n"));
            DATA = DATA.Replace(" \n", "");
        }
        catch
        {
            device.Write(":WAV:SCREENDATA?");
            do
            {
                DATA += device.ReadString();
            } while (!Regex.IsMatch(DATA, @"\n"));
            DATA = DATA.Replace(" \n", "");
        }
        this.isDone();

        string[] data_str = DATA.Split();
        var data_int = data_str.Select((s) => Convert.ToInt32(s, 16)).ToArray();
        var yinc = 0F;
        try
        {
            yinc = this.getYInc(); //use catch just incase the command doesn't read the first time
        }
        catch
        {
            yinc = this.getYInc();
        }
        this.isDone();
        var yor = 0F;
        try
        {
            yor = this.getYor();
        }
        catch
        {
            yor = this.getYor();
        }
        this.isDone();
        var data = data_int.Select((d) => (125 - d) * yinc - yor).ToArray();
        return data;
    }

    public float measurePeriod()
    {
        device.Write(":MEAS:PER?");     //y orgin
        var period = device.ReadString();
        float f = 0;
        try
        {
            f = float.Parse(period);
        }
        catch
        {
            //weird error look at dmm
            f = this.measurePeriod();
        }
        return f;
    }
}
