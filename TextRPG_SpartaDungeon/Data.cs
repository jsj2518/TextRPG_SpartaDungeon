using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.Json;

namespace TextRPG_SpartaDungeon
{
    enum PC //플레이어 직업
    {
        Warrior,
        Rogue,
        Archer
    }

    class GameData
    {
        //플레이어 영역
        //PP : 이름, 직업, 레벨, 경험치, 골드
        public string PP_Name { get; set; }
        public PC PP_Class { get; set; }
        public string PP_ClassKor { get; set; }
        public int PP_Level { get; set; }
        public int PP_Exp { get; set; }
        public int PP_ExpNextLevel { get; set; }
        public int PP_Gold { get; set; }

        //PB : 기본능력치(레벨에 따른) 건강,체력,힘,민첩,손재주
        public int PB_Health { get; set; }
        public int PB_Stamina { get; set; }
        public int PB_Strength { get; set; }
        public int PB_Agility { get; set; }
        public int PB_Dexterity { get; set; }

        public int PA_Point { get; set; } //쓰지 않은 포인트
        //PA : 기본능력치(포인트 투자된)
        public int PA_Health { get; set; }
        public int PA_Stamina { get; set; }
        public int PA_Strength { get; set; }
        public int PA_Agility { get; set; }
        public int PA_Dexterity { get; set; }

        //PSB : 확장 능력치  HP(현재/최대),SP,공격력,방어력,스피드,근접명중,원거리명중,회피,크리확률,방어확률
        public int PS_HPMax { get; set; }
        public int PS_HPCur { get; set; }
        public int PS_SP { get; set; }
        public int PS_SPCur { get; set; } //전투 중에만 사용
        public int PS_Attack { get; set; }
        public int PS_Defense { get; set; }
        public int PS_Speed { get; set; }
        public int PS_MeleeAcc { get; set; }
        public int PS_RangeAcc { get; set; }
        public int PS_RangeAccN { get; set; } //거리 당 명중 감소량
        public int PS_Evasion { get; set; }
        public double PS_Critical { get; set; }
        public double PS_Block { get; set; }
        public int PS_Burden { get; set; } //착용한 장비 무게

        //인벤토리
        public List<WorldItemPotion> inventoryPotion = new List<WorldItemPotion>();
        public Dictionary<string, int> dic_inventoryPotion { get; set; } //저장용 데이터
        //장비
        public List<WorldItemGear> inventoryGear = new List<WorldItemGear>();
        public Dictionary<string, bool> dic_inventoryGear { get; set; } //저장용 데이터
        //착용 장비
        public ItemGear[] equippedGear = new ItemGear[(int)GST.MAX];
        public Dictionary<string, int> dic_equippedGear { get; set; } //저장용 데이터
        //스킬
        public List<Skill> skills = new List<Skill>();
        //용병
        public Mercenary mercenary1, mercenary2;



