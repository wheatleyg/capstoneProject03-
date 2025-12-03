namespace CapstoneBackend.Core.Repositories;

using Models;
using Dapper.Contrib.Extensions;
using MySqlConnector;

public class CatDbRepository(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection") ??
                                                throw new InvalidOperationException("Connection string failed.");
    
    
    //Create
    public CatDb CreateEntry(CatDb catDb)
    {
        using var connection = new MySqlConnection(_connectionString);


        var id = connection.Insert(catDb);
        catDb.Id = (int)id;
        return catDb;
    }
    // Update
    public CatDb UpdateEntry(CatDb catDb)
    {
        using var connection = new MySqlConnection(_connectionString);
        return connection.Update(catDb) ? catDb : throw new Exception("Update failed."); //Using this shortened way as Rider prefers it.
    }
    //Get
    public CatDb GetEntryById(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        return connection.Get<CatDb>(id) ?? throw new Exception("No entry found."); //Same as above.
        
    }
    /* Get all (for debug) */
    public IEnumerable<CatDb> GetAll()
    {
        using var connection = new MySqlConnection(_connectionString);
        return connection.GetAll<CatDb>();
    }
    
    
    //Delete
    public bool DeleteEntryById(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        return connection.Delete(new CatDb { Id = id, FactText = string.Empty });
    }

}