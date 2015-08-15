using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace netLogic.Utilities
{
    public static class Metrics
    {
        public const float Tilesize = 533 + 1.0f / 3.0f;
        public const float Chunksize = Tilesize / 16.0f;
        public const float Unitsize = Chunksize / 8;
        public const float MidPoint = 32.0f * Tilesize;
        public const float ChunkRadius = 23.55549f;

        public static Vector3 ToServerCoords(Vector3 clientCoords)
        {
            var serverVec = clientCoords;
            serverVec.x = -Utilities.Metrics.MidPoint + serverVec.x;
            serverVec.y = -Utilities.Metrics.MidPoint + serverVec.y;
            return serverVec;
        }

        public static Vector3 ToClientCoords(Vector3 serverCoords)
        {
            var clientVec = serverCoords;
            clientVec.x = clientVec.x + MidPoint;
            clientVec.y = clientVec.y + MidPoint;
            return clientVec;
        }

        public static Vector2 ToServerCoords(Vector2 clientCoords)
        {
            var serverVec = clientCoords;
            serverVec.x = -Utilities.Metrics.MidPoint + serverVec.x;
            serverVec.y = -Utilities.Metrics.MidPoint + serverVec.y;
            return serverVec;
        }

        public static Vector2 ToClientCoords(Vector2 serverCoords)
        {
            var clientVec = serverCoords;
            clientVec.x = clientVec.x + MidPoint;
            clientVec.y = clientVec.y + MidPoint;
            return clientVec;
        }
    }
}
