using System;
using System.Collections.Generic;
using System.Text;

namespace TextRPG_SpartaDungeon
{
    class SceneController
    {
        SceneBackground[] sceneBackground;
        SceneEvent[] sceneEvent;
        SB SB_Current;
        SE SE_Current;

        ScreenController screenController;
        KeyController keyController;

        public SceneController()
        {
            sceneBackground = new SceneBackground[(int)SB.Max];
            sceneEvent = new SceneEvent[(int)SE.Max];

            SB_Current = SB.MainMenu;
            SE_Current = SE.MainMenu_Default;

            screenController = new ScreenController();
            keyController = new KeyController();
        }

        public void Start() //게임 루프
        {
            SB SB_Next;
            SE SE_Next;

            while (true)
            {
                //씬 인스턴스 없으면 생성
                if (sceneBackground[(int)SB_Current] == null) sceneBackground[(int)SB_Current] = new SceneBackground(SB_Current);
                if (sceneEvent[(int)SE_Current] == null) sceneEvent[(int)SE_Current] = new SceneEvent(SE_Current);

                for (int y = 0; y < Const.screenH; y++)
                {
                    screenController.WriteLine(y, sceneBackground[(int)SB_Current].screen[y]);
                }

                if (sceneEvent[(int)SE_Current].Execute(out SB_Next, out SE_Next, screenController, keyController) == false)
                {
                    break; //게임 종료
                }
                else
                {
                    SB_Current = SB_Next;
                    SE_Current = SE_Next;
                }
            }
        }
    }



    class ScreenController
    {
        int[,] screenCurrent;
        int[,] screenUpdate;

        public ScreenController()
        {
            Console.SetWindowSize(Const.screenW + 1, Const.screenH + 1);

            screenCurrent = new int[Const.screenH, Const.screenW];
            screenUpdate = new int[Const.screenH, Const.screenW];

            for (int y = 0; y < Const.screenH; y++)
            {
                for (int x = 0; x < Const.screenW; x++)
                {
                    screenCurrent[y, x] = 32;
                    screenUpdate[y, x] = 32;
                }
            }
            Update();
        }

