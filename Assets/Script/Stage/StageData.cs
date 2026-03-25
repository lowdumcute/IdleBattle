using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StageData", menuName = "Game/Stage")]
public class StageData : ScriptableObject
{
    public List<Character> enemies;
}