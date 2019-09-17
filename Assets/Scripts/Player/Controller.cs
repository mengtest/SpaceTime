using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour, IReverseState
{
    public float height; //死亡高度
    public GameObject image;
    Rigidbody2D r2d;
    SpriteRenderer sr; //精灵渲染器
    Character chara;
    float dis;
    bool reverse;
    float timer = 0;

    Stack<Vector2> posInfo = new Stack<Vector2>(); //保存自身位置
    Stack<Sprite> sprInfo = new Stack<Sprite>(); //保存动画动作(图片)
    Stack<int> dirInfo = new Stack<int>(); //保存面朝方向
    Stack<Vector2> velInfo = new Stack<Vector2>(); //保存速度信息

    private void Awake()
    {
        TimeManager.Instance.reverseObj.Add(this); //将自身添加进时间管理局a
        TimeManager.Instance.trail = transform.Find("Trails").gameObject;
    }

    void Update()
    {
        if (transform.position.y < height)
        {
            chara.Death();
        }

        if (reverse || chara.death)
        {
            if (chara.death)
            {
                if (reverse == false) //死亡后且未反转情况下
                {
                    if (timer < 1f) //倒计时一定时间后暂停游戏
                        timer += Time.deltaTime;
                    else
                    {
                        TimeManager.Instance.bgm.pitch = 0;
                        TimeManager.Instance.dir = 0;
                        Time.timeScale = 0;
                        image.SetActive(true);
                    }
                }
                else //反转时计时器复原,如果复原到死亡时间点则复活
                {
                    Time.timeScale = 1;
                    image.SetActive(false);
                    if (timer > 0)
                        timer -= Time.deltaTime;
                    else
                        chara.Resuscitate();
                }
            }
            return; //死亡或反转时不能控制
        }
        chara.Run(Input.GetAxisRaw("Horizontal"));
        chara.Jump(Input.GetKeyDown(KeyCode.J));
    }
    public void InitState() //初始化状态
    {
        r2d = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        chara = GetComponent<Character>();

        SaveState(); //保存初始状态
    }
    public void SaveState() //保存状态
    {
        posInfo.Push(transform.position);
        sprInfo.Push(sr.sprite);
        dirInfo.Push((int)transform.eulerAngles.y);
        velInfo.Push(r2d.velocity);
    }

    public void SwitchState(bool _reverse) //切换状态
    {
        reverse = _reverse;
        if (reverse)
        {
            GetComponentInChildren<Animator>().enabled = false; //禁用动画控制器
            GetComponent<BoxCollider2D>().enabled = false; //禁用碰撞器
            dis = Vector3.Distance(transform.position, posInfo.Peek());
        }
        else
        {
            GetComponentInChildren<Animator>().enabled = true; //启用动画控制器
            if (chara.death == false)
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
            r2d.velocity = velInfo.Peek();
        }
    }
    public void ClearState() //清除状态
    {
        if (sprInfo.Count > 1)
        {
            posInfo.Pop();
            sprInfo.Pop();
            dirInfo.Pop();
            velInfo.Pop();
            dis = Vector3.Distance(transform.position, posInfo.Peek());
        }
        else
        {
            TimeManager.Instance.dir = 0;
            TimeManager.Instance.bgm.pitch = 0;
        }
    }
}
