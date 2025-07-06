using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Item/Create Item Database")]
public class ItemDatabase : ScriptableObject
{
    public ItemData[] items;
}
