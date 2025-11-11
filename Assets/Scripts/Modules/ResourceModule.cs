using System;
using UnityEngine;

[Serializable]
public class ResourceModule : ModuleBase
{
    public event Action OnResourceChanged;
    public event Action OnResourceDepleted;

    public float Max { get; private set; }
    public float Current { get; private set; }
    public bool DisableOnDeplete { get; private set; } = false;
    public bool HasResource => Current > 0f;
    public float Normalized => Max > 0f ? Current / Max : 0f;
    public float RegenAmount = 0f;
    public float RegenInterval = 5f;
    public float RegenDelay = 0f;
    private float regenTimer = 0f;
    private float restTimer = 0f;

    public ResourceModule(bool disableOnDeplete) : base(false, true)
    {
        DisableOnDeplete = disableOnDeplete;
    }

    public override void DoFixedUpdate(float fixedDeltaTime)
    {
        if (!Enabled) return;
        if (RegenAmount <= 0f) return;
        if (Current >= Max) return;

        if (restTimer > 0f)
        {
            restTimer -= fixedDeltaTime;
            if (restTimer > 0f) return;
            restTimer = 0f;
        }

        regenTimer += fixedDeltaTime;
        if (regenTimer >= RegenInterval)
        {
            regenTimer -= RegenInterval;
            Recover(RegenAmount);
        }
    }

    public void SubscribeToEvents(Action<float> maxValueChanged)
    {
        maxValueChanged += (newMax) => SetMaxValue(newMax, true);
    }

    public void Reset(float maxValue)
    {
        Max = maxValue;
        Current = maxValue;
    }

    public void SetMaxValue(float newMaxValue, bool refill = false, bool refillAll = false)
    {
        if (refill)
        {
            Current = refillAll ? newMaxValue : Mathf.Min(Current, newMaxValue);
        }
        else
            Current = Mathf.Min(Current, newMaxValue);

        Max = newMaxValue;
        OnResourceChanged?.Invoke();
    }

    public bool HasAtLeast(float amount) => Current >= amount;

    public float Use(float amount)
    {
        if (!HasResource && DisableOnDeplete) return 0f;
        if (amount <= 0f) return 0f;

        var usedAmount = Mathf.Min(Current, amount);
        Current -= usedAmount;

        restTimer = RegenDelay;
        regenTimer = 0f;

        OnResourceChanged?.Invoke();

        if (Current <= 0)
            Deplete();

        return usedAmount;
    }

    public float Recover(float amount)
    {
        if (!HasResource && DisableOnDeplete) return 0f;
        if (amount <= 0f) return 0f;
        if (Current >= Max) return 0f;

        var refillAmount = Mathf.Min(Max - Current, amount);
        Current += refillAmount;

        OnResourceChanged?.Invoke();

        return refillAmount;
    }

    private void Deplete()
    {
        OnResourceDepleted?.Invoke();
        if (DisableOnDeplete) Enable(false);
    }
}
