namespace Vheos.Mods.Core;
using Vheos.Helpers.KeyCodeCache;
using Vheos.Helpers.RNG;

static public class Extensions
{
    static public bool IsValidKeyCode(this ModSetting<string> t)
        => t != null && t.Value.IsValidKeyCode();
    static public KeyCode ToKeyCode(this ModSetting<string> t)
        => t != null ? t.Value.ToKeyCode() : KeyCode.None;

    static public bool Roll(this ModSetting<float> t)
        => t.Value.Roll();
    static public bool RollPercent(this ModSetting<float> t)
        => t.Value.RollPercent();
    static public bool RollPercent(this ModSetting<int> t)
        => t.Value.RollPercent();
}
