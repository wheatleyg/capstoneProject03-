namespace CapstoneBackend.Core.Repositories;

using Models;
using Dapper.Contrib.Extensions;
using MySqlConnector;


public class MediaRepository(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection String Failed.");
   
    //Create
    public Media CreateEntry(Media media)
    {
        using var connection = new MySqlConnection(_connectionString);


        var id = connection.Insert(media);
        media.Id = (int)id;
        return media;
    }
    // Update
    public Media UpdateEntry(Media media)
    {
        using var connection = new MySqlConnection(_connectionString);
        return connection.Update(media) ? media : throw new Exception("Update failed."); //Using this shortened way as Rider prefers it.
    }
    //Get
    public Media GetEntryById(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        return connection.Get<Media>(id) ?? throw new Exception("No entry found."); //Same as above.
        
    }
    /* Get all (for debug) */
    public IEnumerable<Media> GetAll()
    {
        using var connection = new MySqlConnection(_connectionString);
        return connection.GetAll<Media>();
    }
    
    
    //Delete
    public bool DeleteEntryById(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        return connection.Delete(new Media { Id = id, Link = string.Empty, MediaType = MediaType.None  });
    }



    
    
    
}