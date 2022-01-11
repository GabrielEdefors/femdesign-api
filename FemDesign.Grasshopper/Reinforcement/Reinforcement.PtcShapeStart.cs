﻿using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Linq;
using System.Collections.Generic;

namespace FemDesign.Reinforcement
{
    public class PtcShapeStartComponent : GH_Component
    {
        public PtcShapeStartComponent() : base("PtcShapeStart", "PtcStart", "FemDesign.Reinforcement.PtcShapeStart", "FemDesign", "Ptc")
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("z'", "z'", "Height", GH_ParamAccess.item);
            pManager.AddNumberParameter("Tangent", "Tangent", "Tangent of cable at start", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Start", "Start", "FemDesign.Reinforcement.PtcShapeStart", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double z = 0.0;
            DA.GetData("z'", ref z);
            double tangent = 0.0;
            DA.GetData("Tangent", ref tangent);

            PtcShapeStart start = new PtcShapeStart(z, tangent);

            DA.SetData("Start", start);
        }

        protected override System.Drawing.Bitmap Icon => FemDesign.Properties.Resources.PtcShapeStart;

        public override Guid ComponentGuid => new Guid("a3c7f499-f956-414f-b2b5-104ac864ebf3");
    }
}