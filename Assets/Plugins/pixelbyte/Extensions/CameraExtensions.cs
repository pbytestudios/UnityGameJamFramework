using UnityEngine;

//
// 2020 Pixelbyte Studios LLC
//
namespace Pixelbyte.Extensions
{
    public static class CameraExtensions
    {
        //This sets just the X viewport position and width
        //
        public static void SetViewportX(this Camera cam, float xOffset, float w)
        {
            //x must be 0 <= x <= 1
            cam.rect = new Rect(xOffset, 0, w, 1);
        }

        /// <summary>
        /// Returns the extents (half the height and half the width) in world space units
        /// Note: The camera should be in orthographic mode for this to work right
        /// </summary>
        /// <param name="cam"></param>
        /// <returns></returns>
        public static Vector2 OrthoExtents(this Camera cam) => new Vector2(cam.aspect * cam.orthographicSize, cam.orthographicSize);

        public static float OrthoLeft(this Camera cam) => cam.transform.position.x - cam.aspect * cam.orthographicSize;
        public static float OrthoRight(this Camera cam) => cam.transform.position.x + cam.aspect * cam.orthographicSize;
        public static float OrthoTop(this Camera cam) => cam.transform.position.y + cam.orthographicSize;
        public static float OrthoBottom(this Camera cam) => cam.transform.position.y - cam.orthographicSize;

        //leftpos + cam.aspect * cam.orthographicSize = pos.x
        public static void SetOrthoLeft(this Camera cam, float left) => new Vector3(left + cam.aspect * cam.orthographicSize, cam.transform.position.y, cam.transform.position.z);
        public static void SetOrthoRight(this Camera cam, float right) => new Vector3(right - cam.aspect * cam.orthographicSize, cam.transform.position.y, cam.transform.position.z);
        public static void SetOrthoTop(this Camera cam, float top) => new Vector3(cam.transform.position.x, top - cam.aspect * cam.orthographicSize, cam.transform.position.z);
        public static void SetOrthoBottom(this Camera cam, float bottom) => new Vector3(cam.transform.position.x, bottom + cam.aspect * cam.orthographicSize, cam.transform.position.z);

        /// <summary>
        /// Returns a bounds object of the camera's viewport in world space units
        /// Note: The camera should be in orthographic mode for this to work right
        /// </summary>
        /// <param name="cam"></param>
        /// <returns></returns>
        public static Bounds OrthoBounds(this Camera cam) => new Bounds(cam.transform.position, cam.OrthoExtents() * 2);

        /// <summary>
        /// Returns the required ortho size to fit both dimensions given by size
        /// camera heigh is simply orthosize * 2 so we will
        /// return whichever of these is larger
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="size">The dimensions to fit</param>
        /// <returns>The ortho size that fits the given size</returns>
        public static float OrthoSizeToFit(this Camera cam, Vector2 size) => Mathf.Max(size.x / (cam.aspect * 2), size.y / 2);

        /// <summary>
        /// Returns a Vector2 containing the camera's frustrum width and height
        /// (viewport size) at the given distance from the camera
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static Vector2 FrustrumSizeAtDistance(this Camera cam, float distance)
        {
            float frustrumHeight = 2f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);

            //frustrumWidth = frustrumHeight * camera.aspect
            return new Vector2(frustrumHeight * cam.aspect, frustrumHeight);
        }

        /// <summary>
        /// Returns the required distance from the camera that will produce the 
        /// desired frustrum height (camera viewport height)
        /// </summary>
        /// <param name="cam">The Camera</param>
        /// <param name="desiredFrustrumWidth">The desired frustrum height</param>
        /// <returns></returns>
        public static float DistanceForHeight(this Camera cam, float desiredFrustrumHeight) => desiredFrustrumHeight * 0.5f / Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);

        /// <summary>
        /// Returns the required distance from the camera that will produce the 
        /// desired frustrum width (camera viewport width)
        /// </summary>
        /// <param name="cam">The Camera</param>
        /// <param name="desiredFrustrumWidth">The desired frustrum width</param>
        /// <returns></returns>
        public static float DistanceForWidth(this Camera cam, float desiredFrustrumWidth) => (desiredFrustrumWidth / cam.aspect) * 0.5f / Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);

        /// <summary>
        /// Returns the required distance away from the camera in order for the 
        /// given desired frustrum size to be fulfilled. Note, this will fit the 
        /// larger of the two sizes
        /// </summary>
        /// <param name="cam">The Camera</param>
        /// <param name="frustrum">The desired frustrum size width and height</param>
        /// <returns></returns>
        public static float DistanceForFrustrum(this Camera cam, Vector2 frustrum) => Mathf.Max(cam.DistanceForHeight(frustrum.y), cam.DistanceForWidth(frustrum.x));
    }
}