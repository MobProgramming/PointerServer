﻿using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Owin.Hosting;
using Image = System.Windows.Controls.Image;

namespace PointerApplication
{

    public partial class MainWindow : Window
    {
        private const double CLIENT_DISPLAY_HEIGHT = 480;
        private const double CLIENT_DISPLAY_WIDTH = 640;
        private const string REMOTE_URL_PATH = "http://{0}:8181";
        public MainWindow()
        {
            InitializeComponent();

            
            string remoteUrl = String.Format(REMOTE_URL_PATH, Environment.MachineName);
            WebApp.Start<Startup>(remoteUrl);

            var hubPointerConnection = new HubConnection(remoteUrl);
            var hubPointerProxy = hubPointerConnection.CreateHubProxy("PointerHub");
            hubPointerProxy.On<string, string>("addMessage", (invoker, data) =>
            {
                if (invoker == "position")
                {
                    var positionData = ParsePositionData(data);
                    if (positionData.ScreenPosition >= Screen.AllScreens.Count())
                        positionData.ScreenPosition = 0;
                    Rectangle selectedScreen = Screen.AllScreens[positionData.ScreenPosition].Bounds;
                    var screenPositionDivisor = selectedScreen.Width > 1280 ? 2 : 1;
                    Dispatcher.InvokeAsync(() =>
                    {
                        var screenWidthPercentage = positionData.HorizontalPosition / CLIENT_DISPLAY_WIDTH;
                        var screenHeightPercentage = positionData.VerticalPosition / CLIENT_DISPLAY_HEIGHT;
                        var screenPositionX = screenWidthPercentage * selectedScreen.Width;
                        var screenPositionY = screenHeightPercentage * selectedScreen.Height;

                        this.Left = (screenPositionX + selectedScreen.X) / screenPositionDivisor;
                        this.Top = (screenPositionY + selectedScreen.Y) / screenPositionDivisor;

                        this.Show();
                        this.Activate();
                    });
                }
            });
            hubPointerConnection.Start();
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
}
