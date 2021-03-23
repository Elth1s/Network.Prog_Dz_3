using Network.Prog_Дз_3_Server;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
            Task.Run(() => {
                IPEndPoint endPoint = new IPEndPoint(iPAddress, port);
                while (true)
                {
                    if (isStarted == false)
                    {
                        MessageBox.Show("Stoped");
                        return;
                    }

                    TcpClient client = new TcpClient();
                    client.Connect(endPoint);
                    // виконуємо підключення
                    Thread.Sleep(secondsDelay);
                    // створюємо клас, який містить інформацію про файл
                    XmlSerializer serializer = new XmlSerializer(typeof(FileTransferInfo));
                    var info = (FileTransferInfo)serializer.Deserialize(client.GetStream());
                    using (FileStream fs = new FileStream(info.Name, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(info.Data, 0, info.Data.Length);
                    }
                    
                    client.Close();
                }
            });
        }
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            isStarted = false;
        }

        private void FirstRadionButton_Checked(object sender, RoutedEventArgs e)
        {
            secondsDelay = Convert.ToInt32((sender as RadioButton).Content);
        }

    }
}
