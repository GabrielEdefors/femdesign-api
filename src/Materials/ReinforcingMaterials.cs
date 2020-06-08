// https://strusoft.com/

using System.Collections.Generic;
using System.Xml.Serialization;

namespace FemDesign.Materials
{
    /// <summary>
    /// reinforcing_materials.
    /// </summary>
    public class ReinforcingMaterials
    {
        [XmlElement("material", Order = 1)]
        public List<Material> material = new List<Material>(); // sequence: rfmaterial_type
    }
}