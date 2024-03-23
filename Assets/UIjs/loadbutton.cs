using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class loadbutton : MonoBehaviour
{
    public InputField inputField1;//ÕËºÅ
    public InputField inputField2;//ÃÜÂë
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
        //ÅĞ¶¨ÕËºÅÃÜÂëÊÇ·ñÕıÈ·  if...

        SceneManager.LoadScene(1);
    }
    public void returnClick()
    {
        SceneManager.LoadScene(6);
    }
}
