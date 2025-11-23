namespace CapstoneBackend.Core.Repositories;
using CapstoneBackend.Core.Models;

//TODO I will research if this is needed. If it is, finish it. Otherwise, remove.
//Apparently this will let me fix code a lot easier if I realize  I did something really stupid (which happens a lot)



public interface ICatDbRepository
{
    Task<IEnumerable<dynamic>> GetCatFactsWithGenreAsync();
    Task<CatDb?> GetById(int id);
    
    Task<IEnumerable<CatDb>> GetAllAsync();

    Task<IEnumerable<CatDb>> GetByIncludedTextAsync(string searchText);
}