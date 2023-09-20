using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class loadbutton : MonoBehaviour
{
    public InputField inputField1;//’À∫≈
    public InputField inputField2;//√‹¬Î
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void loadClick()
    {
        //≈–∂®’À∫≈√‹¬Î «∑Ò’˝»∑  if...

        SceneManager.LoadScene(1);
    }
    public void returnClick()
    {
        SceneManager.LoadScene(6);
    }
}
