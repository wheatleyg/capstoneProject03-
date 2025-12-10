using CapstoneBackend.Core.Repositories;
using CapstoneBackend.Core.Models;

namespace CapstoneBackend.Core.Services;

public class CatDbService
{
    private readonly CatDbRepository _catDbRepository;

    public CatDbService(CatDbRepository catDbRepository)
    {
        _catDbRepository = catDbRepository;
    }

    public CatDb CreateEntry(CatDb catDb)
    {
        if (catDb.CreatedAt == null)
        {
            catDb.CreatedAt = DateTime.Now;
        }

        if (catDb.Id != 0)
        {
            throw new ArgumentException("Id must be 0 for new entries." , nameof(catDb.Id)); 
        }

        return _catDbRepository.CreateEntry(catDb);
    }

    public CatDb UpdateEntry(CatDb catDb)
    {
        if (catDb.Id <= 0) throw new ArgumentOutOfRangeException(nameof(catDb.Id), "Id must be greater than 0.");
        var _ = _catDbRepository.GetEntryById(catDb.Id) ??
                            throw new KeyNotFoundException($"No entry found with 'Id': {catDb.Id}.");
        return _catDbRepository.UpdateEntry(catDb);
    }

    public CatDb GetEntryById(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than 0.");
        }
        var result = _catDbRepository.GetEntryById(id) ?? throw new KeyNotFoundException($"No entry found with 'id' {id}.");
        return result; //ts is tuff   
    }

    public IEnumerable<CatDb> GetAll()
    {
        var result = _catDbRepository.GetAll(); //?? Enumerable.Empty<CatDb>();
        return result;
    }

    public bool DeleteEntryById(int id)
    {
        if(id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than 0.");
        var _ = _catDbRepository.GetEntryById(id) ?? throw new KeyNotFoundException("No entry found.");
        return _catDbRepository.DeleteEntryById(id);
    }
} 