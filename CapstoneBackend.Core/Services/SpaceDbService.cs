using CapstoneBackend.Core.Models;
using CapstoneBackend.Core.Repositories;

namespace CapstoneBackend.Core.Services;

public class SpaceDbService
{
    private readonly SpaceDbRepository _spaceDbRepository;

    public SpaceDbService(SpaceDbRepository spaceDbRepository)
    {
        _spaceDbRepository = spaceDbRepository;
    }



    public SpaceDb CreateEntry(SpaceDb spaceDb)
    {
        if (spaceDb.CreatedAt == null)
        {
            spaceDb.CreatedAt = DateTime.Now;
        }

        if (spaceDb.Id != 0)
        {
            throw new ArgumentException("Id must be 0 for new entries", nameof(spaceDb.Id));
        }

        return _spaceDbRepository.CreateEntry(spaceDb);
    }

    public SpaceDb UpdateEntry(SpaceDb spaceDb)
    {
        if (spaceDb.Id <= 0) throw new ArgumentOutOfRangeException(nameof(spaceDb.Id), "Id must be greater than 0.");
        var _ = _spaceDbRepository.GetEntryById(spaceDb.Id) ??
                throw new KeyNotFoundException($"No entry found with 'Id': {spaceDb.Id}.");
        return _spaceDbRepository.UpdateEntry(spaceDb);
    }

    public SpaceDb GetEntryById(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than 0.");
        }
        var result = _spaceDbRepository.GetEntryById(id) ?? throw new KeyNotFoundException($"No entry found with 'id' {id}.");
        return result;
    }

    public IEnumerable<SpaceDb> GetAll()
    {
        var result = _spaceDbRepository.GetAll();
        return result;
    }

    public bool DeleteEntryById(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than 0.");
        }
        var _ = _spaceDbRepository.GetEntryById(id) ?? throw new KeyNotFoundException("No Entry Found");
        return _spaceDbRepository.DeleteEntryById(id);
    }

    public IEnumerable<SpaceFactsWithDetails> GetSpaceFactsWithDetails(int genreId)
    {
        if (genreId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(genreId), "GenreId must be greater than 0.");
        }
        return _spaceDbRepository.GetSpaceFactsWithDetails(genreId);
    }
}