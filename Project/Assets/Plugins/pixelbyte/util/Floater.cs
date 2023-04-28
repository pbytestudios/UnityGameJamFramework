using UnityEngine;

namespace Pixelbyte
{
    public class Floater : MonoBehaviour
    {
        public Vector3 positionChange = new Vector3(1, 0, 0);
        public Vector3 frequency = new Vector3(1, 0, 0);

        public bool randomStart = false;
        //public bool useYStartPosOnly = false;

        Vector3 timer;
        Vector3 endTime = Vector3.one;

        Vector3 startPos;

        // Use this for initialization

        void Awake()
        {
            startPos = transform.localPosition;

            if (frequency.x < 1)
                endTime.x /= frequency.x;
            if (frequency.y < 1)
                endTime.y /= frequency.y;
            if (frequency.z < 1)
                endTime.z /= frequency.z;

            if (randomStart)
                timer = new Vector3(Random.Range(0f, endTime.x), Random.Range(0f, endTime.y), Random.Range(0f, endTime.z));
            timer = Vector3.zero;
        }

        public void SetStart(Vector3 start) { startPos = start; }
        public void SetCurrentAsStart() { startPos = transform.position; }

        // Update is called once per frame
        void Update()
        {
            if (timer.x > endTime.x)
                timer.x = 0;
            if (timer.y > endTime.y)
                timer.y = 0;
            if (timer.z > endTime.z)
                timer.z = 0;

            var p = transform.localPosition;
            p.x = startPos.x + (positionChange.x / 2f * Mathf.Sin(timer.x * 2 * Mathf.PI * frequency.x));
            p.y = startPos.y + (positionChange.y / 2f * Mathf.Sin(timer.y * 2 * Mathf.PI * frequency.y));
            p.z = startPos.z + (positionChange.z / 2f * Mathf.Sin(timer.z * 2 * Mathf.PI * frequency.z));
            transform.localPosition = p;
            //transform.position = startPos + Vector3.right * (positionChange * Mathf.Cos(timer * 2 * Mathf.PI * frequency));

            timer = timer + new Vector3(Time.deltaTime, Time.deltaTime, Time.deltaTime);
            //print(positionChange * Mathf.Cos(timer * Mathf.PI * frequency));
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            var restPos = transform.localPosition;
            if (Application.isPlaying)
                restPos = startPos;

            var left = restPos - Vector3.right * positionChange.x / 2f;
            var right = restPos + Vector3.right * positionChange.x / 2f;

            var lower = restPos - Vector3.up * positionChange.y / 2f;
            var upper = restPos + Vector3.up * positionChange.y / 2f;

            var front = restPos + Vector3.forward * positionChange.z / 2f;
            var back = restPos - Vector3.forward * positionChange.z / 2f;

            if (!Mathf.Approximately((upper - lower).sqrMagnitude, 0))
            {
                UnityEditor.Handles.color = Color.green;
                UnityEditor.Handles.DrawDottedLine(lower, upper, positionChange.y * 0.15f);
                UnityEditor.Handles.DotHandleCap(0, upper, Quaternion.identity, 0.025f, EventType.Repaint);
                UnityEditor.Handles.DotHandleCap(0, lower, Quaternion.identity, 0.025f, EventType.Repaint);
            }


            if (!Mathf.Approximately((right - left).sqrMagnitude, 0))
            {
                UnityEditor.Handles.color = Color.red;
                UnityEditor.Handles.DrawDottedLine(left, right, positionChange.x * 0.15f);
                UnityEditor.Handles.DotHandleCap(0, right, Quaternion.identity, 0.025f, EventType.Repaint);
                UnityEditor.Handles.DotHandleCap(0, left, Quaternion.identity, 0.025f, EventType.Repaint);
            }

            if (!Mathf.Approximately((front - back).sqrMagnitude, 0))
            {
                UnityEditor.Handles.color = Color.blue;
                UnityEditor.Handles.DrawDottedLine(front, back, positionChange.x * 0.15f);
                UnityEditor.Handles.DotHandleCap(0, right, Quaternion.identity, 0.025f, EventType.Repaint);
                UnityEditor.Handles.DotHandleCap(0, left, Quaternion.identity, 0.025f, EventType.Repaint);
            }
        }
#endif
    }
}
