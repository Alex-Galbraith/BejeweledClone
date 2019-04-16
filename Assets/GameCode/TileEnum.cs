using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TSwapper {
    [System.Flags]
    public enum TileType {
        EMPTY   = 0,
        RED     = 1,
        GREEN   = 2,
        BLUE    = 4,
        YELLOW  = 8,
        PURPLE  = 16,
        BOMB    = 32,
    }

    public enum PoolGroup {
        BASIC
    }
}