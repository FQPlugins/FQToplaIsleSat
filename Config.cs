using FQToplaSatIsle.Models;
using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FQToplaSatIsle
{
    public class Config : IRocketPluginConfiguration
    {
        public void LoadDefaults()
        {
            toplamaAlanlar = new List<ToplamaAlan>();
            islemeAlanlar = new List<IslemeAlan>();
            satisAlanlar = new List<SatAlan>();
            Cooldown = 120;
        }

        public List<ToplamaAlan> toplamaAlanlar;
        public List<IslemeAlan> islemeAlanlar;
        public List<SatAlan> satisAlanlar;
        public int Cooldown;
        public float toplamaTime;
        public float islemeTime;
        public float satisTime;
    }
}
