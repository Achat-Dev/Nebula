using System.Text;

namespace Nebula;

public class LogBuffer
{
    private readonly bool r_useEngineLogger;
    private readonly LogLevel r_currentLogLevel;
    private readonly LogLevel r_writeLogLevel;
    private readonly StringBuilder r_stringBuilder = new StringBuilder();
    private readonly List<object> r_objectBuffer = new List<object>();

    internal LogBuffer(LogLevel currentLogLevel, LogLevel writeLogLevel, bool useEngineLogger)
    {
        r_useEngineLogger = useEngineLogger;
        r_currentLogLevel = currentLogLevel;
        r_writeLogLevel = writeLogLevel;
    }

    public LogBuffer Verbose(object o)
    {
        if (r_currentLogLevel <= LogLevel.Verbose)
        {
            r_stringBuilder.Append(o);
        }
        return this;
    }

    public LogBuffer Verbose(string message, params object[] objects)
    {
        if (r_currentLogLevel <= LogLevel.Verbose)
        {
            r_stringBuilder.Append(message);
            r_objectBuffer.AddRange(objects);
        }
        return this;
    }

    public LogBuffer Debug(object o)
    {
        if (r_currentLogLevel <= LogLevel.Debug)
        {
            r_stringBuilder.Append(o);
        }
        return this;
    }

    public LogBuffer Debug(string message, params object[] objects)
    {
        if (r_currentLogLevel <= LogLevel.Debug)
        {
            r_stringBuilder.Append(message);
            r_objectBuffer.AddRange(objects);
        }
        return this;
    }

    public LogBuffer Info(object o)
    {
        if (r_currentLogLevel <= LogLevel.Info)
        {
            r_stringBuilder.Append(o);
        }
        return this;
    }

    public LogBuffer Info(string message, params object[] objects)
    {
        if (r_currentLogLevel <= LogLevel.Info)
        {
            r_stringBuilder.Append(message);
            r_objectBuffer.AddRange(objects);
        }
        return this;
    }

    public LogBuffer Warn(object o)
    {
        if (r_currentLogLevel <= LogLevel.Warn)
        {
            r_stringBuilder.Append(o);
        }
        return this;
    }

    public LogBuffer Warn(string message, params object[] objects)
    {
        if (r_currentLogLevel <= LogLevel.Warn)
        {
            r_stringBuilder.Append(message);
            r_objectBuffer.AddRange(objects);
        }
        return this;
    }

    public LogBuffer Error(object o)
    {
        if (r_currentLogLevel <= LogLevel.Error)
        {
            r_stringBuilder.Append(o);
        }
        return this;
    }

    public LogBuffer Error(string message, params object[] objects)
    {
        if (r_currentLogLevel <= LogLevel.Error)
        {
            r_stringBuilder.Append(message);
            r_objectBuffer.AddRange(objects);
        }
        return this;
    }

    public LogBuffer Fatal(object o)
    {
        r_stringBuilder.Append(o);
        return this;
    }

    public LogBuffer Fatal(string message, params object[] objects)
    {
        r_stringBuilder.Append(message);
        r_objectBuffer.Add(objects);
        return this;
    }

    public void Write()
    {
        if (r_useEngineLogger)
        {
            switch (r_writeLogLevel)
            {
                case LogLevel.Verbose:  Logger.EngineVerbose(r_stringBuilder.ToString(), r_objectBuffer.ToArray()); break;
                case LogLevel.Debug:    Logger.EngineDebug(r_stringBuilder.ToString(), r_objectBuffer.ToArray());   break;
                case LogLevel.Info:     Logger.EngineInfo(r_stringBuilder.ToString(), r_objectBuffer.ToArray());    break;
                case LogLevel.Warn:     Logger.EngineWarn(r_stringBuilder.ToString(), r_objectBuffer.ToArray());    break;
                case LogLevel.Error:    Logger.EngineError(r_stringBuilder.ToString(), r_objectBuffer.ToArray());   break;
                case LogLevel.Fatal:    Logger.EngineFatal(r_stringBuilder.ToString(), r_objectBuffer.ToArray());   break;
            }
        }
        else
        {
            switch (r_writeLogLevel)
            {
                case LogLevel.Verbose:  Logger.Verbose(r_stringBuilder.ToString(), r_objectBuffer.ToArray());   break;
                case LogLevel.Debug:    Logger.Debug(r_stringBuilder.ToString(), r_objectBuffer.ToArray());     break;
                case LogLevel.Info:     Logger.Info(r_stringBuilder.ToString(), r_objectBuffer.ToArray());      break;
                case LogLevel.Warn:     Logger.Warn(r_stringBuilder.ToString(), r_objectBuffer.ToArray());      break;
                case LogLevel.Error:    Logger.Error(r_stringBuilder.ToString(), r_objectBuffer.ToArray());     break;
                case LogLevel.Fatal:    Logger.Fatal(r_stringBuilder.ToString(), r_objectBuffer.ToArray());     break;
            }
        }

        r_stringBuilder.Clear();
        r_objectBuffer.Clear();
        r_objectBuffer.TrimExcess();
    }
}
