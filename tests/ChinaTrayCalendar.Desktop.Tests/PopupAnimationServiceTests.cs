using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ChinaTrayCalendar.Desktop.PopupPlacement;

namespace ChinaTrayCalendar.Desktop.Tests;

public sealed class PopupAnimationServiceTests
{
    [Fact]
    public void PlayEntranceAnimatesContentTransformWithoutSettingWindowTransform()
    {
        Exception? exception = null;
        Transform? contentTransform = null;
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

                contentTransform = content.RenderTransform;
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
        Assert.IsType<TranslateTransform>(contentTransform);
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
}
