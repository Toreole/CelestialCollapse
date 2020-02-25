using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Celestial
{
    public class ControllerIconSet : ScriptableObject
    {
        [SerializeField]
        protected Sprite northButton, eastButton, southButton, westButton;
        [SerializeField]
        protected Sprite startButton, backButton;
        [SerializeField]
        protected Sprite leftStick, rightStick;
        [SerializeField]
        protected Sprite leftTrigger, rightTrigger, leftShoulder, rightShoulder;
        [SerializeField]
        protected Sprite dPadNeutral, dPadUp, dPadRight, dPadDown, dPadLeft;

        public Sprite NorthButton => northButton;
        public Sprite EastButton => eastButton;
        public Sprite SouthButton => southButton;
        public Sprite WestButton => westButton;

        public Sprite StartButton => startButton;
        public Sprite BackButton => backButton;

        public Sprite LeftStick => leftStick;
        public Sprite RightStick => rightStick;

        public Sprite LeftTrigger => leftTrigger;
        public Sprite RightTrigger => rightTrigger;
        public Sprite LeftShoulder => leftShoulder;
        public Sprite RightShoulder => rightShoulder;

        public Sprite DPadNeutral => dPadNeutral;
        public Sprite DPadUp => dPadUp;
        public Sprite DPadRight => dPadRight;
        public Sprite DPadDown => dPadDown;
        public Sprite DPadLeft => dPadLeft;

        public Sprite GetSprite(string path)
        {
            switch(path)
            {
                case "buttonSouth": return southButton;
                case "buttonNorth": return northButton;
                case "buttonEast": return eastButton;
                case "buttonWest": return westButton;
                case "start": return startButton;
                case "select": return backButton;
                case "leftTrigger": return leftTrigger;
                case "rightTrigger": return rightTrigger;
                case "leftShoulder": return leftShoulder;
                case "rightShoulder": return rightShoulder;
                case "dpad": return dPadNeutral;
                case "dpad/up": return dPadUp;
                case "dpad/down": return dPadDown;
                case "dpad/left": return dPadLeft;
                case "dpad/right": return dPadRight;
                case "leftStick": return leftStick;
                case "rightStick": return rightStick;
                //case "leftStickPress": return leftStickPress;
                //case "rightStickPress": return rightStickPress;
            }
            return null;
        }
    }
}