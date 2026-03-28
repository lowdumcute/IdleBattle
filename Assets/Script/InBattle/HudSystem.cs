using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
public class HudSystem : MonoBehaviour
{
    [HideInInspector] public CharacterManager owner;
    [Header(" Stats")]
    private float maxHealth;
    private float maxMana = 100f;
    float currentHealth;
    [HideInInspector] public float currentMana;

    [Header("UI References")]
    public Image healthFill;
    public Image manaFill;
    public TMP_Text LevelText;
    public TMP_Text NameText;
    public RectTransform uiParent; // Rung khung Máu Mana

    private Vector3 originalParentPos;

    [Header("Type")]
    public bool isPlayer = false;
    public bool isDead = false;
    [HideInInspector]public Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        UpdateUI();
        ApplyFacingDirection();
        if (uiParent != null)
            originalParentPos = uiParent.localPosition;
    }

    // ==========================
    //       HEALTH LOGIC
    // ==========================
    public void SetUI(float health, float ManaBonus, float level, string Name)
    {
        maxHealth = health;
        currentHealth = maxHealth;
        currentMana += ManaBonus; // mốt sửa sau nếu cần
        LevelText.text = $"{level}";
        NameText.text = Name;
        UpdateUI();
    }
    public void RefreshFromStats(CurrentStats stats)
    {
        maxHealth = stats.baseStats.GetHealthByLevel(stats.CurrentLevel);
        currentHealth = stats.MHealth;
        UpdateUI();
    }
    public void TakeDamage(float amount)
    {
        animator.SetTrigger("Damaged");
        BattleStageManager.Instance.ShowDamagePopup(
            transform.position + Vector3.up * 2f,
            amount
        );

        RefreshFromStats(owner.Stats);

        StartCoroutine(ShakeUI());
    }

    // ==========================
    //        MANA LOGIC
    // ==========================
    public bool UseMana(float amount)
    {
        if (currentMana < amount)
            return false;

        currentMana -= amount;
        UpdateUI();
        return true;
    }

    public void RecoverMana(float amount)
    {
        currentMana = Mathf.Clamp(currentMana + amount, 0, maxMana);
        UpdateUI();
    }

    // ==========================
    //          UI UPDATE
    // ==========================
    void UpdateUI()
    {
        if (healthFill)
        {
            healthFill.fillAmount = currentHealth / maxHealth;
            healthFill.color = isPlayer ? Color.green : new Color(0.7f, 0, 0);
        }

        if (manaFill)
        {
            float mp = currentMana / maxMana;
            manaFill.fillAmount = mp;
            manaFill.color = Color.Lerp(Color.gray, Color.cyan, mp);
        }
    }

    // ==========================
    //        NATURAL SHAKE
    // ==========================
    IEnumerator ShakeUI()
    {
        if (uiParent == null)
            yield break;

        float duration = 0.1f;
        float strength = 0.05f;

        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            // Rung ngẫu nhiên theo 2 chiều → tự nhiên hơn
            float offsetX = Random.Range(-strength, strength);
            float offsetY = Random.Range(-strength * 0.5f, strength * 0.5f);

            uiParent.localPosition = originalParentPos + new Vector3(offsetX, offsetY, 0);

            yield return null;
        }

        // Trả về đúng vị trí ban đầu
        uiParent.localPosition = originalParentPos;
    }

    public void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        animator.SetTrigger("Death");
        animator.SetBool("isDeath", true);
        isDead = true;
        uiParent.gameObject.SetActive(false);
        LevelText.gameObject.SetActive(false);
        NameText.gameObject.SetActive(false);
    }
    void ApplyFacingDirection()
    {
        Vector3 s = transform.localScale;

        if (isPlayer)
            s.x = -Mathf.Abs(s.x);  // luôn âm
        else
            s.x = Mathf.Abs(s.x);   // luôn dương

        transform.localScale = s;
    }
}
