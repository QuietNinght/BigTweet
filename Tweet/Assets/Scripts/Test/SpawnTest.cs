using System.Collections.Generic;
using UnityEngine;

public class SpawnTest : MonoBehaviour {

    //主角
    Player player;

    Transform mTransform;

    //地图上的道路数量
    public int trackCount;
    //生成多少行物品
    public int spawnLineCount;
    //行与行的间距
    public float lineSpace;
    //地图上的物品列表（销毁地图时可能会使用）
    private List<Barrier> BarriersList;
    private List<Prop> PropsList;

    [Header("Prop")]
    public GameObject propPre;
    //每多少秒内限制道具的刷出数量
    public float spawnPropLimitTime;
    //一张地图上最多道具数量
    public int maxPropCount = 5;
    //当前道具数量
    private int currentPropCount = 0;

    [Header("Barrier")]
    public GameObject barrierPre;

    [Header("Railing")]
    public GameObject railPre;

    //游戏物品类型
    public enum GoodsType
    {
        Barrier,        //障碍，阻挡并消除主角元件
        Prop,           //道具
        Count,          //其它      
    }

    //游戏道具类型
    public enum PropType
    {
        Cooky,          //姜饼饼干，增加主角元件
        Doughunt,       //甜甜圈，护盾 +　爆炸（销毁整行障碍）
        Lollipop,       //棒棒糖，开启主角的攻击
        Chocolate,      //巧克力，冲刺
        Bread,          //U型面包，磁铁功能
        Count,          //其它      
    }
    //道具 类型-prefab 字典
    private Dictionary<PropType, GameObject> PropTypeDic;

    [Header("Array")]
    //障碍贴图
    public Sprite[] barrierSpritesArr;

    [System.Serializable]
    public struct PropTypeForPre
    {
        public PropType type;
        public GameObject prefab;
    }
    public PropTypeForPre[] PropPrefabsArr;