        public void SetLevel1() //초기 스텟 설정
        {
            PP_Level = 1;
            PP_Exp = 0;
            PP_ExpNextLevel = 150;

            if (PP_Class == PC.Warrior)
            {
                PB_Health = PA_Health = 15;
                PB_Stamina = PA_Stamina = 10;
                PB_Strength = PA_Strength = 14;
                PB_Agility = PA_Agility = 6;
                PB_Dexterity = PA_Dexterity = 5;
            }
            else if (PP_Class == PC.Rogue)
            {
                PB_Health = PA_Health = 10;
                PB_Stamina = PA_Stamina = 9;
                PB_Strength = PA_Strength = 9;
                PB_Agility = PA_Agility = 14;
                PB_Dexterity = PA_Dexterity = 8;
            }
            else
            {
                PB_Health = PA_Health = 5;
                PB_Stamina = PA_Stamina = 7;
                PB_Strength = PA_Strength = 12;
                PB_Agility = PA_Agility = 11;
                PB_Dexterity = PA_Dexterity = 15;
            }
        }
        public void LevelUP() //스텟 자동 2포인트, 수동 3포인트 부여
        {
            PP_Level++;
            PP_Exp = PP_Exp - PP_ExpNextLevel;
            PP_ExpNextLevel = PP_ExpNextLevel + PP_Level * 20 + 50; //150, 240, 350, 480, 630, ...
            int mod5 = PP_Level % 5;
            if (PP_Class == PC.Warrior)
            {
                if (mod5 == 0)
                {
                    PA_Health++; PB_Health++;
                    PA_Strength++; PB_Strength++;
                }
                else if (mod5 == 1)
                {
                    PA_Stamina++; PB_Stamina++;
                    PA_Agility++; PB_Agility++;
                }
                else if (mod5 == 2)
                {
                    PA_Health++; PB_Health++;
                    PA_Strength++; PB_Strength++;
                }
                else if (mod5 == 3)
                {
                    PA_Strength++; PB_Strength++;
                    PA_Dexterity++; PB_Dexterity++;
                }
                else
                {
                    PA_Health++; PB_Health++;
                    PA_Stamina++; PB_Stamina++;
                }
            }
            else if (PP_Class == PC.Rogue)
            {
                if (mod5 == 0)
                {
                    PA_Stamina++; PB_Stamina++;
                    PA_Strength++; PB_Strength++;
                }
                else if (mod5 == 1)
                {
                    PA_Health++; PB_Health++;
                    PA_Agility++; PB_Agility++;
                }
                else if (mod5 == 2)
                {
                    PA_Strength++; PB_Strength++;
                    PA_Dexterity++; PB_Dexterity++;
                }
                else if (mod5 == 3)
                {
                    PA_Health++; PB_Health++;
                    PA_Agility++; PB_Agility++;
                }
                else
                {
                    PA_Agility++; PB_Agility++;
                    PA_Dexterity++; PB_Dexterity++;
                }
            }
            else
            {
                if (mod5 == 0)
                {
                    PA_Strength++; PB_Strength++;
                    PA_Dexterity++; PB_Dexterity++;
                }
                else if (mod5 == 1)
                {
                    PA_Health++; PB_Health++;
                    PA_Agility++; PB_Agility++;
                }
                else if (mod5 == 2)
                {
                    PA_Strength++; PB_Strength++;
                    PA_Dexterity++; PB_Dexterity++;
                }
                else if (mod5 == 3)
                {
                    PA_Health++; PB_Health++;
                    PA_Dexterity++; PB_Dexterity++;
                }
                else
                {
                    PA_Stamina++; PB_Stamina++;
                    PA_Agility++; PB_Agility++;
                }
            }
            PA_Point += 3;

            ApplyPS();
        }
        public void ApplyPS() //확장 능력치 적용
        {
            int beforeHpMax = PS_HPMax; //최대 hp 변화에 따라 현재 hp도 같이 변함

            int hp = 0, sp = 0, attack = 0, defense = 0, speed = 0, macc = 0, racc = 0, racc_n = 0, evasion = 0, weight = 0;
            double critical = 0.0, block = 0.0;
            for (int i = 0; i < (int)GST.MAX; i++)
            {
                if (equippedGear[i] == null) continue;

                hp += equippedGear[i].p_hp;
                sp += equippedGear[i].p_sp;
                attack += equippedGear[i].p_attack;
                defense += equippedGear[i].p_defense;
                speed += equippedGear[i].p_speed;
                macc += equippedGear[i].p_Macc;
                racc += equippedGear[i].p_Racc;
                racc_n += equippedGear[i].p_RaccN;
                evasion += equippedGear[i].p_evasion;
                critical += equippedGear[i].p_critical;
                block += equippedGear[i].p_block;
                weight += equippedGear[i].weight;
            }

            //여기 공식에 장비 효과 적용
            PS_HPMax = 50 + PA_Health * 2 + hp;
            PS_SP = 50 + PA_Stamina * 2 + sp;
            PS_Attack = 30 + PA_Strength + attack;
            PS_Defense = defense;
            int speedReduction = (weight * weight - 5 * (PA_Strength + 10) * (PA_Strength + 10)) / 200;
            if (speedReduction < 0) speedReduction = 0;
            PS_Speed = 30 + PA_Agility + speed - speedReduction;
            PS_MeleeAcc = 30 + PA_Agility + macc;
            PS_RangeAcc = 20 + PA_Dexterity * 2 - PA_Strength * 2 + racc;
            PS_RangeAccN = -5 + racc_n;
            PS_Evasion = 25 + PA_Agility + evasion;
            PS_Critical = Math.Pow(PA_Dexterity + 25.0, 0.5) * 5.0 - 1.5 + critical;
            if (PS_Critical > 100.0) PS_Critical = 100.0;
            PS_Block = block;
            if (PS_Block > 100.0) PS_Block = 100.0;
            PS_Burden = weight;

            PS_HPCur += PS_HPMax - beforeHpMax; //최대 hp 변화에 따라 현재 hp도 같이 변함
            if (PS_HPCur <= 0) PS_HPCur = 1;
        }

