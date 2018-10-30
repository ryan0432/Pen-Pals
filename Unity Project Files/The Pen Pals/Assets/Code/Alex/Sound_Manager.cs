//*!----------------------------!*//
//*! Programmer: Alex Scicluna
//*!----------------------------!*//

//*! Using namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Sound_Manager : MonoBehaviour
{
    public Block_Sounds Block_Sounds;

    public Line_Sounds Line_Sounds;

    public Game_Sounds Game_Sounds;

    public enum Game_Sound
    {
        STICKER_COLLECT = 0,
        LEVEL_COMPLETE = 1,
        BUTTON_SELECT = 2,
        BUTTON_CONFIRM = 3,
        BACKGROUND = 4
    }


    /// <summary>
    /// Block Player Sounds
    /// </summary>
    /// <param name="current_key"></param>
    /// <param name="in_air"></param>
    public void Block_PlaySound(KeyCode current_key, bool in_air)
    {
        if (current_key == KeyCode.W || current_key == KeyCode.UpArrow)
        {
            if (in_air == true)
            {
                Block_Sounds.move.Play();//*! Higher
            }
            else
            {
                Block_Sounds.move.Play();//*! Lower
            }
        }
        else if (current_key == KeyCode.A || current_key == KeyCode.LeftArrow)
        {
            if (in_air == true)
            {
                Block_Sounds.move.Play();//*! Higher
            }
            else
            {
                Block_Sounds.move.Play();//*! Lower
            }
        }
        else if (current_key == KeyCode.S || current_key == KeyCode.DownArrow)
        {
            if (in_air == true)
            {
                Block_Sounds.move.Play();//*! Higher
            }
            else
            {
                Block_Sounds.land.Play();//*! Lower
            }
        }
        else if (current_key == KeyCode.D || current_key == KeyCode.RightArrow)
        {
            if (in_air == true)
            {
                Block_Sounds.move.Play();//*! Higher
            }
            else
            {
                Block_Sounds.move.Play();//*! Lower
            }
        }
    }



    /// <summary>
    /// Line Player Sounds
    /// </summary>
    /// <param name="current_key"></param>
    public void Line_PlaySound(KeyCode current_key)
    {
        Line_Sounds.move.Play();
    }


    public void Game_PlaySound(Game_Sound game_sound)
    {
        switch (game_sound)
        {
            case Game_Sound.STICKER_COLLECT:
                Game_Sounds.sticker_collect.Play();
                break;
            case Game_Sound.LEVEL_COMPLETE:
                Game_Sounds.level_complete.Play();
                break;
            case Game_Sound.BUTTON_SELECT:
                Game_Sounds.button_select.Play();
                break;
            case Game_Sound.BUTTON_CONFIRM:
                Game_Sounds.button_confirm.Play();
                break;
            default:
                break;
        }
    }

}

[System.Serializable]
public struct Block_Sounds
{
    public AudioSource move;
    public AudioSource land;
    public AudioSource jump;
    public AudioSource jump_step;
}



[System.Serializable]
public struct Line_Sounds
{
    public AudioSource move;
}


[System.Serializable]
public struct Game_Sounds
{
    public AudioSource sticker_collect;
    public AudioSource level_complete;
    public AudioSource button_select;
    public AudioSource button_confirm;
}