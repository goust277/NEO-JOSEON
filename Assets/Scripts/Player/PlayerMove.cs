
using Cinemachine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.LookDev;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
   

    private Coroutine coroutine;
    public string currentMapName;
    [Header("�÷��̾� Ʃ�丮��")]
    private bool _jump = true;
    private bool _dash = true;
    private bool _atk = true;
    private bool _skill = true;

    private int tutorial = 0;


    [Header("�÷��̾� ����Ʈ")]
    [SerializeField] private GameObject UnderLine;
    [SerializeField] private ParticleSystem DashEffect;

    [Header("�ٴ�üũ")]
    [SerializeField] private Vector3 groundCheckSize = new Vector3(0.5f, 0.1f, 0.5f);
    [SerializeField] private Transform _groundCheck;

    [Header("�� üũ")]
    [SerializeField] private Vector3 wallCheckSize = new Vector3(0.5f, 0.5f, 0.5f); // �� üũ�� ���� �ڽ��� ũ��
    [SerializeField] private float wallCheckDistance = 0.5f; // �� üũ�� ���� �Ÿ�

    [Header("�÷��̾� �̵�")]

    public float speed = 5.0f;
    public float dash;
    public float dashTime;
    public float dashDelay;
    public float dashCoolTime;
    public float rotspeed;
    public float maxspeed;
    public float jumpHeight;
    [SerializeField] private float customGravityScale = 2.0f;

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

    [Header("��ų")]
    public int skillCool;
    public float skillCoolTime;

    [Header("�޴�")]
    private GameObject mainSetting;
    private GameObject stageSetting;

    [SerializeField] private string main;

    [Header("����")]
    public Weapon weapon;
    [SerializeField] private int damage;
    [SerializeField] private float rate;
    [SerializeField] private float effectTime;

    private PlayerDamage playerDamge;

    [Header("Ʃ�丮��")]
    public bool onTxt = false;

    private bool wallcollision; // ���� ����ħ
    private void Awake()
    {
        skillCoolTime = skillCool;
        dashDelay = dashCoolTime;
        if (SceneManager.GetActiveScene().name == "tutorial")
        {
            _jump = false;
            _dash = false;
            _skill = false;
            _atk = false;
        }
        if (SceneManager.GetActiveScene().name == "tutorial _atk")
        {
            _skill = false;
            _atk = false;
        }

        rb = GetComponent<Rigidbody>();
        detect = GetComponent<PlayerDetect>();
        skill = GetComponent<PlayerSkill>();
        animator = GetComponent<Animator>();
        playerDamge = GetComponent<PlayerDamage>();

        //rb.useGravity = false;

        mainSetting = GameObject.Find("Main_Setting");
        stageSetting = GameObject.Find("Stage_Setting");

        MouseOff();

        cam = Camera.main;
        freeLookCamera = GameObject.FindGameObjectWithTag("FLCamera");
    }

    public bool Ondie = false;

    void Update()
    {
        weapon.rate = rate;
        weapon.damage = damage;
        weapon.effectTime = effectTime;

        Vector3 cameraForward = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1)).normalized;


        CheckGround();
        WallCheck();

        if (!isGround)
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

        if (isDashing)
            rb.drag = 0f;

        if (skillCoolTime < skillCool)
        {
            skillCoolTime += Time.deltaTime;
        }

        if (onTxt == false && Ondie == false) 
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            dir = z * cameraForward + x * cam.transform.right;

            if (dashDelay <= dashCoolTime)
            {
                isCooldown = true;
                dashDelay += Time.deltaTime;
            }
            else
            {
                isCooldown = false;
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
                
                Attack();

                if (isAttackReady == false)
                {
                    atkDeley += Time.deltaTime;
                }

                if (weapon.rate < atkDeley)
                {
                    isAttackReady = true;
                }

                if (isAttackReady && !weapon.isAtkTime && !skill.isSkillTime && !playerDamge.isHit && _jump && !isDashing)
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
                if (isGround && !isDashing && _skill && skillCoolTime >= skillCool)
                {
                    if (Input.GetKeyDown(KeyCode.Mouse1))
                    {
                        weapon.StopAtk();
                        skill.TriggerSkill();
                        animator.SetBool("Move", false);
                        skillCoolTime = 0f;
                    }
                }
                if (Input.GetKeyDown(KeyCode.LeftShift) && !isCooldown && _dash)
                {
                    coroutine = StartCoroutine(Dash());
                    dashDelay = 0;
                }

                if (Input.GetKeyDown(KeyCode.Mouse0) && !isInteracting && !isDashing && _atk)
                {
                    isNextAtk = true;
                }
            }
            else if (isSetting)
            {
                dir = Vector3.zero;
            }

        }

        else
        {
            if (freeLookCamera != null)
            {
                freeLookCamera.SetActive(false);
            }
            dir = Vector3.zero;
        }
        if (Input.GetKeyDown(KeyCode.Escape) && !isSetting && !isInteracting)
        {
            // ��ȣ �ۿ� ����
            StartSetting();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isSetting && !isInteracting)
        {
            // ��ȣ �ۿ� ����

            EndSettring();
        }
        //ApplyCustomGravity();


    }
    private void WallCheck()
    {
        wallcollision = Physics.Raycast(transform.position + new Vector3(0, 1.0f, 0), transform.forward, 0.6f, LayerMask.GetMask("Plane"));
    }
    void OnDrawGizmosSelected()
    {
        if (_groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(_groundCheck.position, groundCheckSize);
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
            isDashing = false;
        }
    }

    private void FixedUpdate()
    {

        if (isAttackReady && !weapon.isAtkTime && !skill.isSkillTime && !playerDamge.isHit && !isDashing && !onTxt)
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

            if (!wallcollision)
                gameObject.transform.position += dir * speed * Time.deltaTime;

            

        }
        Vector3 playerPosition = transform.position;

        // ��ȣ �ۿ� ������ ������Ʈ ã��
        Collider[] colliders = Physics.OverlapSphere(playerPosition, interactionRange, interactableLayer);

        if (colliders.Length > 0)
        {
            // ���� ����� ��ȣ �ۿ� ������ ������Ʈ ����
            currentInteractableObject = colliders[0].gameObject;
        }
        else
        {
            // ��ȣ �ۿ� ������ ������Ʈ�� ���� ��
            currentInteractableObject = null;
        }

    }
    private void Jump()
    {
        Vector3 jumpPower = Vector3.up * jumpHeight;
        rb.AddForce(jumpPower, ForceMode.VelocityChange);
        //rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);

    }
    private void CheckGround()
    {
        isGround = Physics.CheckBox(_groundCheck.position, groundCheckSize / 2, Quaternion.identity, layer);

        if(isGround) 
        {
            isDoubleJump = false;
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
    //��ȣ �ۿ� ���� �Լ�
    private void StartInteraction()
    {
        MouseOn();
        isInteracting = true;
        currentInteractableObject.GetComponent<InteractableObject>().Interact();
    }

    // ��ȣ �ۿ� ���� �Լ�
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

    public void MouseOn()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (freeLookCamera != null)
        {
            freeLookCamera.SetActive(false);
        }
    }

    public void MouseOff()
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

    public void ClearTutorial()
    {
        if (tutorial == 0)
        {
            tutorial = 1;
            _jump = true;
        }
        else if (tutorial == 1) 
        {
            tutorial = 2;
            _dash = true;

        }
    }

    public void ClearAtkTutorial()
    {
        if(tutorial == 0)
        {
            tutorial = 1;
            _atk = true;
        }
        else if (tutorial == 1)
        {
            tutorial = 2;
            _skill =  true;
        }
    }
}