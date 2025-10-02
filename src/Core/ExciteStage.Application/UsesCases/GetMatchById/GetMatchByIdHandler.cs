using MediatR;
using AutoMapper;
using ExciteStage.Application.Repositories;

namespace ExciteStage.Application.UseCases.GetMatchById
{
    public class GetMatchByIdHandler : IRequestHandler<GetMatchByIdRequest, GetMatchByIdResponse>
    {
        private readonly IMatchRepository _matchRepo;
        private readonly IMapper _mapper;

        public GetMatchByIdHandler(IMatchRepository matchRepo, IMapper mapper)
        {
            _matchRepo = matchRepo;
            _mapper = mapper;
        }

        public async Task<GetMatchByIdResponse> Handle(GetMatchByIdRequest request, CancellationToken cancellationToken)
        {
            var match = await _matchRepo.GetByIdAsync(request.MatchId, cancellationToken);
            if (match == null)
                throw new InvalidOperationException($"Match {request.MatchId} not found");

            return _mapper.Map<GetMatchByIdResponse>(match);
        }
    }
}