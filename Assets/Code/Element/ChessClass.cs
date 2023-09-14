using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//һ�����Ӷ�����3�㣺
//���ڲ������ӵ�ʵ��ģ�ͣ�������ײ�Ǹò������壩
//���������һ�㸸��������������ģ�͵�����ƫ�ƣ��������������һ�㣩
//����㸸�������ڽ���������ᷢ��λ��ʱ��ʹ��ͬһ����������ȫ���䵽ͬһλ�õ����⣨�����ƶ�ʱ��������������transform
public class ChessClass : MonoBehaviour
{
    private int x;//���������ӹ������е�X����
    private int y;//���������ӹ������е�Y����
    private bool colorType;//���ӵ���ɫ��trueΪ��ɫ��falseΪ��ɫ��
    private ChessType chessType;//���ӵ�����
    private bool isUp = false;//�����Ƿ���
    private bool isLive = true;//�����Ƿ���
    private bool isSelected = false;//�����Ƿ�ѡ��
    private Vector3 target = Vector3.zero;//����Ҫ�ƶ�����Ŀ�����꣨�����Ϊzero�����������ƶ�������Ϊ�����ƶ���
    private BoardClass boardClass;//���̹�����
    private Animator animator;//���������
    private AnimatorStateInfo animatorStateInfo;//���ڲ��ŵĶ���
    private bool isAnimating;//�������Ƿ��ж������ڽ���

    private void Start()
    {
        Init();
    }

    /*public ChessClass()
    {
        Init();
    }*/

    //ͨ�����ӵ���������ʼ������
    //��������:[��ɫ.��������.����������1234��]
    public void Init()
    {
        string[] values =  gameObject.name.Split(".");

        //����������ɫ
        if(values[0].Equals("Red"))
        {
            colorType = true;
        }
        else if(values[0].Equals("Black"))
        {
            colorType = false;
        }

        //������������
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

        //����mesh��material��tag
        GetComponent<MeshFilter>().mesh = (Mesh)Resources.Load("Mesh/" + values[1]);

        Material[] mat = new Material[2];
        mat[0]= (Material)Resources.Load("Materials/chess");
        mat[1] = (Material)Resources.Load("Materials/" + values[0]);
        GetComponent<MeshRenderer>().materials = mat;

        tag = values[0] + "Chess";

        //��ȡ���̹�����
        boardClass = BoardClass.GetBoard();

        //��ȡ�������������ϵ�����
        boardClass.GetChessIndex(gameObject, out x, out y);

        //��ȡ������������ڸ������ϣ�
        animator = transform.parent.GetComponent<Animator>();
        //��ȡ���ڲ��ŵĶ���
        animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
    }

    public void Select()
    {
        if(!IsSelected)
        {
            //��ȡ����Ķ������
            //�����������������������ĸ����壬��������ڸ������У�
            Animator animator = transform.parent.GetComponent<Animator>(); 

            //�������Ϊѡ�У��������������Ӷ����Ϊδѡ��
            IsSelected = true;

            //��ʾ�߹�
            AddHighLight();
            //ѡ�ж���������ѡ�ж���
            animator.SetBool("isSelected", true);
        }
    }

    //ȡ��ѡ������
    public void Unselect()
    {
        if(IsSelected)
        {
            //��ȡ����Ķ������
            //�����������������������ĸ����壬��������ڸ������У�
             Animator animator = transform.parent.GetComponent<Animator>();

            //���Ϊδѡ��
            IsSelected = false;

            //ȡ����ʾ�߹�
            RemoveHighLight();

            //ȡ��ѡ�ж���
            animator.SetBool("isSelected", false);
        }
    }