        //지우기
        public void ClearAll()
        {
            for (int y = 0; y < Const.screenH; y++)
            {
                for (int x = 0; x < Const.screenW; x++)
                {
                    screenUpdate[y, x] = 32;
                }
            }
        }
        public void ClearLine(int _y)
        {
            if (_y < 0 || _y >= Const.screenH)
            {
                return;
            }

            for (int x = 0; x < Const.screenW; x++)
            {
                screenUpdate[_y, x] = 32;
            }
        }
        //덮어쓰기
        public void WriteLine(int _y, string _s)
        {
            if (_y < 0 || _y >= Const.screenH)
            {
                return;
            }

            int len = _s.Length;
            int l = 0;

            int x;
            for (x = 0; l < len && x < Const.screenW; x++)
            {
                int c = (int)_s[l];
                if (c > 128) //2칸 차지
                {
                    if (x + 1 < Const.screenW) //2칸 쓰기
                    {
                        screenUpdate[_y, x] = c;
                        screenUpdate[_y, ++x] = 0;
                        l++;
                    }
                    else //공백으로 처리
                    {
                        screenUpdate[_y, x] = 32;
                        break;
                    }
                }
                else
                {
                    screenUpdate[_y, x] = c;
                    l++;
                }
            }

            if (l == len) //주어진 글 다 쓰고 자리가 남으면 공백으로 채움
            {
                for (int i = x; i < Const.screenW; i++)
                {
                    screenUpdate[_y, i] = 32;
                }
            }
        }
        public void Write(int _y, int _x, int _w, string _s)
        {
            if (_y < 0 || _y >= Const.screenH)
            {
                return;
            }

            int x_end = (Const.screenW < _x + _w) ? Const.screenW : _x + _w;

            int len = _s.Length;
            int l = 0;

            int x;
            for (x = _x; l < len && x < x_end; x++)
            {
                int c = (int)_s[l];
                if (c > 128) //2칸 차지
                {
                    if (x + 1 < x_end) //2칸 쓰기
                    {
                        screenUpdate[_y, x] = c;
                        screenUpdate[_y, ++x] = 0;
                        l++;
                    }
                    else //공백으로 처리
                    {
                        screenUpdate[_y, x] = 32;
                        break;
                    }
                }
                else
                {
                    screenUpdate[_y, x] = c;
                    l++;
                }
            }

            if (l == len) //주어진 글 다 쓰고 자리가 남으면 공백으로 채움
            {
                for (int i = x; i < x_end; i++)
                {
                    screenUpdate[_y, i] = 32;
                }
            }
        }
        //최종 적용
        public void Update()
        {
            for (int y = 0; y < Const.screenH; y++)
            {
                for (int x = 0; x < Const.screenW; x++)
                {
                    if (screenCurrent[y, x] != screenUpdate[y, x])
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write((char)screenUpdate[y, x]);
                        screenCurrent[y, x] = screenUpdate[y, x];
                    }
                }
            }
            Console.SetCursorPosition(0, 25);
        }
    }



    class KeyController
    {
        //치트코드는 영문자와 스페이스만 받음
        int[] cheat1 = null;
        int[] cheat2 = null;
        int[] cheat3 = null;
        int cheat1Fill = 0;
        int cheat2Fill = 0;
        int cheat3Fill = 0;


        public ConsoleKey GetUserInput(ConsoleKey[] filter, out int cheatActive)
        {
            ConsoleKey keyReturn = 0;
            ConsoleKey keyInput;
            cheatActive = 0;

            while (Console.KeyAvailable)
            {
                keyInput = Console.ReadKey(true).Key;
                foreach (ConsoleKey k in filter)
                {
                    if (keyInput == k) keyReturn = keyInput;
                }

                //치트 검사
                if (cheat1 != null)
                {
                    if (cheat1Fill < cheat1.Length)
                    {
                        if (cheat1[cheat1Fill] == (int)keyInput)
                        {
                            cheat1Fill++;
                            if(cheat1Fill == cheat1.Length)
                            {
                                cheatActive = 1;
                                cheat1Fill = 0;
                            }
                        }
                        else
                        {
                            cheat1Fill = 0;
                        }
                    }
                }
                if (cheat2 != null)
                {
                    if (cheat2Fill < cheat2.Length)
                    {
                        if (cheat2[cheat2Fill] == (int)keyInput)
                        {
                            cheat2Fill++;
                            if (cheat2Fill == cheat2.Length)
                            {
                                cheatActive = 2;
                                cheat2Fill = 0;
                            }
                        }
                        else
                        {
                            cheat2Fill = 0;
                        }
                    }
                }
                if (cheat3 != null)
                {
                    if (cheat3Fill < cheat3.Length)
                    {
                        if (cheat3[cheat3Fill] == (int)keyInput)
                        {
                            cheat3Fill++;
                            if (cheat3Fill == cheat3.Length)
                            {
                                cheatActive = 3;
                                cheat3Fill = 0;
                            }
                        }
                        else
                        {
                            cheat3Fill = 0;
                        }
                    }
                }
            }

            return keyReturn;
        }

        public void ClearCheat()
        {
            cheat1 = null;
            cheat2 = null;
            cheat3 = null;
            cheat1Fill = 0;
            cheat2Fill = 0;
            cheat3Fill = 0;
        }
        public void SetCheat(int n, string s)
        {
            if (n < 1 || n > 3)
            {
                return;
            }

            List<int> cheatCodeInt = new List<int>();
            string cheatCodeStr = s.ToUpper();

            for (int i = 0; i < cheatCodeStr.Length; i++)
            {
                if (cheatCodeStr[i] == ' ' || ('A' <= cheatCodeStr[i] && cheatCodeStr[i] <= 'Z'))
                {
                    cheatCodeInt.Add(cheatCodeStr[i]);
                }
            }

            switch (n)
            {
                case 1: cheat1 = cheatCodeInt.ToArray(); break;
                case 2: cheat2 = cheatCodeInt.ToArray(); break;
                case 3: cheat3 = cheatCodeInt.ToArray(); break;
            }
        }
    }
}
