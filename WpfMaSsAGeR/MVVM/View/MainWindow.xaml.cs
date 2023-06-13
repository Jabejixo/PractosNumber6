using Wpf.Ui.Controls;
using WpfMaSsAGeR.MVVM.ViewModel;

namespace WpfMaSsAGeR.MVVM.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UiWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
    }
}
