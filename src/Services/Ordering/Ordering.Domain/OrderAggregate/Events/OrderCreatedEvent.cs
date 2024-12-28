using Contracts.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.OrderAggregate.Events
{
    public record OrderCreatedEvent(
       long Id,
       string UserName,
       decimal TotalPrice,
       string DocumentNo,
       string EmailAddress,
       string ShippingAddress,
       string InvoiceAddress,
       string FullName) : BaseEvent;
}
