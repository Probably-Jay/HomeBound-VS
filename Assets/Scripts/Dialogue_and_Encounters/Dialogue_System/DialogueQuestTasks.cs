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

   [CustomEditor(typeof(DialogueQuestTasks))]
    public class DialogueQuestTasksEditor : Editor
    {

        // This will contain the <wave> array of the WaveManager. 
        SerializedProperty tasks;

        // The Reorderable List we will be working with 
        ReorderableList list;

        private void OnEnable()
        {
            // Get the <wave> array from WaveManager, in SerializedProperty form.
            // Note that <serializedObject> is a property of the parent Editor class.
            tasks = serializedObject.FindProperty(nameof(DialogueQuestTasks.displayTasks));

            // Set up the reorderable list       
            list = new ReorderableList(serializedObject, tasks, true, true, true, true);


            // Update the array property's representation in the inspector
            // Have the ReorderableList do its work
            // Apply any property modification
            list.drawElementCallback = DrawListItems; // Delegate to draw the elements on the list
            list.drawHeaderCallback = DrawHeader; // Skip this line if you set displayHeader to 'false' in your ReorderableList constructor.
        }

        //This is the function that makes the custom editor work
        public override void OnInspectorGUI()
        {

           // base.OnInspectorGUI();


            serializedObject.Update(); // Update the array property's representation in the inspector

            list.DoLayoutList(); // Have the ReorderableList do its work

            // We need to call this so that changes on the Inspector are saved by Unity.
            serializedObject.ApplyModifiedProperties();
        }

        // Draws the elements on the list
        void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index); // The element in the list

            //Create a property field and label field for each property. 


            EditorGUI.LabelField(new Rect(rect.x, rect.y, 50, EditorGUIUtility.singleLineHeight), "Task ID:");


            EditorGUI.PropertyField(
                new Rect(rect.x+60, rect.y, 50, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative(nameof(Internal.StringQuestDict.id)),
                GUIContent.none
            );


            EditorGUI.LabelField(new Rect(rect.x + 120, rect.y, 40, EditorGUIUtility.singleLineHeight), "Task:");

            //The property field for level. Since we do not need so much space in an int, width is set to 20, height of a single line.
            EditorGUI.PropertyField(
                new Rect(rect.x+160, rect.y, 150, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative(nameof(Internal.StringQuestDict.task)),
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


    public class DialogueQuestTasks : MonoBehaviour
    {
        public List<Internal.StringQuestDict> displayTasks;
        Dictionary<string, Quests.SimpleQuestStep> tasks;

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
                throw new System.Exception($"The quest task id {ID} does not exist in {nameof(DialogueQuestTasks)}");
            }
        }

    }

    namespace Internal
    {
        [System.Serializable]
        public struct StringQuestDict
        {
            public string id;
            public Quests.SimpleQuestStep task;
        }

    }
  
}