        public void SetInitialEquipment() //초기장비 지급
        {
            if (PP_Class == PC.Warrior)
            {
                inventoryGear.Add(new WorldItemGear(ItemData.sword1, false));
                inventoryGear.Add(new WorldItemGear(ItemData.shield1, false));
                inventoryGear.Add(new WorldItemGear(ItemData.armorH1, false));
                EquipGear(ItemData.sword1.name, GST.WEAPON);
                EquipGear(ItemData.shield1.name, GST.SUBWEAPON);
                EquipGear(ItemData.armorH1.name, GST.ARMOR);
            }
            else if (PP_Class == PC.Rogue)
            {
                inventoryGear.Add(new WorldItemGear(ItemData.dagger1, false));
                inventoryGear.Add(new WorldItemGear(ItemData.dagger1_d, false));
                inventoryGear.Add(new WorldItemGear(ItemData.armorL1, false));
                inventoryGear.Add(new WorldItemGear(ItemData.shoesL1, false));
                EquipGear(ItemData.dagger1.name, GST.WEAPON);
                EquipGear(ItemData.dagger1_d.name, GST.SUBWEAPON);
                EquipGear(ItemData.armorL1.name, GST.ARMOR);
                EquipGear(ItemData.shoesL1.name, GST.SHOSE);
            }
            else
            {
                inventoryGear.Add(new WorldItemGear(ItemData.bow1, false));
                inventoryGear.Add(new WorldItemGear(ItemData.arrow1, false));
                inventoryGear.Add(new WorldItemGear(ItemData.armorL1, false));
                inventoryGear.Add(new WorldItemGear(ItemData.glovesL1, false));
                EquipGear(ItemData.bow1.name, GST.WEAPON);
                EquipGear(ItemData.arrow1.name, GST.SUBWEAPON);
                EquipGear(ItemData.armorL1.name, GST.ARMOR);
                EquipGear(ItemData.glovesL1.name, GST.SHOSE);
            }
        }

        public bool EquipGear(string gearName, GST slotType) //단도는 보조무기에 갈 수도 있어서 장착 타입을 따로 받음
        {
            ItemGear gear = null;
            foreach (WorldItemGear item in inventoryGear)
            {
                if (item.itemGear.name == gearName) { gear = item.itemGear; break; }
            }
            if (gear == null)
            {
                return false;
            }

            if (PP_Class == PC.Warrior)
            {
                if (gear.type == ITG.DAGGER || gear.type == ITG.BOW || gear.type == ITG.ARROW)
                    return false;
            }
            else if (PP_Class == PC.Rogue)
            {
                if (gear.type == ITG.SWORD || gear.type == ITG.SHIELD || gear.type == ITG.BOW || gear.type == ITG.ARROW)
                    return false;
            }
            else
            {
                if (gear.type == ITG.SWORD || gear.type == ITG.SHIELD || gear.type == ITG.DAGGER)
                    return false;
            }
            
            UnequipGear(slotType);
            equippedGear[(int)slotType] = gear;

            foreach (WorldItemGear item in inventoryGear)
            {
                if (item.itemGear == gear) { item.equipped = true; break; }
            }
            return true;
        }
        public void UnequipGear(GST slotType)
        {
            ItemGear temp = equippedGear[(int)slotType];
            equippedGear[(int)slotType] = null;

            foreach (WorldItemGear item in inventoryGear)
            {
                if (item.itemGear == temp) {item.equipped = false; break; }
            }
        }



