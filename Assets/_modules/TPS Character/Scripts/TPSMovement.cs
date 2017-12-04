using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TPSMovement : NetworkBehaviour
{

    #region Public members

    [Header("Walking")]
    public float m_walkingSpeed;
    public float m_minWalkingSpeed;
    [Header("Turning")]
    public float m_maxDeltaRotation;
    public float m_angleBeginTurnLowerBody;
    [Header("Camera")]
    public Vector3 m_cameraOffsetToPlayer;
    public float m_cameraPreview;
    public float m_cameraSmoothing;
    [Header("Misc")]
    public float m_cursorPlaneHeight;

    #endregion

    #region Private members

    private CharacterController m_characterController;

    private Transform m_cameraTransform;
    private Vector3 m_cameraVelocity;

    private Vector3 m_screenMovementForward;
    private Vector3 m_screenMovementRight;
    [SyncVar]
    private Vector3 m_movementDirection;

    private Plane m_playerMovementPlane;
    
    private Vector3 m_lowerBodyForward;
    private Vector3? m_lowerBodyForwardTarget;
    
    #endregion

    #region Properties

    // SyncVar variables won't sync from client to server.
    // So we have to manually sync when variables have any changes.

    private Vector3 MovementDirection
    {
        get
        {
            return m_movementDirection;
        }
        set
        {
            if (m_movementDirection != value)
            {
                m_movementDirection = value;
                CmdSetMovementDirection(m_movementDirection);
            }
        }
    }

    private Vector3 LowerBodyForward
    {
        get
        {
            return m_lowerBodyForward;
        }
        set
        {
            if (m_lowerBodyForward != value)
            {
                m_lowerBodyForward = value;
                CmdSetLowerBodyForward(m_lowerBodyForward);
            }
        }
    }

    public TPSHeroProperties ActiveHero { get; set; }

    #endregion

    #region Start/Update

    // Use this for initialization
    void Start () {
        m_characterController = GetComponent<CharacterController>();

        m_cameraTransform = Camera.main.transform;
        
        m_screenMovementForward = Vector3.forward;
        m_screenMovementRight = Vector3.right;
        m_movementDirection = Vector3.zero;

        m_playerMovementPlane = new Plane();
        m_cameraVelocity = Vector3.zero;

        m_lowerBodyForwardTarget = null;
        m_lowerBodyForward = Vector3.forward;
    }
	
	// Update is called once per frame
	void Update () {
        if (isLocalPlayer)
        {
            MovementDirection =
                MyInput.GetAxis("Horizontal") * m_screenMovementRight + 
                MyInput.GetAxis("Vertical") * m_screenMovementForward;

            m_playerMovementPlane.normal = transform.up;
            //Debug.Log("transform.position.y = " + transform.position.y);
            m_playerMovementPlane.distance = -transform.position.y + m_cursorPlaneHeight;

            HandleCameraMovement();
        }
        
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        HandleMovement();
        HandleRigidbodyRotation();
    }

    void LateUpdate()
    {
        if (m_characterController.velocity.magnitude < m_minWalkingSpeed)
        {
            HandleUpperBodyRotation();
        }
        else
        {
            m_lowerBodyForwardTarget = null;
        }
    }

    #endregion
    
    #region Movement

    void HandleMovement()
    {
        var targetVelocity = MovementDirection;
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= m_walkingSpeed;
        targetVelocity.y = -10;
        
        m_characterController.Move(targetVelocity * Time.deltaTime);
    }

    #endregion

    #region Rotation

    void HandleRigidbodyRotation()
    {
        var cursorScreenPosition = MyInput.GetMousePosition();
        var cursorWorldPosition = Utils.ScreenPointToWorldPointOnPlane(cursorScreenPosition, m_playerMovementPlane, Camera.main);

        // The facing direction is the direction from the character to the cursor world position
        var facingDirection = (cursorWorldPosition - transform.position);
        facingDirection.y = 0;

        // Make the character rotate towards the target rotation
        var newRotatation = Quaternion.LookRotation(facingDirection);
        transform.rotation = newRotatation;
    }

    void HandleUpperBodyRotation()
    {
        if (isLocalPlayer)
        {
            if (m_lowerBodyForwardTarget == null)
            {
                m_lowerBodyForwardTarget = transform.forward;
                LowerBodyForward = m_lowerBodyForwardTarget.Value;
            }

            // Turn the lower body towards it's target direction
            var t = m_maxDeltaRotation * Time.deltaTime * Mathf.Deg2Rad;
            LowerBodyForward = Vector3.RotateTowards(LowerBodyForward, m_lowerBodyForwardTarget.Value, t, 0.0f);
        }
        
        // Calculate delta angle to make the lower body stay in place
        float lowerBodyDeltaAngle = Mathf.DeltaAngle(
            HorizontalAngle(transform.forward),
            HorizontalAngle(LowerBodyForward)
        );

        // If the body is twisted more than particular amount of degrees,
        // set a new target direction for the lower body, so it begins turning
        if (Mathf.Abs(lowerBodyDeltaAngle) > m_angleBeginTurnLowerBody)
        {
            m_lowerBodyForwardTarget = transform.forward;
        }

        // Create a Quaternion rotation from the rotation angle
        var lowerBodyDeltaRotation = Quaternion.Euler(0, lowerBodyDeltaAngle, 0);

        if (ActiveHero != null)
        {
            // Rotate the whole body by the angle
            ActiveHero.m_rootBone.rotation = lowerBodyDeltaRotation * ActiveHero.m_rootBone.rotation;

            // Counter-rotate the upper body so it won't be affected
            ActiveHero.m_upperBodyBone.rotation = Quaternion.Inverse(lowerBodyDeltaRotation) * ActiveHero.m_upperBodyBone.rotation;
        }
    }

    static float HorizontalAngle(Vector3 direction)
    {
        return (Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg);
    }

    #endregion

    #region Misc

    void UpdateAnimator()
    {
        if (ActiveHero != null)
        {
            ActiveHero.m_animator.SetFloat("XVel", MovementDirection.x);
            ActiveHero.m_animator.SetFloat("ZVel", MovementDirection.z);
        }
    }

    void HandleCameraMovement()
    {
        Vector3 cameraAdjustmentVector;

        // On PC, the cursor point is the mouse position
        var cursorScreenPosition = MyInput.GetMousePosition();
        
        var halfWidth = Screen.width / 2.0f;
        var halfHeight = Screen.height / 2.0f;
        var maxHalf = Mathf.Max(halfWidth, halfHeight);

        // Acquire the relative screen position			
        var posRel = cursorScreenPosition - new Vector3(halfWidth, halfHeight, cursorScreenPosition.z);
        posRel.x /= maxHalf;
        posRel.y /= maxHalf;

        cameraAdjustmentVector = posRel.x * m_screenMovementRight + posRel.y * m_screenMovementForward;
        cameraAdjustmentVector.y = 0.0f;

        // Set the target position of the camera to point at the focus point
        var cameraTargetPosition = transform.position + m_cameraOffsetToPlayer + cameraAdjustmentVector * m_cameraPreview;

        // Apply some smoothing to the camera movement
        m_cameraTransform.position = Vector3.SmoothDamp(
            m_cameraTransform.position, 
            cameraTargetPosition, 
            ref m_cameraVelocity, 
            m_cameraSmoothing);
    }

    #endregion

    #region RPC functions

    [Command]
    void CmdSetMovementDirection(Vector3 dir)
    {
        m_movementDirection = dir;
    }

    [Command]
    void CmdSetLowerBodyForward(Vector3 dir)
    {
        RpcSetLowerBodyForward(dir);
    }

    [ClientRpc]
    void RpcSetLowerBodyForward(Vector3 dir)
    {
        if (!isLocalPlayer)
        {
            m_lowerBodyForward = dir;
        }
    }

    #endregion

}
