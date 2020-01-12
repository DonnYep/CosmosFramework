using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.Test
{
    public class Cat : Creature
    { 
        public Cat()
        {
            this.Message = "Cat";
        }
        public Cat(string mess)
        {
            this.Message = mess;
        }
    }
}