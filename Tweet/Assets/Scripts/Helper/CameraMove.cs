using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************************************
 * 相机移动
 ******************************************************/
public class CameraMove : MonoBehaviour {


    public float verticalOffset;                            //垂直偏移量
    public float verticalSmoothTime;                        //垂直位移平滑参数
    public Vector2 focusAreaSize;                           //焦点区域大小
    public Player target;                                   //相机跟随目标

    private FocusArea focusArea;                            //焦点信息结构体
    private float smoothPositionY = 0.2f;                   //垂直位置变换平滑参数

    public bool isFollowing { get; set; }                   //检测是否开启跟随

    void Start () {
		if(target == null)
        {
            //FindObjectOfType 效率很低，据说一般用单例模式替代
            target = FindObjectOfType<Player>();
        }
        focusArea = new FocusArea(target.pCollider.bounds, focusAreaSize);
    }
	
	void LateUpdate () {
        if (!isFollowing)
        {
            return;
        }
        //更新焦点位置
        focusArea.Update(target.pCollider.bounds);
        //计算相机对焦位置
        Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;
        //从相机当前y轴位置到新的y轴位置给一个平滑变换
        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothPositionY, verticalSmoothTime);
        //改变相机位置
        transform.position = (Vector3)focusPosition + Vector3.forward * -10;
    }

    struct FocusArea
    {
        public Vector2 center;          //焦点中心
        public Vector2 velocity;        //焦点移动速度
        float left, right;              //焦点的左、右、上、下
        float top, bottom;

        //计算焦点位置
        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x;
            right = targetBounds.center.x;
            top = targetBounds.min.y + size.y;
            bottom = targetBounds.min.y;

            velocity = Vector2.zero;
            center = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        //更新焦点位置
        public void Update(Bounds targetBounds)
        {
            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }
            bottom += shiftY;
            top += shiftY;

            //由于不需要水平方向的跟随，将水平方向的位置变化忽略
            center = new Vector2(0, (top + bottom) / 2);
            velocity = new Vector2(0, shiftY);
        }
    }
}
