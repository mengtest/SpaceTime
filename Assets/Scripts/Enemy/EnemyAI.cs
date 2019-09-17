using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IReverseState
{
    public float moveSpeed;
    Transform rayPos; //射线源
    SpriteRenderer sr;
    bool reverse;
    Stack<Vector2> posInfo = new Stack<Vector2>(); //保存自身位置
    Stack<Sprite> sprInfo = new Stack<Sprite>(); //保存动画动作(图片)
    Stack<int> dirInfo = new Stack<int>(); //保存面朝方向
    float dis;
    private void Awake()
    {
        TimeManager.Instance.reverseObj.Add(this); //将自身添加进时间管理局
    }

    private void Update()
    {
        if (!reverse)
            Move();
    }
    //移动 
    public void Move()
    {
        RaycastHit2D hit = Physics2D.Raycast(rayPos.position, -transform.up, 0.2f);
        if (hit.point == Vector2.zero)
            transform.rotation *= Quaternion.Euler(0, 180, 0);

        transform.Translate(transform.right.x * moveSpeed * Time.deltaTime, 0, 0, Space.World);
    }
    //碰撞
    private void OnCollisionEnter2D(Collision2D obj)
    {
        if (obj.transform.tag == "Wall") //碰到墙转身
            transform.rotation *= Quaternion.Euler(0, 180, 0);
        if (obj.transform.tag == "Player") //如果和主角碰撞
        {
            Vector3 playerPos = obj.transform.position; //获取主角当前坐标
            Vector3 selfPos = transform.position; //获取自身当前坐标
            float OffsetX = GetComponentInChildren<BoxCollider2D>().bounds.size.x / 2f; //获取偏移量(自身宽度的一半)
            //如果主角在自己上方，并且在x轴偏移量之内,判定被主角踩到
            if (playerPos.y > selfPos.y && Mathf.Abs(playerPos.x - selfPos.x) <= OffsetX)
            {
                obj.transform.GetComponent<Rigidbody2D>().Sleep();
                obj.transform.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 100, ForceMode2D.Impulse); //让主角往上弹
                obj.transform.GetComponent<AnimationManager>().PlayJump(); //播放跳跃动画
            }
            else //否则就视为主角被攻击
                obj.transform.GetComponent<Character>().Death(); //调用主角死亡方法  
        }
    }

    public void InitState() //初始化状态
    {
        rayPos = transform.Find("RayPos");
        sr = GetComponentInChildren<SpriteRenderer>();

        SaveState(); //保存初始位置
    }
    public void SaveState() //保存状态
    {
        posInfo.Push(transform.position);
        sprInfo.Push(sr.sprite);
        dirInfo.Push((int)transform.eulerAngles.y);
    }
    public void SwitchState(bool _reverse) //切换状态
    {
        reverse = _reverse;
        if (reverse)
        {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static; //设置刚体静态
            GetComponentInChildren<Animator>().enabled = false; //禁用动画控制器
            GetComponent<BoxCollider2D>().enabled = false; //禁用碰撞器
            dis = Vector3.Distance(transform.position, posInfo.Peek());
        }
        else
        {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic; ////取消刚体静态
            GetComponentInChildren<Animator>().enabled = true; //启用动画控制器
            GetComponent<BoxCollider2D>().enabled = true; //启用碰撞器
        }
    }
    public void ResetState() //重置状态
    {
        if (sprInfo.Count > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, posInfo.Peek(), dis * 20 * Time.deltaTime);
            sr.sprite = sprInfo.Peek();
            transform.eulerAngles = new Vector3(0, dirInfo.Peek(), 0);
        }
    }
    public void ClearState() //清除状态
    {
        if (sprInfo.Count > 1)
        {
            posInfo.Pop();
            sprInfo.Pop();
            dirInfo.Pop();
            dis = Vector3.Distance(transform.position, posInfo.Peek());
        }
    }
}
