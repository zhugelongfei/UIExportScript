using UnityEditor;
using UnityEngine;

namespace AutoExportScriptData
{
    [CustomEditor(typeof(UIExportScript))]
    internal class UIExportScriptInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (PrefabUtility.GetPrefabType(target) == PrefabType.Prefab)
            {
                //证明是Prefab上的脚本，这个时候，导出脚本功能不正常，所以不能导出。
                return;
            }

            GUILayout.BeginHorizontal();
            //导出脚本
            if (GUILayout.Button("Export Inside Script"))
            {
                ExportScript(false);
            }

            //导出脚本
            if (GUILayout.Button("Export Dll Script"))
            {
                ExportScript(true);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            //编译Dll
            if (GUILayout.Button("Compile dll"))
            {
                CompileDll();
            }

            //大纲
            if (GUILayout.Button("OutLine"))
            {
                UIExportScript expScript = target as UIExportScript;
                OutLineWindow.OutLineWindowData data = new OutLineWindow.OutLineWindowData();
                data.clsName = expScript.ClassName;
                ShowUIProgramDataBaseWindow<OutLineWindow.OutLineWindowData>.Open<OutLineWindow>
                    (OutLineWindow.windowName, expScript.GetComponentsInChildren<UIProgramData>(true), data);
            }

            GUILayout.EndHorizontal();

            GUILayout.Label("by - ZhuGeLongFei");
        }

        public void ExportScript(bool isGenDllScript)
        {
            UIExportScript expScript = (target as UIExportScript);
            try
            {
                System.Diagnostics.Stopwatch stop = new System.Diagnostics.Stopwatch();
                stop.Start();
                bool isNeedReGenDll = false;

                ExportScriptTools tools = new ExportScriptTools(expScript);
                string filePath = FilePathManager.Instance.GenerateFilePath(isGenDllScript, expScript.ClassName, ref isNeedReGenDll);
                tools.ExportScript(filePath);

                if (isGenDllScript || isNeedReGenDll)
                {
                    //编译DLL，编译后，会刷新。
                    CompileDll();
                }
                else
                {
                    //没编译，所以要刷新
                    AssetDatabase.Refresh();
                }
                stop.Stop();
                Debug.Log("导出脚本耗时：" + stop.ElapsedMilliseconds + "ms");
            }
            catch (UIExportDataException e)
            {
                if (e.ErrorObjs != null)
                {
                    if (EditorUtility.DisplayDialog("Export Script Error", e.Message, "Show Error", "Ok"))
                    {
                        ErrorWindow.ErrorWindowData data = new ErrorWindow.ErrorWindowData();
                        data.clsName = expScript.ClassName;
                        data.errorLog = e.Message;
                        ShowUIProgramDataBaseWindow<ErrorWindow.ErrorWindowData>.Open<ErrorWindow>
                            (ErrorWindow.windowName, e.ErrorObjs, data);
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("Export Script Error", e.Message, "Ok");
                }
            }
        }

        private void CompileDll()
        {
            FilePathManager.Instance.DeleteDllFile();
            //检测是否有脚本可以编译
            bool canComp = FilePathManager.Instance.CheckCanCompileDll(Application.dataPath);
            if (canComp)
            {
                EditorApplication.update += WaitForRefreshAssets;
                CompileCSharpDll csd = new CompileCSharpDll(Debug.Log);
                csd.Compile(FilePathManager.Instance.GetCompileConfigInfo());
            }
            else
            {
                AssetDatabase.Refresh();
            }
        }

        private void WaitForRefreshAssets()
        {
            if (!CompileCSharpDll.isFinished)
                return;
            CompileCSharpDll.isFinished = false;
            EditorApplication.update -= WaitForRefreshAssets;
            AssetDatabase.Refresh();
        }

    }
}