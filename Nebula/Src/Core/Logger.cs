using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;

namespace Nebula;

public enum LogLevel
{
    Debug = 1,
    Info = 2,
    Warn = 3,
    Error = 4,
    Fatal = 5,
}

public static class Logger
{
    private static ILogger m_devLogger;
    private static ILogger m_appLogger;

    internal static void Init(LogLevel logLevel)
    {
        string filePath = Game.GetPersistentPath() + "log.txt";
        string engineOutputTemplate = "[{Timestamp:HH:mm:ss} {Level}] ENGINE: {Message}{NewLine}{Exception}";
        string clientOutputTemplate = "[{Timestamp:HH:mm:ss} {Level}] APP: {Message}{NewLine}{Exception}";

        // Delete existing log files because serilog opens files in FileMode.Append
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        LoggingLevelSwitch logLevelSwitch = new LoggingLevelSwitch();
        logLevelSwitch.MinimumLevel = (LogEventLevel)logLevel;

        m_devLogger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(logLevelSwitch)
            .WriteTo.Console(outputTemplate: engineOutputTemplate)
            .WriteTo.File(path: filePath, outputTemplate: engineOutputTemplate, shared: true)
            .CreateLogger();

        m_appLogger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(logLevelSwitch)
            .WriteTo.Console(outputTemplate: clientOutputTemplate)
            .WriteTo.File(path: filePath, outputTemplate: clientOutputTemplate, shared: true)
            .CreateLogger();

        EngineInfo("Initialising log");
    }

    internal static void Flush()
    {
        EngineInfo("Flushing log");
        Serilog.Log.CloseAndFlush();
    }

    internal static void EngineDebug(object o) { m_devLogger.Debug(o.ToString()); }
    internal static void EngineInfo(object o) { m_devLogger.Information(o.ToString()); }
    internal static void EngineWarn(object o) { m_devLogger.Warning(o.ToString() + Environment.NewLine + new StackTrace(1, true).ToString()); }
    internal static void EngineError(object o) { m_devLogger.Error(o.ToString() + Environment.NewLine + new StackTrace(1, true).ToString()); }
    internal static void EngineFatal(object o) { m_devLogger.Fatal(o.ToString() + Environment.NewLine + new StackTrace(1, true).ToString()); }

    [Conditional("DEBUG")]
    internal static void EngineAssert(bool condition, string message = "Assertion failed:")
    {
        if (condition)
        {
            StackTrace trace = new StackTrace(1, true);
            EngineFatal(message + Environment.NewLine + trace.ToString());
            Environment.Exit(1);
        }
    }

    public static void Debug(object o) { m_appLogger.Debug(o.ToString()); }
    public static void Info(object o) { m_appLogger.Information(o.ToString()); }
    public static void Warn(object o) { m_appLogger.Warning(o.ToString()); }
    public static void Error(object o) { m_appLogger.Error(o.ToString()); }
    public static void Fatal(object o) { m_appLogger.Fatal(o.ToString()); }

    [Conditional("DEBUG")]
    public static void Assert(bool condition, string message = "Assertion failed:")
    {
        if (condition)
        {
            StackTrace trace = new StackTrace(1, true);
            Fatal(message + Environment.NewLine + trace.ToString());
            Environment.Exit(1);
        }
    }
}
