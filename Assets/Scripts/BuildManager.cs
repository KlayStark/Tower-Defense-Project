using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    public TurretData laserTurretData;
    public TurretData missileTurretData;
    public TurretData standardTurretData;
    //表示当前选择的炮台（要建造的炮台）
    private TurretData selectedTurredData;//UI上显示和选择的炮台
    public Text moneyText;
    public Animator moneyAnimator;
    private int money = 1000;
    public GameObject upgradeCanvas;//升级UI画板
    public Button buttonUpgrade;
    private MapCube selectedMapCube;//3D场景中选择的炮台
    private Animator upgradeCanvasAnimator;//升级UI显示隐藏的动画转换状态机

    void ChangeMoney(int change = 0)
    {
        money += change;
        moneyText.text = "$ " + money;
    }
    private void Start()
    {
        upgradeCanvasAnimator = upgradeCanvas.GetComponent<Animator>();//得到状态机
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //如果鼠标在UI上面，则不做处理; EventSystem.current得到的是EventSystem模块里EventSystem那个组件。
            if (EventSystem.current.IsPointerOverGameObject() == false)//IsPointerOverGameObject表示鼠标是否按在了UI上
            {
                //开发炮台的建造,首先判断鼠标点击到了哪个MapCube上，就要使用射线检测了，得到一个射线ray
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//把鼠标的点转化成射线
                RaycastHit hit;
                //Physics.Raycast来进行射线检测，（射线，RaycastHit射线检测跟什么东西做了碰撞的结果，maxDistance最大距离，layerMask和哪一层做射线检测如不指定就是和所有的层）
                bool isCollider = Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("MapCube"));//得到是否碰撞到MapCube上
                if (isCollider)
                {
                    MapCube mapCube = hit.collider.GetComponent<MapCube>();//得到点击的mapCube
                    if (mapCube.turretGo == null && selectedTurredData != null)//可以创建
                    {
                        if (money > selectedTurredData.cost)
                        {
                            ChangeMoney(-selectedTurredData.cost);
                            mapCube.BuildTurret(selectedTurredData);
                        }
                        else//提示钱不够
                        {
                            //TODO
                            moneyAnimator.SetTrigger("Flicker");
                        }
                    }
                    else if (mapCube.turretGo != null)//如果上边有炮台，那么判断是否做升级处理
                    {
                        if (mapCube.turretGo == selectedMapCube && upgradeCanvas.activeInHierarchy)//如果第二次点击此炮台了并且UI的激活属性是true
                        {
                            StartCoroutine("HideUpgradeUI");//将UI隐藏，用协程的方式
                        }
                        else
                        {
                            //否则显示升级/拆除UI面板，第二个参数的bool值与是否有炮台判断相符，所以不再if判断直接传即可
                            ShowUpgradeUI(mapCube.transform.position, mapCube.isUpgraded);
                        }
                        selectedMapCube = mapCube;//把点击的炮台赋给点击的炮台
                    }
                }
            }
        }
    }
    //在Canvas里的设备里有On Value Changed里添加GameManager，然后选择对应的下面方法，只要是点击设备值发生改变了，就会触发
    public void OnLaserSelected(bool isOn)
    {
        if (isOn)
        {
            selectedTurredData = laserTurretData;
        }
    }
    public void OnMissileSelected(bool isOn)
    {
        if (isOn)
        {
            selectedTurredData = missileTurretData;
        }
    }
    public void OnStandardSelected(bool isOn)
    {
        if (isOn)
        {
            selectedTurredData = standardTurretData;
        }
    }
    void ShowUpgradeUI(Vector3 pos, bool isDisableUpgrade = false)
    {
        StopCoroutine("HideUpgradeUI");//搜索下面的HideUpgradeUI协程方法有没有在运行，有的话先给暂停掉，没有也不会影响。
        //设置画布禁用，为的是切换到新的炮台时候，状态机会初始化一下，能有一个激活弹出UI的效果，这里调用状态机里show的时候，可能HideUpgradeUI还在播放，
        //为了防止冲突故在上面加上一个暂停的协程方法。
        upgradeCanvas.SetActive(false);
        upgradeCanvas.SetActive(true);//设置画布显示
        pos.y = pos.y + 4;
        upgradeCanvas.transform.position = pos;//设置画布位置
        buttonUpgrade.interactable = !isDisableUpgrade;//开启或者禁用升级按钮
    }
    IEnumerator HideUpgradeUI()
    {
        upgradeCanvasAnimator.SetTrigger("Hide");
        yield return new WaitForSeconds(0.8f);//消失的效果结束后再去调用下面
        upgradeCanvas.SetActive(false);//隐藏的时候不能直接把画布禁用，不然就无法播放禁用的动画了
    }
    public void OnUpgradeButtonDown()//按下升级触发的方法
    {
        if (money >= selectedMapCube.turretData.costUpgraded)//如果大于升级所需要的钱
        {
            ChangeMoney(-selectedMapCube.turretData.costUpgraded);
            selectedMapCube.UpgradeTurret();
        }
        else
        {
            moneyAnimator.SetTrigger("Flicker");
        }
        StartCoroutine("HideUpgradeUI");//把UI隐藏掉
    }
    public void OnDestroyButtonDown()//按下拆除触发的方法
    {
        selectedMapCube.DestroyTurret();
        StartCoroutine("HideUpgradeUI");
    }
}
