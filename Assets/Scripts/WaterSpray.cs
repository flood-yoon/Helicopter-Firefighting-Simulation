using UnityEngine;
using UnityEngine.InputSystem;

public class WaterSpray : MonoBehaviour
{
    public ParticleSystem waterParticle;

    // GameManager에서 라운드마다 직접 조절
    [HideInInspector] public float baseEmissionRate = 10f;
    [HideInInspector] public float baseAngle = 10f;

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

    public void ApplyParticleSettings(float angle, float emissionRate)
    {
        var shape = waterParticle.shape;
        shape.angle = Mathf.Min(angle, 120f);  // 최대 120도 고정

        var emission = waterParticle.emission;
        emission.rateOverTime = emissionRate;
    }
}