﻿using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Linq;
using System.Collections.Generic;

namespace FemDesign.Reinforcement
{
    public class PtcShapeEndComponent : GH_Component
    {
        public PtcShapeEndComponent() : base("PtcShapeEnd", "PtcEnd", "FemDesign.Reinforcement.PtcShapeEnd", "FemDesign", "Ptc")
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("z'", "z'", "Height", GH_ParamAccess.item);
            pManager.AddNumberParameter("Tangent", "Tangent", "Tangent of cable at end", GH_ParamAccess.item);
            pManager.AddNumberParameter("PriorInflection x'", "PriorInflection x'", "Parameter of prior inflection point. Parameter between 0-1 along parent element. Optional", GH_ParamAccess.item);
            pManager[pManager.ParamCount - 1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("End", "End", "FemDesign.Reinforcement.PtcShapeEnd", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double z = 0.0;
            if (!DA.GetData("z'", ref z)) return;

            double tangent = 0.0;
            if (!DA.GetData("Tangent", ref tangent)) return;

            double? priorInflectionParam = null;
            DA.GetData("PriorInflection x'", ref priorInflectionParam);
            
            PtcShapeEnd end = new PtcShapeEnd(z, tangent, priorInflectionParam);

            DA.SetData("End", end);
        }

        protected override System.Drawing.Bitmap Icon => FemDesign.Properties.Resources.PtcShapeEnd;

        public override Guid ComponentGuid => new Guid("0c0d9f85-c235-4eea-84c9-a6fe15603581");
    }
}