using Application.Commands.Events;
using Entities;

namespace Application
{
    /// <summary>
    /// Legacy wrapper for UpdateEventCommand - maintained for backward compatibility
    /// </summary>
    public class UpdateEventUseCase : IUpdateEventUseCase
    {
        private readonly UpdateEventCommandHandler _handler;

        public UpdateEventUseCase(UpdateEventCommandHandler handler)
        {
            _handler = handler;
        }

        public async Task<Event> Execute(Event updated)
        {
            var command = new UpdateEventCommand(updated);
            return await _handler.Handle(command);
        }
    }
}
