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
        [SerializeField] GameObject target;
        [SerializeField] string componentTypeName;
        [SerializeField] string eventName;
        private void Awake()
        {
            BindToEvent();
        }

        private void BindToEvent()
        {
            try
            {
                Component component = target.GetComponent(componentTypeName);
                Type componentType = component.GetType();
                EventInfo eventField = componentType.GetEvent(eventName);
                Type eventFeildDelegateType = eventField.EventHandlerType;
                MethodInfo eventFeildAddMethod = eventField.GetAddMethod();

                MethodInfo localOnInvokeMethodInfo =
                    typeof(EventListnerQuestStep).GetMethod(nameof(OnInvoked),
                    BindingFlags.NonPublic | BindingFlags.Instance);

                Delegate localDelegateToSunscribe = Delegate.CreateDelegate(eventFeildDelegateType, this, localOnInvokeMethodInfo, true);

                object[] boxedLocalDelegateToSunscribe = { localDelegateToSunscribe };
                eventFeildAddMethod.Invoke(component, boxedLocalDelegateToSunscribe);
            }
            catch 
            {
                Debug.LogError($"Attempt to bind to event failed, on \"{gameObject.name}\", targeting \"{target}\", looking for component \"{componentTypeName}\" and event field \"{eventName}\"",gameObject);
                throw;
            }
           
        }

        void OnInvoked()
        {
            CompleteStep();
        }
    }
}