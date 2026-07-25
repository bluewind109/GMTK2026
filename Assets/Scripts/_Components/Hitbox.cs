using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public System.Action<Hurtbox> onHit;

    private string targetTag = "";

    public void Initialize(string targetTag)
    {
        this.targetTag = targetTag;
    }

    public void SetTargetTag(string tag)
    {
        targetTag = tag;
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
