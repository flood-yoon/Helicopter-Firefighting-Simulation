using UnityEngine;

public class Fire : MonoBehaviour
{
    public float firePower = 100f;
    [HideInInspector] public float maxFirePower = 100f;

    // 층수: 1=지형, 2=불위의불, 3=2층위의불
    [HideInInspector] public int floorLevel = 1;
    // 이 불이 올라탄 부모 불 (2층, 3층일 때)
    [HideInInspector] public Fire parentFire = null;
    // 이 불 위에 올라탄 자식 불
    [HideInInspector] public Fire childFire = null;

    private GameManager gameManager;
    private FireHealthUI fireHealthUI;
    private bool isDead = false;
    private float lastHitTime = -999f;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        fireHealthUI = FindAnyObjectByType<FireHealthUI>();
        lastHitTime = -999f;
    }

    public void Extinguish(float amount)
    {
        if (isDead) return;

        firePower -= amount;
        lastHitTime = Time.time;

        if (fireHealthUI != null)
            fireHealthUI.RegisterHit(this);

        if (firePower <= 0)
        {
            isDead = true;

            // 부모 불의 childFire 참조 해제
            if (parentFire != null && parentFire.childFire == this)
                parentFire.childFire = null;

            // 자식 불도 같이 제거
            if (childFire != null && !childFire.isDead)
                childFire.Extinguish(childFire.firePower + 1f);

            if (fireHealthUI != null)
                fireHealthUI.UnregisterFire(this);

            gameManager.FireExtinguished();
            Destroy(gameObject);
        }
    }

    public bool HasChild() => childFire != null;

    public void TryRecover()
    {
        if (isDead) return;
        if (Time.time - lastHitTime >= 30f)
        {
            float lost = maxFirePower - firePower;
            firePower = Mathf.Min(firePower + lost * 0.5f, maxFirePower);
        }
    }

    public bool WasHitRecently() => Time.time - lastHitTime < 30f;
}