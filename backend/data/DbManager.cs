using Microsoft.Data.SqlClient;
using System.Text.Json;
using DotNetEnv;

namespace Data
{
    /// <summary>
    /// Classe responsable de la gestion des opérations de base de données.
    /// </summary>
    public class DbManager
    {
        private readonly string _connectionString;

        /// <summary>
        /// Constructeur par défaut. Charge la chaîne de connexion depuis les variables d'environnement.
        /// </summary>
        public DbManager()
        {
            // Charger les variables d'environnement depuis le fichier .env
            DotNetEnv.Env.Load();

            // Récupérer la chaîne de connexion
            _connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                                ?? throw new InvalidOperationException("La chaîne de connexion est manquante dans les variables d'environnement.");
        }

        /// <summary>
        /// Constructeur avec chaîne de connexion personnalisée.
        /// </summary>
        /// <param name="connectionString">Chaîne de connexion.</param>
        public DbManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Crée une table en fonction d'une structure définie dans un fichier JSON.
        /// </summary>
        /// <param name="json">JSON contenant les informations de la table.</param>
        /// <returns>True si la table a été créée, false si elle existe déjà.</returns>
        public bool CreateTable(string json)
        {
            try
            {
                // Désérialisation du JSON
                var tableData = JsonSerializer.Deserialize<Dictionary<string, object>>(json)
                               ?? throw new ArgumentException("Le JSON fourni est invalide ou vide.");

                if (!tableData.ContainsKey("TableName") || !tableData.ContainsKey("Columns"))
                    throw new ArgumentException("Le JSON doit contenir 'TableName' et 'Columns'.");

                string tableName = (tableData["TableName"]?.ToString() ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(tableName))
                    throw new ArgumentException($"Le nom de la table est invalide.");

                // Désérialisation des colonnes
                var columns = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(tableData["Columns"]?.ToString() ?? string.Empty)
                             ?? throw new ArgumentException("Le JSON doit contenir une liste valide de colonnes sous 'Columns'.");

                if (columns.Count == 0)
                    throw new ArgumentException("Le JSON doit contenir au moins une colonne sous 'Columns'.");

                // Vérification de l'existence de la table
                if (IsTablePresent(tableName))
                {
                    Console.WriteLine($"La table '{tableName}' existe déjà.");
                    return false;
                }

                // Construction de la requête CREATE TABLE
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

                // Exécution de la requête
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

        /// <summary>
        /// Vérifie si une table est déjà présente dans la base de données.
        /// </summary>
        /// <param name="tableName">Nom de la table.</param>
        /// <returns>True si la table existe, false sinon.</returns>
        private bool IsTablePresent(string tableName)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la vérification de la table : {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Valide le type SQL des colonnes.
        /// </summary>
        /// <param name="sqlType">Type SQL.</param>
        /// <returns>True si le type est valide, false sinon.</returns>
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
