using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager: MonoBehaviour
{
    public int totalscore;
    public int stagescore;
    public int stageindex;
    public int hp;
    public PlayerMove player;

	public void NextStage()
    {
        stageindex++;
        totalscore += stagescore;
        stagescore = 0;
    }

	void OnTriggerEnter2D(Collider2D collision)
	{
        //떨어졌을때
        if (collision.tag == "Player")
        {

            if (hp > 1)
            {
                collision.attachedRigidbody.velocity = Vector2.zero;
                collision.transform.position = new Vector3(-3, 3, 0);
            }

            HpDown();
        }
	}

    public void HpDown()
	{
        if (hp > 1)
            hp--;
        else
		{
            player.OnDie();
            Debug.Log("캐릭터가 사망하였습니다..");
        }
	}
}
