using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/MoodItemDatabase")]
public class MoodItemDatabase : ScriptableObject
{
    public List<ItemData> happyItems;
    public List<ItemData> sadItems;
}
