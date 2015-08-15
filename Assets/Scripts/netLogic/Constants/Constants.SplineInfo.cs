using netLogic.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace netLogic.Constants
{

    public class SplineInfo
    {
        public SplineFlags Flags { get; private set; }
        public Vector3 Point { get; private set; }
        public ulong Guid { get; private set; }
        public float Rotation { get; private set; }
        public uint CurrentTime { get; private set; }
        public uint FullTime { get; private set; }
        public uint Unknown1 { get; private set; }
        public float DurationMultiplier { get; private set; }
        public float UnknownFloat2 { get; private set; }
        public float UnknownFloat3 { get; private set; }
        public uint Unknown2 { get; private set; }
        public uint Count { get; private set; }
        private readonly List<Vector3> splines = new List<Vector3>();
        public SplineMode SplineMode { get; private set; }
        public Vector3 EndPoint { get; private set; }

        public List<Vector3> Splines
        {
            get { return splines; }
        }

        public static SplineInfo Read(BinaryReader gr)
        {
            var spline = new SplineInfo();
            spline.Flags = (SplineFlags)gr.ReadUInt32();

            if (spline.Flags.HasFlag(SplineFlags.FINALPOINT))
            {
                spline.Point = gr.ReadCoords3();
            }

            if (spline.Flags.HasFlag(SplineFlags.FINALTARGET))
            {
                spline.Guid = gr.ReadUInt64();
            }

            if (spline.Flags.HasFlag(SplineFlags.FINALORIENT))
            {
                spline.Rotation = gr.ReadSingle();
            }

            spline.CurrentTime = gr.ReadUInt32();
            spline.FullTime = gr.ReadUInt32();
            spline.Unknown1 = gr.ReadUInt32();

            spline.DurationMultiplier = gr.ReadSingle();
            spline.UnknownFloat2 = gr.ReadSingle();
            spline.UnknownFloat3 = gr.ReadSingle();

            spline.Unknown2 = gr.ReadUInt32();

            spline.Count = gr.ReadUInt32();

            for (uint i = 0; i < spline.Count; ++i)
            {
                spline.splines.Add(gr.ReadCoords3());
            }

            spline.SplineMode = (SplineMode)gr.ReadByte();

            spline.EndPoint = gr.ReadCoords3();
            return spline;
        }
    };
}
