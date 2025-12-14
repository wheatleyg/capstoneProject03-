namespace CapstoneBackend.Core.Repositories;

using Models;
using Dapper;
using Dapper.Contrib.Extensions;
using MySqlConnector;
using CapstoneBackend.Utilities;


public class MediaRepository(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetValue<string>(EnvironmentVariables.MYSQL_CONNECTION_STRING) ??
                                                throw new InvalidOperationException("Connection string failed.");
   
    //Create
    public Media CreateEntry(Media media)
    {
        using var connection = new MySqlConnection(_connectionString);
        
        var mediaTypeString = media.MediaType.ToString().ToLowerInvariant();
        
        var sql = "INSERT INTO media (MediaType, Link) VALUES (@MediaType, @Link); SELECT LAST_INSERT_ID();";
        var id = connection.QuerySingle<int>(sql, new { MediaType = mediaTypeString, Link = media.Link });
        
        media.Id = id;
        return media;
    }
    // Update
    public Media UpdateEntry(Media media)
    {
        using var connection = new MySqlConnection(_connectionString);
        
        var mediaTypeString = media.MediaType.ToString().ToLowerInvariant();
        
        var sql = "UPDATE media SET MediaType = @MediaType, Link = @Link WHERE Id = @Id";
        var rowsAffected = connection.Execute(sql, new { Id = media.Id, MediaType = mediaTypeString, Link = media.Link });
        
        return rowsAffected > 0 ? media : throw new Exception("Update failed.");
    }
    //Get
    public Media GetEntryById(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        return connection.Get<Media>(id); //Same as above.
        
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