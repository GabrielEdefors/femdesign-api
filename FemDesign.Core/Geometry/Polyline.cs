﻿// https://strusoft.com/

using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;


namespace FemDesign.Geometry
{
    [System.Serializable]
    public partial class Polyline
    {
        [XmlElement("point")]
        public List<FdPoint3d> Verticies;

        /// <summary>
        /// Parameterless constructor for serialization.
        /// </summary>
        public Polyline()
        {

        }

        /// <summary>
        /// Construct Polyline from points.
        /// </summary>
        /// <param name="verticies"></param>
        public Polyline(List<FdPoint3d> verticies)
        {
            Initialize(verticies);
        }

        /// <summary>
        /// Construct Polyline from points.
        /// </summary>
        /// <param name="verticies"></param>
        public Polyline(params FdPoint3d[] verticies)
        {
            Initialize(verticies.ToList());
        }

        private void Initialize(List<FdPoint3d> verticies)
        {
            if (verticies.Count < 3)
                throw new ArgumentException($"Polyline must have at least 3 points but got {verticies.Count}.");

            this.Verticies = verticies;
        }
    }
}