using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.StaticFiles;
using Owin;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;

namespace PointerApplication
{

    public partial class MainWindow : Window
    {
        private const double CLIENT_DISPLAY_HEIGHT = 480;
        private const double CLIENT_DISPLAY_WIDTH = 640;
        private const double CLIENT_DISPLAY_OFFSET_PERCENTAGE = .93;

        public MainWindow()
        {
            InitializeComponent();

            var machineName = Environment.MachineName;

            string url = String.Format("http://{0}:8181", machineName);
            WebApp.Start<Startup>(url);

            var hubPointerConnection = new HubConnection(url);
            var hubPointerProxy = hubPointerConnection.CreateHubProxy("PointerHub");
            hubPointerProxy.On<string, string>("addMessage", (invoker, data) =>
            {
                if (invoker == "position")
                {
                    var positionData = ParsePositionData(data);
                    if (positionData.ScreenPosition >= Screen.AllScreens.Count())
                        positionData.ScreenPosition = 0;
                    Rectangle rectangle = Screen.AllScreens[positionData.ScreenPosition].Bounds;
                    var multiplier = GetScreenMultiplier(rectangle);
                    var offSet = GetOffSetScreenMargin(positionData.ScreenPosition);
                    Dispatcher.InvokeAsync(() =>
                    {
                        var screenWidthPercentage = positionData.HorizontalPosition / CLIENT_DISPLAY_WIDTH;
                        var screenHeightPercentage = positionData.VerticalPosition / CLIENT_DISPLAY_HEIGHT;
                        var screenPositionX = screenWidthPercentage * rectangle.Width;
                        var screenPositionY = screenHeightPercentage * rectangle.Height;
                        
                        this.Left = (screenPositionX   + rectangle.X) / Screen.AllScreens.Count();
                        this.Top = (screenPositionY + rectangle.Y) / Screen.AllScreens.Count();
                        this.Show();
                        this.Activate();
                    });
                }
            });
            hubPointerConnection.Start();
        }

        private int GetOffSetScreenMargin(int screenPosition)
        {
            var offSet = 0;
            for (int i = 0; i < screenPosition; i++)
            {
                offSet += ((Rectangle) Screen.AllScreens[i].Bounds).Width;
            }

            return offSet;
        }

        private PositionData ParsePositionData(string data)
        {
            var items = data.Split(',');
            return new PositionData()
            {
                ScreenPosition = int.Parse(items[0]),
                HorizontalPosition = int.Parse(items[1]),
                VerticalPosition = int.Parse(items[2])
            };
        }

        private double GetScreenMultiplier(Rectangle rectangle)
        {
            return rectangle.Height / CLIENT_DISPLAY_HEIGHT * CLIENT_DISPLAY_OFFSET_PERCENTAGE;
        }


        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            var imagePath = Path.Combine(Environment.CurrentDirectory, "lookhere.png");
            b.UriSource = new Uri(imagePath);
            b.EndInit();

            var image = sender as Image;

            image.Source = b;
        }



        private void MainWindow_OnDeactivated(object sender, EventArgs e)
        {
            this.Hide();
        }
    }

    internal class PositionData
    {
        public int VerticalPosition { get; set; }
        public int ScreenPosition { get; set; }
        public int HorizontalPosition { get; set; }
    }

    internal class ScreenSize
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }

    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseDefaultFiles(new DefaultFilesOptions
            {
                DefaultFileNames = Enumerable.Repeat("index.html", 1).ToList()
            });
            app.UseStaticFiles();
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }
    public class PointerHub : Hub
    {
        public void Send(string name, string message)
        {
            Clients.All.addMessage(name, message);
        }
    }


}