        public void UseItem(ItemPotion potion)
        {
            bool valid = false;
            foreach (WorldItemPotion i in inventoryPotion) //아이템이 있으면 하나 줄이고 없으면 리턴
            {
                if (i.itemPotion.name == potion.name)
                {
                    if (--i.num <= 0) inventoryPotion.Remove(i); //아이템 소지수가 0 이하가 되면 리스트에서 제거
                    valid = true;
                }
            }
            if (valid == false) return;

            switch (potion.type)
            {
                case ITP.HP:
                    PS_HPCur += potion.power;
                    if (PS_HPCur > PS_HPMax) PS_HPCur = PS_HPMax;
                    break;
                case ITP.SP:
                    PS_SPCur += potion.power;
                    if (PS_SPCur > PS_SP) PS_SPCur = PS_SP;
                    break;
                case ITP.PP: //스텟 초기화
                    PA_Health = PB_Health;
                    PA_Stamina = PB_Stamina;
                    PA_Strength = PB_Strength;
                    PA_Agility = PB_Agility;
                    PA_Dexterity = PB_Dexterity;
                    PA_Point = (PP_Level - 1) * 3;
                    break;
            }
        }




        //게임 플래그
        public int GF_MercenaryUnlock { get; set; } //용병샵 0-비활성화 1-활성화(저렙만) 2-완전활성화 
        public int GF_MansionUnlock { get; set; } //스네이크 미니게임(알바) 진행도 0-비활성화 1-수상한저택 2-뱀애호가의저택(언락됨)


        //장비가게 정보
        public GearShop gearShop = new GearShop();
        public Dictionary<string, int> dic_gearShop { get; set; } //저장용 데이터
        //소모품 가게 정보
        public PotionShop potionShop = new PotionShop();





        public static bool ExistsData(int saveNum)
        {
            if (saveNum < 1 || saveNum > 3)
            {
                return false;
            }

            return File.Exists($"{Const.saveFileName}{saveNum}");
        }

        public void SaveData(int saveNum)
        {
            if (saveNum < 1 || saveNum > 3)
            {
                return;
            }

            SaveToFile($"{Const.saveFileName}{saveNum}");
        }

        // 데이터를 JSON 파일로 저장하는 메서드
        private void SaveToFile(string filePath)
        {
            dic_inventoryPotion = new Dictionary<string, int>();
            foreach (WorldItemPotion i in inventoryPotion) dic_inventoryPotion.Add(i.itemPotion.name, i.num);
            dic_inventoryGear = new Dictionary<string, bool>();
            foreach (WorldItemGear i in inventoryGear) dic_inventoryGear.Add(i.itemGear.name, i.equipped);
            dic_equippedGear = new Dictionary<string, int>();
            for (int i = 0; i < (int)GST.MAX; i++)
            {
                if (equippedGear[i] != null) dic_equippedGear.Add(equippedGear[i].name, i);
            }
            dic_gearShop = new Dictionary<string, int>();
            foreach (ItemGear i in gearShop.list) dic_gearShop.Add(i.name, 0);

            try
            {
                // 객체를 JSON으로 직렬화
                string jsonString = JsonSerializer.Serialize(this);
                File.WriteAllText(filePath, jsonString);
                Console.SetCursorPosition(0, Const.screenH + 10);
                Console.WriteLine("데이터가 성공적으로 저장되었습니다.");
                Console.SetCursorPosition(0, 0);
            }
            catch (Exception ex)
            {
                Console.SetCursorPosition(0, Const.screenH + 10);
                Console.WriteLine("데이터 저장 중 오류가 발생했습니다: " + ex.Message);
                Console.SetCursorPosition(0, 0);
            }
        }

        
    }









