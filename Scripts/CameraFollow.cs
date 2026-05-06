using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("추적 대상")]
    public Transform target;             // 헬리콥터 Transform

    [Header("카메라 위치 설정")]
    public Vector3 offset = new Vector3(0f, 5f, -12f);  // 헬리콥터 기준 카메라 오프셋
    public float followSpeed = 8f;       // 카메라 이동 부드러움
    public float rotationSpeed = 6f;     // 카메라 회전 부드러움

    [Header("카메라 각도")]
    public float pitchAngle = 10f;       // 위에서 내려다보는 각도

    [Header("충돌 방지")]
    public bool avoidObstacles = true;
    public LayerMask obstacleLayer;
    public float minDistance = 2f;

    private Vector3 currentVelocity;

    void LateUpdate()
    {
        if (target == null) return;

        FollowTarget();
    }

    void FollowTarget()
    {
        // 헬리콥터의 Y축 회전만 따라가도록 (기울기는 무시)
        Quaternion targetRotation = Quaternion.Euler(0f, target.eulerAngles.y, 0f);

        // 목표 카메라 위치 계산
        Vector3 desiredPosition = target.position + targetRotation * offset;

        // 장애물 감지 (선택적)
        if (avoidObstacles)
        {
            Vector3 dirToCamera = desiredPosition - target.position;
            float distance = dirToCamera.magnitude;

            if (Physics.Raycast(target.position, dirToCamera.normalized, out RaycastHit hit, distance, obstacleLayer))
            {
                desiredPosition = hit.point - dirToCamera.normalized * 0.5f;
            }
        }

        // 부드럽게 카메라 이동 (SmoothDamp 사용)
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref currentVelocity,
            1f / followSpeed
        );

        // 카메라가 헬리콥터를 바라보도록 회전
        Quaternion lookRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation,
            Time.deltaTime * rotationSpeed
        );
    }
}
