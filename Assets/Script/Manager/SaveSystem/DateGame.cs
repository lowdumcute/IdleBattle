using UnityEngine;
using System.Linq;
using System;
public enum Realm
{ // đổi tên vương quốc sau khi có ý tưởng
    wind,
    glass,
    earth,
    electric,
}
// 
[CreateAssetMenu(fileName = "DataGame", menuName = "Game/DataGame")]
public class DataGame : ScriptableObject
{
    public TypeRealm[] typeRealms;
    public InfoStar[] UIStar;
    public CharacterBaseStats GetCharacter(string characterID, Realm realm)
    {
        return typeRealms?
            .Where(t => t != null && t.realm == realm)
            .SelectMany(t => t.characterBaseStats)
            .FirstOrDefault(c =>
                c != null &&
                c.characterID.Equals(characterID, System.StringComparison.OrdinalIgnoreCase)
            );
    }
    public Sprite GetSpriteStar(StarType type)
    {
        var star = UIStar.FirstOrDefault(s => s.starType == type);

        if (star == null)
        {
            Debug.LogWarning($"Star sprite not found: {type}");
            return null;
        }

        return star.ImageStar;
    }
}

[System.Serializable]
public class TypeRealm
{
    public Realm realm;
    public CharacterBaseStats[] characterBaseStats;

}
public enum StarType
{
    Empty,
    Yellow,
    Red,
    Diamond
}
[System.Serializable]
public class InfoStar
{
    public StarType starType;
    public Sprite ImageStar;
}

