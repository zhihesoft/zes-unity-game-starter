using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Zes
{
    public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {


        public Image background;
        public Image handler;
        public float range;
        public bool fixPosition = false; // 是否固定摇杆
        public Color activeBackgroundColor = Color.white;
        public Color inactiveBackgroundColor = new Color(1, 1, 1, 0.5f);
        public Color activeStickColor = Color.white;
        public Color inactiveStickColor = new Color(1, 1, 1, 0.5f);

        public Action<Vector2> onAction;

        private bool pointerDown = false;
        private Vector2 startupPosition = Vector2.zero;

        private void Awake()
        {
            SetActiveStatus(false);
            if (range <= 0)
            {
                range = background.rectTransform.sizeDelta.x / 2;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            SetActiveStatus(true);
            if (!fixPosition)
            {
                background.rectTransform.anchoredPosition = eventData.position - transform.GetComponent<RectTransform>().anchoredPosition;
                startupPosition = eventData.position;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            SetActiveStatus(false);
            background.rectTransform.anchoredPosition = Vector2.zero;
            handler.rectTransform.anchoredPosition = Vector2.zero;
            onAction?.Invoke(Vector2.zero);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!pointerDown)
            {
                return;
            }
            var offset = (eventData.position - startupPosition);
            offset = Vector2.ClampMagnitude(offset, range);
            handler.rectTransform.anchoredPosition = offset;
            var value = offset / range;
            onAction?.Invoke(value);
        }


        private void SetActiveStatus(bool status)
        {
            pointerDown = status;
            background.color = status ? activeBackgroundColor : inactiveBackgroundColor; // new Color(1, 1, 1, status ? 1 : 0.5f);
            handler.color = status ? activeStickColor : inactiveStickColor; // new Color(1, 1, 1, status ? 1 : 0.5f);
        }

    }
}
