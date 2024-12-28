using Contracts.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.OrderAggregate.Events
{
    public record OrderDeletedEvent(long Id) : BaseEvent;

}
