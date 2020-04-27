using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace Celestial.UI
{
    public class UITooltip : MonoBehaviour
    {
        private static UITooltip _instance;

        [SerializeField, Tooltip("the text area")]
        private TextMeshProUGUI content;

        UnityEngine.UI.VerticalLayoutGroup layoutGroup;
        GameObject contentChild;

        //set up the tooltip object.
        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(this.gameObject);
            contentChild = transform.GetChild(0).gameObject;
            contentChild.SetActive(false);
            layoutGroup = GetComponent<UnityEngine.UI.VerticalLayoutGroup>();
        }

        public static void Show(string text, Vector3 position) => _instance._Show(text, position);
        private void _Show(string text, Vector3 position)
        {
            content.text = text;
            transform.position = position;
            contentChild.SetActive(true);
            SetRealPosition(position);
        }

        /// <summary>
        /// Update position with no arguments, requires a Mouse.
        /// </summary>
        public static void UpdatePosition() => _instance._UpdatePosition();
        private void _UpdatePosition()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null)
                return;
            Vector2 mousePos = mouse.position.ReadValue();
            transform.position = mousePos;
            SetRealPosition(mousePos);
        }

        /// <summary>
        /// Set the "real" position, so it doesnt overlap with the cursor, and remains onscreen as a whole.
        /// </summary>
        /// <param name="relativeCenter"></param>
        private void SetRealPosition(Vector2 relativeCenter)
        {
            Vector2 mid = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            //get current anchored position.
            RectTransform rect = transform as RectTransform;
            Vector2 anchoredPos = rect.anchoredPosition;
            //offset by half height/width of the layout.
            float xOffset = 0.5f * (relativeCenter.x < mid.x ? layoutGroup.preferredWidth : -layoutGroup.preferredWidth);
            float yOffset = 0.5f * (relativeCenter.y < mid.y ? layoutGroup.preferredHeight : -layoutGroup.preferredHeight);
            anchoredPos.x += xOffset;
            anchoredPos.y += yOffset;
            //set it!
            rect.anchoredPosition = anchoredPos;
        }

        public static void Hide() => _instance._Hide();
        private void _Hide()
        {
            contentChild.SetActive(false);
        }
    }
}