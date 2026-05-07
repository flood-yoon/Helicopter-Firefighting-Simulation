using UnityEngine;

public class Fire : MonoBehaviour
{
    public float firePower = 100f;

    private GameManager gameManager;

    private bool isDead = false;

    void Start()
    {
        gameManager =
            FindAnyObjectByType<GameManager>();
    }

    public void Extinguish(float amount)
    {
        if (isDead)
            return;

        firePower -= amount;

        if (firePower <= 0)
        {
            isDead = true;

            gameManager.FireExtinguished();

            Destroy(gameObject);
        }
    }
}