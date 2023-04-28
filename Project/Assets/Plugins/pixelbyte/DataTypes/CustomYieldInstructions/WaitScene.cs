using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pixelbyte.YieldInstructions
{
    public class WaitScene : CustomYieldInstruction
    {
        bool loading;
        int currentIndex;
        string[] scenes;
        AsyncOperation asyncOp;

        public string CurrentScene { get { if (asyncOp == null || scenes == null || currentIndex >= scenes.Length) return ""; return scenes[currentIndex]; } }

        public WaitScene() { }

        public override bool keepWaiting
        {
            get
            {
                if (asyncOp == null) return false;
                else if (asyncOp.isDone)
                {
                    //Cycle through any remaining scenes and wait on them individually
                    if (loading)
                        return StartLoadingNextScene();
                    else
                        return StartUnloadingNextScene();
                }
                //Keep waiting
                return true;
            }
        }


        bool StartLoadingNextScene()
        {
            currentIndex++;
            if (scenes == null || scenes.Length == 0 || currentIndex >= scenes.Length) { asyncOp = null; return false; }

            var scn = SceneManager.GetSceneByName(scenes[currentIndex]);
            if(scn.isLoaded)
            {
                Dbg.Warn($"Scene {scenes[currentIndex]} is already loaded. Skipping");
                return StartLoadingNextScene();
            }
            //Only loading one scene? Make the scene mode single, otherwise make it additive
            asyncOp = SceneManager.LoadSceneAsync(scenes[currentIndex], LoadSceneMode.Additive);
            return true;
        }

        private bool StartUnloadingNextScene()
        {
            currentIndex++;
            if (scenes == null || scenes.Length == 0 || currentIndex >= scenes.Length) { asyncOp = null; return false; }
            asyncOp = SceneManager.UnloadSceneAsync(scenes[currentIndex]);
            return true;
        }

        /// <summary>
        /// Waits for a single scene to be loaded asynchronously
        /// </summary>
        /// <param name="sceneToLoad"></param>
        /// <param name="sceneLoadMode"></param>
        /// <returns></returns>
        public WaitScene WaitLoadSingle(string sceneToLoad, LoadSceneMode sceneLoadMode)
        {
            loading = true;
            currentIndex = -1;
            scenes = null;
            asyncOp = SceneManager.LoadSceneAsync(sceneToLoad, sceneLoadMode);
            return this;
        }

        /// <summary>
        /// Loads ALL given scenes asyncronously and waits until all of them are done
        /// The scenes will be loaded in ORDER, the next scene will NOT be loaded until
        /// the first one is FINISHED
        /// </summary>
        /// <param name="scenesToLoad"></param>
        /// <returns></returns>
        public WaitScene WaitLoad(params string[] scenesToLoad)
        {
            loading = true;
            currentIndex = -1;
            scenes = scenesToLoad;
            StartLoadingNextScene();
            return this;
        }

        /// <summary>
        /// Unloads all the given scenes asynchronously AND in ORDER.
        /// The 1st
        /// scene listed will be unloaded first, then the next.
        /// </summary>
        /// <param name="scenesToUnload"></param>
        /// <returns></returns>
        public WaitScene WaitUnload(params string[] scenesToUnload)
        {
            loading = false;
            currentIndex = -1;
            scenes = scenesToUnload;
            StartUnloadingNextScene();
            return this;
        }
    }
}
