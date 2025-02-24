using BLL.DTO;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;
using FluentValidation;
using MapsterMapper;

namespace BLL.Services;

internal class EventServices: IEventServices
{
    private readonly IEventRepository _eventRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<EventDTO> _validator;

    public EventServices(IEventRepository eventRepository, IMapper mapper, IValidator<EventDTO> validator)
    {
        _eventRepository = eventRepository;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<List<EventDTO>> GetAllEvents()
    {
        var events = await _eventRepository.GetAll(); 
        return _mapper.Map<List<EventDTO>>(events);
    }

    public async Task<EventDTO> GetEventById(int id)
    {
        var foundEvent = await _eventRepository.GetById(id);
        return _mapper.Map<EventDTO>(foundEvent);
    }

    public async Task<EventDTO> GetEventByName(string name)
    {
        var foundEvent = await _eventRepository.GetEventByName(name);
        return _mapper.Map<EventDTO>(foundEvent);
    }

    public async Task AddEvent(EventDTO eventDto)
    {
        var validationResult = await _validator.ValidateAsync(eventDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var eventEntity = _mapper.Map<Event>(eventDto);
        await _eventRepository.Add(eventEntity);
    }

    public async Task UpdateEvent(EventDTO eventDto)
    {
        var validationResult = await _validator.ValidateAsync(eventDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var eventEntity = _mapper.Map<Event>(eventDto);
        await _eventRepository.Update(eventEntity);
    }

    public async Task DeleteEvent(int id)
    {
        await _eventRepository.Delete(id);
    }

    public async Task<List<EventDTO>> GetAll(DateOnly? date, string? location, string? category)
    {
        var events = await _eventRepository.GetAll();
    
        if (date != null)
            events = events.Where(e => e.EventDateTime == date.Value).ToList();
    
        if (!string.IsNullOrEmpty(location) )
            events = events.Where(e => e.Location.Equals(location, StringComparison.OrdinalIgnoreCase)).ToList();
    
        if (!string.IsNullOrEmpty(category))
            events = events.Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();

        return _mapper.Map<List<EventDTO>>(events);
    }

    public async Task UploadImage(int id, MemoryStream memoryStream)
    {
        var eventEntity = await _eventRepository.GetById(id) ?? throw new NullReferenceException();
            eventEntity.Image = memoryStream.ToArray();
        
        await _eventRepository.Update(eventEntity);
    }
}