using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;

namespace BlazorApp1.Session;

public static class LoginDB
{
    public static string DBFolder => Path.Combine(AppContext.BaseDirectory, "Login");
    public static string DBPath => Path.Combine(DBFolder, "Users.db");
    const string UserTableName = "users";

    public static SQLiteConnection Connection { get; }

    static LoginDB()
    {
        Directory.CreateDirectory(DBFolder);

        string connectionString = $"Data Source={DBPath};Version=3;";
        Connection = new SQLiteConnection(connectionString);
        Connection.Open();

        using var cmd = Connection.CreateCommand();
        cmd.CommandText = $"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=@TableName";
        cmd.Parameters.AddWithValue("@TableName", UserTableName);

        bool tableExists = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        if (!tableExists)
        {
            cmd.CommandText = $@"CREATE TABLE {UserTableName} (
                Username TEXT PRIMARY KEY,
                Password TEXT NOT NULL,
                DataID INTEGER NOT NULL
            )";
            cmd.Parameters.Clear();
            cmd.ExecuteNonQuery();

            cmd.CommandText = $@"CREATE INDEX idx_{UserTableName}_DataID ON {UserTableName}(DataID);";
            cmd.ExecuteNonQuery();
        }
    }

    public static void Init() { }

    public static bool HasUser(string username)
    {
        using var cmd = new SQLiteCommand($"SELECT COUNT(*) FROM {UserTableName} WHERE Username = @username", Connection);
        cmd.Parameters.AddWithValue("@username", username);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public static int AddUser(string username, string password)
    {
        if (HasUser(username)) return 0;

        string hash = HashPassword(password);
        using var cmd = new SQLiteCommand(
            $"INSERT INTO {UserTableName} (Username, Password, DataID) " +
            $"VALUES (@username, @password, @DataID)", Connection);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.Parameters.AddWithValue("@password", hash);
        cmd.Parameters.AddWithValue("@DataID", GetNextID());
        return cmd.ExecuteNonQuery();
    }

    public static bool TryLogin(string username, string password)
    {
        string hash = HashPassword(password);
        using var cmd = new SQLiteCommand(
            $"SELECT COUNT(*) FROM {UserTableName} WHERE Username = @username AND Password = @password", Connection);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.Parameters.AddWithValue("@password", hash);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }
    public static long? GetDataID(string username)
    {
        if (!HasUser(username)) return null;
        using var cmd = new SQLiteCommand($"SELECT DataID FROM {UserTableName} WHERE Username = @username", Connection);
        cmd.Parameters.AddWithValue("@username", username);
        var result = cmd.ExecuteScalar();
        if (result is DBNull || result is null) return null;
        return Convert.ToInt64(result);
    }

    private static string HashPassword(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
    private static long GetNextID()
    {
        using var cmd = new SQLiteCommand($"SELECT MAX(DataID) FROM {UserTableName}", Connection);
        var result = cmd.ExecuteScalar();
        if (result is DBNull || result is null) result = 1000;
        var maxID = Convert.ToInt64(result);
        return maxID + Random.Shared.NextInt64(100, 200);
    }
}
