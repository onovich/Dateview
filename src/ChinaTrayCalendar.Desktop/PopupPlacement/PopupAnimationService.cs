using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ChinaTrayCalendar.Desktop.PopupPlacement;

internal sealed class PopupAnimationService
{
    private static readonly Duration EntranceDuration = new(TimeSpan.FromMilliseconds(170));
    private static readonly Duration ExitDuration = new(TimeSpan.FromMilliseconds(140));

    public void PlayEntrance(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);

        StopAnimations(window);
        window.Opacity = 0;

        DoubleAnimation opacityAnimation = new(1, EntranceDuration)
        {
            EasingFunction = CreateEntranceEase(),
        };

        window.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);

        ContentMotion? contentMotion = PrepareContentMotion(window, scale: 0.985, translateY: 6);
        if (contentMotion is null)
        {
            return;
        }

        contentMotion.Scale.BeginAnimation(
            ScaleTransform.ScaleXProperty,
            new DoubleAnimation(1, EntranceDuration) { EasingFunction = CreateEntranceEase() });
        contentMotion.Scale.BeginAnimation(
            ScaleTransform.ScaleYProperty,
            new DoubleAnimation(1, EntranceDuration) { EasingFunction = CreateEntranceEase() });
        contentMotion.Translate.BeginAnimation(
            TranslateTransform.YProperty,
            new DoubleAnimation(0, EntranceDuration) { EasingFunction = CreateEntranceEase() });
    }

    public Task PlayExitAsync(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);

        StopAnimations(window);
        window.Opacity = 1;

        ContentMotion? contentMotion = PrepareContentMotion(window, scale: 1, translateY: 0);
        if (contentMotion is not null)
        {
            contentMotion.Scale.BeginAnimation(
                ScaleTransform.ScaleXProperty,
                new DoubleAnimation(0.985, ExitDuration) { EasingFunction = CreateExitEase() });
            contentMotion.Scale.BeginAnimation(
                ScaleTransform.ScaleYProperty,
                new DoubleAnimation(0.985, ExitDuration) { EasingFunction = CreateExitEase() });
            contentMotion.Translate.BeginAnimation(
                TranslateTransform.YProperty,
                new DoubleAnimation(4, ExitDuration) { EasingFunction = CreateExitEase() });
        }

        TaskCompletionSource completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        DoubleAnimation opacityAnimation = new(0, ExitDuration)
        {
            EasingFunction = CreateExitEase(),
            FillBehavior = FillBehavior.HoldEnd,
        };

        opacityAnimation.Completed += (_, _) =>
        {
            StopAnimations(window);
            ResetVisualState(window, contentMotion);
            completion.TrySetResult();
        };

        window.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);

        return completion.Task;
    }

    private static ContentMotion? PrepareContentMotion(Window window, double scale, double translateY)
    {
        if (window.Content is not UIElement contentElement)
        {
            return null;
        }

        ScaleTransform scaleTransform = new(scale, scale);
        TranslateTransform translateTransform = new()
        {
            Y = translateY,
        };
        TransformGroup transformGroup = new();
        transformGroup.Children.Add(scaleTransform);
        transformGroup.Children.Add(translateTransform);

        contentElement.RenderTransformOrigin = new System.Windows.Point(0.5, 0);
        contentElement.RenderTransform = transformGroup;

        return new ContentMotion(scaleTransform, translateTransform);
    }

    private static void StopAnimations(Window window)
    {
        window.BeginAnimation(UIElement.OpacityProperty, null);

        if (window.Content is not UIElement contentElement)
        {
            return;
        }

        if (contentElement.RenderTransform is TransformGroup transformGroup)
        {
            foreach (Transform transform in transformGroup.Children)
            {
                StopTransformAnimation(transform);
            }

            return;
        }

        StopTransformAnimation(contentElement.RenderTransform);
    }

    private static void StopTransformAnimation(Transform transform)
    {
        switch (transform)
        {
            case ScaleTransform scaleTransform:
                scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, null);
                break;
            case TranslateTransform translateTransform:
                translateTransform.BeginAnimation(TranslateTransform.YProperty, null);
                break;
        }
    }

    private static void ResetVisualState(Window window, ContentMotion? contentMotion)
    {
        window.Opacity = 1;

        if (contentMotion is null)
        {
            return;
        }

        contentMotion.Scale.ScaleX = 1;
        contentMotion.Scale.ScaleY = 1;
        contentMotion.Translate.Y = 0;
    }

    private static IEasingFunction CreateEntranceEase()
    {
        return new CubicEase { EasingMode = EasingMode.EaseOut };
    }

    private static IEasingFunction CreateExitEase()
    {
        return new CubicEase { EasingMode = EasingMode.EaseIn };
    }

    private sealed record ContentMotion(
        ScaleTransform Scale,
        TranslateTransform Translate);
}
