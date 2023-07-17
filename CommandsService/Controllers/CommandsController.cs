using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepository _commandRepository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepository commandRepository, IMapper mapper)
        {
            _commandRepository = commandRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetAllCommandsForPlatform(int platformId)
        {
            Console.WriteLine($"--> Hit GetCommandsForPlatform: {platformId}");

            if(!_commandRepository.PlatformExists(platformId))
            {
                return NotFound();
            }

            var commands = _mapper.Map<IEnumerable<CommandReadDto>>(_commandRepository.GetAllCommandsForPlatform(platformId)); 
            return Ok(commands);
        }

        [HttpGet("{commandId}", Name ="GetCommand")]
        public ActionResult<CommandReadDto> GetCommand(int platformId,int commandId)
        {
            Console.WriteLine($"--> Hit GetCommand: platform: {platformId}/command: {commandId} ");

            if (!_commandRepository.PlatformExists(platformId))
            {
                return NotFound();
            }

            var command = _mapper.Map<CommandReadDto>(_commandRepository.GetCommand(platformId, commandId));
            return Ok(command);
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommand(int platformId, CommandCreateDto commandDto)
        {
            Console.WriteLine($"--> Hit CreateCommand: {platformId}");

            if (!_commandRepository.PlatformExists(platformId))
            {
                return NotFound();
            }

            var commandModel = _mapper.Map<Command>(commandDto);
            _commandRepository.CreateCommand(commandModel,platformId);
            _commandRepository.SaveChanges();

            var commandRead = _mapper.Map<CommandReadDto>(commandModel);

            return CreatedAtRoute(nameof(GetCommand), new { commandId = commandRead.Id, platformId = commandRead.PlatformId }, commandRead);
        }

    }
}
