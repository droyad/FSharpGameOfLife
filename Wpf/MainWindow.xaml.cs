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
using System.Windows.Navigation;
using System.Windows.Shapes;
using GameOfLife;

namespace Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Pop(Generation gen)
        {
            new Display(gen).Show();
        }

        private void Blinker_OnClick(object sender, RoutedEventArgs e)
        {
            Pop(StartCultures.Blinker);
        }

        private void GliderGun_OnClick(object sender, RoutedEventArgs e)
        {
            Pop(StartCultures.GliderGun);
        }

        private void BlockLayer2_OnClick(object sender, RoutedEventArgs e)
        {
            Pop(StartCultures.BlockLayer2);
        }
    }
}
