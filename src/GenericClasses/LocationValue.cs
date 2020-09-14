// https://strusoft.com/
using System.Xml.Serialization;

#region dynamo
using Autodesk.DesignScript.Runtime;
#endregion

namespace FemDesign
{
    /// <summary>
    /// location_value_type
    /// </summary>
    [System.Serializable]
    [IsVisibleInDynamoLibrary(false)]
    public class LocationValue: Geometry.FdPoint3d
    {
        /// <summary>
        /// Value.
        /// </summary>
        [XmlAttribute("val")]
        public double _value;
        [XmlIgnore]
        public double Value
        {
            get { return this._value; }
            set { this._value = RestrictedDouble.AbsMax_1e20(value); }
        }

        /// <summary>
        /// Parameterless constructor for serialization.
        /// </summary>
        internal LocationValue()
        {

        }

        /// <summary>
        /// Convert coordinates of LocationValue to FdPoint.
        /// </summary>
        internal Geometry.FdPoint3d GetFdPoint()
        {
            return new Geometry.FdPoint3d(this.X, this.Y, this.Z);
        }
    }
}