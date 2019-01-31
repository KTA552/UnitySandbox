using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditorInternal.Profiling;
using System;

public class ProfilerExport : EditorWindow
{

    [MenuItem("UnitySandbox/ProfilerExport")]
    private static void ShowWindow()
    {
        var window = GetWindow<ProfilerExport>();
        window.titleContent = new GUIContent("ProfilerExport");
        window.Show();
    }

    public class CollectColumn
    {
        public string _name;

        public string[] _columnValue = new string[System.Enum.GetValues(typeof(ProfilerColumn)).Length];
    }

    [Flags]
    public enum ExportProfilerColumn
    {
        FunctionName = 1 << ProfilerColumn.FunctionName,
        ObjectName = 1 << ProfilerColumn.ObjectName,
        TotalPercent = 1 << ProfilerColumn.TotalPercent,
        SelfPercent = 1 << ProfilerColumn.SelfPercent,
        Calls = 1 << ProfilerColumn.Calls,
        GCMemory = 1 << ProfilerColumn.GCMemory,
        TotalTime = 1 << ProfilerColumn.TotalTime,
        SelfTime = 1 << ProfilerColumn.SelfTime,
        DrawCalls = 1 << ProfilerColumn.DrawCalls,
        WarningCount = 1 << ProfilerColumn.WarningCount,
    }



    List<CollectColumn> _profilerPropertyData = new List<CollectColumn>();

    string _logText = string.Empty;

    Vector2 _logScrollPosition = Vector2.zero;

    int _captureIndex;

    float _collectionThreshold = 0.05f;

    ExportProfilerColumn _exportCollectColumn = ExportProfilerColumn.FunctionName |
                                                ExportProfilerColumn.SelfTime;

    bool _isCaptureProfiler = false;

    private void OnGUI()
    {

        using (new EditorGUILayout.VerticalScope())
        {
            var firstIndex = ProfilerDriver.firstFrameIndex;
            var lastIndex = ProfilerDriver.lastFrameIndex;

            _captureIndex = EditorGUILayout.IntSlider("CaptureFrame", _captureIndex, firstIndex, lastIndex);

            _collectionThreshold = EditorGUILayout.FloatField("SelfTime Threshold", _collectionThreshold);

            using (new EditorGUILayout.ToggleGroupScope("ExportColumns", _isCaptureProfiler))
            {
                foreach (var item in Enum.GetValues(typeof(ExportProfilerColumn)))
                {
                    var columnName = Enum.GetName(typeof(ExportProfilerColumn), item);
                    bool isToggle = false;

                    var columnValue = (ExportProfilerColumn)item;

                    if ((_exportCollectColumn & columnValue) == columnValue)
                    {
                        isToggle = true;
                    }

                    var toggleResult = EditorGUILayout.Toggle(columnName, isToggle);
                    if (toggleResult != isToggle)
                    {
                        _exportCollectColumn ^= columnValue;
                    }
                }
            }


            if (GUILayout.Button("Capture"))
            {
                CaptureProfilerData();
                _isCaptureProfiler = true;
            }

            using (var scrollView = new EditorGUILayout.ScrollViewScope(_logScrollPosition))
            {
                _logScrollPosition = scrollView.scrollPosition;

                if (_logText != string.Empty)
                {
                    EditorGUILayout.TextArea(_logText);
                }
                else
                {
                    EditorGUILayout.TextArea("not capture log.");
                }
            }

        }
    }

    /// <summary>
    /// プロファイラーの情報のキャプチャ
    /// </summary>
    private void CaptureProfilerData()
    {
        // ProfilerのRootPropertyを取得
        var property = new ProfilerProperty();

        // Profilerに表示されているCurrentFrame+1のデータが取れる
        property.SetRoot(_captureIndex - 1, ProfilerColumn.SelfTime, ProfilerViewType.Hierarchy);

        if (property.HasChildren)
        {
            _profilerPropertyData.Clear();

            _logText = string.Empty;
            _logText += "Name, SelfTime, SelfPercent \n";
            _logText += "---\n";
        }


        // PropertyのHierarchyを辿る
        while (property.Next(true))
        {
            var value = property.GetColumnAsSingle(ProfilerColumn.SelfTime);

            // 全部取れるので適当な閾値で切る
            if (value > _collectionThreshold)
            {
                var addData = new CollectColumn();

                addData._name = property.propertyName;

                for (int i = 0; i < System.Enum.GetValues(typeof(ProfilerColumn)).Length; i++)
                {
                    if (Enum.IsDefined(typeof(ProfilerColumn), i))
                    {
                        var columnElement = (ProfilerColumn)Enum.ToObject(typeof(ProfilerColumn), i);

                        addData._columnValue[i] = property.GetColumn(columnElement);
                    }
                }

                _profilerPropertyData.Add(addData);

                _logText += String.Format("{0}, {1}, {2}. \n",
                                addData._name,
                                addData._columnValue[(int)ProfilerColumn.SelfTime],
                                addData._columnValue[(int)ProfilerColumn.SelfPercent]);
            }
        }

        _logText += "---\n";

    }
}
