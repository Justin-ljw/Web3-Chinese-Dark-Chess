using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    //��ǰ�غϵ�������ɫ����ɫΪtrue����ɫΪfalse���췽���У�
    private bool bound = true;
    //һ�غϵ�ʱ��
    private static float boundTime = 60f;
    //���غϿ�ʼ��ʱ��
    private float boundStartTime = 0f;
    
    //���ֲ������
    private AudioSource audioSource;
    //����������Դ�Ľű�
    private MusicClass musicClass;

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
            }
        }

        //�ж��Ƿ������Ϸ
        EndGame();

        
    }

    //�ж��Ƿ�ʱ����ʱ��ֱ���ж����ˣ�trueΪ��ʱ��falseΪû�г�ʱ��
    private bool IsOutOfBound()
    {
        //�����Ϸ��ǰ��ʱ����뱾�غϿ�ʼ��ʱ�䳬��һ���غϵ�ʱ�����Զ��ж���һ������
        if(Time.time - boundStartTime >= boundTime)
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
            Debug.Log("Enemy Win");

            //��ͣ��Ϸ
            Time.timeScale = 0;

            GameObject.Find("GameData").GetComponent<GameData>().param = 12313213;
            SceneManager.LoadScene(8);
        }
        else if(enemy.Hp <= 0)
        {
            ///��ʾ��Ӯ
            Debug.Log("Player Win");

            //��ͣ��Ϸ
            Time.timeScale = 0;

            GameObject.Find("GameData").GetComponent<GameData>().param = 12313213;
            SceneManager.LoadScene(7);
        }
    }

    //�غ��л�
    private void BoundChange()
    {
        //��ǰ�ĻغϷ��л���ɫ
        bound = !bound;
        //Debug.Log("��ǰΪ��"+bound);
        //��ǰ��ʱ������Ϊ�»غϵĿ�ʼʱ��
        boundStartTime = Time.time;
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
                }
            }
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
