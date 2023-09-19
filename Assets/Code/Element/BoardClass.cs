using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

//����ģʽ���ڴ���ֻ��һ����������Ҫ�������˶����õ�ͬһ����������
public class BoardClass
{
    //�����������ϵ�����
    //    X    -3.5    -2.5    -1.5    -0.5    0.5    1.5    2.5    3.5
    //Z
    //1.5
    //0.5
    //-0.5
    //-1.5

    //���̸��ӹ����������̸������飬���ڼ�¼�������ӵ�����λ��
    private GameObject[,] boardManager = null;
    //���ӹ����������������ӵ����飬���ڼ�¼���̸��������ϵ���ʲô���ӣ�û��Ϊnull�� 
    private GameObject[,] chessManager = null;
    //���������������Ե������ӵ��б����ڴ���ѱ��Ե�������
    private List<GameObject> deadChessManager = null;

    //���ڴ�Ų�ͬ���͵����Ӷ�Ӧ��Ѫ��
    private Dictionary<ChessType, int> chessHP = null;

    //���̸��ӹ�����ֻ�ܶ�������д
    public GameObject[,] BoardManager { get => boardManager; }
    public GameObject[,] ChessManager { get => chessManager; }
    public List<GameObject> DeadChessManager { get => deadChessManager;}

    //����ĳһ���̸���Ԫ�ص�����������Ҫ���ҵ����̸��Ӷ��󣬷��ص�xΪ�����꣬yΪ�����꣬û�ҵ�ʱ����-1��
    public void GetBoardIndex(GameObject boardElement , out int x , out int y)
    {
        for (int i = 0; i < BoardManager.GetLength(0); i++)
        {
            for (int j = 0; j < BoardManager.GetLength(1); j++)
            {
                if(BoardManager[i,j] == boardElement)
                {
                    x = i;
                    y = j;

                    return;
                }
            }
        }

        //û�ҵ�ʱ���� - 1
        x = -1;
        y = -1;
    }

    //����ĳһ����Ԫ�ص�����������Ҫ���ҵ����Ӷ��󣬷��ص�xΪ�����꣬yΪ�����꣬û�ҵ�ʱ����-1��
    public void GetChessIndex(GameObject chess, out int x, out int y)
    {
        for (int i = 0; i < ChessManager.GetLength(0); i++)
        {
            for (int j = 0; j < ChessManager.GetLength(1); j++)
            {
                if (ChessManager[i, j] == chess)
                {
                    x = i;
                    y = j;

                    return;
                }
            }
        }

        //û�ҵ�ʱ���� - 1
        x = -1;
        y = -1;
    }

    //ͨ���������ͻ�ȡ�����Ͷ�Ӧ��Ѫ��
    public int GetChessHp(ChessType chessType)
    {
        int hp;
        chessHP.TryGetValue(chessType, out hp);

        return hp;
    }

    //��������������
    public void AddDeadChess(GameObject deadChess)
    {
        DeadChessManager.Add(deadChess);
    }

    //�Ƴ����ӣ����ӱ���ʱ�����Ӵ����ӹ������Ƴ��������뵽������������
    /*public void RemoveChess(GameObject deadChess)
    {
        for (int i = 0; i < ChessManager.GetLength(0); i++)
        {
            for (int j = 0; j < ChessManager.GetLength(1); j++)
            {
                if(ChessManager[i , j] == deadChess)
                {
                    //�ѱ��Ե������ӷ�������������
                    deadChessManager.Add(deadChess);
                    //�ѱ��Ե������Ӵ����ӹ������Ƴ�
                    chessManager[i, j] = null;
                }
            }
        }
    }*/

    //�������ࣺ�ڴ���ֻ��Ψһһ��
    private static BoardClass boardClass = null;
    //�߳����������ڸ��߳�����
    private static Object locker = new Object();

    //����Ψһ�Ĺ��������������û�о�newһ����
    //����ģʽ��Ҫ�õ�ʱ��Ŵ���
    //Ҫ��һ���߳�������ֹͬʱִ�еĲ�ͬ����ͬʱ�õ����޸�
    public static BoardClass GetBoard()
    {
        lock(locker)
        {
            if (boardClass == null)
            {
                boardClass = new BoardClass();
            }

            return boardClass;
        }
    }

    //�����������˴������ڴ���ֻ����һ��
    private BoardClass()
    {
        Init();
    }

    private void Init()
    {
        boardManager = new GameObject[4, 8];
        chessManager = new GameObject[4, 8];
        deadChessManager = new List<GameObject>();
        chessHP = new Dictionary<ChessType, int>();

        //��ʼ����ͬ�����Ӷ�Ӧ��Ѫ��
        chessHP.Add(ChessType.SHUAI, 30);
        chessHP.Add(ChessType.SHI, 10);
        chessHP.Add(ChessType.XIANG, 5);
        chessHP.Add(ChessType.JU , 5);
        chessHP.Add(ChessType.MA , 5);
        chessHP.Add(ChessType.PAO , 5);
        chessHP.Add(ChessType.BING, 2);

        //��ʼ�����̸����б�
        GameObject[] boardElement = GameObject.FindGameObjectsWithTag("BoardElement");

        if(boardElement != null)
        {
            for (int i = 0, k = 0; i < BoardManager.GetLength(0); i++)
            {
                for (int j = 0; j < BoardManager.GetLength(1); j++)
                {
                    BoardManager[i, j] = boardElement[k];
                    k++;
                }
            }
        }

    
        //TODO:���������������
        //��ʼ��һ��32λ����С��List����Ӧ32�����ӣ�ȡ�����ظ��������

        List<string> chessName = new List<string>() {
            "Red.Shuai.1" ,"Red.Shi.1","Red.Shi.2","Red.Xiang.1","Red.Xiang.2","Red.Ju.1",
           "Red.Ju.2","Red.Ma.1","Red.Ma.2","Red.Pao.1","Red.Pao.2","Red.Bing.1","Red.Bing.2",
           "Red.Bing.3","Red.Bing.4","Red.Bing.5",
           "Black.Shuai.1" ,"Black.Shi.1","Black.Shi.2","Black.Xiang.1","Black.Xiang.2","Black.Ju.1",
           "Black.Ju.2","Black.Ma.1","Black.Ma.2","Black.Pao.1","Black.Pao.2","Black.Bing.1","Black.Bing.2",
           "Black.Bing.3","Black.Bing.4","Black.Bing.5"
        };
    
        Random random = new System.Random((int)System.DateTime.Now.Ticks);

        //��ȡԤ�Ƽ�
        GameObject chessPrefab = Resources.Load("Prefab/Chess") as GameObject;

       
        //ʵ��������
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                int index = random.Next(0, chessName.Count);
                //Debug.Log("i:"+i +"j:"+ j + "index:" + index);


                GameObject chess = GameObject.Instantiate(chessPrefab, boardManager[i, j].transform.position, Quaternion.identity);
                chess.transform.GetChild(0).transform.GetChild(0).name = chessName[index];

                //�����ű������������ֽ����Զ���������
                chess.transform.GetChild(0).transform.GetChild(0).GetComponent<ChessClass>().enabled = true;

                chessManager[i, j] = chess.transform.Find("Chess/"+chessName[index]).gameObject;
                chessName.RemoveAt(index);
            }
        }

        //��ʼ�����������б�
        GameObject[] redChess = GameObject.FindGameObjectsWithTag("RedChess");
        GameObject[] blackChess = GameObject.FindGameObjectsWithTag("BlackChess");

    }
}
