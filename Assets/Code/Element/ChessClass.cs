using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//一个棋子对象有3层：
//最内层是棋子的实际模型（发生碰撞是该层子物体）
//由内向外第一层父父物体用于修正模型的轴心偏移（动画组件操作这一层）
//最外层父物体用于解决：动画会发生位移时，使用同一动画的物体全部变到同一位置的问题（棋子移动时操作的是最外层的transform
public class ChessClass : MonoBehaviour
{
    private int x;//棋子在棋子管理器中的X索引
    private int y;//棋子在棋子管理器中的Y索引
    private bool colorType;//棋子的颜色（true为红色，false为黑色）
    private ChessType chessType;//棋子的类型
    private bool isUp = false;//棋子是否翻起
    private bool isLive = true;//棋子是否存活
    private bool isSelected = false;//棋子是否选择
    private Vector3 target = Vector3.zero;//棋子要移动到的目的坐标（如果不为zero则本棋子正在移动，否则为不在移动）
    private BoardClass boardClass;//棋盘管理器
    private Animator animator;//动画器组件
    private AnimatorStateInfo animatorStateInfo;//正在播放的动画
    private bool isAnimating;//本棋子是否有动画正在进行

    private void Start()
    {
        Init();
    }

    /*public ChessClass()
    {
        Init();
    }*/

    //通过棋子的名称来初始化棋子
    //棋子命名:[颜色.棋子类型.（从左到右数1234）]
    public void Init()
    {
        string[] values =  gameObject.name.Split(".");

        //设置棋子颜色
        if(values[0].Equals("Red"))
        {
            colorType = true;
        }
        else if(values[0].Equals("Black"))
        {
            colorType = false;
        }

        //设置棋子类型
        switch(values[1])
        {
            case "Shuai":
                chessType = ChessType.SHUAI;
                break;
            case "Shi":
                chessType = ChessType.SHI;
                break;
            case "Xiang":
                chessType = ChessType.XIANG;
                break;
            case "Ju":
                chessType = ChessType.JU ;
                break;
            case "Ma":
                chessType = ChessType.MA;
                break;
            case "Pao":
                chessType = ChessType.PAO;
                break;
            case "Bing":
                chessType = ChessType.BING;
                break;
        }

        //配置mesh、material、tag
        GetComponent<MeshFilter>().mesh = (Mesh)Resources.Load("Mesh/" + values[1]);

        Material[] mat = new Material[2];
        mat[0]= (Material)Resources.Load("Materials/chess");
        mat[1] = (Material)Resources.Load("Materials/" + values[0]);
        GetComponent<MeshRenderer>().materials = mat;

        tag = values[0] + "Chess";

        //获取棋盘管理器
        boardClass = BoardClass.GetBoard();

        //获取本棋子在棋盘上的索引
        boardClass.GetChessIndex(gameObject, out x, out y);

        //获取动画器组件（在父物体上）
        animator = transform.parent.GetComponent<Animator>();
        //获取正在播放的动画
        animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
    }

    public void Select()
    {
        if(!IsSelected)
        {
            //获取物体的动画组件
            //（操作的是棋子物体最外层的父物体，动画组件在父物体中）
            Animator animator = transform.parent.GetComponent<Animator>(); 

            //把他标记为选中，并把其他的棋子都标记为未选中
            IsSelected = true;

            //显示高光
            AddHighLight();
            //选中动画：启动选中动作
            animator.SetBool("isSelected", true);
        }
    }

    //取消选中棋子
    public void Unselect()
    {
        if(IsSelected)
        {
            //获取物体的动画组件
            //（操作的是棋子物体最外层的父物体，动画组件在父物体中）
             Animator animator = transform.parent.GetComponent<Animator>();

            //标记为未选中
            IsSelected = false;

            //取消显示高光
            RemoveHighLight();

            //取消选中动画
            animator.SetBool("isSelected", false);
        }
    }

