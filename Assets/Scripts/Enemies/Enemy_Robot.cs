using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Robot : Enemy
{
    [Header("추적")]
    [SerializeField] private float chaseTimeMax = 7f; // 추적 시간

    [Header("상태 전환")]
    [SerializeField] private int melee1Weight = 1;
    [SerializeField] private int melee2Weight = 1;
    [SerializeField] private int melee3Weight = 1;

    [SerializeField] private int melee4Weight = 1;
    [SerializeField] private List<Transform> melee4Pos = new List<Transform>();
    private int melee4lastPos = -1;

    private List<int> meleeTable = new List<int>();
    [SerializeField] [Range(0.0f, 1.0f)] private float chargeChance = 0.5f;
    private int chargeRepeat = 2;
    private int chargeRepeatCurr = 0;
    [SerializeField] [Range(0.0f, 1.0f)] private float crossChance = 0.5f;

    [SerializeField] [Range(0.0f, 1.0f)] private float phase2HP = 0.5f;
    [SerializeField] private float phase2SpeedUp = 0.2f;
    [SerializeField] private bool isPhase2 = false;

    [Header("MISC")]
    [SerializeField] private ArrowIndicator TEST_INDICATOR = null;
    [SerializeField] private float stepBackTime = 2.0f;

    // 각 공격 상태들
    private EnemyStateAttack melee1;
    private EnemyStateAttack melee2;
    private EnemyStateAttack melee3;

    private EnemyStateAttack project;
    private EnemyStateAttack cross;
    private EnemyStateAttack charge;

    private EnemyStateChargeAttack melee4_1;
    private EnemyStateMeleeBox melee4_2;

    protected override void OnAwake()
    {
        melee1 = (EnemyStateAttack)StateList[1];
        melee2 = (EnemyStateAttack)StateList[2];
        melee3 = (EnemyStateAttack)StateList[3];

        project = (EnemyStateAttack)StateList[4];
        cross = (EnemyStateAttack)StateList[5];
        charge = (EnemyStateAttack)StateList[6];

        melee4_1 = (EnemyStateChargeAttack)StateList[7];
        melee4_2 = (EnemyStateMeleeBox)StateList[8];

        for (int i = 0; i < melee1Weight; i++)
            meleeTable.Add(1);
        for (int i = 0; i < melee2Weight; i++)
            meleeTable.Add(2);
        for (int i = 0; i < melee3Weight; i++)
            meleeTable.Add(3);
        for (int i = 0; i < melee4Weight; i++)
            meleeTable.Add(7);

        SetDefaultState(0);
        Game.Instance.HPbar.Active(this);
    }

    protected override void OnUpdate()
    {
        if (!isPhase2)
        {
            if (phase2HP * hpMax >= hpCurr)
            {
                isPhase2 = true;
                chargeRepeat++;
                TrySetAnimFloat("AnimSpeed", 1.2f);
                for (int i = 1; i < StateList.Length-1; i++)
                {
                    EnemyStateAttack state = ((EnemyStateAttack)StateList[i]);
                    state.MultTiming(1 - phase2SpeedUp);
                    if (i == 5) // Cross
                    {
                        EnemyStateProjectile statep = (EnemyStateProjectile)state;
                        statep.ShotWays = 8;
                        statep.WayDiff = 45f;
                        statep.DiffOffset = 0f;
                    }
                }
            }
        }
        switch (StateCurrIdx)
        {
            case 0: // 추적
                if (melee1.CanAttackTarget())
                {
                    int next = meleeTable[Random.Range(0, meleeTable.Count)];
                    while(!isPhase2 && next == 7)
                    {
                        next = meleeTable[Random.Range(0, meleeTable.Count)];
                    }    
                    if (next == 7)
                    {
                        melee4lastPos = Random.Range(0, melee4Pos.Count);
                        melee4_1.SetDest(melee4Pos[melee4lastPos]);
                    }
                    SetState(next);
                    TrySetAnimTrigger("Melee1");
                }
                if (StateDuration > chaseTimeMax)
                {
                    if (Random.value <= chargeChance)
                    {
                        SetState(6);
                        TrySetAnimTrigger("Charge");
                    }
                    else
                    {
                        if (Random.value <= crossChance)
                            SetState(5);
                        else
                            SetState(4);
                    }
                }
                break;
            case 1: // 부채꼴 공격
                if (melee1.IsAttackOver())
                    SetState(9);
                break;
            case 2: // 내리치기
                if (melee2.IsAttackOver())
                    SetState(9);
                break;
            case 3: // 원형
                if (melee3.IsAttackOver())
                    SetState(9);
                break;
            case 4: // 발사체
                if (project.IsAttackOver())
                    SetState(0);
                break;
            case 5: // 십자
                if (cross.IsAttackOver())
                    SetState(9);
                break;
            case 6: // 돌진
                if (charge.IsAttacking())
                    TrySetAnimBool("Charging", true);
                else
                    TrySetAnimBool("Charging", false);

                if (charge.IsAttackOver())
                {
                    TrySetAnimBool("Charging", false);
                    chargeRepeatCurr = (chargeRepeatCurr + 1) % chargeRepeat;
                    if (chargeRepeatCurr == 0)
                    {
                        TrySetAnimTrigger("ChargeEnd");
                        SetState(0);
                    }
                    else
                        SetState(6);
                }
                break;
            case 7:
                if (melee4_1.IsAttackOver())
                {
                    Vector3 origin = transform.position;
                    origin.y += 0.3f;
                    Vector3 end = origin + transform.forward * melee4_2.Range;
                    TEST_INDICATOR.SetThickness(melee4_2.Width * 2f);
                    TEST_INDICATOR.StartFill(origin, end, melee4_2.DelayBefore);

                    int lockIndex = (melee4lastPos + (int)(melee4Pos.Count * 0.5f)) % melee4Pos.Count;
                    SetSightLock(melee4Pos[lockIndex].position);

                    SetState(8);
                }
                break;
            case 8:
                {
                    Vector3 origin = transform.position;
                    origin.y += 0.3f;
                    Vector3 end = origin + transform.forward * melee4_2.Range;
                    TEST_INDICATOR.ChangeVector(origin, end);
                    if (melee4_2.IsAttackOver())
                    {
                        ResetSightLock();
                        SetState(0);
                    }
                }
                break;
            case 9:
                if (StateDuration > stepBackTime)
                {
                    SetState(0);
                }
                break;
        }
    }
}