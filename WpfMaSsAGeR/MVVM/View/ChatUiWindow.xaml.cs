using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wpf.Ui.Controls;
using WpfMaSsAGeR.MVVM.ViewModel;

namespace WpfMaSsAGeR.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для ChatUiWindow.xaml
    /// </summary>
    public partial class ChatUiWindow : UiWindow
    {
        public ChatUiWindow(string clientName)
        {
            InitializeComponent();
            var ChatVM = new ChatUiWindowViewModel(clientName)
            {
                Window = this
            };
            DataContext = ChatVM;
        }
    }
}
