using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CardButton : MonoBehaviour
{
    public int characterID; // bắt đầu từ 1
    [Header("UI Elements")]
    public Image characterImage;
    public GameObject Star;
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
        GetSpriteStar(character);
    }
    public void GetSpriteStar(Character character)
    {
        int star = character.star;

        Sprite emptyStar = DataManager.Instance.AllData.GetSpriteStar(StarType.Empty);
        Sprite yellowStar = DataManager.Instance.AllData.GetSpriteStar(StarType.Yellow);
        Sprite redStar = DataManager.Instance.AllData.GetSpriteStar(StarType.Red);
        Sprite diamondStar = DataManager.Instance.AllData.GetSpriteStar(StarType.Diamond);

        Image[] starImages = Star.GetComponentsInChildren<Image>();

        // reset
        for (int i = 0; i < starImages.Length; i++)
        {
            starImages[i].sprite = emptyStar;
        }

        // logic
        if (star <= 5)
        {
            for (int i = 0; i < star; i++)
            {
                starImages[i].sprite = yellowStar;
            }
        }
        else if (star <= 10)
        {
            int redCount = star - 5;

            for (int i = 0; i < redCount; i++)
            {
                starImages[i].sprite = redStar;
            }
        }
        else
        {
            int diamondCount = star - 10;

            for (int i = 0; i < diamondCount && i < starImages.Length; i++)
            {
                starImages[i].sprite = diamondStar;
            }
        }
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
