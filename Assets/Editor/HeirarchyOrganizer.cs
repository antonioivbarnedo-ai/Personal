using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class HierarchyOrganizer
{
    static HierarchyOrganizer()
    {
        // Subscribe to the event that draws the hierarchy window
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
    }

    static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        // ---------------------------------------------------------
        // FIX: We tell Unity to ignore the "Obsolete" warning here
        // ---------------------------------------------------------
#pragma warning disable 618
        var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
#pragma warning restore 618
        // ---------------------------------------------------------

        // Check if the object exists and starts with our special key "---"
        if (gameObject != null && gameObject.name.StartsWith("---"))
        {
            // 1. Draw a dark background box
            EditorGUI.DrawRect(selectionRect, new Color(0.15f, 0.15f, 0.15f));

            // 2. Create the text style (Centered, Bold, Colored)
            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = 12
            };

            // Set the text color (Teal/Cyan)
            style.normal.textColor = new Color(0.2f, 1.0f, 1.0f);

            // 3. Draw the label (We remove the "---" so it looks clean)
            string headerText = gameObject.name.Replace("-", "").ToUpperInvariant();
            EditorGUI.LabelField(selectionRect, headerText, style);
        }
    }
}