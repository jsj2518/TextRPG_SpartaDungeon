using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TextRPG_SpartaDungeon
{
    enum TM //마을 메뉴
    {
        Status,
        Dungen,
        GearShop,
        PotionShop,
        Inn,
        Mercenary,
        Mansion,
        Menu
    }

    class ScenePreset
    {
        public static void VoidConversation(string[] conversationPreset, ScreenController screenController, KeyController keyController)
        {
            ConsoleKey[] keyFilter = new ConsoleKey[] { ConsoleKey.NoName };
            keyController.GetUserInput(keyFilter, out int cheatActivate);

            ConsoleKey keyInput;
            keyFilter = new ConsoleKey[] { ConsoleKey.Z };
            screenController.Write(23, 76, 8, "Z : 확인");
            for (int i = 0; i < conversationPreset.Length; i++)
            {
                screenController.Write(12, 10, Const.screenW - 10, conversationPreset[i]);
                screenController.Update();
                while (true)
                {
                    keyInput = keyController.GetUserInput(keyFilter, out cheatActivate);
                    if (keyInput == ConsoleKey.Z) break;
                }
            }
        }

        public static TM TownDefault(GameData gameData, ScreenController screenController, KeyController keyController)
        {
            List<TM> townMenu = new List<TM>();
            townMenu.Add(TM.Status);
            townMenu.Add(TM.Dungen);
            townMenu.Add(TM.GearShop);
            townMenu.Add(TM.PotionShop);
            townMenu.Add(TM.Inn);
            if (gameData.GF_MercenaryUnlock > 0) townMenu.Add(TM.Mercenary);
            if (gameData.GF_MansionUnlock > 0) townMenu.Add(TM.Mansion);
            townMenu.Add(TM.Menu);

            screenController.Write(18, 80, 17, $"소지금 : {gameData.PP_Gold,6} G");

            screenController.Write(21, 10, 50, "크게 발달하진 않았지만 활기찬 마을이다.");
            string strMenu = "";
            int lineSpacing = (townMenu.Count <= 7) ? 2 : 1; //선택지 8개 이상이면 간격 줄임
            for (int i = 0; i < townMenu.Count; i++)
            {
                switch (townMenu[i])
                {
                    case TM.Status: strMenu = "스테이터스"; break;
                    case TM.Dungen: strMenu = "던전"; break;
                    case TM.GearShop: strMenu = "장비 상점"; break;
                    case TM.PotionShop: strMenu = "포션 상점"; break;
                    case TM.Inn: strMenu = "여관"; break;
                    case TM.Mercenary: strMenu = "용병 거래소"; break;
                    case TM.Mansion: strMenu = (gameData.GF_MansionUnlock == 1) ? "수상한 저택" : "뱀 애호가의 저택"; break;
                    case TM.Menu: strMenu = "메뉴"; break;
                }
                screenController.Write(i * lineSpacing + 1, 77, 20, $"{i + 1}. {strMenu}");
            }
            screenController.Write(15, 85, 12, $"(1 ~ {townMenu.Count} 선택)");
            screenController.Update();

            ConsoleKey[] keyFilter = new ConsoleKey[] { ConsoleKey.NoName };
            keyController.GetUserInput(keyFilter, out int cheatActivate);
            //돈 치트 추가
            keyController.SetCheat(1, "show me the money");

            ConsoleKey keyInput;
            keyFilter = new ConsoleKey[] { ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.D3, ConsoleKey.D4, ConsoleKey.D5, ConsoleKey.D6, ConsoleKey.D7, ConsoleKey.D8 };
            while (true)
            {
                keyInput = keyController.GetUserInput(keyFilter, out cheatActivate);
                //돈 치트 발동
                if (cheatActivate == 1)
                {
                    gameData.PP_Gold += 10000;
                    screenController.Write(18, 80, 17, $"소지금 : {gameData.PP_Gold,6} G");
                    screenController.Update();
                }

                if (ConsoleKey.D1 <= keyInput && keyInput <= ConsoleKey.D8)
                {
                    int select = (int)keyInput - (int)ConsoleKey.D1;
                    if (select < townMenu.Count) //유효값 추려냄
                    {
                        keyController.ClearCheat();
                        return townMenu[select];
                    }
                }
            }
        }

        public static void StatusMenu(GameData gameData, ScreenController screenController, KeyController keyController, ref SB SB_Next, ref SE SE_Next)
        {
            int selectX = 0; //0, 1, 2
            int selectY = 0;

            ConsoleKey[] keyFilter = new ConsoleKey[] { ConsoleKey.NoName };
            keyController.GetUserInput(keyFilter, out int cheat);
            StatusMenuUpdate(selectX, selectY, gameData, screenController);
            //레벨업 치트 추가
            keyController.SetCheat(1, "level up");

            ConsoleKey keyInput;
            int[] yMin = new int[3];
            int[] yMax = new int[3];
            yMin[0] = 0;
            yMax[0] = (gameData.mercenary2 == null) ? (gameData.mercenary1 == null) ? 2 : 3 : 4;
            yMin[1] = 0;
            yMax[1] = 4;
            yMin[2] = -1;
            yMax[2] = 8;
            keyFilter = new ConsoleKey[] { ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.LeftArrow, ConsoleKey.RightArrow, ConsoleKey.Z, ConsoleKey.X };
            while (true)
            {
                keyInput = keyController.GetUserInput(keyFilter, out int cheatActivate);
                //돈 치트 발동
                if (cheatActivate == 1)
                {
                    gameData.LevelUP();
                    gameData.PP_Exp = 0;
                }

                switch (keyInput)
                {
                    case ConsoleKey.UpArrow:
                        selectY = (selectY == yMin[selectX]) ? selectY : selectY - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        selectY = (selectY == yMax[selectX]) ? selectY : selectY + 1;
                        break;
                    case ConsoleKey.LeftArrow:
                        selectX = (selectX == 0) ? 0 : selectX - 1;
                        selectY = (selectY < yMin[selectX]) ? yMin[selectX] : (selectY > yMax[selectX]) ? yMax[selectX] : selectY;
                        break;
                    case ConsoleKey.RightArrow:
                        selectX = (selectX == 2) ? 2 : selectX + 1;
                        selectY = (selectY < yMin[selectX]) ? yMin[selectX] : (selectY > yMax[selectX]) ? yMax[selectX] : selectY;
                        break;
                    case ConsoleKey.Z:
                        if (selectX == 0)
                        {
                            if (selectY == 0) //인벤토리
                            {
                                SB_Next = SB.Inventory;
                                SE_Next = SE.Inventory_Inventory;

                                keyController.ClearCheat();
                                return;
                            }
                            else if (selectY == 1)  //장비
                            {
                                SB_Next = SB.Inventory;
                                SE_Next = SE.Inventory_Gear;

                                keyController.ClearCheat();
                                return;
                            }
                            else if (selectY == 2) ; //여기 스킬
                        }
                        else if (selectX == 1)
                        {
                            if (gameData.PA_Point > 0) //분배 가능한 포인트가 있음
                            {
                                gameData.PA_Point--;
                                if (selectY == 0) gameData.PA_Health++;
                                else if (selectY == 1) gameData.PA_Stamina++;
                                else if (selectY == 2) gameData.PA_Strength++;
                                else if (selectY == 3) gameData.PA_Agility++;
                                else gameData.PA_Dexterity++;
                                gameData.ApplyPS();
                            }
                        }
                        break;
                    case ConsoleKey.X:
                        SB_Next = SB.Town;
                        SE_Next = SE.Town_Default;

                        keyController.ClearCheat();
                        return;
                }

                StatusMenuUpdate(selectX, selectY, gameData, screenController);
            }
        }
        private static void StatusMenuUpdate(int selectX, int selectY, GameData gameData, ScreenController screenController)
        {
            for (int i = 0; i < 5; i++) screenController.Write(6 + i * 2, 4, 3, "");
            for (int i = 0; i < 5; i++) screenController.Write(6 + i * 2, 30, 3, "");
            for (int i = 0; i < 10; i++) screenController.Write(4 + i * 2, 71, 3, "");
            screenController.Write(4, 44, 20, "");
            for (int i = 0; i < 5; i++) screenController.Write(6 + i * 2, 43, 10, "");
            for (int i = 0; i < 10; i++) screenController.Write(4 + i * 2, 88, 10, "");
            for (int i = 0; i < 7; i++) screenController.Write(18 + i, 0, 67, "");

            screenController.Write(1, 11, 2, $"{gameData.PP_Level}");
            screenController.Write(1, 16, 14, gameData.PP_Name);
            screenController.Write(1, 33, 4, gameData.PP_ClassKor);
            screenController.Write(1, 53, 13, $"{gameData.PP_Exp} / {gameData.PP_ExpNextLevel}");
            screenController.Write(2, 53, 13, $"{gameData.PP_Gold} G");

            screenController.Write(8, 14, 12, $"(무게 : {gameData.PS_Burden})");
            if (gameData.mercenary1 != null)
                screenController.Write(12, 8, 6, "/용병1");
            if (gameData.mercenary2 != null)
                screenController.Write(12, 8, 6, "/용병2");

            if (gameData.PA_Point != 0)
            {
                screenController.Write(4, 44, 20, $"(스텟 포인트 : {gameData.PA_Point})");
            }
            screenController.Write(6, 43, 3, $"{gameData.PA_Health}");
            screenController.Write(8, 43, 3, $"{gameData.PA_Stamina}");
            screenController.Write(10, 43, 3, $"{gameData.PA_Strength}");
            screenController.Write(12, 43, 3, $"{gameData.PA_Agility}");
            screenController.Write(14, 43, 3, $"{gameData.PA_Dexterity}");

            screenController.Write(4, 88, 9, $"{gameData.PS_HPCur} / {gameData.PS_HPMax}");
            screenController.Write(6, 88, 3, $"{gameData.PS_SP}");
            screenController.Write(8, 88, 3, $"{gameData.PS_Attack}");
            screenController.Write(10, 88, 3, $"{gameData.PS_Defense}");
            screenController.Write(12, 88, 3, $"{gameData.PS_Speed}");
            screenController.Write(14, 88, 3, $"{gameData.PS_MeleeAcc}");
            screenController.Write(16, 88, 10, $"{gameData.PS_RangeAcc} ({gameData.PS_RangeAccN})");
            screenController.Write(18, 88, 3, $"{gameData.PS_Evasion}");
            screenController.Write(20, 88, 7, $"{gameData.PS_Critical:N1} %");
            screenController.Write(22, 88, 7, $"{gameData.PS_Block:N1} %");

            switch (selectX)
            {
                case 0:
                    switch (selectY)
                    {
                        case 0:
                            screenController.Write(6, 4, 3, "-->");
                            screenController.Write(19, 3, 61, "인벤토리 : 보유한 소모품을 확인할 수 있습니다.");
                            break;
                        case 1:
                            screenController.Write(8, 4, 3, "-->");
                            screenController.Write(19, 3, 61, "장비 : 보유한 장비와 현재 착용중인 장비를 확인할 수 있습니다.");
                            break;
                        case 2:
                            screenController.Write(10, 4, 3, "-->");
                            screenController.Write(19, 3, 61, "스킬 : 플레이어가 사용할 수 있는 스킬 목록입니다.");
                            break;
                        case 3:
                            screenController.Write(12, 4, 3, "-->");
                            //여기 용병1 스펙
                            break;
                        case 4:
                            screenController.Write(14, 4, 3, "-->");
                            //여기 용병2 스펙
                            break;
                    }
                    break;
                case 1:
                    switch (selectY)
                    {
                        case 0:
                            screenController.Write(6, 30, 3, "-->");
                            screenController.Write(19, 3, 61, "건강 : 최대 HP를 증가시킵니다.");
                            break;
                        case 1:
                            screenController.Write(8, 30, 3, "-->");
                            screenController.Write(19, 3, 61, "체력 : 최대 SP를 증가시킵니다.");
                            break;
                        case 2:
                            screenController.Write(10, 30, 3, "-->");
                            screenController.Write(19, 3, 61, "힘 : 공격력을 증가시킵니다.");
                            screenController.Write(20, 3, 61, "     장비 무게에 따른 스피드 감소를 줄입니다.");
                            screenController.Write(21, 3, 61, "     원거리 명중률을 감소시킵니다.");
                            break;
                        case 3:
                            screenController.Write(12, 30, 3, "-->");
                            screenController.Write(19, 3, 61, "민첩 : 스피드, 근접명중, 회피를 증가시킵니다.");
                            screenController.Write(20, 3, 61, "       도적 클래스 스킬에 영향을 줍니다.");
                            break;
                        case 4:
                            screenController.Write(14, 30, 3, "-->");
                            screenController.Write(19, 3, 61, "손재주 : 원거리명중, 크리티컬을 증가시킵니다.");
                            screenController.Write(20, 3, 61, "         궁수 클래스 스킬에 영향을 줍니다.");
                            break;
                    }
                    break;
                case 2:
                    switch (selectY)
                    {
                        case -1:
                            screenController.Write(4, 71, 3, "-->");
                            screenController.Write(19, 3, 61, "HP : 적의 공격으로부터 버틸 수 있는 힘입니다.");
                            screenController.Write(20, 3, 61, "     건강 스텟으로 올릴 수 있습니다.");
                            break;
                        case 0:
                            screenController.Write(6, 71, 3, "-->");
                            screenController.Write(19, 3, 61, "SP : SP를 소모하여 스킬을 사용합니다.");
                            screenController.Write(20, 3, 61, "     전투 중 자동으로 회복됩니다.");
                            screenController.Write(21, 3, 61, "     체력 스텟으로 올릴 수 있습니다.");
                            break;
                        case 1:
                            screenController.Write(8, 71, 3, "-->");
                            screenController.Write(19, 3, 61, "공격력 : 적에게 가할 수 있는 피해량입니다.");
                            screenController.Write(20, 3, 61, "         힘 스텟으로 올릴 수 있습니다.");
                            break;
                        case 2:
                            screenController.Write(10, 71, 3, "-->");
                            screenController.Write(19, 3, 61, "방어력 : 받는 피해량을 감소시킵니다.");
                            screenController.Write(20, 3, 61, "         착용 장비의 영향만 받습니다.");
                            break;
                        case 3:
                            screenController.Write(12, 71, 3, "-->");
                            screenController.Write(19, 3, 61, "스피드 : AP(전투 중 행동 턴)가 빨리 차오르게 합니다.");
                            screenController.Write(20, 3, 61, "         이동 명령 시 AP 소모량이 줄어듭니다.");
                            screenController.Write(21, 3, 61, "         민첩 스텟으로 올릴 수 있습니다.");
                            screenController.Write(22, 3, 61, "         착용한 장비의 무게에 따라 감소합니다.");
                            break;
                        case 4:
                            screenController.Write(14, 71, 3, "-->");
                            screenController.Write(19, 3, 61, "근접명중 : 근접 공격의 명중률을 높입니다.");
                            screenController.Write(20, 3, 61, "           민첩 스텟으로 올릴 수 있습니다.");
                            break;
                        case 5:
                            screenController.Write(16, 71, 3, "-->");
                            screenController.Write(19, 3, 61, "원거리명중 : 원거리 공격의 명중률을 높입니다.");
                            screenController.Write(20, 3, 61, "             손재주 스텟으로 올릴 수 있습니다.");
                            screenController.Write(21, 3, 61, "             힘 스텟에 따라 감소합니다.");
                            screenController.Write(22, 3, 61, "             목표와의 거리가 멀수록 감소합니다.");
                            break;
                        case 6:
                            screenController.Write(18, 71, 3, "-->");
                            screenController.Write(19, 3, 61, "회피 : 적 공격으로부터의 회피율을 높입니다.");
                            screenController.Write(20, 3, 61, "       민첩 스텟으로 높일 수 있습니다.");
                            break;
                        case 7:
                            screenController.Write(20, 71, 3, "-->");
                            screenController.Write(19, 3, 61, "크리티컬 : 공격 적중 시 치명타가 발생할 확률입니다.");
                            screenController.Write(20, 3, 61, "           치명타 발생 시 상대의 방어를 절반 무시하고");
                            screenController.Write(21, 3, 61, "            1.5배의 피해를 가합니다.");
                            screenController.Write(22, 3, 61, "           손재주로 올릴 수 있습니다.");
                            break;
                        case 8:
                            screenController.Write(22, 71, 3, "-->");
                            screenController.Write(19, 3, 61, "방어확률 : 방어 성공 시 피해를 반으로 줄입니다.");
                            screenController.Write(20, 3, 61, "           치명타 발생보다 판정이 우선시 되며");
                            screenController.Write(21, 3, 61, "            방어 성공 시 치명타가 발생하지 않습니다.");
                            screenController.Write(22, 3, 61, "           착용 장비의 영향만 받습니다.");
                            break;
                    }
                    break;
            }

            screenController.Update();
        }

        public static int InventoryMenu(int tab, GameData gameData, ScreenController screenController, KeyController keyController, ref SB SB_Next, ref SE SE_Next)
        {
            int selectTab = tab; //0-인벤토리   1-장비   2-스킬
            int topItem = 0;
            int selectY = 0;
            int selectItem = 0;
            int gearSelect = 0; //장비선택창   0-선택안함, 1~7-장착해제 가능, 11~17-장비교체 가능
            bool boolVal = true;

            SB_Next = SB.Status; //나갈 곳은 스테이터스 창밖에 없음
            SE_Next = SE.Status_Default;

            ConsoleKey keyInput;
            ConsoleKey[] keyFilter = new ConsoleKey[] { ConsoleKey.NoName };
            keyController.GetUserInput(keyFilter, out int cheat);
            InventoryMenuUpdate(selectTab, topItem, selectY, gearSelect, gameData, screenController, keyController);

            int listCount;
            while (true) //한번에 10개 항목씩 표시 가능
            {
                if (selectTab == 0) //인벤토리
                {
                    listCount = gameData.inventoryPotion.Count;
                    listCount += gameData.inventoryGear.Count;
                    topItem = 0;
                    selectY = 0;

                    boolVal = true;
                    while (boolVal)
                    {
                        selectItem = topItem + selectY;

                        keyFilter = new ConsoleKey[] { ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.Z, ConsoleKey.X, ConsoleKey.Tab };
                        keyInput = keyController.GetUserInput(keyFilter, out int cheatActivate);
                        switch (keyInput)
                        {
                            case ConsoleKey.UpArrow:
                                if (selectItem == 0)
                                {
                                    if (listCount >= 10)
                                    {
                                        topItem = listCount - 10;
                                        selectY = 9;
                                    }
                                    else
                                    {
                                        topItem = 0;
                                        selectY = listCount - 1;
                                    }
                                }
                                else
                                {
                                    if (selectY == 0)
                                    {
                                        topItem--;
                                    }
                                    else
                                    {
                                        selectY--;
                                    }
                                }
                                break;
                            case ConsoleKey.DownArrow:
                                if (selectItem == listCount - 1)
                                {
                                    topItem = 0;
                                    selectY = 0;
                                }
                                else
                                {
                                    if (selectY == 9)
                                    {
                                        topItem++;
                                    }
                                    else
                                    {
                                        selectY++;
                                    }
                                }
                                break;
                            case ConsoleKey.Z:
                                if (selectItem < gameData.inventoryPotion.Count)
                                {
                                    if (gameData.inventoryPotion[selectItem].itemPotion.type == ITP.HP || gameData.inventoryPotion[selectItem].itemPotion.type == ITP.PP)
                                        gameData.UseItem(gameData.inventoryPotion[selectItem].itemPotion);
                                }
                                break;
                            case ConsoleKey.X:
                                return selectTab;
                            case ConsoleKey.Tab:
                                selectTab = 1;
                                boolVal = false;
                                break;
                        }

                        InventoryMenuUpdate(selectTab, topItem, selectY, gearSelect, gameData, screenController, keyController);
                    }
                }
                else if (selectTab == 1) //장비
                {
                    listCount = gameData.inventoryGear.Count;
                    topItem = 0;
                    selectY = 0;
                    gearSelect = 0;

                    boolVal = true;
                    while (boolVal)
                    {
                        selectItem = topItem + selectY;

                        keyFilter = new ConsoleKey[] { ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.LeftArrow, ConsoleKey.RightArrow, ConsoleKey.Z, ConsoleKey.X, ConsoleKey.Tab };
                        keyInput = keyController.GetUserInput(keyFilter, out int cheatActivate);
                        switch (keyInput)
                        {
                            case ConsoleKey.UpArrow:
                                if (listCount == 0) break;
                                if (gearSelect == 0)
                                {
                                    if (selectItem == 0)
                                    {
                                        if (listCount >= 10)
                                        {
                                            topItem = listCount - 10;
                                            selectY = 9;
                                        }
                                        else
                                        {
                                            topItem = 0;
                                            selectY = listCount - 1;
                                        }
                                    }
                                    else
                                    {
                                        if (selectY == 0)
                                        {
                                            topItem--;
                                        }
                                        else
                                        {
                                            selectY--;
                                        }
                                    }
                                }
                                else if (1 <= gearSelect && gearSelect <= 7)
                                {
                                    if (gearSelect == 1) gearSelect = 7;
                                    else gearSelect--;
                                }
                                else if (gearSelect == 12 && gameData.inventoryGear[selectItem].itemGear.type == ITG.DAGGER) //단도를 교체하고 싶을 때만 이동 가능하다
                                {
                                    gearSelect = 11;
                                }
                                break;
                            case ConsoleKey.DownArrow:
                                if (listCount == 0) break;
                                if (gearSelect == 0)
                                {
                                    if (selectItem == listCount - 1)
                                    {
                                        topItem = 0;
                                        selectY = 0;
                                    }
                                    else
                                    {
                                        if (selectY == 9)
                                        {
                                            topItem++;
                                        }
                                        else
                                        {
                                            selectY++;
                                        }
                                    }
                                }
                                else if (1 <= gearSelect && gearSelect <= 7)
                                {
                                    if (gearSelect == 7) gearSelect = 1;
                                    else gearSelect++;
                                }
                                else if (gearSelect == 11 && gameData.inventoryGear[selectItem].itemGear.type == ITG.DAGGER) //단도를 교체하고 싶을 때만 이동 가능하다
                                {
                                    gearSelect = 12;
                                }
                                break;
                            case ConsoleKey.LeftArrow:
                                if (1 <= gearSelect && gearSelect <= 7)
                                {
                                    gearSelect = 0;
                                }
                                break;
                            case ConsoleKey.RightArrow:
                                if (gearSelect == 0)
                                {
                                    gearSelect = 1;
                                }
                                break;
                            case ConsoleKey.Z:
                                if (listCount == 0) break;
                                if (gearSelect == 0) //교체 장비 선택
                                {
                                    bool isAlreadyEquipped = false;
                                    for (int i = 0; i < (int)GST.MAX; i++)
                                    {
                                        if (gameData.inventoryGear[selectItem].itemGear == gameData.equippedGear[i])
                                        {
                                            isAlreadyEquipped = true;
                                        }
                                    }

                                    if (isAlreadyEquipped == false) //착용하지 않은 장비만 교체 가능
                                    {
                                        switch (gameData.inventoryGear[selectItem].itemGear.type)
                                        {
                                            //gearSelect = GST type + 11
                                            case ITG.SWORD: gearSelect = 11; break;
                                            case ITG.SHIELD: gearSelect = 12; break;
                                            case ITG.DAGGER: gearSelect = 11; break;
                                            case ITG.BOW: gearSelect = 11; break;
                                            case ITG.ARROW: gearSelect = 12; break;
                                            case ITG.HELMET: gearSelect = 13; break;
                                            case ITG.ARMOR: gearSelect = 14; break;
                                            case ITG.GLOVES: gearSelect = 15; break;
                                            case ITG.SHOES: gearSelect = 16; break;
                                            case ITG.RING: gearSelect = 17; break;
                                        }
                                    }
                                }
                                else if (1 <= gearSelect && gearSelect <= 7) //장착 해제
                                {
                                    gameData.UnequipGear((GST)(gearSelect - 1));
                                    gameData.ApplyPS();
                                }
                                else if (11 <= gearSelect && gearSelect <= 17) //장비 교체
                                {
                                    gameData.EquipGear(gameData.inventoryGear[selectItem].itemGear.name, (GST)(gearSelect - 11));
                                    gameData.ApplyPS();
                                    gearSelect = 0;
                                }
                                break;
                            case ConsoleKey.X:
                                if (0 <= gearSelect && gearSelect <= 7)
                                {
                                    return selectTab;
                                }
                                else
                                {
                                    gearSelect = 0;
                                }
                                break;
                            case ConsoleKey.Tab:
                                selectTab = 2;
                                boolVal = false;
                                break;
                        }

                        InventoryMenuUpdate(selectTab, topItem, selectY, gearSelect, gameData, screenController, keyController);
                    }
                }
                else //스킬
                {
                    listCount = gameData.skills.Count;
                    topItem = 0;
                    selectY = 0;

                    //여기 스킬 구현
                }

                InventoryMenuUpdate(selectTab, topItem, selectY, gearSelect, gameData, screenController, keyController);
            }

            return selectTab;
        }
        private static void InventoryMenuUpdate(int selectTab, int topItem, int selectY, int gearSelect, GameData gameData, ScreenController screenController, KeyController keyController)
        {
            int listCount;

            List<WorldItemPotion> potionList = gameData.inventoryPotion;
            List<WorldItemGear> gearList = gameData.inventoryGear;
            List<Skill> skillList = gameData.skills;

            for (int i = 0; i < 10; i++) screenController.Write(5 + i * 2, 0, 49, "");
            for (int i = 0; i < 25; i++) screenController.Write(i, 51, 49, "");
            if (selectTab == 0) //인벤토리
            {
                screenController.Write(1, 0, 40, "   <인벤토리>      장비       스킬      ");

                string[] strNameList = new string[potionList.Count + gearList.Count];
                if (strNameList.Length == 0) //아이템이 없는 경우
                {
                    screenController.Write(5, 5, 24, "보유한 아이템이 없습니다");
                    screenController.Update();
                    return;
                }
                for (int i = 0; i < potionList.Count; i++)
                {
                    if ((potionList[i].itemPotion.type == ITP.HP || potionList[i].itemPotion.type == ITP.PP) && topItem + selectY == i)
                        strNameList[i] = $"{potionList[i].itemPotion.name} X {potionList[i].num}  (Z : 사용)";
                    else
                        strNameList[i] = $"{potionList[i].itemPotion.name} X {potionList[i].num}";
                }
                for (int i = 0; i < gearList.Count; i++)
                {
                    if (gearList[i].equipped == true)
                        strNameList[potionList.Count + i] = $"[E] {gearList[i].itemGear.name}";
                    else
                        strNameList[potionList.Count + i] = gearList[i].itemGear.name;
                }


                //아이템 리스트 그리기
                for (int i = topItem; i < strNameList.Length && i < topItem + 10; i++)
                    screenController.Write(5 + i * 2, 5, 24, strNameList[i]);
                screenController.Write(5 + selectY * 2, 1, 3, "-->");

                //아이템 세부 보기
                int selectItem = topItem + selectY;
                if (selectItem < potionList.Count)
                {
                    screenController.Write(2, 55, 41, potionList[selectItem].itemPotion.information);
                }
                else
                {
                    selectItem -= potionList.Count;
                    screenController.Write(2, 55, 41, gearList[selectItem].itemGear.information);
                    int itemEffect = 0;
                    if (gearList[selectItem].itemGear.p_hp != 0) { screenController.Write(4 + itemEffect, 55, 41, $"HP : {gearList[selectItem].itemGear.p_hp}"); itemEffect++; }
                    if (gearList[selectItem].itemGear.p_sp != 0) { screenController.Write(4 + itemEffect, 55, 41, $"SP : {gearList[selectItem].itemGear.p_sp}"); itemEffect++; }
                    if (gearList[selectItem].itemGear.p_attack != 0) { screenController.Write(4 + itemEffect, 55, 41, $"공격력 : {gearList[selectItem].itemGear.p_attack}"); itemEffect++; }
                    if (gearList[selectItem].itemGear.p_defense != 0) { screenController.Write(4 + itemEffect, 55, 41, $"방어력 : {gearList[selectItem].itemGear.p_defense}"); itemEffect++; }
                    if (gearList[selectItem].itemGear.p_speed != 0) { screenController.Write(4 + itemEffect, 55, 41, $"스피드 : {gearList[selectItem].itemGear.p_speed}"); itemEffect++; }
                    if (gearList[selectItem].itemGear.p_Macc != 0) { screenController.Write(4 + itemEffect, 55, 41, $"근접명중 : {gearList[selectItem].itemGear.p_Macc}"); itemEffect++; }
                    if (gearList[selectItem].itemGear.p_Racc != 0) { screenController.Write(4 + itemEffect, 55, 41, $"원거리명중 : {gearList[selectItem].itemGear.p_Racc}"); itemEffect++; }
                    if (gearList[selectItem].itemGear.p_RaccN != 0) { screenController.Write(4 + itemEffect, 55, 41, $"원거리명중(N) : {gearList[selectItem].itemGear.p_RaccN}"); itemEffect++; }
                    if (gearList[selectItem].itemGear.p_evasion != 0) { screenController.Write(4 + itemEffect, 55, 41, $"회피 : {gearList[selectItem].itemGear.p_evasion}"); itemEffect++; }
                    if (gearList[selectItem].itemGear.p_critical != 0) { screenController.Write(4 + itemEffect, 55, 41, $"크리티컬 : {gearList[selectItem].itemGear.p_critical:N1} %"); itemEffect++; }
                    if (gearList[selectItem].itemGear.p_block != 0) { screenController.Write(4 + itemEffect, 55, 41, $"방어확률 : {gearList[selectItem].itemGear.p_block:N1} %"); }
                }
            }
            else if (selectTab == 1) //장비
            {
                screenController.Write(1, 0, 40, "    인벤토리      <장비>      스킬      ");

                string[] strNameList = new string[gearList.Count];
                if (strNameList.Length == 0) //아이템이 없는 경우
                {
                    screenController.Write(5, 5, 24, "보유한 장비가 없습니다");
                    screenController.Update();
                    return;
                }
                for (int i = 0; i < gearList.Count; i++)
                {
                    if (gearList[i].equipped == true)
                        strNameList[potionList.Count + i] = $"[E] {gearList[i].itemGear.name}";
                    else
                        strNameList[potionList.Count + i] = gearList[i].itemGear.name;
                }

                //보유 리스트 그리기
                for (int i = topItem; i < gearList.Count && i < topItem + 10; i++)
                    screenController.Write(5 + i * 2, 5, 24, strNameList[i]);
                if (gearSelect == 0)
                {
                    screenController.Write(5 + selectY * 2, 1, 3, "-->");
                }
                else if (11 <= gearSelect && gearSelect <= 17)
                {
                    screenController.Write(5 + selectY * 2, 3, 1, "*");
                }

                //장착 리스트 그리기
                screenController.Write(1, 55, 40, $"무기 : {((gameData.equippedGear[(int)GST.WEAPON] != null) ? gameData.equippedGear[(int)GST.WEAPON].name : "")}");
                screenController.Write(3, 55, 40, $"보조 : {((gameData.equippedGear[(int)GST.SUBWEAPON] != null) ? gameData.equippedGear[(int)GST.SUBWEAPON].name : "")}");
                screenController.Write(5, 55, 40, $"투구 : {((gameData.equippedGear[(int)GST.HELMET] != null) ? gameData.equippedGear[(int)GST.HELMET].name : "")}");
                screenController.Write(7, 55, 40, $"갑옷 : {((gameData.equippedGear[(int)GST.ARMOR] != null) ? gameData.equippedGear[(int)GST.ARMOR].name : "")}");
                screenController.Write(9, 55, 40, $"장갑 : {((gameData.equippedGear[(int)GST.GLOVES] != null) ? gameData.equippedGear[(int)GST.GLOVES].name : "")}");
                screenController.Write(11, 55, 40, $"신발 : {((gameData.equippedGear[(int)GST.SHOSE] != null) ? gameData.equippedGear[(int)GST.SHOSE].name : "")}");
                screenController.Write(13, 55, 40, $"반지 : {((gameData.equippedGear[(int)GST.RING] != null) ? gameData.equippedGear[(int)GST.RING].name : "")}");
                screenController.Write(17, 51, 49, "=================================================");
                screenController.Write(19, 55, 40, $"HP    : {gameData.PS_HPMax,3}         근접명중  : {gameData.PS_MeleeAcc,3}");
                screenController.Write(20, 55, 40, $"SP    : {gameData.PS_SP,3}         원거리명중: {gameData.PS_RangeAcc,3}");
                screenController.Write(21, 55, 40, $"공격력: {gameData.PS_Attack,3}         회피      : {gameData.PS_Evasion,3}");
                screenController.Write(22, 55, 40, $"방어력: {gameData.PS_Defense,3}         크리티컬  : {gameData.PS_Critical:N1} %");
                screenController.Write(23, 55, 40, $"스피드: {gameData.PS_Speed,3}         방어확률  : {gameData.PS_Block:N1} %");
                if (1 <= gearSelect && gearSelect <= 7)
                {
                    screenController.Write(gearSelect * 2 - 1, 52, 3, "-->");
                }
                else if (11 <= gearSelect && gearSelect <= 17)
                {
                    screenController.Write(gearSelect * 2 - 21, 52, 3, "-->");
                }
            }
            else //스킬
            {
                screenController.Write(1, 0, 40, "    인벤토리       장비      <스킬>     ");
                //여기 스킬창 구현
            }

            screenController.Update();
        }

        public static void Gearshop(GameData gameData, ScreenController screenController, KeyController keyController)
        {
            bool isBuy = true;
            bool confirm = false;
            int listCount = 0;
            int topItem = 0;
            int selectY = 0;
            int selectItem = 0;

            ConsoleKey[] keyFilter = new ConsoleKey[] { ConsoleKey.NoName };
            keyController.GetUserInput(keyFilter, out int cheat);
            ShopUpdate(0, isBuy, confirm, topItem, selectY, 0, gameData, screenController);

            ConsoleKey keyInput;
            keyFilter = new ConsoleKey[] { ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.Z, ConsoleKey.X, ConsoleKey.Tab };
            while (true)
            {
                listCount = (isBuy == true) ? gameData.gearShop.list.Count : gameData.inventoryGear.Count;
                selectItem = topItem + selectY;

                keyInput = keyController.GetUserInput(keyFilter, out int cheatActivate);
                switch (keyInput)
                {
                    case ConsoleKey.UpArrow:
                        if (confirm == false)
                        {
                            if (listCount == 0) break;
                            if (selectItem == 0)
                            {
                                if (listCount >= 8)
                                {
                                    topItem = listCount - 8;
                                    selectY = 7;
                                }
                                else
                                {
                                    topItem = 0;
                                    selectY = listCount - 1;
                                }
                            }
                            else
                            {
                                if (selectY == 0)
                                {
                                    topItem--;
                                }
                                else
                                {
                                    selectY--;
                                }
                            }
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (confirm == false)
                        {
                            if (listCount == 0) break;
                            if (selectItem == listCount - 1)
                            {
                                topItem = 0;
                                selectY = 0;
                            }
                            else
                            {
                                if (selectY == 7)
                                {
                                    topItem++;
                                }
                                else
                                {
                                    selectY++;
                                }
                            }
                        }
                        break;
                    case ConsoleKey.Z:
                        if (confirm == false)
                        {
                            if (listCount != 0) confirm = true;
                        }
                        else //거래 확정
                        {
                            if (isBuy == true) //구매
                            {
                                ItemGear selectGear = gameData.gearShop.list[selectItem];
                                if (selectGear.value > gameData.PP_Gold)
                                {
                                    screenController.Write(23, 5, 90, "장비상인 : 돈이 부족한데?");
                                    screenController.Update();
                                    Thread.Sleep(1000);
                                    keyFilter = new ConsoleKey[] { ConsoleKey.NoName };
                                    keyController.GetUserInput(keyFilter, out int cheatC);
                                    keyFilter = new ConsoleKey[] { ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.Z, ConsoleKey.X, ConsoleKey.Tab };
                                    confirm = false;
                                }
                                else
                                {
                                    gameData.PP_Gold -= selectGear.value;
                                    gameData.inventoryGear.Add(new WorldItemGear(selectGear, false));
                                    gameData.gearShop.list.Remove(selectGear); //구매한 장비 판매목록에서 제외

                                    screenController.Write(23, 5, 90, "장비상인 : 당신 보는 눈이 있구만!");
                                    screenController.Update();
                                    Thread.Sleep(1000);
                                    keyFilter = new ConsoleKey[] { ConsoleKey.NoName };
                                    keyController.GetUserInput(keyFilter, out int cheatC);
                                    keyFilter = new ConsoleKey[] { ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.Z, ConsoleKey.X, ConsoleKey.Tab };

                                    if (listCount == 1) break;
                                    if (selectItem == listCount - 1)
                                    {
                                        if (selectY == 7)
                                        {
                                            if (topItem == 0)
                                            {
                                                selectY--;
                                            }
                                            else
                                            {
                                                topItem--;
                                            }
                                        }
                                        else
                                        {
                                            selectY--;
                                        }
                                    }
                                    else //내비둠
                                    {
                                    }

                                    confirm = false;
                                }
                            }
                            else //판매
                            {
                                ItemGear selectGear = gameData.inventoryGear[selectItem].itemGear;
                                gameData.PP_Gold += selectGear.value * 7 / 10; //감가된 가격으로 판매
                                if (gameData.inventoryGear[selectItem].equipped == true) //착용 중인 장비를 판매할 경우 장착 해제
                                {
                                    for (int i = 0; i < (int)GST.MAX; i++)
                                        if (gameData.equippedGear[i] == selectGear) gameData.UnequipGear((GST)i);
                                }
                                gameData.inventoryGear.Remove(gameData.inventoryGear[selectItem]);
                                gameData.gearShop.list.Add(selectGear); //판매한 장비 구매목록에 추가

                                screenController.Write(23, 5, 90, "장비상인 : 어디보자 하나 둘 셋...  돈 잘 받았지?");
                                screenController.Update();
                                Thread.Sleep(1000);
                                keyFilter = new ConsoleKey[] { ConsoleKey.NoName };
                                keyController.GetUserInput(keyFilter, out int cheatC);
                                keyFilter = new ConsoleKey[] { ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.Z, ConsoleKey.X, ConsoleKey.Tab };

                                if (listCount == 1) break;
                                if (selectItem == listCount - 1)
                                {
                                    if (selectY == 7)
                                    {
                                        if (topItem == 0)
                                        {
                                            selectY--;
                                        }
                                        else
                                        {
                                            topItem--;
                                        }
                                    }
                                    else
                                    {
                                        selectY--;
                                    }
                                }
                                else //내비둠
                                {
                                }

                                confirm = false;
                            }
                        }
                        break;
                    case ConsoleKey.X:
                        if (confirm == true) confirm = false;
                        else
                        {
                            return;
                        }
                        break;
                    case ConsoleKey.Tab:
                        isBuy = !isBuy;
                        confirm = false;
                        topItem = 0;
                        selectY = 0;
                        break;
                }

                ShopUpdate(0, isBuy, confirm, topItem, selectY, 0, gameData, screenController);
            }
        }
        public static void Potionshop(GameData gameData, ScreenController screenController, KeyController keyController)
        {
            bool isBuy = true;
            bool confirm = false;
            int listCount = 0;
            int topItem = 0;
            int selectY = 0;
            int selectItem = 0;
            int quantity = 1;

            ConsoleKey[] keyFilter = new ConsoleKey[] { ConsoleKey.NoName };
            keyController.GetUserInput(keyFilter, out int cheat);
            ShopUpdate(1, isBuy, confirm, topItem, selectY, quantity, gameData, screenController);

            ConsoleKey keyInput;
            keyFilter = new ConsoleKey[] { ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.LeftArrow, ConsoleKey.RightArrow, ConsoleKey.Z, ConsoleKey.X, ConsoleKey.Tab };
            while (true)
            {
                listCount = (isBuy == true) ? gameData.gearShop.list.Count : gameData.inventoryPotion.Count;
                selectItem = topItem + selectY;

                keyInput = keyController.GetUserInput(keyFilter, out int cheatActivate);
                switch (keyInput)
                {
                    case ConsoleKey.UpArrow:
                        if (confirm == false)
                        {
                            if (listCount == 0) break;
                            if (selectItem == 0)
                            {
                                if (listCount >= 8)
                                {
                                    topItem = listCount - 8;
                                    selectY = 7;
                                }
                                else
                                {
                                    topItem = 0;
                                    selectY = listCount - 1;
                                }
                            }
                            else
                            {
                                if (selectY == 0)
                                {
                                    topItem--;
                                }
                                else
                                {
                                    selectY--;
                                }
                            }
                        }
                        else
                        {
                            quantity += 10;
                            if (isBuy == true)
                            {
                                if (quantity > 99) quantity = 99;
                            }
                            else
                            {
                                if (quantity > gameData.inventoryPotion[selectItem].num) quantity = gameData.inventoryPotion[selectItem].num;
                            }
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (confirm == false)
                        {
                            if (listCount == 0) break;
                            if (selectItem == listCount - 1)
                            {
                                topItem = 0;
                                selectY = 0;
                            }
                            else
                            {
                                if (selectY == 7)
                                {
                                    topItem++;
                                }
                                else
                                {
                                    selectY++;
                                }
                            }
                        }
                        else
                        {
                            quantity -= 10;
                            if (quantity < 1) quantity = 1;
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        if (confirm == true)
                        {
                            quantity--;
                            if (quantity < 1) quantity = 1;
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        if (confirm == true)
                        {
                            quantity++;
                            if (isBuy == true)
                            {
                                if (quantity > 99) quantity = 99;
                            }
                            else
                            {
                                if (quantity > gameData.inventoryPotion[selectItem].num) quantity = gameData.inventoryPotion[selectItem].num;
                            }
                        }
                        break;
                    case ConsoleKey.Z:
                        if (confirm == false)
                        {
                            if (listCount != 0) confirm = true;
                        }
                        else //거래 확정
                        {
                            if (isBuy == true) //구매
                            {
                                ItemPotion selectPotion = gameData.potionShop.list[selectItem];
                                if (selectPotion.value * quantity > gameData.PP_Gold)
                                {
                                    screenController.Write(23, 5, 90, "포션상인 : 어... 돈이 부족하신 것 같은데요?");
                                    screenController.Update();
                                    Thread.Sleep(1000);
                                    keyFilter = new ConsoleKey[] { ConsoleKey.NoName };
                                    keyController.GetUserInput(keyFilter, out int cheatC);
                                    keyFilter = new ConsoleKey[] { ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.LeftArrow, ConsoleKey.RightArrow, ConsoleKey.Z, ConsoleKey.X, ConsoleKey.Tab };

                                    quantity = 1;
                                    confirm = false;
                                }
                                else
                                {
                                    gameData.PP_Gold -= selectPotion.value * quantity;
                                    bool isThere = false; //아이템을 이미 소지하고 있는지
                                    for (int i = 0; i < gameData.inventoryPotion.Count; i++)
                                    {
                                        if (gameData.inventoryPotion[i].itemPotion.name == selectPotion.name)
                                        {
                                            isThere = true;
                                            gameData.inventoryPotion[i].num += quantity;
                                            break;
                                        }
                                    }
                                    if (isThere == false)
                                    {
                                        gameData.inventoryPotion.Add(new WorldItemPotion(selectPotion, quantity));
                                    }

                                    screenController.Write(23, 5, 90, "포션상인 : 구매해 주셔서 감사합니다!");
                                    screenController.Update();
                                    Thread.Sleep(1000);
                                    keyFilter = new ConsoleKey[] { ConsoleKey.NoName };
                                    keyController.GetUserInput(keyFilter, out int cheatC);
                                    keyFilter = new ConsoleKey[] { ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.LeftArrow, ConsoleKey.RightArrow, ConsoleKey.Z, ConsoleKey.X, ConsoleKey.Tab };

                                    if (listCount == 1) break;
                                    if (selectItem == listCount - 1)
                                    {
                                        if (selectY == 7)
                                        {
                                            if (topItem == 0)
                                            {
                                                selectY--;
                                            }
                                            else
                                            {
                                                topItem--;
                                            }
                                        }
                                        else
                                        {
                                            selectY--;
                                        }
                                    }
                                    else //내비둠
                                    {
                                    }

                                    quantity = 1;
                                    confirm = false;
                                }
                            }
                            else //판매
                            {
                                ItemPotion selectPotion = gameData.inventoryPotion[selectItem].itemPotion;
                                gameData.PP_Gold += (selectPotion.value * 7 / 10) * quantity; //감가된 가격으로 판매
                                gameData.inventoryPotion[selectItem].num -= quantity;
                                if (gameData.inventoryPotion[selectItem].num <= 0) //남은 개수 0 이하면 목록에서 삭제
                                {
                                    gameData.inventoryPotion.RemoveAt(selectItem);
                                    listCount--; //판매 항목 수 여기서 수정함
                                }

                                screenController.Write(23, 5, 90, "포션상인 : 판매하신 대금 여기있습니다!");
                                screenController.Update();
                                Thread.Sleep(1000);
                                keyFilter = new ConsoleKey[] { ConsoleKey.NoName };
                                keyController.GetUserInput(keyFilter, out int cheatC);
                                keyFilter = new ConsoleKey[] { ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.LeftArrow, ConsoleKey.RightArrow, ConsoleKey.Z, ConsoleKey.X, ConsoleKey.Tab };

                                if (listCount == 0) break;
                                if (selectItem == listCount - 1)
                                {
                                    if (selectY == 7)
                                    {
                                        if (topItem == 0)
                                        {
                                            selectY--;
                                        }
                                        else
                                        {
                                            topItem--;
                                        }
                                    }
                                    else
                                    {
                                        selectY--;
                                    }
                                }
                                else //내비둠
                                {
                                }

                                quantity = 1;
                                confirm = false;
                            }
                        }
                        break;
                    case ConsoleKey.X:
                        if (confirm == true)
                        {
                            quantity = 1;
                            confirm = false;
                        }
                        else
                        {
                            return;
                        }
                        break;
                    case ConsoleKey.Tab:
                        isBuy = !isBuy;
                        quantity = 1;
                        confirm = false;
                        topItem = 0;
                        selectY = 0;
                        break;
                }

                ShopUpdate(1, isBuy, confirm, topItem, selectY, quantity, gameData, screenController);
            }
        }
        private static void ShopUpdate(int tab, bool isBuy, bool confirm, int topItem, int selectY, int quantity, GameData gameData, ScreenController screenController)
        {
            //tab : 0-장비 상점   1-소모품 상점   2-용병 거래소
            //quantity : 소모품 상점에서만 사용

            for (int i = 0; i < 8; i++) screenController.Write(5 + i * 2, 0, 49, "");
            for (int i = 0; i < 17; i++) screenController.Write(4 + i, 51, 49, "");
            screenController.Write(23, 5, 90, "");

            screenController.Write(1, 87, 6, $"{gameData.PP_Gold,6}"); //소지금 표시

            if (tab == 0) //장비 상점
            {
                if (isBuy == true) //구매
                {
                    screenController.Write(1, 0, 31, "        <구매>      판매       ");

                    string[] strNameList = new string[gameData.gearShop.list.Count];
                    if (strNameList.Length == 0) //아이템이 없는 경우
                    {
                        screenController.Write(23, 5, 90, "장비상인 : 품절이야 품절 ~");
                        screenController.Update();
                        return;
                    }

                    screenController.Write(23, 5, 90, "장비상인 : 우리는 싼것이든 비싼 것이든 다 최상품만 취급하지!");
                    string itemType = "";
                    for (int i = 0; i < strNameList.Length; i++) //나중에 시간 되면 업데이트 함수에서 빼자
                    {
                        switch (gameData.gearShop.list[i].type)
                        {
                            case ITG.SWORD: itemType = "검 : "; break;
                            case ITG.SHIELD: itemType = "방패 : "; break;
                            case ITG.DAGGER: itemType = "단검 : "; break;
                            case ITG.BOW: itemType = "활 : "; break;
                            case ITG.ARROW: itemType = "화살 : "; break;
                            case ITG.HELMET: itemType = "투구 : "; break;
                            case ITG.ARMOR: itemType = "갑옷 : "; break;
                            case ITG.GLOVES: itemType = "장갑 : "; break;
                            case ITG.SHOES: itemType = "신발 : "; break;
                            case ITG.RING: itemType = "반지 : "; break;
                        }

                        strNameList[i] = itemType + gameData.gearShop.list[i].name;
                    }

                    //보유 리스트 그리기
                    for (int i = 0; topItem + i < strNameList.Length && i < 8; i++)
                    {
                        screenController.Write(5 + i * 2, 5, 25, strNameList[topItem + i]);
                        screenController.Write(5 + i * 2, 37, 7, $"{gameData.gearShop.list[topItem + i].value,5} G"); //구매 가격 표시
                    }
                    if (confirm == false) screenController.Write(5 + selectY * 2, 1, 3, "-->");

                    //아이템 세부 표시
                    int selectItem = topItem + selectY;
                    ItemGear selectedGear = gameData.gearShop.list[selectItem];
                    screenController.Write(7, 56, 49, selectedGear.information);
                    int itemEffect = 0;
                    if (selectedGear.p_hp != 0) { screenController.Write(9 + itemEffect, 56, 39, $"HP : {selectedGear.p_hp}"); itemEffect++; }
                    if (selectedGear.p_sp != 0) { screenController.Write(9 + itemEffect, 56, 39, $"SP : {selectedGear.p_sp}"); itemEffect++; }
                    if (selectedGear.p_attack != 0) { screenController.Write(9 + itemEffect, 56, 39, $"공격력 : {selectedGear.p_attack}"); itemEffect++; }
                    if (selectedGear.p_defense != 0) { screenController.Write(9 + itemEffect, 56, 39, $"방어력 : {selectedGear.p_defense}"); itemEffect++; }
                    if (selectedGear.p_speed != 0) { screenController.Write(9 + itemEffect, 56, 39, $"스피드 : {selectedGear.p_speed}"); itemEffect++; }
                    if (selectedGear.p_Macc != 0) { screenController.Write(9 + itemEffect, 56, 39, $"근접명중 : {selectedGear.p_Macc}"); itemEffect++; }
                    if (selectedGear.p_Racc != 0) { screenController.Write(9 + itemEffect, 56, 39, $"원거리명중 : {selectedGear.p_Racc}"); itemEffect++; }
                    if (selectedGear.p_RaccN != 0) { screenController.Write(9 + itemEffect, 56, 39, $"원거리명중(N) : {selectedGear.p_RaccN}"); itemEffect++; }
                    if (selectedGear.p_evasion != 0) { screenController.Write(9 + itemEffect, 56, 39, $"회피 : {selectedGear.p_evasion}"); itemEffect++; }
                    if (selectedGear.p_critical != 0) { screenController.Write(9 + itemEffect, 56, 39, $"크리티컬 : {selectedGear.p_critical:N1} %"); itemEffect++; }
                    if (selectedGear.p_block != 0) { screenController.Write(9 + itemEffect, 56, 39, $"방어확률 : {selectedGear.p_block:N1} %"); }

                    //구매 확인
                    if (confirm == true)
                    {
                        screenController.Write(5, 52, 14, "--> << 산다 >>");
                    }
                }
                else //판매
                {
                    screenController.Write(1, 0, 31, "         구매      <판매>      ");
                    screenController.Write(23, 5, 90, "장비상인 : 장비를 팔고 싶으면 70% 가격으로 매입해주지.");

                    string[] strNameList = new string[gameData.inventoryGear.Count];
                    if (strNameList.Length == 0) //아이템이 없는 경우
                    {
                        screenController.Write(23, 5, 90, "판매할 장비가 없다.");
                        screenController.Update();
                        return;
                    }

                    for (int i = 0; i < strNameList.Length; i++)
                    {
                        if (gameData.inventoryGear[i].equipped == true)
                            strNameList[i] = $"[E] {gameData.inventoryGear[i].itemGear.name}";
                        else
                            strNameList[i] = gameData.inventoryGear[i].itemGear.name;
                    }

                    //보유 리스트 그리기
                    for (int i = topItem; i < strNameList.Length && i < topItem + 8; i++)
                    {
                        screenController.Write(5 + i * 2, 5, 39, strNameList[i]);
                        screenController.Write(5 + i * 2, 37, 7, $"{gameData.inventoryGear[topItem + i].itemGear.value * 7 / 10,5} G"); //판매 가격 표시
                    }
                    if (confirm == false) screenController.Write(5 + selectY * 2, 1, 3, "-->");

                    //아이템 세부 표시
                    int selectItem = topItem + selectY;
                    ItemGear selectedGear = gameData.inventoryGear[selectItem].itemGear;
                    screenController.Write(7, 56, 49, selectedGear.information);
                    int itemEffect = 0;
                    if (selectedGear.p_hp != 0) { screenController.Write(9 + itemEffect, 56, 39, $"HP : {selectedGear.p_hp}"); itemEffect++; }
                    if (selectedGear.p_sp != 0) { screenController.Write(9 + itemEffect, 56, 39, $"SP : {selectedGear.p_sp}"); itemEffect++; }
                    if (selectedGear.p_attack != 0) { screenController.Write(9 + itemEffect, 56, 39, $"공격력 : {selectedGear.p_attack}"); itemEffect++; }
                    if (selectedGear.p_defense != 0) { screenController.Write(9 + itemEffect, 56, 39, $"방어력 : {selectedGear.p_defense}"); itemEffect++; }
                    if (selectedGear.p_speed != 0) { screenController.Write(9 + itemEffect, 56, 39, $"스피드 : {selectedGear.p_speed}"); itemEffect++; }
                    if (selectedGear.p_Macc != 0) { screenController.Write(9 + itemEffect, 56, 39, $"근접명중 : {selectedGear.p_Macc}"); itemEffect++; }
                    if (selectedGear.p_Racc != 0) { screenController.Write(9 + itemEffect, 56, 39, $"원거리명중 : {selectedGear.p_Racc}"); itemEffect++; }
                    if (selectedGear.p_RaccN != 0) { screenController.Write(9 + itemEffect, 56, 39, $"원거리명중(N) : {selectedGear.p_RaccN}"); itemEffect++; }
                    if (selectedGear.p_evasion != 0) { screenController.Write(9 + itemEffect, 56, 39, $"회피 : {selectedGear.p_evasion}"); itemEffect++; }
                    if (selectedGear.p_critical != 0) { screenController.Write(9 + itemEffect, 56, 39, $"크리티컬 : {selectedGear.p_critical:N1} %"); itemEffect++; }
                    if (selectedGear.p_block != 0) { screenController.Write(9 + itemEffect, 56, 39, $"방어확률 : {selectedGear.p_block:N1} %"); }

                    //판매 확인
                    if (confirm == true)
                    {
                        screenController.Write(5, 52, 14, "--> << 판다 >>");
                    }
                }
            }
            else if (tab == 1) //포션 상점
            {
                if (isBuy == true) //구매
                {
                    screenController.Write(1, 0, 31, "        <구매>      판매       ");
                    screenController.Write(23, 5, 90, "포션상인 : 천천히 둘러 보세요~");

                    string[] strNameList = new string[gameData.potionShop.list.Count];
                    string itemType = "";
                    for (int i = 0; i < strNameList.Length; i++) //나중에 시간 되면 업데이트 함수에서 빼자
                    {
                        switch (gameData.potionShop.list[i].type)
                        {
                            case ITP.HP: itemType = "HP : "; break;
                            case ITP.SP: itemType = "SP : "; break;
                            default: itemType = "기타 : "; break;
                        }

                        strNameList[i] = itemType + gameData.potionShop.list[i].name;
                    }

                    //보유 리스트 그리기
                    for (int i = 0; topItem + i < strNameList.Length && i < 8; i++)
                    {
                        screenController.Write(5 + i * 2, 5, 25, strNameList[topItem + i]);
                        screenController.Write(5 + i * 2, 37, 7, $"{gameData.potionShop.list[topItem + i].value,5} G"); //구매 가격 표시
                    }
                    if (confirm == false) screenController.Write(5 + selectY * 2, 1, 3, "-->");

                    //아이템 세부 표시
                    int selectItem = topItem + selectY;
                    ItemPotion selectedPotion = gameData.potionShop.list[selectItem];
                    screenController.Write(7, 56, 49, selectedPotion.information);

                    //구매 확인
                    if (confirm == true)
                    {
                        screenController.Write(5, 52, 45, $"--> 갯수 : {quantity,2}   총합 : {selectedPotion.value * quantity} G   >> 산다 <<");
                    }
                }
                else //판매
                {
                    screenController.Write(1, 0, 31, "         구매      <판매>      ");
                    screenController.Write(23, 5, 90, "포션상인 : 저희는 취급하는 품목에 대해 70% 가격으로 사고 있어요.");

                    string[] strNameList = new string[gameData.inventoryPotion.Count];
                    if (strNameList.Length == 0) //아이템이 없는 경우
                    {
                        screenController.Write(23, 5, 90, "판매할 아이템이 없다.");
                        screenController.Update();
                        return;
                    }

                    for (int i = 0; i < strNameList.Length; i++)
                    {
                        strNameList[i] = $"{gameData.inventoryPotion[i].itemPotion.name}  X {gameData.inventoryPotion[i].num}";
                    }

                    //보유 리스트 그리기
                    for (int i = topItem; i < strNameList.Length && i < topItem + 8; i++)
                    {
                        screenController.Write(5 + i * 2, 5, 39, strNameList[i]);
                        screenController.Write(5 + i * 2, 37, 7, $"{gameData.inventoryPotion[topItem + i].itemPotion.value * 7 / 10,5} G"); //판매 가격 표시
                    }
                    if (confirm == false) screenController.Write(5 + selectY * 2, 1, 3, "-->");

                    //아이템 세부 표시
                    int selectItem = topItem + selectY;
                    ItemPotion selectedPotion = gameData.inventoryPotion[selectItem].itemPotion;
                    screenController.Write(7, 56, 49, selectedPotion.information);

                    //판매 확인
                    if (confirm == true)
                    {
                        screenController.Write(5, 52, 45, $"--> 갯수 : {quantity,2}   총합 : {(selectedPotion.value * 7 / 10) * quantity} G   << 판다 >>");
                    }
                }
            }

            screenController.Update();
        }
    }
}
