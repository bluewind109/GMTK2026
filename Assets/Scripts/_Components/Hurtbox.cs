using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    public System.Action<int> onHit;

    public void SetTag(string tag)
    {
        this.gameObject.tag = tag;
    } 

    public void TakeDamage(int damage)
    {
        onHit?.Invoke(damage);
    }
}
