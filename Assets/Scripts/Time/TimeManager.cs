using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private static TimeManager _Instance;
    public static TimeManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                GameObject obj = new GameObject("TimeManager");
                _Instance = obj.AddComponent<TimeManager>();
            }
            return _Instance;
        }
    }
    public List<IReverseState> reverseObj = new List<IReverseState>();
    bool reverse;
    public AudioSource bgm;
    public Material bj;
    float pos;
    public float dir = -0.1f;
    public GameObject trail; //轨迹

    private void Start()
    {
        bgm = FindObjectOfType<AudioSource>();
        bj = GameObject.Find("Bj").GetComponent<SpriteRenderer>().material;
        InitAllState();
        InvokeRepeating("SaveAllState", 0.05f, 0.05f);
        InvokeRepeating("ClearAllState", 0.1f, 0.05f);
    }

    private void Update()
    {
        pos += Time.deltaTime * dir;
        bj.mainTextureOffset = new Vector2(pos, 0);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            trail.SetActive(true);
            bgm.pitch = -1;
            dir = 0.1f;
            bj.color = new Color(1, 1, 0.5f, 1);
            reverse = true;
            SwitchAllState();
        }
        if (Input.GetKeyUp(KeyCode.Space)) //ffdfye
        {
            trail.SetActive(false);
            bgm.pitch = 1;
            dir = -0.1f;
            bj.color = new Color(1, 1, 1, 1);
            reverse = false;
            SwitchAllState();
        }

        if (reverse == true)
            ResetAllState();
    }
    void InitAllState() //所有物体初始化状态
    {
        for (int i = 0; i < reverseObj.Count; i++)
        {
            reverseObj[i].InitState();
        }
    }
    void SaveAllState() //所有物体保存状态
    {
        if (reverse == false)
        {
            for (int i = 0; i < reverseObj.Count; i++)
            {
                reverseObj[i].SaveState();
            }
        }
    }
    void SwitchAllState() //所有物体切换状态
    {
        for (int i = 0; i < reverseObj.Count; i++)
        {
            reverseObj[i].SwitchState(reverse);
        }
    }
    void ResetAllState() //所有物体重置状态
    {
        for (int i = 0; i < reverseObj.Count; i++)
        {
            reverseObj[i].ResetState();
        }
    }
    void ClearAllState() //所有物体清除状态
    {
        if (reverse == true)
        {
            for (int i = 0; i < reverseObj.Count; i++)
            {
                reverseObj[i].ClearState();
            }
        }
    }
}

public interface IReverseState
{
    void InitState(); //初始化状态
    void SaveState(); //保存状态
    void SwitchState(bool reverse); //切换状态
    void ResetState(); //重置状态
    void ClearState(); //清除状态
}
