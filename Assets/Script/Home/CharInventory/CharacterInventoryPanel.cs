using UnityEngine;

public class CharacterInventoryPanel : MonoBehaviour
{
    public GameObject cardPrefab; // Prefab cho thẻ nhân vật
    public GameObject Parent; // địa chỉ chứa các thẻ nhân vật
    public GameObject Panel; // Panel chứa các thẻ nhân vật
    public void Start()
    {
        Panel.SetActive(false); // ẩn panel khi bắt đầu
    }
    public void TogglePanel()
    {
        Panel.SetActive(!Panel.activeSelf);
        if (Panel.activeSelf)
        {
            LoadCharacters();
        }
    }
    public void LoadCharacters()
    {
        // Clear existing character cards
        foreach (Transform child in Parent.transform)
        {
            Destroy(child.gameObject);
        }

        // Load characters from inventory
        foreach (Character character in CharacterInventory.Instance.ownedCharacters)
        {
            GameObject cardObject = Instantiate(cardPrefab, Parent.transform);
            CardButton cardButton = cardObject.GetComponent<CardButton>();
            cardButton.InitializeCard(character);
        }
    }
}
