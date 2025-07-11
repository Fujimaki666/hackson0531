using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    private ItemData itemData;

    public void Setup(ItemData data)
    {
        itemData = data;
        icon.sprite = data.icon;
    }

    public void OnClick()
    {
        if (itemData != null)
        {
            // ItemPlacementManager に2つの引数を渡す
            ItemPlacementManager.Instance.SetItemToPlace(itemData, this);
        }
    }

    public void DisableSlot()
    {
        Destroy(gameObject);
    }
    public ItemData GetItemData()
    {
        return itemData;
    }

}
