using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/******************************************************
 * 游戏物品生成器，负责生成障碍、道具、栏杆
 ******************************************************/
public class SpawnManager : MonoBehaviour {

    private static SpawnManager instance;
    public static SpawnManager Instance
    {
        get { return instance; }
    }

    [Header("Property")]
    Player player;                          //主角
    Transform mTransform;

    public int trackCount;                  //地图上的道路数量
    public float lineSpace;                 //行与行的间距
    private int currentLine;                //当前物品行数
    private float gameTimer;                //记录游戏持续时间
    private bool isRefreshRate = true;      //标记是否更新概率数组
    private int refreshRateRuncount = 1;    //更新概率数组操作的执行次数

    //第一种方式，按照时间进行生成
    private float spawnIntervalTime;        //每一行游戏物品的生成时间间隔（根据主角的速度进行减少，1/speed）
    private float propTimer;
    //第二种方式，按照主角的移动距离进行生成
    public float initialCheckShift;         //用来进行主角移动检测的位移量
    private Vector2 previousPlayerPos;      //上一个检测点记录的主角位置

    public bool isSpawnBarrier = true;
    public bool isSpawnProp = true;

    [Header("Prop")]
    public float spawnPropLimitTime;        //每多少秒内限制道具的刷出数量
    public int maxPropCount = 5;            //一张地图上最多道具数量
    private int currentPropCount = 0;       //当前道具数量
    private int preSpawnPropLine = 0;       //上一个生成了道具的行序，初始值给0

    //游戏道具类型
    public enum PropType
    {
        Doughunt,       //甜甜圈，护盾 +　爆炸（销毁整行障碍）
        Chocolate,      //巧克力，冲刺
        Lollipop,       //棒棒糖，开启主角的攻击
        Bread,          //U型面包，磁铁功能
        Cooky,          //姜饼饼干，增加主角元件
        Count,          //其它      
    }
    //道具 类型-prefab 字典
    private Dictionary<PropType, GameObject> PropTypeDic;

    [System.Serializable]
    public struct PropTypeForPre
    {
        public PropType type;
        public GameObject prefab;
    }
    public PropTypeForPre[] PropPrefabsArr;

    [Header("Barrier")]
    public GameObject barrierPrefab;            //障碍的prefab
    private int preSpawnBarrierLine = 0;        //记录上一个生成了障碍的行序，初始值给0
    private int nextFullBarrierLine;            //下一个生成整行障碍物的行数
    public int minFullSpace;                    //生成整行障碍的最小、最大间隔行数
    public int maxFullSpace;    
    public Sprite[] barrierSpritesArr;          //障碍贴图

    [Header("Railing")]
    public int railSpawnRate;                   //栏杆生成概率
    //记录可生成栏杆的位置点 的字典（key为列数，value为行数）
    //只有 该列的行数 > 字典中对应列的value值，才能在该位置生成栏杆
    private Dictionary<int, int> NextSpawnRailPosList;
    public GameObject[] railPrefabsArr;         //栏杆的prefab数组

    //游戏物品类型
    public enum GoodsType
    {
        Barrier,        //障碍，阻挡并消除主角元件
        Prop,           //道具
        Count,          //其它      
    }

    private int[] GoodsSpawnRateArr;        //物品生成概率表
    private int[] PropSpawnRateArr;         //各类型道具生成概率表
    private int[] BarrierSpawnRateArr;      //各类型障碍生成概率表
    private int[] RailSpawnRateArr;         //各类型栏杆生成概率表

