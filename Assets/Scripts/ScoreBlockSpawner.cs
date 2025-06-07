using UnityEditor.ShaderGraph.Legacy;
using UnityEngine;

public class ScoreBlockSpawner : MonoBehaviour
{
    private Vector3 standardPosition = new Vector3(-40, 0.5f, -40);

    private float xSize = 20 * 5;
    private float ySize = 20 * 5;

    private float distance = 5f;

    [SerializeField]
    private ScoreBlock scoreBlockPrefab;

    [Header("큰 경험치")]
    [SerializeField]
    private Color bigBlockColor = Color.cyan;
    [SerializeField]
    private int bigBlockScore = 200;

    [Header("중간 경험치")]
    [SerializeField]
    private Color mediumBlockColor = Color.white;
    [SerializeField]
    private int mediumBlockScore = 100;

    [Header("작은 경험치")]
    [SerializeField]
    private Color smallBlockColor = Color.blue;
    [SerializeField]
    private int smallBlockScore = 50;

    public void Setup(Vector3 standardPosition, float xSize, float ySize)
    {
        this.standardPosition = standardPosition;

        this.xSize = xSize;
        this.ySize = ySize;
    }


    void Start()
    {
        SpawnScoreBlocks();
    }
    public void SpawnScoreBlocks()
    {
        for(float i = 0; i < ySize; i+=distance)
        {
            for(float j = 0; j < xSize; j+=distance)
            {
                Vector3 addedAmount = new Vector3(j, 0, i);
                if(! Physics.CheckBox(standardPosition + addedAmount, new Vector3(0.5f, 0.3f, 0.5f), Quaternion.identity))
                {
                    Spawn(standardPosition + addedAmount).YoYoMoving();

                }
            }
        }
    }

    public void SpawnScoreBlocksByKilling(Vector3 spawnPosition, int amount)
    {
        for (int i = 0;i < amount; ++i)
        {
            Spawn(spawnPosition).LaunchUpwards();
        }
    }

    private ScoreBlock Spawn(Vector3 spawnPoint)
    {
        ScoreBlock clone = Instantiate(scoreBlockPrefab, spawnPoint, Quaternion.identity);

        float randomValue = Random.Range(0, 1f);
        if(randomValue > 0.8f)
        {
            clone.Setup(bigBlockScore, bigBlockColor, 2f);
        }
        else if(randomValue > 0.5f)
        {
            clone.Setup(mediumBlockScore, mediumBlockColor, 1.5f);
        }
        else if(randomValue >= 0f)
        {
            clone.Setup(smallBlockScore, smallBlockColor, 0.8f);
        }

        return clone;
    }
}
