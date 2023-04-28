using UnityEngine;

//
// 2020 Pixelbyte Studios LLC
//
namespace Pixelbyte.Extensions
{
    public static class RectTransformExtensions
    {
        /// <summary>
        /// This extension method returns the required local position for the given RectTransform to be aligned with the
        /// given edge of its parent.
        /// Note: If the rect transform also has a ContentSizeFitter component, this may not work correctly!
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="edge"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static Vector3 GetOutsideParentLocalPosition(this RectTransform rt, RectTransform.Edge edge, RectTransform parent = null)
        {
            if (parent == null)
                parent = rt.parent.GetComponent<RectTransform>();
            if (parent == null)
            {
                Dbg.Err("No parent RectTransform found!");
                throw new System.Exception("Parent Rect required!!");
            }

            var p = rt.localPosition;
            if (edge == RectTransform.Edge.Right)
            {
                p.x = parent.rect.xMax - rt.rect.xMin * rt.localScale.x;
            }
            else if (edge == RectTransform.Edge.Left)
            {
                p.x = parent.rect.xMin - rt.rect.xMax * rt.localScale.x;
            }

            if (edge == RectTransform.Edge.Top)
            {
                p.y = parent.rect.yMax + (rt.rect.size.y * rt.pivot.y);
            }
            else if (edge == RectTransform.Edge.Bottom)
            {
                p.y = parent.rect.yMin - (rt.rect.size.y * (1 - rt.pivot.y));
            }
            return p;
        }

        /// <summary>
        /// This extension method sets the RectTransform object's position so that it is
        /// outside sitting on a given edge of its parent RectTransform. If there is no parent, then
        /// the Screen size is used.
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="edge"></param>
        public static void SetOutsideParent(this RectTransform rt, RectTransform.Edge edge, RectTransform parent = null)
        {
            if (parent == null)
                parent = rt.parent.GetComponent<RectTransform>();
            if (parent == null)
            {
                Dbg.Err("No parent RectTransform found!");
                return;
            }

            rt.localPosition = GetOutsideParentLocalPosition(rt, edge, parent);
        }

        public static float GetWorldEdge(this RectTransform rt, RectTransform.Edge edge)
        {
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);

            switch (edge)
            {
                case RectTransform.Edge.Left:
                    return Mathf.Min(corners[0].x, corners[1].x, corners[2].x, corners[3].x);
                case RectTransform.Edge.Right:
                    return Mathf.Max(corners[0].x, corners[1].x, corners[2].x, corners[3].x);
                case RectTransform.Edge.Top:
                    return Mathf.Min(corners[0].y, corners[1].y, corners[2].y, corners[3].y);
                case RectTransform.Edge.Bottom:
                    return Mathf.Max(corners[0].y, corners[1].y, corners[2].y, corners[3].y);
                default:
                    return 0;
            }
        }

        public static void ConstrainToRect(this RectTransform rtrans, Transform parent, Vector2 desiredPos)
        {
            ConstrainToRect(rtrans, parent as RectTransform, desiredPos);
        }

        /// <summary>
        /// Keeps a child rectTransform object completely contained within the given
        /// parent RectTransform
        /// </summary>
        /// <param name="rtrans"></param>
        /// <param name="parent">The parent or constraining RectTransform</param>
        /// <param name="desiredPos">The position in screen coordinates where you want the rect moved</param>
        public static void ConstrainToRect(this RectTransform rtrans, RectTransform parent, Vector2 desiredPos)
        {
            //To do this right, get a bounds for our Rect relative to its parent
            Vector3 actualPos = desiredPos;
            rtrans.position = desiredPos;
            var b = RectTransformUtility.CalculateRelativeRectTransformBounds(parent, rtrans);

            //Because the pivot might be different, we must adjust where the top left of
            //the parent rect is
            float parentTop = parent.transform.position.y + (1 - parent.pivot.y) * parent.rect.height * rtrans.lossyScale.y;
            float parentLeft = parent.transform.position.x - parent.pivot.x * parent.rect.width * rtrans.lossyScale.x;

            //Vector2 scale = new Vector2(rt.localScale.x * parent.localScale.x)
            //Now we can use the Parent Rect and check if both the min and max bounds
            //are within the rect
            if (b.min.y < parent.rect.yMin)  //Bottom
            {
                actualPos.y = parentTop - (parent.rect.height - b.size.y) * rtrans.lossyScale.y;
            }
            else if (b.max.y > parent.rect.yMax)  //Top
            {
                actualPos.y = parentTop;
            }

            if (b.min.x < parent.rect.xMin) //Right
            {
                actualPos.x = parentLeft;
            }
            else if (b.max.x > parent.rect.xMax) //Left
            {
                //NOTE: Using lossy scale here. It works perfectly UNLESS you have a parent
                //transform with a scale and a child that is arbitrarily rotated
                actualPos.x = parentLeft + (parent.rect.width - b.size.x) * rtrans.lossyScale.x;
            }
            rtrans.position = actualPos;
        }

        //public static Vector2 ActualTopLeft(this RectTransform rt)
        //{
        //    if (rt.pivot == Vector2.zero) return new Vector2(rt.transform.position.x, rt.transform.position.y);
        //    return new Vector2(rt.transform.position.x -rt.pivot.x * rt.rect.width * rt.lossyScale.x,
        //        rt.transform.position.x - rt.pivot.x * rt.rect.width * rt.lossyScale.x
        //        );
        //}

        public static float WorldRight(this RectTransform rt)
        {
            return rt.anchoredPosition.x + rt.sizeDelta.x / 2f;
        }

        public static float WorldLeft(this RectTransform rt)
        {
            return rt.anchoredPosition.x - rt.sizeDelta.x / 2f;
        }

        public static float WorldTop(this RectTransform rt)
        {
            return rt.anchoredPosition.y + rt.sizeDelta.y / 2f;
        }

        public static float WorldBottom(this RectTransform rt)
        {
            return rt.anchoredPosition.y - rt.sizeDelta.y / 2f;
        }

        public static Rect CalculateWorldBounds(this RectTransform rt, Vector3[] corners)
        {
            if (corners == null || corners.Length < 4) return new Rect();
            rt.GetWorldCorners(corners);
            var size = new Vector2(rt.localScale.x * rt.sizeDelta.x, rt.localScale.y * rt.rect.size.y);
            return new Rect(corners[0], size);
        }

        public static Canvas RootCanvas(this RectTransform rt)
        {
            var c = rt.GetComponentInParent<Canvas>();
            while(c.transform.parent != null)
            {
                var g = c.GetComponentInParent<Canvas>();
                if (g != null) c = g;
                else break;
            }
            return c;
        }

        public static Canvas ParentCanvas(this RectTransform rt) => rt.GetComponentInParent<Canvas>();
    }
}