    void Awake()
    {
        player = FindObjectOfType<Player>();

        mTransform = transform;

        instance = this;
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        if (isRefreshRate)
        {
            switch ((int)(Time.time - gameTimer))
            {
                case 20:
                    GoodsSpawnRateArr = GlobalData.GoodsRateListByLevel[1];
                    PropSpawnRateArr = GlobalData.PropRateListByLevel[1];
                    BarrierSpawnRateArr = GlobalData.BarrierRateListByLevel[1];
                    break;
                case 30:
                    GoodsSpawnRateArr = GlobalData.GoodsRateListByLevel[2];
                    PropSpawnRateArr = GlobalData.PropRateListByLevel[2];
                    BarrierSpawnRateArr = GlobalData.BarrierRateListByLevel[2];
                    break;
                case 40:
                    GoodsSpawnRateArr = GlobalData.GoodsRateListByLevel[3];
                    isRefreshRate = false;
                    break;
                default:
                    break;
            }
        }

        //Debug.Log("物品生成概率数组中障碍的生成概率：" + GoodsSpawnRateArr[1]);

        if(Time.time - propTimer >= spawnPropLimitTime)
        {
            currentPropCount = 0;
            propTimer = Time.time;
        }

        HandleSpawn();
    }

    public void Init()
    {
        gameTimer = Time.time;
        propTimer = Time.time;
        previousPlayerPos = player.mTransform.position;

        nextFullBarrierLine = UnityEngine.Random.Range(minFullSpace, maxFullSpace + 1);

        //初始化物品生成概率数组(每一行的各物品生成概率相同)
        //不生成，生成障碍，生成道具的概率：40%，30%，30%
        GoodsSpawnRateArr = GlobalData.GoodsRateListByLevel[0];
        //初始化道具生成概率数组
        //饼干，甜甜圈，棒棒糖，面包，巧克力的生成概率：
        PropSpawnRateArr = GlobalData.PropRateListByLevel[0];
        //初始化障碍生成概率数组
        //障碍1,2,3,4,5的生成概率
        BarrierSpawnRateArr = GlobalData.BarrierRateListByLevel[0];
        //初始化栏杆生成概率数组
        RailSpawnRateArr = GlobalData.RailRateListByLevel[0];



        //初始化 道具 字典
        PropTypeDic = new Dictionary<PropType, GameObject>();
        for(int i = 0; i < PropPrefabsArr.Length; i++)
        {
            if(PropTypeDic.ContainsKey(PropPrefabsArr[i].type) == false)
            {
                PropTypeDic.Add(PropPrefabsArr[i].type, PropPrefabsArr[i].prefab);
            }
        }

        //初始化下一个可生成栏杆的位置点列表
        NextSpawnRailPosList = new Dictionary<int, int>();
        //根据道路边数量来计算并限制可生成栏杆的列数（最边上的两列不能生成栏杆
        var railLimit = (trackCount + 1) / 2 - 1;
        for(int i = -railLimit; i <= railLimit; i++)
        {
            NextSpawnRailPosList.Add(i, 0);
        }
    }

    //检测进行游戏物体的生成
    private void HandleSpawn()
    {
        if (player.mTransform.position.y - previousPlayerPos.y >= initialCheckShift)
        {
            if (currentLine >= nextFullBarrierLine)
            {
                if (player.GetCellCount() == 0)
                {
                    //生成一行物品
                    SpawnSingleLine();
                }
                else
                {
                    if (isSpawnBarrier)
                    {
                        //生成一行障碍
                        SpawnBarrierFullline();
                    }
                }
                //更新下一个生成整行障碍的行数
                nextFullBarrierLine = nextFullBarrierLine + UnityEngine.Random.Range(minFullSpace, maxFullSpace + 1);
            }
            else
            {
                //生成一行物品
                SpawnSingleLine();
            }

            currentLine++;
            previousPlayerPos = player.mTransform.position;
        }
    }

