using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [Header("추적 대상")]
    public Transform target;

    [Header("카메라 위치 설정")]
    public Vector3 offset = new Vector3(0f, 5f, -12f);
    public float followSpeed = 8f;
    public float rotationSpeed = 6f;

    [Header("마우스 궤도 설정")]
    [Range(0.01f, 1f)]
    public float mouseSensitivity = 0.08f;     // 감도 (낮을수록 둔감)
    public float orbitReturnDelay = 3f;
    public float orbitReturnSpeed = 1.5f;

    [Header("수직 각도 제한")]
    public float minPitch = -15f;              // 아래 한계 (헬리콥터 아래로 보기)
    public float maxPitch = 40f;               // 위 한계 (헬리콥터 위에서 내려다보기)

    [Header("충돌 방지")]
    public bool avoidObstacles = true;
    public LayerMask obstacleLayer;
    public float minDistance = 2f;

    private float currentYaw = 0f;
    private float currentPitch = 0f;
    private float lastMouseInputTime = -999f;
    private Vector3 currentVelocity;

    // 마우스 delta 누적용 (픽셀 스파이크 완화)
    private Vector2 smoothedDelta;

    void LateUpdate()
    {
        if (target == null) return;
        HandleMouseOrbit();
        FollowTarget();
    }

    void HandleMouseOrbit()
    {
        if (Mouse.current == null) return;

        Vector2 rawDelta = Mouse.current.delta.ReadValue();

        // 급격한 스파이크 완화 (프레임 간 부드럽게)
        smoothedDelta = Vector2.Lerp(smoothedDelta, rawDelta, 0.35f);

        bool hasInput = smoothedDelta.sqrMagnitude > 0.001f;

        if (hasInput)
        {
            currentYaw += smoothedDelta.x * mouseSensitivity;
            currentPitch += smoothedDelta.y * mouseSensitivity;  // Y는 그대로 (아래서 부호 처리)

            currentYaw = Mathf.Repeat(currentYaw + 180f, 360f) - 180f;
            currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);

            lastMouseInputTime = Time.time;
        }
        else
        {
            if (Time.time - lastMouseInputTime >= orbitReturnDelay)
            {
                currentYaw = Mathf.LerpAngle(currentYaw, 0f, Time.deltaTime * orbitReturnSpeed);
                currentPitch = Mathf.LerpAngle(currentPitch, 0f, Time.deltaTime * orbitReturnSpeed);
            }
        }
    }

    void FollowTarget()
    {
        // 헬리콥터 진행 방향 기준 yaw
        Quaternion baseYaw = Quaternion.Euler(0f, target.eulerAngles.y, 0f);
        Quaternion yawOffset = Quaternion.Euler(0f, currentYaw, 0f);

        // offset을 yaw만 적용한 방향으로 먼저 회전
        Vector3 yawedOffset = baseYaw * yawOffset * offset;

        // pitch는 카메라-타겟 수평 축을 기준으로 회전 (뒤집힘 없음)
        Vector3 flatDir = yawedOffset.normalized;
        Vector3 pitchAxis = Vector3.Cross(flatDir, Vector3.up).normalized;
        // pitch 부호: 양수면 카메라가 위로 올라가며 아래를 내려다봄
        Quaternion pitchRot = Quaternion.AngleAxis(-currentPitch, pitchAxis);

        Vector3 desiredPosition = target.position + pitchRot * yawedOffset;

        // 장애물 감지
        if (avoidObstacles)
        {
            Vector3 dirToCamera = desiredPosition - target.position;
            float distance = dirToCamera.magnitude;
            if (Physics.Raycast(target.position, dirToCamera.normalized,
                                out RaycastHit hit, distance, obstacleLayer))
            {
                desiredPosition = hit.point - dirToCamera.normalized * 0.5f;
            }
        }

        // 부드럽게 이동
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref currentVelocity,
            1f / followSpeed
        );

        // 항상 헬리콥터를 바라봄
        Vector3 lookDir = target.position - transform.position;
        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                Time.deltaTime * rotationSpeed
            );
        }
    }
}