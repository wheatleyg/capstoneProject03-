namespace CapstoneBackend.Core.Repositories;

using Models;
using Dapper.Contrib.Extensions;
using MySqlConnector;
using CapstoneBackend.Utilities;

public class MainRepository(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetValue<string>(EnvironmentVariables.MYSQL_CONNECTION_STRING) ??
                                                throw new InvalidOperationException("Connection string failed.");
    
    //Create
    public Main CreateEntry(Main main)
    {
        using var connection = new MySqlConnection(_connectionString);


        var id = connection.Insert(main);
        main.Id = (int)id;
        return main;
    }
    // Update
    public Main UpdateEntry(Main main)
    {
        using var connection = new MySqlConnection(_connectionString);
        return connection.Update(main) ? main : throw new Exception("Update failed."); //Using this shortened way as Rider prefers it.
    }
    //Get
    public Main GetEntryById(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        return connection.Get<Main>(id) ?? throw new Exception("No entry found."); //Same as above.
        
    }
    /* Get all (for debug) */
    public IEnumerable<Main> GetAll()
    {
        using var connection = new MySqlConnection(_connectionString);
        return connection.GetAll<Main>();
    }
    
    
    //Delete
    public bool DeleteEntryById(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        return connection.Delete(new Main { Id = id, GenreName = string.Empty, Description = string.Empty, TableName = string.Empty });
    }
    
    
    
}