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
        //[SerializeField] GameObject eventsManager;
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

            if(initaliseRythmEngine)
                CreateSingleon<Rythm.RythmEngine>(rythmEngine);

            if (initaliseContextController)
                CreateSingleon<Game.GameContextController>(contextController);
        }

        private void CreateSingleon<T>(GameObject singletonPrefab) where T : Singleton<T>
        {
            if (Singleton<T>.InstanceExists)
            {
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
 
    }
}