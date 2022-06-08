namespace Vheos.Mods.Core;
using Vheos.Helpers.KeyCodeCache;

static public class Extensions
{
    static public bool IsValidKeyCode(this ModSetting<string> t)
        => t != null && t.Value.IsValidKeyCode();
    static public KeyCode ToKeyCode(this ModSetting<string> t)
        => t != null ? t.Value.ToKeyCode() : KeyCode.None;
}
