using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;


namespace SceneChange
{
    public class CrossfadeController : MonoBehaviour
    {

        [SerializeField] Animator animator;

        void Start()
        {
            SceneChangeController.Instance.OnSceneChange += RunAnimation;
        }

        //private void OnDisable()
        //{
        //    SceneChangeController.Instance.OnPreSceneChange -= RunAnimation;
        //}

        void RunAnimation()
        {
           // StartCoroutine(UpdateAnimation());
        }

        //IEnumerator UpdateAnimation()
        //{
        //    while (SceneChangeController.Instance.CurrentlyChangingScene)
        //    {
        //        bool intTransition = SceneChangeController.Instance.LoadingScene && !SceneChangeController.Instance.TransitionAnimationDone;
        //        Debug.Log(intTransition);

        //        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Null") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
        //        {
        //            SceneChangeController.Instance.SetTransitionAnimationDone();
        //        }

        //        animator.SetBool("InTransition", intTransition);

        //        yield return null;
        //    }
        //}

    }
}