using System.Collections;
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
    
    public float bulletSpeed = 10f;
    
    public float currentMoveSpeed;
    
    public float life = 0.1f;

    public float shootDelay = 0.1f;

    public Light bigFlashLight;

    public Animator playerAnimator;
    
    private FlashLightController flashLightController;

    public bool isFiring;
    public bool isUsingFlashLight;

    public GameObject Bullet;

    public float Dammage;
    
    private Camera playerCam;
    private Transform playerCamTransform;

    // public Color lifeLightColor;
    // public Color lifeLightColorRed;

    public Transform weaponSlot;

    public bool isOperational = true;
    public bool isMoving = false;

    public bool isCranking = false;
    public bool isFlashing = false;
    public bool ennemiesNearby = false;
	public AudioSource crankingSource;
    public AudioClip flashSound;
    public AudioSource source;

    public GameObject Gun;
    public GameObject Lamp;

    private LayerMask enemyLayer;

    public GameObject muzzleFlash;

    public float lampPressedTime;

    private float ratio;

    private float shootTimer;

    private void Start()
    {
        // lifeLightColor = lifeLight.color;
        SceneManager.LoadScene("Environnment", LoadSceneMode.Additive);

        playerCam = Camera.main;
        playerCamTransform = playerCam.transform.parent;
        
        flashLightController = GetComponentInChildren<FlashLightController>();

        currentMoveSpeed = moveSpeed;

        
        enemyLayer = LayerMask.GetMask("Enemy");
        crankingSource = Lamp.GetComponent<AudioSource>();

        ratio = Screen.width / 480;
        Debug.Log(ratio);
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
        if (ctx.performed && !isCranking)
        {
            playerAnimator.SetFloat("isCranking", 1f);
            Lamp.SetActive(false);
            Gun.SetActive(true);
            isUsingFlashLight = false;

            isFiring = true;
            if (isFlashing)
            {
                StopCoroutine(StartBigFlash());
                bigFlashLight.gameObject.SetActive(false);
            }

        }
        if (ctx.canceled)
        {
            isFiring = false;
        }
    }
    
    public void OnFlashLightCtx(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isFiring)
        {
            lampPressedTime = Time.time;
            playerAnimator.SetFloat("isCranking", 0f);
            Lamp.SetActive(true);
            Gun.SetActive(false);
            isUsingFlashLight = false;

            crankingSource.Play();

            if (!isCranking)
            {
                isCranking = true;
            }
        }
        if (ctx.canceled)
        {
            float delay = Time.time - lampPressedTime;
            if(delay > 1f && !isFiring)
            {
                StartCoroutine(nameof(StartBigFlash));
            }
            else
            {
				playerAnimator.SetFloat("isCranking", 1f);
				Lamp.SetActive(false);
				Gun.SetActive(true);
				isUsingFlashLight = false;
                isCranking = false;
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

    IEnumerator StartBigFlash()
    {
        isFlashing = true;
     //   yield return new WaitForSeconds(crankingSource.clip.length / crankingSource.pitch);
        isCranking = false;
        bigFlashLight.gameObject.SetActive(true);
        var maxIntensity = bigFlashLight.intensity;
        var t = 0f;
        bigFlashLight.intensity = 0f;
        
        source.PlayOneShot(flashSound);
        while (true)
        {
            t += Time.deltaTime * 20f;
            bigFlashLight.intensity = Mathf.Lerp(0f, maxIntensity, t);
            if (Mathf.Approximately(bigFlashLight.intensity, maxIntensity)) break;
            yield return null;
        }
        isUsingFlashLight = true;
        //flashing
        yield return new WaitForSeconds(0.3f);
        bigFlashLight.gameObject.SetActive(false);
        isUsingFlashLight = false;
        isFlashing = false;
		playerAnimator.SetFloat("isCranking", 1f);
		Lamp.SetActive(false);
		Gun.SetActive(true);
	}
    
    // void OnCrankingFinished()
    // {
    //     isCranking = false;
    //     source.PlayOneShot(flashSound);
    //     Invoke("OnFlashFinished", 1.2f);
    //     flashLight.intensity *= 10f;
    //     //DO FLASH
    // }
    // void OnFlashFinished()
    // {
    //     flashLight.intensity /= 10f;
    // }
    
    public void OnHit()
    {
        FXManager.instance.ShowVignette();
    }
    
    
    public void UpdateLife(float life)
    {
        flashLightController.UpdateLife(life);
        
        if (GameManager.instance.isStartingFromNothing && this.life > life)
        {
            return; 
        }
        
        this.life = Mathf.Min(life, 10f);
        
        if (life <= 0 && isOperational)
        {
            isOperational = false;
            playerAnimator.SetTrigger("OnDeath");
            GameManager.instance.OnGameOver();
        }
    }

    private void Update()
    {
        Fire();
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
                Vector3 lookAt = playerCam.ScreenToWorldPoint(new Vector3(m.x/ratio, m.y/ratio, depth));
                lookAt.y = transform.position.y; // yaw uniquement
                transform.rotation = Quaternion.LookRotation(lookAt - transform.position, Vector3.up);
            }


            // lifeLight.innerSpotAngle = Mathf.Lerp(10f, 180f, life / GameManager.instance.maxLife);
            // lifeLight.spotAngle = Mathf.Lerp(10f, 180f, life / GameManager.instance.maxLife);
        }
    }

    void Fire()
    {
        if (shootTimer >= 0)
        {
			shootTimer -= Time.deltaTime;
		}
		if (isOperational)
        {
            if (isFiring && shootTimer <= 0)
            {
                shootTimer = shootDelay;
                Quaternion fireRotation = transform.rotation;
                Vector3 fireDirection = transform.forward;
                if (Physics.Raycast(playerCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, enemyLayer))
                {
                    if (hit.collider is CapsuleCollider)
                    {
                        var newFireDirection = (hit.transform.position + Vector3.up - weaponSlot.transform.position).normalized;
                        var newFireRotation = Quaternion.LookRotation(hit.transform.position + Vector3.up - weaponSlot.transform.position, Vector3.up);
                        if (Vector3.Angle(fireDirection, newFireDirection) < 20f)
                        {
                            fireDirection = newFireDirection;
                            fireRotation = newFireRotation;
                        }
                    }
                }
                // Quaternion.LookRotation(lookAt + Vector3.up - weaponSlot.transform.position)
                GameObject bullet = Instantiate(Bullet, weaponSlot.transform.position, fireRotation);
                bullet.GetComponent<BulletTrigger>().Dammage = Dammage;
                bullet.GetComponent<Rigidbody>().linearVelocity = fireDirection * bulletSpeed;
                bullet.transform.rotation = fireRotation;
                // bullet.transform.rotation = Quaternion.Euler(90, this.transform.rotation.eulerAngles.y, 0);
                if (muzzleFlash) StartCoroutine(nameof(MuzzleFlash));
            }
        }
    }

    IEnumerator MuzzleFlash()
    {
        muzzleFlash.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.gameObject.SetActive(false);
    }
}
