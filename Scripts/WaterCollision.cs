using UnityEngine;
using System.Collections.Generic;

public class WaterCollision : MonoBehaviour
{
    private List<ParticleCollisionEvent> collisionEvents
        = new List<ParticleCollisionEvent>();

    public float extinguishPower = 10f;

    void OnParticleCollision(GameObject other)
    {
        Fire fire = other.GetComponent<Fire>();

        if (fire != null)
        {
            fire.Extinguish(extinguishPower * Time.deltaTime);
        }
    }
}