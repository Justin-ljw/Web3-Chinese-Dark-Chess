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
        //��ȡ�Ծ�ʤ����ͼƬ���
        Image result =  GameObject.Find("result").GetComponent<Image>();
        //��ȡ�������
        AudioSource audioSource = GetComponent<AudioSource>();

        int param = GameObject.Find("GameData").GetComponent<GameData>().param;
        Debug.Log(param); //���Դ�ֵ�Ƿ�ɹ�
        //ͨ����Ϸ���̴����Ĳ������ж�ʤ���������ִ����Ӧ�Ĳ���
        if (param == 1)//ʤ��
        {
            result.sprite = winimage;
            audioSource.PlayOneShot(winMusic);
        }
        else if (param == 2)//ʧ��
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
