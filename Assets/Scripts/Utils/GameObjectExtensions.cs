using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class GameObjectExtensions 
    {
        const string PLAYER_TAG = "Player";
        public static bool IsPlayer(this GameObject gameObject)
        {
            return gameObject.CompareTag(PLAYER_TAG);
        }
    }
}