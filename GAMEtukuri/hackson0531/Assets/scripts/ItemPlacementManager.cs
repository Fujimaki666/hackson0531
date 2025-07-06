using UnityEngine;

public class ItemPlacementManager : MonoBehaviour
{
    public static ItemPlacementManager Instance;

    private ItemData currentItem;              // ���ݑI�𒆂̃A�C�e��
    private ItemSlotUI currentSlotUI;          // �Ή�����X���b�gUI

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
    /// �z�u����A�C�e���ƑΉ�����X���b�gUI��ݒ�
    /// </summary>
    public void SetItemToPlace(ItemData item, ItemSlotUI slot)
    {
        currentItem = item;
        currentSlotUI = slot;
    }

    /// <summary>
    /// �}�E�X�ʒu�ɃA�C�e����z�u���A�����Ȃ�X���b�g���폜
    /// </summary>
    public void TryPlaceItem(Vector2 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("PlaceArea"))
        {
            Instantiate(currentItem.prefab, hit.point, Quaternion.identity);

            if (currentSlotUI != null)
            {
                currentSlotUI.DisableSlot(); // �X���b�g�폜
            }

            currentItem = null;
            currentSlotUI = null;
        }
        else
        {
            Debug.Log("�z�u�ł���G���A�ł͂���܂���");
        }
    }
}
