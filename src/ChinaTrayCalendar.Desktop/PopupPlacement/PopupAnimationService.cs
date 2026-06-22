using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ChinaTrayCalendar.Desktop.PopupPlacement;

internal sealed class PopupAnimationService
{
    private static readonly Duration EntranceDuration = new(TimeSpan.FromMilliseconds(150));

    public void PlayEntrance(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);

        window.Opacity = 0;

        DoubleAnimation opacityAnimation = new(1, EntranceDuration)
        {
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut },
        };

        window.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);

        if (window.Content is not UIElement contentElement)
        {
            return;
        }

        TranslateTransform translateTransform = new()
        {
            Y = 8,
        };
        DoubleAnimation translateAnimation = new(0, EntranceDuration)
        {
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut },
        };

        contentElement.RenderTransform = translateTransform;
        translateTransform.BeginAnimation(TranslateTransform.YProperty, translateAnimation);
    }
}
