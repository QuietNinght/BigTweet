using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************
 * 地图管理器，控制地图的生成
 ******************************************************/
public class MapManager : MonoBehaviour {

    private static MapManager instance;
    public static MapManager Instance
    {
        get { return instance; }
    }

    //两张地图的距离
    public float mapDistance;

    //Map的prefab
    public GameObject mapPre;

    private Transform mTransform;

    //当前存在的地图列表（一般为两个
    private List<Transform> MapsList;

    void Awake()
    {
        instance = this;

        mTransform = transform;

        MapsList = new List<Transform>();
    }

    void Start()
    {
        CreateMap();
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
                return;
            }
        }
        else
        {
            //删除前一张地图
            if(MapsList[0] != null)
            {
                Destroy(MapsList[0].gameObject);
            }
            //0存储正在通过的地图，1存储下一张要通过的地图
            //生成新地图后，更新正在通过的地图，将新地图设置为下一张要通过的地图
            MapsList[0] = MapsList[1];
            MapsList[1] = newMap;
        }
        var basePos = MapsList[0].localPosition;
        newMap.localPosition = new Vector2(basePos.x, basePos.y + mapDistance);
    }
}
