using CapstoneBackend.Core.Models;
using CapstoneBackend.Core.Repositories;
using CapstoneBackend.Core.Services;
using Moq;
using Xunit;
// MOQ DOESN'T SUPPORT MOCKING EXTENSION METHODS
namespace CapstoneBackend.Test.Services;

public class MediaServiceTests
{
    private readonly Mock<IMediaRepository> _mockRepository;
    private readonly MediaService _service;

    public MediaServiceTests()
    {
        _mockRepository = new Mock<IMediaRepository>();
        _service = new MediaService(_mockRepository.Object);
    }

    #region CreateEntry Tests

    [Fact]
    public void CreateEntry_WithValidMedia_ReturnsCreatedMedia()
    {
        // Arrange
        var inputMedia = new Media
        {
            Id = 0,
            Link = "https://example.com/image.jpg",
            MediaType = MediaType.Image
        };

        var expectedMedia = new Media
        {
            Id = 1,
            Link = "https://example.com/image.jpg",
            MediaType = MediaType.Image
        };

        _mockRepository.Setup(r => r.CreateEntry(It.IsAny<Media>()))
            .Returns(expectedMedia);

        // Act
        var result = _service.CreateEntry(inputMedia);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedMedia.Id, result.Id);
        Assert.Equal(expectedMedia.Link, result.Link);
        Assert.Equal(expectedMedia.MediaType, result.MediaType);
        _mockRepository.Verify(r => r.CreateEntry(inputMedia), Times.Once);
    }

    [Fact]
    public void CreateEntry_WithNonZeroId_ThrowsArgumentException()
    {
        // Arrange
        var invalidMedia = new Media
        {
            Id = 5,
            Link = "https://example.com/image.jpg",
            MediaType = MediaType.Image
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _service.CreateEntry(invalidMedia));
        Assert.Contains("Id must be 0", exception.Message);
        Assert.Equal("Id", exception.ParamName);
        _mockRepository.Verify(r => r.CreateEntry(It.IsAny<Media>()), Times.Never);
    }

    #endregion

    #region UpdateEntry Tests

    [Fact]
    public void UpdateEntry_WithValidMedia_ReturnsUpdatedMedia()
    {
        // Arrange
        var existingMedia = new Media
        {
            Id = 1,
            Link = "https://example.com/old.jpg",
            MediaType = MediaType.Image
        };

        var updatedMedia = new Media
        {
            Id = 1,
            Link = "https://example.com/new.jpg",
            MediaType = MediaType.Video
        };

        _mockRepository.Setup(r => r.GetEntryById(1))
            .Returns(existingMedia);
        _mockRepository.Setup(r => r.UpdateEntry(It.IsAny<Media>()))
            .Returns(updatedMedia);

        // Act
        var result = _service.UpdateEntry(updatedMedia);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedMedia.Id, result.Id);
        Assert.Equal(updatedMedia.Link, result.Link);
        Assert.Equal(updatedMedia.MediaType, result.MediaType);
        _mockRepository.Verify(r => r.GetEntryById(1), Times.Once);
        _mockRepository.Verify(r => r.UpdateEntry(updatedMedia), Times.Once);
    }

    [Fact]
    public void UpdateEntry_WithZeroId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var invalidMedia = new Media
        {
            Id = 0,
            Link = "https://example.com/image.jpg",
            MediaType = MediaType.Image
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _service.UpdateEntry(invalidMedia));
        Assert.Equal("Id", exception.ParamName);
        _mockRepository.Verify(r => r.GetEntryById(It.IsAny<int>()), Times.Never);
        _mockRepository.Verify(r => r.UpdateEntry(It.IsAny<Media>()), Times.Never);
    }

    [Fact]
    public void UpdateEntry_WithNegativeId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var invalidMedia = new Media
        {
            Id = -1,
            Link = "https://example.com/image.jpg",
            MediaType = MediaType.Image
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _service.UpdateEntry(invalidMedia));
        Assert.Equal("Id", exception.ParamName);
        _mockRepository.Verify(r => r.GetEntryById(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void UpdateEntry_WithNonExistentId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var media = new Media
        {
            Id = 999,
            Link = "https://example.com/image.jpg",
            MediaType = MediaType.Image
        };

        _mockRepository.Setup(r => r.GetEntryById(999))
            .Returns((Media?)null);

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => _service.UpdateEntry(media));
        Assert.Contains("999", exception.Message);
        _mockRepository.Verify(r => r.GetEntryById(999), Times.Once);
        _mockRepository.Verify(r => r.UpdateEntry(It.IsAny<Media>()), Times.Never);
    }

    #endregion

    #region GetEntryById Tests

    [Fact]
    public void GetEntryById_WithValidId_ReturnsMedia()
    {
        // Arrange
        var expectedMedia = new Media
        {
            Id = 1,
            Link = "https://example.com/image.jpg",
            MediaType = MediaType.Image
        };

        _mockRepository.Setup(r => r.GetEntryById(1))
            .Returns(expectedMedia);

        // Act
        var result = _service.GetEntryById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedMedia.Id, result.Id);
        Assert.Equal(expectedMedia.Link, result.Link);
        Assert.Equal(expectedMedia.MediaType, result.MediaType);
        _mockRepository.Verify(r => r.GetEntryById(1), Times.Once);
    }

    [Fact]
    public void GetEntryById_WithZeroId_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _service.GetEntryById(0));
        Assert.Equal("id", exception.ParamName);
        _mockRepository.Verify(r => r.GetEntryById(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void GetEntryById_WithNegativeId_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _service.GetEntryById(-1));
        Assert.Equal("id", exception.ParamName);
        _mockRepository.Verify(r => r.GetEntryById(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void GetEntryById_WithNonExistentId_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetEntryById(999))
            .Returns((Media?)null);

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => _service.GetEntryById(999));
        Assert.Contains("999", exception.Message);
        _mockRepository.Verify(r => r.GetEntryById(999), Times.Once);
    }

    #endregion

    #region GetAll Tests

    [Fact]
    public void GetAll_ReturnsAllMedia()
    {
        // Arrange
        var expectedMedia = new List<Media>
        {
            new Media { Id = 1, Link = "https://example.com/image1.jpg", MediaType = MediaType.Image },
            new Media { Id = 2, Link = "https://example.com/video1.mp4", MediaType = MediaType.Video },
            new Media { Id = 3, Link = "https://example.com/audio1.mp3", MediaType = MediaType.Audio }
        };

        _mockRepository.Setup(r => r.GetAll())
            .Returns(expectedMedia);

        // Act
        var result = _service.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        _mockRepository.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public void GetAll_WhenNoMedia_ReturnsEmptyCollection()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAll())
            .Returns(Enumerable.Empty<Media>());

        // Act
        var result = _service.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockRepository.Verify(r => r.GetAll(), Times.Once);
    }

    #endregion

    #region DeleteEntryById Tests

    [Fact]
    public void DeleteEntryById_WithValidId_ReturnsTrue()
    {
        // Arrange
        var existingMedia = new Media
        {
            Id = 1,
            Link = "https://example.com/image.jpg",
            MediaType = MediaType.Image
        };

        _mockRepository.Setup(r => r.GetEntryById(1))
            .Returns(existingMedia);
        _mockRepository.Setup(r => r.DeleteEntryById(1))
            .Returns(true);

        // Act
        var result = _service.DeleteEntryById(1);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.GetEntryById(1), Times.Once);
        _mockRepository.Verify(r => r.DeleteEntryById(1), Times.Once);
    }

    [Fact]
    public void DeleteEntryById_WithZeroId_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _service.DeleteEntryById(0));
        Assert.Equal("id", exception.ParamName);
        _mockRepository.Verify(r => r.GetEntryById(It.IsAny<int>()), Times.Never);
        _mockRepository.Verify(r => r.DeleteEntryById(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void DeleteEntryById_WithNegativeId_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _service.DeleteEntryById(-1));
        Assert.Equal("id", exception.ParamName);
        _mockRepository.Verify(r => r.GetEntryById(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void DeleteEntryById_WithNonExistentId_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetEntryById(999))
            .Returns((Media?)null);

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => _service.DeleteEntryById(999));
        Assert.Contains("No Entry Found", exception.Message);
        _mockRepository.Verify(r => r.GetEntryById(999), Times.Once);
        _mockRepository.Verify(r => r.DeleteEntryById(It.IsAny<int>()), Times.Never);
    }

    #endregion
}
