using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    /*
    #region Custom Editor 
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditorInternal;
    [CustomEditor(typeof(Conversation))]
    public class InventoryListEditor : Editor
    {
        SerializedProperty phrases;
        ReorderableList list;

        private void OnEnable()
        {
            phrases = serializedObject.FindProperty(nameof(Conversation.dialoguePhrases));

            list = new ReorderableList(
                serializedObject: serializedObject
                , elements: phrases
                , draggable: true
                , displayHeader: true
                , displayAddButton: true
                , displayRemoveButton: true
                );

            list.drawElementCallback = DrawListItems;
            list.drawHeaderCallback = DrawHeader;

        }

        void DrawListItems(Rect rect, int index, bool isActive, bool isFocused) // draws each element
        {
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index); // element in list

            element.objectReferenceValue = EditorGUI.ObjectField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight)
                , new GUIContent($"Item {index}: ")//{(element.objectReferenceValue != null ? ((DialoguePhrase)element.objectReferenceValue).ToString() : "(Empty)") }")
                , element.objectReferenceValue
                , typeof(DialoguePhrase)
                , false
            );

        }

        void DrawHeader(Rect rect) // the title of the list
        {
            EditorGUI.LabelField(rect, "List of inventory objects");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            list.DoLayoutList(); // draw the list

            serializedObject.ApplyModifiedProperties();
        }


    }
#endif
    #endregion
    */

    [CreateAssetMenu(fileName = "Conversation", menuName = "ScriptableObjects/Dialogue/Conversation", order = 1)]
    public class Conversation : ScriptableObject
    {
        public string conversationID;
        public DialogueMode initialMode;
        public readonly List<DialoguePhrase> dialoguePhrases = new List<DialoguePhrase>();
        public long context;

        public event Action<DialogueMode> OnSetDialogueMode;
        public void SetDialougeMode(DialogueMode mode) => OnSetDialogueMode?.Invoke(mode);

        public event Action<int> OnSetColour;
        public void SetColour(int colour) => OnSetColour?.Invoke(colour);

        public event Action<string> OnTriggerRythmSection;
        public void StartRyhtmSection(string id) => OnTriggerRythmSection?.Invoke(id);

        public event Action<int> OnPause;
        /// <summary>
        /// Pause for <paramref name="v"/> <c>1/16</c> notes
        /// </summary>
        public void Pause(int v) => OnPause?.Invoke(v);

        public event Action<float> OnShake;
        public void Shake(float v) => OnShake?.Invoke(v);
    }


}