using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class loadbutton : MonoBehaviour
{
    public InputField inputField1;//�˺�
    public InputField inputField2;//����
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
        //�ж��˺������Ƿ���ȷ  if...

        SceneManager.LoadScene(1);
    }
    public void returnClick()
    {
        SceneManager.LoadScene(6);
    }
}
