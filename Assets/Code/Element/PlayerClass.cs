using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClass
{
    private bool isMe;//�Ƿ�Ϊ�ҷ���trueΪ�ҷ���falseΪ�з���
    private bool chessColor;//������ɫ��trueΪ��ɫ��falseΪ��ɫ���췽���У�
    private int hp;//Ѫ��
    private string playerName = "";//�������

    public bool IsMe { get => isMe; set => isMe = value; }
    public bool ChessColor { get => chessColor; set => chessColor = value; }
    public int Hp { get => hp; set => hp = value; }
    public string PlayerName { get => playerName; }

    //��̬���ԣ�����ͬ�๲��һ��
    //��Ҷ���
    private static PlayerClass player = null;
    //���˶���
    private static PlayerClass enemy = null;
    //�߳����������ڸ��߳�����
    private static Object locker = new Object();

    //��ȡΨһ����Ҷ���
    public static PlayerClass GetPlayer()
    {
        lock(locker)
        {
            if(player == null)
            {
                player = new PlayerClass();

                player.IsMe = true;
                ///������
                player.ChessColor = true;
            }

            return player;
        }
    }

    //��ȡΨһ�ĵ��˶���
    public static PlayerClass GetEnemy()
    {
        lock (locker)
        {
            if(enemy == null)
            {
                enemy = new PlayerClass();

                enemy.IsMe = false;
                ///������
                enemy.ChessColor = false;
            }

            return enemy;
        }
    }

    //������new��ֻ��ͨ��get���Ψһ����Ҷ����Ψһ�ĵ��˶���
    private PlayerClass(){}

    //��ʼ������
    //����ʹ��˫���࣬����ʹ�þ�̬������
    //�ڶ������½��뵱ǰ����ʱ��������ڴ治���������������ı����ᣬ����Ҫ���³�ʼ��һ��
    //���Թ��캯���ͳ�ʼ�������ֿ����ã�ÿ�ν������峡������ʼ��һ�±���
    public void Init()
    {
        //��ʼ��Ѫ��
        hp = 60;
    }
}
