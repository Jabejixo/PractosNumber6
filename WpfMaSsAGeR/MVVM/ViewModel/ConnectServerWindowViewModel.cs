using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMaSsAGeR.MVVM.Commands;
using WpfMaSsAGeR.MVVM.View;

namespace WpfMaSsAGeR.MVVM.ViewModel
{
    public class ConnectServerWindowViewModel : ViewModelBase
    {
        public ConnectServerWindow Window { get; set; }

        private string _ip;

        public string Ip
        {
            get => _ip;
            set
            {
                _ip = value;
                Set(ref _ip, value);
            }
        }


        private RelayCommand commandConnect;
        public RelayCommand CommandConnect
        {
            get =>
                commandConnect ??= new RelayCommand(obj =>
                {
                    ConnectServer();
                });
        }

        private void ConnectServer()
        {
            Window.DialogResult = true;
        }



    }
}
