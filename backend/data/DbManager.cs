using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace Data
{
    public class DbManager
    {
        private readonly string _connectionString;

        public DbManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool CreateTable(string json)
        {
            try
            {
                var tableData = JsonSerializer.Deserialize<Dictionary<string, object>>(json)
                               ?? throw new ArgumentException("Le JSON fourni est invalide ou vide.");

                if (!tableData.ContainsKey("TableName") || !tableData.ContainsKey("Columns"))
                    throw new ArgumentException("Le JSON doit contenir 'TableName' et 'Columns'.");

                string tableName = (tableData["TableName"]?.ToString() ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(tableName))
                    throw new ArgumentException($"Le nom de la table est invalide.");

                var columns = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(tableData["Columns"]?.ToString() ?? string.Empty)
                             ?? throw new ArgumentException("Le JSON doit contenir une liste valide de colonnes sous 'Columns'.");

                if (columns.Count == 0)
                    throw new ArgumentException("Le JSON doit contenir au moins une colonne sous 'Columns'.");

                if (IsTablePresent(tableName))
                {
                    Console.WriteLine($"La table '{tableName}' existe déjà.");
                    return false;
                }

                var columnDefinitions = new List<string>();
                foreach (var col in columns)
                {
                    if (!col.ContainsKey("Name") || !col.ContainsKey("Type"))
                        throw new ArgumentException("Chaque colonne doit contenir les clés 'Name' et 'Type'.");

                    string columnName = (col["Name"]?.Trim() ?? string.Empty);
                    string columnType = (col["Type"]?.Trim() ?? string.Empty);

                    if (string.IsNullOrWhiteSpace(columnName))
                        throw new ArgumentException($"Le nom de la colonne est invalide.");

                    if (string.IsNullOrWhiteSpace(columnType) || !IsValidSqlType(columnType))
                        throw new ArgumentException($"Le type SQL '{columnType}' pour la colonne '{columnName}' est invalide.");

                    columnDefinitions.Add($"{columnName} {columnType}");
                }

                string createTableQuery = $"CREATE TABLE {tableName} ({string.Join(", ", columnDefinitions)})";

                using var connection = new SqlConnection(_connectionString);
                connection.Open();

                using var command = new SqlCommand(createTableQuery, connection);
                command.ExecuteNonQuery();

                Console.WriteLine($"La table '{tableName}' a été créée avec succès.");
                return true;
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Erreur SQL : {sqlEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur générale : {ex.Message}");
                throw;
            }
        }

        private bool IsTablePresent(string tableName)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            string query = @"
                SELECT CASE WHEN EXISTS (
                    SELECT 1 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_NAME = @TableName
                ) THEN 1 ELSE 0 END";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TableName", tableName);

            return Convert.ToBoolean(command.ExecuteScalar());
        }

        private bool IsValidSqlType(string sqlType)
        {
            var validTypes = new HashSet<string>
            {
                "INT",
                "BIGINT",
                "NVARCHAR(50)",
                "NVARCHAR(100)",
                "NVARCHAR(MAX)",
                "VARCHAR(50)",
                "VARCHAR(100)",
                "TEXT",
                "DATE",
                "DATETIME",
                "FLOAT",
                "DECIMAL(18,2)",
                "BIT",
                "SMALLINT"
            };

            return validTypes.Contains(sqlType.ToUpper());
        }
    }
}
