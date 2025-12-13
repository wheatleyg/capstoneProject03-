namespace CapstoneBackend.Core.Repositories;

using Models;
using Dapper.Contrib.Extensions;
using Dapper;
using MySqlConnector;
using CapstoneBackend.Utilities;
using System.Text.Json;

public class FactTagsRepository(IConfiguration configuration)
{
    private readonly string _connectionString =
        configuration.GetValue<string>(EnvironmentVariables.MYSQL_CONNECTION_STRING) ??
        throw new InvalidOperationException("Connection string failed.");



    //Create
    public FactTags CreateEntry(FactTags factTags)
    {
        using var connection = new MySqlConnection(_connectionString);



        //must use raw sql cause dapper.contrib doesn't have anything for jsons
        var jsonTags = JsonSerializer.Serialize(factTags.AvailableTags ?? new List<string>());


        var sql =
            "INSERT INTO fact_tags (GenreId, AvailableTags) VALUES (@GenreId, @AvailableTags); SELECT LAST_INSERT_ID();";
        var id = connection.QuerySingle<int>(sql, new { GenreId = factTags.GenreId, AvailableTags = jsonTags });

        factTags.Id = id;
        return factTags;
    }

    // Update
    public FactTags UpdateEntry(FactTags factTags)
    {
        using var connection = new MySqlConnection(_connectionString);


        var jsonTags = JsonSerializer.Serialize(factTags.AvailableTags ?? new List<string>());


        var sql = "UPDATE fact_tags SET GenreId = @GenreId, AvailableTags = @AvailableTags WHERE Id = @Id";
        var rowsAffected = connection.Execute(sql,
            new { Id = factTags.Id, GenreId = factTags.GenreId, AvailableTags = jsonTags });

        return rowsAffected > 0 ? factTags : throw new Exception("Update failed.");
    }

    //Get
    public FactTags? GetEntryById(int id)
    {
        using var connection = new MySqlConnection(_connectionString);


        var sql = "SELECT Id, GenreId, AvailableTags AS AvailableTagsJson FROM fact_tags WHERE Id = @Id";
        var result = connection.QueryFirstOrDefault<FactTagsDto>(sql, new { Id = id });

        if (result == null) return null;

        return new FactTags
        {
            Id = result.Id,
            GenreId = result.GenreId,
            AvailableTags = DeserializeTags(result.AvailableTagsJson)
        };

    }

    /* Get all (for debug) */
    public IEnumerable<FactTags> GetAll()
    {
        using var connection = new MySqlConnection(_connectionString);

  
        var sql = "SELECT Id, GenreId, AvailableTags AS AvailableTagsJson FROM fact_tags";
        var results = connection.Query<FactTagsDto>(sql);

        return results.Select(r => new FactTags
        {
            Id = r.Id,
            GenreId = r.GenreId,
            AvailableTags = DeserializeTags(r.AvailableTagsJson)
        });
    }



    //Delete
    public bool DeleteEntryById(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        return connection.Delete(new FactTags { Id = id });
    }

   
    private class FactTagsDto
    {
        public int Id { get; set; }
        public int GenreId { get; set; }
        public string AvailableTagsJson { get; set; } = string.Empty;
    }



    // helper
    private List<string> DeserializeTags(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new List<string>();

        try
        {
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        catch
        {
            // If deserialization fails, return empty list
            return new List<string>();
        }
    }

}



    