    /*public void UnselectOtherChess()
    {
        //其他棋子标记为未选中
        GameObject[] otherChesses = null;
        //根据本棋子的颜色获取所有本颜色的棋子
        if (colorType)
        {
            otherChesses = GameObject.FindGameObjectsWithTag("RedChess");
        }
        else
        {
            otherChesses = GameObject.FindGameObjectsWithTag("BlackChess");
        }

        if (otherChesses != null)
        {
            //遍历所有本颜色的棋子，将除了该棋子外的所有棋子设置为未选中
            foreach (GameObject chess in otherChesses)
            {
                if (chess == gameObject)
                {
                    continue;
                }

                chess.GetComponent<ChessClass>().UnselectChess();
            }
        }
    }*/

    //翻起棋子
    public void Up()
    {
        //获取物体的动画组件
        //（操作的是棋子物体最外层的父物体，动画组件在父物体中）
        Animator animator = transform.parent.GetComponent<Animator>();

        //设置棋子翻起状态，取消选中状态
        IsUp = true;
        IsSelected = false;

        //取消显示高光
        RemoveHighLight();
        //翻起动画:启动翻起动作，翻起完成后取消选中状态，并取消选中动作
        animator.SetTrigger("isUp");
        animator.SetBool("isSelected", false);
    }

    //开始移动棋子（传入要移动到的棋盘格子）
    //返回是否可以移动（true为可移动，false为不可移动）
    public bool Move(GameObject boardElement)
    {
        //本棋子没有在移动,才能开始移动
        if (target == Vector3.zero)
        {
            //本棋子不是炮，并且点击的棋盘格子可达或本棋子是炮时，棋子才开始移动
            if ((ChessType != ChessType.PAO && CanArrive(boardElement)) || ChessType == ChessType.PAO)
            {
                //获取点击的棋盘格子的坐标
                int board_i, board_j;

                boardClass.GetBoardIndex(boardElement, out board_i, out board_j);

                //将棋子管理器中，本棋子放到目的格子的索引，本棋子原来的索引标记为null
                boardClass.ChessManager[board_i, board_j] = gameObject;
                boardClass.ChessManager[x, y] = null;
                //把本棋子的索引改为点击的棋盘格子的索引
                x = board_i;
                y = board_j;

                //给target赋值表示本棋子正在移动
                //棋子移动时只改变x和z坐标
                target = new Vector3(boardElement.transform.position.x, 0.15f, boardElement.transform.position.z);

                return true;
            }
        }

        return false;
    }

    //棋子移动中（时刻判断棋子是否需要移动）
    public int Moving()
    {
        //棋子正在移动
        if (target != Vector3.zero)
        {

            //棋子移动时操作的是棋子实际模型的父物体的父物体
            Transform t = transform.parent.parent;

            //棋子移动到目的地
            if (t.position == target)
            {
                //棋子到达目的地后取消选中
                Unselect();

                target = Vector3.zero;
                
                return -1;
            }

            //棋子移动时只改变x和z坐标
            t.Translate((target - t.position) * Time.deltaTime * 50f);

            return 1;
        }

        return 0;
    }

    //判断是否本棋子可以到达某一棋盘格子（传入要移动到的棋盘格子）
    private bool CanArrive(GameObject boardElement)
    {
        //获取点击的棋盘格子的坐标和本棋子的坐标
        int board_i, board_j;

        boardClass.GetBoardIndex(boardElement, out board_i, out board_j);

        //如果点击的棋盘格子的坐标在本棋子上下左右1格则可达，否则不可达
        if ((board_i == x && (board_j == y + 1 || board_j == y - 1)) ||
           (board_j == y && (board_i == x + 1 || board_i == x - 1)))
        {
            return true;
        }

        return false;
    }

