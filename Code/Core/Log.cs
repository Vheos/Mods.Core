namespace Vheos.Mods.Core;
using BepInEx.Logging;

public static class Log
{
    // Publics
    public static void Debug(object text)
    => _logger.Log(LogLevel.Debug, text);
    public static void Message(object text)
    => _logger.Log(LogLevel.Message, text);
    public static void Level(LogLevel level, object text)
    => _logger.Log(level, text);

    // Privates
    private static ManualLogSource _logger;

    // Initializers
    public static void Initialize(ManualLogSource logger)
        => _logger = logger;
}
