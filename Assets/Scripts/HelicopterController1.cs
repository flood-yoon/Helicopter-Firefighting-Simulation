using UnityEngine;
using UnityEngine.InputSystem;

public class HelicopterController1 : MonoBehaviour
{
    [Header("상승/하강 설정 (W/S)")]
    public float liftForce = 15f;
    public float descentForce = 8f;
    public float hoverForce = 9.8f;

    [Header("기수 회전 설정 (A/D)")]
    public float yawSpeed = 80f;

    [Header("기울기 설정")]
    public float tiltSpeed = 40f;
    public float maxTiltAngle = 30f;
    public float tiltMoveForce = 12f;

    [Header("속도 제한")]
    public float normalMaxSpeed = 20f;       // 노말 모드 기본 최대속도
    public float maxNormalSpeedCap = 30f;    // 5라운드에 도달하는 노말 최대속도
    public float modeSpeedMultiplier = 1f;   // GameManager에서 모드별로 세팅

    [Header("로터 설정")]
    public Transform mainRotor;
    public Transform tailRotor;
    public float mainRotorIdleSpeed = 300f;
    public float mainRotorMaxSpeed = 700f;
    public float tailRotorSpeed = 800f;

    [Header("물리 설정")]
    public float gravityScale = 2f;
    public float linearDamping = 1.5f;

    // 모드: 1=화재진압, 2=노말, 3=스피드
    [HideInInspector] public int currentMode = 2;
    [HideInInspector] public int currentRound = 1;
    [HideInInspector] public bool modeUnlocked = false; // 3라운드부터 true
    [HideInInspector] public float roundSpeedBonus = 0f; // 라운드마다 누적 속도 보너스

    private Rigidbody rb;
    private float currentTiltX = 0f;
    private float currentTiltZ = 0f;
    private float currentRotorSpeed = 300f;
    private float currentYaw = 0f;

