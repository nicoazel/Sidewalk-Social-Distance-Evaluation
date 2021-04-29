using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

using Grasshopper.Kernel.Types;

namespace Sidewalk_Social_Distancing_Plugin
{
    public class SidewalkWidths : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public SidewalkWidths()
          : base("SidewalkWidths", "plyWidth",
              "This component uses a polygon centerline and edges to calculate width of the polygon at equal intervals along it's center.",
             "SocialDistancingKPF", "Sidewalk Analysis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddLineParameter("Centerline", "cl", "Centerlines of polygon as list of lines per polygon", GH_ParamAccess.list);
            pManager.AddCurveParameter("Edges", "e", "edges of polygon as list of polylines", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("SizeLines", "l", "Lines from Polygon Center to Edge", GH_ParamAccess.list);
            pManager.AddNumberParameter("Widths", "w", "width of sidewalk at each sample line", GH_ParamAccess.list);
            pManager.AddPointParameter("SamplePoints", "pt", "center point of each centerline segment. Sidewalk width is calculated at this points.", GH_ParamAccess.list);
            pManager.AddCurveParameter("SampleLines", "sl", "Lines from Edge to Edge", GH_ParamAccess.list);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Input variable declaration 
            List<Line> CenterLine = new List<Line>();
            List<GH_Curve> Edges = new List<GH_Curve>();

            //Input Variable Retrieval
            if (!DA.GetDataList(0, CenterLine)) return;
            if (!DA.GetDataList(1, Edges)) return;


            ///Get Center Point of CenterLines
            List<Point3d> cntrPts = new List<Point3d>();
            foreach (Line ln in CenterLine)
            {
                Point3d thispt = ln.PointAt(0.5);
                cntrPts.Add(thispt);
            }


            //Convert to Nurvs curves to access methods
            List<NurbsCurve> checkedEdges = new List<NurbsCurve>();
            foreach (GH_Curve e in Edges)
            {
                Curve rhinoCurve = null;
                GH_Convert.ToCurve(e, ref rhinoCurve, 0);
                checkedEdges.Add(rhinoCurve.ToNurbsCurve());
            }

            //output width and line
            List<Line> widthLines = new List<Line>();
            List<Double> widthLengths = new List<Double>();
            List<Curve> joinedCurves = new List<Curve>();

            //Per line center - Find Closest Point to Each Edge
            foreach (Point3d pnt in cntrPts)
            {
                List<Curve> theseCurves = new List<Curve>();
                foreach (NurbsCurve c in checkedEdges)
                {
                    Double param;
                    c.ClosestPoint(pnt, out param, 100);
                    Point3d closestPoint = c.PointAt(param);
                    Line ln = new Line(pnt, closestPoint);
                    theseCurves.Add(ln.ToNurbsCurve());
                    widthLines.Add(ln);
                    widthLengths.Add(ln.Length);
                }
                joinedCurves.AddRange(Rhino.Geometry.Curve.JoinCurves(theseCurves));
            }

            //outputs
            DA.SetDataList(0, widthLines);//SizeLines
            DA.SetDataList(1, widthLengths);//Widths
            DA.SetDataList(2, cntrPts);//SamplePoints
            DA.SetDataList(3, joinedCurves);//SamplePoints

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Sidewalk_Social_Distancing_Plugin.Properties.Resources.SSD_Icon_06;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("f4b57f57-f54a-453c-b449-3f4f553890df"); }
        }
    }
}