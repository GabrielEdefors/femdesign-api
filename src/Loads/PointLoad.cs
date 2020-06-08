// https://strusoft.com/

using System.Xml.Serialization;

namespace FemDesign.Loads
{
    /// <summary>
    /// point_load_type
    /// </summary>
    public class PointLoad: ForceLoadBase
    {
        [XmlElement("direction")]
        public Geometry.FdVector3d direction { get; set; } // point_type_3d
        [XmlElement("load")]
        public LoadLocationValue load { get; set; } // location_value

        /// <summary>
        /// Parameterless constructor for serialization.
        /// </summary>
        private PointLoad()
        {

        }

        /// <summary>
        /// Internal constructor accessed by static methods.
        /// </summary>
        internal PointLoad(Geometry.FdPoint3d point, Geometry.FdVector3d force, LoadCase loadCase, string comment, string type)
        {
            this.EntityCreated();
            this.loadCase = loadCase.guid;
            this.comment = comment;
            this.loadType = type;
            this.direction = force.Normalize();
            this.load = new LoadLocationValue(point, force.Length());
        }

        
        #region grasshopper

        /// <summary>
        /// Convert PointLoad point to Rhino point.
        /// </summary>
        internal Rhino.Geometry.Point3d GetRhinoGeometry()
        {
            return this.load.GetFdPoint().ToRhino();
        }
        #endregion
    }
}