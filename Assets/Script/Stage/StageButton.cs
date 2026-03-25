using UnityEngine;
using UnityEngine.SceneManagement;

public class StageButton : MonoBehaviour
{
    public StageData stageData;
    public string battleSceneName = "BattleScene";

    public void OnClickStage()
    {
        if (stageData == null)
        {
            Debug.LogWarning("StageData is null!");
            return;
        }

        // set enemy team
        GameManager.Instance.SetEnemyFromStage(stageData);

        // load scene battle
        SceneManager.LoadScene(battleSceneName);
    }
}