using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class winback : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int param = GameObject.Find("GameData").GetComponent<GameData>().param;
        Debug.Log(param); //���Դ�ֵ�Ƿ�ɹ�

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void returnClick()
    {
        SceneManager.LoadScene(1);
    }

}
