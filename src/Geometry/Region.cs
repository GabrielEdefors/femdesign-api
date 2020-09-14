// https://strusoft.com/

using System.Collections.Generic;
using System.Xml.Serialization;

#region dynamo
using Autodesk.DesignScript.Runtime;
#endregion

namespace FemDesign.Geometry
{
    /// <summary>
    /// region_type
    /// 
    /// Surfaces in FEM-Design are expressed as regions of contours (outlines). This extended region also contains a LCS to keep track of directions.
    /// </summary>
    [System.Serializable]
    [IsVisibleInDynamoLibrary(false)]
    public class Region
    {
        [XmlIgnore]
        internal Geometry.FdCoordinateSystem CoordinateSystem { get; set; }
        [XmlElement("contour")]
        public List<Contour> Contours = new List<Contour>(); // sequence: contour_type

        /// <summary>
        /// Parameterless constructor for serialization.
        /// </summary>
        internal Region()
        {
            
        }

        internal Region(List<Contour> contours)
        {
            this.Contours = contours;
        }

        internal Region(List<Contour> contours, FdCoordinateSystem coordinateSystem)
        {
            this.Contours = contours;
            this.CoordinateSystem = coordinateSystem;
        }

        /// <summary>
        /// Create region by points and coordinate system.
        /// </summary>
        /// <param name="points">List of sorted points defining the outer perimeter of the region.</param>
        /// <param name="coordinateSystem">Coordinate system of the region</param>
        internal Region(List<FdPoint3d> points, FdCoordinateSystem coordinateSystem)
        {
            // edge normal
            FdVector3d edgeLocalY = coordinateSystem.LocalZ;

            List<Edge> edges = new List<Edge>();
            for (int idx = 0 ; idx < points.Count; idx++)
            {
                // startPoint
                FdPoint3d p0 = p0 = points[idx];

                // endPoint
                FdPoint3d p1;
                if (idx != points.Count - 1)
                {
                    p1 = points[idx + 1];
                }

                else
                {
                    p1 = points[0];
                }

                // create edge
                edges.Add(new Edge(p0, p1, edgeLocalY));
            }
            
            // create contours
            Contour contour = new Contour(edges);

            // set properties
            this.Contours = new List<Contour>{contour};
            this.CoordinateSystem = coordinateSystem;
        }
        
        /// <summary>
        /// Get region from a Slab.
        /// </summary>
        internal static Region FromSlab(Shells.Slab slab)
        {
            return slab.SlabPart.Region;
        }

        /// <summary>
        /// Set EdgeConnection on Edge in region by index.
        /// </summary>
        internal void SetEdgeConnection(Shells.ShellEdgeConnection edgeConnection, int index)
        {
            if (edgeConnection.Release)
            {
                int edgeIdx = 0;
                foreach (Contour contour in this.Contours)
                {
                    if (contour.Edges != null)
                    {
                        int cInstance = 0;
                        foreach (Edge edge in contour.Edges)
                        {
                            cInstance++;
                            if (index == edgeIdx)
                            {
                                string name = "CE." + cInstance.ToString();
                                Shells.ShellEdgeConnection ec = Shells.ShellEdgeConnection.CopyExisting(edgeConnection, name);
                                edge.EdgeConnection = ec;
                                return;
                            }
                            edgeIdx++;
                        }
                    }
                    else
                    {
                        throw new System.ArgumentException("No edges in contour!");
                    }
                }
            }
            else
            {
                // don't modify edges if no release on edgeConnection.
            }

            // edge not found
            throw new System.ArgumentException("Edge not found.");
        }

        /// <summary>
        /// Set EdgeConnection on all Edges in Region.
        /// </summary>
        internal void SetEdgeConnections(Shells.ShellEdgeConnection edgeConnection)
        {
            if (edgeConnection.Release)
            {
                foreach (Contour contour in this.Contours)
                {
                    if (contour.Edges != null)
                    {
                        int cInstance = 0;
                        foreach (Edge edge in contour.Edges)
                        {
                            cInstance++;
                            string name = "CE." + cInstance.ToString();
                            Shells.ShellEdgeConnection ec = Shells.ShellEdgeConnection.CopyExisting(edgeConnection, name);
                            edge.EdgeConnection = ec;
                        }
                    }
                    else
                    {
                        throw new System.ArgumentException("No edges in contour!");
                    }
                }
            }
            else
            {
                // don't modify edges if no releases on edgeConnection.
            }
        }

