using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace kt
{
    public class ProfilerDriverSample : EditorWindow
    {

        [MenuItem("Tools/kt/ProfilerDriverSample")]
        private static void ShowWindow()
        {
            var window = GetWindow<ProfilerDriverSample>();
            window.titleContent = new GUIContent("ProfilerDriverSample");
            window.Show();
        }

        private void OnGUI()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.Space();

                EditorGUI.BeginDisabledGroup(true);

                // Profilerで選択されている関数のパスを取得
                var selectPath = ProfilerDriver.selectedPropertyPath;
                if (selectPath == string.Empty) selectPath = "not selected.";

                EditorGUILayout.LabelField("selectedPropertyPath", selectPath);

                // Profilerで計測している開始と最後のフレーム数
                var firstFrame = ProfilerDriver.firstFrameIndex;
                var lastFrame = ProfilerDriver.lastFrameIndex;

                EditorGUILayout.IntField("firstFrameIndex", firstFrame);
                EditorGUILayout.IntField("lastFrameIndex", lastFrame);

                // 計測対象がEditorかどうか
                var isProfileEditor = ProfilerDriver.profileEditor;

                // DeepProfileをしているかどうか
                var isDeepProfile = ProfilerDriver.deepProfiling;

                EditorGUILayout.Toggle("profileEditor", isProfileEditor);
                EditorGUILayout.Toggle("deepProfiling", isDeepProfile);

                var connectedProfilerCount = ProfilerDriver.connectedProfiler;
                EditorGUILayout.IntField("connectedProfiler", connectedProfilerCount);

                EditorGUI.EndDisabledGroup();

                // Profilerの中の情報をクリア
                if (GUILayout.Button("ClearAllFrames"))
                {
                    ProfilerDriver.ClearAllFrames();
                }


            }





        }
    }

}
