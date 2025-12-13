using CapstoneBackend.Core.Models;
using CapstoneBackend.Core.Repositories;

namespace CapstoneBackend.Core.Services;

public class MediaService
{
    private readonly MediaRepository _mediaRepository;

    public MediaService(MediaRepository mediaRepository)
    {
        _mediaRepository = mediaRepository;
    }

    public Media CreateEntry(Media media)
    {
        if (media.Id != 0)
        {
            throw new ArgumentException("Id must be 0 for new entires.", nameof(media.Id));
        }
        
        return _mediaRepository.CreateEntry(media);
    }

    public Media UpdateEntry(Media media)
    {
        if (media.Id <= 0) throw new ArgumentOutOfRangeException(nameof(media.Id), "Id must be greater than 0.");
        var _ = _mediaRepository.GetEntryById(media.Id) ??
                            throw new KeyNotFoundException($"No entry found with 'Id': {media.Id}.");
        return _mediaRepository.UpdateEntry(media);
    }
    
    public Media GetEntryById(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than 0.");
        }
        var result = _mediaRepository.GetEntryById(id) ?? throw new KeyNotFoundException($"No entry found with 'id' {id}.");
        return result;
    }

    public IEnumerable<Media> GetAll()
    {
        var result = _mediaRepository.GetAll();
        return result;
    }

    public bool DeleteEntryById(int id)
    {
        if(id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than 0.");
        var _ = _mediaRepository.GetEntryById(id) ?? throw new KeyNotFoundException("No Entry Found");
        return _mediaRepository.DeleteEntryById(id);
    }
    
}