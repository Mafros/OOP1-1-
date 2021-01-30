using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    void LateUpdate()
    {
        if (Input.GetMouseButton(2))
        {
            Vector2 mousePos = Input.mousePosition;
            mousePos.x /= Screen.width;
            mousePos.y /= Screen.height;
 
            Vector2 delta = mousePos - new Vector2(0.5f, 0.5f);

            float sideBorder = Mathf.Min(Screen.width, Screen.height) / 5f;

            float xDist = Screen.width * (0.5f - Mathf.Abs(delta.x));
            float yDist = Screen.height * (0.5f - Mathf.Abs(delta.y));

            if (xDist < sideBorder || yDist < sideBorder)
            {
                delta = delta.normalized;
                delta *= Mathf.Clamp01(1 - Mathf.Min(xDist, yDist) / sideBorder);

                transform.Translate(delta * 5 * Time.deltaTime, Space.Self);
            }
        }
    }
}
