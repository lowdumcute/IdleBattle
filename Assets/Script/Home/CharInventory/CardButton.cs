using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CardButton : MonoBehaviour
{
    public int characterID; // bắt đầu từ 1
    [Header("UI Elements")]
    public Image characterImage;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI RarityText;

    public void InitializeCard(Character character)
    {
        if (character == null)
        {
            Debug.LogWarning("Character is null. Cannot initialize card.");
            return;
        }

        NameText.text = character.currentStats.baseStats.characterName;
        LevelText.text = $"{character.currentStats.CurrentLevel}";
        RarityText.text = character.currentStats.baseStats.rare.rarity.ToString();
        characterImage.sprite = character.currentStats.baseStats.characterIcon;
    }
    public void OnCardButtonClicked()
    {
        if (CharacterInventory.Instance == null)
        {
            Debug.LogWarning("CharacterInventory Instance is null.");
            return;
        }

        int index = characterID;

        if (index < 0 || index >= CharacterInventory.Instance.ownedCharacters.Count)
        {
            Debug.LogWarning($"CharacterID {characterID} is out of range.");
            return;
        }

        Character character = CharacterInventory.Instance.ownedCharacters[index];
        CharDetail.Instance.UpdateInfo(character.currentStats, characterID);
        CharDetail.Instance.OpenPanel();
        // TODO: mở UI chi tiết nhân vật
        // CharacterDetailUI.Instance.Show(character);
    }
}
