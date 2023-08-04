namespace Infrastructure.Settings;

public class DatabaseSetting : object
{
	public DatabaseSetting() : base()
	{
	}

	public string? Provider { get; set; }

	public string? SQLiteConnectionString { get; set; }

	public string? SqlServerConnectionString { get; set; }

	public string? PostgreSqlConnectionString { get; set; }

	public Enums.DatabaseProviderType DatabaseProviderType
	{
		get
		{
			var result =
				Enums.DatabaseProviderType.Unknown;

			if (Provider is null)
			{
				return result;
			}

			Provider = Provider.Replace
				(oldValue: " ", newValue: string.Empty);

			switch (Provider.ToLower())
			{
				case "sqlite":
				{
					result = Enums
						.DatabaseProviderType.SQLite;

					break;
				}

				case "sqlserver":
				case "mssqlserver":
				{
					result = Enums
						.DatabaseProviderType.SqlServer;

					break;
				}

				case "postgre":
				case "postgresql":
				{
					result = Enums
						.DatabaseProviderType.PostgreSql;

					break;
				}
			}

			return result;
		}
	}
}
