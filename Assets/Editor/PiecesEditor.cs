using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(StartPieces))]
public class PiecesEditor : Editor
{
    private StartPieces startPieces = null!;

    public void OnEnable()
    {
        startPieces = (StartPieces) target;
    }

    public override void OnInspectorGUI()
    {
        const int spriteWidth = 48;
        const int spriteHeight = 48;
        const int buttonWidth = 20;
        const int buttonHeight = 20;
        var maxCount = 0;

        if (startPieces.list.Count == 0) EditorGUILayout.LabelField("Нет элементов в списке");
        foreach (var item in startPieces.list)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("X", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            {
                startPieces.list.Remove(item);
                break;
            }

            if (GUILayout.Button("+", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            {
                item.images.Add(null);
                refreshSpriteCount(item.images.Count);
                break;
            }

            if (GUILayout.Button("-", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
            {
                item.images.RemoveAt(item.images.Count - 1);
                refreshSpriteCount(item.images.Count);
                break;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            maxCount = Math.Max(item.images.Count, maxCount);
            for (var i = 0; i < item.images.Count; i++)
            {
                var image = item.images[i];
                item.images[i] = (Sprite) EditorGUILayout.ObjectField(image, typeof(Sprite), false,
                    GUILayout.Width(spriteWidth),
                    GUILayout.Height(spriteHeight));
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Добавить строку"))
        {
            startPieces.list.Add(new SelectSprite());
            refreshSpriteCount(maxCount);
        }

        if (GUI.changed) SetDirty(startPieces.gameObject);
    }

    private void refreshSpriteCount(int count)
    {
        foreach (var images in startPieces.list.Select(t => t.images))
        {
            bool equal;
            do
            {
                if (images.Count < count) images.Add(null);
                if (images.Count > count) images.RemoveAt(images.Count - 1);
                equal = images.Count == count;
            } while (!equal);
        }
    }

    private static void SetDirty(GameObject obg)
    {
        EditorUtility.SetDirty(obg);
        EditorSceneManager.MarkSceneDirty(obg.scene);
    }
}