    /*public void UnselectOtherChess()
    {
        //�������ӱ��Ϊδѡ��
        GameObject[] otherChesses = null;
        //���ݱ����ӵ���ɫ��ȡ���б���ɫ������
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
            //�������б���ɫ�����ӣ������˸��������������������Ϊδѡ��
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

    //��������
    public void Up()
    {
        //��ȡ����Ķ������
        //�����������������������ĸ����壬��������ڸ������У�
        Animator animator = transform.parent.GetComponent<Animator>();

        //�������ӷ���״̬��ȡ��ѡ��״̬
        IsUp = true;
        IsSelected = false;

        //ȡ����ʾ�߹�
        RemoveHighLight();
        //���𶯻�:������������������ɺ�ȡ��ѡ��״̬����ȡ��ѡ�ж���
        animator.SetTrigger("isUp");
        animator.SetBool("isSelected", false);
    }

    //��ʼ�ƶ����ӣ�����Ҫ�ƶ��������̸��ӣ�
    //�����Ƿ�����ƶ���trueΪ���ƶ���falseΪ�����ƶ���
    public bool Move(GameObject boardElement)
    {
        //������û�����ƶ�,���ܿ�ʼ�ƶ�
        if (target == Vector3.zero)
        {
            //�����Ӳ����ڣ����ҵ�������̸��ӿɴ����������ʱ�����Ӳſ�ʼ�ƶ�
            if ((ChessType != ChessType.PAO && CanArrive(boardElement)) || ChessType == ChessType.PAO)
            {
                //��ȡ��������̸��ӵ�����
                int board_i, board_j;

                boardClass.GetBoardIndex(boardElement, out board_i, out board_j);

                //�����ӹ������У������ӷŵ�Ŀ�ĸ��ӵ�������������ԭ�����������Ϊnull
                boardClass.ChessManager[board_i, board_j] = gameObject;
                boardClass.ChessManager[x, y] = null;
                //�ѱ����ӵ�������Ϊ��������̸��ӵ�����
                x = board_i;
                y = board_j;

                //��target��ֵ��ʾ�����������ƶ�
                //�����ƶ�ʱֻ�ı�x��z����
                target = new Vector3(boardElement.transform.position.x, 0.15f, boardElement.transform.position.z);

                return true;
            }
        }

        return false;
    }

    //�����ƶ��У�ʱ���ж������Ƿ���Ҫ�ƶ���
    public int Moving()
    {
        //���������ƶ�
        if (target != Vector3.zero)
        {

            //�����ƶ�ʱ������������ʵ��ģ�͵ĸ�����ĸ�����
            Transform t = transform.parent.parent;

            //�����ƶ���Ŀ�ĵ�
            if (t.position == target)
            {
                //���ӵ���Ŀ�ĵغ�ȡ��ѡ��
                Unselect();

                target = Vector3.zero;
                
                return -1;
            }

            //�����ƶ�ʱֻ�ı�x��z����
            t.Translate((target - t.position) * Time.deltaTime * 50f);

            return 1;
        }

        return 0;
    }

    //�ж��Ƿ����ӿ��Ե���ĳһ���̸��ӣ�����Ҫ�ƶ��������̸��ӣ�
    private bool CanArrive(GameObject boardElement)
    {
        //��ȡ��������̸��ӵ�����ͱ����ӵ�����
        int board_i, board_j;

        boardClass.GetBoardIndex(boardElement, out board_i, out board_j);

        //�����������̸��ӵ������ڱ�������������1����ɴ���򲻿ɴ�
        if ((board_i == x && (board_j == y + 1 || board_j == y - 1)) ||
           (board_j == y && (board_i == x + 1 || board_i == x - 1)))
        {
            return true;
        }

        return false;
    }

    //�ж����Ƿ���Գ��壺�ж����뱻������֮���Ƿ���һ�����ӣ����뱻��������ӣ�
    public bool IsPaoCanEat(ChessClass chessObject)
    {
        //����Ƿ���Ա��ԣ����뱻������֮���Ƿ�ֻ��һ�����ӣ�
        bool isCanEat = false;
        //�ж����뱻������֮���Ƿ�������һ�����ӣ�û������ʱΪfalse��������һ������ʱΪtrue��
        bool haveChess = false;

        //��������Ӻ�����ͬһ��
        if (chessObject.X == X)
        {
            //��ȡ���������н�С��j�ͽϴ��j
            int min_j = Mathf.Min(chessObject.Y, Y);
            int max_j = Mathf.Max(chessObject.Y, Y);

            //����ͬһ���У�������֮������̸��ӣ����������������ӱ���,�ж��Ƿ�ֻ��һ������
            for (int j = min_j + 1; j < max_j; j++)
            {
                //������һ������ʱ����ǿɳԣ���������������ӣ�haveChess��
                if (!haveChess && boardClass.ChessManager[X, j] != null)
                {
                    haveChess = true;
                    isCanEat = true;
                }
                //�����Ѿ�����������֮�����������ӣ��ͱ�ǲ��ɳ�
                else if (haveChess && boardClass.ChessManager[X, j] != null)
                {
                    isCanEat = false;
                }
            }
        }
        //��������Ӻ�����ͬһ��
        else if (chessObject.Y == Y)
        {
            //��ȡ���������н�С��j�ͽϴ��j
            int min_i = Mathf.Min(chessObject.X, X);
            int max_i = Mathf.Max(chessObject.X, X);

            //����ͬһ���У�������֮������̸��ӣ����������������ӱ���,�ж��Ƿ�ֻ��һ������
            for (int i = min_i + 1; i < max_i; i++)
            {
                //������һ������ʱ����ǿɳԣ���������������ӣ�haveChess��
                if (!haveChess && boardClass.ChessManager[i, Y] != null)
                {
                    haveChess = true;
                    isCanEat = true;
                }
                //�����Ѿ�����������֮�����������ӣ��ͱ�ǲ��ɳ�
                else if (haveChess && boardClass.ChessManager[i, Y] != null)
                {
                    isCanEat = false;
                }
            }
        }

        return isCanEat;
    }

    //��ѡ��ʱ��Ӹ���
    private void AddHighLight()
    {
        if(GetComponent<Outline>() == null)
        {
            Outline outline = gameObject.AddComponent<Outline>();
            outline.OutlineColor = Color.red;//���ø�������ɫ
            outline.OutlineWidth = 7.5f;//���ø����Ŀ��
        }
    }

    //û�б�ѡ��ʱȡ������
    private void RemoveHighLight()
    {
        if(GetComponent<Outline>() != null)
        {
            Destroy(GetComponent<Outline>());
        }
    }

    //������⣺�սӴ�ʱ����
    //�����ӱ���ʱ����
    private void OnTriggerEnter(Collider other)
    {
        //���������ײʱ�����ӵ�����״̬Ϊ�������������Ǳ��Ե����ӣ������ǳ��������
        if (!IsLive)
        {
            //�ѱ����ӷ����������ӹ�����
            //�����ӹ������б��������ڵ��������ڳ�������Move()ʱ���滻Ϊ�������ӣ�
            boardClass.AddDeadChess(gameObject);

            //�жϱ����ӵ���ɫ���������۳���Ӧһ����ұ����Ӷ�Ӧ��Ѫ��
            if(colorType == PlayerClass.GetPlayer().ChessColor)
            {
                PlayerClass.GetPlayer().Hp -= boardClass.GetChessHp(ChessType);
            }
            else
            {
                PlayerClass.GetEnemy().Hp -= boardClass.GetChessHp(ChessType);
            }

            //��ȡ���ӵ�����㸸����
            GameObject outObject = transform.parent.parent.gameObject;
            //���ٱ�����������ص����壨�ɸ�ΪisActive = false��
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
