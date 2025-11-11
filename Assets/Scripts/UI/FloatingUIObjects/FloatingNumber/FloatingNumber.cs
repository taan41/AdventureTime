using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Random = UnityEngine.Random;

// [RequireComponent(typeof(CanvasRenderer))]
// [RequireComponent(typeof(Text))]
// [RequireComponent(typeof(Outline))]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(TextMeshPro))]
public class FloatingNumber : FloatingUIObject
{
    [System.Serializable]
    public struct FloatingNumberColor
    {
        public Color minColor;
        public Color maxColor;
        public Color outlineColor;
    }

    private static Unity.Mathematics.Random rng = new((uint)System.DateTime.Now.Ticks);

    private FloatingNumberData data;
    private RectTransform rectTransform;
    private MeshRenderer meshRenderer;
    private TextMeshPro text;
    // private Material material;
    // private Outline outline;

    private float elapsedTime;
    private float duration;
    private float minSpeed;
    private Vector3 speed = Vector3.zero;
    private Vector3 acceleration = Vector3.zero;

    private bool initialized = false;

    public void Initialize(FloatingNumberData newData)
    {
        if (initialized) return;
        initialized = true;

        rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(1, 1);
        rectTransform.localScale = Vector3.one;

        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;

        text = GetComponent<TextMeshPro>();
        text.alignment = TextAlignmentOptions.Center;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        text.overflowMode = TextOverflowModes.Overflow;
        text.raycastTarget = false;
        text.sortingLayerID = SortingLayer.NameToID("Foreground");
        text.enabled = false;

        data = newData;
        data.OnDataChanged += SetupData;
        SetupData();

        Enable(false);
    }

    private void SetupData()
    {
        text.font = data.tmpFont;
        text.fontSize = data.fontSize;
    }

    public override void Enable(bool enabled)
    {
        meshRenderer.enabled = enabled;
        text.enabled = enabled;
        // outline.enabled = enabled;

        base.Enable(enabled);
    }

    public void Activate(float value, Vector3 position, FloatingNumberColor color, Material material, bool critical = false)
    {
        float lerp = Mathf.Clamp01(value / data.maxOnNumber);
        float randomFactor = rng.NextFloat(data.minRandomFactor, data.maxRandomFactor);
        float randomFactor2 = rng.NextFloat(data.minRandomFactor, data.maxRandomFactor);
        float randomFactor3 = rng.NextFloat(data.minRandomFactor, data.maxRandomFactor);

        text.text = critical ? $"{value:F0}!" : value.ToString("F0");

        text.color = Color.Lerp(color.minColor, color.maxColor, lerp + randomFactor * data.colorVarianceFactor);
        text.fontMaterial = material;

        elapsedTime = 0f;
        duration = data.duration + randomFactor2 * data.durationVariance;
        if (duration < 0.1f) duration = 0.1f;

        float scale = CMath.Lerp(data.minScale, data.maxScale, lerp);

        minSpeed = data.minSpeed * scale;
        speed = new Vector3(0f, (data.speed + randomFactor3 * data.speedVariance) * scale, 0f);
        acceleration = new Vector3(0f, data.acceleration * scale, 0f);

        rectTransform.position = position + (Vector3)data.positionOffset + new Vector3(randomFactor * data.positionOffsetVariance.x, randomFactor2 * data.positionOffsetVariance.y, 0f);
        rectTransform.localScale = new Vector3(scale, scale, 1f);
        rectTransform.rotation = Quaternion.Euler(0f, 0f, randomFactor3 * data.rotationVariance);

        Enable(true);
    }

    // public void Activate(float value, Vector3 position, TMP_FontAsset font, bool critical = false)
    // {
    //     float lerp = Mathf.Clamp01(value / data.maxOnNumber);
    //     float randomFactor = Random.Range(data.minRandomFactor, data.maxRandomFactor);
    //     float randomFactor2 = Random.Range(data.minRandomFactor, data.maxRandomFactor);
    //     float randomFactor3 = Random.Range(data.minRandomFactor, data.maxRandomFactor);

    //     text.text = critical ? $"{value:F0}!" : value.ToString("F0");
    //     text.font = font;
    //     // outline.effectColor = color.outlineColor;

    //     elapsedTime = 0f;
    //     duration = data.duration + randomFactor2 * data.durationVariance;
    //     if (duration < 0.1f) duration = 0.1f;

    //     float scale = CMath.Lerp(data.minScale, data.maxScale, lerp);

    //     minSpeed = data.minSpeed * scale;
    //     speed = new Vector3(0f, (data.speed + randomFactor3 * data.speedVariance) * scale, 0f);
    //     acceleration = new Vector3(0f, data.acceleration * scale, 0f);

    //     rectTransform.position = position + (Vector3)data.positionOffset + new Vector3(randomFactor * data.positionOffsetVariance.x, randomFactor2 * data.positionOffsetVariance.y, 0f);
    //     rectTransform.localScale = new Vector3(scale, scale, 1f);
    //     rectTransform.rotation = Quaternion.Euler(0f, 0f, randomFactor3 * data.rotationVariance);

    //     Enable(true);

    // }

    public override void DoUpdate(float deltaTime)
    {
        elapsedTime += deltaTime;
        if (elapsedTime >= duration)
        {
            elapsedTime = duration;
            Enable(false);
            FloatingUIObjectManager.Instance.Return(this);
            return;
        }

        if (speed.y > minSpeed)
        {
            speed.y += acceleration.y * deltaTime;
            if (speed.y < minSpeed) speed.y = minSpeed;
        }
        rectTransform.position += speed * deltaTime;
    }

    public static FloatingNumber Create(Transform parent, FloatingNumberData data)
    {
        var obj = new GameObject("FloatingNumber");
        obj.transform.SetParent(parent, false);
        var number = obj.AddComponent<FloatingNumber>();
        number.Initialize(data);
        return number;
    }
}
