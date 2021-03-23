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
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace Network.Prog_Дз_3_Server
{
    [Serializable] // атрибут для серіалізації об'єкта класа
    public class FileTransferInfo
    {
        // ім'я файла
        public string Name { get; set; }
        // вміст файла
        public byte[] Data { get; set; }
    }
    class Program
    {
        static IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
        static int port = 8080;
        static void Main(string[] args)
        {
            IPEndPoint localEndPoint = new IPEndPoint(iPAddress, port);
            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

            // створюємо екземпляр сервера вказуючи кінцеву точку для приєднання
            TcpListener server = new TcpListener(localEndPoint);
            // запускаємо прослуховування вказаної кінцевої точки
            server.Start(10);

                    Console.WriteLine("\tWaiting for file...");
            while (true)
            {
                try
                {
                    // отримуємо зв'язок з клієнтом
                    TcpClient client = server.AcceptTcpClient();

                    // отримуємо дані від клієнта
                    // та десеріалізуємо об'єкт
                    Graphics graph = null;

                    var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                    Screen.PrimaryScreen.Bounds.Height);
                    graph = Graphics.FromImage(bmp);
                    graph.CopyFromScreen(0, 0, 0, 0, bmp.Size);
                    bmp.Save("screen.png");
                    FileTransferInfo info = new FileTransferInfo();
                    info.Name = Path.GetFileName("screen.png");
                    using (FileStream fs = new FileStream("screen.png", FileMode.Open, FileAccess.Read))
                    {

                        byte[] fileData = new byte[fs.Length];
                        fs.Read(fileData, 0, fileData.Length);
                        info.Data = fileData;
                    }

                    XmlSerializer serializer = new XmlSerializer(typeof(FileTransferInfo));
                    using (NetworkStream stream = client.GetStream())
                    {
                        serializer.Serialize(stream, info);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    Console.ResetColor();
                }
            }

            // зупиняємо роботу сервера
            server.Stop();
        }
    }
}
