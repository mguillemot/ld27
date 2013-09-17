using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{

    override public void OnInspectorGUI()
    {
        if (Application.isPlaying)
        {
            if (GUILayout.Button("Serialize current run"))
            {
                Debug.LogWarning(JSON.Serialize(GameManager.currentRun, true));
            }
            if (GUILayout.Button("Unserialize test run"))
            {
                var testRunData = (TextAsset) Resources.Load("testRun");
                var run = JSON.Deserialize<Run>(testRunData.text);
                foreach (var action in run.actions)
                {
                    Debug.LogWarning(action);
                }
            }

            Repaint();
        }
    }

}
