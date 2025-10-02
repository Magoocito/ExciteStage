using MediatR;
using AutoMapper;
using ExciteStage.Application.Repositories;

namespace ExciteStage.Application.UseCases.GetMatches
{
    public class GetMatchesHandler : IRequestHandler<GetMatchesRequest, GetMatchesResponse>
    {
        private readonly IMatchRepository _matchRepo;
        private readonly IMapper _mapper;

        public GetMatchesHandler(IMatchRepository matchRepo, IMapper mapper)
        {
            _matchRepo = matchRepo;
            _mapper = mapper;
        }

        public async Task<GetMatchesResponse> Handle(GetMatchesRequest request, CancellationToken cancellationToken)
        {
            var matches = request.UpcomingOnly
                ? await _matchRepo.GetUpcomingMatchesAsync(cancellationToken)
                : await _matchRepo.GetAllAsync(cancellationToken);

            var matchDtos = _mapper.Map<List<MatchDto>>(matches);
            return new GetMatchesResponse { Matches = matchDtos };
        }
    }
}