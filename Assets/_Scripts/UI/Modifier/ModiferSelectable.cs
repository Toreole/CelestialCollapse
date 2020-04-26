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
    }
}