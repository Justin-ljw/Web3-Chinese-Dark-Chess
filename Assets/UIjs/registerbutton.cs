using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class registerbutton : MonoBehaviour
{
    public InputField inputField1;
    public InputField inputField2;
    public InputField inputField3;
    public Text messageText;
    // Start is called before the first frame update
    void Start()
    {
        messageText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void loadClick()
    {
        SceneManager.LoadScene(2);//2��Ҫ�л��ĳ���������
    }
    public void ifRegister()
    {
        // Debug.Log(inputField1.text);
        string text1 = inputField1.text;
        string text2 = inputField2.text;
        string text3 = inputField3.text;
        if (text2.Equals(text3))
        {
            // Debug.Log(text3);
            messageText.text = "ע��ɹ�";
            messageText.gameObject.SetActive(true);

        }
        else
        {
            messageText.text = "ע��ʧ�� �����������벻һ��";
            messageText.gameObject.SetActive(true);
        }
       
    }
}
