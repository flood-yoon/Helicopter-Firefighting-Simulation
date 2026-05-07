using UnityEngine;

public class Fire : MonoBehaviour
{
    public float firePower = 100f;

    public void Extinguish(float amount)
    {
        firePower -= amount;

        if (firePower <= 0)
        {
            Destroy(gameObject);
        }
    }
}