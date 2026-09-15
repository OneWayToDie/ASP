using System.Data;
using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AcademyAgain.Helpers
{
	public static class BackupExport
	{
		private const string JsonFormat = "academyagain-backup-v1";

		public sealed class BackupDto
		{
			public string format { get; set; } = JsonFormat;
			public List<BackupTableDto> tables { get; set; } = new();
		}

		public sealed class BackupTableDto
		{
			public string name { get; set; } = "";
			public bool identity { get; set; }
			public List<string> columns { get; set; } = new();
			public List<string> types { get; set; } = new();
			public List<List<object?>> rows { get; set; } = new();
		}

		private static IEnumerable<string> EntityTables(AcademyAgainContext context)
			=> context.Model.GetEntityTypes()
				.Select(et => et.GetTableName())
				.Where(name => !string.IsNullOrEmpty(name))
				.Cast<string>()
				.OrderBy(name => name, StringComparer.OrdinalIgnoreCase);

		public static async Task<BackupDto> ReadBackupAsync(AcademyAgainContext context)
		{
			var connection = context.Database.GetDbConnection();
			var wasClosed = connection.State == ConnectionState.Closed;
			if (wasClosed) await connection.OpenAsync();

			try
			{
				var dto = new BackupDto();
				foreach (var table in EntityTables(context))
				{
					var entityType = context.Model.GetEntityTypes().First(et => et.GetTableName() == table);
					var props = entityType.GetProperties().ToList();
					var columns = props.Select(p => p.GetColumnName()!).ToList();
					var hasIdentity = props.Any(p => p.ValueGenerated == ValueGenerated.OnAdd);

					using var cmd = connection.CreateCommand();
					cmd.CommandText = $"SELECT [{string.Join("],[", columns)}] FROM [{table}]";
					using var reader = await cmd.ExecuteReaderAsync();

					var rows = new List<List<object?>>();
					while (await reader.ReadAsync())
					{
						var row = new List<object?>(columns.Count);
						for (int i = 0; i < reader.FieldCount; i++)
						{
							row.Add(reader.IsDBNull(i) ? null : Normalize(reader.GetValue(i)));
						}
						rows.Add(row);
					}
					await reader.CloseAsync();

					dto.tables.Add(new BackupTableDto
					{
						name = table,
						identity = hasIdentity,
						columns = columns,
						types = props.Select(p => p.ClrType.FullName ?? p.ClrType.Name).ToList(),
						rows = rows
					});
				}
				return dto;
			}
			finally
			{
				if (wasClosed) await connection.CloseAsync();
			}
		}

		private static object Normalize(object value)
			=> value switch
			{
				char[] chars => new string(chars),
				_ => value
			};

		public static string BuildSql(BackupDto dto)
		{
			var sb = new StringBuilder();
			sb.AppendLine("-- ============================================================");
			sb.AppendLine("-- AcademyAgain — резервная копия базы данных");
			sb.AppendLine($"-- Дата создания: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
			sb.AppendLine("-- ============================================================");
			sb.AppendLine();
			sb.AppendLine("SET NOCOUNT ON;");
			sb.AppendLine("SET XACT_ABORT ON;");
			sb.AppendLine();
			sb.AppendLine("-- Отключаем проверку внешних ключей на время восстановления");
			sb.AppendLine("EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';");
			sb.AppendLine();

			foreach (var t in dto.tables.OrderBy(t => t.name, StringComparer.OrdinalIgnoreCase))
			{
				if (t.rows.Count == 0) continue;

				sb.AppendLine($"-- [{t.name}] ({t.rows.Count} строк)");
				if (t.identity) sb.AppendLine($"SET IDENTITY_INSERT [{t.name}] ON;");

				foreach (var chunk in t.rows.Chunk(100))
				{
					sb.AppendLine($"INSERT INTO [{t.name}] ([{string.Join("],[", t.columns)}]) VALUES");
					var lines = chunk.Select(r => "(" + string.Join(",", r.Select(SqlLiteral)) + ")");
					sb.AppendLine(string.Join("," + Environment.NewLine, lines) + ";");
				}

				if (t.identity) sb.AppendLine($"SET IDENTITY_INSERT [{t.name}] OFF;");
				sb.AppendLine();
			}

			sb.AppendLine("-- Включаем проверку внешних ключей обратно");
			sb.AppendLine("EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';");
			sb.AppendLine();
			return sb.ToString();
		}

		private static string SqlLiteral(object? value)
		{
			return value switch
			{
				null => "NULL",
				bool b => b ? "1" : "0",
				byte v => v.ToString(CultureInfo.InvariantCulture),
				sbyte v => v.ToString(CultureInfo.InvariantCulture),
				short v => v.ToString(CultureInfo.InvariantCulture),
				ushort v => v.ToString(CultureInfo.InvariantCulture),
				int v => v.ToString(CultureInfo.InvariantCulture),
				uint v => v.ToString(CultureInfo.InvariantCulture),
				long v => v.ToString(CultureInfo.InvariantCulture),
				ulong v => v.ToString(CultureInfo.InvariantCulture),
				decimal v => v.ToString(CultureInfo.InvariantCulture),
				double v => v.ToString("R", CultureInfo.InvariantCulture),
				float v => v.ToString("R", CultureInfo.InvariantCulture),
				DateTime v => "'" + v.ToString("yyyy-MM-ddTHH:mm:ss.fffffff") + "'",
				DateTimeOffset v => "'" + v.ToString("O") + "'",
				DateOnly v => "'" + v.ToString("yyyy-MM-dd") + "'",
				TimeOnly v => "'" + v.ToString("HH:mm:ss.fffffff") + "'",
				TimeSpan v => "'" + (v.Days != 0
					? v.ToString(@"d\.hh\:mm\:ss\.fffffff")
					: v.ToString(@"hh\:mm\:ss\.fffffff")) + "'",
				Guid v => "'" + v.ToString("D") + "'",
				byte[] v => "0x" + Convert.ToHexString(v),
				_ => "N'" + Convert.ToString(value, CultureInfo.InvariantCulture)!.Replace("'", "''") + "'"
			};
		}

		public static async Task RestoreJsonAsync(AcademyAgainContext context, BackupDto dto)
		{
			context.Database.SetCommandTimeout(TimeSpan.FromMinutes(15));
			using var tx = await context.Database.BeginTransactionAsync();

			await context.Database.ExecuteSqlRawAsync("EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';");
			await ClearAllAsync(context);

			foreach (var t in dto.tables.OrderBy(t => t.name, StringComparer.OrdinalIgnoreCase))
			{
				if (t.rows.Count == 0 || t.columns.Count == 0) continue;

				if (t.identity)
					await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [" + t.name + "] ON;");

				var columnList = string.Join("],[", t.columns);
				var placeholders = string.Join(",", Enumerable.Range(0, t.columns.Count).Select(i => $"@p{i}"));
				var insertSql = $"INSERT INTO [{t.name}] ([{columnList}]) VALUES ({placeholders})";

				foreach (var row in t.rows)
				{
					var parameters = new object[t.columns.Count];
					for (int i = 0; i < t.columns.Count; i++)
					{
						parameters[i] = Coerce(row.Count > i ? row[i] : null, t.types.Count > i ? t.types[i] : null)!;
					}
					await context.Database.ExecuteSqlRawAsync(insertSql, parameters);
				}

				if (t.identity)
					await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [" + t.name + "] OFF;");
			}

			await context.Database.ExecuteSqlRawAsync("EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';");
			await tx.CommitAsync();
		}

		public static async Task RestoreSqlAsync(AcademyAgainContext context, string script)
		{
			context.Database.SetCommandTimeout(TimeSpan.FromMinutes(15));
			using var tx = await context.Database.BeginTransactionAsync();

			await context.Database.ExecuteSqlRawAsync("EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';");
			await ClearAllAsync(context);

			var batches = System.Text.RegularExpressions.Regex.Split(script, @"(?m)^\s*GO\s*$");
			foreach (var batch in batches)
			{
				var sql = batch.Trim();
				if (sql.Length == 0 || sql.StartsWith("--")) continue;
				await context.Database.ExecuteSqlRawAsync(sql);
			}

			await context.Database.ExecuteSqlRawAsync("EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';");
			await tx.CommitAsync();
		}

		private static async Task ClearAllAsync(AcademyAgainContext context)
		{
			foreach (var table in EntityTables(context))
			{
				await context.Database.ExecuteSqlRawAsync("DELETE FROM [" + table + "];");
			}
		}

		private static object? Coerce(object? raw, string? typeName)
		{
			if (raw is null) return DBNull.Value;

			var target = typeName is null ? null : Type.GetType(typeName);
			if (raw is not System.Text.Json.JsonElement el)
			{
				return raw is DBNull ? DBNull.Value : raw;
			}

			if (el.ValueKind == System.Text.Json.JsonValueKind.Null) return DBNull.Value;

			if (target == null) return el.ToString();

			if (target == typeof(string)) return el.GetString();
			if (target == typeof(bool)) return el.GetBoolean();
			if (target == typeof(byte)) return el.GetByte();
			if (target == typeof(sbyte)) return el.GetSByte();
			if (target == typeof(short)) return el.GetInt16();
			if (target == typeof(ushort)) return el.GetUInt16();
			if (target == typeof(int)) return el.GetInt32();
			if (target == typeof(uint)) return el.GetUInt32();
			if (target == typeof(long)) return el.GetInt64();
			if (target == typeof(ulong)) return el.GetUInt64();
			if (target == typeof(decimal)) return el.GetDecimal();
			if (target == typeof(double)) return el.GetDouble();
			if (target == typeof(float)) return el.GetSingle();
			if (target == typeof(DateTime)) return el.GetDateTime();
			if (target == typeof(DateTimeOffset)) return el.GetDateTimeOffset();
			if (target == typeof(DateOnly)) return DateOnly.FromDateTime(el.GetDateTime());
			if (target == typeof(TimeOnly)) return TimeOnly.FromTimeSpan(TimeSpan.Parse(el.GetString() ?? ""));
			if (target == typeof(TimeSpan)) return TimeSpan.Parse(el.GetString() ?? "");
			if (target == typeof(Guid)) return el.GetGuid();
			if (target == typeof(byte[])) return el.GetBytesFromBase64();
			if (target.IsEnum) return Enum.Parse(target, el.GetString() ?? "");

			return el.ToString();
		}
	}
}