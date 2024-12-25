using MediatR;
using Ordering.Domain.OrderAggregate.Events;
using Serilog;

namespace Ordering.Application.Features.V1.Orders;

public class OrdersDomainHandler : 
    INotificationHandler<OrderCreatedEvent>,
    INotificationHandler<OrderDeletedEvent>
{
    private readonly ILogger _logger;
    public OrdersDomainHandler(ILogger logger)
    {
        _logger = logger;
    }
    
    public Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.Information("Ordering Domain Event: {DomainEvent}", notification.GetType().Name);
        try
        {
            _logger.Information($"Sent Created Order to email {notification.EmailAddress}");
        }
        catch (Exception ex)
        {
            _logger.Error($"Order {notification.Id} failed due to an error with the email service: {ex.Message}");
        }
        
        return Task.CompletedTask;
    }

    public Task Handle(OrderDeletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.Information("Ordering Domain Event: {DomainEvent}", notification.GetType().Name);
        return Task.CompletedTask;
    }
}