    //记录上一个生成了障碍的行序，初始值给0
    private int preSpawnBarrierLine = 0;
    //物品生成概率表
    private int[] GoodsSpawnChance;
    //道具生成概率表
    private int[] PropSpawnChance;
    //障碍生成概率表
    private int[] BarrierSpawnChance;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        mTransform = transform;
    }

    public void Init()
    {
        BarriersList = new List<Barrier>();
        PropsList = new List<Prop>();
        //初始化道具生成概率数组
        //饼干，甜甜圈，棒棒糖，面包，巧克力的生成概率：
        PropSpawnChance = new int[] { 50, 12, 12, 12, 14 };
        //初始化障碍生成概率数组
        BarrierSpawnChance = new int[] { 50, 30, 10, 7, 3 };

        //初始化 道具 字典
        PropTypeDic = new Dictionary<PropType, GameObject>();
        for (int i = 0; i < PropPrefabsArr.Length; i++)
        {
            if (PropTypeDic.ContainsKey(PropPrefabsArr[i].type) == false)
            {
                PropTypeDic.Add(PropPrefabsArr[i].type, PropPrefabsArr[i].prefab);
            }
        }

        #region 按照每张地图生成游戏物品的方法，现在先注释掉
        //生成游戏物品
        for (int i = 0; i < spawnLineCount; i++)
        {
            //初始化物品生成概率数组(每一行的各物品生成概率相同)
            //不生成，生成障碍，生成道具的概率：40%，30%，30%
            GoodsSpawnChance = new int[] { 40, 30, 30 };

            float screenWidthWorldPos = Camera.main.orthographicSize * Screen.width / Screen.height;
            float distBetweenBlocks = screenWidthWorldPos / trackCount;

            //在中心生成 一行物品 的父物体
            Transform root = (new GameObject("root" + (i + 1))).transform;
            root.SetParent(mTransform);
            root.localPosition = new Vector2(0, i * lineSpace);
            
            //如果是最后一行，直接生成一整行
            if (i == spawnLineCount - 1)
            {
                SpawnBarrierFullline(distBetweenBlocks, root);
                return;
            }

            //根据上一个障碍的生成行数和当前行数改变概率
            //如果是在倒数第二行，障碍生成概率为0
            if (i == spawnLineCount - 2)
            {
                RefreshChanceArr(ref GoodsSpawnChance, 1, 0);
            }

            var spaceOfpreBarrierLine = Mathf.Abs(i - preSpawnBarrierLine);
            //如果是在上一个生成障碍的行的上面一行，障碍生成概率减为1/4
            if (spaceOfpreBarrierLine == 1)
            {
                RefreshChanceArr(ref GoodsSpawnChance, 1, GoodsSpawnChance[1] / 4);
            }
            //如果是在上一个生成障碍的行的上面两行，障碍生成概率减半
            else if (spaceOfpreBarrierLine == 2)
            {
                //RefreshChanceArr(ref GoodsSpawnChance, 1, GoodsSpawnChance[1] / 2);
            }
            
            //根据道路数量来生成每一行的物品个数
            var limit = trackCount / 2;
            //从左边开始生成
            for(int m = -limit; m <= limit; m++)
            {
                if ( (trackCount%2 == 0) && m == 0)
                {
                    //如果道路数量为偶数，且m为0，则不在该节点进行生成操作
                    continue;
                }
                else
                {
                    //随机物品生成类型，0：不生成， 1：障碍， 2：道具
                    int type = GetRandomType(GoodsSpawnChance);

                    //计算生成位置
                    float posX;
                    if(trackCount%2 == 0)
                    {
                        posX = m * 2 * distBetweenBlocks + (Mathf.Sign(m) * -distBetweenBlocks);
                    }
                    else
                    {
                        posX = m * 2 * distBetweenBlocks;
                    }
                    var pos = new Vector2(posX, 0);

                    //根据随机到的不同类型，进行操作
                    if(type == 0)
                    {
                        //如果随机到0，则结束该次的物品生成
                        continue;
                    }
                    else if(type == 1)
                    {
                        SpawnBarrier(pos, root, false);
                        preSpawnBarrierLine = i;
                        //同一行，生成一次障碍后，障碍生成概率减半
                        RefreshChanceArr(ref GoodsSpawnChance, 1, GoodsSpawnChance[1] / 2);
                    }
                    else
                    {
                        if (currentPropCount < maxPropCount)
                        {
                            //其它表示道具
                            SpawnProp(pos, root);
                            //同一行，生成一次道具后，道具生成概率减半
                            RefreshChanceArr(ref GoodsSpawnChance, 2, GoodsSpawnChance[2] / 2);
                            //当前道具数量增加
                            currentPropCount++;
                        }
                    }
                }
            }

            //生成栏杆（主要点在于计算栏杆的位置
            //可放置栏杆的点一共有trankCount + 1个，但是有边界存在，所以第一个和最后一个位置不能放置栏杆

        }
        #endregion
    }

    int GetRandomType(int[] chanceArr)
    {
        //chanceList对应序号存储各可选类型的出现概率
        int total = 0;
        for (int i = 0; i < chanceArr.Length; i++)
        {
            total += chanceArr[i];
        }

        int rad = Random.Range(0, total);
        for (int i = 0; i < chanceArr.Length; i++)
        {
            if (rad < chanceArr[i])
            {
                return i;
            }
            else
            {
                rad -= chanceArr[i];
            }
        }
        return chanceArr.Length - 1;
    }

    void RefreshChanceArr(ref int[] chanceArr, int index, int cgValue)
    {
        chanceArr[0] += chanceArr[index] - cgValue;
        chanceArr[index] = cgValue;
    }

    //生成障碍
    public void SpawnBarrier(Vector2 _pos, Transform _parent, bool isFullLine)
    {
        //随机障碍类型
        int type = GetRandomType(BarrierSpawnChance);
        int _point;
        if (isFullLine)
        {
            //当是生成整排障碍的情况下，障碍的分数需小于主角当前元件个数
            _point = Random.Range(1, player.GetCellCount() + 1);
        }
        else
        {
            //根据类型计算障碍的分数区间
            int minPoint = (type * 10) > 0 ? (type * 10) : (type * 10) + 1;
            int maxPoint = (type * 10) + 9;
            _point = Random.Range(minPoint, maxPoint);
        }

        //生成障碍，设置位置
        Transform _b = Instantiate(barrierPre, _parent).transform;
        _b.localPosition = _pos;

        //初始化障碍图片，分数
        var _bScript = _b.GetComponentInChildren<Barrier>();
        //_bScript.Init(barrierSpritesArr[type], _point);

        BarriersList.Add(_bScript);
    }

    //生成一整行障碍
    public void SpawnBarrierFullline(float distBetweenBlocks, Transform root)
    {
        Debug.Log("-------------------- 生成一整行障碍 --------------------");
        //根据道路数量来生成每一行的物品个数
        var limit = trackCount / 2;
        //随机一条道路生成生命值必定小于主角当前生命值的障碍
        int mustLessIndex = Random.Range(-limit, limit + 1);
        //从左边开始生成
        for (int m = -limit; m <= limit; m++)
        {
            if ((trackCount % 2 == 0) && m == 0)
            {
                //如果道路数量为偶数，且m为0，则不在该节点进行生成操作
                while (mustLessIndex == 0)
                {
                    mustLessIndex = Random.Range(-limit, limit + 1);
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
                    SpawnBarrier(pos, root, true);

                }
                else
                {
                    SpawnBarrier(pos, root, false);
                }
            }
        }
    }

    //生成道具
    public void SpawnProp(Vector2 _pos, Transform _parent)
    {
        //随机道具类型
        int type = GetRandomType(PropSpawnChance);

        //生成道具，设置位置
        Transform _p = Instantiate(PropTypeDic[(PropType)type], _parent).transform;
        _p.localPosition = _pos;

        //初始化道具
        var _pScript = _p.GetComponent<Prop>();
        _pScript.Init();

        PropsList.Add(_pScript);
    }

    //生成栏杆
    public void SpawnRailing(Vector2 _pos, Transform _parent)
    {
        Transform _r = Instantiate(railPre, _parent).transform;
        _r.localPosition = _pos;
    }

    //销毁一张地图
    public void OnDestroy()
    {
        //调用障碍和道具格子身上的销毁函数
        foreach (Barrier barrier in BarriersList)
        {

        }

        foreach (Prop porp in PropsList)
        {

        }

        Destroy(gameObject);
    }
}
