using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data.Interfaces;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.Models.Enums;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepository _platformRepository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(
            IPlatformRepository platformRepository, 
            IMapper mapper, 
            ICommandDataClient commandDataClient,
            IMessageBusClient messageBusClient)
        {
            _platformRepository = platformRepository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetAllPlatforms()
        {
            var platforms = _mapper.Map<IEnumerable<PlatformReadDto>>(_platformRepository.GetAllPlatforms());
            return platforms is null ? NotFound() : Ok(platforms);
        }

        [HttpGet("{id}", Name = nameof(GetPlatformById))]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            var platform = _mapper.Map<PlatformReadDto>(_platformRepository.GetPlatformById(id));   
            return platform is null ? NotFound() : Ok(platform);
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platform)
        {
            var model = _mapper.Map<Platform>(platform);
            _platformRepository.CreatePlatform(model);
            _platformRepository.SaveChanges();

            var createdModel = _mapper.Map<PlatformReadDto>(model);

            // Envio de mensagem (sincrona) para o CommandService via http.
            try
            {
                await _commandDataClient.SendPlatformTocommand(createdModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send synchronously {ex.Message}. ");
            }

            // Envio de mensagem (assincrona) para o CommandService via RabbitMQ (bus)
            try
            {
                var platformPublished = _mapper.Map<PlatformPublishedDto>(createdModel);
                
                // É interessante documentar os tipos de eventos que podem acontecer dentro do seu app.
                platformPublished.Event = Event.PlatformPublished;

                _messageBusClient.PublishNewPlatform(platformPublished);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send asynchronously {ex.Message}. ");
            }

            return CreatedAtRoute(nameof(GetPlatformById), new { createdModel.Id }, createdModel);
        }
    }
}
