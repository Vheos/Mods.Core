namespace Vheos.Mods.Core;

public class ColorSettings
{
    // Settings
    public readonly ModSetting<Color> Sliders;
    public readonly ModSetting<Vector4> Numbers;
    public ColorSettings(AMod mod, string settingKey, Color defaultColor)
    {
        Sliders = mod.CreateSetting(settingKey + nameof(Sliders), defaultColor);
        Numbers = mod.CreateSetting(settingKey + nameof(Numbers), (Vector4)defaultColor);

        // Events
        Sliders.AddEventSilently(OnChangeSliders);
        Numbers.AddEventSilently(OnChangeNumbers);
        ConfigHelper.NumericalColorRange.AddEvent(OnChangeSliders);
    }
    public void Format(string displayName)
    {
        Sliders.Format(displayName);
        Numbers.DisplayResetButton = false;
        Numbers.Format("", Sliders, t => ColorRange != 0);
    }
    public void Format(string displayName, ModSetting<bool> toggle)
    {
        Sliders.Format(displayName, toggle);
        Numbers.DisplayResetButton = false;
        Numbers.Format("", toggle, t => t && ColorRange != 0);
    }
    public string Description
    {
        get => Sliders.Description;
        set => Sliders.Description = value;
    }

    // Private
    private void OnChangeSliders()
    => Numbers.SetSilently(Sliders.Value * ColorRange);
    private void OnChangeNumbers()
    {
        Numbers.SetSilently(Numbers.Value.Clamp(0, ColorRange));
        Sliders.SetSilently(Numbers.Value / ColorRange);
    }
    private int ColorRange
    => ConfigHelper.NumericalColorRange;

    // Operators
    public static implicit operator ModSetting<Color>(ColorSettings colorSettings)
    => colorSettings.Sliders;
    public static implicit operator Color(ColorSettings colorSettings)
    => colorSettings.Sliders.Value;
}
