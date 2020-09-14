// https://strusoft.com/

using System.Xml.Serialization;
#region dynamo
using Autodesk.DesignScript.Runtime;
#endregion

namespace FemDesign.Loads
{
    /// <summary>
    /// load_case (child of load_combination_type)
    /// </summary>
    [System.Serializable]
    [IsVisibleInDynamoLibrary(false)]
    public class ModelLoadCase
    {
        [XmlAttribute("guid")]
        public System.Guid Guid { get; set; } // common_load_case --> guidtype indexed_guid
        [XmlAttribute("gamma")]
        public double Gamma { get; set; } // double
        public ModelLoadCase()
        {
            // parameterless constructor for serialization
        }

        /// <summary>
        /// Public constructor.
        /// </summary>
        /// <param name="guid">LoadCase guid reference.</param>
        /// <param name="gamma">Gamma value.</param>
        public ModelLoadCase(System.Guid guid, double gamma)
        {
            this.Guid = guid;
            this.Gamma = gamma;
        }     
    }
}