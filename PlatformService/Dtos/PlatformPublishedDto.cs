using PlatformService.Models.Enums;

namespace PlatformService.Dtos
{
    public class PlatformPublishedDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Event Event { get; set; }
    }
}
