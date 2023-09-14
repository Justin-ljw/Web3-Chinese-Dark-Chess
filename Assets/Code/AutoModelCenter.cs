using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AutoModelCenter : MonoBehaviour
{
    //<summary>��ȡģ�͵����ĵ�</summary>
    [MenuItem("SKFramework/Tools/GetModelCenter")]
    public static void GetModelCenter()
    {
        //���δѡ���κ�Transformreturn
        if (Selection.activeTransform == null)
            return;

        Transform transform = Selection.activeTransform;

        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

        //��ȡ����MeshRenderer ����������
        var mrs = transform.GetComponentsInChildren<MeshRenderer>(true);
        Vector3 center = Vector3.zero;

        for (int i = 0; i < mrs.Length; i++)
        {
            center += mrs[i].bounds.center;
            //Encapsulate�������¼���bounds
            bounds.Encapsulate(mrs[i].bounds);
        }

        center /= mrs.Length;

        //����һ����������Ϊ�ո���
        GameObject obj = new GameObject();
        obj.name = transform.name;
        obj.transform.position = center;
        obj.transform.rotation = Quaternion.identity;
        obj.transform.SetParent(transform.parent);
        transform.SetParent(obj.transform);
    }
}
