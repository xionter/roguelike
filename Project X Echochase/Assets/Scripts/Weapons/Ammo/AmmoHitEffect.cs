using UnityEngine;

[DisallowMultipleComponent]
public class AmmoHitEffect : MonoBehaviour
{
    private ParticleSystem ammoHitEffectParticleSystem;

    private void Awake()
    {
        ammoHitEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// Устанавливаем HitEffect в зависимости от переданных WeaponHitEffectSO
    /// </summary>
    public void SetHitEffect(AmmoHitEffectSO ammoHitEffect)
    {
        // градиент
        SetHitEffectColorGradient(ammoHitEffect.colorGradient);

        // начальные значения для частиц
        SetHitEffectParticleStartingValues(ammoHitEffect.duration, ammoHitEffect.startParticleSize, ammoHitEffect.startParticleSpeed, ammoHitEffect.startLifetime, ammoHitEffect.effectGravity, ammoHitEffect.maxParticleNumber);

        // emission(хз как на русском), распределние мб
        SetHitEffectParticleEmission(ammoHitEffect.emissionRate, ammoHitEffect.burstParticleNumber);

        // спрайт
        SetHitEffectParticleSprite(ammoHitEffect.sprite);

        // жизнь частиц и мин/макс скорости
        SetHitEffectVelocityOverLifeTime(ammoHitEffect.velocityOverLifetimeMin, ammoHitEffect.velocityOverLifetimeMax);

    }

    private void SetHitEffectColorGradient(Gradient gradient)
    {
        var colorOverLifetimeModule = ammoHitEffectParticleSystem.colorOverLifetime;
        colorOverLifetimeModule.color = gradient;
    }

    private void SetHitEffectParticleStartingValues(float duration, float startParticleSize, float startParticleSpeed, float startLifetime, float effectGravity, int maxParticles)
    {
        var mainModule = ammoHitEffectParticleSystem.main;

        mainModule.duration = duration;

        mainModule.startSize = startParticleSize;

        mainModule.startSpeed = startParticleSpeed;

        mainModule.startLifetime = startLifetime;

        mainModule.gravityModifier = effectGravity;

        mainModule.maxParticles = maxParticles;
    }

    private void SetHitEffectParticleEmission(int emissionRate, float burstParticleNumber)
    {
        var emissionModule = ammoHitEffectParticleSystem.emission;

        var burst = new ParticleSystem.Burst(0f, burstParticleNumber);
        emissionModule.SetBurst(0, burst);

        emissionModule.rateOverTime = emissionRate;
    }

    private void SetHitEffectParticleSprite(Sprite sprite)
    {
        var textureSheetAnimationModule = ammoHitEffectParticleSystem.textureSheetAnimation;

        textureSheetAnimationModule.SetSprite(0, sprite);
    }

    private void SetHitEffectVelocityOverLifeTime(Vector3 minVelocity, Vector3 maxVelocity)
    {
        var velocityOverLifetimeModule = ammoHitEffectParticleSystem.velocityOverLifetime;

        // мин макс скорость по X
        var minMaxCurveX = new ParticleSystem.MinMaxCurve();
        minMaxCurveX.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveX.constantMin = minVelocity.x;
        minMaxCurveX.constantMax = maxVelocity.x;
        velocityOverLifetimeModule.x = minMaxCurveX;

        // мин макс скорость по Y
        var minMaxCurveY = new ParticleSystem.MinMaxCurve();
        minMaxCurveY.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveY.constantMin = minVelocity.y;
        minMaxCurveY.constantMax = maxVelocity.y;
        velocityOverLifetimeModule.y = minMaxCurveY;

        // мин макс скорость по Z
        var minMaxCurveZ = new ParticleSystem.MinMaxCurve();
        minMaxCurveZ.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveZ.constantMin = minVelocity.z;
        minMaxCurveZ.constantMax = maxVelocity.z;
        velocityOverLifetimeModule.z = minMaxCurveZ;

    }

}