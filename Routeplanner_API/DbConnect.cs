using System;
using Npgsql;

public class DbConnect
{
    public void ReadUsers()
    {
        var connectionString = "Host=145.24.222.95;Port=8765;Username=dreamteam;Password=dreamteam;Database=postgres";

        try{
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                //var sql = "SELECT id, name, age FROM users";

                // using (var command = new NpgsqlCommand(sql, connection))
                // using (var reader = command.ExecuteReader())
                // {
                //     while (reader.Read())
                //     {
                //         var id = reader.GetInt32(0); // Column 0: id
                //         var name = reader.GetString(1); // Column 1: name
                //         var age = reader.GetInt32(2); // Column 2: age

                //         Console.WriteLine($"ID: {id}, Name: {name}, Age: {age}");
                //     }
                // }
            }
        }
        catch(Exception exception){
            Console.WriteLine(exception);
        }
        
    }
}
