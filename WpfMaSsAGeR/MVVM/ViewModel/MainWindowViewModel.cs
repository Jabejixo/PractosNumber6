using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMaSsAGeR.MVVM.Commands;
using WpfMaSsAGeR.MVVM.View;

namespace WpfMaSsAGeR.MVVM.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {

        private RelayCommand commandOpenUser;
        public RelayCommand CommandOpenUser
        {
            get
            {
                return commandOpenUser ??= new RelayCommand(obj =>
                {
                    OpenUser();
                });
            }
        }



        private RelayCommand commandOpenHost;
        public RelayCommand CommandOpenHost
        {
            get =>
                commandOpenHost ??= new RelayCommand(obj =>
                {
                    OpenHost();
                });
        }


        private string clientName;
        public string ClientName
        {
            get => clientName;
            set
            {
                clientName = value;
                Set(ref clientName, value);
            }
        }

        public void OpenUser()
        {
            new ChatUiWindow(ClientName).Show();
        }

        public void OpenHost()
        {
            new Host_ServerWindow().Show();
        }
    }
}
