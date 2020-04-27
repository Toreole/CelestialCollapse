using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Celestial;
using UnityEngine.UI;

namespace Celestial.UI
{
    public class ModiferSelectable : Selectable
    {
        [SerializeField]
        protected LevelModifier modifier;

        private IEnumerator mouseRoutine;

        protected override void Start()
        {
            mouseRoutine = UpdateTooltip();
        }

        //TODO: selection based tooltip only for gamepad based input
        public override void OnDeselect(BaseEventData eventData)
        {
            UITooltip.Hide();
            base.OnDeselect(eventData);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            UITooltip.Show(modifier.descriptor, transform.position);
            base.OnSelect(eventData);
        }

        //TODO: Pointer stuff only when Mouse&Keyboard is used.
        public override void OnPointerEnter(PointerEventData eventData)
        {
            UITooltip.Show(modifier.descriptor, transform.position);
            StartCoroutine(mouseRoutine);
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            StopCoroutine(mouseRoutine);
            UITooltip.Hide();
        }

        IEnumerator UpdateTooltip()
        {
            for(; ; )
            {
                UITooltip.UpdatePosition();
                yield return null;
            }
        }
    }
}