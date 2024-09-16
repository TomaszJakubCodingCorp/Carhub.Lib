namespace Carhub.Lib.Postgres.Options;

public class PostgresOptions
{
    internal const string OptionsName = nameof(PostgresOptions);
    public string ConnectionString { get; set; } = string.Empty;
}