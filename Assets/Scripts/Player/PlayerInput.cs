using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public System.Action<eDirection> onDirectionPressed;
    public System.Action<eAttackType> onAttackPressed;

    private const KeyCode ROTATE_LEFT = KeyCode.A;
    private const KeyCode ROTATE_RIGHT = KeyCode.D;

    private const KeyCode STANDING_ATTACK = KeyCode.J;
    private const KeyCode DASH_ATTACK = KeyCode.K;
    private const KeyCode SWIPE_ATTACK = KeyCode.L;
    private const KeyCode DRILL_ATTACK = KeyCode.I;

    public void UpdateAttackInput()
    {
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
        if (Input.GetKey(ROTATE_LEFT))
        {
            onDirectionPressed?.Invoke(eDirection.Left);
        }
        else if (Input.GetKey(ROTATE_RIGHT))
        {
            onDirectionPressed?.Invoke(eDirection.Right);
        }
    }
}

public enum eDirection
{
    Left,
    Right
}
