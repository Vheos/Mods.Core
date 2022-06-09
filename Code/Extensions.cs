namespace Vheos.Mods.Core;
using Vheos.Helpers.KeyCodeCache;
using Vheos.Helpers.RNG;

public static class Extensions
{
    public static bool IsValidKeyCode(this ModSetting<string> t)
        => t != null && t.Value.IsValidKeyCode();
    public static KeyCode ToKeyCode(this ModSetting<string> t)
        => t != null ? t.Value.ToKeyCode() : KeyCode.None;

    public static bool Roll(this ModSetting<float> t)
        => t.Value.Roll();
    public static bool RollPercent(this ModSetting<float> t)
        => t.Value.RollPercent();
    public static bool RollPercent(this ModSetting<int> t)
        => t.Value.RollPercent();
}
