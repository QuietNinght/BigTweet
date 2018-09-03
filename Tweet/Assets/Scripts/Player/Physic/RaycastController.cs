using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************
 * 射线控制器，提供了对射线属性的设置
 ******************************************************/
[RequireComponent(typeof(BoxCollider2D))]               //关联BoxCollider2D类
public class RaycastController : MonoBehaviour {

    //设置碰撞层
    public LayerMask collisionMask;

    //对射线长度的修正参数
    public const float skinWidth = 0.015f;
    //水平射线数量
    public int horizontalRayCount = 4;
    //垂直射线数量
    public int verticalRayCount = 4;                    

    [HideInInspector]
    //水平射线间的间距
    public float horizontalRaySpacing;                  
    [HideInInspector]
    //垂直射线间的间距
    public float verticalRaySpacing;

    [HideInInspector]
    //获取2D碰撞体
    public BoxCollider2D boxcollider;
    //射线起点结构体
    public RaycastOrigins raycastOrigins;

    public virtual void Awake()
    {
        //获取对象的碰撞体组件
        boxcollider = GetComponent<BoxCollider2D>();
    }

    public virtual void Start()
    {
        CalculateRaySpacing();
    }

    //设置物体的射线起点
    public void UpdateRaycastOrigins()
    {
        Bounds bounds = boxcollider.bounds;             //物体碰撞体的边框
        bounds.Expand(skinWidth * -1f);                  //通过提供的数值沿着每条边扩大边框

        //设置基于碰撞体的下左、下右、上左、上右四个射线起点
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        raycastOrigins.centerLeft = new Vector2(bounds.min.x, bounds.center.y);
        raycastOrigins.centerRight = new Vector2(bounds.max.x, bounds.center.y);
    }

    //计算水平或垂直方向上每条射线的间距
    public void CalculateRaySpacing()
    {
        Bounds bounds = boxcollider.bounds;
        bounds.Expand(skinWidth * -1f);

        //Mathf.Clamp(float:a,min:b,max:c)，限制a的值在min和max之间，小于min为min，大于max为max，否则为a
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 1, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 1, int.MaxValue);

        //计算间距
        if(horizontalRayCount == 1)
        {
            horizontalRaySpacing = 1;

        }else if(horizontalRayCount > 1)
        {
            horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        }
        if(verticalRayCount == 1)
        {
            verticalRaySpacing = 1;
        }else if(verticalRayCount > 1)
        {
            verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
        }
    }

    //存储射线起点的结构体
    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
        public Vector2 centerLeft, centerRight;
    }
}
