using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Celestial.UI
{
    public class UITooltip : MonoBehaviour
    {
        private static UITooltip _instance;

        [SerializeField, Tooltip("the text area")]
        private TextMeshProUGUI content;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(this.gameObject);
            gameObject.SetActive(false);
        }

        public static void Show(string text, Vector3 position) => _instance._Show(text, position);
        private void _Show(string text, Vector3 position)
        {
            content.text = text;
            transform.position = position;
            gameObject.SetActive(true);
        }

        public static void Hide() => _instance._Hide();
        private void _Hide()
        {
            gameObject.SetActive(false);
        }
    }
}