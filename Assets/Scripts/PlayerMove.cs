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
        // ����
        if(Input.GetButtonDown("Jump") && !ani.GetBool("isJump"))
		{
            rigid.AddForce(Vector2.up * jumppower, ForceMode2D.Impulse);
            ani.SetBool("isJump", true);    //���� �ִϸ��̼�
		}


        // ������ȯ
        if (Input.GetButton("Horizontal"))
        {
            spriterenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        if (Input.GetButtonUp("Horizontal"))
		{
			// ���⶧ �ӷ� ���̱� rigid.velocity.normalized < ��������
			rigid.velocity = new Vector2(rigid.velocity.normalized.x * 1.0f, rigid.velocity.y);
		}

        //�ִϸ��̼� ��ȯ
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

        if (rigid.velocity.x > maxspeed)    // �ְ�ӷ� ����
            rigid.velocity = new Vector2(maxspeed, rigid.velocity.y);
        else if (rigid.velocity.x < (maxspeed * (-1)))
            rigid.velocity = new Vector2((maxspeed * (-1)), rigid.velocity.y);

        // ����ĳ��Ʈ ���� (�������϶���)
        if (rigid.velocity.y < 0)
		{
            // ���� �׸���
            Debug.DrawRay(rigid.position, Vector3.down, new Color(255, 0, 0));// ����ĳ��Ʈ �׸���
            
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
        //�����۰� �浹
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

        //��߰� �浹
        if(collision.tag == "Finish")
		{
            gamemanager.NextStage();
            //���� ��������
        }

	}

	void OnCollisionEnter2D(Collision2D collision)
	{
        //enemy�� �浹
		if(collision.gameObject.tag == "Enemy")
		{
            //�Ʒ��� �̵��� enemy���� �����ִٸ�
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
        //���׼�
        rigid.AddForce(Vector2.up * 3, ForceMode2D.Impulse);
        //enemyó��
        Enemy enemy = enemyobject.GetComponent<Enemy>();
        enemy.OnDamaged();
    }

    //���ظ� �Ծ�����
    void OnDamaged(Vector2 collision)
    {
        //hp
        gamemanager.HpDown();
        //��������
        gameObject.layer = 8;
        //��������Ʈ ����
        spriterenderer.color = new Color(1, 1, 1, 0.5f);//������, ����
        //�� ����
        int dir = transform.position.x - collision.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dir, 1) * 3, ForceMode2D.Impulse);

        ani.SetTrigger("damaged");

        Invoke("OffDamaged", 2);

    }

    void OffDamaged()
    {
        //��������
        gameObject.layer = 7;
        //��������Ʈ ����
        spriterenderer.color = new Color(1, 1, 1);//������, ����
    }

    public void OnDie()
    {
       
    }
}
