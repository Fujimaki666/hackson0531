using UnityEngine;

public class ItemPlacer : MonoBehaviour
{
    public RectTransform placementArea;
    public Canvas canvas;

    private ItemData selectedItem;

    public void SelectItem(ItemData item)
    {
        selectedItem = item;
    }

    void Update()
    {
        if (selectedItem != null && Input.GetMouseButtonDown(0))
        {
            Vector2 localMousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                placementArea, Input.mousePosition, canvas.worldCamera, out localMousePosition);

            if (placementArea.rect.Contains(localMousePosition))
            {
                Instantiate(selectedItem.prefab, placementArea.TransformPoint(localMousePosition), Quaternion.identity, placementArea);
            }
        }
    }
}
