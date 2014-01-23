using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using GameOfLife;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace GameOfLife.Phone
{
    public partial class Display : PhoneApplicationPage
    {
        private readonly DispatcherTimer _timer;

        public static readonly DependencyProperty GenerationProperty = DependencyProperty.Register(
            "Generation", typeof(Generation), typeof(Display), new PropertyMetadata(default(Generation)));

        private bool _running = false;
        private readonly WriteableBitmap _bmp = new WriteableBitmap(30, 30);

        public Generation Generation
        {
            get { return (Generation)GetValue(GenerationProperty); }
            set { SetValue(GenerationProperty, value); }
        }

        public Display()
        {
            InitializeComponent();
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(20) };
            _timer.Tick += (sender, args) => Tick();
            Image.Source = _bmp;
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _running = true;
            base.OnNavigatedTo(e);

            var selection = (StartCultureSelection)Enum.Parse(typeof(StartCultureSelection), NavigationContext.QueryString["selection"]);

            switch (selection)
            {
                case StartCultureSelection.Blinker:
                    Generation = StartCultures.Blinker;
                    Title = "Blinker";
                    break;
                case StartCultureSelection.GliderGun:
                    Generation = StartCultures.GliderGun;
                    Title = "Blinker";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Render();
            _timer.Start();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _running = false;
            _timer.Stop();
            base.OnNavigatedFrom(e);
        }

        private void Tick()
        {
            _timer.Stop();
            Generation = Life.tick1(Generation);
            Render();
            if (_running)
                _timer.Start();
        }

        private void Render()
        {
            _bmp.Clear();
            var xOffset = 0 - Generation.MinX + 5;
            var yOffset = 0 - Generation.MinY + 5;
            foreach (var cell in Generation.AliveCells)
                _bmp.SetPixel(cell.X + xOffset, cell.Y + yOffset, Colors.Magenta);
            _bmp.Invalidate();
        }

    }
}