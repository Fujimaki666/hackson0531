using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Transform itemSlotParent;
    public GameObject itemSlotPrefab;

    private List<GameObject> currentSlots = new List<GameObject>();
    private int inputCount = 0;
    private const int maxInputs = 5;

    public MoodItemDatabase moodItemDatabase;

    public void ClearItems()
    {
        foreach (var slot in currentSlots)
        {
            Destroy(slot);
        }
        currentSlots.Clear();
        inputCount = 0;
    }

    public List<GameObject> GetCurrentSlots()
    {
        return currentSlots;
    }

    // 1�̃A�C�e��������ɉ����Ēǉ�����i��b���Ƃ�1�j
    public void AddItemByMood(string mood)
    {
        List<ItemData> moodItems = new List<ItemData>();

        if (mood == "happy")
        {
            moodItems = new List<ItemData>(moodItemDatabase.happyItems);
        }
        else if (mood == "sad")
        {
            moodItems = new List<ItemData>(moodItemDatabase.sadItems);
        }
        // ���̊�����K�v�Ȃ炱���ɒǉ�

        // null�ȃX���b�g����菜��
        currentSlots.RemoveAll(slot => slot == null);

        // ���ɕ\���ς݂̃A�C�e�������O
        foreach (var slot in currentSlots)
        {
            if (slot == null) continue;

            ItemSlotUI itemSlot = slot.GetComponent<ItemSlotUI>();
            if (itemSlot == null) continue;

            ItemData existingItem = itemSlot.GetItemData();
            moodItems.Remove(existingItem);
        }

        // ��₪�c���Ă��Ȃ���Ή������Ȃ�
        if (moodItems.Count == 0) return;

        // �����_����1�I��
        int index = Random.Range(0, moodItems.Count);
        ItemData selectedItem = moodItems[index];

        // �X���b�g���쐬���ĕ\��
        GameObject newSlot = Instantiate(itemSlotPrefab, itemSlotParent);
        ItemSlotUI slotUI = newSlot.GetComponent<ItemSlotUI>();
        slotUI.Setup(selectedItem);
        currentSlots.Add(newSlot);
    }
}
