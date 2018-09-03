using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************
 * 移动功能控制器，要移动的物体需绑定该脚本
 ******************************************************/
public class MoveController : RaycastController
{
    public CollisionInfo collisions;            //记录碰撞信息的结构体

    [HideInInspector]
    public Vector2 playerInput;                 //记录玩家按键输入
    [HideInInspector]
    public bool HandlePhysic = true;            //记录是否开启物理效果，默认为开启

    public enum RayOriginType
    {
        Center,
        Edge
    }
    public RayOriginType rayOriginType;

    public override void Start()
    {
        base.Start();                           //调用父类的start函数
        collisions.faceDir = 1;                 //默认移动方向为右
    }

    //移动函数
    public void Move(Vector3 velocity, Vector2 input)
    {
        UpdateRaycastOrigins();                 //更新当前物体的射线起点
        collisions.Reset();                     //重置碰撞信息
        collisions.velocityOld = velocity;      //存储当前速度
        playerInput = input;                    //存储玩家输入

        //如果水平方向的速度不为0，则获取速度的方向
        if (velocity.x != 0)
        {
            collisions.faceDir = (int)Mathf.Sign(velocity.x);
        }

        //如果有物理效果,
        if (HandlePhysic)
        {
            //检测水平方向上的碰撞
            HorizontalCollisions(ref velocity);
            //检测垂直方向上的碰撞
            VerticalCollisions(ref velocity);
        }
        //移动主角
        transform.Translate(velocity, Space.World);
    }

    public void Move(Vector3 newPos)
    {
        UpdateRaycastOrigins();                 //更新当前物体的射线起点
        collisions.Reset();                     //重置碰撞信息

        //如果有水平方向的位置变化，则获取变化的方向
        if ((newPos.x - transform.position.x) != 0)
        {
            collisions.faceDir = (int)Mathf.Sign(newPos.x - transform.position.x);
        }

        var velocity = new Vector3(0.1f, 0, 0);
        //检测水平方向上的碰撞
        if(HorizontalCollisions(ref velocity))
        {
            newPos.x = transform.position.x;
        }

        transform.position = newPos;
    }

    //检测水平方向上的碰撞
    private bool HorizontalCollisions(ref Vector3 velocity)
    {
        //获取水平速度的方向
        float directionX = collisions.faceDir;
        //设置射线长度（射线很短，差不多是检测下一次移动位置的情况）
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        //如果速度大小小于修正参数
        if (Mathf.Abs(velocity.x) < skinWidth)
        {
            //rayLength = 5 * skinWidth;
        }

        //遍历物体水平射线，检测碰撞
        for (int i = 0; i < horizontalRayCount; i++)
        {
            //根据速度方向设置射线起点初值
            Vector2 rayOrigin = raycastOrigins.centerRight;
            if (rayOriginType == RayOriginType.Edge)
            {
                rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            }
            else if (rayOriginType == RayOriginType.Center)
            {
                rayOrigin = (directionX == -1) ? raycastOrigins.centerLeft : raycastOrigins.centerRight;
            }
            //根据i值设定第i根射线的起点
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            //发射射线，括号内属性为：起点，方向，长度，碰撞对象
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            //如果与碰撞对象发生碰撞
            if (hit)
            {
                //当检测到前面有阻挡，重新计算速度
                //由于射线长度很短，所以速度也会变成很小的值（差不多原地不动）
                velocity.x = (hit.distance - skinWidth) * directionX;
                rayLength = hit.distance;

                //如果速度方向左（-1），left为真；如果速度方向右（1），right为真
                collisions.left = directionX == -1;
                collisions.right = directionX == 1;

                return true;
            }
        }
        //没有碰撞
        return false;
    }

    //检测垂直方向上的碰撞
    private bool VerticalCollisions(ref Vector3 velocity)
    {
        //获取垂直速度的方向
        float directionY = Mathf.Sign(velocity.y);
        //计算垂直方向的射线长度（射线很短，为下一帧移动的距离）
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        //遍历垂直方向的射线
        for (int i = 0; i < verticalRayCount; i++)
        {
            //判断射线初始点
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            //设置4条射线各自的起点
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            //生成射线
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            //如果物体上升时上方有碰撞对象或下降时下方有碰撞对象
            if (hit)
            {
                //减少物体速度（反正在垂直运动时遇到障碍物则减少物体垂直速度）
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                //通过垂直速度的方向来记录物体上下的碰撞信息
                collisions.below = directionY == -1;
                collisions.above = directionY == 1;

                return true;
            }
        }
        //没有碰撞
        return false;
    }

    //用于碰撞信息的结构体
    public struct CollisionInfo
    {
        public bool above, below;               //上下是否有碰撞
        public bool left, right;                //左右是否有碰撞

        public Vector3 velocityOld;             //旧速度，记录从Player中传入的速度值
        public int faceDir;                     //朝向

        public void Reset()
        {
            above = below = false;
            left = right = false;
        }
    }
}
