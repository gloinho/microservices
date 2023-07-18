using AutoMapper;
using Grpc.Core;
using PlatformService.Data.Interfaces;

namespace PlatformService.SyncDataServices.Grpc
{
    // GrpcPlatform é a classe auto-generated pelo Grpc, no momento do build, definida pelo arquivo platforms.proto
    public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
    {
        private readonly IPlatformRepository _platformRepository;
        private readonly IMapper _mapper;

        public GrpcPlatformService(IPlatformRepository repository, IMapper mapper)
        {
            _platformRepository = repository;
            _mapper = mapper;
        }

        public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
        {
            var response = new PlatformResponse();
            var platforms = _platformRepository.GetAllPlatforms();
            foreach ( var platform in platforms )
            {
                response.Platform.Add(_mapper.Map<GrpcPlatformModel>(platform));
            }
            return Task.FromResult(response);
        }
    }
}
