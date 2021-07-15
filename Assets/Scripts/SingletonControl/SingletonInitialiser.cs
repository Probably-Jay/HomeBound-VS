using UnityEngine;
using System.Collections.Generic;
using System;

namespace SingletonManagement
{
    /// <summary>
    /// Class responsible for ensuring there is only ever exactly one of each singleton in each scene
    /// </summary>
    public class SingletonInitialiser : MonoBehaviour
    {
        [SerializeField] bool instantiateSingletonsInThisScene = true;
      
        [Header("Prefabs of singleton objects")]

        [Header("Events")]
        [SerializeField] GameObject eventsManager;
        [SerializeField] bool initaliseEventsManager;         
        
        [Header("Scene Change")]
        [SerializeField] GameObject sceneChangeController;
        [SerializeField] bool initalisesceneChangeController; 
        
        [Header("Rythm")]
        [SerializeField] GameObject rythmEngine;
        [SerializeField] bool initaliseRythmEngine;

        [Header("Context")]
        [SerializeField] GameObject contextController;
        [SerializeField] bool initaliseContextController;


        private void Awake()
        {
            CreateSingletons();
        }

        private void CreateSingletons()
        {
            if (!instantiateSingletonsInThisScene) return;

            // create all singletons

            SetUpSingleton<EventsManager>(initaliseEventsManager, eventsManager);

            SetUpSingleton<SceneChange.SceneChangeController>(initalisesceneChangeController, sceneChangeController);

            SetUpSingleton<Rythm.RythmEngine>(initaliseRythmEngine, rythmEngine);

            SetUpSingleton<Game.GameContextController>(initaliseContextController, contextController);

        }

        private void SetUpSingleton<T>(bool initialise, GameObject prefab) where T : Singleton<T>
        {
            if (initialise)
                CreateSingleon<T>(prefab);
            else
                DisableSingletonIfExists<T>();
        }


        private void CreateSingleon<T>(GameObject singletonPrefab) where T : Singleton<T>
        {
            if (Singleton<T>.InstanceExists)
            {
                Singleton<T>.Instance.gameObject.SetActive(true);
                return;
            }

            if (singletonPrefab == null)
            {
                Singleton<T>.WarnInstanceDoesNotExist();
                return;
            }

            var singletonObject = Instantiate(singletonPrefab);
            singletonObject.GetComponent<T>().Initialise();
        }

        private void DisableSingletonIfExists<T>() where T : Singleton<T>
        {
            if (!Singleton<T>.InstanceExists)
            {
                return;
            }
            Singleton<T>.Instance.gameObject.SetActive(false); // this may not work, or do nothing
        }

    }
}