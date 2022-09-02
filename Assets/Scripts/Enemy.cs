using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

    public float speed = 10;
    public float hp = 150;
    private float totalHp;
    public GameObject explosionEffect;
    private Slider hpSlider;
    private Transform[] positions;
    private int index = 0;


	// Use this for initialization
	void Start () {
        positions = Waypoints.positions;
        totalHp = hp;
        hpSlider = GetComponentInChildren<Slider>(); //从子物体中寻找Slider物体装配上
    }
	
	// Update is called once per frame
	void Update () {
        Move();
	}


    void Move()
    {
        //if (index > positions.Length - 1) return;//当到达最后一个位置
        //（目标位置 - 当前位置）得到一个向量.单位化每次移动1，取得单位向量之后再做计算
        transform.Translate((positions[index].position - transform.position).normalized * Time.deltaTime * speed);
        //判断有没有到达目标位置，取得两个点位置是否小于一定距离
        if (Vector3.Distance(positions[index].position, transform.position) < 0.2f)
        {
            index++;
        }
        if (index > positions.Length - 1)
        {
            ReachDestination();
        }
    }
    //达到终点
    void ReachDestination()
    {
        GameManager.Instance.Failed();
        GameObject.Destroy(this.gameObject);
    }


    void OnDestroy()
    {
        EnemySpawner.CountEnemyAlive--;
    }

    public void TakeDamage(float damage)
    {
        if (hp <= 0) return;
        hp -= damage;
        hpSlider.value = (float)hp / totalHp; //hpSlider.value是一个0~1的值
        if (hp <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        GameObject effect = GameObject.Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(effect, 1.5f);
        Destroy(this.gameObject);
    }

}
