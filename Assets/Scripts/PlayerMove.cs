using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gamemanager;
    public float maxspeed;
    public float jumppower;
    Rigidbody2D rigid;
    Animator ani;
    SpriteRenderer spriterenderer;
    
    // Start is called before the first frame update
    void Awake()
    {
        maxspeed = 5.0f;
        jumppower = 10.0f;
        rigid = GetComponent<Rigidbody2D>();
        spriterenderer = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
    }

	void Update()
	{
        // 점프
        if(Input.GetButtonDown("Jump") && !ani.GetBool("isJump"))
		{
            rigid.AddForce(Vector2.up * jumppower, ForceMode2D.Impulse);
            ani.SetBool("isJump", true);    //점프 애니메이션
		}


        // 방향전환
        if (Input.GetButton("Horizontal"))
        {
            spriterenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        if (Input.GetButtonUp("Horizontal"))
		{
			// 멈출때 속력 줄이기 rigid.velocity.normalized < 단위벡터
			rigid.velocity = new Vector2(rigid.velocity.normalized.x * 1.0f, rigid.velocity.y);
		}

        //애니메이션 변환
        if (rigid.velocity.normalized.x == 0)
            ani.SetBool("isRun", false);
        else
            ani.SetBool("isRun", true);

    }

	// Update is called once per frame
	void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxspeed)    // 최고속력 설정
            rigid.velocity = new Vector2(maxspeed, rigid.velocity.y);
        else if (rigid.velocity.x < (maxspeed * (-1)))
            rigid.velocity = new Vector2((maxspeed * (-1)), rigid.velocity.y);

        // 레이캐스트 착지 (낙하중일때만)
        if (rigid.velocity.y < 0)
		{
            // 레이 그리기
            Debug.DrawRay(rigid.position, Vector3.down, new Color(255, 0, 0));// 레이캐스트 그리기
            
            RaycastHit2D rayhit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Floor")); ;
            if (rayhit.collider != null)
            {
                if (rayhit.distance < 0.5f)
                {
                    ani.SetBool("isJump", false);
                }
            }
        }

    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
        //아이템과 충돌
		if(collision.tag == "Item")
		{
            bool isBronze = collision.name.Contains("Bronze");
            bool isSilver = collision.name.Contains("Silver");
            bool isGold = collision.name.Contains("Gold");

            if(isBronze)
                gamemanager.stagescore += 100;
            if (isSilver)
                gamemanager.stagescore += 200;
            if (isGold)
                gamemanager.stagescore += 300;


            collision.gameObject.SetActive(false);
		}

        //깃발과 충돌
        if(collision.tag == "Finish")
		{
            gamemanager.NextStage();
            //다음 스테이지
        }

	}

	void OnCollisionEnter2D(Collision2D collision)
	{
        //enemy와 충돌
		if(collision.gameObject.tag == "Enemy")
		{
            //아래로 이동중 enemy보다 위에있다면
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
			{
                OnAttack(collision.gameObject);
                gamemanager.stagescore += 100;
			}
            else
                OnDamaged(collision.transform.position);
        }
	}

    void OnAttack(GameObject enemyobject)
    {
        //리액션
        rigid.AddForce(Vector2.up * 3, ForceMode2D.Impulse);
        //enemy처리
        Enemy enemy = enemyobject.GetComponent<Enemy>();
        enemy.OnDamaged();
    }

    //피해를 입었을때
    void OnDamaged(Vector2 collision)
    {
        //hp
        gamemanager.HpDown();
        //상태적용
        gameObject.layer = 8;
        //스프라이트 적용
        spriterenderer.color = new Color(1, 1, 1, 0.5f);//검은색, 투명도
        //힘 적용
        int dir = transform.position.x - collision.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dir, 1) * 3, ForceMode2D.Impulse);

        ani.SetTrigger("damaged");

        Invoke("OffDamaged", 2);

    }

    void OffDamaged()
    {
        //상태적용
        gameObject.layer = 7;
        //스프라이트 적용
        spriterenderer.color = new Color(1, 1, 1);//검은색, 투명도
    }

    public void OnDie()
    {
       
    }
}