        /// <summary>
        /// Get all EdgeConnection from all Edges in Region.
        /// </summary>
        internal List<Shells.ShellEdgeConnection> GetEdgeConnections()
        {
            var edgeConnections = new List<Shells.ShellEdgeConnection>();
            foreach (Contour contour in this.Contours)
            {
                foreach (Edge edge in contour.Edges)
                {
                    edgeConnections.Add(edge.EdgeConnection);
                }
            }
            return edgeConnections;
        }

        /// <summary>
        /// Returns a new instance of region, without any EdgeConnections.
        /// </summary>
        /// <returns></returns>
        internal Region RemoveEdgeConnections()
        {
            Region newRegion = this.DeepClone();
            foreach (Contour newContour in newRegion.Contours)
            {
                foreach (Edge newEdge in newContour.Edges)
                {
                    if (newEdge.EdgeConnection != null)
                    {
                        newEdge.EdgeConnection = null;
                    }
                }
            }
            return newRegion;
        }

        #region dynamo
        /// <summary>
        /// Create Region from Dynamo surface.
        /// </summary>
        public static Geometry.Region FromDynamo(Autodesk.DesignScript.Geometry.Surface obj)
        {
            // get all perimeter curves
            // curves[] is ordered by the loops of the surface.
            // the assumption here is that the loop with the largest circumference (i.e. the outline) is placed first in the array
            // for fd definition it is neccessary that the outline is the first contour, the subsequent loops can have any order.
            Autodesk.DesignScript.Geometry.Curve[] curves = obj.PerimeterCurves();

            // find closed outlines
            List<Autodesk.DesignScript.Geometry.Curve> perimeterCurves = new List<Autodesk.DesignScript.Geometry.Curve>();
            List<List<Autodesk.DesignScript.Geometry.Curve>> container = new List<List<Autodesk.DesignScript.Geometry.Curve>>();

            Autodesk.DesignScript.Geometry.Point pA0, pA1, pB0, pB1;
            // control if new perimeter
            // this happens is pA0/pA1 != pB0/pB1
            for (int idx = 0; idx < curves.Length; idx++)
            {
                if (idx == 0)
                {
                    perimeterCurves.Add(curves[idx]);
                }

                else
                {
                    pA0 = curves[idx - 1].StartPoint;
                    pA1 = curves[idx - 1].EndPoint;
                    pB0 = curves[idx].StartPoint;
                    pB1 = curves[idx].EndPoint;

                    // using Autodesk.DesignScript.Geometry.Point.Equals causes tolerance errors
                    // instead Autodesk.DesignScript.Geometry.Point.IsAlmostEqualTo is used
                    // note that this can cause errors as it is unclear what tolerance IsAlmostEqualTo uses
                    // another alternative would be to create an extension method, Equal(Point point, double tolerance), to Autodesk.DesignScript.Geometry.Point
                    if (pA0.IsAlmostEqualTo(pB0) || pA0.IsAlmostEqualTo(pB1) || pA1.IsAlmostEqualTo(pB0) || pA1.IsAlmostEqualTo(pB1))
                    { 
                        perimeterCurves.Add(curves[idx]);
                    }

                    // new perimeter
                    else
                    {
                        container.Add(new List<Autodesk.DesignScript.Geometry.Curve>(perimeterCurves));
                        perimeterCurves.Clear();
                        perimeterCurves.Add(curves[idx]);
                    }
                }
            }
            // add last perimeter to container
            container.Add(new List<Autodesk.DesignScript.Geometry.Curve>(perimeterCurves));
            perimeterCurves.Clear();

            // control if direction is consistent
            // as FromRhinoBrep.
            foreach (List<Autodesk.DesignScript.Geometry.Curve> items in container)
            {
                // if Contour consists of one curve.
                if (items.Count == 1)
                {
                    // check if curve is a Circle

                    // if curve is not a Circle, raise error.
                }

                // if Contour consists of more than one curve.
                else
                {
                    // using Autodesk.DesignScript.Geometry.Point.Equals causes tolerance errors
                    // instead Autodesk.DesignScript.Geometry.Point.IsAlmostEqualTo is used
                    // note that this can cause errors as it is unclear what tolerance IsAlmostEqualTo uses
                    // another alternative would be to create an extension method, Equal(Point point, double tolerance), to Autodesk.DesignScript.Geometry.Point
                    for (int idx = 0; idx < items.Count - 1; idx++)
                    {
                        // curve a = items[idx]
                        // curve b = items[idx + 1]
                        pA0 = items[idx].StartPoint;
                        pA1 = items[idx].EndPoint;
                        pB0 = items[idx + 1].StartPoint;
                        pB1 = items[idx + 1].EndPoint;

                        if (pA0.IsAlmostEqualTo(pB0))
                        {
                            if (idx == 0)
                            {
                                items[idx] = items[idx].Reverse();
                            }
                            else
                            {
                                throw new System.ArgumentException("pA0 == pB0 even though idx != 0. Bad outline.");
                            }
                        }

                        else if (pA0.IsAlmostEqualTo(pB1))
                        {
                            if (idx == 0)
                            {
                                items[idx] = items[idx].Reverse();
                                items[idx + 1] = items[idx + 1].Reverse();
                            }
                            else
                            {
                                throw new System.ArgumentException("pA0 == pB1 even though idx != 0. Bad outline.");
                            }
                        }

                        else if (pA1.IsAlmostEqualTo(pB0))
                        {
                            // pass
                        }

                        else if (pA1.IsAlmostEqualTo(pB1))
                        {
                            items[idx + 1] = items[idx + 1].Reverse();
                        }

                        else
                        {
                            throw new System.ArgumentException("Can't close outline. Bad outline.");
                        }
                    }
                }
            }

            // create contours
            List<Geometry.Edge> edges = new List<Geometry.Edge>();
            List<Geometry.Contour> contours = new List<Geometry.Contour>();

            foreach (List<Autodesk.DesignScript.Geometry.Curve> items in container)
            {
                foreach (Autodesk.DesignScript.Geometry.Curve curve in items)
                {
                    edges.Add(Geometry.Edge.FromDynamo(curve));
                }
                contours.Add(new Geometry.Contour(new List<Edge>(edges)));
                edges.Clear();
            }

            // get LCS
            FdCoordinateSystem cs = FdCoordinateSystem.FromDynamoSurface(obj);

            // return
            return new Geometry.Region(contours, cs);
        }

