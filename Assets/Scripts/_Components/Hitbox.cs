using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public System.Action<Hurtbox> onHit;

    private string targetTag = "";
    private Collider hitboxCollider;

    void Awake()
    {
        hitboxCollider = GetComponent<Collider>();
        if (hitboxCollider == null)
        {
            Debug.LogError("Hitbox requires a Collider component.");
        }
    }

    public void Initialize(string targetTag)
    {
        this.targetTag = targetTag;
    }

    public void SetArea(Vector2 size)
    {
        transform.localScale = new Vector3(size.x, size.y, 1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            var hurtbox = other.GetComponent<Hurtbox>();
            if (hurtbox != null)
            {
                onHit?.Invoke(hurtbox);
            }
        }
    }
}
