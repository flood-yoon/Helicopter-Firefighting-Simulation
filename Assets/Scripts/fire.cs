using UnityEngine;

public class Fire : MonoBehaviour
{
    public float firePower = 100f;

    private GameManager gameManager;
    private FireHealthUI fireHealthUI;
    private bool isDead = false;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        fireHealthUI = FindAnyObjectByType<FireHealthUI>();
    }

    public void Extinguish(float amount)
    {
        if (isDead) return;

        firePower -= amount;

        // 물에 맞는 순간 UI에 등록
        if (fireHealthUI != null)
            fireHealthUI.RegisterHit(this);

        if (firePower <= 0)
        {
            isDead = true;

            // 꺼지는 순간 즉시 UI에서 제거
            if (fireHealthUI != null)
                fireHealthUI.UnregisterFire(this);

            gameManager.FireExtinguished();
            Destroy(gameObject);
        }
    }
}