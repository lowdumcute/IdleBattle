using UnityEngine;
using System.Collections.Generic;

public class LineUp : MonoBehaviour
{
    public static LineUp Instance;

    public List<Character> myLineup = new List<Character>();

    private void Awake()
    {
        Instance = this;
    }

    // 🔥 dùng cho network
    public NetworkCharacterData[] GetNetworkLineup()
    {
        List<NetworkCharacterData> result = new List<NetworkCharacterData>();

        foreach (var c in myLineup)
        {
            if (c.currentStats == null)
            {
                Debug.LogError("currentStats NULL");
                continue;
            }

            if (c.currentStats.baseStats == null)
            {
                Debug.LogError("baseStats NULL");
                continue;
            }

            Debug.Log($"Lineup ID: {c.currentStats.baseStats.characterID}");

            result.Add(new NetworkCharacterData
            {
                characterID = c.currentStats.baseStats.characterID,
                level = (int)c.currentStats.CurrentLevel,
                star = c.star,
                realm = (int)c.currentStats.baseStats.realm
            });
        }

        return result.ToArray();
    }
}