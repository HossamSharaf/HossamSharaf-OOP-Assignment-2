namespace PatternsLab.Problems.Singleton;

using System;
using System.Threading;

public class AppConfig
{
    public static int LoadCount;

    public string DbConnection { get; set; }
    public string Theme { get; set; }

    // R1 & R4: Thread-safe, lazily initialized single instance
    private static readonly Lazy<AppConfig> _instance = new(() => new AppConfig());

    // Public access point to the singleton instance
    public static AppConfig Instance => _instance.Value;

    // R5: Private constructor prevents other code from using 'new AppConfig()'
    private AppConfig()
    {
        LoadCount++;
        Console.WriteLine($"[AppConfig] Loading settings from disk... (load #{LoadCount})");
        Thread.Sleep(300);
        DbConnection = "Server=localhost;Db=School";
        Theme = "Light";
    }
}

public class DatabaseService
{
    // R2: Now references the shared Singleton instance
    public AppConfig Config => AppConfig.Instance;

    public void Connect() => Console.WriteLine($"Connecting to {Config.DbConnection}");
}

public class UiService
{
    // R2: Now references the shared Singleton instance
    public AppConfig Config => AppConfig.Instance;

    public void Render() => Console.WriteLine($"UI is using theme: {Config.Theme}");
}