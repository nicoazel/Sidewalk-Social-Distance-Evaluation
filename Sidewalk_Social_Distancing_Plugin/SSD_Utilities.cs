using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using Grasshopper.Kernel.Geometry;

namespace Sidewalk_Social_Distancing_Plugin
{

    /// <summary>
    /// Registers all the output parameters for this component.
    /// </summary>
    class SSD_Utilities
    {


        /// <summary>
        /// returns a mesh from brep for use in MeshRay
        /// </summary>
        public static List<Mesh> ReturnMesh(List<Brep> thisSrfs)
        {
            //empty list
            List<Mesh> brep_meshes = new List<Mesh>();

            //each in sf list
            foreach (Brep b in thisSrfs)
            {
                //carrier mesh
                var brep_mesh = new Mesh();
                var meshes = Mesh.CreateFromBrep(b, MeshingParameters.QualityRenderMesh);
                //add each face to carrier
                foreach (var mesh in meshes)
                {
                    brep_mesh.Append(mesh);
                }
                //add carrier to list
                brep_meshes.Add(brep_mesh);
            }
            return brep_meshes;
        }

        /// <summary>
        /// returns edges of a brep surface
        /// </summary>
        public static List<GH_Curve> ReturnEdges(List<Brep> breps)
        {
            List<GH_Curve> surfaceEdges = new List<GH_Curve>();
            foreach (Brep srf in breps)
            {
                foreach (BrepLoop loop in srf.Loops)
                {
                    Rhino.Geometry.Curve thisLoop = loop.To3dCurve();
                    Grasshopper.Kernel.Types.GH_Curve thisLoopGH = null;
                    Grasshopper.Kernel.GH_Convert.ToGHCurve(thisLoop, 0, ref thisLoopGH);

                    surfaceEdges.Add(thisLoopGH);
                }
            }
            return surfaceEdges;
        }

        /// <summary>
        /// Return edge points derived from diving sidewalk edges
        /// </summary>
        public static List<Point3d> EdgePoints(List<GH_Curve> edges, Double dividebydist)
        {
            List<Point3d> edgePoints = new List<Point3d>();
            //Rhino.Geometry.Point3d[] edgePoints;
            foreach (GH_Curve crv in edges)
            {
                Curve rc = null;
                Grasshopper.Kernel.GH_Convert.ToCurve(crv, ref rc, 0);
                Rhino.Geometry.Point3d[] theseEdgePoints;
                rc.DivideByLength(dividebydist, false, out theseEdgePoints);

                foreach (Point3d thisPt in theseEdgePoints)
                {
                    edgePoints.Add(thisPt);
                }
            }
            return edgePoints;
        }


        /// <summary>
        /// Return polines of a veroni for use in identifying polygon centerline
        /// </summary>
        public static List<Polyline> VeroniEdges(List<Point3d> nodePts)
        {
            //Flowing Code Complements of Laurent Delrieu
            //https://discourse.mcneel.com/t/voronoi-c/91379/5
            //# Create a boundingbox and get its corners
            BoundingBox bb = new BoundingBox(nodePts);
            Vector3d d = bb.Diagonal;
            double dl = d.Length;
            double f = dl / 15;
            bb.Inflate(f, f, f);
            Point3d[] bbCorners = bb.GetCorners();
            //# Create a list of nodes
            Node2List nodes = new Node2List();
            foreach (Point3d p in nodePts)
            {
                Node2 n = new Node2(p.X, p.Y);
                nodes.Append(n);
            }
            //Create a list of outline nodes using the BB
            Node2List outline = new Node2List();
            foreach (Point3d p in bbCorners)
            {
                Node2 n = new Node2(p.X, p.Y);
                outline.Append(n);
            }
            //# Calculate the delaunay triangulation
            var delaunay = Grasshopper.Kernel.Geometry.Delaunay.Solver.Solve_Connectivity(nodes, 0.1, false);
            // # Calculate the voronoi diagram
            var voronoi = Grasshopper.Kernel.Geometry.Voronoi.Solver.Solve_Connectivity(nodes, delaunay, outline);
            //# Get polylines from the voronoi cells and return them to GH
            List<Polyline> polys = new List<Polyline>();
            foreach (var c in voronoi)
            {
                Polyline pl = c.ToPolyline();
                polys.Add(pl);
            }
            return polys;
        }


        /// <summary>
        /// Returns the centerline from the mesh and veroni lines
        /// </summary>
        public static List<Line> PolygonCenterlines(List<Polyline> veroni, List<Mesh> mesh, List<Brep> srf)
        {
            // 1 - extract endpoints
            //Rhino.Geometry.Curve[] segmnt = Rhino.Geometry.Curve.  veroni.
            List<Line> allSegmnts = new List<Line>();
            foreach (Polyline pl in veroni)
            {
                Line[] theseSegments;
                theseSegments = pl.GetSegments();
                //theseSegments = pl.GetSegments();
                foreach (Line seg in theseSegments)
                {
                    allSegmnts.Add(seg);
                }
            }

            List<Line> centerSegments = new List<Line>();
            // 2 - get distance to surface

            foreach (Line segment in allSegmnts)
            {
                //Get Endpoints
                NurbsCurve segNurb = segment.ToNurbsCurve();
                Point3d pt1 = new Point3d(segNurb.PointAtEnd.X, segNurb.PointAtEnd.Y, segNurb.PointAtEnd.Z - 1111);
                Point3d pt2 = new Point3d(segNurb.PointAtStart.X, segNurb.PointAtStart.Y, segNurb.PointAtStart.Z - 111);

                //Construct rays
                Rhino.Geometry.Vector3d vect = new Vector3d(0, 0, 100000000000);
                Ray3d r1 = new Ray3d(pt1, vect);
                Ray3d r2 = new Ray3d(pt2, vect);

                //get interesection distance
                int[] faceids;
                Double d1 = Rhino.Geometry.Intersect.Intersection.MeshRay(mesh[0], r1, out faceids);
                Double d2 = Rhino.Geometry.Intersect.Intersection.MeshRay(mesh[0], r2, out faceids);

                // 3 - sort out desired edges
                if (d1 + d2 < 0.01 && d1 + d2 > 0)
                {
                    centerSegments.Add(segment);
                    //Expansion Node: this could be a good point to drop any really small dead end segments
                }

            }
            return centerSegments;


        }


    }
}
