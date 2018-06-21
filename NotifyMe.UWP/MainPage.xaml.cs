using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NotifyMe.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private HubConnection _connection;
        public MainPage()
        {
            this.InitializeComponent();

            _connection = new HubConnectionBuilder()
                .WithUrl("/Ntfctn")
                .Build();


            CreateConnection();
        }

        private async void CreateConnection()
        {
            _connection.On<string, string>("ReceiveMessage", async (user, message) =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var newMessage = $"{user}: {message}";

                    txtContent.Text += $"{DateTime.Now}: { newMessage + Environment.NewLine }";
                });
            });

            try
            {
                await _connection.StartAsync();

            }
            catch (Exception)
            {
                //TODO: Handle exception
            }

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await _connection.InvokeAsync("SendMessage", "Some UWP App.", txtMessage.Text);
            }
            catch (Exception)
            {
                //TODO: Handle exception
            }
        }
    }
}
