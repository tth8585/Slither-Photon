using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseEventData
{
    public const byte GAME_START = 100;
    public const byte SPAWN_FOOD_FIRST_TIME = 101;
    public const byte SPAWN_PLAYER_FIRST_TIME = 102;
    public const byte SPAWN_BOT_FIRST_TIME = 103;

    public const byte RESPAWN_FOOD = 104;
    public const byte RESPAWN_BOT = 105;
    public const byte RESPAWN_PLAYER = 106;

    public const byte PLAYER_EAT_FOOD = 107; //??
    public const byte PLAYER_DIES = 108;
    public const byte PLAYER_LEFT_GAME = 109;
    public const byte PLAYER_JOIN_GAME = 110;

    public const byte PLAYER_UPDATE_SCORE = 111;
    public const byte FOOD_IS_TOO_OLD = 112;
    public const byte ADD_PLAYER_PART = 113;
    public const byte UPDATE_PLAYER_PART_AND_SCORE = 114;
    public const byte FOOD_SPRINT = 115;
    public const byte CHECK_HEAD_COLLIDER = 116;
    public const byte SPAWN_FOOD_WHEN_PLAYER_DIE = 117;
}
