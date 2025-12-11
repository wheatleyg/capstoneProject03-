using CapstoneBackend.Core.Models;
using CapstoneBackend.Core.Repositories;

namespace CapstoneBackend.Core.Services;

public class FactTagsService
{
    private readonly FactTagsRepository _factTagsRepository;

    public FactTagsService(FactTagsRepository factTagsRepository)
    {
        _factTagsRepository = factTagsRepository;
    }

    public FactTags CreateEntry(FactTags factTags)
    {
        if (factTags.Id != 0)
        {
            throw new ArgumentException("Id must be 0 for new entries.", nameof(factTags.Id));
        }
        return _factTagsRepository.CreateEntry(factTags);
    }

    public FactTags UpdateEntry(FactTags factTags)
    {
        if (factTags.Id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(factTags.Id), "Id must be greater than 0.");
        }
        var _ = _factTagsRepository.GetEntryById(factTags.Id) ??
                throw new KeyNotFoundException($"No entry found with 'Id': {factTags.Id}.");
        return _factTagsRepository.UpdateEntry(factTags);
    }

    public FactTags GetEntryById(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than 0.");
        }
        var result = _factTagsRepository.GetEntryById(id) ?? throw new KeyNotFoundException($"No entry found with 'id' {id}.");
        return result;
    }

    public IEnumerable<FactTags> GetAll()
    {
        var result = _factTagsRepository.GetAll();
        return result;
    }

    public bool DeleteEntryById(int id)
    {
        if(id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than 0.");
        var _ = _factTagsRepository.GetEntryById(id) ?? throw new KeyNotFoundException("No Entry Found");
        return _factTagsRepository.DeleteEntryById(id);
    }
    
}