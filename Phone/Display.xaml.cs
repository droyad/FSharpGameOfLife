using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly WriteableBitmap _bmp = new WriteableBitmap(600, 400);

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
                case StartCultureSelection.BlockLayer2:
                    Generation = StartCultures.BlockLayer2;
                    Title = "Block Layer 2";
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
            var currentGen = Generation;
            Task.Run(() =>
            {
                var gen = Life.tick1(currentGen);
                Dispatcher.BeginInvoke(() =>
                {
                    Generation = gen;
                    Render();
                    if (_running)
                        _timer.Start();
                });
            });
        }

        private void Render()
        {
            _bmp.Clear();
            var xOffset = GetOffset(c => c.Item1);
            var yOffset = GetOffset(c => c.Item2);
            var scale = Math.Min(GetScale(_bmp.PixelWidth, c => c.Item1, xOffset), GetScale(_bmp.PixelHeight, c => c.Item2, yOffset));

            foreach (var cell in Generation.AliveCells)
            {
                var x = (cell.Item1 + xOffset) * scale;
                var y = (cell.Item2 + yOffset) * scale;
                if (x > 0 && x + scale < _bmp.PixelWidth && y > 0 && y + scale < _bmp.PixelHeight)
                    _bmp.FillEllipse(x, y, x + scale, y + scale, Colors.Green);
            }
            _bmp.DrawRectangle(0, 0, _bmp.PixelWidth - 1, _bmp.PixelHeight - 1, Colors.Red);
            _bmp.Invalidate();
        }

        private int GetOffset(Func<Tuple<int, int>, int> func)
        {
            double offset = 0 - Generation.AliveCells.Min(func);
            var buffer = 5;
            offset = offset / buffer;
            offset = Math.Ceiling(offset);
            return (int)(offset * buffer);
        }

        private int GetScale(int dimension, Func<Tuple<int, int>, int> func, int offset)
        {
            var max = Generation.AliveCells.Max(func) + offset;
            var ratio = dimension/max;
            ratio = new[] {21, 13, 8, 5, 3, 2, 1}.FirstOrDefault(e => ratio > e);
            return ratio == 0 ? 1 : ratio;
        }

    }
}