﻿using ClassifiedAds.Application;
using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Infrastructure.Identity;
using ClassifiedAds.Services.Product.Commands;
using ClassifiedAds.Services.Product.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Product.EventHandlers
{
    public class ProductDeletedEventHandler : IDomainEventHandler<EntityDeletedEvent<Entities.Product>>
    {
        private readonly Dispatcher _dispatcher;
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<EventLog, long> _eventLogRepository;

        public ProductDeletedEventHandler(Dispatcher dispatcher,
            ICurrentUser currentUser,
            IRepository<EventLog, long> eventLogRepository)
        {
            _dispatcher = dispatcher;
            _currentUser = currentUser;
            _eventLogRepository = eventLogRepository;
        }

        public async Task HandleAsync(EntityDeletedEvent<Entities.Product> domainEvent, CancellationToken cancellationToken = default)
        {
            await _dispatcher.DispatchAsync(new AddAuditLogEntryCommand
            {
                AuditLogEntry = new AuditLogEntry
                {
                    UserId = _currentUser.UserId,
                    CreatedDateTime = domainEvent.EventDateTime,
                    Action = "DELETED_PRODUCT",
                    ObjectId = domainEvent.Entity.Id.ToString(),
                    Log = domainEvent.Entity.AsJsonString(),
                },
            });

            await _eventLogRepository.AddOrUpdateAsync(new EventLog
            {
                EventType = "PRODUCT_DELETED",
                TriggeredById = _currentUser.UserId,
                CreatedDateTime = domainEvent.EventDateTime,
                ObjectId = domainEvent.Entity.Id.ToString(),
                Message = domainEvent.Entity.AsJsonString(),
                Published = false,
            }, cancellationToken);

            await _eventLogRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
