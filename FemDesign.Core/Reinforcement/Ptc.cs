using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using FemDesign.GenericClasses;
using FemDesign.Bars;

namespace FemDesign.Reinforcement
{
    public enum JackingSide
    {
        [Parseable("start", "Start", "START")]
        [XmlEnum("start")]
        Start,
        [Parseable("end", "End", "END")]
        [XmlEnum("end")]
        End,
        [Parseable("start_then_end", "start then end", "Start then end", "StartThenEnd", "START_THEN_END")]
        [XmlEnum("start_then_end")]
        StartThenEnd,
        [Parseable("end_then_start", "end then start", "End then start", "EndThenStart", "END_THEN_START")]
        [XmlEnum("end_then_start")]
        EndThenStart,
    }

    [System.Serializable]
    public partial class PtcLosses
    {
        [XmlAttribute("curvature_coefficient")]
        public double CurvatureCoefficient { get; set; }

        [XmlAttribute("wobble_coefficient")]
        public double WobbleCoefficient { get; set; }

        [XmlAttribute("anchorage_set_slip")]
        public double AnchorageSetSlip { get; set; }
        [XmlAttribute("elastic_shortening")]
        public double ElasticShortening { get; set; }

        [XmlAttribute("creep_stress")]
        public double CreepStress { get; set; }

        [XmlAttribute("shrinkage_stress")]
        public double ShrinkageStress { get; set; }

        [XmlAttribute("relaxation_stress")]
        public double RelaxationStress { get; set; }

        /// <summary>
        /// Private constructor for serialization
        /// </summary>
        private PtcLosses()
        {

        }

        public PtcLosses(double curvatureCoefficient, double wobbleCoefficient, double anchorageSetSlip, double elasticShortening, double creepStress, double shrinkageStress, double relaxationStress)
        {
            CurvatureCoefficient = curvatureCoefficient;
            WobbleCoefficient = wobbleCoefficient;
            AnchorageSetSlip = anchorageSetSlip;
            ElasticShortening = elasticShortening;
            CreepStress = creepStress;
            ShrinkageStress = shrinkageStress;
            RelaxationStress = relaxationStress;
        }
    }

    /// <summary>
    /// Defines the shape of a post-tensioned cable (PTC) element.
    /// </summary>
    [System.Serializable]
    public partial class PtcShapeType
    {
        [XmlElement("start_point", Order = 1)]
        public PtcShapeStart StartPoint { get; set; }

        [XmlElement("intermediate_point", Order = 2)]
        public PtcShapeInner[] IntermediatePoint { get; set; }

        [XmlElement("end_point", Order = 3)]
        public PtcShapeEnd EndPoint { get; set; }

        [XmlAttribute("top")]
        public double Top { get; set; }
        [XmlAttribute("bottom")]
        public double Bottom { get; set; }

        /// <summary>
        /// Private constructor for serialization
        /// </summary>
        private PtcShapeType()
        {

        }

        public PtcShapeType(PtcShapeStart start, PtcShapeEnd end, IEnumerable<PtcShapeInner> intermediates = null)
        {
            StartPoint = start;
            EndPoint = end;
            IntermediatePoint = intermediates.ToArray();
        }
    }

    [System.Serializable]
    public partial class PtcShapeStart
    {
        [XmlAttribute("z")]
        public double Z { get; set; }

        [XmlAttribute("tangent")]
        public double Tangent { get; set; }
    }

    [System.Serializable]
    public partial class PtcShapeInner
    {
        [XmlAttribute("pos")]
        public double Position { get; set; }
        [XmlAttribute("z")]
        public double Z { get; set; }
        [XmlAttribute("tangent")]
        public double Tangent { get; set; }
        [XmlAttribute("prior_inflection_pos")]
        public string _priorInflectionPosition;
        [XmlIgnore]
        public double? PriorInflectionPosition
        {
            get
            {
                return _priorInflectionPosition == null ? (double?)null : (double?)double.Parse(_priorInflectionPosition, System.Globalization.CultureInfo.InvariantCulture);
            }
            set
            {
                _priorInflectionPosition = value.HasValue ? ((double)value).ToString(System.Globalization.CultureInfo.InvariantCulture) : null; 
            }
        }
    }

