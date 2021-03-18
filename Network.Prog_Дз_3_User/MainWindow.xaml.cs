using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using System.Xml.Serialization;

namespace Network.Prog_Дз_3_User
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isStarted = false;
        int secondsDelay = 2;
        static int port = 8080;
        IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            isStarted = true;
            Task.Run(() => GettingScreenshots());
        }
        private void GettingScreenshots()
        {
            IPEndPoint ipPoint = new IPEndPoint(iPAddress, port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipPoint);

            try
            {
                while (true)
                {
                    if (isStarted == false) return;
                    byte[] data = Encoding.Unicode.GetBytes(secondsDelay.ToString());
                    socket.Send(data);

                    data = new byte[256];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;

                    do
                    {
                        bytes = socket.Receive(data, data.Length, 0);
                    }
                    while (socket.Available > 0);


                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        var bitmapImage = BitmaSourceFromByteArray(data);
                        image.Source = bitmapImage;

                    }));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                socket?.Shutdown(SocketShutdown.Both);
                socket?.Close();
            }

        }
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            isStarted = false;
        }

        private void FirstRadionButton_Checked(object sender, RoutedEventArgs e)
        {
            secondsDelay = Convert.ToInt32((sender as RadioButton).Content);
        }
        public BitmapSource BitmaSourceFromByteArray(byte[] buffer)
        {
            return (BitmapSource)new ImageSourceConverter().ConvertFrom(buffer);
        }
    }
}
