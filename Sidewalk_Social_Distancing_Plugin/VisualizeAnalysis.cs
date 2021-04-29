using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sidewalk_Social_Distancing_Plugin
{
    public class VisualizeAnalysis : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public VisualizeAnalysis()
          : base("VisualizeAnalysis", "visWeights",
              "Maps weight points to mesh vertex colors based on weight locations, wight, and average of n number of closist points",
              "SocialDistancingKPF", "Sidewalk Analysis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "m", "Mesh to visualize wights on", GH_ParamAccess.item);
            pManager.AddPointParameter("WeightPoints", "wp", "location with analysis weighting", GH_ParamAccess.list);
            pManager.AddNumberParameter("Weights", "w", "Weights of each Weighting Point", GH_ParamAccess.list);
            pManager.AddNumberParameter("WeightCount", "wc", "number of closest weight points to consider (average)", GH_ParamAccess.item, 1);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "m", "Mesh to visualize wights on", GH_ParamAccess.item);
            pManager.AddNumberParameter("Weights", "w", "Weights of each Weighting Point", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Def variabls
            Mesh thisMesh = null;
            List<Point3d> wPts = new List<Point3d>();
            List<double> weights = new List<double>();
            double wCount = 1;
            //Grab Inputs
            if (!DA.GetData(0, ref thisMesh)) return;
            if (!DA.GetDataList(1, wPts)) return;
            if (!DA.GetDataList(2, weights)) return;
            if (!DA.GetData(3, ref wCount)) return;
            
            
            //Build output Weights List
            PointCloud weightCloud = new PointCloud(wPts);
            List<Double> outWeights = new List<Double>();
            
            //use point cloud if only need closest point
            if (wCount == 1)
            {
                foreach (Point3d thisVert in thisMesh.Vertices)
                {
                    outWeights.Add(weights[weightCloud.ClosestPoint(thisVert)]);
                }
            }
            //use dist to sort out close points if wcout is greater than one
            else
            {
                foreach (Point3d thisVert in thisMesh.Vertices)
                {
                    //Generate distances
                    List<double> distances = new List<double>();
                    foreach (Point3d cloudPoint in wPts)
                    {
                        distances.Add(thisVert.DistanceTo(cloudPoint));
                    }
                    //sort weight by distance
                    List<double> sortedWeights = (List<double>)weights.SortLike(distances);

                    //sum distance to points closest
                    IEnumerable<int> shortIndexList = Enumerable.Range(0, Convert.ToInt32(wCount));
                    List<double> shortList = new List<double>();
                    foreach (int i in shortIndexList)
                    {
                        shortList.Add(sortedWeights[i]);
                    }
                    //output average
                    outWeights.Add(System.Linq.Enumerable.Average(shortList));
                }
            }
            DA.SetData(0, thisMesh);
            DA.SetDataList(1, outWeights);
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
                return Sidewalk_Social_Distancing_Plugin.Properties.Resources.SSD_Icon_05;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("314fa35f-9676-41a7-ab53-b28ff37de5d0"); }
        }
    }
    public static class ICollectionExtensions
    {
        //complements of Gert Arnold:Stacks Overflow 
        public static IEnumerable<TSource> SortLike<TSource, TKey>(this ICollection<TSource> source,
                                            IEnumerable<TKey> sortOrder)
        {
            var cloned = sortOrder.ToArray();
            var sourceArr = source.ToArray();
            Array.Sort(cloned, sourceArr);
            return sourceArr.ToList();
        }
    }
}