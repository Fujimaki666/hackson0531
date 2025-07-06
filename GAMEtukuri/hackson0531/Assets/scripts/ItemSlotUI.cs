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
            // ItemPlacementManager ‚É2‚Â‚Ìˆø”‚ğ“n‚·
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
