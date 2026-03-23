using UnityEngine;
using System.Linq;
public enum Realm
{ // đổi tên vương quốc sau khi có ý tưởng
    wind,
    glass,
    earth,
    electric,
}
// 
[CreateAssetMenu(fileName = "CharacterDatabase", menuName = "Game/CharacterDatabase")]
public class DataCharacter : ScriptableObject
{
    public TypeRealm[] typeRealms;
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
}

[System.Serializable]
public class TypeRealm
{
    public Realm realm;
    public CharacterBaseStats[] characterBaseStats;

}

