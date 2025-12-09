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
        return _mainRepository.CreateEntry(main);
    }

    public Main UpdateEntry(Main main)
    {
        if(main.Id == 0) throw new Exception("Can't be 0!");
        _ = _mainRepository.GetEntryById(main.Id) ?? throw new Exception("No entry found.");
        return _mainRepository.UpdateEntry(main);
    }

    public Main GetEntryById(int id)
    {
        var result = _mainRepository.GetEntryById(id) ?? throw new Exception("No entry found.");
        return result;
    }

    public IEnumerable<Main> GetAll()
    {
        var result = _mainRepository.GetAll();
        return result;
    }

    public bool DeleteEntryById(int id)
    {
        _ = _mainRepository.GetEntryById(id) ?? throw new Exception("No Entry Found");
        return _mainRepository.DeleteEntryById(id);
    }
    
    
    
    
}