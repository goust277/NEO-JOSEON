
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.LookDev;

public class PlayerMove : MonoBehaviour
{
    public string currentMapName;

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

    [SerializeField]private bool isGround;
    private bool isAttackReady;
    private bool isNextAtk;
    private bool isDashing = false;
    private bool isCooldown = false;
    [SerializeField] private float groundCheck;
    private bool isDoubleJump;

    public LayerMask layer, interactableLayer;

    private Vector3 dir = Vector3.zero;
    private Animator animator;

    public Weapon weapon;
    PlayerDetect detect;
    PlayerSkill skill;


    [Header("무기")]

    [SerializeField] private int damage;
    [SerializeField] private float rate;
    [SerializeField] private float atkDelay;
    [SerializeField] private float effectTime;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        detect = GetComponent<PlayerDetect>();
        skill = GetComponent<PlayerSkill>();
        animator = GetComponent<Animator>();


    }

    void Update()
    {
        weapon.atkDelay = atkDelay;
        weapon.rate = rate;
        weapon.damage = damage;
        weapon.effectTime = effectTime;

        Vector3 cameraForward = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1)).normalized;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        dir = z * cameraForward + x * cam.transform.right;

        CheckGround();
        Attack();
        animator.SetInteger("Attack", weapon.attackLv);

        if (isAttackReady == false)
        {
            atkDeley += Time.deltaTime;
        }

        if (weapon.rate < atkDeley)
        {
            isAttackReady = true;
        }

        if (!isGround)
        {
            animator.SetBool("jump", true);
        }
        else if(isGround)
        {
            animator.SetBool("jump", false);
        }
        if (isDoubleJump)
        {
            animator.SetBool("DoubleJump", true);
        }
        else if(!isDoubleJump)
        {
            animator.SetBool("DoubleJump", false);
        }
        if (isDashing)
        {
            animator.SetBool("Dash", true);
        }
        else if (!isDashing)
        {
            animator.SetBool("Dash", false);
        }


        if (isAttackReady && !weapon.isAtkTime && !skill.isSkillTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isGround == true)
                {
                    Jump();
                }
                else if (isDoubleJump == false)
                {
                    Jump();
                    isDoubleJump = true;
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
               skill.TriggerSkill();
            }

        }

        if (Input.GetKeyDown(KeyCode.E) && !isInteracting && currentInteractableObject != null)
        {
            // 상호 작용 시작
            StartInteraction();
        }
        else if (Input.GetKeyDown(KeyCode.E) && isInteracting)
        {
            // 상호 작용 중지
            EndInteraction();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }    
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isCooldown)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            isNextAtk = true;
        }


    }

    private void FixedUpdate()
    {
        if (isAttackReady && !weapon.isAtkTime && !skill.isSkillTime)
        {
            if (dir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotspeed);
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
    void Jump()
    {
        Vector3 jumpPower = Vector3.up * jumpHeight;
        rb.AddForce(jumpPower, ForceMode.VelocityChange);

    }

    private void CheckGround()
    {

        if (Physics.BoxCast(transform.position + (Vector3.up * groundCheck), transform.lossyScale / 2.0f, Vector3.down, out RaycastHit hit, transform.rotation, 0.1f, layer))
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
        isCooldown = true;

        weapon.isAtkTime = false;
        rb.useGravity = false;
        Vector3 dashPower = dir * dash;
        rb.AddForce(dashPower, ForceMode.VelocityChange);

        isDashing = true;

        float delay = 0;
        while (delay < dashTime)
        {
            delay += Time.deltaTime;
            yield return null;
        }
        rb.useGravity = true;
        rb.velocity = Vector3.zero;

        isDashing = false;

        yield return new WaitForSeconds(dashCoolTime);

        isCooldown = false;
    }

    void Attack()
    {
        if (isGround == true && isAttackReady == true && isNextAtk == true)
        {
            //animator.SetBool("Move", false);
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
    void StartInteraction()
    {
        isInteracting = true;
        currentInteractableObject.GetComponent<InteractableObject>().Interact();
    }

    // 상호 작용 종료 함수
    void EndInteraction()
    {
        isInteracting = false;
        currentInteractableObject.GetComponent<InteractableObject>().EndInteract();
    }
}

