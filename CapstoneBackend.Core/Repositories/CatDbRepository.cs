namespace CapstoneBackend.Core.Repositories;
 
 
 using Dapper;
 using MySqlConnector;
 using System.Data;
 using Microsoft.Extensions.Configuration;
 using Dapper.Contrib.Extensions;
 using Models;
 
 public class CatDbRepository(IConfiguration configuration) : ICatDbRepository
 {
     private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
                                                 ?? throw new InvalidOperationException("no connection string found");

     // 1. Implement GetCatFactsWithGenreAsync (The Stored Procedure)
     public async Task<IEnumerable<dynamic>> GetCatFactsWithGenreAsync()
     {
         await using var connection = new MySqlConnection(_connectionString);
         return await connection.QueryAsync(
             "GetCatFactsWithGenre", // Name of your stored procedure
             commandType: CommandType.StoredProcedure
         );
     }
 
     // 2. Implement GetById (Using Dapper.Contrib or standard Dapper)
     public async Task<CatDb?> GetById(int id)
     {
         await using var connection = new MySqlConnection(_connectionString);
         // Dapper.Contrib's GetAsync<T> requires an ID. 
         // It finds the table based on the [Table("name")] attribute in your model.
         return await connection.GetAsync<CatDb>(id);
     }
 
     // 3. Implement GetAllAsync (Using Dapper.Contrib)
     public async Task<IEnumerable<CatDb>> GetAllAsync()
     {
         await using var connection = new MySqlConnection(_connectionString);
         // GetAllAsync<T> gets everything from the table mapped to T
         return await connection.GetAllAsync<CatDb>();
     }
 
     // 4. Implement GetByIncludedTextAsync (Using standard Dapper SQL)
     public async Task<IEnumerable<CatDb>> GetByIncludedTextAsync(string searchText)
     {
         await using var connection = new MySqlConnection(_connectionString);
         
         // We write raw SQL here because Dapper.Contrib doesn't do "WHERE LIKE" clauses
         var sql = "SELECT * FROM fun_facts_db.`cat_db` WHERE fact_text LIKE @SearchPattern";
         
         return await connection.QueryAsync<CatDb>(
             sql, 
             new { SearchPattern = $"%{searchText}%" } // Pass parameters safely
         );
     }
 }