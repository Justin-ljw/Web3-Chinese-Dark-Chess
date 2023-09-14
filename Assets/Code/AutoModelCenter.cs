using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AutoModelCenter : MonoBehaviour
{
    //<summary>获取模型的中心点</summary>
    [MenuItem("SKFramework/Tools/GetModelCenter")]
    public static void GetModelCenter()
    {
        //如果未选中任何Transformreturn
        if (Selection.activeTransform == null)
            return;

        Transform transform = Selection.activeTransform;

        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

        //获取所有MeshRenderer 包括子物体
        var mrs = transform.GetComponentsInChildren<MeshRenderer>(true);
        Vector3 center = Vector3.zero;

        for (int i = 0; i < mrs.Length; i++)
        {
            center += mrs[i].bounds.center;
            //Encapsulate函数重新计算bounds
            bounds.Encapsulate(mrs[i].bounds);
        }

        center /= mrs.Length;

        //创建一个新物体作为空父级
        GameObject obj = new GameObject();
        obj.name = transform.name;
        obj.transform.position = center;
        obj.transform.rotation = Quaternion.identity;
        obj.transform.SetParent(transform.parent);
        transform.SetParent(obj.transform);
    }
}
