using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameThread : MonoBehaviour
{
    //被选中的棋子（没有棋子被选中时为null）
    private ChessClass selectedChess = null;
    //玩家对象
    private PlayerClass player1 = PlayerClass.GetPlayer();
    //敌人对象
    private PlayerClass player2 = PlayerClass.GetEnemy();
    //棋盘管理器
    private BoardClass boardClass;

    //当前回合的棋子颜色（红色为true，黑色为false，红方先行）
    private bool bound = true;
    //一回合的时间
    private static float boundTime = 60f;
    //本回合开始的时间
    private float boundStartTime = 0f;
    //UI计时器
    public Text txtTimer;
    private int timer = 60;

    public TMP_Text HP1;
    public TMP_Text HP2;

    // Start is called before the first frame update
    void Start()
    {
        GameLoad();

        //获取棋盘管理器
        boardClass = BoardClass.GetBoard();
    }

    // Update is called once per frame
    void Update()
    {
        GameRun();
    }

    public void GameLoad()
    {

    }

    //游戏进行时
    public void GameRun()
    {
        //判定是否超时，没有超时游戏才能继续
        if (!IsOutOfBound())
        {
            
            //如果没有棋子在移动时，才能进行其他操作
            if (!MovingChess())
            {
                ChessControl();
            }
        }

        //判定是否结束游戏
        EndGame();

        
    }

    //判定是否超时，超时则直接判定输了（true为超时，false为没有超时）
    private bool IsOutOfBound()
    {
        timer = (int)(60 - Time.time + boundStartTime);
        txtTimer.text = Convert.ToString(timer);
        //如果游戏当前的时间距离本回合开始的时间超过一个回合的时间则自动判定这一方输了
        if (timer <= 0)
        {
            //设置当前回合的一方输了
            if(bound == player1.ChessColor)
            {
                player1.Hp = 0;
            }
            else
            {
                player2.Hp = 0;
            }

            return true;
        }

        return false;
    }

    //判定游戏是否分出胜负
    private void EndGame()
    {
        //如果有一方血量小于或等于0，则游戏结束，并显示输赢
        if(player1.Hp <= 0)
        {
            ///显示输赢
            Debug.Log("player2 Win");

            //暂停游戏
            Time.timeScale = 0;
        }
        else if(player2.Hp <= 0)
        {
            ///显示输赢
            Debug.Log("player1 Win");

            //暂停游戏
            Time.timeScale = 0;
        }
    }

    //回合切换
    private void BoundChange()
    {
        //当前的回合方切换颜色
        bound = !bound;
        //Debug.Log("当前为："+bound);
        //当前的时间设置为新回合的开始时间
        boundStartTime = Time.time;
    }

    //监听鼠标点击
    private void ChessControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //判断是Ray否发生了碰撞
            if (Physics.Raycast(ray, out hit))
            {
                //棋子的结构是一个用于修正中心点的父物体和一个实际子物体组成，但因为父物体为空对象，碰撞检测会检测到子物体
                //（获取的是子物体，但是操作时要操作父物体）
                GameObject hitObject = hit.collider.gameObject;

                //判断点击的物体是否是Chess（标签为红棋或黑棋）
                if (hitObject.tag == "RedChess" || hitObject.tag == "BlackChess")
                {
                    //如果是棋子就获取棋子类脚本ChessClass
                    ChessClass chessObject = hitObject.GetComponent<ChessClass>();

                    //判断点击的棋子是什么类型的棋子
                    //点击的棋子是当前回合方已翻起的棋子
                    /*或是未翻起的棋子，并且当前没有棋子选中或并且选中的棋子未翻起
                    ，或并且选中的已翻起棋子不是炮（炮除了已翻起的我方棋子都能吃）,或并且选中的棋子为已翻起的炮，但炮不能吃*/
                    if((chessObject.ColorType == bound && chessObject.IsUp)  || 
                        (!chessObject.IsUp && (selectedChess == null || selectedChess.IsUp == false || 
                        (selectedChess.ChessType != ChessType.PAO && selectedChess.IsUp == true) || 
                        (selectedChess.ChessType == ChessType.PAO && selectedChess.IsUp == true && !selectedChess.IsPaoCanEat(chessObject)))))
                    {
                        //选棋
                        SelectChess(chessObject);
                    }
                    //点击的棋子是敌方已翻起的棋子
                    //或是未翻起的棋子，并且当前选中的棋子是已翻起的炮
                    else
                    {
                        //吃棋
                        EatChess(chessObject);
                    }
                }
                //点击的物体是棋盘格子
                else if(hitObject.tag == "BoardElement")
                {
                    //如果棋盘上有棋子就返回棋盘格子上的棋子，如果没有就返回null
                    GameObject hitChess = IsHaveChess(hitObject);

                    //如果棋盘格子上没有棋子，则执行走棋
                    if(hitChess == null)
                    {
                        MoveChess(hitObject);
                    }
                    //如果有棋子，则执行吃棋
                    else
                    {
                        ChessClass hitChessClass = hitChess.GetComponent<ChessClass>();

                        //吃棋
                        EatChess(hitChessClass);
                    }
                }
            }
        }

    }

    //选中棋子（传入点击的我方棋子对象或未翻起的棋子对象）
    private void SelectChess(ChessClass chessObject)
    {
        //如果棋子选中状态本来为true，则该棋子为已选中，这是第二次点击他
        if (chessObject.IsSelected)
        {
            //若本棋子为未翻起，则将他翻起，否则不操作
            if (chessObject.IsUp == false)
            {
                //翻起棋子（翻完棋子后，棋子取消选中）
                chessObject.Up();

                selectedChess = null;

                //翻完棋后结束回合
                BoundChange();
            }
        }
        //若棋子本来为false，代表之前未被选中
        else
        {
            //选择当前点击的棋子
            chessObject.Select();

            //如果本来有棋子已经被选中，就取消选中原本的棋子
            if(selectedChess != null)
            {
                selectedChess.Unselect();
            }

            //把当前点击的棋子标记为当前选中的棋子
            selectedChess = chessObject;
        }
    }

    //吃棋子（传入点击的已翻起的敌方棋子对象）
    private void EatChess(ChessClass chessObject)
    {
        //有已翻起的棋子已被选中才能吃棋
        if (selectedChess != null && selectedChess.IsUp == true)
        {
            //选中的已翻起的棋子不为炮时，点击的棋子是已翻起的敌方棋子，才执行吃棋（如果是我方棋子或未翻起的棋子则不操作）
            if (selectedChess.ChessType != ChessType.PAO && 
                chessObject.ColorType != bound && chessObject.IsUp)
            {
                //如果点击的棋子比被选中的棋子小，才能吃棋（帅不能吃兵，兵可以吃帅）
                if ((selectedChess.ChessType >= chessObject.ChessType && 
                    !(selectedChess.ChessType == ChessType.SHUAI && chessObject.ChessType == ChessType.BING)) || 
                    (selectedChess.ChessType == ChessType.BING && chessObject.ChessType == ChessType.SHUAI))
                {
                    //把被吃棋子的生存状态标记为死亡
                    chessObject.IsLive = false;

                    //获取被吃棋子所在的棋盘格子对象
                    GameObject board = boardClass.BoardManager[chessObject.X, chessObject.Y];

                    //被选中的棋子移动到被吃棋子所在的棋盘格子
                    //两棋子碰撞时，执行棋子被吃的操作
                    if(selectedChess.Move(board))
                    {
                        //如果可以移动，切换回合
                        BoundChange();
                    }
                }
            }
            //选中的已翻起的棋子为炮时，点击的棋子不是已翻起的当前方的棋子，都执行吃棋
            else if(selectedChess.ChessType == ChessType.PAO && 
                !(chessObject.ColorType == bound && chessObject.IsUp))
            {
                //如果炮能吃到点击的棋子
                if(selectedChess.IsPaoCanEat(chessObject))
                {
                    //把被吃棋子的生存状态标记为死亡
                    chessObject.IsLive = false;

                    //获取被吃棋子所在的棋盘格子对象
                    GameObject board = boardClass.BoardManager[chessObject.X, chessObject.Y];

                    //被选中的棋子移动到被吃棋子所在的棋盘格子
                    //两棋子碰撞时，执行棋子被吃的操作
                    if (selectedChess.Move(board))
                    {
                        //如果可以移动，切换回合
                        BoundChange();
                    }
                }
            }
        }
    }

    //走棋（传入要移动到的棋盘格子对象）
    private void MoveChess(GameObject boardElement)
    {
        //本来有不为炮的已翻起棋子已被选中，点击棋盘格子表示移动
        //本来没有棋子已被选中,或选中的棋子未翻起，或选中的棋子是炮，则不操作（炮不能移动到空的格子上，只能通过吃棋来移动）
        if (selectedChess != null && selectedChess.IsUp == true && selectedChess.ChessType != ChessType.PAO)
        {
            if (selectedChess.Move(boardElement))
            {
                //如果可以移动，切换回合
                BoundChange();
            }
        }
    }

    //不断判断是否要移动棋子（返回false为没有棋子正在移动，true为有棋子正在移动）
    private bool MovingChess()
    {
        //如果有已翻棋的棋子被选中，进入判断该棋子是否正在移动
        if(selectedChess != null && selectedChess.IsUp == true)
        {
            switch(selectedChess.Moving())
            {
                case 1://棋子正在移动
                    return true;

                case 0://棋子没有在移动
                    return false;

                case -1://棋子刚到达目的地，把棋子取消选中，标记为没有棋子已选中
                    selectedChess = null;

                    return false;
            }
        }

        //如果没有已翻起的棋子被选中，不可能有棋子正在移动，直接返回false
        return false;
    }

    //判断棋盘格子上有没有棋子（没有就返回null，有就返回对应的棋子）
    //（传入棋盘格子对象）
    private GameObject IsHaveChess(GameObject boardElement)
    {
        BoardClass boardClass = BoardClass.GetBoard();

        //获取点击的棋盘格子的坐标
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
