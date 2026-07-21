using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float moveSpeed = 5f;

    private Vector2 _movement;
    private Rigidbody2D _rb;
    private Animator _animator;
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";
    private int facingDirection = 1;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        _movement.Set(InputManager.Movement.x, InputManager.Movement.y);

        if (_movement.x != 0)
        {
            facingDirection = _movement.x > 0 ? 1 : -1;
            Flip(facingDirection);
        }

        _animator.SetFloat(HORIZONTAL, Mathf.Abs(_movement.x));
        _animator.SetFloat(VERTICAL, Mathf.Abs(_movement.y));

        _rb.linearVelocity = _movement * moveSpeed;

    }

    private void Flip(int direction)
    {
        spriteRenderer.transform.localScale = new Vector3(direction, 1, 1);
    }
}
