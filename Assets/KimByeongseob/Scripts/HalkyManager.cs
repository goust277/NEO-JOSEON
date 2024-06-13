using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HalkyManager : NewEnemy
{

    /*-------------------------보스 능력치 관련--------------------------*/
    //[Header("능력치")]
    //public int hp = 100;
    /*----------------------------------------------------------------------*/

    /*--------------------------원거리 공격 관련----------------------------*/
    [Header("원거리 공격")]
    public GameObject[] projectilePrefab;           // 투사체 프리팹
    public float projectileSpeed = 5f;    // 투사체 발사 속도
    public float L_initialDelay = 1.2f;  // 선딜
    public float L_endingDelay = 0.2f;  // 후딜
    public int damage = 3;     // 공격 데미지
    /*----------------------------------------------------------------------*/

    /*--------------------------근거리 공격 관련----------------------------*/
    [Header("근거리 공격")]
    public GameObject[] sangmoObject;                 // 상모 오브젝트
    public float slapTime = 0.1f;  // 후려치기 피격타임
    public float spinTime = 2f;    // 상모 돌리기 피격타임
    public float S_initialDelay = 0.7f;  // 선딜
    public float S_endingDelay = 1.2f;  // 후딜
    public float spinSpeed = 30f;  // 상모 돌리기 속도
    public int slapDamage = 5;     // 후려치기 데미지
    public int spinDamage = 4;     // 상모 돌리기 데미지
    /*----------------------------------------------------------------------*/

    /*----------------------------순간이동 관련-----------------------------*/
    [Header("순간이동")]
    public GameObject[] Janggu_pos;  // 장구 위치
    private int Recent_pos;          // 최근 이동한 위치
    public int teleportCounter = 0;  // 순간 이동 횟수 카운터
    /*----------------------------------------------------------------------*/

    /*-------------------------보스패턴(난타) 관련--------------------------*/
    [Header("난타")]
    public float B_initialDelay = 0.7f;  // 선딜
    public float B_runningDelay = 6f;    // 중딜
    public float B_endingDelay = 2.4f;  // 후딜
    public GameObject effectObject;                 // 몬스터에 들어있는 효과 오브젝트

    // 플레이어 주변에서 음파를 발사할 각도 범위 설정 (좌우 360도, 위쪽 180도)
    private float minAngle_Y = -180f;
    private float maxAngle_Y = 180f;
    private float minAngle_X = -90f;
    private float maxAngle_X = 0f;
    /*----------------------------------------------------------------------*/

    /*----------------------보스패턴(북치기 공격) 관련----------------------*/
    [Header("북치기")]
    public GameObject[] attackPrefab;     // 공격 프리팹
    public Transform[] B_attackPositions; // 공격 위치 배열
    public Transform center;              // 중심 위치
    public float attackSpeed;

    public Transform[] drumObjects;       // 미리 할당된 북 오브젝트
    public Transform[] attackPositions;   // 미리 할당된 공격 생성 위치

    public GameObject waterObject; // 물 프리팹
    public float riseSpeed = 1f;   // 물이 차오르는 속도
    private Vector3 waterInitialPosition;

    public int phase = 1;                 // 페이즈 (1 또는 2)

    /*----------------------------------------------------------------------*/

    /*------------------------보스패턴 알고리즘 관련------------------------*/
    [Header("알고리즘")]
    private bool isInvincible = false; // 무적 상태를 나타내는 변수
    public bool linoleumJanggu = true; // 장판 상태
    public bool patterning = false;
    /*----------------------------------------------------------------------*/

    /*---------------------------보스 피격 관련---------------------------*/
    [Header("머테리얼")]
    public GameObject Model;
    public Material newMaterial;
    public float duration;

    private Renderer[] renderers;
    private Material[] originalMaterials;

    [Header("체력바")]
    public GameObject HpBar;
    public Image ImageHp;

    [Header("보호막")]
    public Image shieldHpBar;  // 보호막 체력바 UI
    public float shieldMaxHp = 1000f;  // 보호막 최대 체력
    public float currentShieldHp;  // 현재 보호막 체력
    /*----------------------------------------------------------------------*/

    private Transform playerTransform;         // 플레이어 참조
    public Transform mapCenter;
    public Transform attackPos;
    public Animator animator; // 애니메이터 컴포넌트

    //20240602 추가
    [Header("사물놀이")]
    public GameObject shield;
    private bool hasUsedFirstPattern = false;
    private bool hasUsedSecondPattern = false;
    private bool onSamulNori = false;
    public GameObject bossPrefab; // 복제할 보스 프리팹
    public List<GameObject> clonedBosses = new List<GameObject>(); // 복제된 보스 리스트
    private PlayerMove playerMove; // 플레이어 이동 스크립트 참조


    [Header("이펙트")]
    public ParticleSystem effectOre;
    public ParticleSystem effectAttack1;
    public ParticleSystem effectAttack2;
    public ParticleSystem effectDrumAttack;
    public ParticleSystem effectNanta;
    public ParticleSystem effectTeleport;

    private SoundManager soundManager;
    private WaveManager waveManager;
    public GameObject waveObject;

    [Header("기타")]
    [SerializeField] private Image clearPanel;
    [SerializeField] private Image next;

    private void Awake()
    {
        renderers = Model.GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length];

        // 각 Renderer의 원래 Material을 저장합니다.
        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].material;
        }
    }
    void Start()
    {
        // 플레이어 참조
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerMove = playerTransform.GetComponent<PlayerMove>();
        waterInitialPosition = waterObject.transform.position;
        StartCoroutine(BossRoutineStart());
        soundManager = FindObjectOfType<SoundManager>();
        waveManager = waveObject.GetComponent<WaveManager>();

    }

    void Update()
    {
        //LookAtPlayer();

        if (Input.GetKeyDown(KeyCode.L))
        {
            StartCoroutine(ExecuteDrumAttack());
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            StartCoroutine(StartBatterAttack());
        }

        //if(Input.GetKeyDown(KeyCode.S))
        //{
        //    StartCoroutine(StartShortAttack(1));
        //}

        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    StartCoroutine(StartShortAttack(2));
        //}

        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(StartShortAttack(2));
        }
    }


    #region // 페이즈1

    // 몬스터가 플레이어 방향으로 회전
    private void LookAtPlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    void PlayRandomAttackSound()
    {
        string[] attackSounds = { "LongAttack_Hit_1", "LongAttack_Hit_2", "LongAttack_Hit_3" };
        int randomIndex = Random.Range(0, attackSounds.Length);
        soundManager.PlaySound(attackSounds[randomIndex]);
    }

    // 일반 패턴 [원거리 공격]
    IEnumerator StartLongAttack()
    {
        animator.SetTrigger("IsLongAttack");
        soundManager.PlaySound("LongAttack_Ready");
        yield return new WaitForSeconds(L_initialDelay);
        ShootSoundWave(Random.Range(1, 6));
        yield return new WaitForSeconds(L_endingDelay);
    }

    // 원거리 공격 1 ~ 5
    private void ShootSoundWave(int attackType)
    {
        GameObject projectile;
        Vector3 direction = ((playerTransform.position + new Vector3(0, 0.7f, 0)) - attackPos.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        PlayRandomAttackSound();
        switch (attackType)
        {
            case 1:
                // 공격 1: 부채꼴로 3개 발사
                for (int i = -1; i <= 1; i++)
                {
                    projectile = Instantiate(projectilePrefab[0], attackPos.transform.position, lookRotation * Quaternion.Euler(0, i * 45, 0));
                    projectile.GetComponent<Rigidbody>().velocity = (lookRotation * Quaternion.Euler(0, i * 45, 0)) * Vector3.forward * projectileSpeed;
                }
                break;
            case 2:
                // 공격 2: 직선으로 1개 발사
                projectile = Instantiate(projectilePrefab[1], attackPos.transform.position, lookRotation);
                projectile.GetComponent<Rigidbody>().velocity = lookRotation * Vector3.forward * projectileSpeed;
                break;
            case 3:
                // 공격 3: 십자모양으로 4개 발사
                for (int i = 0; i <= 3; i++)
                {
                    projectile = Instantiate(projectilePrefab[0], attackPos.transform.position, lookRotation * Quaternion.Euler(0, i * 90, 0));
                    projectile.GetComponent<Rigidbody>().velocity = (lookRotation * Quaternion.Euler(0, i * 90, 0)) * Vector3.forward * projectileSpeed;
                }
                break;
            case 4:
                // 공격 4: 45도 튼 십자모양으로 4개 발사
                for (int i = -3; i <= 3; i += 2)
                {
                    projectile = Instantiate(projectilePrefab[0], attackPos.transform.position, lookRotation * Quaternion.Euler(0, i * 45, 0));
                    projectile.GetComponent<Rigidbody>().velocity = (lookRotation * Quaternion.Euler(0, i * 45, 0)) * Vector3.forward * projectileSpeed;
                }
                break;
            case 5:
                // 공격 5: 역방향으로 5개 발사
                for (int i = 2; i <= 6; i++)
                {
                    projectile = Instantiate(projectilePrefab[0], attackPos.transform.position, lookRotation * Quaternion.Euler(0, i * 45, 0));
                    projectile.GetComponent<Rigidbody>().velocity = (lookRotation * Quaternion.Euler(0, i * 45, 0)) * Vector3.forward * projectileSpeed;
                }
                break;
            case 6:
                // 공격 6: 8방향으로 발사
                for (int i = 0; i < 8; i++)
                {
                    projectile = Instantiate(projectilePrefab[0], attackPos.transform.position, lookRotation * Quaternion.Euler(0, i * 45, 0));
                    projectile.GetComponent<Rigidbody>().velocity = (lookRotation * Quaternion.Euler(0, i * 45, 0)) * Vector3.forward * projectileSpeed;
                }
                break;
        }
    }

    // 일반 패턴 [근거리 공격]
    IEnumerator StartShortAttack(int attackType)
    {
        switch (attackType)
        {
            case 1:
                // 후려치기 공격
                animator.SetTrigger("IsShortAttack_1");
                yield return new WaitForSeconds(S_initialDelay);
                soundManager.PlaySound("Attack_1");
                LookAtPlayer();
                sangmoObject[0].SetActive(true);
                effectAttack1.Play();
                yield return new WaitForSeconds(slapTime);
                sangmoObject[0].SetActive(false);
                effectAttack1.Stop();
                yield return new WaitForSeconds(S_endingDelay);
                break;
            case 2:
                // 상모 돌리기 공격
                float elapsedTime = 0f;
                float toggleInterval = 0.2f; // 이펙트를 껐다 켰다 할 간
                LookAtPlayer();
                soundManager.StartLoopSound("Attack_2");
                animator.SetTrigger("IsShortAttack_2");
                yield return new WaitForSeconds(S_initialDelay);
                sangmoObject[1].SetActive(true);
                while (elapsedTime < spinTime)
                {
                    effectAttack2.Play();
                    yield return new WaitForSeconds(toggleInterval);
                    effectAttack2.Stop();
                    yield return new WaitForSeconds(toggleInterval);
                    elapsedTime += 2 * toggleInterval;
                }
                sangmoObject[1].SetActive(false);
                soundManager.StopLoopSound("Attack_2");
                yield return new WaitForSeconds(S_endingDelay);
                break;
        }
    }

    // 일반 패턴 [순간 이동]
    IEnumerator Teleporting(int Type = 0)
    {
        animator.SetTrigger("IsTeleportation");
        yield return new WaitForSeconds(0.8f);
        soundManager.PlaySound("Teleporting");
        effectTeleport.Play();
        yield return new WaitForSeconds(0.2f);
        effectTeleport.Stop();
        int randomIndex;
        if (Type == 0)
        {
            randomIndex = Random.Range(0, Janggu_pos.Length - 1);

            // 최근에 이동한 위치로는 이동하지 않도록 함
            while (randomIndex == Recent_pos)
            {
                randomIndex = Random.Range(0, Janggu_pos.Length - 1);
            }
        }
        else
        {
            randomIndex = 6;
        }
        
        if(linoleumJanggu)
        {
            Janggu_pos[Recent_pos].GetComponent<JangguManager>().upProjectile = true;
            Janggu_pos[Recent_pos].GetComponent<JangguManager>().tag = "Janggu";
            Janggu_pos[Recent_pos].GetComponent<JangguManager>().gameObject.layer = 0;
            Renderer drumRenderer = Janggu_pos[Recent_pos].GetComponent<Renderer>();
            drumRenderer.material.color = Color.white;

            Recent_pos = randomIndex;

            Janggu_pos[Recent_pos].GetComponent<JangguManager>().upProjectile = false;
            Janggu_pos[Recent_pos].GetComponent<JangguManager>().tag = "Plane";
            Janggu_pos[Recent_pos].GetComponent<JangguManager>().gameObject.layer = 9;
            drumRenderer = Janggu_pos[Recent_pos].GetComponent<Renderer>();
            drumRenderer.material.color = Color.black; // 비활성화
        }
        else
        {
            Recent_pos = randomIndex;
        }

        if (randomIndex != 6)
        {
            transform.position = Janggu_pos[randomIndex].transform.position + new Vector3(0.0f, 0.6f, 0.0f);
            teleportCounter++;
        }
        else
        {
            transform.position = Janggu_pos[randomIndex].transform.position + new Vector3(0.0f, 1f, 0.0f);
        }

        Vector3 direction = (mapCenter.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    // 특수 패턴 [난타 공격]
    IEnumerator StartBatterAttack()
    {
        effectObject.SetActive(true);
        isInvincible = true;
        shield.SetActive(true);
        yield return new WaitForSeconds(2f);
        animator.SetTrigger("IsBatterAttack");
        soundManager.StartLoopSound("Nanta");
        yield return new WaitForSeconds(B_initialDelay);

        // 5~6번 반복, 각 반복마다 6발씩 발사
        int totalShots = Random.Range(60, 72);

        // 총 발사 횟수만큼 반복
        for (int i = 0; i < totalShots; i++)
        {
            ShootSoundWaveBatter();

            float delay = B_runningDelay / totalShots;
            effectNanta.Play();
            yield return new WaitForSeconds(delay);
            effectNanta.Stop();
        }
        soundManager.StopLoopSound("Nanta");
        isInvincible = false;
        shield.SetActive(false);
        effectObject.SetActive(false);
        animator.SetTrigger("EndBatterAttack");
        yield return new WaitForSeconds(B_endingDelay);
        animator.SetTrigger("EndGrogi");
    }

    private void ShootSoundWaveBatter()
    {
        // 랜덤한 각도 선택
        float randomAngle_Y = Random.Range(minAngle_Y, maxAngle_Y);
        float randomAngle_X = Random.Range(minAngle_X, maxAngle_X);

        // 선택한 각도로 음파 발사
        Vector3 direction = Quaternion.Euler(randomAngle_X, randomAngle_Y, 0f) * Vector3.forward;
        GameObject projectile = Instantiate(projectilePrefab[0], attackPos.transform.position, Quaternion.LookRotation(direction));
        projectile.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
    }

    // 큰 북치기 공격
    IEnumerator ExecuteAttack()
    {
        animator.SetTrigger("IsDrumAttack");
        yield return new WaitForSeconds(1.8f);
        soundManager.PlaySound("Drum");
        ExecuteAttack_();
        yield return new WaitForSeconds(1.2f);
    }
    void ExecuteAttack_()
    {
        // 랜덤으로 공격 위치 선택
        int randomIndex = Random.Range(0, B_attackPositions.Length);
        Transform selectedPosition = B_attackPositions[randomIndex];

        // 공격 프리팹 생성
        GameObject attack = Instantiate(attackPrefab[0], selectedPosition.position, Quaternion.identity);

        // 중심을 향해 방향 맞추기
        Vector3 direction = (center.position - selectedPosition.position).normalized;
        attack.transform.LookAt(center);
        //attack.transform.Rotate(0, 0, 90);

        // 중심을 향해 공격 이동
        attack.GetComponent<Rigidbody>().velocity = direction * attackSpeed;
        Destroy(attack, 7f);
    }


    // 작은 북치기 공격
    IEnumerator ExecuteDrumAttack()
    {
        LookAtPlayer();
        animator.SetTrigger("IsDrumAttack");
        yield return new WaitForSeconds(1.3f);
        soundManager.PlaySound("Drum");
        effectDrumAttack.Play();
        yield return new WaitForSeconds(0.5f);
        effectDrumAttack.Stop();

        StartCoroutine(ExecuteDrumAttackPattern());
        yield return new WaitForSeconds(1.2f);
    }
    IEnumerator ExecuteDrumAttackPattern()
    {
        int numDrums = phase == 1 ? 3 : 5;
        int[] chosenDrums = new int[numDrums];
        bool[] drumUsed = new bool[drumObjects.Length];

        for (int i = 0; i < numDrums; i++)
        {
            int drumIndex;
            do
            {
                drumIndex = Random.Range(0, drumObjects.Length);
            } while (drumUsed[drumIndex]);

            drumUsed[drumIndex] = true;
            chosenDrums[i] = drumIndex;

            // 각각의 드럼과 공격을 동시에 실행
            StartCoroutine(HandleDrumAttack(drumIndex));
        }

        yield return null; // 패턴이 끝난 후 코루틴 종료
    }

    IEnumerator HandleDrumAttack(int drumIndex)
    {
        // 북을 0.1초 동안 빛나게 하는 부분 (예: Material 색상 변경)
        StartCoroutine(FlashDrum(drumObjects[drumIndex]));

        yield return new WaitForSeconds(1f);

        // 공격 범위 출력
        int attackPrefabIndex = drumIndex == 0 ? 2 : 1; // 북 0번이면 attackPrefabs[2], 그 외는 attackPrefabs[1]
        GameObject attackInstance = Instantiate(attackPrefab[attackPrefabIndex], attackPositions[drumIndex].position, attackPositions[drumIndex].rotation);
        StartCoroutine(DestroyAttackAfterDelay(attackInstance, 0.3f));
    }

    IEnumerator FlashDrum(Transform drum)
    {
        // 예: 드럼의 Material 색상 변경으로 빛나게 하기
        Renderer drumRenderer = drum.GetComponent<Renderer>();
        Color originalColor = drumRenderer.material.color;
        drumRenderer.material.color = Color.red; // 빛나는 색상

        yield return new WaitForSeconds(1f);

        drumRenderer.material.color = originalColor; // 원래 색상으로 복원
    }

    IEnumerator DestroyAttackAfterDelay(GameObject attackInstance, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(attackInstance);
    }

    IEnumerator BossRoutineStart()
    {
        yield return StartCoroutine(Teleporting());
        StartCoroutine(BossRoutine());
    }

    // 보스 루틴
    IEnumerator BossRoutine()
    {
        // 10초 동안 트리거가 없거나 플레이어가 일정 거리 이상 다가오면 다음 패턴으로 이동
        yield return StartCoroutine(CheckPlayerDistanceOrTimeout(10f, 10f)); // 5f는 플레이어와의 거리 임계값
        if (!patterning)
        {
            float randomValue = Random.Range(0f, 1f);
            if (randomValue <= 0.4f)
            {
                yield return StartCoroutine(StartShortAttack(Random.Range(1, 3)));
            }
            else
            {
                if (linoleumJanggu)
                    yield return StartCoroutine(ExecuteDrumAttack());
                else
                    yield return StartCoroutine(ExecuteAttack());
            }
        }

        if (!patterning)
        {
            // 순간 이동 횟수 체크
            if (teleportCounter % 3 == 0 && teleportCounter != 0)
            {
                yield return StartCoroutine(Teleporting(1));
                yield return StartCoroutine(StartBatterAttack());
                yield return StartCoroutine(Teleporting());
                if (!patterning)
                {
                    if (teleportCounter % 2 == 0 && teleportCounter != 0)
                    {
                        if (linoleumJanggu)
                        {
                            yield return StartCoroutine(ExecuteDrumAttack());
                            StartCoroutine(BossRoutine());
                        }
                        else
                        {
                            yield return StartCoroutine(ExecuteAttack());
                            StartCoroutine(BossRoutine());
                        }
                    }
                    else
                    {
                        StartCoroutine(BossRoutine());
                    }
                }

            }
            else
            {
                yield return StartCoroutine(Teleporting());
                if (!patterning)
                {
                    if (teleportCounter % 2 == 0 && teleportCounter != 0)
                    {
                        if (linoleumJanggu)
                        {
                            yield return StartCoroutine(ExecuteDrumAttack());
                            StartCoroutine(BossRoutine());
                        }
                        else
                        {
                            yield return StartCoroutine(ExecuteAttack());
                            StartCoroutine(BossRoutine());
                        }
                    }
                    else
                    {
                        StartCoroutine(BossRoutine());
                    }
                }
            }
        }
    }

    IEnumerator CheckPlayerDistanceOrTimeout(float timeout, float distanceThreshold, int num = 2)
    {
        float timer = 0f;
        while (timer < timeout)
        {
            if (Vector3.Distance(transform.position, playerTransform.position) <= distanceThreshold)
            {
                yield break; // 플레이어가 일정 거리 내로 들어오면 코루틴 종료
            }
            timer += Time.deltaTime;

            // 원거리 공격 2연속 실행
            for (int i = 0; i < num; i++)
            {
                LookAtPlayer();
                yield return StartCoroutine(StartLongAttack());
                timer += L_initialDelay + L_endingDelay;
            }

            yield return new WaitForSeconds(2f); // 2초 간격

            timer += 2f; // 2초 간격으로 타이머 증가
        }
    }
    public void ChangeMaterialsTemporarily()
    {
        // 각 Renderer의 Material을 새 Material로 변경합니다.
        foreach (Renderer renderer in renderers)
        {
            renderer.material = newMaterial;
        }

        // 일정 시간 후에 원래 Material로 복원합니다.
        Invoke("ChangeMaterialsBack", duration);
    }

    private void ChangeMaterialsBack()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = originalMaterials[i];
        }
    }
    public override void TakeDamage(int damage)
    {
        if (!isInvincible)
        {
            if (!onSamulNori)
            {
                ChangeMaterialsTemporarily();
                int tempDmgNum = damage * Random.Range(15, 30);
                currentHp -= tempDmgNum;
                if (currentHp <= 0) currentHp = 0;
                float remainingHpPercentage = Mathf.Round(((float)currentHp / (float)maxHp) * 100f) / 100f;
                ImageHp.fillAmount = remainingHpPercentage;
                ChangeMaterialsTemporarily();

                if (remainingHpPercentage <= 0.66f && !hasUsedFirstPattern && phase == 1 ||
                    remainingHpPercentage <= 0.33f && !hasUsedFirstPattern && phase == 2)
                {
                    hasUsedFirstPattern = true;
                    isInvincible = true;
                    shield.SetActive(true);
                    linoleumJanggu = false;
                    patterning = true;
                    StartCoroutine(TriggerPattern("만조"));
                    
                }
                else if (remainingHpPercentage <= 0.33f && !hasUsedSecondPattern && phase == 1 ||
                         remainingHpPercentage <= 0.66f && !hasUsedSecondPattern && phase == 2)
                {
                    hasUsedSecondPattern = true;
                    isInvincible = true;
                    shield.SetActive(true);
                    linoleumJanggu = true;
                    patterning = true;
                    StartCoroutine(TriggerPattern("북 장판"));
                    waveManager.waveType = 0;
                    waveObject.SetActive(false);
                }
                else if (remainingHpPercentage <= 0 && phase == 1)
                {
                    isInvincible = true;
                    shield.SetActive(true);
                    hasUsedFirstPattern = false;
                    hasUsedSecondPattern = false;
                    phase = 2;
                    linoleumJanggu = false;
                    patterning = true;
                    StartCoroutine(Phase2());
                }
                else if (remainingHpPercentage <= 0 && phase == 2)
                {
                    StopAllCoroutines(); // 모든 코루틴 종료
                    isInvincible = true;
                    effectOre.Stop(true);
                    soundManager.PlaySound("Die");
                    animator.SetTrigger("DoDie");

                    FadeOut(clearPanel);
                    Invoke("ClearSet", 3f);
                    HpBar.SetActive(false);
                    
                }

                Debug.Log(currentHp);
            }
            else
            {
                int tempDmgNum = damage * Random.Range(15, 30);
                currentShieldHp -= tempDmgNum;
                if (currentShieldHp <= 0) currentShieldHp = 0;
                float remainingHpPercentage = Mathf.Round(((float)currentShieldHp / (float)shieldMaxHp) * 100f) / 100f;
                shieldHpBar.fillAmount = remainingHpPercentage;
                ChangeMaterialsTemporarily();

                if (currentShieldHp <= 0)
                {
                    onSamulNori = false;  // 사물놀이 패턴 비활성화
                    shieldHpBar.gameObject.SetActive(false);  // 보호막 체력바 비활성화
                    shieldHpBar.fillAmount = 100f;
                    currentShieldHp = shieldMaxHp;
                    StopAllCoroutines(); // 모든 코루틴 종료
                    StartCoroutine(ActivateClonedBosses_OFF());
                }
            }
            Debug.Log("데미지 입음");
        }
    }

    private void BackMain()
    {
        SceneManager.LoadScene("Main");
    }
    private void ClearSet()
    {
        clearPanel.gameObject.SetActive(false);

        FadeOut(next);
        Invoke("BackMain", 7f);
    }
    private IEnumerator Phase2()
    {
        animator.SetTrigger("IsScream");
        effectOre.Play(true);
        yield return new WaitForSeconds(3f);
        currentHp = maxHp;
        ImageHp.fillAmount = 100f;
        StartCoroutine(TriggerPattern("만조"));
    }

    private IEnumerator TriggerPattern(string pattern)
    {
        yield return new WaitForSeconds(1f);
        soundManager.PlaySound("Felid");
        if (pattern == "만조")
        {
            yield return StartCoroutine(ExecuteNorthPlatePattern());
        }
        else if (pattern == "북 장판")
        {
            yield return StartCoroutine(ExecuteManjoPattern());
        }
        yield return StartCoroutine(Teleporting());
        shield.SetActive(false);
        isInvincible = false;
        patterning = false;

        StopAllCoroutines(); // 모든 코루틴 종료

        if (phase == 1)
            StartCoroutine(BossRoutine()); // 패턴1 처음부터 다시 시작
        else if (phase == 2)
            StartCoroutine(BossRoutine2()); // 패턴2 처음부터 다시 시작
    }

    private IEnumerator ExecuteNorthPlatePattern()
    {
        animator.SetTrigger("IsLongAttack");
        yield return new WaitForSeconds(L_initialDelay);
        ShootSoundWave(6);
        yield return new WaitForSeconds(L_endingDelay);


        yield return StartCoroutine(RaiseWater());
    }

    private IEnumerator ExecuteManjoPattern()
    {
        animator.SetTrigger("IsLongAttack");
        yield return new WaitForSeconds(L_initialDelay);
        ShootSoundWave(6);
        yield return new WaitForSeconds(L_endingDelay);

        yield return StartCoroutine(LowerWater());
    }

    private IEnumerator RaiseWater()
    {
        HighTideEffect();
        SetAllJangguState(true);
        Vector3 endPosition = new Vector3(waterInitialPosition.x, waterInitialPosition.y + 1.21f, waterInitialPosition.z);
        while (Mathf.Abs(waterObject.transform.position.y - endPosition.y) > 0.01f)
        {
            waterObject.transform.position = Vector3.MoveTowards(waterObject.transform.position, endPosition, riseSpeed * Time.deltaTime);
            yield return null;
        }
        waterObject.transform.position = endPosition;
        waveObject.SetActive(true);
        waveManager.waveType = Random.Range(1, 3);
    }

    private IEnumerator LowerWater()
    {
        SetAllJangguState(false);
        while (Mathf.Abs(waterObject.transform.position.y - waterInitialPosition.y) > 0.01f)
        {
            waterObject.transform.position = Vector3.MoveTowards(waterObject.transform.position, waterInitialPosition, riseSpeed * Time.deltaTime);
            yield return null;
        }
        waterObject.transform.position = waterInitialPosition;
    }
    void SetAllJangguState(bool isActive)
    {
        for (int i = 0; i < Janggu_pos.Length; i++)
        {
            var jangguManager = Janggu_pos[i].GetComponent<JangguManager>();
            if (jangguManager == null) continue;

            jangguManager.upProjectile = !isActive;
            jangguManager.tag = isActive ? "Plane" : "Janggu";
            jangguManager.gameObject.layer = isActive ? 9 : 0;

            Renderer drumRenderer = Janggu_pos[i].GetComponent<Renderer>();
            if (drumRenderer != null)
            {
                drumRenderer.material.color = isActive ? Color.black : Color.white;
            }
        }
    }
    #endregion

    #region // 페이즈2

    // 사물놀이 패턴
    private IEnumerator StartSamulNoriPattern()
    {
        onSamulNori = true;
        SetAllJangguState(true);
        shieldHpBar.gameObject.SetActive(true);  // 보호막 체력바 활성화

        // 6개의 큰 북을 쳐서 플레이어를 중앙으로 강제 이동
        isInvincible = true;
        shield.SetActive(true);
        animator.SetTrigger("IsDrumAttack");
        yield return new WaitForSeconds(1.3f);
        effectDrumAttack.Play();
        yield return new WaitForSeconds(0.5f);
        effectDrumAttack.Stop();
        isInvincible = false;
        shield.SetActive(false);

        // 플레이어를 맵 중앙으로 이동시키고 가둡니다
        playerTransform.position = mapCenter.position;

        // 플레이어 움직임 제한
        playerMove.SetLimitMove(true);

        // 미리 생성해둔 클론 보스들 사용
        ActivateClonedBosses();

        // 잠시 대기 후 가두고 있던 오브젝트 제거
        yield return new WaitForSeconds(2f);
        playerMove.SetLimitMove(false);

        // 사물놀이 패턴 실행
        yield return StartCoroutine(SamulNoriAttackPhase());

        SetAllJangguState(false);
    }

    // 클론 보스 활성화 및 설정
    private void ActivateClonedBosses()
    {
        // 본체를 작은 북 위치로 이동
        transform.position = Janggu_pos[0].transform.position + new Vector3(0.0f, 0.6f, 0.0f);
        transform.LookAt(mapCenter);

        // 나머지 5마리의 미리 생성된 보스 설정
        for (int i = 1; i < clonedBosses.Count; i++)
        {
            clonedBosses[i].transform.position = Janggu_pos[i].transform.position + new Vector3(0.0f, 0.6f, 0.0f);
            clonedBosses[i].transform.LookAt(mapCenter);
            clonedBosses[i].SetActive(true); // 클론 보스 활성화
        }
    }

    private IEnumerator ActivateClonedBosses_OFF()
    {
        // 본체를 작은 북 위치로 이동
        transform.position = Janggu_pos[0].transform.position + new Vector3(0.0f, 0.6f, 0.0f);
        transform.LookAt(mapCenter);

        // 나머지 5마리의 미리 생성된 보스 설정
        for (int i = 1; i < clonedBosses.Count; i++)
        {
            clonedBosses[i].SetActive(false); // 클론 보스 활성화
        }

        animator.SetTrigger("IsGrogi");
        yield return new WaitForSeconds(2.4f);
        animator.SetTrigger("EndGrogi");
        yield return StartCoroutine(Teleporting());
        StartCoroutine(BossRoutine2());
    }

    private IEnumerator SamulNoriAttackPhase()
    {
        float elapsedTime = 0f;
        float totalDuration = 7f;  // 전체 회전 시간
        float attackInterval = 2f;
        int totalAttacks = -1;  // 초기값을 -1로 설정하여 최초 2초 동안 공격을 생략
        SetAllAnimatorsBool("StartSamulTurn");

        yield return new WaitForSeconds(1.2f);

        // 초기 위치와 각도 저장
        Vector3[] startPositions = new Vector3[clonedBosses.Count];
        float[] startAngles = new float[clonedBosses.Count];
        for (int i = 0; i < clonedBosses.Count; i++)
        {
            startPositions[i] = clonedBosses[i].transform.position;  // 현재 위치 저장
            Vector3 direction = (startPositions[i] - mapCenter.position).normalized;
            startAngles[i] = Mathf.Atan2(direction.z, direction.x);  // 초기 각도 계산
        }

        bool allReturnedToStart = false;
        soundManager.StartLoopSound("Spin");
        // 회전 실행
        while (elapsedTime < totalDuration && !allReturnedToStart)
        {
            float rotationSpeed = 2 * Mathf.PI / totalDuration;  // 전체 회전을 위한 각속도
            float currentAngle = rotationSpeed * elapsedTime;  // 현재 각도 계산

            allReturnedToStart = true;

            for (int i = 0; i < clonedBosses.Count; i++)
            {
                float angle = startAngles[i] + currentAngle;  // 각 클론의 현재 각도
                float radius = Vector3.Distance(startPositions[i], mapCenter.position);  // 중앙에서의 거리 계산
                Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                clonedBosses[i].transform.position = mapCenter.position + offset;
                clonedBosses[i].transform.LookAt(mapCenter);

                //// 보스가 원래 포지션으로 돌아왔는지 확인
                //if (Vector3.Distance(clonedBosses[i].transform.position, startPositions[i]) > 0.5f)
                //{
                //    SetAllAnimatorsBool("EndSamulTurn");
                //}

                // 보스가 원래 포지션으로 돌아왔는지 확인
                if (Vector3.Distance(clonedBosses[i].transform.position, startPositions[i]) > 0.1f)
                {
                    allReturnedToStart = false;
                }
            }

            if (totalAttacks >= 0 && elapsedTime >= attackInterval * totalAttacks)
            {
                // 6마리 중 랜덤한 3마리가 원거리 공격
                List<int> selectedBosses = new List<int>();
                while (selectedBosses.Count < 3)
                {
                    int randomBossIndex = Random.Range(0, clonedBosses.Count);
                    if (!selectedBosses.Contains(randomBossIndex))
                    {
                        selectedBosses.Add(randomBossIndex);
                    }
                }

                foreach (int bossIndex in selectedBosses)
                {
                    int randomAttackType = Random.Range(1, 6); // 원거리 공격 유형 (1~6)
                    ShootSoundWave(randomAttackType, clonedBosses[bossIndex].transform);
                }

                totalAttacks++;
            }
            else if (totalAttacks < 0)
            {
                totalAttacks++;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        soundManager.StopLoopSound("Spin");
        SetAllAnimatorsBool("EndSamulTurn");


        // 정확히 각자의 북 위치에서 멈추도록 설정
        for (int i = 0; i < clonedBosses.Count; i++)
        {
            clonedBosses[i].transform.position = Janggu_pos[i].transform.position + new Vector3(0.0f, 0.6f, 0.0f);
        }

        // IDLE 상태로 전환
        yield return new WaitForSeconds(1.5f);

        // 근접 공격
        for (int i = 1; i < 6; i++)
        {
            int randomAttackType = Random.Range(1, 3); // 근접 공격 유형 (1 또는 2)
            HalkyCloneManager cloneScript = clonedBosses[i].GetComponent<HalkyCloneManager>();
            if (cloneScript != null)
            {
                StartCoroutine(cloneScript.StartShortAttack(randomAttackType));
            }
        }

        yield return new WaitForSeconds(2.4f);

        // 다시 패턴 반복
        yield return StartCoroutine(SamulNoriAttackPhase());
    }


    // 원거리 공격
    private void ShootSoundWave(int attackType, Transform attackPosition)
    {
        GameObject projectile;
        Vector3 spawnPosition = attackPosition.position + new Vector3(0, 1.3f, 0);
        Vector3 direction = ((playerTransform.position + new Vector3(0, 0.7f, 0)) - attackPosition.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        PlayRandomAttackSound();
        switch (attackType)
        {
            case 1:
                // 공격 1: 부채꼴로 3개 발사
                for (int i = -1; i <= 1; i++)
                {
                    projectile = Instantiate(projectilePrefab[0], spawnPosition, lookRotation * Quaternion.Euler(0, i * 45, 0));
                    projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * projectileSpeed;
                }
                break;
            case 2:
                // 공격 2: 직선으로 1개 발사
                projectile = Instantiate(projectilePrefab[1], spawnPosition, lookRotation);
                projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * projectileSpeed;
                break;
            case 3:
                // 공격 3: 십자모양으로 4개 발사
                for (int i = 0; i <= 3; i++)
                {
                    projectile = Instantiate(projectilePrefab[0], spawnPosition, lookRotation * Quaternion.Euler(0, i * 90, 0));
                    projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * projectileSpeed;
                }
                break;
            case 4:
                // 공격 4: 45도 튼 십자모양으로 4개 발사
                for (int i = -3; i <= 3; i += 2)
                {
                    projectile = Instantiate(projectilePrefab[0], spawnPosition, lookRotation * Quaternion.Euler(0, i * 45, 0));
                    projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * projectileSpeed;
                }
                break;
            case 5:
                // 공격 5: 역방향으로 5개 발사
                for (int i = 2; i <= 6; i++)
                {
                    projectile = Instantiate(projectilePrefab[0], spawnPosition, lookRotation * Quaternion.Euler(0, i * 45, 0));
                    projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * projectileSpeed;
                }
                break;
            case 6:
                // 공격 6: 8방향으로 발사
                for (int i = 0; i < 8; i++)
                {
                    projectile = Instantiate(projectilePrefab[0], spawnPosition, lookRotation * Quaternion.Euler(0, i * 45, 0));
                    projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * projectileSpeed;
                }
                break;
        }
    }

    // 애니메이터 Bool 값 설정
    private void SetAllAnimatorsBool(string parameter)
    {
        foreach (var clone in clonedBosses)
        {
            Animator cloneAnimator = clone.GetComponentInChildren<Animator>();
            if (cloneAnimator != null)
            {
                cloneAnimator.SetTrigger(parameter);
            }
        }
    }

    // 쉴드 활성화 코루틴
    private IEnumerator ActivateShield()
    {
        if (shield != null)
        {
            shield.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            shield.SetActive(false);
        }
    }

    // 보스 루틴
    IEnumerator BossRoutine2()
    {
        // 10초 동안 트리거가 없거나 플레이어가 일정 거리 이상 다가오면 다음 패턴으로 이동
        yield return StartCoroutine(CheckPlayerDistanceOrTimeout(8f, 10f, 3)); // 5f는 플레이어와의 거리 임계값
        if (!patterning)
        {
            float randomValue = Random.Range(0f, 1f);
            if (randomValue <= 0.4f)
            {
                yield return StartCoroutine(StartShortAttack(Random.Range(1, 3)));
            }
            else
            {
                if (linoleumJanggu)
                    yield return StartCoroutine(ExecuteDrumAttack());
                else
                    yield return StartCoroutine(ExecuteAttack());
            }
        }

        if (!patterning)
        {
            // 순간 이동 횟수 체크
            if (teleportCounter % 3 == 0 && teleportCounter != 0)
            {
                yield return StartCoroutine(StartSamulNoriPattern());
                yield return StartCoroutine(Teleporting());
                if (!patterning)
                {
                    if (teleportCounter % 2 == 0 && teleportCounter != 0)
                    {
                        if (linoleumJanggu)
                        {
                            yield return StartCoroutine(ExecuteDrumAttack());
                            StartCoroutine(BossRoutine2());
                        }
                        else
                        {
                            yield return StartCoroutine(ExecuteAttack());
                            StartCoroutine(BossRoutine2());
                        }
                    }
                    else
                    {
                        StartCoroutine(BossRoutine2());
                    }
                }

            }
            else
            {
                yield return StartCoroutine(Teleporting());
                if (!patterning)
                {
                    if (teleportCounter % 2 == 0 && teleportCounter != 0)
                    {
                        if (linoleumJanggu)
                        {
                            yield return StartCoroutine(ExecuteDrumAttack());
                            yield return StartCoroutine(ExecuteDrumAttack());
                            StartCoroutine(BossRoutine2());
                        }
                        else
                        {
                            yield return StartCoroutine(ExecuteAttack());
                            yield return StartCoroutine(ExecuteAttack());
                            StartCoroutine(BossRoutine2());
                        }
                    }
                    else
                    {
                        StartCoroutine(BossRoutine2());
                    }
                }
            }
        }
    }

    private void  HighTideEffect()
    {
        for (int i = 0; i < drumObjects.Length; i++)
        {
            //귀찮아서 그냥 attackPrefab에 물 올라오는 이펙트 넣음.
            GameObject attackInstance = Instantiate(attackPrefab[3], Janggu_pos[i].transform.position, attackPositions[i].rotation);
            Destroy(attackInstance, 2f);
        }
    }

    #endregion

    public void FadeOut(Image image)
    {
        StartCoroutine(Fade(image, 0f, 1f));
    }

    IEnumerator Fade(Image image, float startAlpha, float targetAlpha)
    {
        float startTime = Time.time;
        Color color = image.color;

        while (Time.time < startTime + 3f)
        {
            float t = (Time.time - startTime) / 3f;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            image.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        image.color = color;

        yield return new WaitForSeconds(2f);

        startTime = Time.time;
        color = image.color;

        while (Time.time < startTime + 1.5f)
        {
            float t = (Time.time - startTime) / 1.5f;
            color.a = Mathf.Lerp(targetAlpha, startAlpha, t);
            image.color = color;
            yield return null;
        }

        color.a = startAlpha;
        image.color = color;
    }
}