namespace CapstoneBackend.Core.Services;

using Repositories;
using Models;

public class MainService

// 
{
    private readonly MainRepository _mainRepository;

    public MainService(MainRepository mainRepository)
    {
        _mainRepository = mainRepository;
    }


    public Main CreateEntry(Main main)
    {
        if (main.Id != 0)
        {
            throw new ArgumentException("Id must be 0 for new entries.", nameof(main.Id));
        }
        return _mainRepository.CreateEntry(main);
    }

    public Main UpdateEntry(Main main)
    {
        
        if(main.Id <= 0) throw new ArgumentOutOfRangeException(nameof(main.Id), "Id must be greater than 0.");
        var _ = _mainRepository.GetEntryById(main.Id) ?? 
                throw new KeyNotFoundException($"No entry found with 'Id': '{main.Id}'.");
        return _mainRepository.UpdateEntry(main);
    }

    public Main GetEntryById(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than 0.");
        }
        var result = _mainRepository.GetEntryById(id);
        return result;
    }

    public IEnumerable<Main> GetAll()
    {
        var result = _mainRepository.GetAll();
        return result;
    }

    public bool DeleteEntryById(int id)
    {
        if(id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than 0.");
        _ = _mainRepository.GetEntryById(id) ?? throw new KeyNotFoundException("No Entry Found");
        return _mainRepository.DeleteEntryById(id);
    }
    
    
    
    
}