        /// <summary>
        /// Convert Region to Dynamo surface.
        /// </summary>
        public Autodesk.DesignScript.Geometry.Surface ToDynamoSurface()
        {
            // get closed curves
            List<Autodesk.DesignScript.Geometry.PolyCurve> closedCurves = new List<Autodesk.DesignScript.Geometry.PolyCurve>();
            foreach (Geometry.Contour contour in this.Contours)
            {
                List<Autodesk.DesignScript.Geometry.Curve> curves = new List<Autodesk.DesignScript.Geometry.Curve>();
                foreach (Geometry.Edge edge in contour.Edges)
                {
                    curves.Add(edge.ToDynamo());
                }
                closedCurves.Add(Autodesk.DesignScript.Geometry.PolyCurve.ByJoinedCurves(curves));
                curves.Clear();
            }

            // get surface
            List<Autodesk.DesignScript.Geometry.Surface> surfaces = new List<Autodesk.DesignScript.Geometry.Surface>();
            foreach (Autodesk.DesignScript.Geometry.PolyCurve closedCurve in closedCurves)
            {
                surfaces.Add(Autodesk.DesignScript.Geometry.Surface.ByPatch(closedCurve));
            }
            Autodesk.DesignScript.Geometry.Surface primarySurface = surfaces[0];
            surfaces.RemoveAt(0);
            foreach (Autodesk.DesignScript.Geometry.Surface secondarySurface in surfaces)
            {
                primarySurface = (Autodesk.DesignScript.Geometry.Surface)primarySurface.Split(secondarySurface)[0];
            }

            // return
            return primarySurface;
        }

        /// <summary>
        /// Convert Edges in Region to Dynamo curves.
        /// </summary>
        public List<List<Autodesk.DesignScript.Geometry.Curve>> ToDynamoCurves()
        {
            List<List<Autodesk.DesignScript.Geometry.Curve>> outlines = new List<List<Autodesk.DesignScript.Geometry.Curve>>();
            foreach (Geometry.Contour contour in this.Contours)
            {
                List<Autodesk.DesignScript.Geometry.Curve> curves = new List<Autodesk.DesignScript.Geometry.Curve>();
                foreach (Geometry.Edge edge in contour.Edges)
                {
                    curves.Add(edge.ToDynamo());
                }
                outlines.Add(new List<Autodesk.DesignScript.Geometry.Curve>(curves));
                curves.Clear();
            }
            return outlines;
        }

        #endregion

    }
}