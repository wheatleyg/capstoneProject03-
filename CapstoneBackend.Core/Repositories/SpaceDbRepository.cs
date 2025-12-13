namespace CapstoneBackend.Core.Repositories;

using Models;
using Dapper.Contrib.Extensions;
using MySqlConnector;
using CapstoneBackend.Utilities;


public class SpaceDbRepository(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetValue<string>(EnvironmentVariables.MYSQL_CONNECTION_STRING) ??
                                                throw new InvalidOperationException("Connection string failed.");

    //Create
    public SpaceDb CreateEntry(SpaceDb spaceDb)
    {
        using var connection = new MySqlConnection(_connectionString);

        var id = connection.Insert(spaceDb);
        spaceDb.Id = (int)id;
        return spaceDb;

    }
    
    //Update
    public SpaceDb UpdateEntry(SpaceDb spaceDb)
    {
        using var connection = new MySqlConnection(_connectionString);

        return connection.Update(spaceDb) ? spaceDb : throw new Exception("Update failed.");
        
    }
    //Get
    public SpaceDb GetEntryById(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        return connection.Get<SpaceDb>(id) ?? throw new Exception("No entry found.");
    }

    public IEnumerable<SpaceDb> GetAll()
    {
        using var connection = new MySqlConnection(_connectionString);
        return connection.GetAll<SpaceDb>();
    }
    //Delete
    public bool DeleteEntryById(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        return connection.Delete(new SpaceDb { Id = id, FactText = string.Empty }); //I want to find out how to add error detection to this.
    }
    
}


