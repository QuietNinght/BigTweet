using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************
 * 第一张地图上面的游戏物品生成器，只在第一张地图上面使用一次
 ******************************************************/
public class InitSpawn : MonoBehaviour {

    [Header("Property")]
    Transform mTransform;

    //地图上的道路数量
    public int trackCount;
    //生成多少行物品
    public int spawnLineCount;
    //行与行的间距
    public float lineSpace;
    public float correctWidth;              //地图边缘留出的宽度
    //起始生成位置
    public float basePos;

    [Header("Prop")]
    public GameObject propPrefab;
    //一张地图上最多道具数量
    public int maxPropCount = 5;
    //当前道具数量
    private int currentPropCount = 0;

    [Header("Barrier")]
    public GameObject barrierPrefab;
    public Sprite barrierSprite;

    //游戏物品类型
    public enum GoodsType
    {
        Barrier,        //障碍，阻挡并消除主角元件
        Prop,           //道具
        Count,          //其它      
    }
    //物品生成概率表
    private int[] GoodsSpawnChance;

    void Awake()
    {
        mTransform = transform;
    }

    public void Init()
    {
        int proCount = Random.Range(1, maxPropCount + 1);

        //生成游戏物品
        for (int i = 0; i < spawnLineCount; i++)
        {
            //初始化物品生成概率数组(每一行的各物品生成概率相同)
            //不生成，生成障碍，生成道具的概率：50%，0%，50%
            GoodsSpawnChance = new int[] { 50, 0, 50 };

            float screenWidthWorldPos = Camera.main.orthographicSize * (Screen.width - correctWidth) / Screen.height;
            float distBetweenBlocks = screenWidthWorldPos / trackCount;

            //在中心生成 一行物品 的父物体
            Transform root = (new GameObject("root" + (i + 1))).transform;
            root.SetParent(mTransform);
            root.localPosition = new Vector2(0, (i * lineSpace) + (basePos * lineSpace));

            //如果是倒数第二行，直接生成一整行
            if (i == spawnLineCount - 2)
            {
                SpawnBarrierFullline(distBetweenBlocks, root);
                continue;
            }

            //根据道路数量来生成每一行的物品个数
            var limit = trackCount / 2;
            //从左边开始生成
            for (int m = -limit; m <= limit; m++)
            {
                if ((trackCount % 2 == 0) && m == 0)
                {
                    //如果道路数量为偶数，且m为0，则不在该节点进行生成操作
                    continue;
                }
                else
                {
                    //随机物品生成类型，0：不生成， 1：障碍， 2：道具
                    int type = Utils.GetRandomType(GoodsSpawnChance);

                    //计算生成位置
                    float posX;
                    if (trackCount % 2 == 0)
                    {
                        posX = m * 2 * distBetweenBlocks + (Mathf.Sign(m) * -distBetweenBlocks);
                    }
                    else
                    {
                        posX = m * 2 * distBetweenBlocks;
                    }
                    var pos = new Vector2(posX, 0);

                    //根据随机到的不同类型，进行操作
                    if (type == 0)
                    {
                        //如果随机到0，则结束该次的物品生成
                        continue;
                    }
                    else if (type == 1)
                    {
                        
                    }
                    else
                    {
                        if (currentPropCount < proCount)
                        {
                            //其它表示道具
                            SpawnProp(pos, root);
                            //同一行，生成一次道具后，道具生成概率减半
                            Utils.RefreshRateArr(ref GoodsSpawnChance, 2, GoodsSpawnChance[2] / 2);
                            //当前道具数量增加
                            currentPropCount++;
                        }
                    }
                }
            }
        }

    }



    //生成障碍
    public void SpawnBarrier(int _x, int _y, Vector2 _pos, Transform _parent, bool isFullLine)
    {
        //随机障碍分数
        int _point = Random.Range(1, 4);

        //生成障碍，设置位置
        Transform _b = Instantiate(barrierPrefab, _parent).transform;
        _b.localPosition = _pos;

        //初始化障碍图片，分数
        var _bScript = _b.GetComponentInChildren<Barrier>();
        _bScript.Init(_x, _y, barrierSprite, _point);
    }

    //生成一整行障碍
    public void SpawnBarrierFullline(float distBetweenBlocks, Transform root)
    {
        Debug.Log("-------------------- 生成一整行障碍 --------------------");
        //根据道路数量来生成每一行的物品个数
        var limit = trackCount / 2;
        //随机一条道路生成生命值必定小于主角当前生命值的障碍
        int mustLessIndex = UnityEngine.Random.Range(-limit, limit + 1);
        //从左边开始生成
        for (int m = -limit; m <= limit; m++)
        {
            if ((trackCount % 2 == 0) && m == 0)
            {
                //如果道路数量为偶数，且m为0，则不在该节点进行生成操作
                while (mustLessIndex == 0)
                {
                    mustLessIndex = UnityEngine.Random.Range(-limit, limit + 1);
                }
                continue;
            }
            else
            {
                float posX;
                if (trackCount % 2 == 0)
                {
                    posX = m * 2 * distBetweenBlocks + (Mathf.Sign(m) * -distBetweenBlocks);
                }
                else
                {
                    posX = m * 2 * distBetweenBlocks;
                }
                var pos = new Vector2(posX, 0);

                if (m == mustLessIndex)
                {
                    SpawnBarrier(m, -1, pos, root, true);

                }
                else
                {
                    SpawnBarrier(m, -1, pos, root, false);
                }
            }
        }
    }

    //生成道具
    public void SpawnProp(Vector2 _pos, Transform _parent)
    {
        //生成道具，设置位置
        Transform _p = Instantiate(propPrefab, _parent).transform;
        _p.localPosition = _pos;

        //初始化道具
        var _pScript = _p.GetComponent<Prop>();
        _pScript.Init();
    }
}
