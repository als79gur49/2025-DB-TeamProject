public class RankingData
{
    public string Name {  get; private set; }
    public int Score {  get; private set; }

    public RankingData(Entity entity)
    {
        Name = entity.name;
        Score = entity.Data.Score;
    }
};
