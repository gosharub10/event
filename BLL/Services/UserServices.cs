using BLL.DTO;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;
using MapsterMapper;

namespace BLL.Services;

internal class UserServices : IUserServices
{
    private readonly IUserRepository _userRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IEventParticipantsRepository _eventParticipantsRepository;
    private readonly IMapper _mapper;

    public UserServices(
        IUserRepository userRepository,
        IEventRepository eventRepository,
        IEventParticipantsRepository eventParticipantsRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _eventRepository = eventRepository;
        _eventParticipantsRepository = eventParticipantsRepository;
        _mapper = mapper;
    }

    public async Task<UserDTO> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            throw new OperationCanceledException("Operation was cancelled.");
        
        var user = await _userRepository.GetById(id, cancellationToken) 
            ?? throw new KeyNotFoundException($"No user found with ID {id}");
        
        return _mapper.Map<UserDTO>(user);
    }

    public async Task<List<UserDTO>> GetByEventName(string eventName, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            throw new OperationCanceledException("Operation was cancelled.");

        if (string.IsNullOrWhiteSpace(eventName))
            throw new ArgumentException("Event name cannot be null or empty", nameof(eventName));

        var eventEntity = await _eventRepository.GetEventByName(eventName, cancellationToken);
    
        if (eventEntity == null)
            throw new KeyNotFoundException($"Event with name '{eventName}' not found");

        var eventParticipants = await _eventParticipantsRepository.GetAll(cancellationToken);
        var foundEventParticipants = eventParticipants
            .Where(ep => ep.EventId == eventEntity.Id)
            .ToList();
        
        if (foundEventParticipants.Count == 0)
            throw new KeyNotFoundException($"No users found for event: {eventName}");

        var usersEvent = foundEventParticipants
            .Select(ep => _mapper.Map<UserDTO>(ep))
            .ToList();

        return usersEvent;
    }

    public async Task AddUserToEvent(int eventId, int userId, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            throw new OperationCanceledException("Operation was cancelled.");

        var user = await _userRepository.GetById(userId, cancellationToken) 
            ?? throw new KeyNotFoundException($"User with ID {userId} not found");
        
        var eventEntity = await _eventRepository.GetById(eventId, cancellationToken) 
            ?? throw new KeyNotFoundException($"Event with ID {eventId} not found");
        
        if (eventEntity.EventParticipants.Count >= eventEntity.MaxParticipants)
        {
            throw new InvalidOperationException("Cannot add more participants; the event is full.");
        }
            
        var eventParticipant = new EventParticipant
        {
            EventId = eventId,
            UserId = userId,
            Event = eventEntity,
            RegistrationDate = DateOnly.FromDateTime(DateTime.Now),
            User = user
        };
        
        await _eventParticipantsRepository.Add(eventParticipant, cancellationToken);
    }

    public async Task RemoveUserFromEvent(int eventId, int userId, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            throw new OperationCanceledException("Operation was cancelled.");
        
        _ = await _eventRepository.GetById(eventId, cancellationToken) 
            ?? throw new KeyNotFoundException($"Event with ID {eventId} not found");
        
        _ = await _userRepository.GetById(userId, cancellationToken) 
            ?? throw new KeyNotFoundException($"User with ID {userId} not found");
        
        await _eventParticipantsRepository.RemoveParticipant(eventId, userId, cancellationToken);
    }
}