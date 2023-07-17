using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using CommandsService.Models.Enums;
using System.Text.Json;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory,IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }
        public void ProcessEvent(string message)
        {
            var platformPublished = JsonSerializer.Deserialize<PlatformPublishedDto>(message);
            switch(platformPublished.Event) 
            {
                case Event.PlatformPublished:
                    Console.WriteLine("--> Platform Published Event Detected");
                    addPlatform(platformPublished);
                    break;
                default:
                    break;
            }
        }

        private void addPlatform(PlatformPublishedDto platformPublished)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();
                try
                {
                    var platform = _mapper.Map<Platform>(platformPublished);
                    if (!repository.ExternalPlatformExists(platform.ExternalId))
                    {
                        repository.CreatePlatform(platform);
                        repository.SaveChanges();
                        Console.WriteLine("--> Platform added!");
                    }
                    else
                    {
                        Console.WriteLine("--> Platform already exists.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not add Platform to db {ex.Message}");
                }
            }
        }
    }
}
