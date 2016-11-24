using UnityEngine;

namespace AutoExportScriptData
{
    public class UIExportScript : MonoBehaviour
    {
        // 生成的主类名(等于文件名)
        public string ClassName;

        /// <summary>
        /// 运行时删除（最好是在打包时就删除）
        /// </summary>
        public void Awake()
        {
            DestroyImmediate(this);
        }
    }
}