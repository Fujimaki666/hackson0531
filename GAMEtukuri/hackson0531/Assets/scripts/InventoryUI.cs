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

    // 1つのアイテムを感情に応じて追加する（会話ごとに1つ）
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
        // 他の感情も必要ならここに追加

        // nullなスロットを取り除く
        currentSlots.RemoveAll(slot => slot == null);

        // 既に表示済みのアイテムを除外
        foreach (var slot in currentSlots)
        {
            if (slot == null) continue;

            ItemSlotUI itemSlot = slot.GetComponent<ItemSlotUI>();
            if (itemSlot == null) continue;

            ItemData existingItem = itemSlot.GetItemData();
            moodItems.Remove(existingItem);
        }

        // 候補が残っていなければ何もしない
        if (moodItems.Count == 0) return;

        // ランダムに1つ選ぶ
        int index = Random.Range(0, moodItems.Count);
        ItemData selectedItem = moodItems[index];

        // スロットを作成して表示
        GameObject newSlot = Instantiate(itemSlotPrefab, itemSlotParent);
        ItemSlotUI slotUI = newSlot.GetComponent<ItemSlotUI>();
        slotUI.Setup(selectedItem);
        currentSlots.Add(newSlot);
    }
}
