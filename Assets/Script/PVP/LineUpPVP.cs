using UnityEngine;
using System.Collections.Generic;

public class LineUpPVP : MonoBehaviour
{
    public static LineUpPVP Instance;

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
            result.Add(new NetworkCharacterData
            {
                characterID = c.currentStats.baseStats.characterID,
                level = (int)c.currentStats.CurrentLevel,
                star = c.star,
                realm = (int)c.currentStats.baseStats.realm
                
            });
        }

        return result.ToArray(); // 🔥 QUAN TRỌNG
    }
}