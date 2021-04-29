using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace Sidewalk_Social_Distancing_Plugin
{
    public class Sidewalk_Social_Distancing_PluginInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "SidewalkSocialDistancingPlugin";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return Sidewalk_Social_Distancing_Plugin.Properties.Resources.SSD_Icon_01;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "this is a sidewalk social distance analysis plugin developed by Nicolas Azel for a KPF Coding Challange";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("af73aba0-1c91-4eb9-bf1c-8d2b5db786da");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "Computing Urbanism";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "Nicolas Azel";
            }
        }
    }
}
