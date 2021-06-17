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
        [Header("Prefabs of singleton objects")]
        [SerializeField] bool instantiateSingletonsInThisScene = true;
      
        //[SerializeField] GameObject eventsManager;
        [SerializeField] GameObject rythmEngine;
        [SerializeField] bool intialiseRythmEngine;

        private void Awake()
        {
            CreateSingletons();
        }

        private void CreateSingletons()
        {
            if (!instantiateSingletonsInThisScene) return;

            // create all singletons
            if(intialiseRythmEngine)
                CreateSingleon<Rythm.RythmEngine>(rythmEngine);
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