    [System.Serializable]
    public partial class PtcShapeEnd
    {
        [XmlAttribute("z")]
        public double Z { get; set; }
        [XmlAttribute("tangent")]
        public double Tangent { get; set; }
        [XmlAttribute("prior_inflection_pos")]
        public string _priorInflectionPosition;
        [XmlIgnore]
        public double? PriorInflectionPosition
        {
            get
            {
                return _priorInflectionPosition == null ? (double?)null : (double?)double.Parse(_priorInflectionPosition, System.Globalization.CultureInfo.InvariantCulture);
            }
            set
            {
                _priorInflectionPosition = value.HasValue ? ((double)value).ToString(System.Globalization.CultureInfo.InvariantCulture) : null;
            }
        }
    }

    [System.Serializable]
    public partial class PtcManufacturingType
    {
        private double[] _positions;

        [XmlElement("positions", Order = 1)]
        public string Positions
        {
            get
            {
                string str = "";

                for (int idx = 0; idx < this._positions.Length; idx++)
                {
                    if (idx == 0)
                    {

                    }
                    else
                    {
                        str = str + " ";
                    }

                    str = str + _positions[idx].ToString(System.Globalization.CultureInfo.InvariantCulture);
                }

                return str;
            }
            set
            {

                string[] vals = value.Split(' ');
                double[] dbls = new double[vals.Length];

                for (int idx = 0; idx < vals.Length; idx++)
                {
                    dbls[idx] = double.Parse(vals[idx], System.Globalization.CultureInfo.InvariantCulture);
                }

                this._positions = dbls;
            }
        }

        [XmlAttribute("shift_x")]
        public double ShiftX { get; set; }

        [XmlAttribute("shift_z")]
        public double ShiftZ { get; set; }

        /// <summary>
        /// Private constructor for serialization
        /// </summary>
        private PtcManufacturingType()
        {

        }

        public PtcManufacturingType(List<double> positions, double shiftX, double shiftZ)
        {
            _positions = positions.ToArray();
            ShiftX = shiftX;
            ShiftZ = shiftZ;
        }
    }

    [System.Serializable]
    public partial class Ptc : EntityBase, IStructureElement
    {
        [XmlElement("start_point", Order = 1)]
        public Geometry.FdPoint3d StartPoint { get; set; }

        [XmlElement("end_point", Order = 2)]
        public Geometry.FdPoint3d EndPoint { get; set; }

        [XmlElement("local_z", Order = 3)]
        public Geometry.FdVector3d LocalZ { get; set; }

        [XmlElement("losses", Order = 4)]
        public PtcLosses Losses { get; set; }

        [XmlElement("shape_base_points", Order = 5)]
        public PtcShapeType ShapeBasePoints { get; set; }

        [XmlElement("manufacturing", Order = 6)]
        public PtcManufacturingType Manufacturing { get; set; }

        [XmlAttribute("base_object")]
        public Guid BaseObject { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }


        [XmlAttribute("strand_type")]
        public Guid _StrandType { get; set; }
        [XmlIgnore]
        public PtcStrandLibType StrandType { get; set; }

        [XmlAttribute("number_of_strands")]
        public int NumberOfStrands { get; set; }

        [XmlAttribute("jacking_side")]
        public JackingSide JackingSide { get; set; }

        [XmlAttribute("jacking_stress")]
        public double JackingStress { get; set; }

        private static int instances = 0;

        /// <summary>
        /// Private constructor for serialization
        /// </summary>
        private Ptc()
        {

        }

        /// <summary>
        /// Construct post-tension cable
        /// </summary>
        /// <param name="bar">Reference bar element</param>
        /// <param name="shape"></param>
        /// <param name="losses"></param>
        /// <param name="manufacturing"></param>
        /// <param name="strand"></param>
        /// <param name="numberOfStrands"></param>
        /// <param name="identifier"></param>
        public Ptc(Bars.Bar bar, PtcShapeType shape, PtcLosses losses, PtcManufacturingType manufacturing, PtcStrandLibType strand, JackingSide jackingSide, double jackingStress, int numberOfStrands = 3, string identifier = "PTC")
        {
            if (bar.BarPart.Edge.Type == "line")
            {
                var start = bar.BarPart.Edge.Points[0];
                var end = bar.BarPart.Edge.Points[1];
                Initialize(start, end, bar.BarPart.Guid, shape, losses, manufacturing, strand, jackingSide, jackingStress, numberOfStrands, identifier);
            }
            else
                throw new ArgumentException($"Bar must be of type line but got '{bar.BarPart.Edge.Type}'", "bar");
        }
        /// <summary>
        /// Construct post-tension cable
        /// </summary>
        /// <param name="slab">Reference slab element</param>
        /// <param name="line">Cable line</param>
        /// <param name="shape"></param>
        /// <param name="losses"></param>
        /// <param name="manufacturing"></param>
        /// <param name="strand"></param>
        /// <param name="numberOfStrands"></param>
        /// <param name="identifier"></param>
        public Ptc(Shells.Slab slab, Geometry.LineSegment line, PtcShapeType shape, PtcLosses losses, PtcManufacturingType manufacturing, PtcStrandLibType strand, JackingSide jackingSide, double jackingStress, int numberOfStrands = 3, string identifier = "PTC")
        {
            Initialize(line.StartPoint, line.EndPoint, slab.SlabPart.Guid, shape, losses, manufacturing, strand, jackingSide, jackingStress, numberOfStrands, identifier);
        }

