using UnityEngine;
using System.Collections;

public class HitFlash : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField, Min(0f)] private float holdDuration = 0.04f;
    [SerializeField, Min(0f)] private float fadeDuration = 0.12f;
    [SerializeField] private string flashAmountProperty = "_FlashAmount";
    [SerializeField] private string flashColorProperty = "_FlashColor";

    private MaterialPropertyBlock _propertyBlock;
    private Coroutine _flashRoutine;
    private int _flashAmountId;
    private int _flashColorId;

    private void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<SpriteRenderer>();
        }

        _propertyBlock = new MaterialPropertyBlock();
        _flashAmountId = Shader.PropertyToID(flashAmountProperty);
        _flashColorId = Shader.PropertyToID(flashColorProperty);

        if (targetRenderer == null)
        {
            Debug.LogError($"{nameof(HitFlash)} on {name} needs a {nameof(SpriteRenderer)} reference.", this);
            return;
        }

        ApplyFlash(0f);
    }

    private void OnDisable()
    {
        if (_flashRoutine != null)
        {
            StopCoroutine(_flashRoutine);
            _flashRoutine = null;
        }

        if (targetRenderer != null)
        {
            ApplyFlash(0f);
        }
    }

    public void TriggerHitFlash()
    {
        if (targetRenderer == null)
        {
            Debug.LogError($"{nameof(HitFlash)} on {name} cannot flash because no {nameof(SpriteRenderer)} is assigned.", this);
            return;
        }

        if (_flashRoutine != null)
        {
            StopCoroutine(_flashRoutine);
        }

        _flashRoutine = StartCoroutine(HitFlashRoutine());
    }

    private IEnumerator HitFlashRoutine()
    {
        ApplyFlash(1f);

        if (holdDuration > 0f)
        {
            yield return new WaitForSeconds(holdDuration);
        }

        if (fadeDuration <= 0f)
        {
            ApplyFlash(0f);
            _flashRoutine = null;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            ApplyFlash(1f - t);
            yield return null;
        }

        ApplyFlash(0f);
        _flashRoutine = null;
    }

    private void ApplyFlash(float amount)
    {
        targetRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor(_flashColorId, flashColor);
        _propertyBlock.SetFloat(_flashAmountId, Mathf.Clamp01(amount));
        targetRenderer.SetPropertyBlock(_propertyBlock);
    }
}
