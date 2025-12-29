using Application.Commands.Events;
using Application.DataAdapters;

namespace Application
{
    /// <summary>
    /// Legacy wrapper for CreateEventCommand - maintained for backward compatibility
    /// </summary>
    public class CreateEventUseCase : ICreateEventUseCase
    {
        private readonly CreateEventCommandHandler _handler;

        public CreateEventUseCase(CreateEventCommandHandler handler)
        {
            _handler = handler;
        }

        public async Task<EventDTO> Execute(EventDTO newEvent)
        {
            var command = new CreateEventCommand(newEvent);
            return await _handler.Handle(command);
        }
    }
}
