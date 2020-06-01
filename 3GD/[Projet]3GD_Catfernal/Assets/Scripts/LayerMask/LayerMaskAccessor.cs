using System;
using UnityEngine;

public static class LayerMaskAccessor
{
    #region Fields
    // --- Player ---
    public static int playerBaseLayerMask = 0;
    public static int playerSpriteLayerMask = 0;

    // --- Firefly ---
    public static int fireflyLayerMask = 0;

    // --- Obstacle ---
    public static int obstacleWallLayerMask = 0;
    #endregion

    #region Contructors
    static LayerMaskAccessor ()
    {
        playerBaseLayerMask = 1 << LayerMask.NameToLayer("Player_Base");
        playerSpriteLayerMask = 1 << LayerMask.NameToLayer("Player_Sprite");
        fireflyLayerMask = 1 << LayerMask.NameToLayer("Firefly");
        obstacleWallLayerMask = 1 << LayerMask.NameToLayer("Obstacle_Wall");
    }
	#endregion
}
