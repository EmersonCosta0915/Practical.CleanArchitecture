﻿using ClassifiedAds.Application;
using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Infrastructure.Identity;
using ClassifiedAds.Services.AuditLog.Contracts.DTOs;
using ClassifiedAds.Services.Product.Api.Commands;
using ClassifiedAds.Services.Storage.DTOs;
using ClassifiedAds.Services.Storage.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ClassifiedAds.Services.Storage.EventHandlers
{
    public class FileEntryDeletedEventHandler : IDomainEventHandler<EntityDeletedEvent<FileEntry>>
    {
        private readonly IServiceProvider _serviceProvider;

        public FileEntryDeletedEventHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Handle(EntityDeletedEvent<FileEntry> domainEvent)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var currentUser = serviceProvider.GetService<ICurrentUser>();
                var dispatcher = serviceProvider.GetService<Dispatcher>();

                dispatcher.Dispatch(new AddAuditLogEntryCommand
                {
                    AuditLogEntry = new AuditLogEntryDTO
                    {
                        UserId = currentUser.IsAuthenticated ? currentUser.UserId : Guid.Empty,
                        CreatedDateTime = domainEvent.EventDateTime,
                        Action = "DELETE_FILEENTRY",
                        ObjectId = domainEvent.Entity.Id.ToString(),
                        Log = domainEvent.Entity.AsJsonString(),
                    },
                });

                var fileDeletedEventSender = serviceProvider.GetService<IMessageSender<FileDeletedEvent>>();

                // Forward to external systems
                fileDeletedEventSender.Send(new FileDeletedEvent
                {
                    FileEntry = domainEvent.Entity,
                });
            }
        }
    }
}
