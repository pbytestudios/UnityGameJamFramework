using System;
using UnityEngine;

[Serializable]

[CreateAssetMenu(fileName = "CameraOrbitSettings", menuName = "CameraOrbitSettings",  order = 1)]
public class CameraOrbitSettings : ScriptableObject
{
    [SerializeField] Vector2 speed;
    [SerializeField] Vector2 min;
    [SerializeField] Vector2 max;

    public Vector2 Speed => speed;
    public Vector2 Min => min;
    public Vector2 Max => max;

}
