using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class YEditorUtility
{
    public static void BestGirlBanner()
    {
        Texture banner = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Sprites/Editor/bestgirl_banner.png", typeof(Texture));
        float imageWidth = EditorGUIUtility.currentViewWidth - 40;
        float imageHeight = imageWidth * banner.height / banner.width;
        Rect rect = GUILayoutUtility.GetRect(imageWidth, imageHeight);
        GUI.DrawTexture(rect, banner, ScaleMode.ScaleToFit);
        GUILayout.Space(10);
    }
}
