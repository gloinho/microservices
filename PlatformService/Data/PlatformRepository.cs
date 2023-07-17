using PlatformService.Data.Interfaces;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformRepository : IPlatformRepository
    {
        private readonly AppDbContext _dbcontext;

        public PlatformRepository(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public void CreatePlatform(Platform platform)
        {
            if(platform is null)
            {
                throw new ArgumentException(nameof(platform));
            }
            _dbcontext.Platforms.Add(platform);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _dbcontext.Platforms.ToList();
        }

        public Platform GetPlatformById(int id)
        {
            return _dbcontext.Platforms.FirstOrDefault(p => p.Id == id);
        }

        public bool SaveChanges()
        {
            return (_dbcontext.SaveChanges() >= 0);
        }
    }
}
