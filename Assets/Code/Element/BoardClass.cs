using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

//单例模式：内存中只有一个管理器（要让其他人都能拿到同一个管理器）
public class BoardClass
{
    //棋子在棋盘上的坐标
    //    X    -3.5    -2.5    -1.5    -0.5    0.5    1.5    2.5    3.5
    //Z
    //1.5
    //0.5
    //-0.5
    //-1.5

    //棋盘格子管理器：棋盘格子数组，用于记录各个格子的坐标位置
    private GameObject[,] boardManager = null;
    //棋子管理器：棋盘中棋子的数组，用于记录棋盘各个格子上的是什么棋子（没有为null） 
    private GameObject[,] chessManager = null;
    //死亡管理器：被吃掉的棋子的列表，用于存放已被吃掉的棋子
    private List<GameObject> deadChessManager = null;

    //用于存放不同类型的棋子对应的血量
    private Dictionary<ChessType, int> chessHP = null;

    //棋盘格子管理器只能读，不能写
    public GameObject[,] BoardManager { get => boardManager; }
    public GameObject[,] ChessManager { get => chessManager; }
    public List<GameObject> DeadChessManager { get => deadChessManager;}

    //查找某一棋盘格子元素的索引（传入要查找的棋盘格子对象，返回的x为横坐标，y为纵坐标，没找到时返回-1）
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

        //没找到时返回 - 1
        x = -1;
        y = -1;
    }

    //查找某一棋子元素的索引（传入要查找的棋子对象，返回的x为横坐标，y为纵坐标，没找到时返回-1）
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

        //没找到时返回 - 1
        x = -1;
        y = -1;
    }

    //通过棋子类型获取该类型对应的血量
    public int GetChessHp(ChessType chessType)
    {
        int hp;
        chessHP.TryGetValue(chessType, out hp);

        return hp;
    }

    //放入死亡的棋子
    public void AddDeadChess(GameObject deadChess)
    {
        DeadChessManager.Add(deadChess);
    }

    //移除棋子：棋子被吃时把棋子从棋子管理器移除，并放入到死亡管理器中
    /*public void RemoveChess(GameObject deadChess)
    {
        for (int i = 0; i < ChessManager.GetLength(0); i++)
        {
            for (int j = 0; j < ChessManager.GetLength(1); j++)
            {
                if(ChessManager[i , j] == deadChess)
                {
                    //把被吃掉的棋子放入死亡管理器
                    deadChessManager.Add(deadChess);
                    //把被吃掉的棋子从棋子管理器移除
                    chessManager[i, j] = null;
                }
            }
        }
    }*/

    //单例本类：内存中只有唯一一个
    private static BoardClass boardClass = null;
    //线程锁对象：用于给线程上锁
    private static Object locker = new Object();

    //返回唯一的管理器（如果本来没有就new一个）
    //饱汉模式：要用的时候才创建
    //要加一个线程锁，防止同时执行的不同程序同时拿到并修改
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

    //不允许其他人创建，内存中只能有一个
    private BoardClass(){}

    //初始化方法
    //由于使用双例类，必须使用静态变量，
    //第二次重新进入当前场景时，本类的内存不会清除，但类里面的变量会，所以要重新初始化一下
    //所以构造函数和初始化函数分开调用，每次进入下棋场景都初始化一下本类
    public void Init()
    {
        boardManager = new GameObject[4, 8];
        chessManager = new GameObject[4, 8];
        deadChessManager = new List<GameObject>();
        chessHP = new Dictionary<ChessType, int>();

        //初始化不同类型子对应的血量
        chessHP.Add(ChessType.SHUAI, 30);
        chessHP.Add(ChessType.SHI, 10);
        chessHP.Add(ChessType.XIANG, 5);
        chessHP.Add(ChessType.JU , 5);
        chessHP.Add(ChessType.MA , 5);
        chessHP.Add(ChessType.PAO , 5);
        chessHP.Add(ChessType.BING, 2);

        Debug.Log(boardClass);
        //初始化棋盘格子列表
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
    }
}
