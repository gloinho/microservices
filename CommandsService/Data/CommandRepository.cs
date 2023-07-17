using CommandsService.Models;
using System;

namespace CommandsService.Data
{
    public class CommandRepository : ICommandRepository
    {
        private readonly AppDbContext _dbContext;

        public CommandRepository(AppDbContext dbcontext)
        {
            _dbContext = dbcontext;
        }

        public void CreateCommand(Command command, int platformId)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            command.PlatformId = platformId;
            _dbContext.Add(command);
        }

        public void CreatePlatform(Platform platform)
        {
            if(platform is null)
            {
                throw new ArgumentNullException(nameof(platform));
            }
            _dbContext.Add(platform);
        }

        public IEnumerable<Command> GetAllCommandsForPlatform(int platformId)
        {
            return _dbContext.Commands.Where(c => c.PlatformId == platformId).OrderBy(p => p.Platform.Name);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _dbContext.Platforms.ToList();
        }

        public Command GetCommand(int platformId, int commandId)
        {
            return _dbContext.Commands.FirstOrDefault(c => c.Id == commandId && c.PlatformId == platformId);
        }
        public bool ExternalPlatformExists(int externalPlatformId)
        {
            return _dbContext.Platforms.Any(p => p.ExternalId == externalPlatformId);
        }
        public bool PlatformExists(int platformId)
        {
            return _dbContext.Platforms.Any(p => p.Id == platformId);
        }

        public bool SaveChanges()
        {
            return (_dbContext.SaveChanges() >= 0);
        }
    }
}
