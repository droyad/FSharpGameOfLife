using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using GameOfLife;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GameOfLife.Phone.Resources;

namespace GameOfLife.Phone
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void GliderGun_OnClick(object sender, RoutedEventArgs e)
        {
            Open(StartCultureSelection.GliderGun);
        }

        private void Open(StartCultureSelection selection)
        {
            NavigationService.Navigate(new Uri("/Display.xaml?selection=" + selection, UriKind.Relative));
        }

        private void Blinker_OnClick(object sender, RoutedEventArgs e)
        {
            Open(StartCultureSelection.Blinker);
        }

        private void BlockLayer2_OnClick(object sender, RoutedEventArgs e)
        {
            Open(StartCultureSelection.BlockLayer2);
        }
    }
}