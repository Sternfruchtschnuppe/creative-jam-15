using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    [Header("Reglages")]
    public float moveSpeed = 5f;
    public float smoothingSpeed = 20f;
    public float sprintSpeed = 7f;
    private Vector2 rawInput;
    private Vector2 lerpedInput;
    
    public float currentMoveSpeed;
    
    public float life = 0.1f;

    public Light lifeLight;
    public Light flashLight;

    public Animator playerAnimator;
    
    private FlashLightController flashLightController;
 //   private FlashLightController flashLightController2;

    public bool isFiring;
    public bool isUsingFlashLight;

    public GameObject Bullet;

    public float Dammage = 0.5f;
    
    private Camera playerCam;
    private Transform playerCamTransform;

    public Color lifeLightColor;
    public Color lifeLightColorRed;

    public Transform weaponSlot;

    public bool isOperational = true;
    public bool isMoving = false;

    public bool isCranking = false;
    public AudioClip crankingSound;
    public AudioClip flashSound;
    public AudioSource source;

    public GameObject Gun;
    public GameObject Lamp;
    public Light LampFlashLight;

    private void Start()
    {
        lifeLightColor = lifeLight.color;
        SceneManager.LoadScene("Environnment", LoadSceneMode.Additive);

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
            isMoving = true;
        }
        if (ctx.canceled)
        {
            rawInput = Vector2.zero;
            playerAnimator.SetBool("isMoving", false);
            isMoving = false;
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



            isFiring = true;
            if (Lamp.activeSelf)
            {
                Gun.SetActive(true);
                Lamp.SetActive(false);
            }

        }

        if (ctx.canceled)
        {
            isFiring = false;
        }
    }
    public void OnSwitchCtx(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        
    }

    public void OnFlashLightCtx(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (Gun.activeSelf)
            {
                Gun.SetActive(false);
                Lamp.SetActive(true);
            }

            if (!isCranking)
            {
                isCranking = true;
                source.PlayOneShot(crankingSound, 1);
                Invoke("OnCrankingFinished", crankingSound.length);
            }
        }
        //todo insert secondary weapon / flash light flashing
        // if (ctx.performed)
        // {
        //     isUsingFlashLight = true;
        // }
        // if (ctx.canceled)
        // {
        //     isUsingFlashLight = false;
        // }
    }
    void OnCrankingFinished()
    {
        isCranking = false;
        source.PlayOneShot(flashSound);
        Invoke("OnFlashFinished", 1.2f);
        LampFlashLight.intensity *= 100f;
        //DO FLASH
    }
    void OnFlashFinished()
    {
        LampFlashLight.intensity /= 100f;
    }
    public void OnHit()
    {
        FXManager.instance.ShowVignette();
    }
    
    
    public void UpdateLife(float life)
    {
        this.life = Mathf.Min(life, 10f);
        flashLightController.UpdateLife(life);
        
        if (life <= 0 && isOperational)
        {
            isOperational = false;
            playerAnimator.SetTrigger("OnDeath");
            GameManager.instance.OnGameOver();
        }
    }

    private void Update()
    {
        if (isOperational)
        {
            lerpedInput = Vector2.Lerp(lerpedInput, rawInput, Time.deltaTime * smoothingSpeed);

            Vector3 movement = playerCamTransform.rotation * new Vector3(lerpedInput.x, 0, lerpedInput.y);

            float angle = this.transform.rotation.eulerAngles.y - 45;

            Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.up);
            Vector3 animInput = rotation * movement;

            float angle2 = 45;
            Quaternion rotation2 = Quaternion.AngleAxis(-angle2, Vector3.up);
            Vector3 animInput2 = rotation2 * animInput;
           
            playerAnimator.SetFloat("Vx", animInput2.x);
            playerAnimator.SetFloat("Vz", animInput2.z);

            transform.position += movement * (currentMoveSpeed * Time.deltaTime);

            Vector2 m = Mouse.current.position.ReadValue();

            float depth = playerCam.WorldToScreenPoint(transform.position).z;
            if (depth > 0f)
            {
                Vector3 lookAt = playerCam.ScreenToWorldPoint(new Vector3(m.x, m.y, depth));
                lookAt.y = transform.position.y; // yaw uniquement
                transform.rotation = Quaternion.LookRotation(lookAt - transform.position, Vector3.up);
            }


            lifeLight.innerSpotAngle = Mathf.Lerp(10f, 180f, life / GameManager.instance.maxLife);
            lifeLight.spotAngle = Mathf.Lerp(10f, 180f, life / GameManager.instance.maxLife);
        }
    }

    void Fire()
    {
        if (isOperational)
        {
            if (isFiring)
            {
                GameObject bullet = Instantiate(Bullet, weaponSlot.transform.position, Quaternion.Euler(0, 0, 0));
                bullet.GetComponent<BulletTrigger>().Dammage = Dammage;
                bullet.GetComponent<Rigidbody>().linearVelocity = this.transform.forward * 10f;
                bullet.transform.rotation = Quaternion.Euler(90, this.transform.rotation.eulerAngles.y, 0);
            }
        }
    }
}
