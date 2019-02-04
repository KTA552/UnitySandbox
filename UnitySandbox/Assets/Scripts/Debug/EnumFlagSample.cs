using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class EnumFlagSample : EditorWindow
{

    [Flags]
    enum OutputFlag
    {
        Flag0 = 1 << 0,
        Flag1 = 1 << 1,
        Flag2 = 1 << 2,
        Flag3 = 1 << 3,
        Flag4 = 1 << 4,
    }

    [MenuItem("Tools/kt/Samples/EnumFlagSample")]
    private static void ShowWindow()
    {
        var window = GetWindow<EnumFlagSample>();
        window.titleContent = new GUIContent("EnumFlagSample");
        window.Show();
    }

    OutputFlag _outputFlag = OutputFlag.Flag0 | OutputFlag.Flag3;

    private void OnGUI()
    {

        using (new EditorGUILayout.VerticalScope())
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("GUI Toggle:");

            foreach (var item in Enum.GetValues(typeof(OutputFlag)))
            {
                var flagName = Enum.GetName(typeof(OutputFlag), item);
                bool isToggle = false;
                var flagValue = (OutputFlag)item;

                if (_outputFlag.HasFlag(flagValue))
                {
                    isToggle = true;
                }

                // 確認したフラグが違ってたら反転
                var toggleResult = EditorGUILayout.Toggle(flagName, isToggle);
                if (toggleResult != isToggle)
                {
                    _outputFlag ^= flagValue;
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("GUI EnumFlagsField:");
            EditorGUILayout.EnumFlagsField(_outputFlag);

            EditorGUILayout.Space();

            // 今のフラグの状態の表示
            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUILayout.LabelField("CurrentFlag", _outputFlag.ToString());
                EditorGUILayout.LabelField("FlagHexValue", _outputFlag.ToString("X"));
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}
