namespace CapstoneBackend.Core.Repositories;

using Models;
using Dapper.Contrib.Extensions;
using Dapper;
using MySqlConnector;
using CapstoneBackend.Utilities;
using System.Text.Json;
using System.Data;

public class FactTagsRepository(IConfiguration configuration)
{
    private readonly string _connectionString =
        configuration.GetValue<string>(EnvironmentVariables.MYSQL_CONNECTION_STRING) ??
        throw new InvalidOperationException("Connection string failed.");



    //Create
    public FactTags CreateEntry(FactTags factTags)
    {
        using var connection = new MySqlConnection(_connectionString);

        // Use stored procedure instead of raw SQL
        var jsonTags = JsonSerializer.Serialize(factTags.AvailableTags ?? new List<string>());

        var parameters = new DynamicParameters();
        parameters.Add("p_genre_id", factTags.GenreId);
        parameters.Add("p_available_tags", jsonTags);

        var result = connection.QuerySingle<dynamic>(
            "sp_FactTags_Create",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        factTags.Id = (int)result.NewId;
        return factTags;
    }

    // Update
    public FactTags UpdateEntry(FactTags factTags)
    {
        using var connection = new MySqlConnection(_connectionString);

        // Use stored procedure instead of raw SQL
        var jsonTags = JsonSerializer.Serialize(factTags.AvailableTags ?? new List<string>());

        var parameters = new DynamicParameters();
        parameters.Add("p_id", factTags.Id);
        parameters.Add("p_genre_id", factTags.GenreId);
        parameters.Add("p_available_tags", jsonTags);

        var rowsAffected = connection.Execute(
            "sp_FactTags_Update",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return rowsAffected > 0 ? factTags : throw new Exception("Update failed.");
    }

    //Get
    public FactTags? GetEntryById(int id)
    {
        using var connection = new MySqlConnection(_connectionString);


        var sql = "SELECT Id, GenreId, AvailableTags AS AvailableTagsJson, CreatedAt, UpdatedAt FROM fact_tags WHERE Id = @Id";
        var result = connection.QueryFirstOrDefault<FactTagsDto>(sql, new { Id = id });

        if (result == null) return null;

        return new FactTags
        {
            Id = result.Id,
            GenreId = result.GenreId,
            AvailableTags = DeserializeTags(result.AvailableTagsJson),
            CreatedAt = result.CreatedAt,
            UpdatedAt = result.UpdatedAt
        };

    }

    /* Get all (for debug) */
    public IEnumerable<FactTags> GetAll()
    {
        using var connection = new MySqlConnection(_connectionString);

  
        var sql = "SELECT Id, GenreId, AvailableTags AS AvailableTagsJson, CreatedAt, UpdatedAt FROM fact_tags";
        var results = connection.Query<FactTagsDto>(sql);

        return results.Select(r => new FactTags
        {
            Id = r.Id,
            GenreId = r.GenreId,
            AvailableTags = DeserializeTags(r.AvailableTagsJson),
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
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
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
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

    // Get fact tags with genre information using stored procedure
    public FactTagsWithGenre? GetFactTagsWithGenre(int genreId)
    {
        using var connection = new MySqlConnection(_connectionString);

        var parameters = new DynamicParameters();
        parameters.Add("p_genre_id", genreId);

        return connection.QueryFirstOrDefault<FactTagsWithGenre>(
            "sp_GetFactTagsWithGenre",
            parameters,
            commandType: CommandType.StoredProcedure
        );
    }

}



    
