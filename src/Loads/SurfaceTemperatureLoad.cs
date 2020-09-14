using System.Xml.Serialization;
using System.Collections.Generic;

#region dynamo
using Autodesk.DesignScript.Runtime;
#endregion

namespace FemDesign.Loads
{
    [IsVisibleInDynamoLibrary(false)]
    [System.Serializable]
    public class SurfaceTemperatureLoad: LoadBase
    {
        [XmlElement("region", Order=1)]
        public Geometry.Region Region { get; set; }
        [XmlElement("temperature", Order=2)]
        public List<TopBotLocationValue> _topBotLocVal;
        [XmlIgnore]
        public List<TopBotLocationValue> TopBotLocVal
        {
            get
            {
                return this._topBotLocVal;
            }
            set
            {
                if (value.Count == 1 || value.Count == 3)
                {
                    this._topBotLocVal = value;
                }
                else
                {
                    throw new System.ArgumentException($"Length of list is: {value.Count}, expected 1 or 3");
                }
            }
        }

        /// <summary>
        /// Parameterless constructor for serialization
        /// </summary>
        private SurfaceTemperatureLoad()
        {

        }

        /// <summary>
        /// Construct a surface temperature load by region and temperature location values (top/bottom)
        /// </summary>
        /// <param name="region">Region</param>
        /// <param name="tempLocValue">List of top bottom location value. List should have 1 or 3 elements.></param>
        /// <param name="loadCase">LoadCase.</param>
        /// <param name="comment">Comment.</param>
        public SurfaceTemperatureLoad(Geometry.Region region, List<TopBotLocationValue> tempLocValue, LoadCase loadCase, string comment)
        {
            this.EntityCreated();
            this.Region = region;
            this.TopBotLocVal = tempLocValue;
            this.LoadCase = loadCase.Guid;
            this.Comment = comment;
        }
        
        /// <summary>
        /// Construct surface temperature load by region, top value and bottom value.
        /// </summary>
        /// <param name="region">Region</param>
        /// <param name="topVal">Top value, temperature in celsius</param>
        /// <param name="bottomVal">Bottom value, temperature in celsius</param>
        /// <param name="loadCase">LoadCase.</param>
        /// <param name="comment">Comment.</param>
        public SurfaceTemperatureLoad(Geometry.Region region, double topVal, double bottomVal, LoadCase loadCase, string comment)
        {
            this.EntityCreated();
            this.Region = region;
            this.TopBotLocVal = new List<TopBotLocationValue>{new TopBotLocationValue(region.CoordinateSystem.origin, topVal, bottomVal)};
            this.LoadCase = loadCase.Guid;
            this.Comment = comment;
        }

        #region dynamo
        /// <summary>
        /// Define a surface temperature load
        /// </summary>
        /// <param name="surface">Surface</param>
        /// <param name="tempLocValue">Top and bottom temperature location values. Should be 1 or 3 elements. Use one if uniform and 3 if variable.</param>
        /// <param name="loadCase">LoadCase.</param>
        /// <param name="comment">Comment.</param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(true)]
        public static SurfaceTemperatureLoad Define(Autodesk.DesignScript.Geometry.Surface surface, List<TopBotLocationValue> tempLocValue, LoadCase loadCase, string comment = "")
        {
            // convert geometry
            Geometry.Region region = Geometry.Region.FromDynamo(surface);

            // return
            return new SurfaceTemperatureLoad(region, tempLocValue, loadCase, comment);
        }
        #endregion
    }
}