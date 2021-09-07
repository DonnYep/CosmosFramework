using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.Test
{
    public enum OperationCode:byte
    {
        SYN=73,
        PlayerEnter = 74,
        PlayerExit= 75,
        PlayerInput= 76,
        FIN=77
    }
}