using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;

namespace Nebula;

public enum LogLevel
{
    Verbose,
    Debug,
    Info,
    Warn,
    Error,
    Fatal,
}

public static class Logger
{
    private static LogLevel s_logLevel;
    private static ILogger s_devLogger;
    private static ILogger s_appLogger;

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

        s_logLevel = logLevel;
        LoggingLevelSwitch logLevelSwitch = new LoggingLevelSwitch();
        logLevelSwitch.MinimumLevel = (LogEventLevel)logLevel;

        s_devLogger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(logLevelSwitch)
            .WriteTo.Console(outputTemplate: engineOutputTemplate)
            .WriteTo.File(path: filePath, outputTemplate: engineOutputTemplate, shared: true)
            .CreateLogger();

        s_appLogger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(logLevelSwitch)
            .WriteTo.Console(outputTemplate: clientOutputTemplate)
            .WriteTo.File(path: filePath, outputTemplate: clientOutputTemplate, shared: true)
            .CreateLogger();

        EngineInfo("Initialising logger");
    }

    internal static void Flush()
    {
        EngineInfo("Flushing logger");
        Serilog.Log.CloseAndFlush();
    }

    internal static BufferedLogger EngineBegin(LogLevel logLevel)
    {
        return new BufferedLogger(s_logLevel, logLevel);
    }

    internal static void EngineVerbose(object o)
    {
        s_devLogger.Verbose(o.ToString());
    }

    internal static void EngineVerbose(string message)
    {
        s_devLogger.Verbose(message);
    }

    internal static void EngineVerbose(string message, params object[] objects)
    {
        s_devLogger.Verbose(message, objects);
    }

    internal static void EngineDebug(object o)
    {
        s_devLogger.Debug(o.ToString());
    }

    internal static void EngineDebug(string message)
    {
        s_devLogger.Debug(message);
    }

    internal static void EngineDebug(string message, params object[] objects)
    {
        s_devLogger.Debug(message, objects);
    }

    internal static void EngineInfo(object o)
    {
        s_devLogger.Information(o.ToString());
    }

    internal static void EngineInfo(string message)
    {
        s_devLogger.Information(message);
    }

    internal static void EngineInfo(string message, params object[] objects)
    {
        s_devLogger.Information(message, objects);
    }

    internal static void EngineWarn(object o)
    {
        s_devLogger.Warning(o.ToString() + Environment.NewLine + new StackTrace(1, true));
    }

    internal static void EngineWarn(string message)
    {
        s_devLogger.Warning(message + Environment.NewLine + new StackTrace(1, true).ToString());
    }

    internal static void EngineWarn(string message, params object[] objects)
    {
        message += Environment.NewLine + new StackTrace(1, true).ToString();
        s_devLogger.Warning(message, objects);
    }

    internal static void EngineError(object o)
    {
        s_devLogger.Error(o.ToString() + Environment.NewLine + new StackTrace(1, true));
    }

    internal static void EngineError(string message)
    {
        s_devLogger.Error(message + Environment.NewLine + new StackTrace(1, true).ToString());
    }

    internal static void EngineError(string message, params object[] objects)
    {
        message += Environment.NewLine + new StackTrace(1, true).ToString();
        s_devLogger.Error(message, objects);
    }

    internal static void EngineFatal(object o)
    {
        s_devLogger.Fatal(o.ToString() + Environment.NewLine + new StackTrace(1, true));
    }

    internal static void EngineFatal(string message)
    {
        s_devLogger.Fatal(message + Environment.NewLine + new StackTrace(1, true).ToString());
    }

    internal static void EngineFatal(string message, params object[] objects)
    {
        message += Environment.NewLine + new StackTrace(1, true).ToString();
        s_devLogger.Fatal(message, objects);
    }

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

    public static void Verbose(object o) { s_appLogger.Verbose(o.ToString()); }
    public static void Debug(object o) { s_appLogger.Debug(o.ToString()); }
    public static void Info(object o) { s_appLogger.Information(o.ToString()); }
    public static void Warn(object o) { s_appLogger.Warning(o.ToString()); }
    public static void Error(object o) { s_appLogger.Error(o.ToString()); }
    public static void Fatal(object o) { s_appLogger.Fatal(o.ToString()); }

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
