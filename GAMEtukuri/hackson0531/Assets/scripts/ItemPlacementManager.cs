using UnityEngine;

public class ItemPlacementManager : MonoBehaviour
{
    public static ItemPlacementManager Instance;

    private ItemData currentItem;              // 現在選択中のアイテム
    private ItemSlotUI currentSlotUI;          // 対応するスロットUI

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && currentItem != null)
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            TryPlaceItem(worldPos);
        }
    }

    /// <summary>
    /// 配置するアイテムと対応するスロットUIを設定
    /// </summary>
    public void SetItemToPlace(ItemData item, ItemSlotUI slot)
    {
        currentItem = item;
        currentSlotUI = slot;
    }

    /// <summary>
    /// マウス位置にアイテムを配置し、成功ならスロットを削除
    /// </summary>
    public void TryPlaceItem(Vector2 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("PlaceArea"))
        {
            Instantiate(currentItem.prefab, hit.point, Quaternion.identity);

            if (currentSlotUI != null)
            {
                currentSlotUI.DisableSlot(); // スロット削除
            }

            currentItem = null;
            currentSlotUI = null;
        }
        else
        {
            Debug.Log("配置できるエリアではありません");
        }
    }
}
