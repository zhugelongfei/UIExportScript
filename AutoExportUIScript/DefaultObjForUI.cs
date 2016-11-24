using UnityEngine;

namespace AutoExportScriptData
{
    public class DefaultObjForUI
    {
        public static GameObject DefaultGameObject(GameObject parent,string variableFullName)
        {
            if (parent == null)
            {
                Debug.LogError("This is an UIExportScripts error : Parent is null but you want to try get a game object,variable name is:" + variableFullName);
                return null;
            }

            Debug.LogError("This is an UIExportScripts error : Game object reference is missing,variable name is:" + variableFullName);
            GameObject obj = new GameObject();
            obj.name = "DefaultGameObject " + variableFullName;
            obj.transform.parent = parent.transform;

            return obj;
        }

        public static T DefaultComponent<T>(GameObject parent, string variableFullName)
            where T : Component
        {
            if (parent == null)
            {
                Debug.LogError("This is an UIExportScripts error : Parent is null but you want to try get a component,variable name is:" + variableFullName);
                return null;
            }

            Debug.LogError("This is an UIExportScripts error : Component reference is missing,variable name is:" + variableFullName);
            GameObject obj = new GameObject();
            obj.name = "DefaultComponent " + variableFullName + " Type_" + typeof(T).Name;
            obj.transform.parent = parent.transform;

            T comp = default(T);
            if (typeof(T) == typeof(Transform))
                comp = obj.transform as T;
            else
                comp = obj.AddComponent<T>();

            return comp;
        }

    }
}