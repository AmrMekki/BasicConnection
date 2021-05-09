using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BasicClient
{
    public partial class Client : Form
    {

        private static Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public Client()
        {
            InitializeComponent();
        }

        private void Client_Load(object sender, EventArgs e)
        {
            LoopConnect();
            txtInfo.Text += $"Enter a Request {Environment.NewLine}";

        }

        private void SendLoop()
        {

            if (txtMessage.Text == "")
            {
                MessageBox.Show($"Enter a request {Environment.NewLine}");
            }
            else
            {
                byte[] buffer = Encoding.UTF8.GetBytes(txtMessage.Text);
                try
                {
                    clientSocket.Send(buffer);

                    byte[] receivedBuff = new byte[1024];
                    int rec = clientSocket.Receive(receivedBuff);
                    byte[] data = new byte[rec];
                    Array.Copy(receivedBuff, data, rec);
                    txtInfo.Text += $" Received: {Encoding.UTF8.GetString(data)}{Environment.NewLine}";

                }
                catch (SocketException)
                {
                    txtInfo.Text += $"Server Disconnected {Environment.NewLine}";
                }
 }
        }

        //tries to reconnect continuously until we close the application
        private void LoopConnect()
        {
            int attempts = 0;
            while (!clientSocket.Connected)
            {
                try
                {
                    attempts++;

                    clientSocket.Connect(IPAddress.Parse("192.168.1.18"), 9000);
                }
                catch (SocketException)
                {
                    MessageBox.Show("Connection attempts: " + attempts.ToString());
                }
            }
            txtInfo.Text += $"Connected to Server {Environment.NewLine}";
            
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            SendLoop();
        }
    }
}
