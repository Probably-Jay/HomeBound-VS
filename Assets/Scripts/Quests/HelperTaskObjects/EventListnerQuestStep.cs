using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Reflection;


namespace Quests
{
    public class EventListnerQuestStep : SimpleQuestStep
    {
        private const string onInvokeCompletedString = nameof(OnInvokedCompleted);
        private const string onInvokeUnCompletedString = nameof(OnInvokedUnCompleted);

       

        [Header("Complete event")]
        [SerializeField] GameObject completeTarget;
        [SerializeField] string completeComponentTypeName;
        [SerializeField] string completeEventName;

        [Header("Unomplete event")]
        [SerializeField] bool hasUncompleteEvent = false;
        [SerializeField] GameObject unCompleteTarget;
        [SerializeField] string unCompleteComponentTypeName;
        [SerializeField] string unCompleteEventName;
        private void Awake()
        {
            BindToCompleteEvent();
            if(hasUncompleteEvent)
                BindToUncompleteEvent();
        }

        private void BindToCompleteEvent()
        {
            BindToEvent(completeTarget, completeComponentTypeName, completeEventName, onInvokeCompletedString);
        }

        private void BindToUncompleteEvent()
        {
            BindToEvent(unCompleteTarget, unCompleteComponentTypeName, unCompleteEventName, onInvokeUnCompletedString);
        }

        private void BindToEvent(GameObject _target, string _componentTypeName, string _eventName, string _onInvokedString)
        {
            try
            {
                Component component = _target.GetComponent(_componentTypeName);
                Type componentType = component.GetType();
                EventInfo eventField = componentType.GetEvent(_eventName);
                Type eventFeildDelegateType = eventField.EventHandlerType;
                MethodInfo eventFeildAddMethod = eventField.GetAddMethod();

                MethodInfo localOnInvokeMethodInfo =
                    typeof(EventListnerQuestStep).GetMethod(_onInvokedString,
                    BindingFlags.NonPublic | BindingFlags.Instance);

                Delegate localDelegateToSunscribe = Delegate.CreateDelegate(eventFeildDelegateType, this, localOnInvokeMethodInfo, true);

                object[] boxedLocalDelegateToSunscribe = { localDelegateToSunscribe };
                eventFeildAddMethod.Invoke(component, boxedLocalDelegateToSunscribe);
            }
            catch 
            {
                Debug.LogError($"Attempt to bind to event failed, on \"{gameObject.name}\", targeting \"{_target}\", looking for component \"{_componentTypeName}\" and event field \"{_eventName}\"",gameObject);
                throw;
            }
           
        }

        void OnInvokedCompleted()
        {
            CompleteStep();
        }
        void OnInvokedUnCompleted()
        {
            UnCompleteStep();
        }
    }
}