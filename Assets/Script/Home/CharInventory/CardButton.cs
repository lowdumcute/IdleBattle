using UnityEngine;
using UnityEngine.UI;
using TMPro;
public enum CardType
{
    ButtonShowInfo
    
}
public class CardButton : MonoBehaviour
{
    public InfoCharButton infoButton;

    public void UpdateCard(Character character)
    {
        infoButton.UpdateCard(character);
    }
    public void OnCardButtonClicked()
    {
        if (CharacterInventory.Instance == null)
        {
            Debug.LogWarning("CharacterInventory Instance is null.");
            return;
        }

        int index = infoButton.characterID;

        if (index < 0 || index >= CharacterInventory.Instance.ownedCharacters.Count)
        {
            Debug.LogWarning($"CharacterID {infoButton.characterID} is out of range.");
            return;
        }

        Character character = CharacterInventory.Instance.ownedCharacters[index];
        CharDetail.Instance.UpdateInfo(character.currentStats, infoButton.characterID);
        CharDetail.Instance.OpenPanel();
        // TODO: mở UI chi tiết nhân vật
        // CharacterDetailUI.Instance.Show(character);
    }
}
