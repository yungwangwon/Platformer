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
        //������ �Լ�ȣ��
        Invoke("Think", 5);
    }

    void FixedUpdate()
    {
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //����ĳ��Ʈ 
        Vector2 front = new Vector2(rigid.position.x + nextMove, rigid.position.y);
        Debug.DrawRay(front, Vector3.down, new Color(255, 0, 0));// ����ĳ��Ʈ �׸���
        RaycastHit2D rayhit = Physics2D.Raycast(front, Vector3.down, 1, LayerMask.GetMask("Floor")); ;
        if (rayhit.collider == null)
        {
            nextMove *= -1;
            spriteranderer.flipX = (nextMove == 1);
            CancelInvoke();
            Invoke("Think", Random.Range(2f, 5f));
        }
    }

    //���� ������ ����
    void Think()
	{
        nextMove = Random.Range(-1, 2);   //-1,0,1(����)

        ani.SetInteger("runspeed", nextMove);

        //�¿��Ī
        if (nextMove != 0)
            spriteranderer.flipX = (nextMove == 1);

        Invoke("Think",Random.Range(2f,5f));

    }

    //��������
    public void OnDamaged()
	{
        //�� ��ȯ > ����
        spriteranderer.color = new Color(1, 1, 1, 0.5f);
        //��������Ʈ�ø�
        spriteranderer.flipY = true;
        //�浹 ����
        capcollider.enabled = false;
        //���� �پ������
        rigid.AddForce(Vector2.up * 3, ForceMode2D.Impulse);
        //������Ʈ ��Ȱ��ȭ
        Invoke("Killobject", 3);
    }

    void Killobject()
	{
        gameObject.SetActive(false);
    }

}
