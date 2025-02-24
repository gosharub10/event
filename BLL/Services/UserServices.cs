using BLL.DTO;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;
using MapsterMapper;
using ArgumentNullException = System.ArgumentNullException;

namespace BLL.Services;

internal class UserServices: IUserServices
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

    public async Task<UserDTO> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetById(id);
        return _mapper.Map<UserDTO>(user);
    }

    public async Task<List<UserDTO>> GetByEventName(string eventName)
    {
        if (string.IsNullOrWhiteSpace(eventName))
            throw new ArgumentNullException(nameof(eventName), "event name cannot be null or empty");

        var eventEntity = await _eventRepository.GetEventByName(eventName);
    
        if (eventEntity == null)
            throw new ArgumentNullException($"event with name {eventName} not found");

        var eventParticipants = await _eventParticipantsRepository.GetAll();
        var foundEventParticipants = eventParticipants.Where(e => e.EventId == eventEntity.Id).ToList();
        
        if (foundEventParticipants == null)
            throw new ArgumentNullException($"no users found for event: {eventName}");

        var usersEvent = foundEventParticipants
            .Select(ep => _mapper.Map<UserDTO>(ep))
            .ToList();

        return usersEvent;
    }

    public async Task AddUserToEvent(int eventId, int userId)
    {
        var user = await _userRepository.GetById(userId) ?? throw new ArgumentNullException(nameof(userId));
        var eventEntity = await _eventRepository.GetById(eventId) ?? throw new ArgumentNullException(nameof(eventId));
        
        if (eventEntity.MaxParticipants > eventEntity.EventParticipants.Count)
        {
            throw new ArgumentException("max participants cannot be greater than max participants");
        }
            
        var eventParticipants = new EventParticipant
        {
            EventId = eventId,
            UserId = userId,
            Event = eventEntity,
            RegistrationDate = DateOnly.FromDateTime(DateTime.Now),
            User = user
        };
        
        await _eventParticipantsRepository.Add(eventParticipants);
    }

    public async Task RemoveUserFromEvent(int eventId, int userId)
    {
        var eventEntity = await _eventRepository.GetById(eventId) ?? throw new ArgumentNullException(nameof(eventId));
        var user = await _userRepository.GetById(userId) ?? throw new ArgumentNullException(nameof(userId));
        
        await _eventParticipantsRepository.RemoveParticipant(eventId, userId);
    }
}