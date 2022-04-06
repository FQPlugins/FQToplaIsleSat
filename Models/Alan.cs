using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FQToplaSatIsle.Models
{
    public class Alan
    {
        public String alanName { get; set; }
        public Vector3 alanRegion { get; set; }
        public float alanDistance { get; set; }

        public Alan()
        {

        }

        public Alan(String name, Vector3 pos, float distance)
        {
            alanName = name;
            alanRegion = pos;
            alanDistance = distance;
        }
    }
}
