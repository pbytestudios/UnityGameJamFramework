using System;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pixelbyte.UI
{
    [CreateAssetMenu(menuName = "Theme")]
    public class ThemeData : ScriptableObject
    {
        public Color primary = Color.white;
        public Color secondary = Color.black;
        public Color accent1 = new Color(0.6F, 0.6F, 0.6F, 1.0F);
        public Color accent2 = new Color(0.2F, 0.2F, 0.2F, 1.0F);
        public Color accent3 = new Color(0.1F, 0.2F, 0.1F, 1.0F);
        public Color accent4 = new Color(0.2F, 0.1F, 0.1F, 1.0F);
        public Color text = Color.white;
        public Color disabled = Color.gray;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Themer.Exists && Themer.I.Theme == this)
                Themer.I.Theme = this;
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ThemeData))]
    public class ThemeDataEditor : Editor
    {
        SerializedProperty prim;
        SerializedProperty sec;
        SerializedProperty acc1;
        SerializedProperty acc2;
        SerializedProperty acc3;
        SerializedProperty acc4;

        private void OnEnable()
        {
            prim = serializedObject.FindProperty("primary");
            sec = serializedObject.FindProperty("secondary");
            acc1 = serializedObject.FindProperty("accent1");
            acc2 = serializedObject.FindProperty("accent2");
            acc3 = serializedObject.FindProperty("accent3");
            acc4 = serializedObject.FindProperty("accent4");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Randomize Colors"))
            {
                serializedObject.Update();

                var list = new List<Color>() { prim.colorValue, sec.colorValue, acc1.colorValue, acc2.colorValue, acc3.colorValue, acc4.colorValue };
                var rndList = new List<Color>(6);
                while (list.Count > 0)
                {
                    int rndIndex = UnityEngine.Random.Range(0, list.Count);
                    rndList.Add(list[rndIndex]);
                    list.RemoveAt(rndIndex);
                }

                prim.colorValue = rndList[0];
                sec.colorValue  = rndList[1];
                acc1.colorValue = rndList[2];
                acc2.colorValue = rndList[3];
                acc3.colorValue = rndList[4];
                acc4.colorValue = rndList[5];

                serializedObject.ApplyModifiedProperties();
            }
            else if (GUILayout.Button("Cycle Colors"))
            {
                serializedObject.Update();

                var list = new List<Color>() { acc4.colorValue, prim.colorValue, sec.colorValue, acc1.colorValue, acc2.colorValue, acc3.colorValue };

                prim.colorValue = list[0];
                sec.colorValue = list[1];
                acc1.colorValue = list[2];
                acc2.colorValue = list[3];
                acc3.colorValue = list[4];
                acc4.colorValue = list[5];
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}