    private void RefreshRatearr()
    {
        switch (refreshRateRuncount)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            default:
                break;
        }
        refreshRateRuncount++;
    }

    //生成一排游戏物品
    private void SpawnSingleLine()
    {
        //开始生成时，获取物品生成概率数组(每一行的各物品生成概率相同)
        int[] goodsSpawnRateArr = (int[])GoodsSpawnRateArr.Clone();

        float screenWidthWorldPos = Camera.main.orthographicSize * Screen.width / Screen.height;
        float distBetweenBlocks = screenWidthWorldPos / trackCount;

        //在中心生成 一行物品 的父物体
        Transform line = (new GameObject("Line" + (currentLine + 1))).transform;
        line.SetParent(mTransform);
        line.localPosition = new Vector2(0, currentLine * lineSpace);

        //根据上一个障碍或道具的生成行数和当前行数改变概率
        //如果是在下一个生成整行障碍的行数的前一行，障碍生成概率为0
        if (currentLine == nextFullBarrierLine - 1)
        {
            Utils.RefreshRateArr(ref goodsSpawnRateArr, 1, 0);
        }

        if (isSpawnBarrier)
        {
            //当前行与上一个生成了障碍的行的距离
            var spaceOfpreBarrierLine = Mathf.Abs(currentLine - preSpawnBarrierLine);
            //如果是在上一个生成障碍的行的上面一行，障碍生成概率减为1/6
            if (spaceOfpreBarrierLine == 1)
            {
                Utils.RefreshRateArr(ref goodsSpawnRateArr, 1, goodsSpawnRateArr[1] / 4);
            }
            //如果是在上一个生成障碍的行的上面两行，障碍生成概率减半
            else if (spaceOfpreBarrierLine == 2)
            {
                Utils.RefreshRateArr(ref goodsSpawnRateArr, 1, goodsSpawnRateArr[1] / 2);
            }
        }
        if (isSpawnProp)
        {
            //当前行与上一个生成了道具的行的距离
            var spaceOfprePropLine = Mathf.Abs(currentLine - preSpawnPropLine);
            if (spaceOfprePropLine == 1)
            {
                Utils.RefreshRateArr(ref goodsSpawnRateArr, 2, goodsSpawnRateArr[2] / 8);
            }
            else if (spaceOfprePropLine == 2)
            {
                Utils.RefreshRateArr(ref goodsSpawnRateArr, 2, goodsSpawnRateArr[2] / 6);
            }
            else if (spaceOfprePropLine == 3)
            {
                Utils.RefreshRateArr(ref goodsSpawnRateArr, 2, goodsSpawnRateArr[2] / 4);
            }
            else if (spaceOfprePropLine == 4)
            {
                Utils.RefreshRateArr(ref goodsSpawnRateArr, 2, goodsSpawnRateArr[2] / 2);
            }
        }

        //根据道路数量来生成每一行的物品个数
        var goodLimit = trackCount / 2;
        //从左边开始生成
        for (int m = -goodLimit; m <= goodLimit; m++)
        {
            if ((trackCount % 2 == 0) && m == 0)
            {
                //如果道路数量为偶数，且m为0，则不在该节点进行生成操作
                continue;
            }
            else
            {
                //随机物品生成类型，0：不生成， 1：障碍， 2：道具
                int type = Utils.GetRandomType(goodsSpawnRateArr);

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
                    if (isSpawnBarrier)
                    {
                        SpawnSingleBarrier(m, currentLine, pos, line, false);
                        //记录当前行为生成了障碍的行
                        preSpawnBarrierLine = currentLine;
                        //同一行，生成一次障碍后，障碍生成概率减半
                        Utils.RefreshRateArr(ref goodsSpawnRateArr, 1, goodsSpawnRateArr[1] / 2);
                    }
                }
                else
                {
                    if (isSpawnProp)
                    {
                        if (currentPropCount < maxPropCount)
                        {
                            //其它表示道具
                            SpawnSingleProp(pos, line);
                            //同一行，生成一次道具后，道具生成概率变为1/8
                            Utils.RefreshRateArr(ref goodsSpawnRateArr, 2, goodsSpawnRateArr[2] / 8);
                            //当前道具数量增加
                            currentPropCount++;
                            //记录当前行为生成了道具的行
                            preSpawnPropLine = currentLine;
                        }
                    }
                }
            }
        }

        //根据道路边数量来计算并限制可生成栏杆的列数（最边上的两列不能生成栏杆
        var railLimit = (trackCount + 1) / 2 - 1;
        for(int n = -railLimit; n <= railLimit; n++)
        {
            if( trackCount%2 == 1 && n == 0)
            {
                //如果道路数量为奇数，且n为0，则不在该节点进行生成操作
                continue;
            }
            else
            {
                int currentSpawnRate = railSpawnRate;
                if(currentLine == preSpawnBarrierLine)
                {
                    //如果是在障碍边，栏杆生成概率翻倍
                    currentSpawnRate = 2 * currentSpawnRate / 3;
                }
                int isSpawn = UnityEngine.Random.Range(0, 100);
                if (isSpawn < currentSpawnRate)
                {

                    //如果当前列数的当前行数 >= 可生成位置字典中的对应列数的行数，才可以生成栏杆
                    if (currentLine >= NextSpawnRailPosList[n])
                    {

                        //计算生成位置
                        float posX;
                        if (trackCount % 2 == 1)
                        {
                            posX = n * 2 * distBetweenBlocks + (Mathf.Sign(n) * -distBetweenBlocks);
                        }
                        else
                        {
                            posX = n * 2 * distBetweenBlocks;
                        }
                        var pos = new Vector2(posX, 0);

                        //随机栏杆长度
                        int railLength = Utils.GetRandomType(RailSpawnRateArr);

                        //生成栏杆
                        SpawnSingleRailing(pos, line, railLength);

                        //更新可生成位置字典
                        NextSpawnRailPosList[n] = currentLine + railLength + 1;
                    }
                }
            }
        }
    }

    //生成障碍
    public void SpawnSingleBarrier(int _x, int _y, Vector2 _pos, Transform _parent, bool isFullLine)
    {
        //随机障碍类型
        int type = Utils.GetRandomType(BarrierSpawnRateArr);
        int _point;
        if (isFullLine)
        {
            //当是生成整排障碍的情况下，障碍的分数需小于主角当前元件个数
            _point = UnityEngine.Random.Range(1, player.GetCellCount() + 1);
        }
        else
        {
            //根据类型计算障碍的分数区间
            int minPoint = (type * 10) > 0 ? (type * 10) : (type * 10) + 1;
            int maxPoint = (type * 10) + 9;
            _point = UnityEngine.Random.Range(minPoint, maxPoint);
        }

        //生成障碍，设置位置
        Transform _b = Instantiate(barrierPrefab, _parent).transform;
        _b.localPosition = _pos;

        //初始化障碍图片，分数
        var _bScript = _b.GetComponentInChildren<Barrier>();
        _bScript.Init(_x, _y, barrierSpritesArr[type], _point);
    }

    //生成一整行障碍
    public void SpawnBarrierFullline()
    {
        Debug.Log("-------------------- 生成一整行障碍 --------------------");
        float screenWidthWorldPos = Camera.main.orthographicSize * Screen.width / Screen.height;
        float distBetweenBlocks = screenWidthWorldPos / trackCount;

        //在中心生成 一行物品 的父物体
        Transform line = (new GameObject("Line" + (currentLine + 1))).transform;
        line.SetParent(mTransform);
        line.localPosition = new Vector2(0, currentLine * lineSpace);

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

                if(m == mustLessIndex)
                {
                    SpawnSingleBarrier(m, currentLine, pos, line, true);

                }
                else
                {
                    SpawnSingleBarrier(m, currentLine, pos, line, false);
                }
            }
        }

        //更新上一个生成了障碍的行
        preSpawnBarrierLine = currentLine;
    }

    //生成道具
    public void SpawnSingleProp(Vector2 _pos, Transform _parent)
    {
        //随机道具类型
        int type = Utils.GetRandomType(PropSpawnRateArr);

        //生成道具，设置位置
        Transform _p = Instantiate(PropTypeDic[(PropType)type], _parent).transform;
        _p.localPosition = _pos;

        //初始化道具
        var _pScript = _p.GetComponent<Prop>();
        _pScript.Init();
    }

    //生成栏杆
    public void SpawnSingleRailing(Vector2 _pos, Transform _parent, int _length)
    {
        Transform _r = Instantiate(railPrefabsArr[_length], _parent).transform;
        _r.localPosition = _pos;
    }
}
