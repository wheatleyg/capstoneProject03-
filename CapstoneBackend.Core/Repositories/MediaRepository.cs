namespace CapstoneBackend.Core.Repositories;

using Models;
using Dapper;
using Dapper.Contrib.Extensions;
using MySqlConnector;
using CapstoneBackend.Utilities;
using System.Data;


public class MediaRepository(IConfiguration configuration) : IMediaRepository
{
    private readonly string _connectionString = configuration.GetValue<string>(EnvironmentVariables.MYSQL_CONNECTION_STRING) ??
                                                throw new InvalidOperationException("Connection string failed.");
   
    //Create
    public Media CreateEntry(Media media)
    {
        using var connection = new MySqlConnection(_connectionString);
        
        // Use stored procedure instead of raw SQL
        var mediaTypeString = media.MediaType.ToString().ToLowerInvariant();
        
        var parameters = new DynamicParameters();
        parameters.Add("p_media_type", mediaTypeString);
        parameters.Add("p_link", media.Link);

        var result = connection.QuerySingle<dynamic>(
            "sp_Media_Create",
            parameters,
            commandType: CommandType.StoredProcedure
        );
        
        media.Id = (int)result.NewId;
        return media;
    }
    // Update
    public Media UpdateEntry(Media media)
    {
        using var connection = new MySqlConnection(_connectionString);
        
        // Use stored procedure instead of raw SQL
        var mediaTypeString = media.MediaType.ToString().ToLowerInvariant();
        
        var parameters = new DynamicParameters();
        parameters.Add("p_id", media.Id);
        parameters.Add("p_media_type", mediaTypeString);
        parameters.Add("p_link", media.Link);

        var rowsAffected = connection.Execute(
            "sp_Media_Update",
            parameters,
            commandType: CommandType.StoredProcedure
        );
        
        return rowsAffected > 0 ? media : throw new Exception("Update failed.");
    }
    //Get
    public Media? GetEntryById(int id)
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