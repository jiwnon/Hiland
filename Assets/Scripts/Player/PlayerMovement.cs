using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 720f; // 초당 회전 각도
    Rigidbody rb;

    Vector3 lastMoveDir = Vector3.forward;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0, v);

        // 이동
        Vector3 moveDir = dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector3.zero;
        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);

        // 바라보는 방향 갱신(멈췄을 땐 마지막 방향 유지)
        if (moveDir != Vector3.zero)
        {
            lastMoveDir = moveDir;

            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            Quaternion newRot = Quaternion.RotateTowards(rb.rotation, targetRot, turnSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(newRot);
        }
    }

    public Vector3 ForwardDir => lastMoveDir; // (나중에 공격 판정에도 쓸 수 있음)
}
