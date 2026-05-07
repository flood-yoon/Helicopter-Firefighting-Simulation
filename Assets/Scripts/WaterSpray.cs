using UnityEngine;
using UnityEngine.InputSystem;

public class WaterSpray : MonoBehaviour
{
    public ParticleSystem waterParticle;

    void Update()
    {
        if (Keyboard.current.spaceKey.isPressed)
        {
            if (!waterParticle.isPlaying)
                waterParticle.Play();
        }
        else
        {
            if (waterParticle.isPlaying)
                waterParticle.Stop();
        }
    }
}