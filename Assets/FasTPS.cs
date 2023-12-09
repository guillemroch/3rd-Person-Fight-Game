/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Cinemachine;
using FasTPS;

namespace FasTPS
{
    public class CharacterMovement : MonoBehaviour
    {
        #region Variables
        ///////////////////////////////////////////////////////////
        /////////////////PUBLIC VARIABLES/////////////////////////
        ///////////////////////////////////////////////////////////
        [Header("Character Movement")]
        public float WalkSpeed = 2f;
        public float CrouchSpeed = 1.5f;
        public float SprintSpeed = 3.5f;
        public float SlideSpeed = 4.5f;
        public float jumpHeight = 2f;
        public float BackwardsSpeedDropOff = 0.75f; //When running backwards we should go slower. This is the Speed Drop Off.
        public float Gravity = -19.62f;
        public float LedgeHeightFallRoll = 8f;
        public float groundDistance = 0.4f;
        public float turnSpeed = 15f;
        public float turnSmoothTime = 0.1f;

        [Header("Character Vaulting")]
        public float distanceToCheckForward = 0.75f;
        public float vaultOverHeight = 1.5f;
        public float vaultFloorHeighDifference = 0.3f;
        public float vaultCheckDistance = 2.25f;
        public AnimationCurve VaultCurve;
        public float climbMaxHeight = 2;
        public float walkUpHeight = 1;
        public float WalkUpThreshold = 0.4f;
        public AnimationCurve WalkUpCurve;
        public LayerMask GroundMask;

        [Header("Character Cover")]
        public float minDistanceToWallCheck = 2.0f;
        public float minStandCover = 1.0f;
        public float minCrouchCover = 0.4f;
        public float wallDistance = 1.0f;

        [Header("Customizable")]
        public bool AutoRotation = true;
        public bool Analog = false;
        public bool StrafeRun = true;
        public bool Covering = true;
        public bool Sliding = true;
        public bool Vaulting = true;
        public bool AutoStep = true;
        public bool LandingRoll = true;
        public bool Footsteps = true;

        [Header("Components")]
        public Transform Cam;
        private Animator animator;
        public Transform GroundCheck;
        public Transform Orientation;
        public GameObject MenuPanel;
        private CharacterController Controller;
        private PlayerInput PlayerInput;

        ///////////////////////////////////////////////////////////
        /////////////////PRIVATE VARIABLES/////////////////////////
        ///////////////////////////////////////////////////////////
        [HideInInspector]
        public bool SteepSlope;
        [HideInInspector]
        public bool IsJumping;
        [HideInInspector]
        public bool IsVaulting;
        [HideInInspector]
        public bool IsClimbUp;
        [HideInInspector]
        public bool IsSliding;
        [HideInInspector]
        public bool IsGrounded;
        [HideInInspector]
        public bool IsSprinting;
        [HideInInspector]
        public bool MenuOpen;
        [HideInInspector]
        public bool IsCovering;
        bool IsCrouching;
        bool IsWalkUp;
        bool initVault = false;
        bool obstacleForward;
        bool groundForward;
        bool RollFromJump;
        public bool isGroundForward;
        Vector3 targetVaultPosition;
        Vector3 MoveDirection;
        Vector3 velocity;
        Vector3 startPosition;
        Vector3 overrideDirection;
        Quaternion desiredCoverRot;
        float newCentreY = 0.5f;
        float newHeight = 0.5f;
        Vector3 originalCentre;
        float originalHeight;
        float fc_t = 0;
        float SlideTime = 0.5f;
        float ForceOverLife;
        float forceOverrideTimer;
        float turnSmoothVelocity;
        float overrideSpeed;
        float SlideAngle;
        Vector3 impact = Vector3.zero;

        bool forceOverHasRan;
        delegate void ForceOverrideStart();
        ForceOverrideStart forceOverStart;
        delegate void ForceOverrideWrap();
        ForceOverrideWrap forceOverWrap;

        // Estas son nuestras
        private bool paused;
        #endregion

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            PlayerInput = GetComponentInParent<PlayerInput>();
            Controller = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            Controller.skinWidth = 0.001f;
            originalCentre = Controller.center;
            originalHeight = Controller.height;
            paused = false;
        }

