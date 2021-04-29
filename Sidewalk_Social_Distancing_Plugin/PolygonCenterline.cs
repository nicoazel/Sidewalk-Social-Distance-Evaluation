using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Grasshopper.Kernel.Types;

using Grasshopper.Kernel.Geometry;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace Sidewalk_Social_Distancing_Plugin
{
    public class PolygonCenterline : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public PolygonCenterline()
          : base("PolygonCenterline", "PolyCntr",
              "This component provides the centerline of a polygon for width analysis. ",
              "SocialDistancingKPF", "Sidewalk Analysis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Surface", "srf", "Polygons to analyize. Inputs should be proivded as a graphted list of Breps", GH_ParamAccess.list);
            pManager.AddNumberParameter("DivideDist", "d", "Distance to sample centerline by. Note: Very small distances will result in excess centerlines. default = 5", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("CenterLine", "cl", "Centerlines of surfaces. If problematic segments are present, try increasing the divide distance.", GH_ParamAccess.list);
            pManager.AddCurveParameter("Edges", "e", "Edges of input polygon", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Input Variables
            List<Brep> srfs = new List<Brep>();
            double divideby = 5;
            //Input Variable Retrieval
            if (!DA.GetDataList(0, srfs)) return;
            if (!DA.GetData(1, ref divideby)) return;

            //Generate mesh for ray interesect
            List<Mesh> srf_mesh = SSD_Utilities.ReturnMesh(srfs);
            //DA.SetData(0, srf_mesh);

            //Generate Surface Edges
            List<GH_Curve> edges = SSD_Utilities.ReturnEdges(srfs);
            DA.SetDataList(1, edges);

            //Generate Edge Points
            List<Point3d> edgePts = SSD_Utilities.EdgePoints(edges, divideby);

            //Generate Centerlines
            List<Polyline> veronis = SSD_Utilities.VeroniEdges(edgePts);
            List<Line> polygonCentrs = SSD_Utilities.PolygonCenterlines(veronis, srf_mesh, srfs);
            DA.SetDataList(0, polygonCentrs);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Sidewalk_Social_Distancing_Plugin.Properties.Resources.SSD_Icon_02;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("ff72a090-1b18-4153-89ab-ec17ed86633f"); }
        }
    }
}
