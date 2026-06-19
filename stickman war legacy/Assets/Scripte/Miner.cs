using UnityEngine;

public class Miner : MonoBehaviour
{
    private int Income;
    private float Interval;
    private string Team;
    private float Timer;

    public void Init(int GoldPerTick, float TickInterval, string UnitTeam)
    {
        Income = GoldPerTick;
        Interval = TickInterval;
        Team = UnitTeam;
    }

    public string GetTeam() { return Team; }

    void Update()
    {
        Timer += Time.deltaTime;
        if (Timer >= Interval)
        {
            Timer = 0;
            if (Game.Instance != null) Game.Instance.AddGold(Income, Team);
        }
    }

    public void TakeDamage(int Dmg)
    {
        if (Game.Instance != null) Game.Instance.Remove(gameObject, "Miner");
    }

    void OnDestroy()
    {
        if (Game.Instance != null) Game.Instance.Remove(gameObject, "Miner");
    }
}