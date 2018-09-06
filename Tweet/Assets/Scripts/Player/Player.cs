using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/******************************************************
 * 主角的功能脚本
 ******************************************************/
public class Player : MonoBehaviour
{

    [Header("Cell")]
    public GameObject cellPre;                  //元件的prefab
    public List<Cell> cellList;                 //主角身后的元件列表
    public Vector2 cellBasePos;                 //第一个元件的位置
    public Vector2 cellSpaceOffest;               //每个元件之间的位置距离
    public int initialCellCount;                //初始的时候身后的元件个数
    protected int currentCellCount;             //当前元件个数

    [Header("Property")]
    public bool GodMode;                        //是否处于无敌状态
    public float maxEnergy;
    public float Energy { get; set; }
    public float sprintDuration;
    public float sprintSpeed;
    private bool isEnergyGrouping = true;

    public Transform propNumPoint;              //数字文本的对焦中心
    public Transform firePoint;                 //远程攻击开火点

    protected float magentDuartionRate = 0;     //影响磁铁持续时间的比率值
    protected float shieldDuartionRate = 0;     //影响护盾持续时间的比率值
    protected float sprintDuartionRate = 0;     //影响冲刺持续时间的比率值
    protected float skillDurationRate = 0;      //影响技能开启时间的比率值

    [Header("Attack")]
    public int damage;                          //攻击力
    public float attackSpaceTime;               //攻击间隔时间
    public int maxHitCount;                     //可同时碰撞障碍的最大个数
    protected int currentHitCount;

    public enum SkillType                       //主角的攻击技能种类
    {
        Melee,      //近战
        Range,      //远程
        All,        //全能
        None
    }
    public SkillType skillType;

    public GameObject bulletPrefab;             //子弹prefab
    public float rangeSkillCoolTime;            //远程攻击的冷却时间
    private float timer;

    [Header("Move")]
    public float moveXSpeed;                    //主角左右移动的速度
    public float moveYSpeed;                    //主角前后移动的速度
    public float accelerationTime = 0.1f;       //水平加速时间

    protected float boderOffest;                //左右移动的范围
    protected float minMoveX;
    protected float maxMoveX;

    protected float initialMoveYSpeed;          //主角初始速度（y轴方向）
    protected Vector2 input;                    //记录玩家输入
    protected Vector2 velocity;                 //主角移动速度
    protected float velocityXSmoothing;         //水平速度缓冲参数

    #region Component
    [Header("Component")]
    public GameObject Magnet;                   //磁铁组件
    public GameObject Shield;                   //护盾组件
    public GameObject Sprint;                   //冲刺组件
    public GameObject Skill;                    //技能组件
    [HideInInspector]
    public Transform mTransform;
    public BoxCollider2D pCollider;             //主角用于移动判断碰撞体积
    public BoxCollider2D aCollider;             //主角用于攻击判断的碰撞体积
    [HideInInspector]
    public MoveController controller;           //移动控制器

    protected PropNum cellNum;                  //数字文本
    #endregion

    public bool isPlaying { set; get; }         //主角是否激活
    public bool isSkill { set; get; }           //是否启用攻击技能
    public bool isHitting { set; get; }         //是否处于撞击状态
    public bool isSprint { set; get; }          //是否处于冲刺状态
    public bool isShield { set; get; }          //是否处于护盾状态

    protected void Awake()
    {
        controller = GetComponent<MoveController>();
        mTransform = transform;
    }

    protected virtual void Start()
    {

    }

    //初始化主角
    public virtual void Init()
    {
        Energy = 0;

        //记录初始速度
        initialMoveYSpeed = moveYSpeed;
        //计算主角移动的最小最大范围
        //minMoveX = Camera.main.ScreenToWorldPoint(new Vector3(boderOffest, 0, 0)).x;
        //maxMoveX = Camera.main.orthographicSize * Screen.width / Screen.height;

        //初始化可同时碰撞个数
        currentHitCount = maxHitCount;

        //生成一个PropNum，显示主角的元件数量
        cellNum = GameManager.Instance.CreatePropNum(propNumPoint, Vector3.zero, initialCellCount);

        //初始化主角身后的元件
        if (initialCellCount > 0)
        {
            AddCell(initialCellCount);
        }
        currentCellCount = initialCellCount;

        //激活被动技能
        PassiveSkill();
    }

