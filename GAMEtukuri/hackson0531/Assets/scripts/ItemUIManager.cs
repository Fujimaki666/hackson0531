using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemUIManager : MonoBehaviour
{
    public ItemDatabase itemDatabase;
    public Transform contentArea; // ScrollView‚ÌContent
    public GameObject itemButtonPrefab;

    public ItemPlacer itemPlacer;

    void Start()
    {
        foreach (var item in itemDatabase.items)
        {
            var buttonObj = Instantiate(itemButtonPrefab, contentArea);
            var buttonImage = buttonObj.GetComponent<Image>();
            //buttonImage.sprite = item.itemSprite;

            var button = buttonObj.GetComponent<Button>();
            button.onClick.AddListener(() => itemPlacer.SelectItem(item));
        }
    }
}
