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

        return _catDbRepository.CreateEntry(catDb);
    }

    public CatDb UpdateEntry(CatDb catDb)
    {
        if (catDb.Id == 0) throw new Exception("Can't be 0!");
        var existingEntry = _catDbRepository.GetEntryById(catDb.Id) ?? throw new Exception("No entry found.");
        return _catDbRepository.UpdateEntry(catDb);
    }

    public CatDb GetEntryById(int id)
    {
        var result = _catDbRepository.GetEntryById(id) ?? throw new Exception("No entry found.");
        return result;
    }

    public IEnumerable<CatDb> GetAll()
    {
        var result = _catDbRepository.GetAll();
        return result;
    }

    public bool DeleteEntryById(int id)
    {
        var entry = _catDbRepository.GetEntryById(id) ?? throw new Exception("No entry found.");
        return _catDbRepository.DeleteEntryById(id);
    }
} 