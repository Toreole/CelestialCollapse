using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UIDebugInput : MonoBehaviour
{
    public Image button_A, button_B, button_Y, button_X, button_Menu;
    public Image rightTrigger, rightBumper, leftTrigger, leftBumper;
    public Image dPad_up, dPad_down, dPad_left, dPad_right;
    public Image lStick, rStick;
    public Color pressedColor;
    
    // Update is called once per frame
    void Update()
    {
        var pad = Gamepad.current;
        if(pad != null)
        {
            button_A.color = pad.buttonSouth.isPressed ? pressedColor : Color.white;
            button_B.color = pad.buttonEast.isPressed ? pressedColor : Color.white;
            button_X.color = pad.buttonWest.isPressed ? pressedColor : Color.white;
            button_Y.color = pad.buttonNorth.isPressed ? pressedColor : Color.white;
            button_Menu.color = pad.startButton.isPressed ? pressedColor : Color.white;

            Vector2 dpad = pad.dpad.ReadValue();
            dPad_up.color = dpad.y > 0.1f ? pressedColor : Color.white;
            dPad_down.color = dpad.y < -0.1f ? pressedColor : Color.white;
            dPad_right.color = dpad.x > 0.1f ? pressedColor : Color.white;
            dPad_left.color = dpad.x < -0.1f ? pressedColor : Color.white;

            rightTrigger.color = pad.rightTrigger.isPressed ? pressedColor : Color.white;
            leftTrigger.color = pad.leftTrigger.isPressed ? pressedColor : Color.white;
            rightBumper.color = pad.rightShoulder.isPressed ? pressedColor : Color.white;
            leftBumper.color = pad.leftShoulder.isPressed ? pressedColor : Color.white;

            lStick.color = pad.leftStick.ReadValue().sqrMagnitude > 0.1f ? pressedColor : Color.white;
            rStick.color = pad.rightStick.ReadValue().sqrMagnitude > 0.1f ? pressedColor : Color.white;
        }
    }
}
