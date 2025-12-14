using CapstoneBackend.Core.Models;

namespace CapstoneBackend.Core.Repositories;

public interface IMediaRepository
{
    Media CreateEntry(Media media);
    Media UpdateEntry(Media media);
    Media? GetEntryById(int id);
    IEnumerable<Media> GetAll();
    bool DeleteEntryById(int id);
}
