namespace TMTemplateGen;

public class Logger
{
    public const int ERROR = 0;
    public const int INFO = 1;
    public const int DEBUG = 2;

    public static int LogLevel { get; set; } = 0;
    
    private static void Writeln(int level, string line)
    {
        if (level == ERROR)
            Console.Error.WriteLine(line);
        else if (level <= LogLevel)
            Console.WriteLine(line);
    }

    public static void Error(string line) => Writeln(ERROR, line);
    public static void Info(string line) => Writeln(INFO, line);
    public static void Debug(string line) => Writeln(DEBUG, line);
}
