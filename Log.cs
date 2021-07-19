namespace Vheos.Tools.ModdingCore
{
    using BepInEx.Logging;
    static public class Log
    {
        // Publics
        static public void Debug(object text)
        => _logger.Log(LogLevel.Debug, text);
        static public void Message(object text)
        => _logger.Log(LogLevel.Message, text);
        static public void Level(LogLevel level, object text)
        => _logger.Log(level, text);

        // Privates
        static private ManualLogSource _logger;

        // Initializers
        static public void Initialize(ManualLogSource logger)
        {
            _logger = logger;
        }
    }
}