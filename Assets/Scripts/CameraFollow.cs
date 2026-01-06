using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = new Vector3(0f, 10f, -6f);
    [SerializeField] float smoothTime = 0.12f;

    Vector3 velocity;

    void LateUpdate()
    {
        if (target == null) return;

        // 목표 위치(플레이어 + 오프셋)
        Vector3 desiredPos = target.position + offset;

        // 부드럽게 이동
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothTime);
    }

    // 인스펙터에서 target을 안 넣었을 때 자동으로 Player 찾기(편의)
    void Reset()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) target = p.transform;
    }
}
