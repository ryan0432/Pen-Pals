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
        BACKGROUND_SOUND = 4
    }

    private void Range_Sound_Play(Sound_Container sound_object)
    {
        if (sound_object.sound.isPlaying == false)
        {
            sound_object.sound.pitch = Random.Range(sound_object.r_range_below, sound_object.r_range_below);
            sound_object.sound.Play();
        }
    }



    /// <summary>
    /// Block Player Sounds
    /// </summary>
    /// <param name="current_key"> Current input of the player </param>
    /// <param name="in_air"> is the player in the air </param>
    public void Block_PlaySound(KeyCode current_key, bool in_air)
    {
        if (current_key == KeyCode.W || current_key == KeyCode.UpArrow)
        {
            Range_Sound_Play(Block_Sounds.jump);
        }
        else if (current_key == KeyCode.A || current_key == KeyCode.LeftArrow)
        {
            if (in_air == true)
            {
                Range_Sound_Play(Block_Sounds.jump_step);
            }
            else
            {
                Range_Sound_Play(Block_Sounds.move);
            }
        }
        else if (current_key == KeyCode.S || current_key == KeyCode.DownArrow)
        {
            if (in_air == true)
            {
                if (Block_Sounds.falling.sound.isPlaying == false)
                {
                    Block_Sounds.falling.sound.Play();
                }
            }
            else
            {
                Range_Sound_Play(Block_Sounds.land);
            }

        }
        else if (current_key == KeyCode.D || current_key == KeyCode.RightArrow)
        {
            if (in_air == true)
            {
                Range_Sound_Play(Block_Sounds.jump_step);
            }
            else
            {
                Range_Sound_Play(Block_Sounds.move);
            }
        }
    }




    /// <summary>
    /// Line Player Sound - Move
    /// </summary>
    public void Line_PlaySound()
    {
        Range_Sound_Play(Line_Sounds.move);
    }



    public void Game_PlaySound(Game_Sound game_sound)
    {
        switch (game_sound)
        {
            case Game_Sound.STICKER_COLLECT:
                Range_Sound_Play(Game_Sounds.sticker_collect);
                break;
            case Game_Sound.LEVEL_COMPLETE:
                Range_Sound_Play(Game_Sounds.level_complete);
                break;
            case Game_Sound.BUTTON_SELECT:
                Range_Sound_Play(Game_Sounds.button_select);
                break;
            case Game_Sound.BUTTON_CONFIRM:
                Range_Sound_Play(Game_Sounds.button_confirm);
                break;
            case Game_Sound.BACKGROUND_SOUND:
                if (Game_Sounds.background.sound.isPlaying == false)
                {
                    Game_Sounds.background.sound.Play();
                    Game_Sounds.background.sound.loop = true;
                }
                break;
            default:
                break;
        }
    }
}


[System.Serializable]
public struct Block_Sounds
{
    public Sound_Container move;
    public Sound_Container land;
    public Sound_Container jump;
    public Sound_Container jump_step;
    public Sound_Container falling;
}



[System.Serializable]
public struct Line_Sounds
{
    public Sound_Container move;
}


[System.Serializable]
public struct Game_Sounds
{
    public Sound_Container sticker_collect;
    public Sound_Container level_complete;
    public Sound_Container button_select;
    public Sound_Container button_confirm;
    public Sound_Container background;
}


[System.Serializable]
public struct Sound_Container
{
    [Range(0.1f, 5.0f)]
    public float r_range_above;
    [Range(0.1f, 5.0f)]
    public float r_range_below;

    public AudioSource sound;
}

