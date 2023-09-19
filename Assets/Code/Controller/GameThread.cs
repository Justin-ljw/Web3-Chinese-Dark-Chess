using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameThread : MonoBehaviour
{
    //��ѡ�е����ӣ�û�����ӱ�ѡ��ʱΪnull��
    private ChessClass selectedChess = null;
    //��Ҷ���
    private PlayerClass player = PlayerClass.GetPlayer();
    //���˶���
    private PlayerClass enemy = PlayerClass.GetEnemy();
    //���̹�����
    private BoardClass boardClass;
    //UI����
    public Canvas canvas;

    //��ǰ�غϵ�������ɫ����ɫΪtrue����ɫΪfalse���췽���У�
    private bool bound = true;
    //һ�غϵ�ʱ��
    //private static float boundTime = 60f;
    //���غϿ�ʼ��ʱ��
    private float boundStartTime = 0f;
    
    //���ֲ������
    private AudioSource audioSource;
    //����������Դ�Ľű�
    private MusicClass musicClass;

    //UI��ʱ��
    public TMP_Text txtTimer_red;
    public TMP_Text txtTimer_blue;
    private TMP_Text txtTimer;
    private int timer = 60;

    //UI���Ѫ��
    public TMP_Text HPnum1;
    public TMP_Text HPnum2;
    public Slider HPUI1;
    public Slider HPUI2;

    //UI�˺���ʾ
    public TMP_Text hurtnum_red;
    public TMP_Text hurtnum_blue;

    //UI����
    public Image broadcast;

    //UIͷ������
    public Image left_fire;
    public Image right_fire;

    //UI����ʣ��
    private Dictionary<string, int> record = new Dictionary<string, int>() {
        {"Black_SHUAI",1 },{"Black_SHI",2},{"Black_XIANG",2},{"Black_JU",2},{"Black_MA",2},{"Black_PAO",2},{"Black_BING",5},
        {"Red_SHUAI",1 },{"Red_SHI",2},{"Red_XIANG",2},{"Red_JU",2},{"Red_MA",2},{"Red_PAO",2},{"Red_BING",5}
    };

    // Start is called before the first frame update
    void Start()
    {
        GameLoad();

        //��ȡ���̹�����
        boardClass = BoardClass.GetBoard();
    }

    // Update is called once per frame
    void Update()
    {
        GameRun();
    }

    public void GameLoad()
    {
        //��ȡ���ֲ��������������Դ�ļ�
        audioSource = GetComponent<AudioSource>();
        musicClass = GetComponent<MusicClass>();

        //����������з�
        System.Random random = new System.Random((int)System.DateTime.Now.Ticks);
        int n = random.Next(100);

        Image bc = Instantiate(broadcast, broadcast.transform.parent);

        if (n % 2 == 0)
        {
            bound = true;
            txtTimer = txtTimer_red;
            bc.GetComponent<Image>().sprite = Resources.Load<Sprite>("Img/red_first");
            right_fire.enabled = true;
        }
        else
        {
            bound = false;
            txtTimer = txtTimer_blue;
            bc.GetComponent<Image>().sprite = Resources.Load<Sprite>("Img/blue_first");
            left_fire.enabled = true;

        }

        txtTimer.enabled = true;
        bc.enabled = true;
        Destroy(bc, 2);
    }

    //��Ϸ����ʱ
    public void GameRun()
    {
        //�ж��Ƿ�ʱ��û�г�ʱ��Ϸ���ܼ���
        if (!IsOutOfBound())
        {
            
            //���û���������ƶ�ʱ�����ܽ�����������
            if (!MovingChess())
            {
                ChessControl();
                UIchange();
            }

        }

        //�ж��Ƿ������Ϸ
        EndGame();

        
    }

    //�ж��Ƿ�ʱ����ʱ��ֱ���ж����ˣ�trueΪ��ʱ��falseΪû�г�ʱ��
    private bool IsOutOfBound()
    {
        timer = (int)(60 - Time.time + boundStartTime);
        txtTimer.text = Convert.ToString(timer);
        //�����Ϸ��ǰ��ʱ����뱾�غϿ�ʼ��ʱ�䳬��һ���غϵ�ʱ�����Զ��ж���һ������
        if (timer <= 0)
        {
            //���õ�ǰ�غϵ�һ������
            if(bound == player.ChessColor)
            {
                player.Hp = 0;
            }
            else
            {
                enemy.Hp = 0;
            }

            return true;
        }

        return false;
    }

    //�ж���Ϸ�Ƿ�ֳ�ʤ��
    private void EndGame()
    {
        //�����һ��Ѫ��С�ڻ����0������Ϸ����������ʾ��Ӯ
        if(player.Hp <= 0)
        {
            ///��ʾ��Ӯ
            Debug.Log("enemy Win");

            //��ͣ��Ϸ
            Time.timeScale = 0;
        }
        else if(enemy.Hp <= 0)
        {
            ///��ʾ��Ӯ
            Debug.Log("player Win");

            //��ͣ��Ϸ
            Time.timeScale = 0;
        }
    }

    //�غ��л�
    private void BoundChange()
    {
        left_fire.enabled = false;
        right_fire.enabled = false;

        txtTimer.enabled = false;
        //��ǰ�ĻغϷ��л���ɫ
        bound = !bound;
        //Debug.Log("��ǰΪ��"+bound);
        //��ǰ��ʱ������Ϊ�»غϵĿ�ʼʱ��
        boundStartTime = Time.time;

        //�л���ʱ��
        if (bound)
        {
            txtTimer = txtTimer_red;
            right_fire.enabled = true;
        }
        else
        {
            txtTimer = txtTimer_blue;
            left_fire.enabled = true;
        }
        Debug.Log(txtTimer);
        txtTimer.enabled = true;
    }

    //���������
    private void ChessControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //�ж���Ray��������ײ
            if (Physics.Raycast(ray, out hit))
            {
                //���ӵĽṹ��һ�������������ĵ�ĸ������һ��ʵ����������ɣ�����Ϊ������Ϊ�ն�����ײ�����⵽������
                //����ȡ���������壬���ǲ���ʱҪ���������壩
                GameObject hitObject = hit.collider.gameObject;

                //�жϵ���������Ƿ���Chess����ǩΪ�������壩
                if (hitObject.tag == "RedChess" || hitObject.tag == "BlackChess")
                {
                    //��������Ӿͻ�ȡ������ű�ChessClass
                    ChessClass chessObject = hitObject.GetComponent<ChessClass>();

                    //�жϵ����������ʲô���͵�����
                    //����������ǵ�ǰ�غϷ��ѷ��������
                    /*����δ��������ӣ����ҵ�ǰû������ѡ�л���ѡ�е�����δ����
                    ������ѡ�е��ѷ������Ӳ����ڣ��ڳ����ѷ�����ҷ����Ӷ��ܳԣ�,����ѡ�е�����Ϊ�ѷ�����ڣ����ڲ��ܳ�*/
                    if((chessObject.ColorType == bound && chessObject.IsUp)  || 
                        (!chessObject.IsUp && (selectedChess == null || selectedChess.IsUp == false || 
                        (selectedChess.ChessType != ChessType.PAO && selectedChess.IsUp == true) || 
                        (selectedChess.ChessType == ChessType.PAO && selectedChess.IsUp == true && !selectedChess.IsPaoCanEat(chessObject)))))
                    {
                        //ѡ��
                        SelectChess(chessObject);
                    }
                    //����������ǵз��ѷ��������
                    //����δ��������ӣ����ҵ�ǰѡ�е��������ѷ������
                    else
                    {
                        //����
                        EatChess(chessObject);

                      
                    }
                }
                //��������������̸���
                else if(hitObject.tag == "BoardElement")
                {
                    //��������������Ӿͷ������̸����ϵ����ӣ����û�оͷ���null
                    GameObject hitChess = IsHaveChess(hitObject);

                    //������̸�����û�����ӣ���ִ������
                    if(hitChess == null)
                    {
                        MoveChess(hitObject);
                    }
                    //��������ӣ���ִ�г���
                    else
                    {
                        ChessClass hitChessClass = hitChess.GetComponent<ChessClass>();

                        //����
                        EatChess(hitChessClass);

                    
                    }
                }
            }
        }

    }

    //ѡ�����ӣ����������ҷ����Ӷ����δ��������Ӷ���
    private void SelectChess(ChessClass chessObject)
    {
        //�������ѡ��״̬����Ϊtrue���������Ϊ��ѡ�У����ǵڶ��ε����
        if (chessObject.IsSelected)
        {
            //��������Ϊδ�����������𣬷��򲻲���
            if (chessObject.IsUp == false)
            {
                //�������ӣ��������Ӻ�����ȡ��ѡ�У�
                chessObject.Up();

                selectedChess = null;

                //�����������غ�
                BoundChange();
            }
        }
        //�����ӱ���Ϊfalse������֮ǰδ��ѡ��
        else
        {
            //ѡ��ǰ���������
            chessObject.Select();

            //����ѡ����Ч
            audioSource.PlayOneShot(musicClass.selectChess);

            //��������������Ѿ���ѡ�У���ȡ��ѡ��ԭ��������
            if(selectedChess != null)
            {
                selectedChess.Unselect();
            }

            //�ѵ�ǰ��������ӱ��Ϊ��ǰѡ�е�����
            selectedChess = chessObject;
        }
    }

    //�����ӣ����������ѷ���ĵз����Ӷ���
    private void EatChess(ChessClass chessObject)
    {
        //���ѷ���������ѱ�ѡ�в��ܳ���
        if (selectedChess != null && selectedChess.IsUp == true)
        {
            //ѡ�е��ѷ�������Ӳ�Ϊ��ʱ��������������ѷ���ĵз����ӣ���ִ�г��壨������ҷ����ӻ�δ����������򲻲�����
            if (selectedChess.ChessType != ChessType.PAO && 
                chessObject.ColorType != bound && chessObject.IsUp)
            {
                //�����������ӱȱ�ѡ�е�����С�����ܳ��壨˧���ܳԱ��������Գ�˧��
                if ((selectedChess.ChessType >= chessObject.ChessType && 
                    !(selectedChess.ChessType == ChessType.SHUAI && chessObject.ChessType == ChessType.BING)) || 
                    (selectedChess.ChessType == ChessType.BING && chessObject.ChessType == ChessType.SHUAI))
                {
                    //��ȡ�����������ڵ����̸��Ӷ���
                    GameObject board = boardClass.BoardManager[chessObject.X, chessObject.Y];

                    //��ѡ�е������ƶ��������������ڵ����̸���
                    //��������ײʱ��ִ�����ӱ��ԵĲ���
                    if(selectedChess.Move(board))
                    {
                        //�ѱ������ӵ�����״̬���Ϊ����
                        chessObject.IsLive = false;

                        //��������ƶ����л��غ�
                        BoundChange();
                        
                    }

                    Recordchange(chessObject);
                }
            }
            //ѡ�е��ѷ��������Ϊ��ʱ����������Ӳ����ѷ���ĵ�ǰ�������ӣ���ִ�г���
            else if(selectedChess.ChessType == ChessType.PAO && 
                !(chessObject.ColorType == bound && chessObject.IsUp))
            {
                //������ܳԵ����������
                if(selectedChess.IsPaoCanEat(chessObject))
                {
                    //��ȡ�����������ڵ����̸��Ӷ���
                    GameObject board = boardClass.BoardManager[chessObject.X, chessObject.Y];

                    //��ѡ�е������ƶ��������������ڵ����̸���
                    //��������ײʱ��ִ�����ӱ��ԵĲ���
                    if (selectedChess.Move(board))
                    {
                        //�ѱ������ӵ�����״̬���Ϊ����
                        chessObject.IsLive = false;

                        //��������ƶ����л��غ�
                        BoundChange();
                        
                    }
                    Recordchange(chessObject);
                }
            }
        }
    }
    private void UIchange()
    {
        ////UIѪ������
        HPnum2.text = Convert.ToString(PlayerClass.GetPlayer().Hp);
        HPUI2.value = PlayerClass.GetPlayer().Hp;

        HPnum1.text = Convert.ToString(PlayerClass.GetEnemy().Hp);
        HPUI1.value = PlayerClass.GetEnemy().Hp;
    }
    private void Recordchange(ChessClass chessObject)
    {
        String name = "";
        String i = "";
        Color color;
        if (chessObject.ColorType)
        {
            name += "Red_";
            i += "2";
            color = Color.red;
        }
        else
        {
            name += "Black_";
            i += "1";
            color = Color.blue;
        }
        name += chessObject.ChessType;
        GameObject chessnum = GameObject.Find("record" + i + "/" + name);
        chessnum.GetComponentInChildren<TextMeshProUGUI>().text = "x"+Convert.ToString(record[name]-=1);

        //�˺�Ʈ��
        TMP_Text hurtnum;
        if (bound)
        {
            hurtnum = Instantiate(hurtnum_red,hurtnum_red.transform.parent);
        }
        else
        {
            hurtnum = Instantiate(hurtnum_blue, hurtnum_blue.transform.parent);
        }
        hurtnum.text = "-" + Convert.ToString(boardClass.GetChessHp(chessObject.ChessType));
        hurtnum.enabled = true;
        Destroy(hurtnum, 2);


        if (chessObject.ChessType == ChessType.SHUAI)
        {
            Image bc = Instantiate(broadcast, broadcast.transform.parent);
            bc.sprite = Resources.Load<Sprite>("Img/KillShuai");
            bc.enabled = true;
            Destroy(bc, 2);
        }


    }
    //���壨����Ҫ�ƶ��������̸��Ӷ���
    private void MoveChess(GameObject boardElement)
    {
        //�����в�Ϊ�ڵ��ѷ��������ѱ�ѡ�У�������̸��ӱ�ʾ�ƶ�
        //����û�������ѱ�ѡ��,��ѡ�е�����δ���𣬻�ѡ�е��������ڣ��򲻲������ڲ����ƶ����յĸ����ϣ�ֻ��ͨ���������ƶ���
        if (selectedChess != null && selectedChess.IsUp == true && selectedChess.ChessType != ChessType.PAO)
        {
            if (selectedChess.Move(boardElement))
            {
                //����������Ч
                audioSource.PlayOneShot(musicClass.upAndMoveChess);

                //��������ƶ����л��غ�
                BoundChange();
            }
        }
    }

    //�����ж��Ƿ�Ҫ�ƶ����ӣ�����falseΪû�����������ƶ���trueΪ�����������ƶ���
    private bool MovingChess()
    {
        //������ѷ�������ӱ�ѡ�У������жϸ������Ƿ������ƶ�
        if(selectedChess != null && selectedChess.IsUp == true)
        {
            switch(selectedChess.Moving())
            {
                case 1://���������ƶ�
                    return true;

                case 0://����û�����ƶ�
                    return false;

                case -1://���Ӹյ���Ŀ�ĵأ�������ȡ��ѡ�У����Ϊû��������ѡ��
                    selectedChess = null;

                    return false;
            }
        }

        //���û���ѷ�������ӱ�ѡ�У������������������ƶ���ֱ�ӷ���false
        return false;
    }

    //�ж����̸�������û�����ӣ�û�оͷ���null���оͷ��ض�Ӧ�����ӣ�
    //���������̸��Ӷ���
    private GameObject IsHaveChess(GameObject boardElement)
    {
        BoardClass boardClass = BoardClass.GetBoard();

        //��ȡ��������̸��ӵ�����
        int i, j;
        boardClass.GetBoardIndex(boardElement, out i, out j);

        if(boardClass.ChessManager[i,j] == null)
        {
            return null;
        }
        else
        {
            return boardClass.ChessManager[i, j];
        }
    }
}
