using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private Vector3 uiPosition;

    private void Awake()
    {
        uiPosition = GetComponent<RectTransform>().position;
    }

    void Update()
    {
        //Touch-screen movement
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 touchPos = touch.position;

                ClampToContiner(touchPos, gameObject.GetComponent<RectTransform>(), gameObject.transform.parent.gameObject.GetComponent<RectTransform>());
            }
        }

#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            ClampToContiner(Input.mousePosition, gameObject.GetComponent<RectTransform>(), gameObject.transform.parent.gameObject.GetComponent<RectTransform>());
        }
        else if (Input.GetMouseButtonUp(0))
        {
            gameObject.GetComponent<RectTransform>().position = uiPosition;
        }
#endif
    }

    private void ClampToContiner(Vector3 myMouse, RectTransform panelRectTransform, RectTransform parentRectTransform)
    {

        panelRectTransform.transform.position = myMouse;

        Vector3 pos = panelRectTransform.localPosition;

        Vector3 minPosition = parentRectTransform.rect.min - panelRectTransform.rect.min;
        Vector3 maxPosition = parentRectTransform.rect.max - panelRectTransform.rect.max;

        pos.x = Mathf.Clamp(Mathf.Lerp(panelRectTransform.localPosition.x, myMouse.x, 0.2f), minPosition.x, maxPosition.x);
        pos.y = Mathf.Clamp(Mathf.Lerp(panelRectTransform.localPosition.y, myMouse.y, 0.2f), minPosition.y, maxPosition.y);

        panelRectTransform.localPosition = pos;
    }

}
