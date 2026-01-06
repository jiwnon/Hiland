using UnityEngine;

public class Damageable : MonoBehaviour
{
    public float maxHp = 30f;
    public float hp = 30f;

    public System.Action<float> onDamaged;
    public System.Action onDead;

    void Reset()
    {
        hp = maxHp;
    }

    public void TakeDamage(float amount)
    {
        hp -= amount;
        onDamaged?.Invoke(amount);

        if (hp <= 0f)
        {
            hp = 0f;
            onDead?.Invoke();
            Destroy(gameObject);
        }
    }
}
