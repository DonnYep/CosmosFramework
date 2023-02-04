using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos.UI;
using Cosmos;

public class EntityGameLauncherPanel : UGUIUIForm
{
    Button btnStartGame;
    public override void OnInit()
    {
        btnStartGame = GetUILabel<Button>("BtnStartGame");
        btnStartGame.onClick.AddListener(OnStartGameClick);
    }
    void OnStartGameClick()
    {
        CosmosEntry.ProcedureManager.RunProcedureNode<EntityGameState>();
    }
}
