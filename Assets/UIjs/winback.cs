using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class winback : MonoBehaviour
{
    // Start is called before the first frame update
    public Sprite winimage;
    public Sprite loseimage;
    public AudioClip winMusic;
    public AudioClip loseMusic;
    void Start()
    {
        //获取对局胜负的图片组件
        Image result =  GameObject.Find("result").GetComponent<Image>();
        //获取声音组件
        AudioSource audioSource = GetComponent<AudioSource>();

        int param = GameObject.Find("GameData").GetComponent<GameData>().param;
        Debug.Log(param); //测试传值是否成功
        //通过游戏进程传来的参数，判断胜负结果，并执行相应的操作
        if (param == 1)//胜利
        {
            result.sprite = winimage;
            audioSource.PlayOneShot(winMusic);
        }
        else if (param == 2)//失败
        {
            result.sprite = loseimage;
            audioSource.PlayOneShot(loseMusic);
        }


    }

    // Update is called once per frame
    void Update()
    {

    }
    public void returnClick()
    {
        SceneManager.LoadScene(1);
    }
    public void playClick()
    {
        SceneManager.LoadScene(4);
    }

}
