using UnityEngine; 
using System.Collections.Generic; 
public class Lineup : MonoBehaviour 
{ 
    public static Lineup Instance; 
    public LineupPanel lineupPanel; 
    public List<PositionInLineup> myLineup = new List<PositionInLineup>(); 
    private void Awake() 
    { 
        Instance = this; 
    } 
    // ================= ADD ================= 
    public void AddLineup(int character, Position position) 
    { 
        // Kiểm tra nếu đã có vị trí này trong đội hình, nếu có thì cập nhật lại 
        PositionInLineup existing = myLineup.Find(p => p.position == position); 
        if (existing != null) 
        { 
            existing.CharIndex = character; 
        } 
        else 
        { 
            myLineup.Add(new PositionInLineup { position = position, CharIndex = character }); 
        } 
    } 
    // ================= REMOVE ================= 
    public void RemoveLineup(Position position) 
    { 
        // Tìm và xóa vị trí trong đội hình 
        myLineup.RemoveAll(p => p.position == position); 
    } 
    // ================= NETWORK ================= 
    public NetworkCharacterData[] GetNetworkLineup() 
    { 
        List<NetworkCharacterData> result = new List<NetworkCharacterData>(); 
        foreach (var c in myLineup) 
        { 
            if (c.CharIndex < 0 || c.CharIndex >= CharacterInventory.Instance.ownedCharacters.Count) 
            { 
                Debug.LogWarning("CharIndex out of range"); 
                continue; 
            } 
            Character character = CharacterInventory.Instance.ownedCharacters[c.CharIndex]; 
            if (character == null || character.currentStats == null || character.currentStats.baseStats == null) 
            { 
                Debug.LogError("Character data NULL"); 
                continue; 
            } 
            Debug.Log($"Lineup ID: {character.currentStats.baseStats.characterID}"); 
            result.Add(new NetworkCharacterData 
            { 
                characterID = character.currentStats.baseStats.characterID, 
                level = (int)character.currentStats.CurrentLevel, 
                star = character.star, 
                realm = (int)character.currentStats.baseStats.realm, 
                position = (int)c.position
            }); 
        } 
        return result.ToArray(); 
    } 
} 
public enum Position { Front1, Front2, Front3, Back1, Back2, Back3 } 

[System.Serializable] 
public class PositionInLineup 
{ 
    public Position position; 
    public int CharIndex; // index trong inventory 
}