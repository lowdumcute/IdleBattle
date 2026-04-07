
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LineUpSlotButton : MonoBehaviour
{
    public Position position;
    public GameObject InfoObject;
    public InfoCharButton infoChar;
    public void Start()
    {
        InfoObject.SetActive(false);
        infoChar.characterImage.gameObject.SetActive(false);
    }

    public void UpdateCard(Character character)
    {
        if (character == null)
        {
            return;
        }
        infoChar.UpdateCard(character);
        infoChar.characterImage.gameObject.SetActive(true);
        InfoObject.SetActive(true);
    }
}

[System.Serializable]
public class InfoCharButton
{
    public int characterID; 
    [Header("UI Elements")]
    public Image characterImage;
    public GameObject Star;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI RarityText;
    public void UpdateCard(Character character)
    {
        if (character == null)
        {
            Debug.LogWarning("Character is null. Cannot update card.");
            return;
        }
        InitializeCard(character);
        GetSpriteStar(character);
    }
    public void InitializeCard(Character character)
    {
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
}
