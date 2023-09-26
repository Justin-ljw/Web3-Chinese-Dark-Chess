using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClass
{
    private bool isMe;//是否为我方（true为我方，false为敌方）
    private bool chessColor;//棋子颜色（true为红色，false为黑色，红方先行）
    private int hp;//血量
    private string playerName = "";//玩家名称

    public bool IsMe { get => isMe; set => isMe = value; }
    public bool ChessColor { get => chessColor; set => chessColor = value; }
    public int Hp { get => hp; set => hp = value; }
    public string PlayerName { get => playerName; }

    //静态属性，所用同类共享一个
    //玩家对象
    private static PlayerClass player = null;
    //敌人对象
    private static PlayerClass enemy = null;
    //线程锁对象：用于给线程上锁
    private static Object locker = new Object();

    //获取唯一的玩家对象
    public static PlayerClass GetPlayer()
    {
        lock(locker)
        {
            if(player == null)
            {
                player = new PlayerClass();

                player.IsMe = true;
                ///测试用
                player.ChessColor = true;
            }

            return player;
        }
    }

    //获取唯一的敌人对象
    public static PlayerClass GetEnemy()
    {
        lock (locker)
        {
            if(enemy == null)
            {
                enemy = new PlayerClass();

                enemy.IsMe = false;
                ///测试用
                enemy.ChessColor = false;
            }

            return enemy;
        }
    }

    //不允许new，只能通过get获得唯一的玩家对象和唯一的敌人对象
    private PlayerClass(){}

    //初始化方法
    //由于使用双例类，必须使用静态变量，
    //第二次重新进入当前场景时，本类的内存不会清除，但类里面的变量会，所以要重新初始化一下
    //所以构造函数和初始化函数分开调用，每次进入下棋场景都初始化一下本类
    public void Init()
    {
        //初始化血量
        hp = 60;
    }
}
