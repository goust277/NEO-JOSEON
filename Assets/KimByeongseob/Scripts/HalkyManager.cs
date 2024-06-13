using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HalkyManager : NewEnemy
{

    /*-------------------------���� �ɷ�ġ ����--------------------------*/
    //[Header("�ɷ�ġ")]
    //public int hp = 100;
    /*----------------------------------------------------------------------*/

    /*--------------------------���Ÿ� ���� ����----------------------------*/
    [Header("���Ÿ� ����")]
    public GameObject[] projectilePrefab;           // ����ü ������
    public float projectileSpeed = 5f;    // ����ü �߻� �ӵ�
    public float L_initialDelay = 1.2f;  // ����
    public float L_endingDelay = 0.2f;  // �ĵ�
    public int damage = 3;     // ���� ������
    /*----------------------------------------------------------------------*/

    /*--------------------------�ٰŸ� ���� ����----------------------------*/
    [Header("�ٰŸ� ����")]
    public GameObject[] sangmoObject;                 // ��� ������Ʈ
    public float slapTime = 0.1f;  // �ķ�ġ�� �ǰ�Ÿ��
    public float spinTime = 2f;    // ��� ������ �ǰ�Ÿ��
    public float S_initialDelay = 0.7f;  // ����
    public float S_endingDelay = 1.2f;  // �ĵ�
    public float spinSpeed = 30f;  // ��� ������ �ӵ�
    public int slapDamage = 5;     // �ķ�ġ�� ������
    public int spinDamage = 4;     // ��� ������ ������
    /*----------------------------------------------------------------------*/

    /*----------------------------�����̵� ����-----------------------------*/
    [Header("�����̵�")]
    public GameObject[] Janggu_pos;  // �屸 ��ġ
    private int Recent_pos;          // �ֱ� �̵��� ��ġ
    public int teleportCounter = 0;  // ���� �̵� Ƚ�� ī����
    /*----------------------------------------------------------------------*/

    /*-------------------------��������(��Ÿ) ����--------------------------*/
    [Header("��Ÿ")]
    public float B_initialDelay = 0.7f;  // ����
    public float B_runningDelay = 6f;    // �ߵ�
    public float B_endingDelay = 2.4f;  // �ĵ�
    public GameObject effectObject;                 // ���Ϳ� ����ִ� ȿ�� ������Ʈ

    // �÷��̾� �ֺ����� ���ĸ� �߻��� ���� ���� ���� (�¿� 360��, ���� 180��)
    private float minAngle_Y = -180f;
    private float maxAngle_Y = 180f;
    private float minAngle_X = -90f;
    private float maxAngle_X = 0f;
    /*----------------------------------------------------------------------*/

    /*----------------------��������(��ġ�� ����) ����----------------------*/
    [Header("��ġ��")]
    public GameObject[] attackPrefab;     // ���� ������
    public Transform[] B_attackPositions; // ���� ��ġ �迭
    public Transform center;              // �߽� ��ġ
    public float attackSpeed;

    public Transform[] drumObjects;       // �̸� �Ҵ�� �� ������Ʈ
    public Transform[] attackPositions;   // �̸� �Ҵ�� ���� ���� ��ġ

    public GameObject waterObject; // �� ������
    public float riseSpeed = 1f;   // ���� �������� �ӵ�
    private Vector3 waterInitialPosition;

    public int phase = 1;                 // ������ (1 �Ǵ� 2)

    /*----------------------------------------------------------------------*/

    /*------------------------�������� �˰��� ����------------------------*/
    [Header("�˰���")]
    private bool isInvincible = false; // ���� ���¸� ��Ÿ���� ����
    public bool linoleumJanggu = true; // ���� ����
    public bool patterning = false;
    /*----------------------------------------------------------------------*/

    /*---------------------------���� �ǰ� ����---------------------------*/
    [Header("���׸���")]
    public GameObject Model;
    public Material newMaterial;
    public float duration;

    private Renderer[] renderers;
    private Material[] originalMaterials;

    [Header("ü�¹�")]
    public GameObject HpBar;
    public Image ImageHp;

    [Header("��ȣ��")]
    public Image shieldHpBar;  // ��ȣ�� ü�¹� UI
    public float shieldMaxHp = 1000f;  // ��ȣ�� �ִ� ü��
    public float currentShieldHp;  // ���� ��ȣ�� ü��
    /*----------------------------------------------------------------------*/

    private Transform playerTransform;         // �÷��̾� ����
    public Transform mapCenter;
    public Transform attackPos;
    public Animator animator; // �ִϸ����� ������Ʈ

    //20240602 �߰�
    [Header("�繰����")]
    public GameObject shield;
    private bool hasUsedFirstPattern = false;
    private bool hasUsedSecondPattern = false;
    private bool onSamulNori = false;
    public GameObject bossPrefab; // ������ ���� ������
    public List<GameObject> clonedBosses = new List<GameObject>(); // ������ ���� ����Ʈ
    private PlayerMove playerMove; // �÷��̾� �̵� ��ũ��Ʈ ����


    [Header("����Ʈ")]
    public ParticleSystem effectOre;
    public ParticleSystem effectAttack1;
    public ParticleSystem effectAttack2;
    public ParticleSystem effectDrumAttack;
    public ParticleSystem effectNanta;
    public ParticleSystem effectTeleport;

    private SoundManager soundManager;
    private WaveManager waveManager;
    public GameObject waveObject;

    [Header("��Ÿ")]
    [SerializeField] private Image clearPanel;
    [SerializeField] private Image next;

    private void Awake()
    {
        renderers = Model.GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length];

        // �� Renderer�� ���� Material�� �����մϴ�.
        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].material;
        }
    }
    void Start()
    {
        // �÷��̾� ����
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


    #region // ������1

    // ���Ͱ� �÷��̾� �������� ȸ��
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

    // �Ϲ� ���� [���Ÿ� ����]
    IEnumerator StartLongAttack()
    {
        animator.SetTrigger("IsLongAttack");
        soundManager.PlaySound("LongAttack_Ready");
        yield return new WaitForSeconds(L_initialDelay);
        ShootSoundWave(Random.Range(1, 6));
        yield return new WaitForSeconds(L_endingDelay);
    }

    // ���Ÿ� ���� 1 ~ 5
    private void ShootSoundWave(int attackType)
    {
        GameObject projectile;
        Vector3 direction = ((playerTransform.position + new Vector3(0, 0.7f, 0)) - attackPos.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        PlayRandomAttackSound();
        switch (attackType)
        {
            case 1:
                // ���� 1: ��ä�÷� 3�� �߻�
                for (int i = -1; i <= 1; i++)
                {
                    projectile = Instantiate(projectilePrefab[0], attackPos.transform.position, lookRotation * Quaternion.Euler(0, i * 45, 0));
                    projectile.GetComponent<Rigidbody>().velocity = (lookRotation * Quaternion.Euler(0, i * 45, 0)) * Vector3.forward * projectileSpeed;
                }
                break;
            case 2:
                // ���� 2: �������� 1�� �߻�
                projectile = Instantiate(projectilePrefab[1], attackPos.transform.position, lookRotation);
                projectile.GetComponent<Rigidbody>().velocity = lookRotation * Vector3.forward * projectileSpeed;
                break;
            case 3:
                // ���� 3: ���ڸ������ 4�� �߻�
                for (int i = 0; i <= 3; i++)
                {
                    projectile = Instantiate(projectilePrefab[0], attackPos.transform.position, lookRotation * Quaternion.Euler(0, i * 90, 0));
                    projectile.GetComponent<Rigidbody>().velocity = (lookRotation * Quaternion.Euler(0, i * 90, 0)) * Vector3.forward * projectileSpeed;
                }
                break;
            case 4:
                // ���� 4: 45�� ư ���ڸ������ 4�� �߻�
                for (int i = -3; i <= 3; i += 2)
                {
                    projectile = Instantiate(projectilePrefab[0], attackPos.transform.position, lookRotation * Quaternion.Euler(0, i * 45, 0));
                    projectile.GetComponent<Rigidbody>().velocity = (lookRotation * Quaternion.Euler(0, i * 45, 0)) * Vector3.forward * projectileSpeed;
                }
                break;
            case 5:
                // ���� 5: ���������� 5�� �߻�
                for (int i = 2; i <= 6; i++)
                {
                    projectile = Instantiate(projectilePrefab[0], attackPos.transform.position, lookRotation * Quaternion.Euler(0, i * 45, 0));
                    projectile.GetComponent<Rigidbody>().velocity = (lookRotation * Quaternion.Euler(0, i * 45, 0)) * Vector3.forward * projectileSpeed;
                }
                break;
            case 6:
                // ���� 6: 8�������� �߻�
                for (int i = 0; i < 8; i++)
                {
                    projectile = Instantiate(projectilePrefab[0], attackPos.transform.position, lookRotation * Quaternion.Euler(0, i * 45, 0));
                    projectile.GetComponent<Rigidbody>().velocity = (lookRotation * Quaternion.Euler(0, i * 45, 0)) * Vector3.forward * projectileSpeed;
                }
                break;
        }
    }

    // �Ϲ� ���� [�ٰŸ� ����]
    IEnumerator StartShortAttack(int attackType)
    {
        switch (attackType)
        {
            case 1:
                // �ķ�ġ�� ����
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
                // ��� ������ ����
                float elapsedTime = 0f;
                float toggleInterval = 0.2f; // ����Ʈ�� ���� �״� �� ��
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

    // �Ϲ� ���� [���� �̵�]
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

            // �ֱٿ� �̵��� ��ġ�δ� �̵����� �ʵ��� ��
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
            drumRenderer.material.color = Color.black; // ��Ȱ��ȭ
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

    // Ư�� ���� [��Ÿ ����]
    IEnumerator StartBatterAttack()
    {
        effectObject.SetActive(true);
        isInvincible = true;
        shield.SetActive(true);
        yield return new WaitForSeconds(2f);
        animator.SetTrigger("IsBatterAttack");
        soundManager.StartLoopSound("Nanta");
        yield return new WaitForSeconds(B_initialDelay);

        // 5~6�� �ݺ�, �� �ݺ����� 6�߾� �߻�
        int totalShots = Random.Range(60, 72);

        // �� �߻� Ƚ����ŭ �ݺ�
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
        // ������ ���� ����
        float randomAngle_Y = Random.Range(minAngle_Y, maxAngle_Y);
        float randomAngle_X = Random.Range(minAngle_X, maxAngle_X);

        // ������ ������ ���� �߻�
        Vector3 direction = Quaternion.Euler(randomAngle_X, randomAngle_Y, 0f) * Vector3.forward;
        GameObject projectile = Instantiate(projectilePrefab[0], attackPos.transform.position, Quaternion.LookRotation(direction));
        projectile.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
    }

    // ū ��ġ�� ����
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
        // �������� ���� ��ġ ����
        int randomIndex = Random.Range(0, B_attackPositions.Length);
        Transform selectedPosition = B_attackPositions[randomIndex];

        // ���� ������ ����
        GameObject attack = Instantiate(attackPrefab[0], selectedPosition.position, Quaternion.identity);

        // �߽��� ���� ���� ���߱�
        Vector3 direction = (center.position - selectedPosition.position).normalized;
        attack.transform.LookAt(center);
        //attack.transform.Rotate(0, 0, 90);

        // �߽��� ���� ���� �̵�
        attack.GetComponent<Rigidbody>().velocity = direction * attackSpeed;
        Destroy(attack, 7f);
    }


    // ���� ��ġ�� ����
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

            // ������ �巳�� ������ ���ÿ� ����
            StartCoroutine(HandleDrumAttack(drumIndex));
        }

        yield return null; // ������ ���� �� �ڷ�ƾ ����
    }

    IEnumerator HandleDrumAttack(int drumIndex)
    {
        // ���� 0.1�� ���� ������ �ϴ� �κ� (��: Material ���� ����)
        StartCoroutine(FlashDrum(drumObjects[drumIndex]));

        yield return new WaitForSeconds(1f);

        // ���� ���� ���
        int attackPrefabIndex = drumIndex == 0 ? 2 : 1; // �� 0���̸� attackPrefabs[2], �� �ܴ� attackPrefabs[1]
        GameObject attackInstance = Instantiate(attackPrefab[attackPrefabIndex], attackPositions[drumIndex].position, attackPositions[drumIndex].rotation);
        StartCoroutine(DestroyAttackAfterDelay(attackInstance, 0.3f));
    }

    IEnumerator FlashDrum(Transform drum)
    {
        // ��: �巳�� Material ���� �������� ������ �ϱ�
        Renderer drumRenderer = drum.GetComponent<Renderer>();
        Color originalColor = drumRenderer.material.color;
        drumRenderer.material.color = Color.red; // ������ ����

        yield return new WaitForSeconds(1f);

        drumRenderer.material.color = originalColor; // ���� �������� ����
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

    // ���� ��ƾ
    IEnumerator BossRoutine()
    {
        // 10�� ���� Ʈ���Ű� ���ų� �÷��̾ ���� �Ÿ� �̻� �ٰ����� ���� �������� �̵�
        yield return StartCoroutine(CheckPlayerDistanceOrTimeout(10f, 10f)); // 5f�� �÷��̾���� �Ÿ� �Ӱ谪
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
            // ���� �̵� Ƚ�� üũ
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
                yield break; // �÷��̾ ���� �Ÿ� ���� ������ �ڷ�ƾ ����
            }
            timer += Time.deltaTime;

            // ���Ÿ� ���� 2���� ����
            for (int i = 0; i < num; i++)
            {
                LookAtPlayer();
                yield return StartCoroutine(StartLongAttack());
                timer += L_initialDelay + L_endingDelay;
            }

            yield return new WaitForSeconds(2f); // 2�� ����

            timer += 2f; // 2�� �������� Ÿ�̸� ����
        }
    }
    public void ChangeMaterialsTemporarily()
    {
        // �� Renderer�� Material�� �� Material�� �����մϴ�.
        foreach (Renderer renderer in renderers)
        {
            renderer.material = newMaterial;
        }

        // ���� �ð� �Ŀ� ���� Material�� �����մϴ�.
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
                    StartCoroutine(TriggerPattern("����"));
                    
                }
                else if (remainingHpPercentage <= 0.33f && !hasUsedSecondPattern && phase == 1 ||
                         remainingHpPercentage <= 0.66f && !hasUsedSecondPattern && phase == 2)
                {
                    hasUsedSecondPattern = true;
                    isInvincible = true;
                    shield.SetActive(true);
                    linoleumJanggu = true;
                    patterning = true;
                    StartCoroutine(TriggerPattern("�� ����"));
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
                    StopAllCoroutines(); // ��� �ڷ�ƾ ����
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
                    onSamulNori = false;  // �繰���� ���� ��Ȱ��ȭ
                    shieldHpBar.gameObject.SetActive(false);  // ��ȣ�� ü�¹� ��Ȱ��ȭ
                    shieldHpBar.fillAmount = 100f;
                    currentShieldHp = shieldMaxHp;
                    StopAllCoroutines(); // ��� �ڷ�ƾ ����
                    StartCoroutine(ActivateClonedBosses_OFF());
                }
            }
            Debug.Log("������ ����");
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
        StartCoroutine(TriggerPattern("����"));
    }

    private IEnumerator TriggerPattern(string pattern)
    {
        yield return new WaitForSeconds(1f);
        soundManager.PlaySound("Felid");
        if (pattern == "����")
        {
            yield return StartCoroutine(ExecuteNorthPlatePattern());
        }
        else if (pattern == "�� ����")
        {
            yield return StartCoroutine(ExecuteManjoPattern());
        }
        yield return StartCoroutine(Teleporting());
        shield.SetActive(false);
        isInvincible = false;
        patterning = false;

        StopAllCoroutines(); // ��� �ڷ�ƾ ����

        if (phase == 1)
            StartCoroutine(BossRoutine()); // ����1 ó������ �ٽ� ����
        else if (phase == 2)
            StartCoroutine(BossRoutine2()); // ����2 ó������ �ٽ� ����
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

    #region // ������2

    // �繰���� ����
    private IEnumerator StartSamulNoriPattern()
    {
        onSamulNori = true;
        SetAllJangguState(true);
        shieldHpBar.gameObject.SetActive(true);  // ��ȣ�� ü�¹� Ȱ��ȭ

        // 6���� ū ���� �ļ� �÷��̾ �߾����� ���� �̵�
        isInvincible = true;
        shield.SetActive(true);
        animator.SetTrigger("IsDrumAttack");
        yield return new WaitForSeconds(1.3f);
        effectDrumAttack.Play();
        yield return new WaitForSeconds(0.5f);
        effectDrumAttack.Stop();
        isInvincible = false;
        shield.SetActive(false);

        // �÷��̾ �� �߾����� �̵���Ű�� ���Ӵϴ�
        playerTransform.position = mapCenter.position;

        // �÷��̾� ������ ����
        playerMove.SetLimitMove(true);

        // �̸� �����ص� Ŭ�� ������ ���
        ActivateClonedBosses();

        // ��� ��� �� ���ΰ� �ִ� ������Ʈ ����
        yield return new WaitForSeconds(2f);
        playerMove.SetLimitMove(false);

        // �繰���� ���� ����
        yield return StartCoroutine(SamulNoriAttackPhase());

        SetAllJangguState(false);
    }

    // Ŭ�� ���� Ȱ��ȭ �� ����
    private void ActivateClonedBosses()
    {
        // ��ü�� ���� �� ��ġ�� �̵�
        transform.position = Janggu_pos[0].transform.position + new Vector3(0.0f, 0.6f, 0.0f);
        transform.LookAt(mapCenter);

        // ������ 5������ �̸� ������ ���� ����
        for (int i = 1; i < clonedBosses.Count; i++)
        {
            clonedBosses[i].transform.position = Janggu_pos[i].transform.position + new Vector3(0.0f, 0.6f, 0.0f);
            clonedBosses[i].transform.LookAt(mapCenter);
            clonedBosses[i].SetActive(true); // Ŭ�� ���� Ȱ��ȭ
        }
    }

    private IEnumerator ActivateClonedBosses_OFF()
    {
        // ��ü�� ���� �� ��ġ�� �̵�
        transform.position = Janggu_pos[0].transform.position + new Vector3(0.0f, 0.6f, 0.0f);
        transform.LookAt(mapCenter);

        // ������ 5������ �̸� ������ ���� ����
        for (int i = 1; i < clonedBosses.Count; i++)
        {
            clonedBosses[i].SetActive(false); // Ŭ�� ���� Ȱ��ȭ
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
        float totalDuration = 7f;  // ��ü ȸ�� �ð�
        float attackInterval = 2f;
        int totalAttacks = -1;  // �ʱⰪ�� -1�� �����Ͽ� ���� 2�� ���� ������ ����
        SetAllAnimatorsBool("StartSamulTurn");

        yield return new WaitForSeconds(1.2f);

        // �ʱ� ��ġ�� ���� ����
        Vector3[] startPositions = new Vector3[clonedBosses.Count];
        float[] startAngles = new float[clonedBosses.Count];
        for (int i = 0; i < clonedBosses.Count; i++)
        {
            startPositions[i] = clonedBosses[i].transform.position;  // ���� ��ġ ����
            Vector3 direction = (startPositions[i] - mapCenter.position).normalized;
            startAngles[i] = Mathf.Atan2(direction.z, direction.x);  // �ʱ� ���� ���
        }

        bool allReturnedToStart = false;
        soundManager.StartLoopSound("Spin");
        // ȸ�� ����
        while (elapsedTime < totalDuration && !allReturnedToStart)
        {
            float rotationSpeed = 2 * Mathf.PI / totalDuration;  // ��ü ȸ���� ���� ���ӵ�
            float currentAngle = rotationSpeed * elapsedTime;  // ���� ���� ���

            allReturnedToStart = true;

            for (int i = 0; i < clonedBosses.Count; i++)
            {
                float angle = startAngles[i] + currentAngle;  // �� Ŭ���� ���� ����
                float radius = Vector3.Distance(startPositions[i], mapCenter.position);  // �߾ӿ����� �Ÿ� ���
                Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                clonedBosses[i].transform.position = mapCenter.position + offset;
                clonedBosses[i].transform.LookAt(mapCenter);

                //// ������ ���� ���������� ���ƿԴ��� Ȯ��
                //if (Vector3.Distance(clonedBosses[i].transform.position, startPositions[i]) > 0.5f)
                //{
                //    SetAllAnimatorsBool("EndSamulTurn");
                //}

                // ������ ���� ���������� ���ƿԴ��� Ȯ��
                if (Vector3.Distance(clonedBosses[i].transform.position, startPositions[i]) > 0.1f)
                {
                    allReturnedToStart = false;
                }
            }

            if (totalAttacks >= 0 && elapsedTime >= attackInterval * totalAttacks)
            {
                // 6���� �� ������ 3������ ���Ÿ� ����
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
                    int randomAttackType = Random.Range(1, 6); // ���Ÿ� ���� ���� (1~6)
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


        // ��Ȯ�� ������ �� ��ġ���� ���ߵ��� ����
        for (int i = 0; i < clonedBosses.Count; i++)
        {
            clonedBosses[i].transform.position = Janggu_pos[i].transform.position + new Vector3(0.0f, 0.6f, 0.0f);
        }

        // IDLE ���·� ��ȯ
        yield return new WaitForSeconds(1.5f);

        // ���� ����
        for (int i = 1; i < 6; i++)
        {
            int randomAttackType = Random.Range(1, 3); // ���� ���� ���� (1 �Ǵ� 2)
            HalkyCloneManager cloneScript = clonedBosses[i].GetComponent<HalkyCloneManager>();
            if (cloneScript != null)
            {
                StartCoroutine(cloneScript.StartShortAttack(randomAttackType));
            }
        }

        yield return new WaitForSeconds(2.4f);

        // �ٽ� ���� �ݺ�
        yield return StartCoroutine(SamulNoriAttackPhase());
    }


    // ���Ÿ� ����
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
                // ���� 1: ��ä�÷� 3�� �߻�
                for (int i = -1; i <= 1; i++)
                {
                    projectile = Instantiate(projectilePrefab[0], spawnPosition, lookRotation * Quaternion.Euler(0, i * 45, 0));
                    projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * projectileSpeed;
                }
                break;
            case 2:
                // ���� 2: �������� 1�� �߻�
                projectile = Instantiate(projectilePrefab[1], spawnPosition, lookRotation);
                projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * projectileSpeed;
                break;
            case 3:
                // ���� 3: ���ڸ������ 4�� �߻�
                for (int i = 0; i <= 3; i++)
                {
                    projectile = Instantiate(projectilePrefab[0], spawnPosition, lookRotation * Quaternion.Euler(0, i * 90, 0));
                    projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * projectileSpeed;
                }
                break;
            case 4:
                // ���� 4: 45�� ư ���ڸ������ 4�� �߻�
                for (int i = -3; i <= 3; i += 2)
                {
                    projectile = Instantiate(projectilePrefab[0], spawnPosition, lookRotation * Quaternion.Euler(0, i * 45, 0));
                    projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * projectileSpeed;
                }
                break;
            case 5:
                // ���� 5: ���������� 5�� �߻�
                for (int i = 2; i <= 6; i++)
                {
                    projectile = Instantiate(projectilePrefab[0], spawnPosition, lookRotation * Quaternion.Euler(0, i * 45, 0));
                    projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * projectileSpeed;
                }
                break;
            case 6:
                // ���� 6: 8�������� �߻�
                for (int i = 0; i < 8; i++)
                {
                    projectile = Instantiate(projectilePrefab[0], spawnPosition, lookRotation * Quaternion.Euler(0, i * 45, 0));
                    projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * projectileSpeed;
                }
                break;
        }
    }

    // �ִϸ����� Bool �� ����
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

    // ���� Ȱ��ȭ �ڷ�ƾ
    private IEnumerator ActivateShield()
    {
        if (shield != null)
        {
            shield.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            shield.SetActive(false);
        }
    }

    // ���� ��ƾ
    IEnumerator BossRoutine2()
    {
        // 10�� ���� Ʈ���Ű� ���ų� �÷��̾ ���� �Ÿ� �̻� �ٰ����� ���� �������� �̵�
        yield return StartCoroutine(CheckPlayerDistanceOrTimeout(8f, 10f, 3)); // 5f�� �÷��̾���� �Ÿ� �Ӱ谪
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
            // ���� �̵� Ƚ�� üũ
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
            //�����Ƽ� �׳� attackPrefab�� �� �ö���� ����Ʈ ����.
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