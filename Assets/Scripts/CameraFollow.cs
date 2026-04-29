using UnityEngine;
using UnityEngine.InputSystem; // 새 입력 방식 사용하기 위한 라이브러리

public class CameraFollow : MonoBehaviour
{
    // ===== 인스펙터에서 조절할 수 있는 값들 =====
    public Transform target;            // 따라갈 대상 (플레이어)
    public float maxDistance = 5f;      // 평소 플레이어와의 거리
    public float minDistance = 1f;      // 벽에 막혔을 때 최소 거리
    public float height = 2f;           // 카메라 높이
    public float smoothSpeed = 10f;     // 카메라 이동 & 회전 부드러움 (높을수록 빠르게 따라감)
    public float mouseSensitivity = 2f; // 마우스 감도 (높을수록 빠르게 회전)

    [Header("카메라 상하 각도 제한")] // 인스펙터에서 구분선 + 제목 표시
    public float maxLookUp = 30f;   // 위로 볼 수 있는 최대 각도
    public float maxLookDown = 60f; // 아래로 볼 수 있는 최대 각도

    private float rotX = 10f;        // 목표 상하 각도 (마우스 입력으로 변하는 값)
    private float currentRotX = 10f; // 실제 카메라에 적용되는 각도 (부드럽게 보간된 값)

    void Start()
    {
        // 게임 시작시 마우스 커서 숨기고 화면 중앙에 고정
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate() // 플레이어 이동이 끝난 후에 카메라 이동하려고 LateUpdate 사용
    {
        // 플레이어가 없으면 아무것도 하지 않고 종료
        if (target == null) return;

        // 이번 프레임에 마우스가 얼마나 움직였는지 가져오기
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // 마우스 상하 움직임으로 목표 각도 계산
        // 마우스 위로 움직이면 카메라가 위를 바라보게 - 붙임
        rotX -= mouseDelta.y * mouseSensitivity * Time.deltaTime * 10f;

        // 각도 제한 (인스펙터에서 설정한 값으로 제한)
        // 없으면 카메라가 360도 뱅글뱅글 돌아버림!
        rotX = Mathf.Clamp(rotX, -maxLookUp, maxLookDown);

        // 목표 각도로 부드럽게 보간! (뚝뚝 끊기는 문제 해결!)
        // Lerp = 현재값에서 목표값으로 smoothSpeed 속도로 부드럽게 이동
        currentRotX = Mathf.Lerp(currentRotX, rotX, smoothSpeed * Time.deltaTime);

        // ===== 카메라 위치 계산 (항상 플레이어 뒤에 고정) =====
        // 플레이어 위치에서 플레이어가 바라보는 반대방향으로 maxDistance만큼 이동
        Vector3 desiredPosition = target.position
                                - target.forward * maxDistance
                                + Vector3.up * height; // 높이 추가

        // ===== 벽 감지 (Raycast) =====
        float distance = maxDistance; // 일단 최대 거리로 시작

        // 플레이어에서 카메라 방향으로 광선을 쏴서 벽 감지
        Vector3 direction = (desiredPosition - target.position).normalized;
        if (Physics.Raycast(
            target.position + Vector3.up * height, // 광선 시작점 (플레이어 머리)
            direction,                              // 광선 방향 (카메라 쪽으로)
            out RaycastHit hit,                    // 맞은 물체 정보 저장
            maxDistance))                          // 최대 감지 거리
        {
            // 벽에 맞으면 거리 줄이기 (-0.2f = 벽에 살짝 안닿게 여유)
            distance = Mathf.Clamp(hit.distance - 0.2f, minDistance, maxDistance);
        }

        // 최종 카메라 위치 (벽 감지 반영된 거리로 계산)
        Vector3 finalPosition = target.position - target.forward * distance + Vector3.up * height;

        // 카메라를 최종위치로 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, finalPosition, smoothSpeed * Time.deltaTime);

        // 부드럽게 보간된 각도를 카메라 회전에 적용!
        // currentRotX = 상하각도, target.eulerAngles.y = 플레이어가 바라보는 방향
        transform.rotation = Quaternion.Euler(currentRotX, target.eulerAngles.y, 0);

        // ESC 누르면 마우스 커서 다시 보이게
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            Cursor.lockState = CursorLockMode.None;
    }
}