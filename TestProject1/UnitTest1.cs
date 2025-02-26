using System.ComponentModel;
using DAL.Helpers;
using DAL.Interfaces;
using DAL.Models;
using Moq;

namespace TestProject1;

public class Tests
{
    
    private Mock<IEventRepository> _eventRepositoryMock;
    
    [SetUp]
    public void Setup()
    {
        _eventRepositoryMock = new Mock<IEventRepository>();
    }

    [Test]
    [DisplayName("Тестирования метода добавления")]
    public async Task TestingMethod_Add()
    {
        var newEvent = new Event { Id = 1, Title = "Test Event", Location = "Test Location", Description = "Test Description", EventDateTime = DateOnly.FromDateTime(DateTime.Now), Category = "Test Category", MaxParticipants = 10};

        await _eventRepositoryMock.Object.Add(newEvent);

        _eventRepositoryMock.Verify(repo => repo.Add(It.IsAny<Event>()), Times.Once);
    }

    [Test]
    [DisplayName("Тестирования метода на получения всех")]
    public async Task TestingMethod_GetAll()
    {
        var events = new List<Event>
        {
            new Event {Id = 1, Title = "Test Event1", Location = "Test Location1", Description = "Test Description1", EventDateTime = DateOnly.FromDateTime(DateTime.Now), Category = "Test Category1", MaxParticipants = 10},
            new Event {Id = 2, Title = "Test Event2", Location = "Test Location2", Description = "Test Description2", EventDateTime = DateOnly.FromDateTime(DateTime.Now), Category = "Test Category2", MaxParticipants = 30}
        };
        
        _eventRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<QueryHelper>())).ReturnsAsync(events);
        
        var result = await _eventRepositoryMock.Object.GetAll(new QueryHelper { Page = 1, PageSize = 10 });
        
        _eventRepositoryMock.Verify(repo => repo.GetAll(It.IsAny<QueryHelper>()), Times.Once);
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].Title, Is.EqualTo("Test Event1"));
        Assert.That(result[1].Title, Is.EqualTo("Test Event2"));
        Assert.That(result[0].MaxParticipants, Is.EqualTo(10));
        Assert.That(result[1].MaxParticipants, Is.EqualTo(30));
    }
}