using MediatR;

namespace ExciteStage.Application.UseCases.GetMatches
{
    public class GetMatchesRequest : IRequest<GetMatchesResponse>
    {
        public int Limit { get; set; } = 10;
        public bool UpcomingOnly { get; set; } = true;
    }
}