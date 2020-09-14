// https://strusoft.com/

using System.Collections.Generic;
using System.Xml.Serialization;
#region dynamo
using Autodesk.DesignScript.Runtime;
#endregion

namespace FemDesign.Supports
{
    /// <summary>
    /// supports
    /// </summary>
    [System.Serializable]
    [IsVisibleInDynamoLibrary(false)]
    public class Supports
    {
        [XmlElement("point_support", Order = 1)]
        public List<PointSupport> PointSupport = new List<PointSupport>(); // point_support_type
        [XmlElement("line_support", Order = 2)]
        public List<LineSupport> LineSupport = new List<LineSupport>(); // line_support_type
        [XmlElement("surface_support", Order = 3)] 
        public List<SurfaceSupport> SurfaceSupport = new List<SurfaceSupport>(); // surface_support

        internal List<object> ListSupports()
        {
            var objs = new List<object>();
            objs.AddRange(this.PointSupport);
            objs.AddRange(this.LineSupport);
            objs.AddRange(this.SurfaceSupport);
            return objs;
        }

    }
}