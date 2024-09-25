using System;
using System.Collections.Generic;

namespace TextRPG_SpartaDungeon
{
    class Const
    {
        public const int screenH = 25;
        public const int screenW = 100;

        public const string saveFileName = "SaveData";
    }

    class Program
    {
        public static GameData gameData, gameData1, gameData2, gameData3; //실행중인 인스턴스, 불러온 게임123
        public static ItemData globalItemData = new ItemData();

        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            SceneController sceneController = new SceneController();

            sceneController.Start();

            Console.CursorVisible = true;
        }
    }




}
