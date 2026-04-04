using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem Instance;

    public enum Mode
    {
        Stage,
        PVP
    }

    public Mode currentMode;

    void Awake()
    {
        Instance = this;
    }

    public void Register(CharacterManager c)
    {
            TurnManager.Instance?.Register(c);
    }

    public List<CharacterManager> GetEnemies(TypeTeam myTeam)
    {
        var tm = TurnManager.Instance;

        return myTeam switch
        {
            TypeTeam.Player => tm.enemyTeam,
            TypeTeam.Enemy => tm.playerTeam,
            _ => new List<CharacterManager>()
        };
    }

    public void EndTurn()
    {
            TurnManager.Instance?.EndTurn();
    }
}