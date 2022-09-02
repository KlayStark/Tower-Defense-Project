using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对象池管理
/// </summary>
/// ！！！此脚本写好后基本上不需要改动脚本，使用此脚本时需要把此脚本挂在场景中，否则无法使用此脚本
public class ObjectPoolManager : MonoBehaviour

{   //单例
    public static ObjectPoolManager instance;

    //Dictionary字典
    //创建字典，键为string类型，值为List类型的，字典的名字为pool
    private Dictionary<string, List<GameObject>> pool;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //实例化字典
        pool = new Dictionary<string, List<GameObject>>();
    }

    /// <summary>
    /// 取对象的方法
    /// </summary>
    /// <param name="objName">预设体的名字，也是池子里的名字，也是键值对的名字</param>
    /// <param name="pos">生成对象的坐标</param>
    /// <param name="qua">生成对象的角度</param>
    /// <returns>得到的对象</returns> GameObject为返回值 返回的是实例化的对象
    public GameObject GetObjectFromPool(string objName, Vector3 pos, Quaternion qua)
    {
        //被实例化的对象
        GameObject go;

        //判断是否存在对应的池子（通过字典的键值对判断是否包含objname的键）
        //并判断池子里是否包含对象，有对象才能取出来（通过判断List里的元素个数，大于0说明至少有一个）
        if (pool.ContainsKey(objName) && pool[objName].Count > 0)
        {
            //从链表中取出第一个元素
            go = pool[objName][0];

            //并将第0个元素从链表中移除
            pool[objName].RemoveAt(0);

            //激活取出的对象                          
            go.SetActive(true);

        }

        else
        {
            //如果池子中没有该元素，就从Resources文件夹中实例化出来赋值给go
            go = Instantiate(Resources.Load(objName) as GameObject);
        }

        //赋值坐标      
        go.transform.position = pos;

        //赋值角度
        go.transform.rotation = qua;

        //把生成的预设体返回给go
        return go;
    }


    //把实例化出来的物体都存到池子中
    public void PushObjectToPool(GameObject go)
    {
        //通过Instantiate生成的对象名字均为预设体名字加上（clone），所以需要切割才能得到真正预设体的名字
        string prefabName = go.name.Split('(')[0];

        //通过预设体的名字来判断是否已经有对应的池子，如果有直接将go放到池子中
        if (pool.ContainsKey(prefabName))
        {
            //把对象go添加到池子中
            pool[prefabName].Add(go);
        }

        //没有对应的池子，那就在pool中创建一组键值对，并将go放到新的池子中
        else
        {
            //实例化一个池子并把go放到池子中
            pool[prefabName] = new List<GameObject>() { go };

        }

        //把物体放到池子中后要把物体身上的速度设为0
        go.GetComponent<Rigidbody>().velocity = Vector3.zero;

        //把预设体放入池子中后要把生成出来的物体设为隐藏状态
        go.SetActive(false);
    }


}
