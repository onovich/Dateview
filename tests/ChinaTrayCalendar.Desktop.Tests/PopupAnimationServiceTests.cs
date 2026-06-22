using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using ChinaTrayCalendar.Desktop.PopupPlacement;

namespace ChinaTrayCalendar.Desktop.Tests;

public sealed class PopupAnimationServiceTests
{
    [Fact]
    public void PlayEntranceAnimatesContentTransformWithoutSettingWindowTransform()
    {
        Exception? exception = null;
        bool isTransformGroup = false;
        bool hasScaleTransform = false;
        bool hasTranslateTransform = false;
        double scaleX = 0;
        double scaleY = 0;
        double translateY = 0;
        Point renderTransformOrigin = default;
        object? windowTransformLocalValue = null;
        Thread thread = new(() =>
        {
            try
            {
                Border content = new();
                Window window = new()
                {
                    Content = content,
                };
                PopupAnimationService service = new();

                service.PlayEntrance(window);

                TransformGroup contentTransform = Assert.IsType<TransformGroup>(content.RenderTransform);
                isTransformGroup = true;
                ScaleTransform scaleTransform = Assert.IsType<ScaleTransform>(contentTransform.Children[0]);
                TranslateTransform translateTransform = Assert.IsType<TranslateTransform>(contentTransform.Children[1]);
                hasScaleTransform = true;
                hasTranslateTransform = true;
                scaleX = scaleTransform.ScaleX;
                scaleY = scaleTransform.ScaleY;
                translateY = translateTransform.Y;
                renderTransformOrigin = content.RenderTransformOrigin;
                windowTransformLocalValue = window.ReadLocalValue(UIElement.RenderTransformProperty);
                window.Close();
            }
            catch (Exception caughtException)
            {
                exception = caughtException;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);

        thread.Start();
        thread.Join();

        Assert.Null(exception);
        Assert.True(isTransformGroup);
        Assert.Equal(new Point(0.5, 0), renderTransformOrigin);
        Assert.True(hasScaleTransform);
        Assert.True(hasTranslateTransform);
        Assert.Equal(0.985, scaleX, precision: 3);
        Assert.Equal(0.985, scaleY, precision: 3);
        Assert.Equal(6, translateY);
        Assert.Equal(DependencyProperty.UnsetValue, windowTransformLocalValue);
    }

    [Fact]
    public void PlayEntranceFallsBackToOpacityWhenContentIsNotUiElement()
    {
        Exception? exception = null;
        Thread thread = new(() =>
        {
            try
            {
                Window window = new()
                {
                    Content = "Dateview",
                };
                PopupAnimationService service = new();

                service.PlayEntrance(window);

                window.Close();
            }
            catch (Exception caughtException)
            {
                exception = caughtException;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);

        thread.Start();
        thread.Join();

        Assert.Null(exception);
    }

    [Fact]
    public void PlayExitAsyncAnimatesContentTransformWithoutSettingWindowTransform()
    {
        Exception? exception = null;
        bool isTransformGroup = false;
        bool hasScaleTransform = false;
        bool hasTranslateTransform = false;
        object? windowTransformLocalValue = null;
        double finalOpacity = -1;
        Thread thread = new(() =>
        {
            try
            {
                Border content = new();
                Window window = new()
                {
                    Content = content,
                };
                PopupAnimationService service = new();

                window.Show();
                Task exitTask = service.PlayExitAsync(window);

                TransformGroup contentTransform = Assert.IsType<TransformGroup>(content.RenderTransform);
                isTransformGroup = true;
                Assert.IsType<ScaleTransform>(contentTransform.Children[0]);
                Assert.IsType<TranslateTransform>(contentTransform.Children[1]);
                hasScaleTransform = true;
                hasTranslateTransform = true;
                windowTransformLocalValue = window.ReadLocalValue(UIElement.RenderTransformProperty);
                WaitForTask(window.Dispatcher, exitTask);
                finalOpacity = window.Opacity;
                window.Close();
            }
            catch (Exception caughtException)
            {
                exception = caughtException;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);

        thread.Start();
        thread.Join();

        Assert.Null(exception);
        Assert.True(isTransformGroup);
        Assert.True(hasScaleTransform);
        Assert.True(hasTranslateTransform);
        Assert.Equal(DependencyProperty.UnsetValue, windowTransformLocalValue);
        Assert.Equal(1, finalOpacity);
    }

    [Fact]
    public void PlayExitAsyncFallsBackToOpacityWhenContentIsNotUiElement()
    {
        Exception? exception = null;
        Thread thread = new(() =>
        {
            try
            {
                Window window = new()
                {
                    Content = "Dateview",
                };
                PopupAnimationService service = new();

                window.Show();
                WaitForTask(window.Dispatcher, service.PlayExitAsync(window));
                window.Close();
            }
            catch (Exception caughtException)
            {
                exception = caughtException;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);

        thread.Start();
        thread.Join();

        Assert.Null(exception);
    }

    private static void WaitForTask(Dispatcher dispatcher, Task task)
    {
        DispatcherFrame frame = new();
        task.ContinueWith(
            _ => dispatcher.BeginInvoke(() => frame.Continue = false),
            TaskScheduler.Default);

        Dispatcher.PushFrame(frame);
        task.GetAwaiter().GetResult();
    }
}
