using Microsoft.UI.Xaml.Shapes;

namespace Leagueoflegends.Support.UI.Units;

public sealed class RiotActivityBar : Control
{
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(double), typeof(RiotActivityBar), new PropertyMetadata(0d, OnValueChanged));

    public double Value
    {
        get { return (double)GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }

    private Rectangle _backgroundRect;
    private Rectangle _foregroundRect;

    public RiotActivityBar()
    {
        this.DefaultStyleKey = typeof(RiotActivityBar);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _backgroundRect = GetTemplateChild("PART_Background") as Rectangle;
        _foregroundRect = GetTemplateChild("PART_Foreground") as Rectangle;

        this.SizeChanged += ActivityBar_SizeChanged;
        UpdateBarHeight();
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = d as RiotActivityBar;
        control?.UpdateBarHeight();
    }

    private void ActivityBar_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateBarHeight();
    }

    private void UpdateBarHeight()
    {
        if (_foregroundRect != null)
        {
            _foregroundRect.Height = this.ActualHeight * (Value / 100);
        }
    }
}
