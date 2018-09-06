using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrectPointByPlayerCell : MonoBehaviour {

    private Barrier target;
    private Player player;
    private bool isCorrect = false;

	public void Init(Barrier barrier)
    {
        target = barrier;
        player = FindObjectOfType<Player>();

        isCorrect = true;
    }
	
	void Update () {
        if (isCorrect)
        {
            if(player.GetCellCount() == 0)
            {
                Destroy(gameObject);
                return;
            }

            //如果该障碍的分数大于主角的元件数量，则修改其分数
            if (target.Point > player.GetCellCount())
            {
                int newPoint = Random.Range(1, player.GetCellCount());
                target.CorrectPoint(newPoint);
            }
        }
	}
}
