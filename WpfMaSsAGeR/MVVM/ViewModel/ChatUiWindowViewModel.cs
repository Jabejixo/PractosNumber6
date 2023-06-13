using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using JsonLib;
using Wpf.Ui.Controls;
using WpfMaSsAGeR.MVVM.Commands;
using WpfMaSsAGeR.MVVM.Model;
using WpfMaSsAGeR.MVVM.View;
using MessageBox = System.Windows.MessageBox;

namespace WpfMaSsAGeR.MVVM.ViewModel
{
    public class ChatUiWindowViewModel : ViewModelBase
    {
//----------------------------------------------------------------------------------------------------

        private UiWindow _window;

        public UiWindow Window
        {
            get => _window;
            set
            {
                _window = value;
                Set(ref _window, value);
                _window.Closed += OnClosed;
            }
        }

        private ObservableCollection<Message> messages = new();

        public ObservableCollection<Message> Messages
        {
            get => messages;
            set
            {
                messages = value;
                Set(ref messages, messages);
            }
        }

        private ObservableCollection<string> clientNames = new ObservableCollection<string>();
        
        public ObservableCollection<string> ClientNames
        {
            get => clientNames;
            set
            {
                clientNames = value;
                Set(ref clientNames, value);
            }
        }

        private string connectionInfo;

        public string ConnectionInfo
        {
            get => connectionInfo ?? "Присоединился";
            set => Set(ref connectionInfo, value);
        }

        private string _clientName;

        public string ClientName
        {
            get => _clientName;
            set
            {
                _clientName = value;
                Set(ref _clientName, value);
            }
        }

        private string _messageText;

        public string MessageText
        {
            get => _messageText;
            set
            {
                _messageText = value;
                Set(ref _messageText, value);
            }
        }


        private string hostname;

        private CancellationTokenSource isConnected = new();
        
        private Socket serverSocket;

        //----------------------------------------------------------------------------------------------------


        //----------------------------------------------------------------------------------------------------
        private RelayCommand commandOpenConnectServerDialog;
        public RelayCommand CommandOpenConnectServerDialog
        {
            get
            {
                return commandOpenConnectServerDialog ??= new RelayCommand(obj =>
                {
                    OpenConnectDialog();
                });
            }
        }



        private RelayCommand commandSendMessage;
        public RelayCommand CommandSendMessage
        {
            get
            {
                return commandSendMessage ??= new RelayCommand(obj =>
                {
                    SendMessage(ClientName, MessageText);
                });
            }
        }



        private RelayCommand commandOpenMainMenu;
        public RelayCommand CommandOpenMainMenu
        {
            get
            {
                return commandOpenMainMenu ??= new RelayCommand(obj =>
                {
                    OpenMainMenu();
                });
            }
        }



        //----------------------------------------------------------------------------------------------------

        public ChatUiWindowViewModel(string clientName)
        {
            ClientName = clientName;
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }


        private async Task ReceiveMessages(CancellationTokenSource tokenSource)
        {
            while (!tokenSource.IsCancellationRequested)
            {
                var buffer = new byte[1024];
                await serverSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                var message = JsonManager.ConvertToString<Message>(Encoding.UTF8.GetString(buffer));

                if (message == null) continue;
                switch (message.Type)
                {
                    case MessageType.Info:
                    {
                        ConnectionInfo = "Подключен к " + hostname;
                        ClientNames.Clear();
                        foreach (string username in JsonManager.ConvertToCollection<string>(message.MessageText))
                        {
                            ClientNames.Add(username);
                        }

                        break;
                    }
                    case MessageType.Error:
                        System.Windows.MessageBox.Show(message.MessageText);
                        break;
                    case MessageType.Text:
                        message.ClientName = message.ClientName + " " + DateTime.Now;
                        Messages.Add(message);
                        break;
                    case MessageType.ToServer:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            serverSocket.Close();
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }





        async void OpenConnectDialog()
        {
            switch (connectionInfo)
            {
                case "Connect":
                {
                    var dialog = new ConnectServerWindow();
                    var result = dialog.ShowDialog();

                    if (result.HasValue && result.Value)
                    {
                        var dialogResult = dialog.DataContext as ConnectServerWindowViewModel;
                        hostname = dialogResult.Ip;
                    }

                    try
                    {
                        await serverSocket.ConnectAsync(hostname, 18888);
                        isConnected = new CancellationTokenSource();
                        ReceiveMessages(isConnected);
                        SendMessage(ClientName, "/connect", MessageType.ToServer);

                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Connection Error: invalid IP!");
                    }

                    break;
                }
                default:
                    SendMessage(ClientName, "/disconnect");
                    connectionInfo = "Connect";
                    break;
            }
        }

        private async Task SendMessage(string clientName, string text, MessageType messageType = MessageType.Text)
        {
            var message = new Message() { ClientName = clientName, MessageText = text };

            if (text != "/connect")
            {
                if (text == "/disconnect")
                {
                    message.Type = MessageType.ToServer;
                    isConnected.Cancel();
                    Messages.Clear();
                    ClientNames.Clear();
                }
                else
                {
                    Messages.Add(new Message()
                        { ClientName = "Вы " + DateTime.Now, IsOwn = true, MessageText = message.MessageText });
                    message.Type = messageType;
                }
            }
            else
            {
                message.Type = messageType;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(JsonManager.ConvertToJson(message));
            await serverSocket.SendAsync(new ArraySegment<byte>(bytes), SocketFlags.None);
        }

        private void OpenMainMenu()
        {
            SendMessage(ClientName, "/disconnect");
            Window.Close();
        }



        private void OnClosed(object? sender, EventArgs e)
        {
            SendMessage(ClientName, "/disconnect");
        }

    }
}
