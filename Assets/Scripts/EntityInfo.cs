using UnityEngine;

public class EntityInfo
{
    private string entityName;
    private string imageName;

    public string EntityName => entityName;
    public string ImageName => imageName;

    public void Setup(EntityInfo info)
    {
        Setup(info.entityName, info.imageName);
    }
    public void Setup(string entityName = "Player_00", string imageName = "player_image_00")
    {
        this.entityName = entityName;
        this.imageName = imageName;
    }
};