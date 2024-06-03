using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class NewBoss1 : NewEnemy
{
    [Header("보스 능력치")]
    public float speedMove;
    public float rotationSpeed;

    [Header("보스 상태")]
    public bool bIsDie;
    public bool bIsChase; // 지금 사용을 안하고 있음
    public bool isAct;
    public bool bIsRotate;
    private bool bPhase2 = false; // 현재 보스 페이즈가 1인지 2인지 

    [Header("머테리얼")]
    public GameObject Model;
    public Material newMaterial;
    public float duration;

    private Renderer[] renderers;
    private Material[] originalMaterials;

    [Header("애니메이터")]
    public Animator anim;

    private Transform player;
    private NavMeshAgent nav;
    private Coroutine nowCoroutine;
    private Rigidbody rb;
    public Collider collider;


    [Header("공격 관련")]
    [SerializeField] private bool bDetectPlayer;
    [SerializeField] private bool isSearchingPlayer;
    [SerializeField] private float searchTimer;
    public float targetRadius; 
    public float targetRange;
    public Transform[] positionAttack4;
    public GameObject attackArea1; 
    public GameObject attackArea2; 
    public GameObject attackArea3;
    public GameObject attackArea4;
    public GameObject slashObj1; 
    public GameObject slashObj2;
    public GameObject attackAreaRush;

    [Header("이펙트")]
    public ParticleSystem particleAttack1;
    public ParticleSystem particleAttack2;
    public ParticleSystem particleAttack3;
    public ParticleSystem particleAttack4_Start;
    public ParticleSystem particleAttack4_End;
    public ParticleSystem particleDash;
    public ParticleSystem particlePhase2;

    [Header("체력바")]
    public GameObject HpBar;
    public Image ImageHp;


    [Header("도깨비불 / 몬스터 생성 / 기타")]
    public MonsterCreator monsterCreator;
    public GoblinFireCreator goblinFireCreator;
    public GameObject ClearStep;
    public GameObject[] DestroyRoad;
    public Image image_ClearPanel;
    public GameObject image_HpPanel;
    public CapsuleCollider colliderMine;

    private int dashCnt;

    private void Awake()
    {
        // #. 네비게이션 관련 정보 가져오기
        player = GameObject.FindGameObjectWithTag("Player").transform;
        nav = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        renderers = Model.GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length];

        // 각 Renderer의 원래 Material을 저장합니다.
        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].material;
        }

        // #. 추격 관련 설정
        BossMoveStart();
    }

    private void Update()
    {
        nav.SetDestination(player.position);
        RotateTowardsPlayer();

        if (!isAct && !bIsDie)
        {
            if(currentHp <= maxHp * 0.6 && !bPhase2)
            {
                bPhase2 = true;
                Phase2Start();
            }

            if (!isSearchingPlayer)
            {
                searchTimer += Time.deltaTime;
                if (searchTimer >= 4f)
                {
                    BossMoveStop();
                    RandomAttackSelect_Failed();

                    searchTimer = 0f;
                    isSearchingPlayer = false;
                }
            }

            bDetectPlayer = CheckTargetInRange();
            if (bDetectPlayer)
            {
                BossMoveStop();
                RandomAttackSelect_Detect();

                searchTimer = 0f;
                isSearchingPlayer = false;
            }
        }



        if (Input.GetKeyDown(KeyCode.L))
        {
            Attack_4();
        }
    }


    // #. 랜덤 공격 실행 - 감지
    public void RandomAttackSelect_Detect()
    {
        int ranNum = bPhase2 ? Random.Range(0, 16) : Random.Range(0, 12);
        if (ranNum <= 3) Attack_1();
        else if (ranNum > 3 && ranNum <= 4) Attack_2();
        else if (ranNum > 4 && ranNum <= 6) Attack_3();
        else if (ranNum > 6 && ranNum <= 7) Slash_1();
        else if (ranNum > 7 && ranNum <= 9) Slash_2();
        else if (ranNum > 9 && ranNum <= 13) Rush();
        else if (ranNum > 13) Attack_4();
    }

    // #. 랜덤 공격 실행 - 감지 실패
    public void RandomAttackSelect_Failed()
    {
        int ranNum = Random.Range(6, 10);
        if (ranNum <= 2) Slash_1();
        else if (ranNum > 2 && ranNum <= 5) Slash_2();
        else if (ranNum > 5) Rush();
    }


    #region // 공격 관련 함수들


    // #. 공격 1
    public void Attack_1()
    {
        if (!isAct)
        {
            isAct = true;
            if (nowCoroutine != null) StopCoroutine(nowCoroutine);
            StartCoroutine(Attack_1__());
        }
    }
    IEnumerator Attack_1__()
    {
        anim.SetTrigger("Attack_1");

        yield return new WaitForSeconds(1.0f);
        particleAttack1.Play();
        attackArea1.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        particleAttack1.Stop();
        attackArea1.SetActive(false);
        yield return new WaitForSeconds(1.2f);
        isAct = false;
        BossMoveStart();
    }


    // #. 공격 2
    public void Attack_2()
    {
        if (!isAct)
        {
            isAct = true;
            if (nowCoroutine != null) StopCoroutine(nowCoroutine);
            StartCoroutine(Attack_2__());
        }
    }
    IEnumerator Attack_2__()
    {
        anim.SetTrigger("Attack_2");

        yield return new WaitForSeconds(1.5f);
        particleAttack2.Play();
        attackArea2.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        particleAttack2.Stop();
        attackArea2.SetActive(false);
        yield return new WaitForSeconds(1.2f);
        particleAttack2.Play();
        attackArea2.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        particleAttack2.Stop();
        attackArea2.SetActive(false);
        yield return new WaitForSeconds(2.0f);
        isAct = false;
        BossMoveStart();
    }


    // #. 공격 3
    public void Attack_3()
    {
        if (!isAct)
        {
            isAct = true;
            if (nowCoroutine != null) StopCoroutine(nowCoroutine);
            StartCoroutine(Attack_3__());
        }
    }
    IEnumerator Attack_3__()
    {
        anim.SetTrigger("Attack_3");

        yield return new WaitForSeconds(0.7f);
        particleAttack3.Play();
        attackArea3.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        particleAttack3.Stop();
        attackArea3.SetActive(false);
        yield return new WaitForSeconds(1.6f);
        isAct = false;
        BossMoveStart();
    }


    // #. 공격 4
    public void Attack_4()
    {
        if (!isAct)
        {
            isAct = true;
            if (nowCoroutine != null) StopCoroutine(nowCoroutine);
            StartCoroutine(Attack_4__());
        }
    }
    IEnumerator Attack_4__()
    {
        float dashSpeed = 30f;
        float dashDuration = 0.7f;
        Vector3 targetPosition = positionAttack4[GetQuadrant()-1].position;
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);

        nav.speed = 0; // 보스 일시 정지
        rotationSpeed = 500;
        anim.SetBool("isWalk", false);
        bIsRotate = false;
        yield return new WaitForSeconds(0.5f);

        float rotationTime = 0.5f;
        float elapsedRotationTime = 0f;
        while (elapsedRotationTime < rotationTime)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            elapsedRotationTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1.3f);
        anim.SetTrigger("Dash_Start");
        yield return new WaitForSeconds(0.5f);
        attackAreaRush.SetActive(true);
        particleDash.Play();
       
        float elapsedTime = 0f;
        bool bbAnim = false;
        while (elapsedTime < dashDuration)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= 0.5f && !bbAnim)
            {
                bbAnim = true;
                anim.SetTrigger("Dash_End");
            }
            Vector3 direction = (targetPosition - transform.position).normalized;
            nav.velocity = direction * dashSpeed;

            yield return null;
        }
        particleDash.Stop();
        attackAreaRush.SetActive(false);
        nav.velocity = Vector3.zero;

        yield return new WaitForSeconds(1f);

        // #. 중심 방향으로 회전
        elapsedRotationTime = 0f;
        targetRotation = Quaternion.LookRotation(new Vector3(0, 0, 0) - transform.position);
        while (elapsedRotationTime < rotationTime)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            elapsedRotationTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        anim.SetTrigger("Attack4_Start");
        yield return new WaitForSeconds(0.3f);
        particleAttack4_Start.Play();
        yield return new WaitForSeconds(3.2f);
        particleAttack4_Start.Stop();
        anim.SetTrigger("Attack4_End");
        yield return new WaitForSeconds(0.4f);
        attackArea4.SetActive(true);
        particleAttack4_End.Play();
        yield return new WaitForSeconds(0.8f);
        attackArea4.SetActive(false);
        yield return new WaitForSeconds(1.6f);
        particleAttack4_End.Stop();

        targetPosition = positionAttack4[GetQuadrant() - 1].position;
        targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
        yield return new WaitForSeconds(1.2f);
        anim.SetTrigger("Dash_Start");
        yield return new WaitForSeconds(0.5f);
        attackAreaRush.SetActive(true);
        particleDash.Play();
        elapsedTime = 0f;
        bbAnim = false;
        while (elapsedTime < dashDuration)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= 0.5f && !bbAnim)
            {
                bbAnim = true;
                anim.SetTrigger("Dash_End");
            }
            Vector3 direction = (targetPosition - transform.position).normalized;
            nav.velocity = direction * dashSpeed;

            yield return null;
        }
        particleDash.Stop();
        attackAreaRush.SetActive(false);
        nav.velocity = Vector3.zero;
        yield return new WaitForSeconds(1f);
        isAct = false;
        BossMoveStart();
    }


    // #. 참격 1
    public void Slash_1()
    {
        if (!isAct)
        {
            isAct = true;
            if (nowCoroutine != null) StopCoroutine(nowCoroutine);
            StartCoroutine(Slash_1__());
        }

    }
    IEnumerator Slash_1__()
    {
        anim.SetTrigger("Slash_1");

        yield return new WaitForSeconds(1.0f);
        Vector3 spawnPosition = transform.position + transform.forward * 1.5f;
        GameObject newSlashObj = Instantiate(slashObj1, spawnPosition, Quaternion.identity);
        newSlashObj.transform.rotation = transform.rotation;
        yield return new WaitForSeconds(1.7f);
        isAct = false;
        BossMoveStart();
    }


    // #. 참격 2
    public void Slash_2()
    {
        if (!isAct)
        {
            isAct = true;
            if (nowCoroutine != null) StopCoroutine(nowCoroutine);
            StartCoroutine(Slash_2__());
        }
    }
    IEnumerator Slash_2__()
    {
        anim.SetTrigger("Slash_2");

        yield return new WaitForSeconds(1.4f);

        // 생성 위치를 현재 위치에서 앞, 뒤, 왼쪽, 오른쪽으로 옮깁니다.
        Vector3[] spawnPositions = new Vector3[]
        {
        transform.position + transform.forward * 2.3f, // 앞쪽
        transform.position - transform.forward * 2.3f, // 뒤쪽
        transform.position + transform.right * 2.3f,   // 오른쪽
        transform.position - transform.right * 2.3f    // 왼쪽
        };

        // 생성될 오브젝트의 회전 각도를 설정합니다.
        Quaternion[] rotations = new Quaternion[]
        {
        transform.rotation,  // 앞쪽 방향
        Quaternion.LookRotation(-transform.forward),  // 뒤쪽 방향
        Quaternion.LookRotation(transform.right),    // 오른쪽 방향
        Quaternion.LookRotation(-transform.right)    // 왼쪽 방향
        };

        // 각 방향에 따라 오브젝트를 생성합니다.
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            GameObject newSlashObj = Instantiate(slashObj2, spawnPositions[i], rotations[i]);
        }

        yield return new WaitForSeconds(1.7f);
        isAct = false;
        BossMoveStart();
    }

    #endregion

    #region // 돌진, 물러나기 함수들

    // #. 돌진
    public void Rush()
    {
        if (!isAct)
        {
            isAct = true;
            if (nowCoroutine != null) StopCoroutine(nowCoroutine);
            StartCoroutine(Rush__());
        }
    }
    IEnumerator Rush__()
    {
        float dashSpeed = 30f;
        float dashDuration = 0.7f;

        yield return new WaitForSeconds(0.2f);
        bIsRotate = true;
        nav.speed = 0; // 보스 일시 정지
        rotationSpeed = 1000;
        anim.SetBool("isWalk", false);

        yield return new WaitForSeconds(1.0f);
        anim.SetTrigger("Dash_Start");
        bIsRotate = false;

        yield return new WaitForSeconds(0.5f);
        colliderMine.isTrigger = true;

        attackAreaRush.SetActive(true);
        particleDash.Play();

        float elapsedTime = 0f;
        bool bbAnim = false;

        while (elapsedTime < dashDuration)
        {
            elapsedTime += Time.deltaTime;

            if(elapsedTime >= 0.5f && !bbAnim)
            {
                bbAnim = true;
                anim.SetTrigger("Dash_End"); 
            }

            if (Physics.Raycast(transform.position, transform.forward * 2, out RaycastHit hit, dashSpeed * Time.deltaTime))
            {
                if (hit.collider.CompareTag("WallBoss_1"))
                {
                    bbAnim = true;
                    anim.SetTrigger("Dash_End");
                    break; 
                }
            }

            nav.velocity = transform.forward * dashSpeed;

            yield return null;
        }

        particleDash.Stop();
        attackAreaRush.SetActive(false);
         
        nav.velocity = Vector3.zero; // 돌진 종료 후 속도 초기화

        if(!bPhase2)
        {
            if (dashCnt < 1)
            {
                isAct = false;
                dashCnt++;
                StopAllCoroutines();
                Rush();
            } 
        }
        else if(bPhase2)
        {
            if(dashCnt < 2)
            {
                isAct = false;
                dashCnt++;
                StopAllCoroutines();
                Rush();
            }
        }
        colliderMine.isTrigger = false;

        yield return new WaitForSeconds(1.0f);

        isAct = false;
        BossMoveStart();
        dashCnt = 0;
    }



    // #. 물러나기
    public void BackStep()
    {
        if (!isAct)
        {
            isAct = true;
            if (nowCoroutine != null) StopCoroutine(nowCoroutine);
            StartCoroutine(BackStep__());
        }
    }
    IEnumerator BackStep__()
    {
        anim.SetTrigger("BackStep");


        yield return new WaitForSeconds(1.0f);
        isAct = false;
        BossMoveStart();
    }


    #endregion


    #region // 기타 함수들

    // #. 페이즈 2 진입
    public void Phase2Start()
    {
        if (!isAct)
        {
            isAct = true;
            if (nowCoroutine != null) StopCoroutine(nowCoroutine);
            StartCoroutine(Phase2Start__());
        }
    }
    IEnumerator Phase2Start__()
    {
        BossMoveStop();
        anim.SetTrigger("Loud");
        yield return new WaitForSeconds(0.5f);
        particlePhase2.Play();
        yield return new WaitForSeconds(2.0f);

        monsterCreator.iMaxMonsterCnt = 4;
        goblinFireCreator.interval = 2.5f;

        isAct = false;
        BossMoveStart();
    }


    // #. 데미지 입음 함수
    public override void TakeDamage(int damage)
    {
        if(!bIsDie)
        {
            ChangeMaterialsTemporarily();

            int tempDmgNum = damage * Random.Range(15, 30);
            currentHp -= tempDmgNum;
            if (currentHp <= 0) currentHp = 0;
            float remainingHpPercentage = Mathf.Round(((float)currentHp / (float)maxHp) * 100f) / 100f;
            ImageHp.fillAmount = remainingHpPercentage;

            Debug.Log(currentHp);

            if (currentHp <= 0)
            {
                DieBoss();
            }
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


    public override void Die() { } // 오버라이드용 함수 형식상 들어가 있는 거임 작동 X

    // #. 죽음 함수
    public void DieBoss()
    {
        if (nowCoroutine != null) StopCoroutine(nowCoroutine);
        StopAllCoroutines();
        StartCoroutine(Die__());
    }
    IEnumerator Die__()
    {
        particleAttack1.Stop();
        particleAttack2.Stop();
        particleAttack3.Stop();
        particleAttack4_Start.Stop();
        particleAttack4_End.Stop();
        particleDash.Stop();
        particlePhase2.Stop();


        bIsDie = true;
        particlePhase2.Stop();

        anim.SetTrigger("doDie");

        nav.isStopped = true; // NavMeshAgent 멈춤
        nav.speed = 0;
        nav.angularSpeed = 0;
        collider.enabled = false;
        FreezeMonster();
        FixPosition(transform.position);


        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            NewEnemy newEnemyScript = enemy.GetComponent<NewEnemy>();
            if (newEnemyScript != null)
            {
                newEnemyScript.Die(); // 실행할 함수
            }
        }
        yield return new WaitForSeconds(2f);

        FadeOut();
        image_HpPanel.SetActive(false);

        yield return new WaitForSeconds(6f);

        ClearStep.SetActive(true);
        DestroyRoad[0].SetActive(false);
        DestroyRoad[1].SetActive(false);
        DestroyRoad[2].SetActive(false);
    }


    // #. 공격 사정거리 감지
    bool CheckTargetInRange()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, targetRadius, transform.forward, out hit, targetRange, LayerMask.GetMask("Player")))
        {
            return true;
        }
        return false;
    }


    // #. 보스 이동 시작
    private void BossMoveStart()
    {
        anim.SetBool("isWalk", true);
        nav.speed = speedMove;
        rotationSpeed = 220;
        bIsRotate = true;
    }

    // #. 보스 이동 중지
    private void BossMoveStop()
    {
        anim.SetBool("isWalk", false);
        nav.speed = 0f;

        bDetectPlayer = false;
    }

    private void RotateTowardsPlayer()
    {
        if(bIsRotate && !bIsDie)
        {
            Vector3 direction = player.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // #. 현재 위치한 사분면 계산
    public int GetQuadrant()
    {
        Vector3 position = transform.position;

        if (position.x >= 0 && position.z >= 0)
        {
            return 1; // X가 양수, Z가 양수인 경우
        }
        else if (position.x >= 0 && position.z < 0)
        {
            return 2; // X가 양수, Z가 음수인 경우
        }
        else if (position.x < 0 && position.z >= 0)
        {
            return 3; // X가 음수, Z가 양수인 경우
        }
        else if (position.x < 0 && position.z < 0)
        {
            return 4; // X가 음수, Z가 음수인 경우
        }

        return 0; // 이론적으로 발생하지 않지만, 안전을 위해 추가
    }


    public void FixPosition(Vector3 desiredPosition)
    {
        transform.position = desiredPosition;
    }
    public void FreezeMonster()
    {
        nav.velocity = Vector3.zero;  // NavMeshAgent의 이동 속도 초기화
        rb.velocity = Vector3.zero;   // Rigidbody의 이동 속도 초기화
        rb.angularVelocity = Vector3.zero;  // Rigidbody의 각속도 초기화
    }



    public void FadeOut()
    {
        StartCoroutine(Fade(image_ClearPanel, 0f, 1f));
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
    IEnumerator Fade__(Image image, float startAlpha, float targetAlpha)
    {
        float startTime = Time.time;
        Color color = image.color;

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

    #endregion






}
