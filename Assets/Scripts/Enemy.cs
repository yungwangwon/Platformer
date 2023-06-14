using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator ani;
    SpriteRenderer spriteranderer;
    CapsuleCollider2D capcollider;
    public int nextMove;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        spriteranderer = GetComponent<SpriteRenderer>();
        capcollider = GetComponent<CapsuleCollider2D>();
        //딜레이 함수호출
        Invoke("Think", 5);
    }

    void FixedUpdate()
    {
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //레이캐스트 
        Vector2 front = new Vector2(rigid.position.x + nextMove, rigid.position.y);
        Debug.DrawRay(front, Vector3.down, new Color(255, 0, 0));// 레이캐스트 그리기
        RaycastHit2D rayhit = Physics2D.Raycast(front, Vector3.down, 1, LayerMask.GetMask("Floor")); ;
        if (rayhit.collider == null)
        {
            nextMove *= -1;
            spriteranderer.flipX = (nextMove == 1);
            CancelInvoke();
            Invoke("Think", Random.Range(2f, 5f));
        }
    }

    //몬스터 움직임 설정
    void Think()
	{
        nextMove = Random.Range(-1, 2);   //-1,0,1(방향)

        ani.SetInteger("runspeed", nextMove);

        //좌우대칭
        if (nextMove != 0)
            spriteranderer.flipX = (nextMove == 1);

        Invoke("Think",Random.Range(2f,5f));

    }

    //밟혔을때
    public void OnDamaged()
	{
        //색 변환 > 투명
        spriteranderer.color = new Color(1, 1, 1, 0.5f);
        //스프라이트플립
        spriteranderer.flipY = true;
        //충돌 삭제
        capcollider.enabled = false;
        //위로 뛰어오르게
        rigid.AddForce(Vector2.up * 3, ForceMode2D.Impulse);
        //오브젝트 비활성화
        Invoke("Killobject", 3);
    }

    void Killobject()
	{
        gameObject.SetActive(false);
    }

}
