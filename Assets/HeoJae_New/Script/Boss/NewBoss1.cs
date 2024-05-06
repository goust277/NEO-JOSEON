using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NewBoss1 : NewEnemy
{
    [Header("보스 능력치")] 
    public float speedMove; 
    public float rotationSpeed; 

    [Header("보스 상태")] 
    public bool bIsChase;  
    public bool isAct;  

    [Header("머테리얼")] 
    public Material matWhite; 
    private Material[] originalMaterials; 
    Renderer[] renderers; 

    [Header("애니메이터")] 
    public Animator anim; 

    private Transform player;  
    private NavMeshAgent nav; 
    private Coroutine nowCoroutine; 


    [Header("공격 범위")] 
    public GameObject attackArea1; 
    public GameObject attackArea2; 
    public GameObject attackArea3; 
    public GameObject slashObj1; 
    public GameObject slashObj2;   


    private void Awake()
    {
        // #. 네비게이션 관련 정보 가져오기
        player = GameObject.FindGameObjectWithTag("Player").transform;
        nav = GetComponent<NavMeshAgent>();

        // #. 머테리얼 찾아오기
        renderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++) originalMaterials[i] = renderers[i].material;

        anim.SetBool("isWalk",true);

        // nav.isStopped = true;



        // 네비게이션 설정
        // bIsChase = true;

        nav.speed = speedMove;
        nav.angularSpeed = rotationSpeed;
    }

    private void Update()
    {
       
        // 추적 중
        if (bIsChase)
        {
            nav.SetDestination(player.position);
        }








        if (Input.GetKeyDown(KeyCode.L))
        {
            Attack_1();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            Attack_2();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            Attack_3();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            Slash_1();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            Slash_2();
        }

       
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
        attackArea1.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        attackArea1.SetActive(false);
        isAct = false;
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

        yield return new WaitForSeconds(1.7f);
        attackArea2.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        attackArea2.SetActive(false);
        yield return new WaitForSeconds(0.7f);
        attackArea2.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        attackArea2.SetActive(false);
        yield return new WaitForSeconds(2.4f);
        isAct = false;
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
        anim.SetTrigger("Attack_1");

        yield return new WaitForSeconds(1.0f);
        attackArea3.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        attackArea3.SetActive(false);
        isAct = false;
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
        yield return new WaitForSeconds(1.0f);
        isAct = false;
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
        anim.SetTrigger("Attack_1");

        yield return new WaitForSeconds(1.0f);
        Vector3 spawnPosition = transform.position + transform.forward * 1.5f;
        GameObject newSlashObj = Instantiate(slashObj1, spawnPosition, Quaternion.identity);
        newSlashObj.transform.rotation = transform.rotation;
        yield return new WaitForSeconds(3.0f);
        
        isAct = false;
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
        anim.SetTrigger("Attack_1");

        yield return new WaitForSeconds(1.0f);

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
        transform.rotation,                     // 앞쪽 방향
        Quaternion.LookRotation(-transform.forward),  // 뒤쪽 방향
        Quaternion.LookRotation(transform.right),    // 오른쪽 방향
        Quaternion.LookRotation(-transform.right)    // 왼쪽 방향
        };

        // 각 방향에 따라 오브젝트를 생성합니다.
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            GameObject newSlashObj = Instantiate(slashObj2, spawnPositions[i], rotations[i]);
        }

        yield return new WaitForSeconds(3.0f);

        isAct = false;
    }

    #endregion


    #region // 돌진, 물러나기 함수들

    // #. 돌진
    public void Rush()
    {
        if(!isAct)
        {
            isAct = true;
            if (nowCoroutine != null) StopCoroutine(nowCoroutine);
            StartCoroutine(Rush__());
        }
    }
    IEnumerator Rush__()
    {
        yield return new WaitForSeconds(1.0f);
        isAct = false;
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
        yield return new WaitForSeconds(1.0f);
        isAct = false;
    }

    #endregion




    #region // 기타 함수들

    // #. 데미지 입음 함수
    public override void TakeDamage(int damage)
    {
        currentHp -= damage;
        StartCoroutine(ChangeMaterialsTemporarily(matWhite, 0.1f));
    }
    IEnumerator ChangeMaterialsTemporarily(Material newMaterial, float duration)
    {
        foreach (Renderer renderer in renderers) renderer.material = newMaterial;
        yield return new WaitForSeconds(duration);
        for (int i = 0; i < renderers.Length; i++) renderers[i].material = originalMaterials[i];
    }

    #endregion






}
