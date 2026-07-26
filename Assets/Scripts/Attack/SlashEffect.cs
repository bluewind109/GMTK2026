using System.Collections;
using UnityEngine;

/// <summary>
/// Plays a slash-split animation on a SpriteRenderer that uses the
/// "Custom/Sprites/SlashEffect" shader.
///
/// Quick start:
///   1. Create a Material from Custom/Sprites/SlashEffect and assign it to the SpriteRenderer.
///   2. Add this component to the same GameObject.
///   3. Call TriggerSlash() from code or a UnityEvent.
///
/// Atlas sprites are handled automatically — the UV rect is detected and
/// pushed to the shader in Awake. If the displayed sprite changes at runtime
/// (e.g. from an Animator), call RefreshSprite() after the change so the
/// shader gets the updated UV rect.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SlashEffect : MonoBehaviour
{
    [Header("Slash Visual")]
    [Tooltip("Angle of the slash line in degrees. 0 = horizontal, -45 = diagonal.")]
    public float slashAngle = -45f;

    [Tooltip("Shifts the slash line from the sprite centre (sprite-local space, -0.5 to 0.5).")]
    [Range(-0.5f, 0.5f)]
    public float slashOffset = 0f;

    [Tooltip("Maximum distance each half travels from the cut (UV units).")]
    [Range(0f, 0.5f)]
    public float separationAmount = 0.15f;

    [Tooltip("How much the pieces fade as they separate (0 = no fade, 1 = fully transparent).")]
    [Range(0f, 1f)]
    public float fadeAmount = 0.7f;

    [Header("Animation")]
    [Tooltip("Duration of the full slash animation in seconds.")]
    public float duration = 0.4f;

    [Tooltip("Optional delay before the animation starts.")]
    public float startDelay = 0f;

    [Tooltip("Destroy this GameObject once the animation finishes.")]
    public bool destroyOnComplete = false;

    // Shader property IDs cached for performance
    private static readonly int SlashProgressID   = Shader.PropertyToID("_SlashProgress");
    private static readonly int SlashAngleID       = Shader.PropertyToID("_SlashAngle");
    private static readonly int SlashOffsetID      = Shader.PropertyToID("_SlashOffset");
    private static readonly int SeparationAmountID = Shader.PropertyToID("_SeparationAmount");
    private static readonly int FadeAmountID       = Shader.PropertyToID("_FadeAmount");
    private static readonly int UVRectID           = Shader.PropertyToID("_UVRect");

    private SpriteRenderer _sr;
    private Material       _material;
    private Coroutine      _activeCoroutine;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        // Per-instance material so we don't modify the shared asset.
        _material = new Material(_sr.sharedMaterial);
        _sr.material = _material;

        // Push the atlas UV rect right away (works for standalone sprites too).
        RefreshSprite();
    }

    private void OnDestroy()
    {
        if (_material != null)
            Destroy(_material);
    }

    // ------------------------------------------------------------------
    // Public API
    // ------------------------------------------------------------------

    /// <summary>Plays the slash animation from the beginning.</summary>
    public void TriggerSlash()
    {
        if (_activeCoroutine != null)
            StopCoroutine(_activeCoroutine);

        PushStaticProperties();
        _activeCoroutine = StartCoroutine(RunAnimation());
    }

    /// <summary>Immediately resets the effect so the sprite looks normal again.</summary>
    public void ResetSlash()
    {
        if (_activeCoroutine != null)
        {
            StopCoroutine(_activeCoroutine);
            _activeCoroutine = null;
        }
        _material.SetFloat(SlashProgressID, 0f);
    }

    /// <summary>
    /// Re-reads the current sprite's UV rect and pushes it to the shader.
    ///
    /// Call this after changing the displayed sprite at runtime (e.g. from an
    /// Animator or SpriteRenderer.sprite = … assignment) so the slash stays
    /// correctly aligned. For standalone sprites this is a no-op in practice
    /// because their rect is always (0, 0, 1, 1).
    /// </summary>
    public void RefreshSprite()
    {
        if (_sr.sprite != null)
            SetAtlasUVRect(ComputeUVRect(_sr.sprite));
    }

    /// <summary>
    /// Manually sets the atlas UV rect used by the shader.
    ///
    /// You only need this if you are computing the rect yourself. In most cases
    /// calling <see cref="RefreshSprite"/> is sufficient — it auto-detects the
    /// rect from the current sprite.
    ///
    /// <para>
    /// How to get the rect manually (if needed):
    /// <code>
    /// Sprite s = GetComponent&lt;SpriteRenderer&gt;().sprite;
    /// Vector2[] uvs = s.uv;   // atlas-space UVs of the sprite mesh
    /// float minU = float.MaxValue, minV = float.MaxValue;
    /// float maxU = float.MinValue, maxV = float.MinValue;
    /// foreach (var uv in uvs)
    /// {
    ///     if (uv.x &lt; minU) minU = uv.x;
    ///     if (uv.y &lt; minV) minV = uv.y;
    ///     if (uv.x &gt; maxU) maxU = uv.x;
    ///     if (uv.y &gt; maxV) maxV = uv.y;
    /// }
    /// slashEffect.SetAtlasUVRect(Rect.MinMaxRect(minU, minV, maxU, maxV));
    /// </code>
    /// </para>
    /// </summary>
    public void SetAtlasUVRect(Rect uvRect)
    {
        // Pack rect as (minU, minV, maxU, maxV) — matches _UVRect in the shader.
        _material.SetVector(UVRectID, new Vector4(
            uvRect.xMin, uvRect.yMin,
            uvRect.xMax, uvRect.yMax));
    }

    public void ToggleSprite(bool enabled) => _sr.enabled = enabled;

    // ------------------------------------------------------------------
    // Private helpers
    // ------------------------------------------------------------------

    /// <summary>
    /// Derives the atlas UV rect from the sprite's vertex UVs.
    /// For a standalone (non-atlas) sprite this returns (0, 0, 1, 1).
    /// </summary>
    private static Rect ComputeUVRect(Sprite sprite)
    {
        Vector2[] uvs = sprite.uv; // already in atlas texture space [0,1]
        float minU = float.MaxValue, minV = float.MaxValue;
        float maxU = float.MinValue, maxV = float.MinValue;
        foreach (var uv in uvs)
        {
            if (uv.x < minU) minU = uv.x;
            if (uv.y < minV) minV = uv.y;
            if (uv.x > maxU) maxU = uv.x;
            if (uv.y > maxV) maxV = uv.y;
        }
        return Rect.MinMaxRect(minU, minV, maxU, maxV);
    }

    private void PushStaticProperties()
    {
        _material.SetFloat(SlashAngleID,       slashAngle);
        _material.SetFloat(SlashOffsetID,      slashOffset);
        _material.SetFloat(SeparationAmountID, separationAmount);
        _material.SetFloat(FadeAmountID,       fadeAmount);
        _material.SetFloat(SlashProgressID,    0f);
    }

    private IEnumerator RunAnimation()
    {
        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _material.SetFloat(SlashProgressID, EaseOutCubic(Mathf.Clamp01(elapsed / duration)));
            yield return null;
        }

        _material.SetFloat(SlashProgressID, 1f);
        _activeCoroutine = null;

        if (destroyOnComplete)
            Destroy(gameObject);
    }

    private static float EaseOutCubic(float t) => 1f - Mathf.Pow(1f - t, 3f);
}
