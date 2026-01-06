using System.Collections;
using UnityEngine;

public class StoneAttack : MonoBehaviour
{
    [Header("Attack")]
    public float attackDamage = 10f;
    public float attackCooldown = 0.25f;

    // "앞쪽 원" 판정 파라미터
    public float attackRadius = 0.9f;      // 원 반지름
    public float forwardOffset = 1.0f;     // 플레이어 앞쪽으로 얼마나 떨어진 지점에 원 중심을 둘지
    public LayerMask hitMask = ~0;         // 맞출 레이어(기본: 전부)

    float lastAttackTime = -999f;

    [Header("Indicator (optional)")]
    public bool debugShowIndicator = false;
    public KeyCode toggleKey = KeyCode.F1;
    public float indicatorShowTime = 0.12f;
    public Transform indicatorRoot;      // AttackIndicator
    public Transform indicatorQuad;      // IndicatorQuad

    Coroutine showRoutine;

    void Start()
    {
        ApplyIndicatorTransform();
        SetIndicatorVisible(debugShowIndicator);
    }

    void Update()
    {
        // 디버그 토글
        if (Input.GetKeyDown(toggleKey))
        {
            debugShowIndicator = !debugShowIndicator;
            SetIndicatorVisible(debugShowIndicator);
        }

        // 좌클릭 공격
        if (Input.GetMouseButtonDown(0))
        {
            TryAttack();
        }
    }

    void TryAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;
        lastAttackTime = Time.time;

        if (!debugShowIndicator) ShowIndicatorBriefly();

        // "앞쪽 원"의 중심점: 플레이어 위치 + forward * forwardOffset
        Vector3 center = transform.position + transform.forward * forwardOffset;

        // 범위 안 콜라이더들 찾기
        Collider[] hits = Physics.OverlapSphere(center, attackRadius, hitMask, QueryTriggerInteraction.Ignore);

        // Damageable 중에서 '가장 가까운 1개' 선택
        Damageable best = null;
        float bestDist = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            var d = hits[i].GetComponentInParent<Damageable>();
            if (d == null) continue;

            // "플레이어 앞쪽"만 더 엄격히: 뒤쪽/옆쪽은 제외(원 중심은 앞이지만, 추가 필터로 더 확실히)
            Vector3 to = (d.transform.position - transform.position);
            to.y = 0f;
            if (to.sqrMagnitude < 0.0001f) continue;
            float dot = Vector3.Dot(transform.forward, to.normalized);
            if (dot < 0.25f) continue; // 0.25면 대략 75도 안쪽만 허용(앞쪽 위주)

            float dist = (d.transform.position - transform.position).sqrMagnitude;
            if (dist < bestDist)
            {
                bestDist = dist;
                best = d;
            }
        }

        if (best != null)
        {
            best.TakeDamage(attackDamage);
            // TODO: 여기서 타격 이펙트/사운드 붙이면 끝내줌
        }
    }

    void ApplyIndicatorTransform()
    {
        if (indicatorRoot != null)
        {
            indicatorRoot.localPosition = new Vector3(0f, 0.01f, forwardOffset);
            indicatorRoot.localRotation = Quaternion.Euler(90f, 0f, 0f);
        }

        if (indicatorQuad != null)
        {
            float d = attackRadius * 2f;
            indicatorQuad.localScale = new Vector3(d, d, 1f);
        }
    }

    void ShowIndicatorBriefly()
    {
        if (showRoutine != null) StopCoroutine(showRoutine);
        showRoutine = StartCoroutine(CoShow());
    }

    IEnumerator CoShow()
    {
        SetIndicatorVisible(true);
        yield return new WaitForSeconds(indicatorShowTime);
        SetIndicatorVisible(false);
    }

    void SetIndicatorVisible(bool v)
    {
        if (indicatorRoot != null)
            indicatorRoot.gameObject.SetActive(v);
    }

    // (선택) Scene 뷰에서 공격 범위를 보이게
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position + transform.forward * forwardOffset;
        Gizmos.DrawWireSphere(center, attackRadius);
    }
}
