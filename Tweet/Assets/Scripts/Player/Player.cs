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
    public GameObject cellPre;                              //元件的prefab
    public List<Cell> cellList;                             //主角身后的元件列表
    public Vector2 cellBasePos = new Vector2(-0.2f, -1.6f); //第一个元件的位置
    public Vector2 cellSpaceOffest = new Vector2(0, -1);    //每个元件之间的位置距离
    public int initialCellCount = 3;                        //初始的时候身后的元件个数
    protected int currentCellCount;                         //当前元件个数

    [Header("Property")]
    public bool GodMode = false;                            //是否处于无敌状态
    public float maxEnergy = 20f;
    public float Energy { get; set; }
    public float sprintDuration = 5f;
    public float sprintSpeed = 4f;
    private bool isEnergyGrouping = true;

    public int ID;                              //角色id
    public int Level{ get; set; }               //角色等级

    protected Transform propNumPoint;           //数字文本的对焦中心
    protected Transform firePoint;              //远程攻击开火点

    protected float magentDuartionRate = 0;     //影响磁铁持续时间的比率值
    protected float shieldDuartionRate = 0;     //影响护盾持续时间的比率值
    protected float sprintDuartionRate = 0;     //影响冲刺持续时间的比率值
    protected float skillDurationRate = 0;      //影响技能开启时间的比率值

    [Header("Attack")]
    public int damage = 1;                      //攻击力
    public float attackSpaceTime = 0.3f;        //攻击间隔时间
    public int maxHitCount = 1;                 //可同时碰撞障碍的最大个数
    protected int currentHitCount;

    public enum SkillType                       //主角的攻击技能种类
    {
        Melee,      //近战
        Range,      //远程
        All,        //全能
        None
    }
    public SkillType skillType;

    public GameObject meleeAttackEffect;        //近战攻击特效
    public GameObject bulletPrefab;             //子弹prefab
    public float rangeSkillCoolTime = 0.5f;     //远程攻击的冷却时间
    protected float timer;                      //计时器

    [Header("Move")]
    public float moveXSpeed = 8f;               //主角左右移动的速度
    public float moveYSpeed = 2f;               //主角前后移动的速度
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
    protected GameObject Magnet;                   //磁铁组件
    protected GameObject Shield;                   //护盾组件
    protected GameObject Sprint;                   //冲刺组件
    protected GameObject Skill;                    //技能组件

    public BoxCollider2D pCollider;             //用于进行移动检测的碰撞体
    public BoxCollider2D aCollider;             //用于进行攻击检测的碰撞体
    [HideInInspector]
    public Transform mTransform;
    [HideInInspector]
    public MoveController controller;           //移动控制器
    [HideInInspector]
    public Animator anim;

    protected PropNum cellNum;                  //数字文本
    #endregion

    public bool isPlaying { set; get; }         //主角是否激活
    public bool isSkill { set; get; }           //是否启用攻击技能
    public bool isHitting { set; get; }         //是否处于撞击状态
    public bool isSprint { set; get; }          //是否处于冲刺状态
    public bool isShield { set; get; }          //是否处于护盾状态

    protected void Awake()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<MoveController>();
        mTransform = transform;
    }

    //初始化主角
    public virtual void Init()
    {
        /**************************** 初始化组件开始 ****************************/
        Magnet = mTransform.Find("Magent").gameObject;
        Shield = mTransform.Find("Shield").gameObject;
        Sprint = mTransform.Find("Sprint").gameObject;
        Skill = mTransform.Find("Skill").gameObject;
        propNumPoint = mTransform.Find("NumPoint");
        firePoint = mTransform.Find("FirePoint");

        Magnet.SetActive(false);
        Shield.SetActive(false);
        Sprint.SetActive(false);

        //生成一个PropNum，显示主角的元件数量
        cellNum = GameManager.Instance.CreatePropNum(propNumPoint, Vector3.zero, initialCellCount);
        /**************************** 初始化组件完毕 ****************************/

        /**************************** 初始化属性开始 ****************************/
        Energy = 0;

        //通过id获取当前角色的等级
        Level = PlayerPrefs.GetInt(GlobalData.PlayerLevel + ID, 0);

        timer = rangeSkillCoolTime;

        //记录初始速度
        initialMoveYSpeed = moveYSpeed;
        //计算主角移动的最小最大范围
        //minMoveX = Camera.main.ScreenToWorldPoint(new Vector3(boderOffest, 0, 0)).x;
        //maxMoveX = Camera.main.orthographicSize * Screen.width / Screen.height;

        //初始化可同时碰撞个数
        currentHitCount = maxHitCount;

        //初始化主角身后的元件
        if (initialCellCount > 0)
        {
            AddCell(initialCellCount);
        }
        currentCellCount = initialCellCount;

        //激活被动技能
        PassiveSkill();
        /**************************** 初始化属性完毕 ****************************/
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
    public virtual void OpenMagnet(float _duration)
    {
        var _t = _duration * (1 + magentDuartionRate);
        Magnet.GetComponent<Magnet>().Open(_t);
    }

    //开启护盾
    public virtual void OpenShield(float _duration)
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
    public virtual void OpenSprint(float _duration, float _speed)
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
    public virtual void OpenSkill(float _duration)
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
    public virtual void OpenGodMode()
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
        delCell.DestroyCell();
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
        //关闭动画
        anim.enabled = false;
        //关闭主角的碰撞体积
        var colliders = GetComponents<BoxCollider2D>();
        foreach(var collider in colliders)
        {
            collider.enabled = false;
        }
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
    protected virtual void Attack(Barrier barrier)
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
    }

    //远程技能（不同角色根据攻击种类不同，需重写该函数
    protected virtual void RangeSkill()
    {
    }
    //远程攻击命中敌人
    public virtual void OnRangeHitBarrier(Barrier barrier, int damage, GameObject instigator)
    {
        barrier.OnDamage(damage, gameObject);
    }

    //被动技能（不同角色被动技能不同，需重写该函数
    protected virtual void PassiveSkill()
    {
        Debug.Log("----------------------- 被动技能启用 ------------------------");
    }

    //碰撞检测，只用来检测敌人
    protected void OnTriggerEnter2D(Collider2D other)
    {
        var barrier = other.GetComponent<Barrier>();
        if (barrier != null)
        {
            if (OpenCollision())
            {
                Attack(barrier);
            }
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
                if (OpenCollision())
                {
                    Attack(barrier);
                }
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
