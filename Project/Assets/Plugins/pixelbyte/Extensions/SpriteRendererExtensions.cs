using UnityEngine;

//
// 2019 Pixelbyte Studios LLC
//
namespace Pixelbyte.Extensions
{
    public static class SpriteExtensions
    {
        //The following functions give the height/width of a sprite
        //renderer using a LOCAL bounding box
        //That means the height/width will ALWAYS be the
        //same even if rotated or scaled
        //If you want a bounding box that resizes to 
        //include rotation of the sprite, use:
        //rend.bounds
        public static Vector2 LSize(this SpriteRenderer rend) => rend.sprite.bounds.extents * 2;
        public static Vector2 LExtents(this SpriteRenderer rend) => rend.sprite.bounds.extents;
        public static Vector2 Size(this SpriteRenderer rend) => rend.bounds.extents * 2;
        public static Vector2 Extents(this SpriteRenderer rend) => rend.bounds.extents;

        public static float Top (this SpriteRenderer rend) => rend.transform.position.y + rend.bounds.extents.y; 
        public static float Bottom (this SpriteRenderer rend) => rend.transform.position.y - rend.bounds.extents.y; 
        public static float Left (this SpriteRenderer rend) => rend.transform.position.x - rend.bounds.extents.x; 
        public static float Right(this SpriteRenderer rend) { return rend.transform.position.x + rend.bounds.extents.x; }

        //These methods use a bounding box that resizes when the sprite
        //is rotated or scaled
        public static float Width(this SpriteRenderer rend) => rend.bounds.extents.x * 2;


        public static float Height(this SpriteRenderer rend) => rend.bounds.extents.y * 2;

        public static float HalfWidth(this SpriteRenderer rend) => rend.bounds.extents.x;

        public static float HalfHeight(this SpriteRenderer rend) => rend.bounds.extents.y;

        public static void Alpha(this SpriteRenderer rend, float val)
        {
            var c = rend.color;
            c.a = val;
            rend.color = c;
        }
    }
}