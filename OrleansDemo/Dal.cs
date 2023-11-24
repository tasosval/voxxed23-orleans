using Microsoft.Data.Sqlite;

public class Dal
{
    public static void SetupDb()
    {
        using var connection = new SqliteConnection("Data Source=demo.db");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "create table if not exists t1 (id integer primary key, counter integer default 0)";
        command.ExecuteNonQuery();

        var command2 = connection.CreateCommand();
        command2.CommandText = "delete from t1";
        command2.ExecuteNonQuery();

        var command3 = connection.CreateCommand();
        command3.CommandText = "insert into t1 (id, counter) values (1, 0)";
        command3.ExecuteNonQuery();
    }

    public static async Task UpdateRow()
    {
        using var connection = new SqliteConnection("Data Source=demo.db");
        connection.Open();
        
        var command = connection.CreateCommand();
        command.CommandText = "update t1 set counter = counter + 1 where id = 1";
        await command.ExecuteNonQueryAsync();
    }

    public static async Task<long> GetRow()
    {
        using var connection = new SqliteConnection("Data Source=demo.db");
        connection.Open();
        
        var command = connection.CreateCommand();
        command.CommandText = "select counter from t1";
        var result = await command.ExecuteScalarAsync();
        return result is null ? -1 : (long)result; 
    }
}