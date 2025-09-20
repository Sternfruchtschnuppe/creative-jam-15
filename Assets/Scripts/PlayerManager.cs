using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [Header("Reglages")]
    public float moveSpeed = 5f;
    public float smoothingSpeed = 20f;
    public float sprintSpeed = 7f;
    private Vector2 rawInput;
    private Vector2 lerpedInput;
    
    private float currentMoveSpeed;
    
    public float life = 0.1f;

    public Light flashLight;

    public Animator playerAnimator;
    
    private FlashLightController flashLightController;

    public bool isFiring;

    public GameObject Bullet;

    public float Dammage = 0.5f;
    
    private Camera playerCam;
    private Transform playerCamTransform;
    
    private void Start()
    {
        playerCam = Camera.main;
        playerCamTransform = playerCam.transform.parent;
        
        flashLightController = GetComponentInChildren<FlashLightController>();

        currentMoveSpeed = moveSpeed;
        InvokeRepeating("Fire", 0.1f, 0.2f);
    }

    public void OnMoveCtx(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            rawInput = ctx.ReadValue<Vector2>();
            playerAnimator.SetBool("isMoving", true);
        }
        if (ctx.canceled)
        {
            rawInput = Vector2.zero;
            playerAnimator.SetBool("isMoving", false);
        }
    }
    public void OnSprintCtx(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            currentMoveSpeed = sprintSpeed;
            playerAnimator.SetBool("isSprinting", true);
        }
        if (ctx.canceled)
        {
            currentMoveSpeed = moveSpeed;
            playerAnimator.SetBool("isSprinting", false);
        }
    }

    public void OnAttackCtx(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
        //    moveSpeed = 7f;
            isFiring = true;
       //     playerAnimator.SetBool("isSprinting", true);
        }
        if (ctx.canceled)
        {
            isFiring = false;
         //   moveSpeed = 4f;
         //   playerAnimator.SetBool("isSprinting", false);
        }
    }

    public void UpdateLife(float life)
    {
        this.life = life;
        flashLightController.UpdateLife(life);
        
        if (life <= 0)
        {
            GameManager.instance.OnGameOver();
        }
    }

    private void Update()
    {
        lerpedInput = Vector2.Lerp(lerpedInput, rawInput, Time.deltaTime * smoothingSpeed);
        
        Vector3 movement = playerCamTransform.rotation * new Vector3(lerpedInput.x, 0, lerpedInput.y);
        
        transform.position += movement * (currentMoveSpeed * Time.deltaTime);
        
        // playerCam.transform.position = initialVector + this.transform.position;

        Vector2 m = Mouse.current.position.ReadValue();

        float depth = playerCam.WorldToScreenPoint(transform.position).z;
        if (depth > 0f) 
        {
            Vector3 lookAt = playerCam.ScreenToWorldPoint(new Vector3(m.x, m.y, depth));
            lookAt.y = transform.position.y; // yaw uniquement
            transform.rotation = Quaternion.LookRotation(lookAt - transform.position, Vector3.up);
        }


        flashLight.innerSpotAngle = Mathf.Lerp(10f, 180f, life / GameManager.instance.maxLife);
        flashLight.spotAngle = Mathf.Lerp(10f, 180f, life / GameManager.instance.maxLife);

     //   flashLight.range = Mathf.Lerp(1f, 10f, life / gameManager.maxLife);



    }
    void Fire()
    {
        if (isFiring)
        {
            GameObject bullet = Instantiate(Bullet, this.transform.position, Quaternion.Euler(0,0,0));
            bullet.GetComponent<BulletTrigger>().Dammage = Dammage;
            bullet.GetComponent<Rigidbody>().linearVelocity = this.transform.forward * 10f;
            bullet.transform.rotation = Quaternion.Euler(90, this.transform.rotation.eulerAngles.y, 0);
        }
    }
}