    //전역 변수
    class ConsoleData
    {
        public static int countReadLine = 0; //Console.ReadLine() 받으러 안드로메다 갔다온 횟수
    }

    //장비 가게
    class GearShop
    {
        public List<ItemGear> list = new List<ItemGear>();

        public void ResetList() //캐릭터 정보 물러온 뒤 실행할 것
        {
            list = new List<ItemGear>();
            list.AddRange(new ItemGear[]
                { ItemData.sword2, ItemData.shield2, ItemData.dagger2, ItemData.dagger2_s, ItemData.bow2, ItemData.arrow2,
                  ItemData.helmetH2, ItemData.helmetL2, ItemData.armorH2, ItemData.armorL2, ItemData.glovesH2, ItemData.glovesL2_s,ItemData.shoesH2_s,  ItemData.shoesL2});

            //사용자 직업과 맞지 않는 무기 삭제
            if (Program.gameData.PP_Class == PC.Warrior)
            {
                list.RemoveAll(i => i.type == ITG.DAGGER || i.type == ITG.BOW || i.type == ITG.ARROW);
            }
            else if (Program.gameData.PP_Class == PC.Rogue)
            {
                list.RemoveAll(i => i.type == ITG.SWORD || i.type == ITG.SHIELD || i.type == ITG.BOW || i.type == ITG.ARROW);
            }
            else
            {
                list.RemoveAll(i => i.type == ITG.SWORD || i.type == ITG.SHIELD || i.type == ITG.DAGGER);
            }
        }
    }
    //소모품 가게
    class PotionShop
    {
        public List<ItemPotion> list;

        public PotionShop() //클래스 만들 때 자동완성
        {
            list = new List<ItemPotion>();
            list.AddRange(new ItemPotion[]
                { Program.globalItemData.hp1, Program.globalItemData.hp2, Program.globalItemData.hp3, Program.globalItemData.sp1, Program.globalItemData.sp2, Program.globalItemData.sp3, Program.globalItemData.pp });
        }
    }

    class ItemData
    {
        public List<ItemPotion> potions = new List<ItemPotion>();
        public List<ItemGear> gears = new List<ItemGear>();
        public ItemData()
        {
            potions.AddRange(new ItemPotion[] { hp1, hp2, hp3, sp1, sp2, sp3, pp });
            gears.AddRange(new ItemGear[] { sword1, shield1, dagger1, dagger1_d, bow1, arrow1, armorH1, armorL1, glovesL1, shoesL1,
                                            sword2, shield2, dagger2, dagger2_s, bow2, arrow2, helmetH2, helmetL2, armorH2, armorL2, glovesH2, glovesL2_s, shoesH2_s, shoesL2});
        }


        public ItemPotion hp1 { get; } = new ItemPotion("작은 체력 포션", "HP를 30 회복한다.", ITP.HP, 30, 100);
        public ItemPotion hp2 { get; } = new ItemPotion("체력 포션", "HP를 80 회복한다.", ITP.HP, 80, 300);
        public ItemPotion hp3 { get; } = new ItemPotion("고급 체력 포션", "HP를 150 회복한다.", ITP.HP, 150, 600);
 
        public ItemPotion sp1 { get; } = new ItemPotion("맑은 물", "SP를 50 회복한다.", ITP.SP, 50, 80);
        public ItemPotion sp2 { get; } = new ItemPotion("자양강장제", "SP를 100 회복한다.", ITP.HP, 100, 170);
        public ItemPotion sp3 { get; } = new ItemPotion("기력환", "SP를 200 회복한다.", ITP.HP, 200, 350);

