using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ConditionManager))]
public class ConditionManagerTester : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(10);

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontStyle = FontStyle.Bold;

        if (GUILayout.Button("QA: Automatizált Rendszerteszt Futtatása", buttonStyle, GUILayout.Height(40)))
        {
            RunAutomatedTest();
        }
    }

    private void RunAutomatedTest()
    {
        ConditionManager manager = (ConditionManager)target;
        string testCondition = "Test_QA_Lock_01";

        bool originalState = manager.CheckCondition(testCondition);

        manager.SetCondition(testCondition, true);
        bool testResult = manager.CheckCondition(testCondition);

        if (testResult == true)
        {
            Debug.Log("<color=green>✔ SIKERES TESZT: A ConditionManager hibátlanul menti és olvassa az állapotokat a memóriában!</color>");
        }
        else
        {
            Debug.LogError("❌ SIKERTELEN TESZT: Valami elromlott a ConditionManager-ben!");
        }

        manager.SetCondition(testCondition, originalState);
    }
}