using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;

namespace Server
{
    public partial class Form1 : Form
    {
        TcpListener lyssnare;
        TcpClient klient;
        int port = 12345;
        public List<TcpClient> klienter = new List<TcpClient>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                lyssnare = new TcpListener(IPAddress.Any, port);
                lyssnare.Start();
            }
            catch (Exception error)
            {
                MessageBox.Show("Server_form1: " + error.Message, Text);
                return;
            }
            StartaMottagning();

        }
        public async void StartaMottagning()
        {
            try
            {
                klient = await lyssnare.AcceptTcpClientAsync();
                lbxLogger.Items.Add("En klient har anslutit: " + klient.Client.RemoteEndPoint);
                klienter.Add(klient);
            
                foreach (TcpClient i in klienter)
                {
                    lbxLogger.Items.Add("klienter anslutna: " + i.Client.RemoteEndPoint);
                   
                }


            }
            catch (Exception error)
            {
                MessageBox.Show("Server_Starta mottagning: " + error.Message, Text);
                return;
            }
            StartaLäsning(klient);
            StartaMottagning();
           
        }
        public async void StartaLäsning(TcpClient k)
        {
            byte[] buffert = new byte[1024];

            int n = 0;
            try
            {
                
                n = await k.GetStream().ReadAsync(buffert, 0, buffert.Length);
                lbxLogger.Items.Add("Starta läsning, n: " + n.ToString());
                
            }
            catch (Exception error)
            {
                foreach (TcpClient i in klienter)
                {
                    lbxLogger.Items.Add("DC klienter : " + i.Client.RemoteEndPoint);

                }
                MessageBox.Show("Server_StartaLäsning: " + error.Message, Text);
                return;
            }
            string result = Encoding.Unicode.GetString(buffert, 0, n);
            lbxLogger.Items.Add(result);
            StartaSändning(result);
            StartaLäsning(k);
        }
            public async void StartaSändning(string message)
        {
            byte[] utData = Encoding.Unicode.GetBytes(message);

            try
            {
                foreach(TcpClient klient in klienter)
                {
                    await klient.GetStream().WriteAsync(utData, 0, utData.Length);
                    lbxLogger.Items.Add("Starta sändning");
                }

            }
            catch (Exception error)
            {
                MessageBox.Show("Server_StartaSändning: " + error.Message, Text);
                return;
            }
            //StartaLäsning(klient);
            
        }
    }

}

