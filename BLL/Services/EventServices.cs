using BLL.DTO;
using BLL.Interfaces;
using DAL.Helpers;
using DAL.Interfaces;
using DAL.Models;
using FluentValidation;
using MapsterMapper;

namespace BLL.Services;

internal class EventServices: IEventServices
{
    private readonly IEventRepository _eventRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<EventNewDTO> _validatorNewEvent;
    private readonly IValidator<EventDTO> _validatorEvent;

    public EventServices(IEventRepository eventRepository, IMapper mapper, IValidator<EventNewDTO> validatorNewEvent, IValidator<EventDTO> validatorEvent)
    {
        _eventRepository = eventRepository;
        _mapper = mapper;
        _validatorNewEvent = validatorNewEvent;
        _validatorEvent = validatorEvent;
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

    public async Task AddEvent(EventNewDTO eventDto)
    {
        var validationResult = await _validatorNewEvent.ValidateAsync(eventDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var eventEntity = _mapper.Map<Event>(eventDto);
        await _eventRepository.Add(eventEntity);
    }

    public async Task UpdateEvent(EventDTO eventDto)
    {
        var validationResult = await _validatorEvent.ValidateAsync(eventDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var eventEntity = _mapper.Map<Event>(eventDto);
        await _eventRepository.Update(eventEntity);
    }

    public async Task DeleteEvent(int id)
    {
        await _eventRepository.Delete(id);
    }

    public async Task<List<EventDTO>> GetAll(QueryHelperDTO query)
    {
        var events = await _eventRepository.GetAll(_mapper.Map<QueryHelper>(query));
        
        // DateOnly? parsedDate = null;
        //
        // if (!string.IsNullOrEmpty(query.Date) && DateOnly.TryParseExact(query.Date, "yyyy-MM-dd", out var dateOnly))
        //     parsedDate = dateOnly;
        //
        // if (parsedDate != null)
        //     events = events.Where(e => e.EventDateTime == parsedDate).ToList();
        //
        // if (!string.IsNullOrEmpty(query.Location) )
        //     events = events.Where(e => e.Location.Equals(query.Location, StringComparison.OrdinalIgnoreCase)).ToList();
        //
        // if (!string.IsNullOrEmpty(query.Category))
        //     events = events.Where(e => e.Category.Equals(query.Category, StringComparison.OrdinalIgnoreCase)).ToList();

        return _mapper.Map<List<EventDTO>>(events);
    }

    public async Task UploadImage(int id, MemoryStream memoryStream)
    {
        var eventEntity = await _eventRepository.GetById(id) ?? throw new NullReferenceException();
            eventEntity.Image = memoryStream.ToArray();
        
        await _eventRepository.Update(eventEntity);
    }
}