        public ItemPotion pp { get; } = new ItemPotion("체질개선 약", "투자한 스텟 포인트를 초기화한다.", ITP.PP, 0, 500);



        public static ItemGear sword1 { get; } = new ItemGear("낡은 검", "군데군데 이가 나가있다.", ITG.SWORD, 0, 0, 15, 0, 0, 0, 0, 0, 0, 0.0, 0.0, 20, 50);
        public static ItemGear shield1 { get; } = new ItemGear("빛바랜 방패", "언제부터 썼는지도 모르겠다.", ITG.SHIELD, 0, 0, 0, 10, 0, 0, 0, 0, 0, 0.0, 20.0, 20, 50);
        public static ItemGear dagger1 { get; } = new ItemGear("낡은 단도", "날이 약간 닳아 있다.", ITG.DAGGER, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0.0, 0.0, 5, 25);
        public static ItemGear dagger1_d { get; } = new ItemGear("오래된 단도", "중고로 산 기성품 단도다.", ITG.DAGGER, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0.0, 0.0, 5, 25);
        public static ItemGear bow1 { get; } = new ItemGear("낡은 활", "시위가 약간 느슨한 활이다.", ITG.BOW, 0, 0, 12, 0, 0, 0, 0, 0, 0, 0.0, 0.0, 7, 35);
        public static ItemGear arrow1 { get; } = new ItemGear("나뭇가지 화살", "급조된 화살이다.", ITG.ARROW, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0.0, 0.0, 1, 15);

        public static ItemGear armorH1 { get; } = new ItemGear("덧댄 철갑옷", "갑옷 앞뒤에 철판이 붙어있다.", ITG.ARMOR, 0, 0, 0, 15, 0, 0, 0, 0, 0, 0.0, 0.0, 25, 50);
        public static ItemGear armorL1 { get; } = new ItemGear("나무갑옷", "나무로 깎아 만들었다.", ITG.ARMOR, 0, 0, 0, 5, 0, 0, 0, 0, 0, 0.0, 0.0, 10, 30);
        public static ItemGear glovesL1 { get; } = new ItemGear("낡은 가죽글러브", "약간 해진 가죽글러브다.", ITG.GLOVES, 0, 0, 0, 3, 0, 0, 2, 0, 0, 0.0, 0.0, 5, 20);
        public static ItemGear shoesL1 { get; } = new ItemGear("털신발", "가벼운 털로 만들어졌다.", ITG.SHOES, 0, 0, 0, 3, 2, 0, 0, 0, 0, 0.0, 0.0, 5, 20);





        public static ItemGear sword2 { get; } = new ItemGear("철검", "휘두르기 좋게 만들어졌다.", ITG.SWORD, 0, 0, 30, 0, 0, 0, 0, 0, 0, 0.0, 0.0, 30, 250);
        public static ItemGear shield2 { get; } = new ItemGear("철 방패", "단단하고 묵직하다.", ITG.SHIELD, 0, 0, 0, 25, 0, 0, 0, 0, 0, 0.0, 25.0, 35, 200);
        public static ItemGear dagger2 { get; } = new ItemGear("날선 단검", "날이 약간 반듯하게 서 있다.", ITG.DAGGER, 0, 0, 10, 0, 0, 0, 0, 0, 0, 0.0, 0.0, 7, 100);
        public static ItemGear dagger2_s { get; } = new ItemGear("초급 인챈트 단검", "마법의 힘이 깃들어 있다.", ITG.DAGGER, 0, 0, 12, 0, 0, 0, 0, 0, 0, 5.0, 0.0, 8, 300);
        public static ItemGear bow2 { get; } = new ItemGear("각궁", "시위의 장력이 상당하다.", ITG.BOW, 0, 0, 25, 0, 0, 0, 0, 0, 0, 0.0, 0.0, 10, 250);
        public static ItemGear arrow2 { get; } = new ItemGear("돌 화살", "화살촉으로 뾰족한 돌이 쓰였다.", ITG.ARROW, 0, 0, 10, 0, 0, 0, 0, 0, 0, 0.0, 0.0, 3, 200);

