/// <summary>
/// 랭킹 데이터를 담는 클래스
/// </summary>
[System.Serializable]
public class RankingData
{
    public string Name { get; private set; }
    public int Score { get; private set; }

    /// <summary>
    /// Entity로부터 RankingData 생성
    /// </summary>
    public RankingData(Entity entity)
    {
        Name = entity.name;
        Score = entity.Data.Score;
    }

    /// <summary>
    /// 직접 값으로 RankingData 생성
    /// </summary>
    public RankingData(string name, int score)
    {
        Name = name;
        Score = score;
    }

    /// <summary>
    /// 빈 생성자
    /// </summary>
    public RankingData()
    {
        Name = string.Empty;
        Score = 0;
    }

    public override string ToString()
    {
        return $"Name: {Name}, Score: {Score}";
    }
}
