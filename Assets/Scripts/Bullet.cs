using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 50;//伤害
    public float speed = 40;
    private Transform target;//目标
    public GameObject explosionEffectPrefab;//爆炸特效💥

    public void SetTarget(Transform _target)
    {
        this.target = _target;
    }
    private void Update()//控制子弹的移动
    {
        if (target == null)//如果目标不存在了,飞行中的子弹自行销毁
        {
            Die();
            return;
        }
        transform.LookAt(target.position);//面向目标的位置
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            //让敌人掉血
            other.GetComponent<Enemy>().TakeDamage(damage);
            Die();
        }
    }
    void Die()
    {
        GameObject effect = GameObject.Instantiate(explosionEffectPrefab, transform.position, transform.rotation);//增加特效
        Destroy(effect, 1);
        Destroy(this.gameObject);//销毁这个游戏物体,如果只是this，只是销毁Bullet组件
        // ObjectPoolManager.instance.PushObjectFromPool();
    }
}