        public static ItemGear helmetH2 { get; } = new ItemGear("철 투구", "철로 된 둥그런 투구.", ITG.HELMET, 0, 0, 0, 12, 0, 0, 0, 0, 0, 0.0, 0.0, 11, 150);
        public static ItemGear helmetL2 { get; } = new ItemGear("가죽 투구", "가죽으로 만든 각진 투구.", ITG.HELMET, 0, 0, 0, 8, 0, 0, 0, 0, 0, 0.0, 0.0, 5, 100);
        public static ItemGear armorH2 { get; } = new ItemGear("철갑옷", "전부 철로 만들어졌다.", ITG.ARMOR, 0, 0, 0, 30, 0, 0, 0, 0, 0, 0.0, 0.0, 40, 300);
        public static ItemGear armorL2 { get; } = new ItemGear("가죽갑옷", "가죽으로 짜여진 갑옷이다.", ITG.ARMOR, 0, 0, 0, 13, 0, 0, 0, 0, 0, 0.0, 0.0, 12, 200);
        public static ItemGear glovesH2 { get; } = new ItemGear("철 장갑", "마디마디가 철로 감싸져 있다.", ITG.GLOVES, 0, 0, 0, 10, 0, 0, 0, 0, 0, 0.0, 2.0, 11, 150);
        public static ItemGear glovesL2_s { get; } = new ItemGear("빛나는 가죽장갑", "세공이라도 한 건가?", ITG.GLOVES, 0, 0, 0, 8, 0, 5, 0, 0, 5, 0.0, 2.0, 8, 400);
        public static ItemGear shoesH2_s { get; } = new ItemGear("검은 돌신발", "검은 기운이 올라오고 있다.", ITG.SHOES, 13, 0, 0, 10, 0, 0, 0, 0, 0, 0.0, 0.0, 15, 450);
        public static ItemGear shoesL2 { get; } = new ItemGear("가죽 신발", "튼튼하고 가볍다.", ITG.SHOES, 0, 0, 0, 5, 4, 0, 0, 0, 0, 0.0, 0.0, 6, 150);
    }

    enum ITP //소모품 타입
    {
        HP,
        SP,
        PP //스텟분배 초기화
    }
    class WorldItemPotion
    {
        public ItemPotion itemPotion { get; set; }
        public int num { get; set; }
        public WorldItemPotion(ItemPotion _itemPotion, int _num)
        {
            itemPotion = _itemPotion;
            num = _num;
        }
    }
    class ItemPotion
    {
        public string name { get; set; } //이름은 식별자의 역할을 하기 때문에 중복되면 안됨
        public string information { get; set; }
        public ITP type { get; set; }
        public int power { get; set; }
        public int value { get; set; }

        public ItemPotion(string _name, string _information, ITP _type, int _power, int _value)
        {
            name = _name;
            information = _information;
            type = _type;
            power = _power;
            value = _value;
        }
    }

    enum ITG //장비품 타입
    {
        SWORD,
        SHIELD,
        DAGGER,
        BOW,
        ARROW,

        HELMET,
        ARMOR,
        GLOVES,
        SHOES,
        RING
    }
    enum GST //장비 부위 타입
    {
        WEAPON,
        SUBWEAPON,
        HELMET,
        ARMOR,
        GLOVES,
        SHOSE,
        RING,
        MAX
    }
    class WorldItemGear
    {
        public ItemGear itemGear { get; set; }
        public bool equipped { get; set; }
        public WorldItemGear(ItemGear _itemGear, bool _equipped)
        {
            itemGear = _itemGear;
            equipped = _equipped;
        }
    }
    class ItemGear
    {
        public string name { get; } //이름은 식별자의 역할을 하기 때문에 중복되면 안됨
        public string information { get; }
        public ITG type { get; }
        public int p_hp { get; }
        public int p_sp { get; }
        public int p_attack { get; }
        public int p_defense { get; }
        public int p_speed { get; }
        public int p_Macc { get; }
        public int p_Racc { get; }
        public int p_RaccN { get;}
        public int p_evasion { get;}
        public double p_critical { get;}
        public double p_block { get; }
        public int weight { get; }
        public int value { get; }

