using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace M8 {

public static class Buttons
{
    public const int Edit   = 1;
    public const int Option = 1 << 1;
    public const int Right  = 1 << 2;
    public const int Play  = 1 << 3;
    public const int Shift = 1 << 4;
    public const int Down   = 1 << 5;
    public const int Up     = 1 << 6;
    public const int Left   = 1 << 7;
}

[RequireComponent(typeof(PlayerInput))]
public sealed class InputHandler : MonoBehaviour
{
	byte _prev;
	private bool _hasInitialized;
	public byte CurrentState => _prev;

	[SerializeField, Range(0f, 0.9f)]
	private float analogDeadzone;

	private InputAction _moveAction;
	private InputAction _playAction;
	private InputAction _shiftAction;
	private InputAction _editAction;
	private InputAction _optionAction;

	private bool Play => _playAction.IsPressed();
	private bool Shift => _shiftAction.IsPressed();
	private bool Edit => _editAction.IsPressed();
	private bool Option => _optionAction.IsPressed();

	private void Start()
	{
		var input = GetComponent<PlayerInput>().currentActionMap;
		_moveAction = input.FindAction("Move");
		_playAction = input.FindAction("Play");
		_shiftAction = input.FindAction("Shift");
		_editAction = input.FindAction("Edit");
		_optionAction = input.FindAction("Option");
		_hasInitialized = true;
	}

	private byte GetState()
    {
	    var move = _moveAction.ReadValue<Vector2>();
        var state = (byte)0;
        if (move.x > analogDeadzone) state += Buttons.Right;
        if (move.x < -analogDeadzone) state += Buttons.Left;
        if (move.y > analogDeadzone) state += Buttons.Up;
        if (move.y < -analogDeadzone) state += Buttons.Down;
        if (Edit) state += Buttons.Edit;
        if (Option) state += Buttons.Option;
        if (Play) state += Buttons.Play;
        if (Shift) state += Buttons.Shift;
        return state;
    }

    public bool UpdateState()
    {
	    if (!_hasInitialized)
		    return false;
        var state = GetState();
        var changed = (_prev != state);
        _prev = state;
        return changed;
    }
}

} // namespace M8
