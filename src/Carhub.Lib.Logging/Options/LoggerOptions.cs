namespace Carhub.Lib.Logging.Options;

public sealed class LoggerOptions
{
    public string Level { get; set; } = string.Empty;
    public SeqOptions Seq { get; set; } = new();
    public ConsoleOptions Console { get; set; } = new();
    public FileOptions File { get; set; } = new();
    public AppOptions App { get; set; } = new();
    public Dictionary<string, object> Tags { get; set; } = [];
    public Dictionary<string, string> MinimumLevelOverrides { get; set; } = [];
    public List<string> ExcludePaths { get; set; } = [];
    public List<string> ExcludeProperties { get; set; } = [];
}