using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    private List<GameObject> enemys = new List<GameObject>();
    //进入到触发事件，只攻击最先进入攻击范围内的敌人
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Enemy")
        {
            enemys.Add(col.gameObject);
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.tag == "Enemy")
        {
            enemys.Remove(col.gameObject);
        }
    }

    public float attackRateTime = 1;//多少秒攻击一次,外置能调节
    private double timer = 0;//利用计时器判断时间是否到
    public GameObject bulletPrefab;//攻击的子弹,把在Prefab中的bullet预制体拖拽一下
    public Transform firePosition;//攻击的炮口位置,（直接拖拽在StandarTurret里拖拽它下面的FirePosition）
    public Transform head;//炮塔的头部
    public bool useLaser = false;//表示是否使用激光
    public float damageRate = 70;//激光的伤害值/秒
    public LineRenderer laserRenderer;//激光线
    public GameObject laserEffect;

    private void Start()
    {
        timer = attackRateTime;//刚发现第一次就能攻击
    }
    private void Update()
    {
        //炮头的位置望向敌人
        if (enemys.Count > 0 && enemys[0] != null)
        {
            Vector3 targetPosition = enemys[0].transform.position;
            targetPosition.y = head.position.y;//如果敌人和head的z轴的垂直方向不一致，也就是高度不一致，设置y值相等后再望向。
            head.LookAt(targetPosition);
        }
        if (!useLaser)
        {
            //先调整完方向，再进行攻击
            timer += Time.deltaTime;
            if (enemys.Count > 0 && timer >= attackRateTime)//如果有敌军并且大于攻击时间，则timer归零，开始进行攻击
            {
                timer = 0;
                Attack();
            }
        }
        else if (enemys.Count > 0)//再做一个判断，附近必须有敌人
        {
            if (!laserRenderer.enabled) laserRenderer.enabled = true;
            laserEffect.SetActive(true);//特效组件激活
            if (enemys[0] == null)
            {
                UpdateEnemys();//如果第一号敌人为空，就更新敌人
            }
            if (enemys.Count > 0)//更新完敌人后，再判断是否还有敌人。
            {
                laserRenderer.SetPositions(new Vector3[] { firePosition.position, enemys[0].transform.position });
                enemys[0].GetComponent<Enemy>().TakeDamage(damageRate * Time.deltaTime);
                laserEffect.transform.position = enemys[0].transform.position;//特效位置在敌人那
                //因为每一个炮台的位置都是跟Cube的位置保持一致的，所以先取得炮台的位置，让这个炮台的Y和敌人的保持一致，因为特效是在敌人的位置，然后让特效朝向这个位置。
                Vector3 pos = transform.position;
                pos.y = enemys[0].transform.position.y;
                laserEffect.transform.LookAt(pos);
            }
        }
        else
        {
            laserEffect.SetActive(false);//特效组件禁用
            laserRenderer.enabled = false;//离开视野了，不使用的时候把激光关了
        }
    }
    void Attack()
    {
        if (enemys[0] == null)//等于空，则意味着集合中已经有空元素了
        {
            UpdateEnemys();
        }
        if (enemys.Count > 0)
        {
            GameObject bullet = GameObject.Instantiate(bulletPrefab, firePosition.position, firePosition.rotation);
            //解释：通过bullet这个GameObject获得它下面名为Bullet脚本的组件对象，然后在调用此对象的方法。
            bullet.GetComponent<Bullet>().SetTarget(enemys[0].transform);//攻击目标默认使用集合中的第一个元素
        }
        else
        {
            timer = attackRateTime;//归置炮弹状态，下次来敌人就可以直接进行攻击了
        }
    }
    void UpdateEnemys()//移出所有为空的元素
    {
        List<int> emptyIndex = new List<int>();
        for (int index = 0; index < enemys.Count; index++)
        {
            if (enemys[index] == null)
            {
                emptyIndex.Add(index);
            }
        }
        for (int i = 0; i < emptyIndex.Count; i++)
        {
            enemys.RemoveAt(emptyIndex[i] - i);//因为每删完一个，后边的值就会往前移动一个，所以要做一下从新定位的"-i"处理
        }
    }
}