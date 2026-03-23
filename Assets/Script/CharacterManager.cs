using UnityEngine;
using System.Linq;
using System.Collections;
[System.Serializable]
public class CharacterManager : MonoBehaviour, ITurnActor
{
    
    public HudSystem Hud;
    public CurrentStats Stats;
    [Header("Skills")]
    public SkillController Skills = new SkillController();
    void Start()
    {
        // nếu không thấy turnmanager thì không đăng ký
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.Register(this);
            Stats.InitializeStats();
            Hud.owner = this;
            Hud.SetUI(Stats.MHealth, Stats.ManaBonus, Stats.CurrentLevel, Stats.baseStats.characterName);
        }
        else
        {
            Hud.uiParent.gameObject.SetActive(false);
            Hud.NameText.gameObject.SetActive(false);
        }
    }

    // Đây là method thỏa interface ITurnActor
    public void OnMyTurn()
    {
        StartCoroutine(OnMyTurnCoroutine());
    }

    private IEnumerator OnMyTurnCoroutine()
    {
        Debug.Log($"{Stats.baseStats.characterName}'s turn!");

        // Lấy danh sách mục tiêu từ enemyTeam
        var enemies = Hud.isPlayer ? TurnManager.Instance.enemyTeam : TurnManager.Instance.playerTeam;
        enemies = enemies.Where(e => !e.Hud.isDead).ToList();

        if (enemies.Count == 0)
        {
            Debug.Log("No targets available.");
            yield break;
        }

        // Chọn mục tiêu gần nhất
        CharacterManager target = enemies.OrderBy(e => Vector3.Distance(transform.position, e.transform.position)).First();

        Vector3 originalPosition = transform.position; // lưu vị trí cũ

        if (Stats.baseStats.attackType == AttackType.Melee)
        {
            // Di chuyển tới mục tiêu
            yield return StartCoroutine(MoveToTarget(target.transform.position, 1f, 15f));

            // Tấn công
            yield return StartCoroutine(AttackTarget(target));

            // Lùi về vị trí cũ
            yield return StartCoroutine(MoveToTarget(originalPosition, 0f, 15f));
        }
        else if (Stats.baseStats.attackType == AttackType.Ranged)
        {
            // đứng yên → tấn công
            yield return StartCoroutine(AttackTarget(target));
        }
        yield return new WaitForSeconds(0.2f);
            // Kết thúc lượt
        TurnManager.Instance.EndTurn();
    }

    private IEnumerator MoveToTarget(Vector3 destination, float stopDistance, float moveSpeed = 5f)
    {
        // moveSpeed là tốc độ cố định (units/giây), có thể thay đổi để nhanh hơn hoặc chậm hơn
        while (Vector3.Distance(transform.position, destination) > stopDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);

            yield return null; // chờ frame tiếp theo
        }
    }


    private IEnumerator AttackTarget(CharacterManager target)
    {
        yield return StartCoroutine(Skills.UseSkill(this, target));
    }
    public void OnDeath()
    {
        if (Hud.isDead) return;

        Hud.Die();
    }
}
