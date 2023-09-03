using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP {
    public sealed class GWorld
    {
        private static readonly GWorld _instance = new GWorld(); // Private Singleton instance
        private static WorldStates world;


        static GWorld()
        {
            world = new WorldStates();
        }

        private GWorld()
        {

        }

        public static GWorld Instance { get { return _instance; } } // Singleton instance getter

        public WorldStates GetWorld()
        {
            return world;
        }
    }
}

