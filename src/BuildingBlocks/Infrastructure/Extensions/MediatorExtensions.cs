using Contracts.Common.Events;
using Infrastructure.Common;
using MediatR;
using Serilog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public static class MediatorExtensions
    {

        public static async Task DispatchDomainEventsAsync(this IMediator mediator,
            List<BaseEvent> domainEvents,
            ILogger logger)
        {
            foreach (var domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent);
                var data = new SerializeService().Serialize(domainEvent);
                logger.Information($"\n-----\nA domain event has been published!\n" +
                                   $"Event: {domainEvent.GetType().Name}\n" +
                                   $"Data: {data})\n-----\n");
            }
        }
    }
}
