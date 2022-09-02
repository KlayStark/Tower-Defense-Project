using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����ع���
/// </summary>
/// �������˽ű�д�ú�����ϲ���Ҫ�Ķ��ű���ʹ�ô˽ű�ʱ��Ҫ�Ѵ˽ű����ڳ����У������޷�ʹ�ô˽ű�
public class ObjectPoolManager : MonoBehaviour

{   //����
    public static ObjectPoolManager instance;

    //Dictionary�ֵ�
    //�����ֵ䣬��Ϊstring���ͣ�ֵΪList���͵ģ��ֵ������Ϊpool
    private Dictionary<string, List<GameObject>> pool;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //ʵ�����ֵ�
        pool = new Dictionary<string, List<GameObject>>();
    }

    /// <summary>
    /// ȡ����ķ���
    /// </summary>
    /// <param name="objName">Ԥ��������֣�Ҳ�ǳ���������֣�Ҳ�Ǽ�ֵ�Ե�����</param>
    /// <param name="pos">���ɶ��������</param>
    /// <param name="qua">���ɶ���ĽǶ�</param>
    /// <returns>�õ��Ķ���</returns> GameObjectΪ����ֵ ���ص���ʵ�����Ķ���
    public GameObject GetObjectFromPool(string objName, Vector3 pos, Quaternion qua)
    {
        //��ʵ�����Ķ���
        GameObject go;

        //�ж��Ƿ���ڶ�Ӧ�ĳ��ӣ�ͨ���ֵ�ļ�ֵ���ж��Ƿ����objname�ļ���
        //���жϳ������Ƿ���������ж������ȡ������ͨ���ж�List���Ԫ�ظ���������0˵��������һ����
        if (pool.ContainsKey(objName) && pool[objName].Count > 0)
        {
            //��������ȡ����һ��Ԫ��
            go = pool[objName][0];

            //������0��Ԫ�ش��������Ƴ�
            pool[objName].RemoveAt(0);

            //����ȡ���Ķ���                          
            go.SetActive(true);

        }

        else
        {
            //���������û�и�Ԫ�أ��ʹ�Resources�ļ�����ʵ����������ֵ��go
            go = Instantiate(Resources.Load(objName) as GameObject);
        }

        //��ֵ����      
        go.transform.position = pos;

        //��ֵ�Ƕ�
        go.transform.rotation = qua;

        //�����ɵ�Ԥ���巵�ظ�go
        return go;
    }


    //��ʵ�������������嶼�浽������
    public void PushObjectToPool(GameObject go)
    {
        //ͨ��Instantiate���ɵĶ������־�ΪԤ�������ּ��ϣ�clone����������Ҫ�и���ܵõ�����Ԥ���������
        string prefabName = go.name.Split('(')[0];

        //ͨ��Ԥ������������ж��Ƿ��Ѿ��ж�Ӧ�ĳ��ӣ������ֱ�ӽ�go�ŵ�������
        if (pool.ContainsKey(prefabName))
        {
            //�Ѷ���go��ӵ�������
            pool[prefabName].Add(go);
        }

        //û�ж�Ӧ�ĳ��ӣ��Ǿ���pool�д���һ���ֵ�ԣ�����go�ŵ��µĳ�����
        else
        {
            //ʵ����һ�����Ӳ���go�ŵ�������
            pool[prefabName] = new List<GameObject>() { go };

        }

        //������ŵ������к�Ҫ���������ϵ��ٶ���Ϊ0
        go.GetComponent<Rigidbody>().velocity = Vector3.zero;

        //��Ԥ�����������к�Ҫ�����ɳ�����������Ϊ����״̬
        go.SetActive(false);
    }


}
