using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class NewBoss1 : NewEnemy
{
    [Header("���� �ɷ�ġ")]
    public float speedMove;
    public float rotationSpeed;

    [Header("���� ����")]
    public bool bIsDie;
    public bool bIsChase; // ���� ����� ���ϰ� ����
    public bool isAct;
    public bool bIsRotate;
    private bool bPhase2 = false; // ���� ���� ����� 1���� 2���� 

    [Header("���׸���")]
    public GameObject Model;
    public Material newMaterial;
    public float duration;

    private Renderer[] renderers;
    private Material[] originalMaterials;

    [Header("�ִϸ�����")]
    public Animator anim;

    private Transform player;
    private NavMeshAgent nav;
    private Coroutine nowCoroutine;
    private Rigidbody rb;
    public Collider collider;


    [Header("���� ����")]
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

    [Header("����Ʈ")]
    public ParticleSystem particleAttack1;
    public ParticleSystem particleAttack2;
    public ParticleSystem particleAttack3;
    public ParticleSystem particleAttack4_Start;
    public ParticleSystem particleAttack4_End;
    public ParticleSystem particleDash;
    public ParticleSystem particlePhase2;

    [Header("ü�¹�")]
    public GameObject HpBar;
    public Image ImageHp;


    [Header("������� / ���� ���� / ��Ÿ")]
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
        // #. �׺���̼� ���� ���� ��������
        player = GameObject.FindGameObjectWithTag("Player").transform;
        nav = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        renderers = Model.GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length];

        // �� Renderer�� ���� Material�� �����մϴ�.
        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].material;
        }

        // #. �߰� ���� ����
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


    // #. ���� ���� ���� - ����
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

    // #. ���� ���� ���� - ���� ����
    public void RandomAttackSelect_Failed()
    {
        int ranNum = Random.Range(6, 10);
        if (ranNum <= 2) Slash_1();
        else if (ranNum > 2 && ranNum <= 5) Slash_2();
        else if (ranNum > 5) Rush();
    }


    #region // ���� ���� �Լ���


    // #. ���� 1
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


    // #. ���� 2
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


    // #. ���� 3
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


    // #. ���� 4
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

        nav.speed = 0; // ���� �Ͻ� ����
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

        // #. �߽� �������� ȸ��
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


    // #. ���� 1
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


    // #. ���� 2
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

        // ���� ��ġ�� ���� ��ġ���� ��, ��, ����, ���������� �ű�ϴ�.
        Vector3[] spawnPositions = new Vector3[]
        {
        transform.position + transform.forward * 2.3f, // ����
        transform.position - transform.forward * 2.3f, // ����
        transform.position + transform.right * 2.3f,   // ������
        transform.position - transform.right * 2.3f    // ����
        };

        // ������ ������Ʈ�� ȸ�� ������ �����մϴ�.
        Quaternion[] rotations = new Quaternion[]
        {
        transform.rotation,  // ���� ����
        Quaternion.LookRotation(-transform.forward),  // ���� ����
        Quaternion.LookRotation(transform.right),    // ������ ����
        Quaternion.LookRotation(-transform.right)    // ���� ����
        };

        // �� ���⿡ ���� ������Ʈ�� �����մϴ�.
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            GameObject newSlashObj = Instantiate(slashObj2, spawnPositions[i], rotations[i]);
        }

        yield return new WaitForSeconds(1.7f);
        isAct = false;
        BossMoveStart();
    }

    #endregion

    #region // ����, �������� �Լ���

    // #. ����
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
        nav.speed = 0; // ���� �Ͻ� ����
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
         
        nav.velocity = Vector3.zero; // ���� ���� �� �ӵ� �ʱ�ȭ

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



    // #. ��������
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


    #region // ��Ÿ �Լ���

    // #. ������ 2 ����
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


    // #. ������ ���� �Լ�
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


    public override void Die() { } // �������̵�� �Լ� ���Ļ� �� �ִ� ���� �۵� X

    // #. ���� �Լ�
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

        nav.isStopped = true; // NavMeshAgent ����
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
                newEnemyScript.Die(); // ������ �Լ�
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


    // #. ���� �����Ÿ� ����
    bool CheckTargetInRange()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, targetRadius, transform.forward, out hit, targetRange, LayerMask.GetMask("Player")))
        {
            return true;
        }
        return false;
    }


    // #. ���� �̵� ����
    private void BossMoveStart()
    {
        anim.SetBool("isWalk", true);
        nav.speed = speedMove;
        rotationSpeed = 220;
        bIsRotate = true;
    }

    // #. ���� �̵� ����
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

    // #. ���� ��ġ�� ��и� ���
    public int GetQuadrant()
    {
        Vector3 position = transform.position;

        if (position.x >= 0 && position.z >= 0)
        {
            return 1; // X�� ���, Z�� ����� ���
        }
        else if (position.x >= 0 && position.z < 0)
        {
            return 2; // X�� ���, Z�� ������ ���
        }
        else if (position.x < 0 && position.z >= 0)
        {
            return 3; // X�� ����, Z�� ����� ���
        }
        else if (position.x < 0 && position.z < 0)
        {
            return 4; // X�� ����, Z�� ������ ���
        }

        return 0; // �̷������� �߻����� ������, ������ ���� �߰�
    }


    public void FixPosition(Vector3 desiredPosition)
    {
        transform.position = desiredPosition;
    }
    public void FreezeMonster()
    {
        nav.velocity = Vector3.zero;  // NavMeshAgent�� �̵� �ӵ� �ʱ�ȭ
        rb.velocity = Vector3.zero;   // Rigidbody�� �̵� �ӵ� �ʱ�ȭ
        rb.angularVelocity = Vector3.zero;  // Rigidbody�� ���ӵ� �ʱ�ȭ
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
