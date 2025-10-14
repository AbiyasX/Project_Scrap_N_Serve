using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public interface Iinteract
{
    void Interact();
}

public class PlayerControls : MonoBehaviour
{
    private InputSystem_Actions inputActions;
    private Vector2 moveInput;
    [Header("Player Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
   
    [Header("Player Dash")]
    [SerializeField] private float dashCooldown = 0.5f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] float dashForce = 10f;
    [SerializeField] bool canDash = true;
    bool isDashing = false;
    [Header("Interaction")]
    [SerializeField] private float rayHeight = 1.5f;


    private PickUpSystem interactObj;
    Animator playerAnim;
    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        interactObj = GetComponent<PickUpSystem>();
        playerAnim = GetComponent<Animator>();
    }

    #region InputSystem !!Do Not Edit!!

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Interact.performed += Interact_performed;
        // move input
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        // throw
        inputActions.Player.Throw.performed += Throw_performed;
        inputActions.Player.Throw.canceled += Throw_canceled;
        // pick up
        inputActions.Player.PickUp.performed += PickUp_performed;
        // Dash
        inputActions.Player.Dash.performed += Dash_performed;
    }

    

    private void OnDisable()
    {
        inputActions.Player.Interact.performed -= Interact_performed;
        // move input
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        // throw
        inputActions.Player.Throw.performed -= Throw_performed;
        inputActions.Player.Throw.canceled -= Throw_canceled;
        // pick up
        inputActions.Player.PickUp.performed -= PickUp_performed;
        // dash
        inputActions.Player.Dash.performed -= Dash_performed;
        inputActions.Player.Disable();
    }

    private void Interact_performed(InputAction.CallbackContext obj)
    {
        float interactRadius = 1.5f;
        Vector3 center = transform.position + Vector3.up * rayHeight;

        Collider[] hits = Physics.OverlapSphere(center, interactRadius, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);

        foreach (Collider hit in hits)
        {
            Iinteract interactable = hit.GetComponent<Iinteract>();
            if (interactable != null)
            {
                interactable.Interact();
                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = transform.position + Vector3.up * rayHeight;
        float interactRadius = 1.5f;

        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawWireSphere(center, interactRadius);
    }

    private void Dash_performed(InputAction.CallbackContext obj)
    {
        player_dash();
    }

    private void PickUp_performed(InputAction.CallbackContext obj)
    {
        interactObj.PickUpItem();
    }

    private void Throw_canceled(InputAction.CallbackContext obj)
    {
        interactObj.ChargeAndThrow(false);
    }

    private void Throw_performed(InputAction.CallbackContext obj)
    {
        interactObj.ChargeAndThrow(true);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    #endregion InputSystem !!Do Not Edit!!

    private void Update()
    {
        player_movement();
    }

    private void player_movement()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

        if (move.magnitude > 0.1f)
        {
            transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);

            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            playerAnim.SetBool("IsWalking", true);
        }
        else
        {
            playerAnim.SetBool("IsWalking", false);
        }

    }

    private void player_dash()
    {
        if (!canDash || isDashing) return;
        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;

        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            transform.Translate(transform.forward * dashForce * Time.deltaTime, Space.World);
            yield return null;
        }

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}