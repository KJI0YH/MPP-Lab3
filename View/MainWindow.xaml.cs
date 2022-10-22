using System.Windows;
using System.Windows.Controls;
using View.ViewModel;

namespace View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ApplicatioinViewModel();
        }

        private void ScrollViewer_PreviewMouseWheel(object? sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer sv = sender as ScrollViewer;
            if (sv != null)
            {
                if (e.Delta > 0)
                {
                    if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
                    {
                        sv.LineLeft();
                    }
                    else
                    {
                        sv.LineUp();
                    }
                }

                else if (e.Delta < 0)
                {
                    if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
                    {
                        sv.LineRight();
                    }
                    else
                    {
                        sv.LineDown();
                    }
                }
            }
        }
    }
}
