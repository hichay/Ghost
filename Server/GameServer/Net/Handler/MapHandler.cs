﻿using Server.Common.Constants;
using Server.Common.Data;
using Server.Common.IO.Packet;
using Server.Ghost;
using Server.Ghost.Characters;
using Server.Ghost.Provider;
using Server.Net;
using Server.Packet;
using System.Collections.Generic;

namespace Server.Handler
{
    public static class MapHandler
    {
        public static void WarpToMap_Req(InPacket lea, Client gc)
        {
            int CharacterID = lea.ReadInt();
            short MapX = lea.ReadShort();
            short MapY = lea.ReadShort();
            short PositionX = lea.ReadShort();
            short PositionY = lea.ReadShort();
            var chr = gc.Character;

            Map Map = MapFactory.GetMap(chr.MapX, chr.MapY);
            Map.Characters.Remove(chr);
            foreach (Character All in Map.Characters)
                MapPacket.removeUser(All.Client, chr);

            if (MapX == 77 && MapY == 1)
            {
                CashShopPacket.CashShopList1(gc); // 人物
                CashShopPacket.CashShopList2(gc); // 裝備
                CashShopPacket.CashShopList3(gc); // 能力
                CashShopPacket.CashShopList4(gc); // 靈物
                CashShopPacket.CashShopList5(gc); // 寶牌
                CashShopPacket.CashShopList6(gc);
                CashShopPacket.CashShopList7(gc); // 紅利積點
                CashShopPacket.CashShopList8(gc);
                CashShopPacket.CashShopList9(gc);
                CashShopPacket.MgameCash(gc);
                CashShopPacket.GuiHonCash(gc);

                // 接收禮物
                List<int> Gifts = new List<int>();
                
                foreach (dynamic datum in new Datums("Gifts").Populate())
                {
                    if (chr.Name.Equals(datum.name) && datum.receive == 0)
                    {
                        Gifts.Add(datum.itemID);
                        datum.receive = 1;
                        datum.Update("id = '{0}'", datum.id);
                    }
                }
                foreach (int ItemID in Gifts)
                {
                    chr.Items.Add(new Item(ItemID, true, 0, -1, (byte)InventoryType.ItemType.Cash, chr.Items.GetNextFreeSlot(InventoryType.ItemType.Cash)));
                    chr.Items.Save();
                }
                InventoryPacket.getInvenCash(gc);
                MapPacket.warpToMap(gc, chr, CharacterID, MapX, MapY, PositionX, PositionY);
                return;
            }

            chr.MapX = MapX;
            chr.MapY = MapY;
            chr.PlayerX = PositionX;
            chr.PlayerY = PositionY;

            Map = MapFactory.GetMap(chr.MapX, chr.MapY);
            Map.Characters.Add(chr);
            foreach (Character All in Map.Characters)
                MapPacket.warpToMap(All.Client, chr, CharacterID, MapX, MapY, PositionX, PositionY);

            if (Map.GetMapCharactersTotal() > 1)
            {
                foreach (Character All in Map.Characters)
                {
                    MapPacket.createUser(All.Client, chr, Map);
                }
            }

            if ((Map.MapX == 1 && Map.MapY == 53) || (Map.MapX == 1 && Map.MapY == 54) || (Map.MapX == 1 && Map.MapY == 55))
                return;
            
            MonsterPacket.createAllMonster(gc, Map, Map.Monster);

            int j = 0;

            for (int i = 0; i < 50; i++)
            {
                if (Map.Monster[i] != null)
                {
                    j++;
                }
            }

            //for (int i = 0; i < j; i++)
            //{
            //    foreach (Character All in map.Characters)
            //    {
            //        if (map.Monster[i].IsAlive == true)
            //            MonsterPacket.spawnMonster(All.Client, map.Monster[i], 0, 0, 0, 0);
            //    }
            //}

            //if (map.GetMapCharactersTotal() < 1)
            //{
                Map.ControlMonster(gc, j);
            //}
            
            if (chr.IsFuring == true)
                StatusPacket.Fury(gc, chr.FuringType);
        }

        public static void WarpToMapAuth_Req(InPacket lea, Client gc)
        {
            var chr = gc.Character;
            short MapX = lea.ReadShort();
            short MapY = lea.ReadShort();
            short PositionX = lea.ReadShort();
            short PositionY = lea.ReadShort();
            bool IsAvailableMap = true;
            if (chr.IsAlive == false)
            {
                StatusPacket.UpdateHpMp(gc, chr.Hp, chr.Mp, chr.Fury, chr.MaxFury);
                chr.IsAlive = true;
                switch (MapX)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 22:
                    case 23:
                        MapX = 1;
                        MapY = 1;
                        break;
                    case 7:
                    case 8:
                    case 9:
                        MapX = 16;
                        MapY = 1;
                        break;
                    case 10:
                    case 11:
                    case 20:
                        MapX = 10;
                        MapY = 1;
                        break;
                    case 12:
                    case 13:
                        MapX = 12;
                        MapY = 1;
                        break;
                    case 14:
                    case 15:
                    case 17:
                    case 18:
                    case 19:
                    case 21:
                        MapX = 15;
                        MapY = 1;
                        break;
                    case 16:
                        MapX = 16;
                        MapY = 1;
                        break;
                    case 24:
                    case 25:
                    case 26:
                    case 31:
                    case 32:
                    case 33:
                        MapX = 25;
                        MapY = 1;
                        break;
                    case 27:
                    case 28:
                        MapX = 27;
                        MapY = 1;
                        break;
                    default:
                        MapX = 1;
                        MapY = 1;
                        break;
                }
                PositionX = 0;
                PositionY = 0;
            }
            MapPacket.warpToMapAuth(gc, IsAvailableMap, MapX, MapY, PositionX, PositionY);
        }
    }
}