    // UI 참조
    private GameManager gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearDamping = linearDamping;
            rb.angularDamping = 10f;
            rb.constraints = RigidbodyConstraints.FreezeRotationX
                           | RigidbodyConstraints.FreezeRotationZ;
        }
        currentYaw = transform.eulerAngles.y;
        gameManager = FindAnyObjectByType<GameManager>();
    }

    void Update()
    {
        RotateRotors();
        if (modeUnlocked) HandleModeInput();
    }

    void FixedUpdate()
    {
        if (rb == null) return;
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        HandleLift(keyboard);
        HandleYaw(keyboard);
        HandleTilt(keyboard);
        HandleGravity();
        ApplyTiltMovement();
        ApplyRotation();
        ClampSpeed();
    }

    void HandleModeInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.digit1Key.wasPressedThisFrame) gameManager?.SetMode(1);
        else if (keyboard.digit2Key.wasPressedThisFrame) gameManager?.SetMode(2);
        else if (keyboard.digit3Key.wasPressedThisFrame) gameManager?.SetMode(3);
    }

    void HandleLift(Keyboard keyboard)
    {
        //라운드 마다 모터 힘 증가 (최대 2배)
        float motorBonus = Mathf.Min(1f + (currentRound - 1) * 0.15f, 2f);

        if (keyboard.wKey.isPressed)
        {
            rb.AddForce(Vector3.up * liftForce * motorBonus, ForceMode.Acceleration);
            currentRotorSpeed = Mathf.Lerp(currentRotorSpeed, mainRotorMaxSpeed, Time.fixedDeltaTime * 3f);
        }
        else if (keyboard.sKey.isPressed)
        {
            // 호버링 힘 없이 하강력만 + motorBonus 추가 적용
            rb.AddForce(Vector3.down * descentForce * motorBonus * 2f, ForceMode.Acceleration);
            currentRotorSpeed = Mathf.Lerp(currentRotorSpeed, mainRotorIdleSpeed * 0.5f, Time.fixedDeltaTime * 3f);
        }
        else
        {
            rb.AddForce(Vector3.up * hoverForce, ForceMode.Acceleration);
            currentRotorSpeed = Mathf.Lerp(currentRotorSpeed, mainRotorIdleSpeed, Time.fixedDeltaTime * 2f);
        }
    }

    void HandleYaw(Keyboard keyboard)
    {
        // 현재 속도 비율에 따라 yaw 속도 증가
        float speedRatio = rb.linearVelocity.magnitude / Mathf.Max(GetCurrentNormalMaxSpeed(), 1f);
        float dynamicYawSpeed = yawSpeed * Mathf.Lerp(1f, 2.5f, speedRatio);

        if (keyboard.aKey.isPressed)
            currentYaw -= dynamicYawSpeed * Time.fixedDeltaTime;
        else if (keyboard.dKey.isPressed)
            currentYaw += dynamicYawSpeed * Time.fixedDeltaTime;
    }

    void HandleTilt(Keyboard keyboard)
    {
        if (keyboard.numpad4Key.isPressed)
            currentTiltZ = Mathf.Clamp(currentTiltZ + tiltSpeed * Time.fixedDeltaTime, -maxTiltAngle, maxTiltAngle);
        else if (keyboard.numpad6Key.isPressed)
            currentTiltZ = Mathf.Clamp(currentTiltZ - tiltSpeed * Time.fixedDeltaTime, -maxTiltAngle, maxTiltAngle);
        else
            currentTiltZ = Mathf.Lerp(currentTiltZ, 0f, Time.fixedDeltaTime * 3f);

        if (keyboard.numpad8Key.isPressed)
            currentTiltX = Mathf.Clamp(currentTiltX + tiltSpeed * Time.fixedDeltaTime, -maxTiltAngle, maxTiltAngle);
        else if (keyboard.numpad5Key.isPressed)
            currentTiltX = Mathf.Clamp(currentTiltX - tiltSpeed * Time.fixedDeltaTime, -maxTiltAngle, maxTiltAngle);
        else
            currentTiltX = Mathf.Lerp(currentTiltX, 0f, Time.fixedDeltaTime * 3f);
    }

    void ApplyTiltMovement()
    {
        Vector3 forward = Quaternion.Euler(0f, currentYaw, 0f) * Vector3.forward;
        Vector3 right = Quaternion.Euler(0f, currentYaw, 0f) * Vector3.right;

        float pitchRatio = currentTiltX / maxTiltAngle;
        float rollRatio = -currentTiltZ / maxTiltAngle;

        Vector3 tiltForce = (forward * pitchRatio + right * rollRatio) * tiltMoveForce * modeSpeedMultiplier;
        rb.AddForce(tiltForce, ForceMode.Acceleration);
    }

    void ClampSpeed()
    {
        // 모드별 최대속도 계산
        float maxSpeed;
        if (currentMode == 3)
        {
            // 스피드 모드: 제한 없음 (roundSpeedBonus로 계속 증가)
            maxSpeed = maxNormalSpeedCap + roundSpeedBonus;
        }
        else if (currentMode == 1)
        {
            // 화재진압 모드: 노말의 60%
            maxSpeed = GetCurrentNormalMaxSpeed() * 0.6f;
        }
        else
        {
            // 노말 모드
            maxSpeed = GetCurrentNormalMaxSpeed();
        }

        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

    public float GetCurrentNormalMaxSpeed()
    {
        // 5라운드에 maxNormalSpeedCap 도달
        float t = Mathf.Clamp01((currentRound - 1) / 4f);
        return Mathf.Lerp(normalMaxSpeed, maxNormalSpeedCap, t);
    }

    public bool IsNormalSpeedMaxed()
    {
        return currentRound >= 5;
    }

    void ApplyRotation()
    {
        transform.rotation = Quaternion.Euler(currentTiltX, currentYaw, currentTiltZ);
    }

    void HandleGravity()
    {
        if (!isGrounded)
            rb.AddForce(Vector3.down * gravityScale * 9.8f, ForceMode.Acceleration);
    }

    void RotateRotors()
    {
        if (mainRotor != null)
            mainRotor.Rotate(Vector3.up, currentRotorSpeed * Time.deltaTime);
        if (tailRotor != null)
            tailRotor.Rotate(Vector3.right, tailRotorSpeed * Time.deltaTime);
    }

    private bool isGrounded = false;

    void OnCollisionEnter(Collision collision) { isGrounded = true; }
    void OnCollisionExit(Collision collision) { isGrounded = false; }
    void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
        if (rb.linearVelocity.y < 0)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
    }
}