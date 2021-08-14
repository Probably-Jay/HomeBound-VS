using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using SingletonManagement;
using UnityEngine.UIElements;
namespace SceneChange
{
    /// <summary>
    /// Scenes in the game
    /// </summary>
    [System.Serializable]
    public enum Scenes
    {
        MainMenu
        // ,Settings

        , Game
        , Credits
    }
    /// <summary>
    /// Class which controls the switching of scenes
    /// </summary>
    public class SceneChangeController : Singleton<SceneChangeController>
    {
        

        [SerializeField] int MainMenuIndex;
        //   [SerializeField] int SettingsBuildIndex;

        [SerializeField] int GameBuildIndex;
        [SerializeField] int CreditsBuildIndex;

        private void CreateBuildIndexDictionary()
        {
            sceneBuildIndexesDictionary.Add(Scenes.MainMenu, MainMenuIndex);
            // sceneBuildIndexesDictionary.Add(Scenes.Settings, SettingsBuildIndex);

            sceneBuildIndexesDictionary.Add(Scenes.Game, GameBuildIndex);
            sceneBuildIndexesDictionary.Add(Scenes.Credits, CreditsBuildIndex);
        }

        [SerializeReference] GameObject LoadingScreen;
       // [SerializeField] Slider progressBar;

        public event Action OnPreSceneChange;
        public event Action OnSceneChange;
        public event Action OnPostSceneChange;

        AsyncOperation loadingScene;
        [SerializeField] bool waitForAnimationToEnd = false;
        bool transitionAnimationDone;
        public bool TransitionAnimationDone { get => waitForAnimationToEnd ? transitionAnimationDone : true; set => transitionAnimationDone = value; }


        /// <summary>
        /// If the scene is changing
        /// </summary>
        public bool CurrentlyChangingScene { get => loadingScene != null; }

        public bool LoadingScene
        {
            get
            {
                if (!CurrentlyChangingScene)
                {
                    return false;
                }
                return ScaledLoadingSceneProgress < 1;
            }
        }


        /// <summary>
        /// A float representation of the loading progress of the scene currently loading
        /// </summary>
        public float ScaledLoadingSceneProgress
        {
            get
            {
                if (!CurrentlyChangingScene) return 0;
                return Mathf.Clamp01(loadingScene.progress / 0.9f); // 0.9 represents a fully loaded scene
            }
        }

        Dictionary<Scenes, int> sceneBuildIndexesDictionary = new Dictionary<Scenes, int>();



        new public static SceneChangeController Instance { get => Singleton<SceneChangeController>.Instance; }

        private void OnEnable()
        {
            //EventsManager.BindEvent(EventsManager.EventType.CrossfadeAnimationEnd, TransitionAnimationDone);
        }

        private void OnDisable()
        {
            // EventsManager.UnbindEvent(EventsManager.EventType.CrossfadeAnimationEnd, TransitionAnimationDone);
        }

        public void SetTransitionAnimationDone() => TransitionAnimationDone = true;


      

        public override void Initialise()
        {
            InitSingleton();
            CreateBuildIndexDictionary();
        }

        /// <summary>
        /// Will asynchornously load the scene at the enumvalue provided and enter that scene
        /// </summary>
        /// <param name="scene">Enum value of the scene to load</param>
        public void ChangeScene(Scenes scene)
        {
            int sceneBuildIndex = sceneBuildIndexesDictionary[scene];
            StartCoroutine(LoadSceneAsyncCoroutine(sceneBuildIndex));
        }

        /// <summary>
        /// Prefer <see cref="ChangeScene"/> for saftey, but this overload allows passing build indexes directly
        /// </summary>
        /// <param name="scene">Build index</param>
        public void ChangeScene(int scene)
        {
            Debug.Assert(scene <= SceneManager.sceneCountInBuildSettings, $"Invalid scene index {scene} attampted to load");
            StartCoroutine(LoadSceneAsyncCoroutine(scene));
        }


        IEnumerator LoadSceneAsyncCoroutine(int buildInxed)
        {
            if (CurrentlyChangingScene) yield break;
            OnPreSceneChange?.Invoke();
            BeginLoad(buildInxed);
            while (ScaledLoadingSceneProgress < 1)
            {
                //progressBar.value = ScaledLoadingSceneProgress;
                yield return null; // wait for load to end
            }
           // progressBar.value = ScaledLoadingSceneProgress;
            while (!TransitionAnimationDone)
            {
                yield return null; // wait for transition to end
            }
            EndLoad();
            while (!TransitionAnimationDone) // load will always need to be done to reach here
            {
                yield return null; // wait for enter scene transition to end
            }
            EnterScene();
        }



        private void BeginLoad(int buildInxed)
        {
            if (EventsManager.InstanceExists)
                EventsManager.CleanEvents();

            LoadingScreen.SetActive(true);

            // EventsManager.InvokeEvent(EventsManager.EventType.BeginSceneLoad);
            loadingScene = SceneManager.LoadSceneAsync(buildInxed);
            loadingScene.allowSceneActivation = false; // wait to finish scene load until we tell it to
            TransitionAnimationDone = false;
            OnSceneChange?.Invoke();
        }



        private void EndLoad()
        {
            //  EventsManager.InvokeEvent(EventsManager.EventType.SceneLoadComplete);
            loadingScene.allowSceneActivation = true; // wait to finish scene load until we tell it to
            TransitionAnimationDone = false; // transition into new scene
        }

        private void EnterScene()
        {
            LoadingScreen.SetActive(false);
            //z EventsManager.InvokeEvent(EventsManager.EventType.EnterNewScene);
            loadingScene = null;
            
            OnPostSceneChange?.Invoke();

        }
    }
}