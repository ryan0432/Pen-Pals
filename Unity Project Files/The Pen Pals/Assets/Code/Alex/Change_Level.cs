//*!----------------------------!*//
//*! Programmer: Alex Scicluna 
//*!----------------------------!*//


//*! Using namespaces
using UnityEngine;

public class Change_Level : MonoBehaviour
{
    Game_Manager game_manager;

    private void Start()
    {
        game_manager = GetComponent<Game_Manager>();
    }

    private void Update()
    {
        if (game_manager.Blue_Sticker_Count == 0 && game_manager.Red_Sticker_Count == 0)
        {
            game_manager.Initialize_Level();
        }
    }
}
