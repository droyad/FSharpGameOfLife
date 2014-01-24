using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
using System.Windows.Threading;
using GameOfLife;

namespace Wpf
{
    /// <summary>
    /// Interaction logic for Display.xaml
    /// </summary>
    public partial class Display : Window
    {

        public static readonly DependencyProperty GenerationProperty = DependencyProperty.Register(
        "Generation", typeof(Generation), typeof(Display), new PropertyMetadata(default(Generation)));


        private readonly DispatcherTimer _timer;
        private readonly WriteableBitmap _bmp;
        private bool _running = true;


        public Generation Generation
        {
            get { return (Generation)GetValue(GenerationProperty); }
            set { SetValue(GenerationProperty, value); }
        }


        public Display(Generation generation)
        {

            InitializeComponent();
            Generation = generation;

            _bmp = new WriteableBitmap(800,600, 96, 96, PixelFormats.Pbgra32, BitmapPalettes.WebPalette);

            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            _timer.Tick += (sender, args) => Tick();
            Image.Source = _bmp;

            Render();
            _timer.Start();
            Closing += (sender, args) =>
            {
                _running = false;
                _timer.Stop();
            };
        }



        private void Tick()
        {
            _timer.Stop();
            var currentGen = Generation;
            Task.Run(() =>
            {
                var sw = new Stopwatch();
                sw.Start();
                var gen = Life.tick1(currentGen);
                sw.Stop();
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    Generation = gen;
                    Render();
                    if (_running)
                    {
                        _timer.Interval = sw.Elapsed < TimeSpan.FromMilliseconds(50) ? TimeSpan.FromMilliseconds(50) - sw.Elapsed : TimeSpan.Zero;
                        _timer.Start();
                    }
                }));
            });
        }

        private void Render()
        {
            _bmp.Lock();
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
            _bmp.Unlock();
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
            var ratio = max == 0 ? 0 : dimension / max;
            ratio = new[] { 21, 13, 8, 5, 3, 2, 1 }.FirstOrDefault(e => ratio > e);
            return ratio == 0 ? 1 : ratio;
        }

    }
}
