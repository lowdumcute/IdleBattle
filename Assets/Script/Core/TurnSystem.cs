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
        if (currentMode == Mode.PVP)
            TurnPVPManager.Instance?.Register(c);
        else
            TurnStageManager.Instance?.Register(c);
    }

    public List<CharacterManager> GetEnemies(bool isPlayer)
    {
        if (currentMode == Mode.PVP)
            return isPlayer 
                ? TurnPVPManager.Instance.enemyTeam 
                : TurnPVPManager.Instance.playerTeam;

        return isPlayer 
            ? TurnStageManager.Instance.enemyTeam 
            : TurnStageManager.Instance.playerTeam;
    }

    public void EndTurn()
    {
        if (currentMode == Mode.PVP)
            TurnPVPManager.Instance?.EndTurn();
        else
            TurnStageManager.Instance?.EndTurn();
    }
}