using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TurretData
{
    public GameObject turretPrefab;//炮塔的模型
    public int cost;//价格
    public GameObject turretUpgradedPrefab;//升级的模型
    public int costUpgraded;//升级的价格
    public TurretType type;
}

public enum TurretType
{
    LaserTurret,//激光炮台
    MissileTurret,//导弹炮台
    StandardTurret,//标准炮台
}