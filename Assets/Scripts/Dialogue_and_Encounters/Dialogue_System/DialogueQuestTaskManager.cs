using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Dialogue
{

#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditorInternal;

   // Tells Unity to use this Editor class with the WaveManager script component.

    [CustomEditor(typeof(DialogueQuestTaskManager))]
    public class DialogueQuestTasksEditor : Editor
    {

        SerializedProperty tasks;

        ReorderableList list;

        private void OnEnable()
        {
            tasks = serializedObject.FindProperty(nameof(DialogueQuestTaskManager.displayTasks));

            list = new ReorderableList(serializedObject, tasks, true, true, true, true);


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


           

            EditorGUI.LabelField(new Rect(rect.x, rect.y, 50, EditorGUIUtility.singleLineHeight), "Task ID:");


            EditorGUI.PropertyField(
                new Rect(rect.x+60, rect.y, 50, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative(nameof(Internal.StringQuestTaskDict.id)),
                GUIContent.none
            );


            EditorGUI.LabelField(new Rect(rect.x + 120, rect.y, 40, EditorGUIUtility.singleLineHeight), "Task:");

            EditorGUI.PropertyField(
                new Rect(rect.x+160, rect.y, 150, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative(nameof(Internal.StringQuestTaskDict.task)),
                GUIContent.none
            );

        }

        //Draws the header
        void DrawHeader(Rect rect)
        {
            string name = "Tasks and task-IDs";
            EditorGUI.LabelField(rect, name);
        }
    }
#endif


    public class DialogueQuestTaskManager : MonoBehaviour
    {
        [SerializeField,HideInInspector] internal List<Internal.StringQuestTaskDict> displayTasks;
        Dictionary<string, Quests.SimpleQuestStep> tasks = new Dictionary<string, Quests.SimpleQuestStep>();

        private void Awake()
        {
            foreach (var task in displayTasks)
            {
                tasks.Add(task.id, task.task);
            }
        }
        public bool HasTask(string ID) => tasks.ContainsKey(ID);

        public void CompleteQuestTaskStep(string ID)
        {
            AssertContainsTaskID(ID);
            tasks[ID].CompleteStep();
        }

        public void UnCompleteTaskStep(string ID)
        {
            AssertContainsTaskID(ID);
            tasks[ID].UnCompleteStep();
        }

        private void AssertContainsTaskID(string ID)
        {
            if (!HasTask(ID))
            {
                throw new System.Exception($"The quest task id {ID} does not exist in {nameof(DialogueQuestTaskManager)}");
            }
        }

    }

    namespace Internal
    {
        [System.Serializable]
        public struct StringQuestTaskDict
        {
            public string id;
            public Quests.SimpleQuestStep task;
        }

    }
  
}