using MediatR;

namespace ExciteStage.Application.UseCases.GetMatchById
{
    public class GetMatchByIdRequest : IRequest<GetMatchByIdResponse>
    {
        public int MatchId { get; set; }
    }
}