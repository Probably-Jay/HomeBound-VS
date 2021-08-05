using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game
{

    public enum Context
    {
        None
        ,PreTutorial
        ,Explore
        ,Dialogue
        ,Rythm
    }


    public class GameContextController : SingletonManagement.Singleton<GameContextController>
    {
        [SerializeField] Context defaultFirstContext;

        new public static GameContextController Instance => SingletonManagement.Singleton<GameContextController>.Instance;

        public override void Initialise()
        {
            base.InitSingleton();
        }

        private void Start()
        {
            PushContext(defaultFirstContext, Context.None);
        }

        private readonly Stack<Context> contextStack = new Stack<Context>();


        public Context CurrentContext => contextStack.Peek();
        public bool HasPreviousContext => contextStack.Count > 1;

        /// <summary>
        /// Subscribe to this event to be notified of an context change. Passes first the *current context* followed by the *previous context* (the context just left)
        /// </summary>
        public event Action<Context, Context> OnContextChange;
        private void InvokeContextChange(Context previousContext)
        {

            if (previousContext == CurrentContext)
            {
                Debug.LogWarning("Switching to context same as current context");
                return;
            }
            OnContextChange?.Invoke(CurrentContext, previousContext);
        }

        /// <summary>
        /// Add a new context to the context stact
        /// </summary>
        public void PushContext(Context context)
        {
            var prevContext = CurrentContext;
            PushContext(context, prevContext);
        }

       
        /// <summary>
        /// Change the current context into a new context without affecting the stack
        /// </summary>
        public void MutateContext(Context newContext)
        {
            var prevContext = CurrentContext;
            contextStack.Pop();
            PushContext(newContext, prevContext);

        }

        /// <summary>
        /// Return to the context previously on top of the stack. If there is no previous context this function will have no effect
        /// </summary>
        public void ReturnToPreviousContext()
        {
            if (!HasPreviousContext)
            {
                Debug.LogError("No previous context to return to");
                return;
            }
            var prevContext = CurrentContext;
            contextStack.Pop();
            InvokeContextChange(prevContext);
        }

        /// <summary>
        /// Resets the context stack to the provided state. *Warning* this action is destructive
        /// </summary>
        public void ResetContextStackTo(Context newBottomContext)
        {
            var prevContext = CurrentContext;
            contextStack.Clear();
            PushContext(newBottomContext, prevContext);

        }

        private void PushContext(Context context, Context prevContext)
        {
            contextStack.Push(context);
            InvokeContextChange(prevContext);
        }
    }
}