    //判断炮是否可以吃棋：判断炮与被吃棋子之间是否有一个棋子（传入被点击的棋子）
    public bool IsPaoCanEat(ChessClass chessObject)
    {
        //标记是否可以被吃（炮与被吃棋子之间是否只有一个棋子）
        bool isCanEat = false;
        //判断炮与被吃棋子之间是否有至少一个棋子（没有棋子时为false，至少有一个棋子时为true）
        bool haveChess = false;

        //点击的棋子和炮在同一行
        if (chessObject.X == X)
        {
            //获取两个棋子中较小的j和较大的j
            int min_j = Mathf.Min(chessObject.Y, Y);
            int max_j = Mathf.Max(chessObject.Y, Y);

            //遍历同一行中，两棋子之间的棋盘格子（不包括这两个棋子本身）,判断是否只有一个棋子
            for (int j = min_j + 1; j < max_j; j++)
            {
                //遇到第一个棋子时，标记可吃，标记已遇到了棋子（haveChess）
                if (!haveChess && boardClass.ChessManager[X, j] != null)
                {
                    haveChess = true;
                    isCanEat = true;
                }
                //本来已经遇到过棋子之后再遇到棋子，就标记不可吃
                else if (haveChess && boardClass.ChessManager[X, j] != null)
                {
                    isCanEat = false;
                }
            }
        }
        //点击的棋子和炮在同一列
        else if (chessObject.Y == Y)
        {
            //获取两个棋子中较小的j和较大的j
            int min_i = Mathf.Min(chessObject.X, X);
            int max_i = Mathf.Max(chessObject.X, X);

            //遍历同一行中，两棋子之间的棋盘格子（不包括这两个棋子本身）,判断是否只有一个棋子
            for (int i = min_i + 1; i < max_i; i++)
            {
                //遇到第一个棋子时，标记可吃，标记已遇到了棋子（haveChess）
                if (!haveChess && boardClass.ChessManager[i, Y] != null)
                {
                    haveChess = true;
                    isCanEat = true;
                }
                //本来已经遇到过棋子之后再遇到棋子，就标记不可吃
                else if (haveChess && boardClass.ChessManager[i, Y] != null)
                {
                    isCanEat = false;
                }
            }
        }

        return isCanEat;
    }

    //被选中时添加高亮
    private void AddHighLight()
    {
        if(GetComponent<Outline>() == null)
        {
            Outline outline = gameObject.AddComponent<Outline>();
            outline.OutlineColor = Color.red;//设置高亮的颜色
            outline.OutlineWidth = 7.5f;//设置高亮的宽度
        }
    }

    //没有被选中时取消高亮
    private void RemoveHighLight()
    {
        if(GetComponent<Outline>() != null)
        {
            Destroy(GetComponent<Outline>());
        }
    }

    //触发检测：刚接触时触发
    //本棋子被吃时调用
    private void OnTriggerEnter(Collider other)
    {
        //如果发生碰撞时，棋子的生存状态为死亡，则本棋子是被吃的棋子，否则是吃棋的棋子
        if (!IsLive)
        {
            //把本棋子放入死亡棋子管理器
            //（棋子管理器中本棋子所在的索引已在吃棋棋子Move()时，替换为吃棋棋子）
            boardClass.AddDeadChess(gameObject);

            //判断本棋子的颜色所属，并扣除对应一方玩家本棋子对应的血量
            if(colorType == PlayerClass.GetPlayer().ChessColor)
            {
                PlayerClass.GetPlayer().Hp -= boardClass.GetChessHp(ChessType);
            }
            else
            {
                PlayerClass.GetEnemy().Hp -= boardClass.GetChessHp(ChessType);
            }

            //获取棋子的最外层父物体
            GameObject outObject = transform.parent.parent.gameObject;
            //销毁本棋子所有相关的物体（可改为isActive = false）
            Destroy(outObject);
        }
    }

    public int X { get => x; set => x = value; }
    public int Y { get => y; set => y = value; }
    public bool ColorType { get => colorType; set => colorType = value; }
    public ChessType ChessType { get => chessType; set => chessType = value; }
    public bool IsUp { get => isUp; set => isUp = value; }
    public bool IsLive { get => isLive; set => isLive = value; }
    public bool IsSelected { get => isSelected; set => isSelected = value; }
}
