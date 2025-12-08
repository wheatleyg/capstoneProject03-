namespace CapstoneBackend.Core.Repositories;

using Models;
using Dapper.Contrib.Extensions;
using MySqlConnector;

public class FactTagsRepository(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string failed.");
    
    
    
    //Create
    public FactTags CreateEntry(FactTags factTags)
    {
        using var connection = new MySqlConnection(_connectionString);


        var id = connection.Insert(factTags);
        factTags.Id = (int)id;
        return factTags;
    }
    // Update
    public FactTags UpdateEntry(FactTags factTags)
    {
        using var connection = new MySqlConnection(_connectionString);
        return connection.Update(factTags) ? factTags : throw new Exception("Update failed."); //Using this shortened way as Rider prefers it.
    }
    //Get
    public FactTags GetEntryById(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        return connection.Get<FactTags>(id) ?? throw new Exception("No entry found."); //Same as above.
        
    }
    /* Get all (for debug) */
    public IEnumerable<FactTags> GetAll()
    {
        using var connection = new MySqlConnection(_connectionString);
        return connection.GetAll<FactTags>();
    }
    
    
    //Delete
    public bool DeleteEntryById(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        return connection.Delete(new FactTags { Id = id});
    }



    
}