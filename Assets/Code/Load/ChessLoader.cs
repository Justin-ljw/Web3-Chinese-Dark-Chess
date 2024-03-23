using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChessLoader
{
    private static ChessLoader chessLoader;
    private static UnityEngine.Object locker = new UnityEngine.Object();

    private ChessLoader() { }

    //����ģʽ
    public static ChessLoader GetChessLoader()
    {
        lock(locker)
        {
            if (chessLoader == null)
            {
                chessLoader = new ChessLoader();
            }

            return chessLoader;
        }
    }

    public void LoadChess()
    {
        //��ȡ���̹�����
        BoardClass boardClass = BoardClass.GetBoard();

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

        System.Random random = new System.Random((int)System.DateTime.Now.Ticks);

        //��ȡԤ�Ƽ�
        GameObject chessPrefab = Resources.Load("Prefab/Chess") as GameObject;


        //ʵ��������
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                int index = random.Next(0, chessName.Count);

                GameObject chess = GameObject.Instantiate(chessPrefab, boardClass.BoardManager[i, j].transform.position, Quaternion.identity);

                chess.transform.GetChild(0).transform.GetChild(0).name = chessName[index];

                //�����ű������������ֽ����Զ���������
                chess.transform.GetChild(0).transform.GetChild(0).GetComponent<ChessClass>().enabled = true;

                boardClass.ChessManager[i, j] = chess.transform.Find("Chess/" + chessName[index]).gameObject;
                chessName.RemoveAt(index);
            }
        }
    }

    public void LoadPlayer()
    {
        PlayerClass player = PlayerClass.GetPlayer();
        PlayerClass enemy = PlayerClass.GetEnemy();

        player.Init();
        enemy.Init();
    }

    public void LoadRecord()
    {
        //UI����ʣ��
        Dictionary<string, int> record = new Dictionary<string, int>() {
        {"Black_SHUAI",1 },{"Black_SHI",2},{"Black_XIANG",2},{"Black_JU",2},{"Black_MA",2},{"Black_PAO",2},{"Black_BING",5},
        {"Red_SHUAI",1 },{"Red_SHI",2},{"Red_XIANG",2},{"Red_JU",2},{"Red_MA",2},{"Red_PAO",2},{"Red_BING",5}
    };
        foreach(string name in record.Keys)
        {
            GameObject chessnum = GameObject.Find(name);
            chessnum.GetComponentInChildren<TextMeshProUGUI>().text = "x" + Convert.ToString(record[name]);
        }
}
}
