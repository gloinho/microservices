using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data.Interfaces;
using PlatformService.Dtos;
using PlatformService.Models;
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

        public PlatformsController(
            IPlatformRepository platformRepository, 
            IMapper mapper, 
            ICommandDataClient commandDataClient)
        {
            _platformRepository = platformRepository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
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

            try
            {
                await _commandDataClient.SendPlatformTocommand(createdModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send synchronously {ex.Message}. ");
            }

            return CreatedAtRoute(nameof(GetPlatformById), new { createdModel.Id }, createdModel);
        }
    }
}
