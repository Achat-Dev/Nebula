using System.Text;

namespace Nebula;

internal class BufferedLogger
{
    private readonly LogLevel r_currentLogLevel;
    private readonly LogLevel r_writeLogLevel;
    private readonly StringBuilder r_stringBuilder = new StringBuilder();

    public BufferedLogger(LogLevel currentLogLevel, LogLevel writeLogLevel)
    {
        r_currentLogLevel = currentLogLevel;
        r_writeLogLevel = writeLogLevel;
    }

    public BufferedLogger Verbose(object o)
    {
        if (r_currentLogLevel <= LogLevel.Verbose)
        {
            r_stringBuilder.Append(o);
        }
        return this;
    }

    public BufferedLogger Verbose(string message, params object[] objects)
    {
        if (r_currentLogLevel <= LogLevel.Verbose)
        {
            r_stringBuilder.AppendFormat(message, objects);
        }
        return this;
    }

    public BufferedLogger Debug(object o)
    {
        if (r_currentLogLevel <= LogLevel.Debug)
        {
            r_stringBuilder.Append(o);
        }
        return this;
    }

    public BufferedLogger Debug(string message, params object[] objects)
    {
        if (r_currentLogLevel <= LogLevel.Debug)
        {
            r_stringBuilder.AppendFormat(message, objects);
        }
        return this;
    }

    public BufferedLogger Info(object o)
    {
        if (r_currentLogLevel <= LogLevel.Info)
        {
            r_stringBuilder.Append(o);
        }
        return this;
    }

    public BufferedLogger Info(string message, params object[] objects)
    {
        if (r_currentLogLevel <= LogLevel.Info)
        {
            r_stringBuilder.AppendFormat(message, objects);
        }
        return this;
    }

    public BufferedLogger Warn(object o)
    {
        if (r_currentLogLevel <= LogLevel.Warn)
        {
            r_stringBuilder.Append(o);
        }
        return this;
    }

    public BufferedLogger Warn(string message, params object[] objects)
    {
        if (r_currentLogLevel <= LogLevel.Warn)
        {
            r_stringBuilder.AppendFormat(message, objects);
        }
        return this;
    }

    public BufferedLogger Error(object o)
    {
        if (r_currentLogLevel <= LogLevel.Error)
        {
            r_stringBuilder.Append(o);
        }
        return this;
    }

    public BufferedLogger Error(string message, params object[] objects)
    {
        if (r_currentLogLevel <= LogLevel.Error)
        {
            r_stringBuilder.AppendFormat(message, objects);
        }
        return this;
    }

    public BufferedLogger Fatal(object o)
    {
        r_stringBuilder.Append(o);
        return this;
    }

    public BufferedLogger Fatal(string message, params object[] objects)
    {
        r_stringBuilder.AppendFormat(message, objects);
        return this;
    }

    public void Write()
    {
        switch (r_writeLogLevel)
        {
            case LogLevel.Verbose:
                Logger.EngineVerbose(r_stringBuilder);
                break;
            case LogLevel.Debug:
                Logger.EngineDebug(r_stringBuilder);
                break;
            case LogLevel.Info:
                Logger.EngineInfo(r_stringBuilder);
                break;
            case LogLevel.Warn:
                Logger.EngineWarn(r_stringBuilder);
                break;
            case LogLevel.Error:
                Logger.EngineError(r_stringBuilder);
                break;
            case LogLevel.Fatal:
                Logger.EngineFatal(r_stringBuilder);
                break;
        }

        r_stringBuilder.Clear();
    }
}