    //在Update函数中进行速度的更新
    protected virtual void Update()
    {
        if (isPlaying)
        {
            //检测玩家输入
            HandleInput();

            //根据玩家输入input的值，设置主角的移动速度
            float targetVelocityX = input.x * moveXSpeed;
            //一个短暂的加速过程
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTime);
            //暂时没有垂直方向的移动
            velocity.y = moveYSpeed;

            //检测主角的能量变化
            HandleEnergy();

            //判断是否启用远程攻击技能
            if (isSkill && (skillType == SkillType.Range || skillType == SkillType.All))
            {
                RangeSkill();
            }
        }
    }

    //在LateUpdate中实现主角的移动
    protected virtual void LateUpdate()
    {
        if (isPlaying)
        {
            controller.Move(velocity * Time.deltaTime, input);
        }
    }

    //检测玩家输入
    protected virtual void HandleInput()
    {
        if (Input.GetMouseButton(0))
        {
            //根据鼠标/手指按下移动的距离 计算 主角移动的距离
            var inputX = Input.GetAxis("Mouse X");
            if (inputX < 0)
            {
                MoveLeft();
            }
            else if (inputX > 0)
            {
                MoveRight();
            }
            else
            {
                StopMove();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopMove();
        }
    }

    //检测主角能量增长
    protected virtual void HandleEnergy()
    {
        if (isEnergyGrouping)
        {
            Energy += Time.deltaTime;
            if (Energy >= maxEnergy)
            {
                Debug.Log("能量条蓄力完成");
                //主角开启冲刺状态
                OpenSprint(sprintDuration, sprintSpeed);
                isEnergyGrouping = false;
                //能量条归零的动画，动画完成后再次开启能量的增长
                DOTween.To(() => Energy, r => Energy = r, 0, sprintDuration * (1 + sprintDuartionRate)).OnComplete(() =>
                {
                    Debug.Log("能量条清零完成");
                    isEnergyGrouping = true;
                });
            }
        }
    }

    //设置移动方向，左
    protected void MoveLeft()
    {
        input = new Vector2(-1, 0);
    }
    //设置移动方向，右
    protected void MoveRight()
    {
        input = new Vector2(1, 0);
    }
    //停止移动
    protected void StopMove()
    {
        input = Vector2.zero;
    }

    //设置主角的前进速度
    public void SetSpeed(float _speed)
    {
        moveYSpeed = _speed;
    }
    //重置主角的前进速度
    public void ResetSpeed()
    {
        moveYSpeed = initialMoveYSpeed;
    }

    //开启磁铁功能
    public void OpenMagnet(float _duration)
    {
        var _t = _duration * (1 + magentDuartionRate);
        Magnet.GetComponent<Magnet>().Open(_t);
    }

    //开启护盾
    public void OpenShield(float _duration)
    {
        var _t = _duration * (1 + shieldDuartionRate);
        Shield.GetComponent<Shield>().Open(_t);
        isShield = true;
    }
    //关闭护盾
    public void CloseShield()
    {
        Shield.GetComponent<Shield>().Close();
        isShield = false;
    }
    //削弱护盾韧性
    public void WeakenShield(int _strength)
    {
        Shield.GetComponent<Shield>().WeakenShield(_strength);
    }

    //开启冲刺
    public void OpenSprint(float _duration, float _speed)
    {
        var _t = _duration * (1 + sprintDuartionRate);
        Sprint.GetComponent<Sprint>().Open(_t);
        SetSpeed(_speed);
        OpenGodMode();
        isSprint = true;
    }
    //关闭冲刺
    public void CloseSprint()
    {
        ResetSpeed();
        CloseGodMode();
        ResetCollision();
        isSprint = false;
    }

    //开启技能攻击
    public void OpenSkill(float _duration)
    {
        Debug.Log("----------------------- 攻击技能启用 ------------------------");
        Skill.GetComponent<Skill>().Open(_duration);
        isSkill = true;
    }
    public void CloseSkill()
    {
        Debug.Log("----------------------- 攻击技能关闭 ------------------------");
        isSkill = false;
    }

    //进入无敌状态
    public void OpenGodMode()
    {
        GodMode = true;
    }
    //退出无敌状态
    public void CloseGodMode()
    {
        GodMode = false;
    }

    //开始碰撞
    public bool OpenCollision()
    {
        //当还有碰撞次数，才能进行碰撞
        if (currentHitCount > 0)
        {
            currentHitCount--;
            //Debug.Log("当前剩余可碰撞个数：" + currentHitCount);
            isHitting = true;
            return true;
        }
        return false;
    }
    //结束碰撞
    public void CloseCollision()
    {
        if (isHitting)
        {
            currentHitCount++;
            //Debug.Log("当前剩余可碰撞个数：" + currentHitCount);
            isHitting = false;
        }
    }
    //重置碰撞
    public void ResetCollision()
    {
        currentHitCount = maxHitCount;
        isHitting = false;
    }

    //增加指定个数的元件
    public void AddCell(int _num)
    {
        for (int i = 0; i < _num; i++)
        {
            //生成一个元件
            var newCell = Instantiate(cellPre, mTransform.parent).GetComponent<Cell>();
            //根据当前元件在列表中位置，设置元件的位置  或  用元件列表最后一个元件的位置+距离得到新元件的位置
            if (cellList.Count > 0)
            {
                var previous = cellList[cellList.Count - 1].transform.localPosition;
                var posY = previous.y + cellSpaceOffest.y;
                newCell.transform.localPosition = new Vector2(previous.x, posY);
            }
            else
            {
                var posY = mTransform.localPosition.y + cellBasePos.y;
                var posX = mTransform.localPosition.x + cellBasePos.x;
                newCell.transform.localPosition = new Vector2(posX, posY);
            }

            //初始化该元件,如果是第一元件，其跟随目标为主角，否则为其前一个元件
            if (cellList.Count == 0)
            {
                newCell.Init(mTransform, cellBasePos);
            }
            else if (cellList.Count > 0)
            {
                newCell.Init(cellList[cellList.Count - 1].transform, cellSpaceOffest);
            }
            //将新元件加入列表
            cellList.Add(newCell);
            //更新当前元件个数
            currentCellCount++;
            //更新文本显示
            cellNum.RefreshNum(currentCellCount);
        }
    }

    //删除指定个数的元件
    public void DelCell(int _num)
    {
        if (_num > cellList.Count)
        {
            Debug.LogError("------------- 指定要删除的元件个数不正确 -------------");
            return;
        }

        for (int i = 0; i < cellList.Count; i++)
        {
            RemoveCell();
        }
    }

    //删除第一个元件（返回true表示主角存活，返回false表示主角死亡
    public bool RemoveCell()
    {
        if (GodMode)
        {
            return true;
        }

        //更新当前元件个数
        currentCellCount--;
        //死亡检测
        if (currentCellCount < 0)
        {
            Dead();
            return false;
        }
        //更新文本显示
        cellNum.RefreshNum(currentCellCount);
        /*
        //删除最后一个元件
        var delCell = cellList[cellList.Count - 1];
        Destroy(delCell.gameObject);
        //从列表中移除最后一个元件
        cellList.RemoveAt(cellList.Count - 1);
        */
        //删除第一个元件
        var delCell = cellList[0];
        Destroy(delCell.gameObject);
        //从列表中移除第一个元件
        cellList.RemoveAt(0);
        if (cellList.Count > 0)
        {
            //刷新原先第二个元件的数据
            cellList[0].Refresh(mTransform, cellBasePos);
        }
        return true;
    }

    //主角死亡函数
    public void Dead()
    {
        isPlaying = false;
        //关闭物理效果
        controller.HandlePhysic = false;
        //关闭主角的碰撞体积
        mTransform.GetComponent<BoxCollider2D>().enabled = false;
        //关闭主角的所有协程
        StopAllCoroutines();
        Debug.Log("------------- 游戏结束 -------------");

        GameManager.Instance.GameFinish();
    }

    //获取主角当前的元件个数
    public int GetCellCount()
    {
        return currentCellCount;
    }

    //主角攻击函数
    protected void Attack(Barrier barrier)
    {
        if (OpenCollision())
        {
            if (isShield)
            {
                //如果加持了护盾，将一排敌人全部销毁
                Transform line = barrier.transform.parent;
                Barrier[] lineBarriers = line.GetComponentsInChildren<Barrier>();
                foreach (Barrier similar in lineBarriers)
                {
                    //安全检测，确认得到的障碍与被撞击的障碍处于同一行
                    if (similar.Y == barrier.Y)
                    {
                        similar.OnExplode();
                    }
                }
                //撞击后减少护盾韧性/关闭护盾
                CloseShield();
            }
            else if (isSprint && isShield == false)
            {
                //如果在冲刺状态，直接撞毁敌人
                barrier.OnExplode();
            }
            else
            {
                StartCoroutine(AttackCor(barrier));
            }
        }
    }

    //主角攻击协程
    protected IEnumerator AttackCor(Barrier barrier)
    {
        //移除元件
        if (RemoveCell())
        {
            //调用障碍的被伤害函数
            barrier.OnDamage(damage, gameObject);
            //判断是否启用技能
            if (isSkill && (skillType == SkillType.Melee || skillType == SkillType.All))
            {
                MeleeSkill(barrier);
            }

            yield return new WaitForSeconds(attackSpaceTime);

            if (barrier.Point > 0)
            {
                StartCoroutine(AttackCor(barrier));
            }
        }
    }

    //近战技能（不同角色根据攻击种类不同，需重写该函数
    protected virtual void MeleeSkill(Barrier victim)
    {
        //测试用近战技能，同时攻击对象的左右障碍
        Transform line = victim.transform.parent;
        Barrier[] lineBarriers = line.GetComponentsInChildren<Barrier>();
        foreach (Barrier implicate in lineBarriers)
        {
            //安全检测，确认得到的障碍与被撞击的障碍处于同一行
            if (implicate.Y == victim.Y)
            {
                //判断得到的障碍与被撞击的障碍是否相差一格或两格（这是基于当前4根跑道的操作，如果跑道数量变化，则不可用
                var dist = Mathf.Abs(implicate.X - victim.X);
                if (dist == 1 || dist == 2)
                {
                    //调用该障碍的被伤害函数
                    implicate.OnDamage(damage, victim.gameObject);
                }
            }
        }
    }

    //远程技能（不同角色根据攻击种类不同，需重写该函数
    protected virtual void RangeSkill()
    {
        //测试用远程技能，发射子弹进行攻击
        timer += Time.deltaTime;
        if (timer >= rangeSkillCoolTime)
        {
            timer = 0;
            //在开火点生成一颗子弹
            Bullet bullet = (Bullet)Instantiate(bulletPrefab, firePoint.position, Quaternion.identity).GetComponent(typeof(Bullet));
            bullet.Init(gameObject, Vector2.up, Vector2.up);
        }
    }

    //被动技能（不同角色被动技能不同，需重写该函数
    protected virtual void PassiveSkill()
    {
        Debug.Log("----------------------- 被动技能启用 ------------------------");
        //测试用被动，增加磁铁持续时间
        magentDuartionRate = 1;
    }

    //碰撞检测，只用来检测敌人
    protected void OnTriggerEnter2D(Collider2D other)
    {
        var barrier = other.GetComponent<Barrier>();
        if (barrier != null)
        {

            Attack(barrier);
        }
    }

    protected void OnTriggerStay2D(Collider2D other)
    {
        var barrier = other.GetComponent<Barrier>();
        if (barrier != null)
        {
            //
            if (isHitting == false)
            {
                Attack(barrier);
            }
        }
    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        var barrier = other.GetComponent<Barrier>();
        if (barrier != null)
        {
            CloseCollision();
            StopAllCoroutines();
        }
    }
}
