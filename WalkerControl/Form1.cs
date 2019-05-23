using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WalkerControl
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            timer1.Tick += new System.EventHandler(this.getCyclicData);
            this.Controls.Add(dataGridView1);
            dataGridView1.ColumnCount = 8;
            dataGridView1.RowCount = 4;
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.Width = 150;
            }
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Height = 35;
            }

            this.Controls.Add(dataGridView2);
            dataGridView2.ColumnCount = 9;
            dataGridView2.RowCount = 1;
            foreach (DataGridViewColumn column in dataGridView2.Columns)
            {
                column.Width = 133;
            }
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                row.Height = 35;
            }
            dataGridView1.DefaultCellStyle.SelectionBackColor = dataGridView2.DefaultCellStyle.BackColor;
            dataGridView1.DefaultCellStyle.SelectionForeColor = dataGridView2.DefaultCellStyle.ForeColor;
            dataGridView2.DefaultCellStyle.SelectionBackColor = dataGridView2.DefaultCellStyle.BackColor;
            dataGridView2.DefaultCellStyle.SelectionForeColor = dataGridView2.DefaultCellStyle.ForeColor;
        }
        UdpClient udpClient = new UdpClient(11000);
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                udpClient.Connect(IPaddr.Text, 11000);
                udpClient.Client.SendTimeout = 5000;
                udpClient.Client.ReceiveTimeout = 5000;
                WalkerRequest req = new WalkerRequest();
                req.type = WalkerRequestType.INFO;
                req.message = "CONNECTED";
                req.time = string.Format("{0:dd.MM.yyyy HH:mm:ss.fff}", DateTime.Now);
                string json = JsonConvert.SerializeObject(req);
                Byte[] sendBytes = Encoding.UTF8.GetBytes(json);
                udpClient.Send(sendBytes, sendBytes.Length);
                timer1.Interval = int.Parse(treshold.Text);
                timer1.Enabled = true;
            }
            catch (Exception exc)
            {
                timer1.Enabled = false;
                MessageBox.Show(exc.ToString(), "Error",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Error);
            }
        }
        private void getCyclicData(object sender, System.EventArgs argv)
        {
            try
            {
                WalkerRequest req = new WalkerRequest();
                req.type = WalkerRequestType.GET_CYCLIC;
                req.message = "";
                req.time = string.Format("{0:dd.MM.yyyy HH:mm:ss.fff}", DateTime.Now);
                string json = JsonConvert.SerializeObject(req);
                byte[] sendBytes = Encoding.UTF8.GetBytes(json);
                udpClient.Send(sendBytes, sendBytes.Length);
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                string returnData = Encoding.UTF8.GetString(receiveBytes);
                WalkerCyclicResponse response = JsonConvert.DeserializeObject<WalkerCyclicResponse>(returnData);
                string[,] ypr = response.ypr;
                string[] degs = response.degs;
                dataGridView1.Rows.Clear();
                dataGridView2.Rows.Clear();
                dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                for (int y = 0; y < 3; y++)
                {
                    string[] row = new string[8];
                    for (int x = 0; x < 8; x++)
                    {
                        row[x] = ypr[x, y];
                        row[x] = string.Format("{0:0.00}", float.Parse(row[x], CultureInfo.InvariantCulture));
                    }
                    dataGridView1.Rows.Add(row);
                }
                for (int i = 0; i < 9; i++)
                {
                    degs[i] = string.Format("{0:0.00}", float.Parse(degs[i], CultureInfo.InvariantCulture));
                }
                dataGridView2.Rows.Add(degs);
                dataGridView2.Columns[0].Name = "ZIELONA KOSTKA";
                dataGridView2.Columns[1].Name = "ZIELONA NOGA PRZÓD/TYŁ";
                dataGridView2.Columns[2].Name = "ZIELONA KOLANO";
                dataGridView2.Columns[3].Name = "ZIELONA NOGA LEWO/PRAWO";
                dataGridView2.Columns[4].Name = "CZERWONA KOLANO";
                dataGridView2.Columns[5].Name = "CZERWONA KOSTKA";
                dataGridView2.Columns[6].Name = "GÓRA LEWO/PRAWO";
                dataGridView2.Columns[7].Name = "CZERWONA NOGA PRZÓD/TYŁ";
                dataGridView2.Columns[8].Name = "CZERWONA NOGA LEWO/PRAWO";

                dataGridView1.Columns[0].Name = "ZIELONA STOPA-KOLANO";
                dataGridView1.Columns[1].Name = "CZERWONA STOPA";
                dataGridView1.Columns[2].Name = "CZERWONA STOPA-KOLANO";
                dataGridView1.Columns[3].Name = "ZIELONA STOPA";
                dataGridView1.Columns[4].Name = "ZIELONA KOLANO-NOGA";
                dataGridView1.Columns[5].Name = "CZERWONA KOLANO-NOGA";
                dataGridView1.Columns[6].Name = "GÓRA";
                dataGridView1.Columns[7].Name = "ODNIESIENIE";

                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    column.Width=150;
                }
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.Height = 35;
                }
                foreach (DataGridViewColumn column in dataGridView2.Columns)
                {
                    column.Width = 133;
                }
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    row.Height = 35;
                }
            }
            catch (Exception exc)
            {
                timer1.Enabled = false;
                MessageBox.Show(exc.ToString(), "Error",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Error);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if(timer1.Enabled)
            {
                try
                {
                    WalkerRequest req = new WalkerRequest();
                    req.type = WalkerRequestType.EXIT;
                    req.time = string.Format("{0:dd.MM.yyyy HH:mm:ss.fff}", DateTime.Now);
                    string json = JsonConvert.SerializeObject(req);
                    Byte[] sendBytes = Encoding.UTF8.GetBytes(json);
                    udpClient.Send(sendBytes, sendBytes.Length);
                    timer1.Enabled = false;
                }
                catch (Exception exc)
                {
                    timer1.Enabled = false;
                    MessageBox.Show(exc.ToString(), "Error",
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Error);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                try
                {
                    WalkerRequest req = new WalkerRequest();
                    req.type = WalkerRequestType.POST_DESTINATIONS;
                    req.time = string.Format("{0:dd.MM.yyyy HH:mm:ss.fff}", DateTime.Now);
                    req.destination1 = float.Parse(textBox1.Text, CultureInfo.InvariantCulture);
                    req.destination2 = float.Parse(textBox2.Text, CultureInfo.InvariantCulture);
                    req.destination3 = float.Parse(textBox3.Text, CultureInfo.InvariantCulture);
                    req.destination4 = float.Parse(textBox4.Text, CultureInfo.InvariantCulture);
                    req.destination5 = float.Parse(textBox5.Text, CultureInfo.InvariantCulture);
                    req.destination6 = float.Parse(textBox6.Text, CultureInfo.InvariantCulture);
                    req.destination7 = float.Parse(textBox7.Text, CultureInfo.InvariantCulture);
                    req.destination8 = float.Parse(textBox8.Text, CultureInfo.InvariantCulture);
                    req.destination9 = float.Parse(textBox9.Text, CultureInfo.InvariantCulture);
                    req.goToDest1 = checkBox1.Checked;
                    req.goToDest2 = checkBox2.Checked;
                    req.goToDest3 = checkBox3.Checked;
                    req.goToDest4 = checkBox4.Checked;
                    req.goToDest5 = checkBox5.Checked;
                    req.goToDest6 = checkBox6.Checked;
                    req.goToDest7 = checkBox7.Checked;
                    req.goToDest8 = checkBox8.Checked;
                    req.goToDest9 = checkBox9.Checked;
                    string json = JsonConvert.SerializeObject(req);
                    Byte[] sendBytes = Encoding.UTF8.GetBytes(json);
                    udpClient.Send(sendBytes, sendBytes.Length);
                }
                catch (Exception exc)
                {
                    timer1.Enabled = false;
                    MessageBox.Show(exc.ToString(), "Error",
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Error);
                }
            }
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                try
                {
                    WalkerRequest req = new WalkerRequest();
                    req.type = WalkerRequestType.POST_SETTINGS;
                    req.time = string.Format("{0:dd.MM.yyyy HH:mm:ss.fff}", DateTime.Now);
                    req.correctionTime = float.Parse(correctionTimeBox.Text, CultureInfo.InvariantCulture);
                    req.correctionDelay = float.Parse(correctionDelayBox.Text, CultureInfo.InvariantCulture);
                    req.verboseMode = verboseCheckBox.Checked;
                    string json = JsonConvert.SerializeObject(req);
                    Byte[] sendBytes = Encoding.UTF8.GetBytes(json);
                    udpClient.Send(sendBytes, sendBytes.Length);
                }
                catch (Exception exc)
                {
                    timer1.Enabled = false;
                    MessageBox.Show(exc.ToString(), "Error",
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Error);
                }
            }
        }
    }
    public enum WalkerRequestType
    {
        GET_CYCLIC = 1,
        POST_DESTINATIONS = 2,
        INFO = 3,
        EXIT = 4,
        POST_SETTINGS = 5
    }
    public class WalkerRequest
    {
        public WalkerRequestType type;
        public string message = "";
        public string time = string.Format("{0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);
        public float destination1, destination2, destination3, destination4, destination5, destination6, destination7, destination8, destination9;
        public bool goToDest1, goToDest2, goToDest3, goToDest4, goToDest5, goToDest6, goToDest7, goToDest8, goToDest9;
        public float correctionTime, correctionDelay;
        public bool verboseMode;
    }
    public class WalkerCyclicResponse
    {
        public bool status;
        public string errorName;
        public string[,] ypr;
        public string[] degs;
        public string time;
    }
}
