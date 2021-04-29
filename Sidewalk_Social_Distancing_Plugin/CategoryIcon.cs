using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sidewalk_Social_Distancing_Plugin
{
    public class CategoryIcon : Grasshopper.Kernel.GH_AssemblyPriority
    {
        public override Grasshopper.Kernel.GH_LoadingInstruction PriorityLoad()
        {
            Grasshopper.Instances.ComponentServer.AddCategoryIcon("SidewalkSocialDistancing", Sidewalk_Social_Distancing_Plugin.Properties.Resources.SSD_Icon_01);
            Grasshopper.Instances.ComponentServer.AddCategoryShortName("SidewalkDistancing", "ssd");
            Grasshopper.Instances.ComponentServer.AddCategorySymbolName("SSD", 'S');

            return Grasshopper.Kernel.GH_LoadingInstruction.Proceed;
        }
    }
}
