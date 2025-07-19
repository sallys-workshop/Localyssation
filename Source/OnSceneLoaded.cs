using System.Collections.Generic;
using System.Linq;
using Localyssation.LangAdjutable;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Localyssation
{
    internal static class OnSceneLoaded
    {
        public static void Init()
        {
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        private static void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
        {
            List<GameObject> GetRootGameObjects()
            {
                var rootGameObjs = new List<GameObject>();
                scene.GetRootGameObjects(rootGameObjs);
                return rootGameObjs;
            }

            switch (scene.name)
            {
                case "00_bootStrapper":
                    var rootGameObjs = GetRootGameObjects();
                    var obj_Canvas_loading = rootGameObjs.First(x => x.name == "Canvas_loading");
                    if (obj_Canvas_loading)
                    {
                        foreach (var text in obj_Canvas_loading.GetComponentsInChildren<UnityEngine.UI.Text>())
                        {
                            if (text.text == "Loading...")
                            {
                                LangAdjustables.RegisterText(text, LangAdjustables.GetStringFunc("GAME_LOADING", text.text));
                                text.alignment = TextAnchor.MiddleRight;
                            }
                        }
                    }
                    break;
                case "01_rootScene":
                    break;
            }
        }
    }
}