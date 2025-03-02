using BLL.DTO;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;
using FluentValidation;
using MapsterMapper;

namespace BLL.Services;

internal class EventServices : IEventServices
{
    private readonly IEventRepository _eventRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<EventNewDTO> _validatorNewEvent;
    private readonly IValidator<EventDTO> _validatorEvent;

    public EventServices(
        IEventRepository eventRepository, 
        IMapper mapper, 
        IValidator<EventNewDTO> validatorNewEvent, 
        IValidator<EventDTO> validatorEvent)
    {
        _eventRepository = eventRepository;
        _mapper = mapper;
        _validatorNewEvent = validatorNewEvent;
        _validatorEvent = validatorEvent;
    }

    public async Task<EventDTO> GetEventById(int id, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            throw new OperationCanceledException("Operation was cancelled.");

        var foundEvent = await _eventRepository.GetById(id, cancellationToken);
        if (foundEvent == null)
            throw new KeyNotFoundException($"Event with ID {id} not found.");

        return _mapper.Map<EventDTO>(foundEvent);
    }

    public async Task<EventDTO> GetEventByName(string name, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            throw new OperationCanceledException("Operation was cancelled.");

        var foundEvent = await _eventRepository.GetEventByName(name, cancellationToken);
        if (foundEvent == null)
            throw new KeyNotFoundException($"Event with name '{name}' not found.");

        return _mapper.Map<EventDTO>(foundEvent);
    }

    public async Task AddEvent(EventNewDTO eventDto, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            throw new OperationCanceledException("Operation was cancelled.");

        var validationResult = await _validatorNewEvent.ValidateAsync(eventDto, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var eventEntity = _mapper.Map<Event>(eventDto);
        await _eventRepository.Add(eventEntity, cancellationToken);
    }

    public async Task UpdateEvent(EventDTO eventDto, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            throw new OperationCanceledException("Operation was cancelled.");

        var validationResult = await _validatorEvent.ValidateAsync(eventDto, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var eventEntity = _mapper.Map<Event>(eventDto);
        if (await _eventRepository.GetById(eventEntity.Id, cancellationToken) == null)
            throw new KeyNotFoundException($"Event with ID {eventEntity.Id} not found.");

        await _eventRepository.Update(eventEntity, cancellationToken);
    }

    public async Task DeleteEvent(int id, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            throw new OperationCanceledException("Operation was cancelled.");

        if (await _eventRepository.GetById(id, cancellationToken) == null)
            throw new KeyNotFoundException($"Event with ID {id} not found.");

        await _eventRepository.Delete(id, cancellationToken);
    }

    public async Task<List<EventDTO>> GetAll(QueryHelperDTO query, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            throw new OperationCanceledException("Operation was cancelled.");

        var events = await _eventRepository.GetAll(cancellationToken);

        if (!string.IsNullOrEmpty(query.Date) && DateOnly.TryParse(query.Date, out var parsedDate))
            events = events.Where(e => e.EventDateTime == parsedDate).ToList();

        if (!string.IsNullOrEmpty(query.Location))
            events = events.Where(e => e.Location.Contains(query.Location)).ToList();

        if (!string.IsNullOrEmpty(query.Category))
            events = events.Where(e => e.Category.Contains(query.Category)).ToList();

        if (events.Count == 0)
            throw new Exception("No events found matching the criteria.");
        
        if (query.Page <= 0 || query.PageSize <= 0)
            return _mapper.Map<List<EventDTO>>(events);
        //пагинация
        var pagedEvents = events
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();
        
        if (pagedEvents.Count == 0)
            throw new Exception("No events found matching the criteria.");
        
        return _mapper.Map<List<EventDTO>>(pagedEvents);
    }

    public async Task UploadImage(int id, MemoryStream memoryStream, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            throw new OperationCanceledException("Operation was cancelled.");

        var eventEntity = await _eventRepository.GetById(id, cancellationToken) ?? throw new KeyNotFoundException($"Event with ID {id} not found.");

        eventEntity.Image = memoryStream.ToArray();
        await _eventRepository.Update(eventEntity, cancellationToken);
    }
}