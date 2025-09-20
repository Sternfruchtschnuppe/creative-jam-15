using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [Header("Reglages")]
    public float moveSpeed = 5f;

    private Vector2 moveInput;

    public Camera playerCam;

    public Vector3 initialVector;

    public float life = 0.1f;

    public Light flashLight;

    public Animator playerAnimator;
    
    private FlashLightController flashLightController;

    public GameManager gameManager;
    
    private void Start()
    {
        initialVector = playerCam.transform.position - this.transform.position;
        flashLightController = GetComponentInChildren<FlashLightController>();

        InvokeRepeating("PassiveDecreaseLife", 0.1f, 2.5f);
    }

    public void OnMoveCtx(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            moveInput = ctx.ReadValue<Vector2>();
            playerAnimator.SetBool("isMoving", true);
        }
        if (ctx.canceled)
        {
            moveInput = Vector2.zero;
            playerAnimator.SetBool("isMoving", false);
        }
    }
    public void OnSprintCtx(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            moveSpeed = 7f;
            playerAnimator.SetBool("isSprinting", true);
        }
        if (ctx.canceled)
        {
            moveSpeed = 4f;
            playerAnimator.SetBool("isSprinting", false);
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
        
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);

        
        transform.position += movement * moveSpeed * Time.deltaTime;
        playerCam.transform.position = initialVector + this.transform.position;

        Vector2 m = Mouse.current.position.ReadValue();

        float depth = playerCam.WorldToScreenPoint(transform.position).z;
        if (depth > 0f) 
        {
            Vector3 lookAt = playerCam.ScreenToWorldPoint(new Vector3(m.x, m.y, depth));
            lookAt.y = transform.position.y; // yaw uniquement
            transform.rotation = Quaternion.LookRotation(lookAt - transform.position, Vector3.up);
        }


        flashLight.innerSpotAngle = Mathf.Lerp(10f, 180f, life / gameManager.maxLife);
        flashLight.spotAngle = Mathf.Lerp(10f, 180f, life / gameManager.maxLife);

    }
    void PassiveDecreaseLife()
    {
        life -= 0.5f;
    }
}
