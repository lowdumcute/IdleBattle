using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BattleStageManager : MonoBehaviour
{
    public static BattleStageManager Instance;
    [Header("Settings")]
    public List<Transform> PlayerSpawnPoints;
    public List<Transform> EnemySpawnPoints;
    [Header("UI References")]
    public float MaxRound ;
    [SerializeField] private GameObject DamagePopupPrefab;

    void Awake()
    {
        // Singleton chuẩn
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void Start()
    {
        StartCoroutine(SetUp());
    }
    public void ShowDamagePopup(Vector3 worldPosition, float damageAmount)
    {
        if (DamagePopupPrefab == null) return;

        GameObject popup = Instantiate(DamagePopupPrefab, worldPosition, Quaternion.identity);
        var popupScript = popup.GetComponent<PopupDamageEffect>();
        if (popupScript != null)
        {
            popupScript.SetText(damageAmount.ToString());
        }
    }
    public IEnumerator SetUp()
    {
        // Xóa toàn bộ nhân vật cũ trong PlayerSpawnPoints
        foreach (Transform point in PlayerSpawnPoints)
        {
            for (int i = point.childCount - 1; i >= 0; i--)
            {
                Destroy(point.GetChild(i).gameObject);
            }
        }

        // Xóa toàn bộ nhân vật cũ trong EnemySpawnPoints
        foreach (Transform point in EnemySpawnPoints)
        {
            for (int i = point.childCount - 1; i >= 0; i--)
            {
                Destroy(point.GetChild(i).gameObject);
            }
        }

        // Spawn player theo thứ tự
        SpawnTeam(GameManager.Instance.playersTeam, PlayerSpawnPoints);

        // Spawn enemy theo thứ tự
        SpawnTeam(GameManager.Instance.enemiesTeam, EnemySpawnPoints);

        yield return new WaitForSeconds(1f);

        // Bắt đầu trận đấu
        TurnStageManager.Instance.GameStart();
    }

    private void SpawnTeam(List<Character> team, List<Transform> points)
    {
        int count = Mathf.Min(team.Count, points.Count); // số lượng spawn tối đa

        for (int i = 0; i < count; i++)
        {
            Character c = team[i];

            if (c.currentStats.baseStats.characterPrefab == null)
            {
                Debug.LogWarning("Character " + i + " không có prefab!");
                continue;
            }

            Transform point = points[i];

            // Spawn làm con của point, giữ vị trí localPosition = 0,0,0 để không đổi x,y,z
            GameObject obj = Instantiate(
                c.currentStats.baseStats.characterPrefab,
                point.position,
                point.rotation,
                point // parent là spawn point
            );

            // Set scale = 61
            obj.transform.localScale = Vector3.one * 61f;

            // Gán Stats cho CharacterManager
            CharacterManager cm = obj.GetComponent<CharacterManager>();
            if (cm != null)
            {
                cm.Stats = c.currentStats;
            }
            else
            {
                Debug.LogWarning("Prefab của " + c.currentStats.baseStats.characterName + " không có CharacterManager!");
            }
        }
    }
}