        public ItemGear(string _name, string _information, ITG _type, int _p_hp, int _p_sp, int _p_attack, int _p_defense, int _p_speed, int _p_Macc, int _p_Racc, int _p_RaccN, int _p_evasion, double _p_critical, double _p_block, int _weight, int _value)
        {
            name = _name;
            information = _information;
            type = _type;
            p_hp = _p_hp;
            p_sp = _p_sp;
            p_attack = _p_attack;
            p_defense = _p_defense;
            p_speed = _p_speed;
            p_Macc = _p_Macc;
            p_Racc = _p_Racc;
            p_RaccN = _p_RaccN;
            p_evasion = _p_evasion;
            p_critical = _p_critical;
            p_block = _p_block;
            weight = _weight;
            value = _value;
        }
    }

    class Skill
    {
        public string name { get; set; }
        public string information { get; set; }

        //탐색 알고리즘

        //공격력 산출
    }

    class Mercenary
    {

    }


    class GameLoader
    {
        public static GameData LoadData(int saveNum)
        {
            if (saveNum < 1 || saveNum > 3)
            {
                return null;
            }

            return LoadFromFile($"{Const.saveFileName}{saveNum}");
        }
        // JSON 파일에서 데이터를 불러오는 메서드
        private static GameData LoadFromFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    // JSON 파일을 읽어서 객체로 역직렬화
                    string jsonString = File.ReadAllText(filePath);
                    GameData playerData = JsonSerializer.Deserialize<GameData>(jsonString);
                    Console.SetCursorPosition(0, Const.screenH + 10);
                    Console.WriteLine("데이터가 성공적으로 불러와졌습니다.");
                    Console.SetCursorPosition(0, 0);

                    //아이템 리스트 가져오기
                    if (playerData.dic_inventoryPotion != null)
                    {
                        foreach (KeyValuePair<string, int> pair in playerData.dic_inventoryPotion)
                        {
                            ItemPotion ip = Program.globalItemData.potions.Find(i => i.name == pair.Key);
                            if (ip != null) playerData.inventoryPotion.Add(new WorldItemPotion(ip, pair.Value));
                        }
                    }
                    if (playerData.dic_inventoryGear != null)
                    {
                        foreach (KeyValuePair<string, bool> pair in playerData.dic_inventoryGear)
                        {
                            ItemGear ig = Program.globalItemData.gears.Find(i => i.name == pair.Key);
                            if (ig != null) playerData.inventoryGear.Add(new WorldItemGear(ig, pair.Value));
                        }
                    }
                    if (playerData.dic_equippedGear != null)
                    {
                        foreach (KeyValuePair<string, int> pair in playerData.dic_equippedGear)
                        {
                            playerData.EquipGear(pair.Key, (GST)pair.Value);
                        }
                    }
                    if (playerData.dic_gearShop != null)
                    {
                        foreach (KeyValuePair<string, int> pair in playerData.dic_gearShop)
                        {
                            ItemGear ig = Program.globalItemData.gears.Find(i => i.name == pair.Key);
                            if (ig != null) playerData.gearShop.list.Add(ig);
                        }
                    }

                    return playerData;
                }
                else
                {
                    Console.SetCursorPosition(0, Const.screenH + 10);
                    Console.WriteLine("파일을 찾을 수 없습니다.");
                    Console.SetCursorPosition(0, 0);
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.SetCursorPosition(0, Const.screenH + 10);
                Console.WriteLine("데이터 불러오기 중 오류가 발생했습니다: " + ex.Message);
                Console.SetCursorPosition(0, 0);
                return null;
            }
        }
    }
}
