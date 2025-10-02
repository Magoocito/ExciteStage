using MediatR;
using AutoMapper;
using ExciteStage.Application.Repositories;
using ExciteStage.Application.Services;
using ExciteStage.Domain.Entities;

namespace ExciteStage.Application.UseCases.CreatePortfolio
{
    public class CreatePortfolioHandler : IRequestHandler<CreatePortfolioRequest, CreatePortfolioResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMatchRepository _matchRepo;
        private readonly IPortfolioRepository _portfolioRepo;
        private readonly IMLPredictionService _mlService;
        private readonly IPortfolioOptimizer _optimizer;
        private readonly IMapper _mapper;

        public CreatePortfolioHandler(
            IUnitOfWork unitOfWork,
            IMatchRepository matchRepo,
            IPortfolioRepository portfolioRepo,
            IMLPredictionService mlService,
            IPortfolioOptimizer optimizer,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _matchRepo = matchRepo;
            _portfolioRepo = portfolioRepo;
            _mlService = mlService;
            _optimizer = optimizer;
            _mapper = mapper;
        }

        public async Task<CreatePortfolioResponse> Handle(CreatePortfolioRequest request, CancellationToken cancellationToken)
        {
            var match = await _matchRepo.GetByIdAsync(request.MatchId, cancellationToken);
            if (match == null)
                throw new InvalidOperationException($"Match {request.MatchId} not found");

            var predictions = await _mlService.GeneratePredictionsAsync(match);
            var sentiment = await _mlService.GetExpertOpinionAsync(match);
            var portfolio = await _optimizer.OptimizePortfolioAsync(match, predictions);

            if (portfolio.TotalStakePercent > request.MaxRiskPercent)
                throw new InvalidOperationException($"Portfolio risk {portfolio.TotalStakePercent:P} exceeds max {request.MaxRiskPercent:P}");

            await _portfolioRepo.CreateAsync(portfolio, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return _mapper.Map<CreatePortfolioResponse>(portfolio);
        }
    }
}