using FQToplaSatIsle.Models;
using Rocket.API;
using Rocket.Core.Commands;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FQToplaSatIsle
{
    public class Main : RocketPlugin<Config>
    {
        Color cl = new Color32(46, 204, 113, 51);


        public Dictionary<ulong, DateTime> toplamaCooldown = new Dictionary<ulong, DateTime>();
        public Dictionary<ulong, DateTime> islemeCooldown = new Dictionary<ulong, DateTime>();
        public Dictionary<ulong, DateTime> satmaCooldown = new Dictionary<ulong, DateTime>();

        protected override void Load()
        {

        }

        protected override void Unload()
        {

        }


        [RocketCommand("alanekle", "", "", AllowedCaller.Player)]
        [RocketCommandPermission("sydefq.ustduzey")]
        public void AddRegion(IRocketPlayer caller, String[] args)
        {
            var player = caller as UnturnedPlayer;
            if(args.Length == 0)
            {
                UnturnedChat.Say(player, "Kullanım: /alanekle [topla/işle/sat]");
                return;
            }

            if(args[0] == "topla")
            {
                if(args.Length < 4)
                {
                    UnturnedChat.Say(player, "Kullanım: /alanekle topla alanadi alandistance [toplanacak item id]", cl);
                    return;
                }


                if(Configuration.Instance.toplamaAlanlar.FirstOrDefault(x => x.alanName == args[1]) != null)
                {
                    UnturnedChat.Say(player, "Böyle bir alan mevcut", cl);
                    return;
                }

                if(!float.TryParse(args[2], out var distance))
                {
                    UnturnedChat.Say(player, "Kullanım: /alanekle topla alanadi alandistance [toplanacak item id]", cl);
                    return;
                }

                if(!ushort.TryParse(args[3], out var itemid))
                {
                    UnturnedChat.Say(player, "Kullanım: /alanekle topla alanadi alandistance [toplanacak item id]", cl);
                    return;
                }

                Configuration.Instance.toplamaAlanlar.Add(new ToplamaAlan
                {
                    alanName = args[1],
                    alanRegion = player.Position,
                    alanDistance = distance,
                    giveItem = itemid
                });
                UnturnedChat.Say(player, "Toplama alanı başarıyla eklendi!", cl);
                Configuration.Save();
                return;
            }

            if (args[0] == "işle")
            {
                if (args.Length < 5)
                {
                    UnturnedChat.Say(player, "Kullanım: /alanekle işle alanadi alandistance [işlenecek item id] [verilecek item id]", cl);
                    return;
                }


                if (Configuration.Instance.islemeAlanlar.FirstOrDefault(x => x.alanName == args[1]) != null)
                {
                    UnturnedChat.Say(player, "Böyle bir alan mevcut", cl);
                    return;
                }

                if (!float.TryParse(args[2], out var distance))
                {
                    UnturnedChat.Say(player, "Kullanım: /alanekle işle alanadi alandistance [işlenecek item id] [verilecek item id]", cl);
                    return;
                }

                if (!ushort.TryParse(args[3], out var islemeitemid))
                {
                    UnturnedChat.Say(player, "Kullanım: /alanekle işle alanadi alandistance [işlenecek item id] [verilecek item id]", cl);
                    return;
                }

                if (!ushort.TryParse(args[4], out var vermeitemid))
                {
                    UnturnedChat.Say(player, "Kullanım: /alanekle işle alanadi alandistance [işlenecek item id] [verilecek item id]", cl);
                    return;
                }

                Configuration.Instance.islemeAlanlar.Add(new IslemeAlan
                {
                    alanName = args[1],
                    alanRegion = player.Position,
                    alanDistance = distance,
                    giveItem = vermeitemid,
                    takeItem = islemeitemid

                });
                UnturnedChat.Say(player, "Işleme alanı başarıyla eklendi!", cl);
                Configuration.Save();
                return;
            }

            if (args[0] == "sat")
            {
                if (args.Length < 5)
                {
                    UnturnedChat.Say(player, "Kullanım: /alanekle sat alanadi alandistance [satılacak itemid] [fiyat]", cl);
                    return;
                }


                if (Configuration.Instance.satisAlanlar.FirstOrDefault(x => x.alanName == args[1]) != null)
                {
                    UnturnedChat.Say(player, "Böyle bir alan mevcut", cl);
                    return;
                }

                if (!float.TryParse(args[2], out var distance))
                {
                    UnturnedChat.Say(player, "Kullanım: /alanekle sat alanadi alandistance [satılacak itemid] [fiyat]", cl);
                    return;
                }

                if (!ushort.TryParse(args[3], out var satitemid))
                {
                    UnturnedChat.Say(player, "Kullanım: /alanekle sat alanadi alandistance [satılacak itemid] [fiyat]", cl);
                    return;
                }

                if (!uint.TryParse(args[4], out var fiyat))
                {
                    UnturnedChat.Say(player, "Kullanım: /alanekle sat alanadi alandistance [satılacak itemid] [fiyat]", cl);
                    return;
                }

                Configuration.Instance.satisAlanlar.Add(new SatAlan
                {
                    alanName = args[1],
                    alanRegion = player.Position,
                    alanDistance = distance,
                    takeItem = satitemid,
                    price = fiyat

                });
                UnturnedChat.Say(player, "Satış alanı başarıyla eklendi!", cl);
                Configuration.Save();
                return;
            }
        }


        [RocketCommand("topla", "", "", Rocket.API.AllowedCaller.Player)]
        [RocketCommandPermission("sydefq.sivil")]
        public void Topla(IRocketPlayer caller)
        {
            var player = caller as UnturnedPlayer;
            ToplamaAlan alan = Configuration.Instance.toplamaAlanlar.FirstOrDefault(x => Vector3.Distance(x.alanRegion, player.Position) <= x.alanDistance);

            if(alan == null)
            {
                UnturnedChat.Say(player, "Herhangi bir toplama alanında değilsin!", cl);
                return;
            }

            if (toplamaCooldown.ContainsKey(player.CSteamID.m_SteamID))
            {
                double timePassed = (DateTime.Now - toplamaCooldown[player.CSteamID.m_SteamID]).TotalSeconds;
                if(timePassed < Configuration.Instance.Cooldown)
                {
                    UnturnedChat.Say(player, "Toplama işlemi yapmak için çok yorgunsun. Biraz dinlen!", cl);
                    return;
                }
            }

            if (player.IsInVehicle)
            {
                UnturnedChat.Say(player, "Toplama işlemini araç dışında yapmalısın", cl);
                return;
            }


            toplamaCooldown[player.CSteamID.m_SteamID] = DateTime.Now;
            UnturnedChat.Say(player, "Toplama işlemine başladın!", cl);
            player.Player.StartCoroutine(ToplamaProcess(player, alan, Configuration.Instance.toplamaTime));

        }


        public IEnumerator ToplamaProcess(UnturnedPlayer player, ToplamaAlan alan, float toplamaTime)
        {
            for (byte page = 0; page < PlayerInventory.PAGES; page++)
            {
                if (page == PlayerInventory.AREA) { continue; }

                for (byte i = 0; player.Inventory.tryAddItemAuto(new Item(alan.giveItem, true), true, true, true, true); i++)
                {

                    if (player.IsInVehicle)
                    {
                        UnturnedChat.Say(player, "Toplama işlemi durduruldu!", cl);
                        yield break;
                    }

                    if(Vector3.Distance(alan.alanRegion, player.Position) > alan.alanDistance)
                    {
                        UnturnedChat.Say(player, "Toplama işlemi durduruldu!", cl);
                        yield break;
                    }

                    
                    UnturnedChat.Say(player, "Toplanıyor...", cl);
                    yield return new WaitForSeconds(Configuration.Instance.toplamaTime);
                }
            }
            UnturnedChat.Say(player, "Toplama işlemi sona erdi", cl);
            yield break;
        }


        [RocketCommand("işle", "", "", Rocket.API.AllowedCaller.Player)]
        [RocketCommandPermission("sydefq.sivil")]
        public void Isle(IRocketPlayer caller)
        {
            var player = caller as UnturnedPlayer;
            IslemeAlan alan = Configuration.Instance.islemeAlanlar.FirstOrDefault(x => Vector3.Distance(x.alanRegion, player.Position) <= x.alanDistance);

            if (alan == null)
            {
                UnturnedChat.Say(player, "Herhangi bir işleme alanında değilsin!", cl);
                return;
            }

            if (islemeCooldown.ContainsKey(player.CSteamID.m_SteamID))
            {
                double timePassed = (DateTime.Now - islemeCooldown[player.CSteamID.m_SteamID]).TotalSeconds;
                if (timePassed < Configuration.Instance.Cooldown)
                {
                    UnturnedChat.Say(player, "Işleme işlemi yapmak için çok yorgunsun. Biraz dinlen!", cl);
                    return;
                }
            }

            if (player.IsInVehicle)
            {
                UnturnedChat.Say(player, "Işleme işlemini araç dışında yapmalısın", cl);
                return;
            }


            islemeCooldown[player.CSteamID.m_SteamID] = DateTime.Now;
            UnturnedChat.Say(player, "Işleme işlemine başladın!", cl);
            player.Player.StartCoroutine(IslemeProcess(player, alan, Configuration.Instance.toplamaTime));

        }


        public IEnumerator IslemeProcess(UnturnedPlayer player, IslemeAlan alan, float toplamaTime)
        {
            for (byte page = 0; page < PlayerInventory.PAGES; page++)
            {
                if (page == PlayerInventory.AREA) { continue; }

                for (byte i = 0; i < player.Inventory.getItemCount(page); i++)
                {

                    if (player.IsInVehicle)
                    {
                        UnturnedChat.Say(player, "Toplama işlemi durduruldu!", cl);
                        yield break;
                    }

                    if (Vector3.Distance(alan.alanRegion, player.Position) > alan.alanDistance)
                    {
                        UnturnedChat.Say(player, "Toplama işlemi durduruldu!", cl);
                        yield break;
                    }

                    if(player.Inventory.getItem(page, i).item.id == alan.takeItem)
                    {
                        player.Inventory.removeItem(page, i);
                        player.Inventory.tryAddItemAuto(new Item(alan.giveItem, true), true, true, true, true);
                        UnturnedChat.Say(player, "Işleniyor...", cl);
                        yield return new WaitForSeconds(Configuration.Instance.islemeTime);
                        i--;
                    }
                    
                }
                
            }
            UnturnedChat.Say(player, "Işleme işlemi sona erdi", cl);
            yield break;
        }

        [RocketCommand("sat", "", "", Rocket.API.AllowedCaller.Player)]
        [RocketCommandPermission("sydefq.sivil")]
        public void Sat(IRocketPlayer caller)
        {
            var player = caller as UnturnedPlayer;
            SatAlan alan = Configuration.Instance.satisAlanlar.FirstOrDefault(x => Vector3.Distance(x.alanRegion, player.Position) <= x.alanDistance);

            if (alan == null)
            {
                UnturnedChat.Say(player, "Herhangi bir satış alanında değilsin!", cl);
                return;
            }

            if (satmaCooldown.ContainsKey(player.CSteamID.m_SteamID))
            {
                double timePassed = (DateTime.Now - satmaCooldown[player.CSteamID.m_SteamID]).TotalSeconds;
                if (timePassed < Configuration.Instance.Cooldown)
                {
                    UnturnedChat.Say(player, "Satış işlemi yapmak için çok yorgunsun. Biraz dinlen!", cl);
                    return;
                }
            }

            if (player.IsInVehicle)
            {
                UnturnedChat.Say(player, "Satış işlemini araç dışında yapmalısın", cl);
                return;
            }


            satmaCooldown[player.CSteamID.m_SteamID] = DateTime.Now;
            UnturnedChat.Say(player, "Satış işlemine başladın!", cl);
            player.Player.StartCoroutine(SatisProcess(player, alan, Configuration.Instance.toplamaTime));

        }


        public IEnumerator SatisProcess(UnturnedPlayer player, SatAlan alan, float toplamaTime)
        {
            for (byte page = 0; page < PlayerInventory.PAGES; page++)
            {
                if (page == PlayerInventory.AREA) { continue; }

                for (byte i = 0; i < player.Inventory.getItemCount(page); i++)
                {

                    if (player.IsInVehicle)
                    {
                        UnturnedChat.Say(player, "Satış işlemi durduruldu!", cl);
                        yield break;
                    }

                    if (Vector3.Distance(alan.alanRegion, player.Position) > alan.alanDistance)
                    {
                        UnturnedChat.Say(player, "Satış işlemi durduruldu!", cl);
                        yield break;
                    }

                    if (player.Inventory.getItem(page, i).item.id == alan.takeItem)
                    {
                        player.Inventory.removeItem(page, i);
                        player.Experience += alan.price;
                        UnturnedChat.Say(player, "Satılıyor...", cl);
                        yield return new WaitForSeconds(Configuration.Instance.islemeTime);
                        i--;
                    }

                }

            }
            UnturnedChat.Say(player, "Satma işlemi sona erdi", cl);
            yield break;
        }
    }
}
