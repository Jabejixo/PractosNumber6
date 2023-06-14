using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using JsonLib;
using WpfMaSsAGeR.MVVM.Commands;
using WpfMaSsAGeR.MVVM.Model;

namespace WpfMaSsAGeR.MVVM.ViewModel
{
    public class Host_ServerWindowViewModel : ViewModelBase
    {
        private ObservableCollection<string> clientsNames = new();

        private ObservableCollection<string> logs = new();

        public ObservableCollection<string> Logs
        {
            get => logs;
            set
            {
                logs = value;
                Set(ref logs, value, "Logs");
            }
        }

        private Socket socket;
        private List<Socket> Clients = new();

        public Host_ServerWindowViewModel()
        {
            var ipPoint = new IPEndPoint(IPAddress.Any, 18888);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipPoint);
            socket.Listen(1000);
            Logs.Add($"{DateTime.Now} Сервер расположен по следующему IP адрессу {ipPoint.Address}");
            ListenToClients();
        }

        private async Task ListenToClients()
        {
            while (true)
            {
                var client = await socket.AcceptAsync();
                Clients.Add(client);
                ReceiveMessagesFromClient(client);
            }
        }

        private async Task ReceiveMessagesFromClient(Socket client)
        {
            while (true)
            {
                var buffer = new byte[1024];
                var receivedBytes = await client.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                var message = JsonManager.ConvertToString<Message>(Encoding.UTF8.GetString(buffer));


                if (message != null)
                {
                    switch (message.Type)
                    {
                        case TypeMessage.ToServer when message.MessageText == "/connect":
                        {
                            if (!ValidateClientName(message.ClientName))
                            {
                                Logs.Add(
                                    $"[{DateTime.Now}] Клиент {((IPEndPoint)client.RemoteEndPoint).Address} попытался присоединиться. Ошибка: Клиент с таким ником уже существует.");
                                if (message.ClientName == "Server")
                                {
                                    message = new Message()
                                    {
                                        ClientName = "Server",
                                        MessageText = "Ошибка! Вы не можете называться 'Server'.",
                                        Type = TypeMessage.Error
                                    };
                                }
                                else
                                {
                                    message = new Message()
                                    {
                                        ClientName = "Server",
                                        MessageText =
                                            "Ошибка! Пользователь с тем же именем пользователя уже подключен.",
                                        Type = TypeMessage.Error
                                    };
                                }

                                SendMessage(client, message);
                                client.Close();
                                Clients.Remove(client);
                            }
                            else
                            {
                                clientsNames.Add(message.ClientName);
                                Logs.Add($"[{DateTime.Now}] {message.ClientName} присоединился.");
                                foreach (var item in Clients)
                                {
                                    if (item != client)
                                    {
                                        SendMessage(item,
                                            new Message()
                                            {
                                                ClientName = "Server",
                                                MessageText = JsonManager.ConvertCollectionToJson(clientsNames),
                                                Type = TypeMessage.Info
                                            });
                                        SendMessage(item,
                                            new Message()
                                            {
                                                ClientName = "Server",
                                                MessageText = $"{message.ClientName} присоединился.",
                                                Type = TypeMessage.Text
                                            });
                                    }
                                    else
                                    {
                                        SendMessage(item,
                                            new Message()
                                            {
                                                ClientName = "Server",
                                                MessageText = JsonManager.ConvertCollectionToJson(clientsNames),
                                                Type = TypeMessage.Info
                                            });
                                    }
                                }
                            }

                            break;
                        }
                        case TypeMessage.ToServer:
                        {
                            if (message.MessageText == "/disconnect")
                            {
                                Logs.Add($"[{DateTime.Now}] {message.ClientName} отключился.");
                                clientsNames.Remove(message.ClientName);
                                foreach (var item in Clients.Where(item => item != client))
                                {
                                    SendMessage(item,
                                        new Message()
                                        {
                                            ClientName = "Server",
                                            MessageText = JsonManager.ConvertCollectionToJson(clientsNames),
                                            Type = TypeMessage.Info
                                        });
                                    SendMessage(item,
                                        new Message()
                                        {
                                            ClientName = "Server",
                                            MessageText = $"{message.ClientName} отключился.",
                                            Type = TypeMessage.Text
                                        });
                                }

                                client.Close();
                                Clients.Remove(client);
                            }

                            break;
                        }
                        case TypeMessage.Text:
                        {
                            Logs.Add(
                                $"[{DateTime.Now}] Received {message.Type} Сообщение от {message.ClientName}: {message.MessageText}");
                            foreach (var item in Clients.Where(item => item != client))
                            {
                                SendMessage(item, message);
                            }

                            break;
                        }
                        case TypeMessage.Error:
                            break;
                        case TypeMessage.Info:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                }
            }
        }

        private bool a = false;

        private RelayCommand commandSave;
        public RelayCommand CommandSave
        {
            get
            {
                return commandSave ??= new RelayCommand(obj =>
                {
                    if (a) JsonManager.Serialize(Logs, "Logs");
                    a = true;
                });
            }
        }

        private RelayCommand commandLoad;
        public RelayCommand CommandLoad
        {
            get
            {
                return commandLoad ??= new RelayCommand(obj =>
                {
                    Logs.Clear();
                    var a = JsonManager.Deserialization<string>("Logs");
                    foreach (var VARIABLE in a)
                    {
                        Logs.Add(VARIABLE);
                    }
                });
            }
        }


        private static async Task SendMessage(Socket item, Message message)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonManager.ConvertToJson(message));
            await item.SendAsync(new ArraySegment<byte>(bytes), SocketFlags.None);
        }

        private bool ValidateClientName(string clientName)
        {
            return (clientsNames.Count == 0 && clientName != "Server") ||
                   clientsNames.Select(name => name != clientName && name != "Server").FirstOrDefault();
        }
    }
}
