using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomEditorNS
{

#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditorInternal;

    // Tells Unity to use this Editor class with the WaveManager script component.

    [CustomEditor(typeof(Quests.QuestTask))]
    public class QuestTaskEditor : Editor
    {

        SerializedProperty questSteps;

        ReorderableList list;

        private void OnEnable()
        {
            questSteps = serializedObject.FindProperty(nameof(Quests.QuestTask.taskPrerequisites));

            list = new ReorderableList(serializedObject, questSteps, true, true, true, true);


          //  list.drawElementCallback = DrawListItems;
           // list.drawHeaderCallback = DrawHeader;
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

          //  list.DoLayoutList();
            base.DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();
        }


        void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index); // The element in the list



            EditorGUI.LabelField(new Rect(rect.x, rect.y, 55, EditorGUIUtility.singleLineHeight), "Step No. ");

            //GUI.enabled = false;

            //EditorGUI.PropertyField(
            //    new Rect(rect.x + 57, rect.y, 20, EditorGUIUtility.singleLineHeight),
            //    element.FindPropertyRelative(nameof(Quests.QuestStep.stepOrder)),
            //    GUIContent.none
            //);

            //EditorGUI.IntField(
            //    new Rect(rect.x + 57, rect.y, 20, EditorGUIUtility.singleLineHeight),
            //    index
            //);
            //GUI.enabled = true;


            EditorGUI.LabelField(new Rect(rect.x+80, rect.y, 180, EditorGUIUtility.singleLineHeight), "Completable Step:");


            EditorGUI.PropertyField(
                new Rect(rect.x + 200, rect.y, 100, EditorGUIUtility.singleLineHeight),
                element,
               // element.FindPropertyRelative(nameof(Quests.QuestStep.step)),
                GUIContent.none
            );



            

        }

        void DrawHeader(Rect rect)
        {
            string name = "Task Steps";
            EditorGUI.LabelField(rect, name);
        }

    }
#endif

}