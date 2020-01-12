using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Cosmos.Test
{
    public class ReflectionTester : MonoBehaviour
    {
        private void Start()
        {
            CreateObject();
            CreateObjectDefault();
        }
        Human human = new Human();

        public void CreateObject()
        {
            string fullName = "Cosmos.Test.Human";
            var obj = Utility.GetTypeInstance<Creature>(human.GetType(),fullName);
           
        }
        public void CreateObjectDefault()
        {
            string fullName = "Cosmos.Test.Human";
            var assembly = human.GetType().Assembly;
            var obj = Utility.GetTypeInstance<Creature>(assembly, fullName);
        }
    }
}