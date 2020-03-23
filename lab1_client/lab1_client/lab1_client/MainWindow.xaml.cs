using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace lab1_client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Socket socket = null;
        private void StartClient()
        {
            
        }
        public MainWindow()
        {
            InitializeComponent();
            StartClient();
        }

        private void buttConnect_Click(object sender, RoutedEventArgs e)
        {
            int port = Int32.Parse(PortTextBox.Text);
            IPEndPoint localEndPoint = null;
            IPAddress ipAddress = null;

            try
            {
                if (DomainNameTextBox.Text.Equals("localhost"))
                {
                    IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                    ipAddress = ipHostInfo.AddressList[0];
                    localEndPoint = new IPEndPoint(ipAddress, port);
                }
                else
                {
                    ipAddress = IPAddress.Parse(DomainNameTextBox.Text);
                    localEndPoint = new IPEndPoint(ipAddress, port);
                }
                

                socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(localEndPoint);

                if (socket.Connected)
                {
                    SendMessButt.IsEnabled = true;
                    DisconnectButt.IsEnabled = true;
                    ConnectButt.IsEnabled = false;
                    PrintLog("Server connected");
                }
            }
            catch (Exception exc)
            {
                PrintLog("Connection refused");
                Console.WriteLine(exc);
            }
        }

        private void buttDisconnect_Click(object sender, RoutedEventArgs e)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();

            SendMessButt.IsEnabled = false;
            DisconnectButt.IsEnabled = false;
            ConnectButt.IsEnabled = true;
            PrintLog("Server disconnected");
        }

        private void SendMessButt_Click(object sender, RoutedEventArgs e)
        {
            string message = MessageTextBox.Text;
            if (message.Equals(""))
                MessageBox.Show("You have to enter your message!");
            else
            {
                try
                {
                    byte[] messBytes = Encoding.ASCII.GetBytes(message);
                    int byteSent = socket.Send(messBytes);
                    PrintLog($"Sent to server: {message}");

                    socket.Receive(messBytes);
                    message = Encoding.ASCII.GetString(messBytes);
                    PrintLog($"Server says: {message}");
                }
                catch(Exception exc)
                {
                    Console.WriteLine(exc.ToString());
                }
            }
        }

        private void PrintLog(string mess)
        {
            LogTextBox.Text += mess+"\n";
        }
    }
}
