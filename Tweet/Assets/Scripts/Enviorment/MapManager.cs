using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************
 * 地图管理器，控制地图的生成
 ******************************************************/
public class MapManager : MonoBehaviour {

    //两张地图的距离
    public float mapDistance;
    //用来进行主角移动检测的位移量
    public float checkShift;
    //Map的prefab
    public GameObject mapPre;
    //游戏时间（根据该时间会改变道具、障碍的生成概率


    private Transform mTransform;

    private Player player;
    //当前存在的地图列表（一般为两个
    private List<Transform> MapsList;

    //上一个检测点记录的主角位置
    private Vector2 previousPlayerPos;

    //物品生成概率表（记录一行不生成，障碍，道具的生成概率
    private int[] GoodsSpawnChance;
    //道具生成概率表（记录不同类型的道具的生成概率
    private int[] PropSpawnChance;
    //障碍生成概率表（记录不同类型的障碍的生成概率
    private int[] BarrierSpawnChance;

    void Awake()
    {
        mTransform = transform;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        MapsList = new List<Transform>();
    }

    void Start()
    {
        CreateMap();

        previousPlayerPos = player.mTransform.position;
    }

    void Update()
    {
        //检测，当 主角现在的位置 - 前一次检测的位置 >= 移动监测位移量 时，生成新的地图
        if(player.mTransform.position.y - previousPlayerPos.y >= checkShift)
        {
            Debug.Log("--------------------------- 达到临界值，生成新地图 ---------------------------");
            CreateMap();
            previousPlayerPos = player.mTransform.position;
        }
    }

    public void CreateMap()
    {
        //生成一张新地图
        Transform newMap = Instantiate(mapPre, mTransform).transform;
        if (MapsList.Count < 2)
        {
            //加入地图列表
            MapsList.Add(newMap);
            //如果是第一张地图，则对其位置进行修正，并初始化
            if (MapsList.Count == 1)
            {
                newMap.localPosition = Vector3.zero;
                //newMap.GetComponent<SpawnManager>().Init();
                return;
            }
        }
        else
        {
            //删除前一张地图
            if(MapsList[0] != null)
            {
                Destroy(MapsList[0].gameObject, 10);
            }
            //0存储正在通过的地图，1存储下一张要通过的地图
            //生成新地图后，更新正在通过的地图，将新地图设置为下一张要通过的地图
            MapsList[0] = MapsList[1];
            MapsList[1] = newMap;
        }
        var basePos = MapsList[0].localPosition;
        newMap.localPosition = new Vector2(basePos.x, basePos.y + mapDistance);
        //newMap.GetComponent<SpawnManager>().Init();
    }
}
