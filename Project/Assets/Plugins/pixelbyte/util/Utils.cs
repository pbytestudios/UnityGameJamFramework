using UnityEngine;
using System.Reflection;
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Pixelbyte
{
    /// <summary>
    /// Aspect ratio enum for screen sizes
    /// </summary>
    public enum ARatio
    {
        _5x4,
        _4x3,
        _3x2,
        _16x10,
        _5x3,
        _16x9,  //1.77
        unknown,

    };
    public static class Utils
    {
        /// <summary>
        /// Gets the current screen Aspect Ratio
        /// </summary>
        public static float AspectRatio { get { return (float)System.Math.Round(Screen.width / (float)Screen.height, 2); } }

        /// <summary>
        /// Returns The aspect ratio of the screen as an enum
        /// </summary>
        /// <returns></returns>
        public static ARatio AspectRatioEnum
        {
            get
            {
                float ratio = AspectRatio;

                if (ratio < 1.25f - 0.03f)
                    return ARatio.unknown;
                else if (1.25f - 0.03f <= ratio && ratio <= 1.25f + 0.03f)
                    return ARatio._5x4;
                else if (1.33 - 0.03f <= ratio && ratio <= 1.33 + 0.03f)
                    return ARatio._4x3;
                else if (1.5f - 0.03f <= ratio && ratio <= 1.5f + 0.03f)
                    return ARatio._3x2;
                else if (1.6f - 0.03f <= ratio && ratio <= 1.6f + 0.03f)
                    return ARatio._16x10;
                else if (1.67 - 0.03f <= ratio && ratio <= 1.67f + 0.03f)
                    return ARatio._5x3;
                else if (1.78 - 0.03f <= ratio && ratio <= 1.78 + 0.03f)
                    return ARatio._16x9;
                else
                    return ARatio.unknown;
            }
        }

        public static void SetTextureColor(Texture2D texture, Color color)
        {
            Color[] colors = new Color[texture.width * texture.height];

            for (int i = 0; i < colors.Length; i++)
                colors[i] = color;

            texture.SetPixels(colors);
            texture.Apply();
        }

        /// <summary>
        /// This gives me access to the system clipboard (tested on Windows)
        /// </summary>
        public static string Clipboard
        {
            get { return GUIUtility.systemCopyBuffer; }
            set { GUIUtility.systemCopyBuffer = value; }
        }

        /// <summary>
        /// This changes the vertex colors of a mesh to the specified color
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="newColor"></param>
        public static void ChangeColor(Mesh mesh, Color newColor)
        {
            Vector3[] vertices = mesh.vertices;
            Color32[] colors = new Color32[vertices.Length];
            int i = 0;
            while (i < vertices.Length)
            {
                colors[i] = newColor;
                i++;
            }
            mesh.colors32 = colors;
        }

        public static void ChangeColor(Mesh m, int subMesh, Color newColor)
        {
            if (subMesh >= m.subMeshCount)
            {
                Dbg.Warn("Submesh index out of range!");
                return;
            }

            int[] smIndices = m.GetIndices(subMesh);
            Color32[] colors = new Color32[m.colors32.Length];
            Array.Copy(m.colors32, colors, colors.Length);

            //Only set the vertices for this submesh
            for (int i = 0; i < smIndices.Length; i++)
            {
                colors[smIndices[i]] = newColor;
            }
            m.colors32 = colors;
        }

        public static string GenerateMD5(string input)
        {
#if !UNITY_WINRT
            var md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
#else
        return WinStore.Crypt.ComputeHash(input, "MD5");
#endif
        }

        /// <summary>
        /// Looks for a GameObject by the given name.
        /// If it finds one it returns it, otherwise it makes
        /// one and returns it.
        /// </summary>
        /// <param name="name"></param>
        public static GameObject FindOrAddGameObject(string name)
        {
            var gob = GameObject.Find(name);
            if (gob == null)
                gob = new GameObject(name);
            return gob;
        }

        public static Type GetClassType(string rootName, string className)
        {
            Type t = null;
            var assembly = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith(rootName)).FirstOrDefault();
            if (assembly != null) t = assembly.GetType(className);
            return t;
        }

        public static Assembly GetAssembly(string rootName)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith(rootName)).FirstOrDefault();
            return assembly;
        }

        public static List<Type> GetDerivedTypes<T>(Assembly asm)
        {
            if (asm == null) return new List<Type>();

            var types = asm.GetExportedTypes().Where
                (t => typeof(T).IsAssignableFrom(t) && t != typeof(T)).Select(t => t).ToList();
            return types;
        }

        /// <summary>
        /// Retunrs a float with the given number of decimal places max
        /// </summary>
        /// <param name="input"></param>
        /// <param name="decimalPlaces"></param>
        /// <returns></returns>
        public static float ChopDecimals(float input, int decimalPlaces)
        {
            return (int)(input * Mathf.Pow(10, decimalPlaces)) / 100.0f;
        }

        /// <summary>
        /// This method creates a delegate of the given type that calls the given MethodInfo on
        /// the given target
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="method"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        internal static M CreateDelegate<M>(MethodInfo method, System.Object target) where M : class
        {
            var del = Delegate.CreateDelegate(typeof(M), target, method) as M;
            if (del == null)
            {
                throw new Exception("Unable to create delegate for method: " + method.Name);
            }
            return del;
        }

        public static float GetGameVersion()
        {
            if (float.TryParse(Application.version, out float ver))
                return ver;
            else
                return 0;
        }

        public static T FindTypeWithName<T>(string gameObjectName, bool includeInactive) where T : Component
        {
            var objs = UnityEngine.Object.FindObjectsOfType<T>(includeInactive);
            if (objs == null)
            {
                Dbg.Warn($"Unable to find Gameobject of type '{typeof(T).Name}' with name '{gameObjectName}'.");
                return null;
            }

            //Trim the name so we don't have any spaces!
            gameObjectName = gameObjectName.Trim();

            //We don't care about beginning or end spaces when matching!
            foreach (var item in objs)
            {
                if (item.name.Trim() == gameObjectName)
                    return item;
            }
            return null;
        }
    }
}
