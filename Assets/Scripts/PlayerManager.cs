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
    
    private void Start()
    {
        initialVector = playerCam.transform.position - this.transform.position;
        flashLightController = GetComponentInChildren<FlashLightController>();
    }

    public void OnMoveCtx(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            moveInput = ctx.ReadValue<Vector2>();
            // playerAnimator.SetBool("isMoving", true);
        }
        if (ctx.canceled)
        {
            moveInput = Vector2.zero;
            // playerAnimator.SetBool("isMoving", false);
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


        flashLight.innerSpotAngle = Mathf.Lerp(10f, 90f, life);
        flashLight.spotAngle = Mathf.Lerp(10f, 90f, life);

    }
}
