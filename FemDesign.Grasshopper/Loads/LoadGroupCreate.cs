
using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;

namespace FemDesign.Grasshopper
{
    public class LoadGroupCreate : GH_Component
    {
        public LoadGroupCreate() : base("CreateLoadGroup", "CreateLoadGroup", "Creates a load group.", "FemDesign", "Loads")
        {

        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "Name", "Name of LoadGroup.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Type", "Type", "LoadGroup type.", GH_ParamAccess.item, 0);
            pManager.AddGenericParameter("LoadCase", "LoadCase", "LoadCase to include in LoadGroup. Single LoadCase or list of LoadCases.", GH_ParamAccess.list);
            pManager.AddGenericParameter("LoadCategory", "LoadCategory", "Psi values to apply when combining loads (not needed for permanent group).", GH_ParamAccess.item);
            pManager.AddNumberParameter("Gamma_d", "Gamma_d", "Partialkoefficient f�r s�kerhetsklass.", GH_ParamAccess.item);
            pManager.AddNumberParameter("UnfavourableSafetyFactor", "UnfavourableSafetyFactor", "Unfavourable safety factor to multiply load with.", GH_ParamAccess.item);
            pManager.AddNumberParameter("FavourableSafetyFactor", "FavourableSafetyFactor", "Favourable safety factor to multiply load with  (only needed for permanent loads).", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("UnfavourableSafetyFactorAccidental", "UnfavourableSafetyFactorAccidental", "Unfavourable safety factor to multiply load with (for accidental combinations).", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("FavourableSafetyFactorAccidental", "FavourableSafetyFactorAccidental", "Favourable safety factor to multiply load with (for accidental combinations).", GH_ParamAccess.item, 1);
            pManager.AddIntegerParameter("LoadCaseRelationship", "LoadCaseRelationship", "Specifies how the load cases are related", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Xi", "Xi", "Factor to multiply permanent load with (only needed for permanent loads).", GH_ParamAccess.item);
            pManager.AddIntegerParameter("PotentiallyLeadingAction", "PotentiallyLeadingAction", "True if the load cases in the group can be leading actions", GH_ParamAccess.item, 1);

            // Named values instead of integers
            Param_Integer type = pManager[1] as Param_Integer;
            type.AddNamedValue("permanent", 0);
            type.AddNamedValue("temporary", 1);

            Param_Integer loadCaseRelation = pManager[9] as Param_Integer;
            loadCaseRelation.AddNamedValue("entire", 0);
            loadCaseRelation.AddNamedValue("alternative", 1);

            Param_Integer potentiallyLeadingAction = pManager[11] as Param_Integer;
            potentiallyLeadingAction.AddNamedValue("False", 0);
            potentiallyLeadingAction.AddNamedValue("True", 1);

            // Set optional parameters
            List<int> optionalParameters = new List<int>() { 1, 3, 6, 7, 8, 11 };
            foreach (int parameter in optionalParameters)
                pManager[parameter].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("LoadGroup", "LoadGroup", "LoadGroup.", GH_ParamAccess.item);
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // get data
            string name = null;
            double gamma_d = 0, unfavourableSafetyFactor = 0, favourableSafetyFactor = 0, unfavourableSafetyFactorAccidental = 0, favourableSafetyFactorAccidental = 0, xi = 0.89;
            int loadCaseRelation = 0, type = 0, potentiallyLeadingAction = 1;
            List<FemDesign.Loads.LoadCase> loadCases = new List<FemDesign.Loads.LoadCase>();
            FemDesign.Loads.LoadCategory psi = new FemDesign.Loads.LoadCategory();

            if (!DA.GetData(0, ref name)) { return; }
            if (!DA.GetData(1, ref type))
            {
                // pass
            }
            if (!DA.GetDataList(2, loadCases)) { return; }
            if (!DA.GetData(3, ref psi))
            {
                // pass
            }
            if (!DA.GetData(4, ref gamma_d)) { return; }
            if (!DA.GetData(5, ref unfavourableSafetyFactor)) { return; }
            if (!DA.GetData(6, ref favourableSafetyFactor)) { return; }
            if (!DA.GetData(7, ref unfavourableSafetyFactorAccidental)) { return; }
            if (!DA.GetData(8, ref favourableSafetyFactorAccidental)) { return; }
            if (!DA.GetData(9, ref loadCaseRelation)) { return; }
            if (!DA.GetData(10, ref xi))
            {
                if(type == 0)
                    throw new System.ArgumentException("Must provide Xi value for permanent load group");                   
            }
            if(!DA.GetData(11, ref potentiallyLeadingAction))
            {
                if (type == 1)
                    throw new System.ArgumentException("Must specify if group is potentially leading action for temporary load group");
            }

            if (name == null)
                if(psi == null || type != 0){ return; }

            if (loadCaseRelation == 1 && type == 0)
                throw new System.ArgumentException("Alternative relationsship not yet implemented for permanent groups!");

            // Convert the load case relation to an enum
            Loads.ELoadGroupRelationship loadCaseRelationEnum = Loads.ELoadGroupRelationship.Entire;
            if(loadCaseRelation == 0)
                loadCaseRelationEnum = Loads.ELoadGroupRelationship.Entire;
            else if(loadCaseRelation == 1)
                loadCaseRelationEnum = Loads.ELoadGroupRelationship.Alternative;

            // Convert 0 and 1 to booleans
            Boolean potentiallyLeadingActionBool = true;
            if (potentiallyLeadingAction == 0)
                potentiallyLeadingActionBool = false;
                

            // Create load group object
            Loads.LoadGroup obj = new Loads.LoadGroup();
            if (type == 0)
                obj = new Loads.LoadGroup(name, Loads.ELoadGroupType.Permanent, loadCases, gamma_d, unfavourableSafetyFactor, favourableSafetyFactor, loadCaseRelationEnum, xi);
            else if (type == 1)
                obj = new Loads.LoadGroup(name, Loads.ELoadGroupType.Temporary, loadCases, psi, gamma_d, unfavourableSafetyFactor, favourableSafetyFactor, loadCaseRelationEnum, potentiallyLeadingActionBool);
            else
                throw new System.ArgumentException("Load group type not yet implemented");

            // return
            DA.SetData(0, obj);
        }
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return FemDesign.Properties.Resources.LoadGroup;
            }
        }
        public override Guid ComponentGuid
        {
            get { return new Guid("ebf804c1-91a6-40bb-adee-5a02a9e42f81"); }
        }
    }
}