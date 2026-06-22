using System.Drawing;
using ChinaTrayCalendar.Desktop.PopupPlacement;

namespace ChinaTrayCalendar.Desktop.Tests;

public sealed class PopupPlacementCalculatorTests
{
    private static readonly Size PopupSize = new(width: 360, height: 420);

    [Fact]
    public void CalculatePlacesPopupAboveBottomTaskbar()
    {
        Point location = PopupPlacementCalculator.Calculate(
            new Rectangle(0, 0, 1920, 1080),
            new Rectangle(0, 0, 1920, 1040),
            new Point(1800, 1060),
            PopupSize,
            margin: 8);

        Assert.Equal(new Point(1552, 612), location);
    }

    [Fact]
    public void CalculatePlacesPopupBelowTopTaskbar()
    {
        Point location = PopupPlacementCalculator.Calculate(
            new Rectangle(0, 0, 1920, 1080),
            new Rectangle(0, 40, 1920, 1040),
            new Point(120, 20),
            PopupSize,
            margin: 8);

        Assert.Equal(new Point(8, 48), location);
    }

    [Fact]
    public void CalculatePlacesPopupRightOfLeftTaskbar()
    {
        Point location = PopupPlacementCalculator.Calculate(
            new Rectangle(0, 0, 1920, 1080),
            new Rectangle(48, 0, 1872, 1080),
            new Point(24, 900),
            PopupSize,
            margin: 8);

        Assert.Equal(new Point(56, 652), location);
    }

    [Fact]
    public void CalculatePlacesPopupLeftOfRightTaskbar()
    {
        Point location = PopupPlacementCalculator.Calculate(
            new Rectangle(0, 0, 1920, 1080),
            new Rectangle(0, 0, 1872, 1080),
            new Point(1900, 900),
            PopupSize,
            margin: 8);

        Assert.Equal(new Point(1504, 652), location);
    }

    [Fact]
    public void CalculateKeepsPopupInsideWorkingAreaWhenNoTaskbarEdgeIsDetected()
    {
        Point location = PopupPlacementCalculator.Calculate(
            new Rectangle(0, 0, 1920, 1080),
            new Rectangle(0, 0, 1920, 1080),
            new Point(12, 12),
            PopupSize,
            margin: 8);

        Assert.Equal(new Point(8, 8), location);
    }

    [Fact]
    public void CalculateRejectsInvalidPopupSize()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PopupPlacementCalculator.Calculate(
            new Rectangle(0, 0, 1920, 1080),
            new Rectangle(0, 0, 1920, 1080),
            new Point(0, 0),
            new Size(width: 0, height: 420),
            margin: 8));
    }
}
