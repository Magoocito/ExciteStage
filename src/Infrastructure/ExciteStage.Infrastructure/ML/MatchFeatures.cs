using Microsoft.ML.Data;

namespace ExciteStage.Infrastructure.ML
{
    public class MatchFeatures
    {
        [LoadColumn(5)]
        public float HomeAltitude { get; set; }

        [LoadColumn(6)]
        public float TravelDistance { get; set; }

        [LoadColumn(7)]
        public float IsHighAltitude { get; set; }

        [LoadColumn(8), ColumnName("Label")]
        public float HomeWin { get; set; }
    }
}