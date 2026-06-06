//namespace Core;

//public class LogWriter : IDisposable
//{
//    private readonly StreamWriter _writer;

//    public LogWriter(string path)
//    {
//        _writer = new StreamWriter(path, append: true)
//        {
//            AutoFlush = true
//        };
//    }

//    public void WriteLog(string level, string message)
//    {
//        var timeStamp = DateTime.Now.ToString("s"); // ISO 8601 format
//        var logEntry = $"{timeStamp} [{level}] {message}";
//        _writer.WriteLine(logEntry);
//    }

//    public void Dispose()
//    {
//        _writer.Dispose();
//    }
//}

namespace Core;

public class LogWriter : IDisposable
{
    private readonly StreamWriter _writer;

    public LogWriter(string path)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        _writer = new StreamWriter(path, append: true)
        {
            AutoFlush = true
        };
    }

    // Log con usuario — requerido por el taller
    public void WriteLog(string level, string username, string message)
    {
        var timeStamp = DateTime.Now.ToString("s");
        _writer.WriteLine($"{timeStamp} [{level.ToUpper()}] [user:{username}] {message}");
    }

    // Sin usuario (para antes del login)
    public void WriteLog(string level, string message)
    {
        WriteLog(level, "system", message);
    }

    public void Dispose()
    {
        _writer.Dispose();
    }
}