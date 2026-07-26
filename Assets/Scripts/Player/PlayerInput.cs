using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public System.Action<eDirection> onDirectionPressed;
    public System.Action<eAttackType> onAttackPressed;

    private const KeyCode LOOK_LEFT = KeyCode.A;
    private const KeyCode LOOK_RIGHT = KeyCode.D;
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

        bool leftDown  = Input.GetKeyDown(LOOK_LEFT);
        bool rightDown = Input.GetKeyDown(LOOK_RIGHT);
        bool upDown    = Input.GetKeyDown(LOOK_UP);
        bool downDown  = Input.GetKeyDown(LOOK_DOWN);

        bool leftHeld  = Input.GetKey(LOOK_LEFT);
        bool rightHeld = Input.GetKey(LOOK_RIGHT);
        bool upHeld    = Input.GetKey(LOOK_UP);
        bool downHeld  = Input.GetKey(LOOK_DOWN);

        // Diagonal: fires when one key is freshly pressed while the other is already held.
        // This makes it reliable regardless of which key the player presses first.
        if ((leftDown && upHeld) || (leftHeld && upDown))
        {
            onDirectionPressed?.Invoke(eDirection.TopLeft);
        }
        else if ((leftDown && downHeld) || (leftHeld && downDown))
        {
            onDirectionPressed?.Invoke(eDirection.BottomLeft);
        }
        else if ((rightDown && upHeld) || (rightHeld && upDown))
        {
            onDirectionPressed?.Invoke(eDirection.TopRight);
        }
        else if ((rightDown && downHeld) || (rightHeld && downDown))
        {
            onDirectionPressed?.Invoke(eDirection.BottomRight);
        }
        // Cardinal: fires only when no perpendicular key is held, so that
        // pressing a second key always upgrades a cardinal to a diagonal.
        else if (leftDown && !upHeld && !downHeld)
        {
            onDirectionPressed?.Invoke(eDirection.Left);
        }
        else if (rightDown && !upHeld && !downHeld)
        {
            onDirectionPressed?.Invoke(eDirection.Right);
        }
        else if (upDown && !leftHeld && !rightHeld)
        {
            onDirectionPressed?.Invoke(eDirection.Up);
        }
        else if (downDown && !leftHeld && !rightHeld)
        {
            onDirectionPressed?.Invoke(eDirection.Down);
        }
    }
}

public enum eDirection
{
    Left,
    TopLeft,
    BottomLeft,
    Right,
    TopRight,
    BottomRight,
    Up,
    Down
}