        private void Initialize(Geometry.FdPoint3d start, Geometry.FdPoint3d end, Guid baseObject, PtcShapeType shape, PtcLosses losses, PtcManufacturingType manufacturing, PtcStrandLibType strand, JackingSide jackingSide, double jackingStress, int numberOfStrands, string identifier)
        {
            StartPoint = start;
            EndPoint = end;

            BaseObject = baseObject;
            StrandType = strand;
            _StrandType = strand.Guid;
            Name = $"{identifier}.{++instances}";

            Losses = losses;
            NumberOfStrands = numberOfStrands;
            JackingSide = jackingSide;
            JackingStress = jackingStress;
            ShapeBasePoints = shape;
            Manufacturing = manufacturing;

            EntityCreated();
        }
    }

    [System.Serializable]
    public partial class PtcStrandType
    {
        [XmlElement("predefined_type")]
        public List<PtcStrandLibType> PtcStrandLibTypes { get; set; } = new List<PtcStrandLibType>();
    }

    [System.Serializable]
    public partial class PtcStrandLibType : LibraryBase
    {
        [XmlElement("ptc_strand_data", Order = 1)]
        public PtcStrandData PtcStrandData { get; set; }

        /// <summary>
        /// Parameterless constructor for serialization
        /// </summary>
        private PtcStrandLibType()
        {

        }

        /// <summary>
        /// Create a custom PTC-strand.
        /// </summary>
        /// <param name="f_pk">f pk [N/mm2]</param>
        /// <param name="a_p">A p [mm2]</param>
        /// <param name="e_p">E p [N/mm2]</param>
        /// <param name="density">Rho [t/mm3]</param>
        /// <param name="relaxationClass">Relaxation class [1, 2, 3] </param>
        /// <param name="rho_1000">Rho 1000 [%]</param>
        public PtcStrandLibType(string name, double f_pk, double a_p, double e_p, double density, int relaxationClass, double rho_1000)
        {
            Name = name;
            PtcStrandData = new PtcStrandData(f_pk, a_p, e_p, density, relaxationClass, rho_1000);
            EntityCreated();
        }
    }

    /// <summary>
    /// Strand material/type data
    /// </summary>
    [System.Serializable]
    public partial class PtcStrandData : EntityBase
    {
        /// <summary>
        /// f pk
        /// </summary>
        [XmlAttribute("f_pk")]
        public double F_pk { get; set; }

        [XmlAttribute("A_p")]
        public double A_p { get; set; }

        [XmlAttribute("E_p")]
        public double E_p { get; set; }

        [XmlAttribute("density")]
        public double Density { get; set; }

        [XmlAttribute("relaxation_class")]
        public int RelaxationClass { get; set; }

        [XmlAttribute("Rho_1000")]
        public double Rho_1000 { get; set; }

        /// <summary>
        /// Parameterless constructor for serialization
        /// </summary>
        private PtcStrandData()
        {

        }

        /// <summary>
        /// Create a custom PTC-strand.
        /// </summary>
        /// <param name="f_pk">f pk [N/mm2]</param>
        /// <param name="a_p">A p [mm2]</param>
        /// <param name="e_p">E p [N/mm2]</param>
        /// <param name="density">Rho [t/mm3]</param>
        /// <param name="relaxationClass">Relaxation class [1, 2, 3] </param>
        /// <param name="rho_1000">Rho 1000 [%]</param>
        public PtcStrandData(double f_pk, double a_p, double e_p, double density, int relaxationClass, double rho_1000)
        {
            F_pk = f_pk;
            A_p = a_p;
            E_p = e_p;
            Density = density;
            RelaxationClass = relaxationClass;
            Rho_1000 = rho_1000;
        }
    }
}