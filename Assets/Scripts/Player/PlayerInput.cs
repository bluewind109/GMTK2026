using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public System.Action<eDirection> onDirectionPressed;
    public System.Action<eAttackType> onAttackPressed;

    private const KeyCode ROTATE_LEFT = KeyCode.A;
    private const KeyCode ROTATE_RIGHT = KeyCode.D;
    private const KeyCode LOOK_UP = KeyCode.W;
    private const KeyCode LOOK_DOWN = KeyCode.S;

    private const KeyCode STANDING_ATTACK = KeyCode.J;
    private const KeyCode DASH_ATTACK = KeyCode.K;
    private const KeyCode SWIPE_ATTACK = KeyCode.L;
    private const KeyCode DRILL_ATTACK = KeyCode.I;

    private bool isDirectionInputEnabled = false;
    private bool isAttackInputEnabled = false;

    void Start()
    {
        EnableDirectionInput(true);
        EnableAttackInput(false);
    }

    public void EnableDirectionInput(bool enable)
    {
        isDirectionInputEnabled = enable;
    }

    public void EnableAttackInput(bool enable)
    {
        isAttackInputEnabled = enable;
    }

    public void UpdateAttackInput()
    {
        if (!isAttackInputEnabled) return;

        if (Input.GetKeyDown(STANDING_ATTACK))
        {
            onAttackPressed?.Invoke(eAttackType.Standing);
        }
        else if (Input.GetKeyDown(DASH_ATTACK))
        {
            onAttackPressed?.Invoke(eAttackType.Dash);
        }
        else if (Input.GetKeyDown(SWIPE_ATTACK))
        {
            onAttackPressed?.Invoke(eAttackType.Swipe);
        }
        else if (Input.GetKeyDown(DRILL_ATTACK))
        {
            onAttackPressed?.Invoke(eAttackType.Drill);
        }
    }

    public void UpdateDirectionInput()
    {
        if (!isDirectionInputEnabled) return;

        if (Input.GetKeyDown(ROTATE_LEFT))
        {
            onDirectionPressed?.Invoke(eDirection.Left);
        }
        else if (Input.GetKeyDown(ROTATE_RIGHT))
        {
            onDirectionPressed?.Invoke(eDirection.Right);
        }
        else if (Input.GetKeyDown(LOOK_UP))
        {
            onDirectionPressed?.Invoke(eDirection.Up);
        }
        else if (Input.GetKeyDown(LOOK_DOWN))
        {
            onDirectionPressed?.Invoke(eDirection.Down);
        }
    }
}

public enum eDirection
{
    Left,
    Right,
    Up,
    Down
}
