
using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.LookDev;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    private Coroutine coroutine;
    public string currentMapName;

    [Header("플레이어 이펙트")]
    [SerializeField] private GameObject UnderLine;
    [SerializeField] private ParticleSystem DashEffect;

    [Header("플레이어 이동")]
    public float speed = 5.0f;
    public float dash;
    public float dashTime;
    public float dashCoolTime;
    public float rotspeed;
    public float maxspeed;
    public float jumpHeight;

    [SerializeField] private float interactionRange = 3.0f;
    [SerializeField] private GameObject currentInteractableObject;
    private bool isInteracting = false;

    private float atkDeley;

    private Rigidbody rb;

    public Camera cam;
    private GameObject freeLookCamera;

    [SerializeField]private bool isGround;
    private bool isAttackReady;
    private bool isNextAtk;
    private bool isDashing = false;
    private bool isCooldown = false;
    [SerializeField] private float groundCheck;
    [SerializeField] private bool isDoubleJump;

    public LayerMask layer, interactableLayer;

    private Vector3 dir = Vector3.zero;
    private Animator animator;


    PlayerDetect detect;
    PlayerSkill skill;

    private bool isSetting = false;
    [Header("메뉴")]
    private GameObject mainSetting;
    private GameObject stageSetting;

    [SerializeField] private string main;

    [Header("무기")]
    public Weapon weapon;
    [SerializeField] private int damage;
    [SerializeField] private float rate;
    [SerializeField] private float effectTime;

    private PlayerDamage playerDamge;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        detect = GetComponent<PlayerDetect>();
        skill = GetComponent<PlayerSkill>();
        animator = GetComponent<Animator>();
        playerDamge = GetComponent<PlayerDamage>();

        mainSetting = GameObject.Find("Main_Setting");
        stageSetting = GameObject.Find("Stage_Setting");

        Cursor.visible = false;

        cam = Camera.main;
        freeLookCamera = GameObject.FindGameObjectWithTag("FLCamera");
    }

    void Update()
    {
        weapon.rate = rate;
        weapon.damage = damage;
        weapon.effectTime = effectTime;

        Vector3 cameraForward = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1)).normalized;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        dir = z * cameraForward + x * cam.transform.right;
        if(!isGround) 
        {
            animator.SetBool("IsOnAir", true);
        }
        else
        {
            animator.SetBool("IsOnAir", false);

        }

        if (dir != Vector3.zero)
        {
            animator.SetBool("Move", true);
        }
        else
        {
            animator.SetBool("Move", false);
        }


        if (Input.GetKeyDown(KeyCode.Escape) && !isSetting&&!isInteracting)
        {
            // 상호 작용 시작
            StartSetting();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isSetting && !isInteracting)
        {
            // 상호 작용 중지

            EndSettring();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isInteracting)
            {
                if (currentInteractableObject != null)
                    StartInteraction();
            }
            else
                EndInteraction();

        }
        if (!isSetting && !playerDamge.isHit && !isInteracting) 
        {
            CheckGround();
            Attack();
            if (weapon.isAtkTime)
            {
                //animator.SetBool("Move", false);
            }

            if (isAttackReady == false)
            {
                atkDeley += Time.deltaTime;
            }

            if (weapon.rate < atkDeley)
            {
                isAttackReady = true;
            }

            if (isAttackReady && !weapon.isAtkTime && !skill.isSkillTime && !playerDamge.isHit)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (isGround == true)
                    {
                        Jump();
                        animator.SetTrigger("Jump1");
                    }
                    else if (isDoubleJump == false)
                    {
                        Jump();
                        animator.SetTrigger("Jump2");
                        isDoubleJump = true;
                    }
                }

            }
            if (isGround && !isDashing)
            {
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    weapon.StopAtk();
                    skill.TriggerSkill();
                    animator.SetBool("Move", false);
                }
            }


            if (Input.GetKeyDown(KeyCode.T))
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            if (Input.GetKeyDown(KeyCode.LeftShift) && !isCooldown)
            {
                coroutine = StartCoroutine(Dash());
            }

            if (Input.GetKeyDown(KeyCode.Mouse0) && !isInteracting &&!isDashing)
            {
                isNextAtk = true;
            }
        }
        else if (isSetting)
        {
            dir = Vector3.zero;
        }
    }

    public void TakeDamage()
    {
        weapon.StopAtk();
        skill.StopSkill();

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            rb.useGravity = true;
            rb.velocity = Vector3.zero;
            DashEffect.Stop();

            Invoke("CoolDown", dashCoolTime);
            isDashing = false;
        }
    }

    private void CoolDown()
    {
        isCooldown = false;
    }
    private void FixedUpdate()
    {

        if (isAttackReady && !weapon.isAtkTime && !skill.isSkillTime && !playerDamge.isHit && !isDashing)
        {
            if (dir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotspeed).normalized;
                if(isGround)
                {
                    animator.SetBool("Move", true);
                }
            }

            else
            {
                if (isGround)
                {
                    animator.SetBool("Move", false);
                }
            }
            rb.MovePosition(this.gameObject.transform.position + dir * speed * Time.deltaTime);
        }
        Vector3 playerPosition = transform.position;

        // 상호 작용 가능한 오브젝트 찾기
        Collider[] colliders = Physics.OverlapSphere(playerPosition, interactionRange, interactableLayer);

        if (colliders.Length > 0)
        {
            // 가장 가까운 상호 작용 가능한 오브젝트 선택
            currentInteractableObject = colliders[0].gameObject;
        }
        else
        {
            // 상호 작용 가능한 오브젝트가 없을 때
            currentInteractableObject = null;
        }

    }
    private void Jump()
    {
        Vector3 jumpPower = Vector3.up * jumpHeight;
        rb.AddForce(jumpPower, ForceMode.VelocityChange);

    }
    private void CheckGround()
    {

        if (Physics.BoxCast(transform.position + (Vector3.up * groundCheck), transform.lossyScale / 2.0f, Vector3.down, out RaycastHit hit, transform.rotation, 0.2f, layer))
        {
            isGround = true;
            isDoubleJump = false;
        }
        else
        {
            isGround = false;
        }
    }

    private IEnumerator Dash()
    {
        weapon.isAtkTime = false;
        weapon.StopAtk();
        skill.StopSkill();  
        animator.SetTrigger("Dash");

        isDashing = true;
        Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 1000000);
        yield return new WaitForSeconds(0.05f);

        isCooldown = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        DashEffect.Play();
        Vector3 dashDirection = (dir != Vector3.zero ? dir : transform.forward).normalized;

        Vector3 dashPower = dashDirection * dash;
        rb.velocity = dashPower;

        Quaternion originalRotation = rb.rotation;


        float delay = 0;
        while (delay < dashTime)
        {
            delay += Time.deltaTime;

            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            rb.rotation = originalRotation;

            yield return null;
        }
        rb.useGravity = true;
        rb.velocity = Vector3.zero;

        isDashing = false;
        DashEffect.Stop();

        yield return new WaitForSeconds(dashCoolTime);

        isCooldown = false;
    }

    void Attack()
    {
        if (isGround == true && isAttackReady == true && isNextAtk == true)
        {
            isAttackReady = false;
            if (detect.visibleTargets.Count > 0)
            {
                dir = Vector3.zero;
                Vector3 currentPosition = transform.position;
                Vector3 enemy = (detect.visibleTargets[0].position - currentPosition);
                enemy.y = 0.0f;

                Quaternion rotation = Quaternion.LookRotation(enemy.normalized);

                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 10000 * Time.deltaTime);
            }
            else { }
            weapon.Use();

            atkDeley = 0.0f;

            isNextAtk = false;
        }

        if (!isGround)
        {
            isNextAtk = false;
        }
    }
    //상호 작용 시작 함수
    private void StartInteraction()
    {
        MouseOn();
        isInteracting = true;
        currentInteractableObject.GetComponent<InteractableObject>().Interact();
    }

    // 상호 작용 종료 함수
    private void EndInteraction()
    {
        MouseOff();
        isInteracting = false;
        currentInteractableObject.GetComponent<InteractableObject>().EndInteract();
    }

    private void StartSetting()
    {
        MouseOn();
        isSetting = true;
        if (SceneManager.GetActiveScene().name == main)
        {
            mainSetting.GetComponent<MainSetting>().Open();
        }
        else
        {
            stageSetting.GetComponent<StageSetting>().Open();
        }
    }

    private void EndSettring()
    {
        MouseOff();
        isSetting = false;
        if (SceneManager.GetActiveScene().name == main)
        {
            mainSetting.GetComponent<MainSetting>().Close();
        }
        else
        {
            stageSetting.GetComponent<StageSetting>().Close();
        }
    }

    private void MouseOn()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        if (freeLookCamera != null)
        {
            freeLookCamera.SetActive(false);
        }
    }

    private void MouseOff()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (freeLookCamera != null)
        {
            freeLookCamera.SetActive(true);
        }
    }
    
    public void AtkOn()
    {
        weapon.AttOn();
    }

    public void AtkOff()
    {
        weapon.AttkOff();
    }
}