        #region FootstepSounds
        public void FootstepSounds()
        {
            if (!Footsteps) { return; }
            GetComponentInChildren<StandardFootstepManager>().FootstepSounds(IsSprinting);
        }
        #endregion
        #region Loops
        void Update()
        {
            HandleMenu();
            HandleStates();
            SlopeSliding();
            if (MenuOpen) { return; }
            HandleMovement();
            HandleGravity();
            HandleCovering();
            if (impact.magnitude > 0.2f)
            {
                Controller.Move(impact * Time.deltaTime);
                impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
            }
        }
        private void FixedUpdate()
        {
            HandleRotation();
            FixedTick();
        }
        void FixedTick()
        {
            Vector3 v = transform.forward * PlayerInput.MoveInput.y;
            Vector3 h = transform.right * PlayerInput.MoveInput.x;

            v.y = 0;
            h.y = 0;

            if (!GetComponent<ClimbBehaviour>().climbing)
            {
                bool isGround = isGroundTowardsDirection((v + h).normalized);
                isGroundForward = isGround;
            }

            if (GetComponent<ClimbBehaviour>().climbing)
                isGroundForward = true;

            if (PlayerInput.DropLedge)
            {
                PlayerInput.DropLedge = false;
            }

            if (IsSliding)
            {
                Controller.center = new Vector3(Controller.center.x, newCentreY, Controller.center.z);
                Controller.height = newHeight;
            }
            else
            {
                Controller.center = originalCentre;
                Controller.height = originalHeight;
            }
            if (IsVaulting)
            {
                if (!initVault)
                {
                    VaultLogicInit();
                    initVault = true;
                }
                else
                {
                    HandleVaulting();
                }
                return;
            }
            obstacleForward = false;
            groundForward = false;
            if (IsGrounded && !IsCovering)
            {
                Vector3 origin = transform.position;
                origin += Vector3.up * 0.5f;
                IsClear(origin, transform.forward, distanceToCheckForward, ref obstacleForward);
                if (!obstacleForward && !IsVaulting)
                {
                    origin += transform.forward * 0.6f;
                    IsClear(origin, -Vector3.up, groundDistance * 3, ref groundForward);
                }
                else
                {
                    if (Vector3.Angle(transform.forward, MoveDirection) > 30)
                    {
                        obstacleForward = false;
                    }
                }
            }
        }
        #endregion
        #region MenuManager
        public void OpenMenu()
        {
            if (MenuPanel.activeSelf == true)
            {
                MenuPanel.SetActive(false);
                MenuOpen = false;
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                MenuPanel.SetActive(true);
                MenuOpen = true;
                Time.timeScale = 0; 
                EventSystem ESystem = GetComponentInChildren<EventSystem>();
                foreach (RebindUI UI in GetComponentsInChildren<RebindUI>())
                {
                    if (UI.actionName == "Crouch")
                    {
                        ESystem.SetSelectedGameObject(UI.GetComponentInChildren<Button>().gameObject);
                    }
                }
                GetComponentInChildren<KeybindUI>().CurrentPlatform = 0;
            }

        }
        void HandleMenu()
        {
            if (MenuOpen)
            {
                Cam.parent.GetComponent<CinemachineInputProvider>().enabled = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cam.parent.GetComponent<CinemachineInputProvider>().enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        #endregion
        #region SlopeLogic 
        void SlopeSliding()
        {
            RaycastHit SlopeHit;
            Vector3 alignment = new Vector3(0, 0.2f, 0f);
            if (Physics.Raycast(transform.position + alignment, Vector3.down, out SlopeHit, 0.5f, GroundMask))
            {
                SlideAngle = Vector3.Angle(Vector3.up, SlopeHit.normal);
            }
            else
            {
                SlideAngle = 0;
            }

            Quaternion lastRot = transform.GetChild(0).rotation;
            if (IsSliding && SlideAngle > 0 && !DirectionSlopeUp())
            {
                transform.GetChild(0).localRotation = Quaternion.Slerp(transform.GetChild(0).localRotation, Quaternion.Euler(SlideAngle, 0f, 0f), turnSpeed * Time.fixedDeltaTime);
            }
            else if (IsSliding && SlideAngle > 0 && DirectionSlopeUp())
            {
                transform.GetChild(0).localRotation = Quaternion.Slerp(transform.GetChild(0).localRotation, Quaternion.Euler(-SlideAngle, 0f, 0f), turnSpeed * Time.fixedDeltaTime);
            }
            else if (IsSliding)
            {
                transform.GetChild(0).localRotation = Quaternion.Slerp(transform.GetChild(0).localRotation, Quaternion.Euler(5f, 0f, 0f), turnSpeed * Time.fixedDeltaTime);
            }
            else
            {
                transform.GetChild(0).localRotation = Quaternion.Slerp(transform.GetChild(0).localRotation, Quaternion.Euler(0f, 0f, 0f), turnSpeed * Time.fixedDeltaTime);
            }
        }
        private Vector3 AdjustVelocityToSlope(Vector3 velocity)
        {
            Vector3 alignment = new Vector3(0, 0.1f, 0);
            var ray = new Ray(transform.position + alignment, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 0.2f))
            {
                var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
                var adjustedVelocity = slopeRotation * velocity;
                if (adjustedVelocity.y < -2f)
                {
                    return adjustedVelocity;
                }
            }
            return velocity;
        }
        bool DirectionSlopeUp()
        {
            if (SlideAngle == 0)
            {
                return false;
            }
            Vector3 alignment = new Vector3(0, 0.3f, 0);
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(transform.position + alignment, transform.forward, out hit, 0.5f, GroundMask))
            {
                Debug.DrawLine(transform.position + alignment, hit.point, Color.blue);
                return true;
            }
            else
            {
                Debug.DrawRay(transform.position + alignment, transform.forward, Color.blue);
                return false;
            }

        }
        #endregion
        #region CoverLogic
        (bool, int) CanCover()
        {
            Vector3 standCoverAlignment = new Vector3(0f, minStandCover + 0.2f, 0f);
            Vector3 crouchCoverAlignment = new Vector3(0f, minCrouchCover + 0.2f, 0f);

            if (!IsSliding && !DirectionSlopeUp() && !IsVaulting && !IsClimbUp && !IsWalkUp)
            {
                if (Physics.Raycast(transform.position + standCoverAlignment, transform.forward, out RaycastHit hitInfoStand, minDistanceToWallCheck, GroundMask))
                {
                    if (Vector3.Distance(transform.position + standCoverAlignment, hitInfoStand.point) > distanceToCheckForward || IsCovering)
                    {
                        desiredCoverRot = Quaternion.LookRotation(-hitInfoStand.normal);
                        return (true, 1);
                    }
                    return (false, 0);
                }
                else if (Physics.Raycast(transform.position + crouchCoverAlignment, transform.forward, out RaycastHit hitInfo, minDistanceToWallCheck, GroundMask))
                {
                    if (Vector3.Distance(transform.position + crouchCoverAlignment, hitInfo.point) > distanceToCheckForward && Vector3.Distance(transform.position + crouchCoverAlignment, hitInfo.point) < minDistanceToWallCheck || IsCovering)
                    {
                        desiredCoverRot = Quaternion.LookRotation(-hitInfo.normal);
                        return (true, 0);
                    }
                    return (false, 0);
                }
                return (false, 0);
            }
            return (false, 0);
        }
        public void Cover()
        {
            (bool, int) canCover = CanCover();
            if (!IsCovering)
            {
                animator.SetBool("IsCrouching", false);
                if (canCover == (true, 1))
                {
                    IsCovering = true;
                    animator.SetBool("IsCovering", true);
                    animator.SetInteger("CoverState", 1);
                }
                else if (canCover == (true, 0))
                {
                    IsCovering = true;
                    animator.SetBool("IsCovering", true);
                    animator.SetInteger("CoverState", 0);
                }
            }
            else
            {
                IsCovering = false;
                animator.SetBool("IsCovering", false);
                animator.SetInteger("CoverState", 0);
            }
        }
        public void HandleCovering()
        {
            if (IsCovering)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredCoverRot, turnSpeed * Time.fixedDeltaTime);
                Controller.Move(transform.forward * 5 * Time.deltaTime);
            }
            if (CanCover() == (false, 0))
            {
                IsCovering = false;
                animator.SetBool("IsCovering", false);
                animator.SetInteger("CoverState", 0);
            }
        }
        #endregion
        
        
        #region CharacterLogic
        bool isGroundTowardsDirection(Vector3 dir)
        {
            bool retVal = false;

            //Vector3 origin = dir;
            //float offset = 0.5f;
            //origin *= offset;
            //origin += Vector3.up / 2;
            //origin += transform.position;

            Vector3 origin = transform.position;
            origin += transform.forward * 3;
            origin += Vector3.up / 2;

            bool forward = DoRayCast(origin, -Vector3.up);

            retVal = forward;

            return retVal;
        }

        bool DoRayCast(Vector3 origin, Vector3 direction)
        {
            bool retVal = false;
            RaycastHit hit;

            if (Physics.Raycast(origin, direction, out hit, 1))
            {
                retVal = true;
            }

            return retVal;
        }
        private void HandleStates()
        {
            IsSprinting = PlayerInput.Sprint;
            bool vertical = PlayerInput.MoveInput.y > 0 || PlayerInput.MoveInput.y < 0;
            bool horizontal = PlayerInput.MoveInput.x > 0 || PlayerInput.MoveInput.x < 0;
            IsCrouching = PlayerInput.Crouch;
            IsSliding = Sliding && (IsCrouching && IsSprinting && PlayerInput.MoveInput.y > 0 || Analog && (vertical || horizontal) && IsSprinting && PlayerInput.Crouch || IsSliding && IsCrouching);
            if (SlideTime != 0.5f) { IsSliding = true; }
            SteepSlope = SlideAngle > 0 && DirectionSlopeUp() && (PlayerInput.MoveInput.y != 0 || PlayerInput.MoveInput.x != 0);
        }
        private void HandleRotation()
        {
            if (!Analog && AutoRotation && !IsCovering)
            {
                float yawCamera = Cam.transform.rotation.eulerAngles.y;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), turnSpeed * Time.fixedDeltaTime);
            }
        }
        private void HandleMovement()
        {
            if (!IsSliding && SlideTime != 0.5f)
            {
                SlideTime -= Time.deltaTime;
                if (SlideTime <= 0f)
                {
                    IsSliding = false;
                    animator.SetBool("Slide", false);
                    PlayerInput.Crouch = false;
                    SlideTime = 0.5f;
                }
            }
            if (IsSliding)
            {
                animator.SetBool("Slide", true);
                if (SlideAngle > 0 && !DirectionSlopeUp()) { SlideTime = 0.5f; }
                SlideTime -= Time.deltaTime;
                float OverrideSlideSpeed = SlideSpeed * SlideTime * 2;
                Controller.Move(transform.forward * (OverrideSlideSpeed * Time.deltaTime));
                if (SlideTime <= 0f)
                {
                    IsSliding = false;
                    animator.SetBool("Slide", false);
                    PlayerInput.Crouch = false;
                    SlideTime = 0.5f;
                }
            }
            float speed = 0f;
            if (!StrafeRun && !Analog)
            {
                speed = IsCovering ? WalkSpeed : IsSliding ? SlideSpeed : IsCrouching ? CrouchSpeed : IsSprinting && (PlayerInput.MoveInput.x == 0 || PlayerInput.MoveInput.y != 0) ? SprintSpeed : WalkSpeed;
            }
            else
            {
                speed = IsCovering ? WalkSpeed : IsSliding ? SlideSpeed : IsCrouching ? CrouchSpeed : IsSprinting ? SprintSpeed : WalkSpeed;
            }
            float horizontal = PlayerInput.MoveInput.x;
            float vertical = PlayerInput.MoveInput.y;
            Vector3 direction = Vector3.zero;
            if (Analog && !IsCovering)
            {
                direction = new Vector3(horizontal, 0f, vertical).normalized;
            }
            else
            {
                direction = transform.right * horizontal + transform.forward * vertical;
            }
            MoveDirection = direction;
            if (direction.magnitude >= 0.1f)
            {
                if (Analog && !IsCovering)
                {
                    float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Cam.eulerAngles.y;
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);
                    Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                    if (Controller.enabled == false) { return; }
                    Controller.Move(moveDir * speed * Time.deltaTime);
                }
                else
                {
                    if (vertical < 0f)
                    {
                        float currentSpeed = IsCrouching ? speed : IsSprinting ? speed - BackwardsSpeedDropOff : speed - BackwardsSpeedDropOff;
                        if (Controller.enabled == false) { return; }
                        Controller.Move(direction * currentSpeed * Time.deltaTime);
                    }
                    else
                    {
                        if (Controller.enabled == false) { return; }
                        Controller.Move(direction * speed * Time.deltaTime);
                    }
                }
                if (!AutoRotation && !Analog && !IsCovering)
                {
                    float yawCamera = Cam.transform.rotation.eulerAngles.y;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), turnSpeed / 5 * Time.fixedDeltaTime);
                }
            }

        }
        private void HandleGravity()
        {
            IsGrounded = Physics.CheckSphere(GroundCheck.position, groundDistance, GroundMask);
            if (IsGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            //Slide Altercations
            if ((SlideAngle > 0) && !IsJumping)
            {
                velocity.y += Gravity * 500 * Time.deltaTime;
            }
            else
            {
                velocity.y += Gravity * Time.deltaTime;
            }
            if (Controller.enabled == false) { return; }
            Controller.Move(velocity * Time.deltaTime);

            if (IsJumping && velocity.y > 0) { IsGrounded = false; }
            //DeclareJumpParam
            if (IsGrounded)
            {
                animator.SetBool("IsFalling", false);
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsFallRoll", RollFromJump);
                animator.SetBool("IsGrounded", true);
                IsJumping = false;
                RollFromJump = false;
            }
            else
            {
                animator.SetBool("IsGrounded", false);
                if ((IsJumping && velocity.y < 0) || velocity.y < -4 && !IsSliding)
                {
                    animator.SetBool("IsFalling", true);
                }
                if (velocity.y < -(LedgeHeightFallRoll) && LandingRoll)
                {
                    RollFromJump = true;
                }
            }
        }
        public void Jump()
        {
            //Note: Called from Player Input Script
            if (IsGrounded)
            {
                animator.SetBool("IsJumping", true);
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * Gravity);
                IsJumping = true;
            }
        }
        #endregion
        
        #region VaultAndClimbLogic
        void VaultLogicInit()
        {
            VaultPhaseInit(targetVaultPosition);
        }
        void VaultPhaseInit(Vector3 targetPos)
        {
            if (IsWalkUp)
            {
                if (!IsSprinting)
                {
                    animator.CrossFade("walk_up", 0.1f);
                }
                else
                    animator.CrossFade("run_up", 0.05f);
            }
            else
            {
                if (!IsSprinting)
                {
                    int mirrorinput = Random.Range(1, 3);
                    animator.CrossFade("vault_over_walk_" + mirrorinput.ToString(), 0.1f);
                }
                else
                {
                    int mirrorinput = Random.Range(1, 3);
                    animator.CrossFade("vault_over_run_" + mirrorinput.ToString(), 0.05f);
                }
            }
            int mirror = Random.Range(0, 2);
            animator.SetBool("MirrorJump", mirror > 0);
            forceOverrideTimer = 0;
            ForceOverLife = Vector3.Distance(transform.position, targetPos);
            fc_t = 0;
            Controller.enabled = false;
            startPosition = transform.position;
            overrideDirection = targetPos - startPosition;
            targetVaultPosition = targetPos;

            bool Run = PlayerInput.MoveInput.y != 0 && IsSprinting;
            overrideSpeed = Run ? 4f : IsClimbUp ? 1.5f : 2f;
        }
        void HandleVaulting()
        {
            fc_t += Time.deltaTime;
            float targetSpeed = overrideSpeed * (IsClimbUp ? VaultCurve.Evaluate(fc_t) : WalkUpCurve.Evaluate(fc_t));

            forceOverrideTimer += Time.deltaTime * targetSpeed / ForceOverLife;

            if (forceOverrideTimer > 1)
            {
                forceOverrideTimer = 1;
                Controller.enabled = true;
                StopVaulting();
            }

            Vector3 targetPosition = Vector3.Lerp(startPosition, targetVaultPosition, forceOverrideTimer);
            transform.position = targetPosition;

            Quaternion targetRot = Quaternion.LookRotation(overrideDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5);
        }
        void StopVaulting()
        {
            initVault = false;
            IsVaulting = false;
            IsClimbUp = false;
            StartCoroutine("OpenCanVaultIfApplicable");
        }
        IEnumerator OpenCanVaultIfApplicable()
        {
            yield return new WaitForSeconds(0.4f);
            Vaulting = IsVaulting;
            yield return new WaitForSeconds(0.1f);
            Vaulting = true;
        }
        void IsClear(Vector3 origin, Vector3 direction, float distance, ref bool isHit)
        {
            RaycastHit hit = new RaycastHit();
            float targetDistance = distance;
            if (IsSprinting)
                targetDistance += 0.5f;
            int numberOfHits = 0;
            for (int i = -1; i < 2; i++)
            {
                Vector3 targetOrigin = origin;
                targetOrigin += transform.right * (i * 0.3f);
                Debug.DrawRay(targetOrigin, direction * targetDistance, Color.green);
                if (Physics.Raycast(targetOrigin, direction, out hit, targetDistance, GroundMask))
                {
                    numberOfHits++;
                }
            }
            if (numberOfHits > 2)
            {
                isHit = true;
            }
            else
            {
                isHit = false;
            }
            if (obstacleForward)
            {
                Vector3 incomingVec = hit.point - origin;
                Vector3 relectVect = Vector3.Reflect(incomingVec, hit.normal);
                float angle = Vector3.Angle(incomingVec, relectVect);
                if (angle < 70)
                {
                    obstacleForward = false;
                }
                else
                {
                    if (numberOfHits > 2)
                    {
                        bool willVault = false;
                        CanVaultOver(hit, ref willVault);
                        if (willVault)
                        {
                            obstacleForward = false;
                            return;
                        }
                        else
                        {
                            bool willClimb = false;
                            ClimbOver(hit, ref willClimb);
                            if (!willClimb)
                            {
                                obstacleForward = true;
                                return;
                            }
                        }
                    }
                }
            }
        }
        void CanVaultOver(RaycastHit hit, ref bool willVault)
        {
            Vector3 wallDirection = -hit.normal * 0.5f;
            RaycastHit vHit;

            Vector3 wallOrigin = transform.position + Vector3.up * vaultOverHeight;
            Debug.DrawRay(wallOrigin, wallDirection * vaultCheckDistance, Color.red);

            if (Physics.Raycast(wallOrigin, wallDirection, out vHit, vaultCheckDistance, GroundMask))
            {
                willVault = false;
                return;
            }
            else
            {
                if (Vaulting && !IsVaulting)
                {
                    Vector3 startOrigin = hit.point;
                    startOrigin.y = transform.position.y;
                    Vector3 vOrigin = startOrigin + Vector3.up * vaultOverHeight;
                    vOrigin += wallDirection * vaultCheckDistance;
                    Debug.DrawRay(vOrigin, -Vector3.up * vaultCheckDistance);

                    if (Physics.Raycast(vOrigin, -Vector3.up, out vHit, vaultCheckDistance, GroundMask))
                    {
                        float hitY = vHit.point.y;
                        float diff = hitY - transform.position.y;
                        if (Mathf.Abs(diff) < vaultFloorHeighDifference)
                        {
                            IsVaulting = true;
                            IsWalkUp = false;
                            targetVaultPosition = vHit.point;
                            willVault = true;
                            return;
                        }
                    }
                }
            }
        }
        void ClimbOver(RaycastHit hit, ref bool willClimb)
        {
            if (!AutoStep) { return; }
            float targetDistance = distanceToCheckForward + 0.1f;
            if (IsSprinting)
                targetDistance += 2.25f;

            Vector3 climbCheckOrigin = transform.position;
            climbCheckOrigin = transform.position + (Vector3.up * walkUpHeight);

            RaycastHit climbHit;
            Vector3 wallDirection = -hit.normal * targetDistance;
            Debug.DrawRay(climbCheckOrigin, wallDirection, Color.yellow);
            if (Physics.Raycast(climbCheckOrigin, wallDirection, out climbHit, 1.2f, GroundMask))
            {

            }
            else
            {
                Vector3 origin2 = hit.point;
                origin2.y = transform.position.y;
                origin2 += Vector3.up * walkUpHeight;
                origin2 += wallDirection * 0.2f;
                Debug.DrawRay(origin2, -Vector3.up, Color.yellow);
                if (Physics.Raycast(origin2, -Vector3.up, out climbHit, 1, GroundMask))
                {
                    float diff = climbHit.point.y - transform.position.y;
                    if (Mathf.Abs(diff) > WalkUpThreshold)
                    {
                        IsVaulting = true;
                        targetVaultPosition = climbHit.point;
                        obstacleForward = false;
                        willClimb = true;
                        IsWalkUp = true;
                        return;
                    }

                }
            }
        }
        #endregion
    }
}*/