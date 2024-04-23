using Drawing;
using UnityEngine;

namespace DailyMeals
{
    public class DrawDisplay : MonoBehaviour
    {
        private Vector3 startPos;
        private Vector3 currentPos;
        private Vector3 endPos;
        private Color c1 = new Color(1f, 0.8f, 0.9f);

        private void Update()
        {
            RenderPointerTrack();
        }
        
        private void RenderPointerTrack()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    startPos = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    currentPos = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    endPos = touch.position;

                }
            }


            // drawCB.PopMatrix();
            using (var draw = DrawingManager.GetBuilder(true)) {
                using (draw.InScreenSpace(Camera.main)) {
                    using (draw.WithDuration(0.3f))
                    {
                        if (!currentPos.Equals(default))
                        {
                            draw.PushLineWidth(5);
                            draw.PushColor(c1);
                            draw.xy.Line(startPos, currentPos);
                            draw.PopColor();
                            draw.PopLineWidth();
                            startPos = currentPos;
                            currentPos = default;
                        }
                    }


                }
            }
        }
    }
}
