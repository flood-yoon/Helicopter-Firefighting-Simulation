using UnityEngine;

public class WaterCollision : MonoBehaviour
{
    public float extinguishPower = 10f;

    void OnParticleCollision(GameObject other)
    {
        Debug.Log("물 충돌함: " + other.name);

        Fire fire = other.GetComponent<Fire>();

        if (fire != null)
        {
            fire.Extinguish(extinguishPower);
        }
    }
}