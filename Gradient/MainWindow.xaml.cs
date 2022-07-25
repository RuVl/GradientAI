using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AI;
using SolidBrush = System.Drawing.SolidBrush;
using Color = System.Drawing.Color;

namespace Gradient;

public partial class MainWindow
{
    private readonly NeuralNetwork _neuralNetwork;

    private WriteableBitmap _writeableBitmap = null!;
    private Int32Rect _updateRect;

    private int _width;
    private int _height;

    private SolidBrush _mouseBrush = new(Color.Red);
    private readonly List<(Vector2 position, SolidBrush brush)> _points = new();


    public MainWindow()
    {
        InitializeComponent();

        _neuralNetwork = new NeuralNetwork(new[] {(7, 4), (7, 10), (3, 10)});
    }

    private void Image_OnLoaded(object sender, RoutedEventArgs e)
    {
        _width = (int)Image.Width;
        _height = (int)Image.Height;
        
        _writeableBitmap = new WriteableBitmap(_width, _height, 96, 96, PixelFormats.Bgr32, null);
        _updateRect = new Int32Rect(0, 0, _width, _height);

        Image.Source = _writeableBitmap;
    }

    private void ColorPicker_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
    {
        System.Windows.Media.Color color = e.NewValue ?? e.OldValue ?? Colors.Red;
        _mouseBrush = new SolidBrush(color.Convert());
    }
    
    private void Image_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var position = e.GetPosition(Image).ToVector2();
        _points.Add((position, _mouseBrush));

        Train();
        Update();
    }

    private void Update()
    {
        var update = new Bitmap(_width / 8, _height / 8);
        for (var y = 0; y < update.Height; y++)
        {
            for (var x = 0; x < update.Width; x++)
            {
                double[] input =
                {
                    Map(x - update.Width / 2.0, -update.Width / 2.0, update.Width / 2.0, 0, 1),
                    Map(y - update.Height / 2.0, -update.Height / 2.0, update.Height / 2.0, 0, 1)
                };
                
                double[] output = _neuralNetwork.FeedForward(input);

                Color color = Color.FromArgb(
                    (int)Map(output[0], 1, 255), 
                    (int)Map(output[1], 1, 255),
                    (int)Map(output[2], 1, 255));
                
                update.SetPixel(x, y, color);
            }
        }

        // Draw mouse points
        using var bitmap = new Bitmap(_width, _height, _writeableBitmap.BackBufferStride, _writeableBitmap.Format.Convert(), _writeableBitmap.BackBuffer);
        using Graphics graphics = Graphics.FromImage(bitmap);

        graphics.DrawImage(update, 0, 0, _width, _height);
        
        foreach ((Vector2 position, SolidBrush brush) in _points)
        {
            graphics.DrawArc(Pens.White, position.X - 6, position.Y - 6, 12, 12, 0, 360);
            graphics.FillEllipse(brush, position.X - 5, position.Y - 5, 10, 10);
        }
        
        // Refresh _writableBitmap
        _writeableBitmap.Lock();
        _writeableBitmap.AddDirtyRect(_updateRect);
        _writeableBitmap.Unlock();
    }

    private void Train()
    {
        for (var _ = 0; _ < 10_000; _++)
        {
            foreach ((Vector2 position, SolidBrush brush) in _points)
            {
                _neuralNetwork.FeedForward(new[]
                {
                    Map(position.X - _width / 2f, -_width / 2.0, _width / 2.0, 0, 1),
                    Map(position.Y - _height / 2f, -_height / 2.0, _height / 2.0, 0, 1)
                });
                
                Color color = brush.Color;
                _neuralNetwork.BackPropagation(new[]
                {
                    Map(color.R, 255, 1),
                    Map(color.G, 255, 1),
                    Map(color.B, 255, 1)
                }, 1);
            }
        }
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        Train();
        Update();
    }

    private static double Map(double value, double start1, double end1, double start2, double end2)
    {
        return (start2 + (value - start1) / (end1 - start1) * (end2 - start2));
    }

    private static double Map(double value, double max1, double max2)
    {
        return value / max1 * max2;
    }
}