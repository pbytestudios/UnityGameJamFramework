using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pixelbyte
{
    public class DestroyWhenPlatform : MonoBehaviour
    {
        public enum PlatformAction
        {
            DestroyWhenFound,
            DestroyUnlessFound
        };

        public RuntimePlatform[] platforms;
        public PlatformAction action;

        private void Start()
        {
            for (int i = 0; i < platforms.Length; i++)
            {
                if (Application.platform == platforms[i])
                {
                    if (action == PlatformAction.DestroyWhenFound)
                    {
                        Destroy(gameObject);
                        break;
                    }
                    else if (action == PlatformAction.DestroyUnlessFound)
                        return;
                }
            }

            Destroy(gameObject);

        }
    } 
}
