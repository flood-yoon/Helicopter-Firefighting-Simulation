using UnityEngine;
using UnityEngine.InputSystem;

public class HelicopterController1 : MonoBehaviour
{
    [Header("상승/하강 설정 (W/S)")]
    public float liftForce = 15f;         // 상승 힘
    public float descentForce = 8f;       // 하강 힘 (중력 보조)
    public float hoverForce = 9.8f;       // 호버링 중력 상쇄력

    [Header("기수 회전 설정 (A/D - 꼬리날개)")]
    public float yawSpeed = 80f;          // 좌우 기수 회전 속도

    [Header("기울기 설정 (키패드 4/6/8/5)")]
    public float tiltSpeed = 40f;         // 기울기 조작 속도
    public float maxTiltAngle = 30f;      // 최대 기울기 각도
    public float tiltMoveForce = 12f;     // 기울기에 의한 이동 힘

    [Header("로터 설정")]
    public Transform mainRotor;
    public Transform tailRotor;
    public float mainRotorIdleSpeed = 300f;   // 기본 로터 속도
    public float mainRotorMaxSpeed = 700f;    // 최대 로터 속도 (W 누를 때)
    public float tailRotorSpeed = 800f;

    [Header("물리 설정")]
    public float gravityScale = 2f;
    public float linearDamping = 1.5f;

    private Rigidbody rb;
    private float currentTiltX = 0f;     // 앞뒤 기울기 (피치)
    private float currentTiltZ = 0f;     // 좌우 기울기 (롤)
    private float currentRotorSpeed = 300f;
    private float currentYaw = 0f;       // 현재 Y축 회전값

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
    }

    void Update()
    {
        RotateRotors();
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
    }

    // ── W/S : 상승 / 하강 ──────────────────────────────
    void HandleLift(Keyboard keyboard)
    {
        if (keyboard.wKey.isPressed)
        {
            // W: 로터 출력 증가 → 상승
            rb.AddForce(Vector3.up * liftForce, ForceMode.Acceleration);
            currentRotorSpeed = Mathf.Lerp(currentRotorSpeed, mainRotorMaxSpeed, Time.fixedDeltaTime * 3f);
        }
        else if (keyboard.sKey.isPressed)
        {
            // S: 로터 출력 감소 → 하강 (중력 + 추가 하강력)
            rb.AddForce(Vector3.down * descentForce, ForceMode.Acceleration);
            currentRotorSpeed = Mathf.Lerp(currentRotorSpeed, mainRotorIdleSpeed * 0.5f, Time.fixedDeltaTime * 3f);
        }
        else
        {
            // 아무것도 안 누르면 호버링 (중력 상쇄)
            rb.AddForce(Vector3.up * hoverForce, ForceMode.Acceleration);
            currentRotorSpeed = Mathf.Lerp(currentRotorSpeed, mainRotorIdleSpeed, Time.fixedDeltaTime * 2f);
        }
    }

    // ── A/D : 기수 좌우 회전 (꼬리날개) ───────────────
    void HandleYaw(Keyboard keyboard)
    {
        if (keyboard.aKey.isPressed)
            currentYaw -= yawSpeed * Time.fixedDeltaTime;
        else if (keyboard.dKey.isPressed)
            currentYaw += yawSpeed * Time.fixedDeltaTime;
    }

    // ── 키패드 4/6/8/5 : 본체 기울이기 ────────────────
    void HandleTilt(Keyboard keyboard)
    {
        // 키패드 4/6 → 좌/우 롤 (Z축 기울기)
        if (keyboard.numpad4Key.isPressed)
            currentTiltZ = Mathf.Clamp(currentTiltZ + tiltSpeed * Time.fixedDeltaTime, -maxTiltAngle, maxTiltAngle);
        else if (keyboard.numpad6Key.isPressed)
            currentTiltZ = Mathf.Clamp(currentTiltZ - tiltSpeed * Time.fixedDeltaTime, -maxTiltAngle, maxTiltAngle);
        else
            currentTiltZ = Mathf.Lerp(currentTiltZ, 0f, Time.fixedDeltaTime * 3f); // 자동 복귀

        // 키패드 8/5 → 앞/뒤 피치 (X축 기울기)
        if (keyboard.numpad8Key.isPressed)
            currentTiltX = Mathf.Clamp(currentTiltX + tiltSpeed * Time.fixedDeltaTime, -maxTiltAngle, maxTiltAngle);
        else if (keyboard.numpad5Key.isPressed)
            currentTiltX = Mathf.Clamp(currentTiltX - tiltSpeed * Time.fixedDeltaTime, -maxTiltAngle, maxTiltAngle);
        else
            currentTiltX = Mathf.Lerp(currentTiltX, 0f, Time.fixedDeltaTime * 3f); // 자동 복귀
    }

    // ── 기울기에 따라 실제 이동력 적용 ────────────────
    void ApplyTiltMovement()
    {
        // 기울어진 방향으로 이동 (앞뒤/좌우)
        Vector3 forward = Quaternion.Euler(0f, currentYaw, 0f) * Vector3.forward;
        Vector3 right   = Quaternion.Euler(0f, currentYaw, 0f) * Vector3.right;

        float pitchRatio = currentTiltX / maxTiltAngle;  // -1 ~ 1
        float rollRatio  = -currentTiltZ / maxTiltAngle;  // -1 ~ 1

        Vector3 tiltForce = (forward * pitchRatio + right * rollRatio) * tiltMoveForce;
        rb.AddForce(tiltForce, ForceMode.Acceleration);
    }

    // ── 회전 적용 ──────────────────────────────────────
    void ApplyRotation()
    {
        transform.rotation = Quaternion.Euler(currentTiltX, currentYaw, currentTiltZ);
    }

    // ── 중력 ───────────────────────────────────────────
    void HandleGravity()
    {
        if (!isGrounded)
            rb.AddForce(Vector3.down * gravityScale * 9.8f, ForceMode.Acceleration);
    }

    // ── 로터 회전 애니메이션 ───────────────────────────
    void RotateRotors()
    {
        if (mainRotor != null)
            mainRotor.Rotate(Vector3.up, currentRotorSpeed * Time.deltaTime);

        if (tailRotor != null)
            tailRotor.Rotate(Vector3.right, tailRotorSpeed * Time.deltaTime);
    }

    private bool isGrounded = false;

    void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    void OnCollisionStay(Collision collision)
    {
        isGrounded = true;

        // 아래로 향하는 속도만 차단
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                0f,
                rb.linearVelocity.z
            );
        }
    }
}
