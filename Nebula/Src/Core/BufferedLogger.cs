using System.Text;

namespace Nebula;

public class BufferedLogger
{
    private readonly bool r_useEngineLogger;
    private readonly LogLevel r_currentLogLevel;
    private readonly LogLevel r_writeLogLevel;
    private readonly StringBuilder r_stringBuilder = new StringBuilder();
    private readonly List<object> r_objectBuffer = new List<object>();

    internal BufferedLogger(LogLevel currentLogLevel, LogLevel writeLogLevel, bool useEngineLogger)
    {
        r_useEngineLogger = useEngineLogger;
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
            r_stringBuilder.Append(message);
            r_objectBuffer.AddRange(objects);
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
            r_stringBuilder.Append(message);
            r_objectBuffer.AddRange(objects);
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
            r_stringBuilder.Append(message);
            r_objectBuffer.AddRange(objects);
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
            r_stringBuilder.Append(message);
            r_objectBuffer.AddRange(objects);
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
            r_stringBuilder.Append(message);
            r_objectBuffer.AddRange(objects);
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
