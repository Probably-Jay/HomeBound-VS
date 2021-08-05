using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SceneChange
{
#if UNITY_EDITOR

    [CustomEditor(typeof(ChangeScene))]
    public class ChangeSceneEditor : Editor
    {
        SerializedProperty sceneToChangeTo;

        void OnEnable()
        {
            sceneToChangeTo = serializedObject.FindProperty(nameof(ChangeScene.sceneToChangeto));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(sceneToChangeTo, new GUIContent("Scene to change to"));
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

    public class ChangeScene : MonoBehaviour
    {
        [SerializeField]internal Scenes sceneToChangeto;
        public void ChangeToScene() => SceneChangeController.Instance.ChangeScene(sceneToChangeto);
    }
}
                               