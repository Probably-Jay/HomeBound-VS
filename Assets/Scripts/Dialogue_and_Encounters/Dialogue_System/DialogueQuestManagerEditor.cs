using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;
namespace HomeboundEditor
{

#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditorInternal;

    // Tells Unity to use this Editor class with the WaveManager script component.

    [CustomEditor(typeof(DialogueQuestManager))]
    public class DialogueQuestEditor : Editor
    {

        SerializedProperty quests;

        ReorderableList list;

        private void OnEnable()
        {
            quests = serializedObject.FindProperty(nameof(DialogueQuestManager.displayQuests));

            list = new ReorderableList(serializedObject, quests, true, true, true, true);


            list.drawElementCallback = DrawListItems;
            list.drawHeaderCallback = DrawHeader;
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.DrawDefaultInspector();

            list.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }


        void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index); // The element in the list



            EditorGUI.LabelField(new Rect(rect.x, rect.y, 50, EditorGUIUtility.singleLineHeight), "Quest ID:");


            EditorGUI.PropertyField(
                new Rect(rect.x + 60, rect.y, 50, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative(nameof(Dialogue.Internal.StringQuestDict.id)),
                GUIContent.none
            );


            EditorGUI.LabelField(new Rect(rect.x + 120, rect.y, 40, EditorGUIUtility.singleLineHeight), "Quest:");

            EditorGUI.PropertyField(
                new Rect(rect.x + 160, rect.y, 150, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative(nameof(Dialogue.Internal.StringQuestDict.quest)),
                GUIContent.none
            );

        }

        void DrawHeader(Rect rect)
        {
            string name = "Tasks and task-IDs";
            EditorGUI.LabelField(rect, name);
        }

    }
#endif
}