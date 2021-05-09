using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
//IPEndpoint, IPAddress
using System.Net;
//used in all functions
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BasicServer
{
    public partial class Server : Form
    {

        //deleted static

        public Server()
        {
            InitializeComponent();
        }
        private static byte[] buffer = new byte[1024]; //we don't need more because we're sending simple messages
        private static List<Socket> clientSockets = new List<Socket>();
        private static Socket serverSocket = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //parameters( working on ip version 4,  how we send the data based on the protocol, protocol tcp)

        public void SetupServer()
        {
            txtInfo.Text+=$"Setting up server...{Environment.NewLine}";
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse("192.168.1.18"),9000));
            serverSocket.Listen(5); //backlog: amount of uncompleted work
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        //accept and receive
        public void AcceptCallback(IAsyncResult AR)
        {
            this.Invoke((MethodInvoker)delegate
            {
                Socket socket = serverSocket.EndAccept(AR);
                clientSockets.Add(socket);
                txtInfo.Text += $"Client Connected{Environment.NewLine}";
                
                //start receiving from this specific client
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                
                //to accept connections from other clients
                serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

            });
        }

        //receive
        //gwaha send
        public void ReceiveCallback(IAsyncResult AR)
        {
            /*
            this.Invoke((MethodInvoker)delegate
            {
            });
            */

            this.Invoke((MethodInvoker)delegate
            {
                Socket socket = (Socket)AR.AsyncState;
                //when we close connection from client side next line stops "exception user unhandled"
                try
                {
                    int received = socket.EndReceive(AR);
                    byte[] dataBuff = new byte[received];
                    Array.Copy(buffer, dataBuff, received);



                    string text = Encoding.UTF8.GetString(dataBuff);
                    txtInfo.Text += $"Text received: {text} {Environment.NewLine}";


                    string response = string.Empty;
                    if (text.ToLower() != "get time")
                    {
                        response = "Invalid Request";
                    }
                    else
                    {
                        response = DateTime.Now.ToLongTimeString();
                    }



                    byte[] data = Encoding.UTF8.GetBytes(response);
                    socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                    socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);


                }
                catch (SocketException)
                {
                    txtInfo.Text += $"Client disconnected {socket.RemoteEndPoint}{Environment.NewLine}";

                }
            });
            
        }


        public void SendCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }
        private void Server_Load(object sender, EventArgs e)
        {
            SetupServer();
            
        }
    }
}
