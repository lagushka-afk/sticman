using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Unit : MonoBehaviour
{
    private int Damage;
    private float Speed;
    private float Range;
    private int CurrentHealth;
    private string Team;
    private RectTransform EnemyBase;
    private RectTransform TargetRect;
    private float LastAttackTime;
    private float AttackCooldown = 1f;
    private RectTransform MyRect;

    public void Init(int Dmg, float Spd, float Rng, int Hp, string Tm, RectTransform EnemyBaseTransform)
    {
        Damage = Dmg;
        Speed = Spd;
        Range = Rng;
        CurrentHealth = Hp;
        Team = Tm;
        EnemyBase = EnemyBaseTransform;
        MyRect = GetComponent<RectTransform>();
    }

    public string GetTeam() { return Team; }

    void Update()
    {
        FindTarget();
        Move();
        TryAttack();
    }

    void FindTarget()
    {
        Unit[] AllUnits = GameObject.FindObjectsOfType<Unit>();
        Miner[] AllMiners = GameObject.FindObjectsOfType<Miner>();
        TargetRect = null;
        float ClosestDistance = Range;

        foreach (Unit U in AllUnits)
        {
            if (U == null || U.gameObject == gameObject) continue;
            if (U.GetTeam() != Team)
            {
                float Dist = Mathf.Abs(MyRect.anchoredPosition.x - U.MyRect.anchoredPosition.x);
                if (Dist < ClosestDistance && Mathf.Abs(MyRect.anchoredPosition.y - U.MyRect.anchoredPosition.y) < 50f)
                {
                    ClosestDistance = Dist;
                    TargetRect = U.MyRect;
                }
            }
        }

        foreach (Miner M in AllMiners)
        {
            if (M == null || M.gameObject == gameObject) continue;
            if (M.GetTeam() != Team)
            {
                RectTransform MRect = M.GetComponent<RectTransform>();
                float Dist = Mathf.Abs(MyRect.anchoredPosition.x - MRect.anchoredPosition.x);
                if (Dist < ClosestDistance && Mathf.Abs(MyRect.anchoredPosition.y - MRect.anchoredPosition.y) < 50f)
                {
                    ClosestDistance = Dist;
                    TargetRect = MRect;
                }
            }
        }

        if (TargetRect == null) TargetRect = EnemyBase;
    }

    void Move()
    {
        if (TargetRect == null) return;

        Vector2 TargetPos;

        if (TargetRect == EnemyBase)
        {
            TargetPos = new Vector2(TargetRect.anchoredPosition.x, MyRect.anchoredPosition.y);
        }
        else
        {
            TargetPos = new Vector2(TargetRect.anchoredPosition.x, MyRect.anchoredPosition.y);
        }

        float Dist = Mathf.Abs(MyRect.anchoredPosition.x - TargetPos.x);

        if (Dist > Range)
        {
            float Dir = TargetPos.x > MyRect.anchoredPosition.x ? 1f : -1f;
            MyRect.anchoredPosition += new Vector2(Dir * Speed * Time.deltaTime * 100f, 0);
        }
    }

    void TryAttack()
    {
        if (TargetRect == null || Time.time - LastAttackTime < AttackCooldown) return;

        float Dist;

        if (TargetRect == EnemyBase)
        {
            Dist = Mathf.Abs(MyRect.anchoredPosition.x - EnemyBase.anchoredPosition.x);
        }
        else
        {
            Dist = Mathf.Abs(MyRect.anchoredPosition.x - TargetRect.anchoredPosition.x);
        }

        if (Dist <= Range)
        {
            LastAttackTime = Time.time;
            Unit EU = TargetRect.GetComponent<Unit>();
            if (EU != null) { EU.TakeDamage(Damage); return; }
            Miner EM = TargetRect.GetComponent<Miner>();
            if (EM != null) { EM.TakeDamage(Damage); return; }
            Base B = TargetRect.GetComponent<Base>();
            if (B != null) B.TakeDamage(Damage);
        }
    }

    public void TakeDamage(int Dmg)
    {
        CurrentHealth -= Dmg;
        StartCoroutine(Flash());
        if (CurrentHealth <= 0 && Game.Instance != null) Game.Instance.Remove(gameObject, "Unit");
    }

    IEnumerator Flash()
    {
        Image Img = GetComponent<Image>();
        if (Img != null) { Img.color = Color.red; yield return new WaitForSeconds(0.1f); Img.color = Color.white; }
    }

    void OnDestroy()
    {
        if (Game.Instance != null) Game.Instance.Remove(gameObject, "Unit");
    }
}