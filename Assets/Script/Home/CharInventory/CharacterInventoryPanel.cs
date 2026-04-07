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

        var list = CharacterInventory.Instance.ownedCharacters;

        for (int i = 0; i < list.Count; i++)
        {
            Character character = list[i];

            GameObject cardObject = Instantiate(cardPrefab, Parent.transform);

            // Update UI
            CardButton cardButton = cardObject.GetComponent<CardButton>();
            cardButton.UpdateCard(character);

            // 🔥 GÁN ID Ở ĐÂY
            InfoCharButton info = cardButton.infoButton; // Lấy InfoCharButton từ CardButton
            if (info != null)
            {
                info.characterID = i;
            }
            else
            {
                Debug.LogError("Card prefab thiếu InfoCharButton!");
            }
        }
    }
}
