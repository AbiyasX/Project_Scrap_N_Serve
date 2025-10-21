using UnityEngine;

[CreateAssetMenu(fileName = "Items", menuName = "Items Data/New Item")]
public class ItemData : ScriptableObject
{
    public string materialName;
    public Sprite materialIcon;
    public GameObject materialPrefab;
}
