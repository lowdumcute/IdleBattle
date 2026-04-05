using UnityEngine;
using System.Collections.Generic;

public class LineUp : MonoBehaviour
{
    public static LineUp Instance;

    public List<PositionInLineup> myLineup = new List<PositionInLineup>();

    private void Awake()
    {
        Instance = this;
    }

    // dùng cho network
    public NetworkCharacterData[] GetNetworkLineup()
    {
        List<NetworkCharacterData> result = new List<NetworkCharacterData>();

        foreach (var c in myLineup)
        {
            if (c.character.currentStats == null)
            {
                Debug.LogError("currentStats NULL");
                continue;
            }

            if (c.character.currentStats.baseStats == null)
            {
                Debug.LogError("baseStats NULL");
                continue;
            }

            Debug.Log($"Lineup ID: {c.character.currentStats.baseStats.characterID}");

            result.Add(new NetworkCharacterData
            {
                characterID = c.character.currentStats.baseStats.characterID,
                level = (int)c.character.currentStats.CurrentLevel,
                star = c.character.star,
                realm = (int)c.character.currentStats.baseStats.realm
            });
        }

        return result.ToArray();
    }
}
public enum Position
    {
        Front1,
        Front2,
        Front3,
        Back1,
        Back2,
        Back3
    }
[System.Serializable]
public class PositionInLineup
{
    
    public Position position;
    public Character character;
}