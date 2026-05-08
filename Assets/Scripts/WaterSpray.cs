using UnityEngine;
using UnityEngine.InputSystem;

public class WaterSpray : MonoBehaviour
{
    public ParticleSystem waterParticle;

    private bool waterDisabled = false;

    void Update()
    {
        if (waterDisabled) return;

        if (Keyboard.current.spaceKey.isPressed)
        {
            if (!waterParticle.isPlaying) waterParticle.Play();
        }
        else
        {
            if (waterParticle.isPlaying) waterParticle.Stop();
        }
    }

    public void SetWaterDisabled(bool disabled)
    {
        waterDisabled = disabled;
        if (disabled && waterParticle.isPlaying)
            waterParticle.Stop();
    }

    public void ApplyParticleSettings(float angle, float emissionRate)
    {
        var shape = waterParticle.shape;
        // Cone은 반각이므로 절반으로 나눠서 적용, 최대 실제각도 120도 = 반각 60도
        shape.angle = Mathf.Min(angle / 2f, 60f);

        var emission = waterParticle.emission;
        emission.rateOverTime = emissionRate;
    }
}