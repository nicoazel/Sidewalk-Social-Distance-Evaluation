# Sidewalk Social Distance Evaluation
 A grasshopper workflow for evaluating social distancing on sidewalks using GIS Data and Grasshopper.

![HeroImage](https://i.imgur.com/Fmyjm7p.jpg)

## Repository Organization
<ol>
<li><strong>Workflow Demo</strong></li>
<ul>Rhino and Grasshopper Example Analysis Files. These Grasshopper Files Demonstrate the workflow</ul>
<li><strong>Sidewalk_Social_Distancing_Plugin</strong></li>
<ul>This folder contains the plugin C# files and Visual Studio project</ul>
<li><strong>Mapping Project</strong></li>
<ul>Holds an ArcGIS Pro Mapping Project with reference GIS Data and Model Data used within the Analysis. Unfortunately source data is too large for github so links to original files can be found in Section 4</ul>
</ol>

-----------
-----------
# 1. Workflow Overview
### Assumptions
This tool/workflow is developed for the evaluation of specific sidewalks in support of social distancing interventions. End Users are interested in several blocks at a time and these tools support design development.
### Analysis
The analysis focuses on questions of social distancing on sidewalks with the understanding that there is generally low covid-19 transmission risk outdoors, but risk rises when individuals spend time in close proximity. In the context of the city sidewalk there are two conditions that are likely to increase risk, areas where there is not sufficient space to spread out, and busy areas with greater pedestrian traffic or zones that gather stationary occupants.


<center><a href="https://imgur.com/enYw1Or"><img src="https://i.imgur.com/enYw1Or.jpg" title="source: imgur.com" width="400"/></a></center>

The analysis deals with each of these through consideration of sidewalk width as an indicator for pedestrians ability to keep distanced, and considering adjacent activates as proxies for how active a stretch of sidewalk is likely to be at a given time.

## Workflow Steps
1) Set up work space based on NYC Planning's 3d rhino model for your study area.
2) Create a study boundary and use the Setup Script to prep and import site GIS data.
3) Use the Analysis Script and Plugin to evaluate sidewalk conditions.
4) Make Changes and modifications to the sidewalk to adjacent uses
5) Compare Results
6) Iterate


-----------
-----------
# 2. Example Guide
The following describes the workflow steps. Note that the workflow makes use of the following grasshopper plugins: [HumanUI](https://www.food4rhino.com/app/human-ui), [BearGIS](https://nicoazel.github.io/BearGIS/), [Urbano](https://www.urbano.io/), [Elfront](https://www.food4rhino.com/app/elefront), & [MetaHopper](https://www.food4rhino.com/app/metahopper). github size limitations prevent source gis data for NYC to be shared here. Original data can be found using the links in section 4.


### a) Open SocialDistance_Example_Workspace.3dm
  This model has had the following step (b) already completed. It contains a portion of the citywide rhino model, parcel and sidewalk gis layers imported for this study area, and a user generated boundary curve.

### b) Run __1_SocialDistance_Sidewalk_Setup.gh
  This script helps export a shp file for use in clipping down external gis data to the study site of focus, as well as import and bake city sidewalk and land use data. This has already been run in the demo, but you can check out the process and an additional [urbano.io](https://www.urbano.io/) route intensity model.

### c) Run __2_SocialDistance_Sidewalk_Analysis.gh
  This file contains an example of how to use the sidewalk analysis plugin in two examples. Example One classifies sidewalks for social distancing risk based on width and Example two uses land use data to estimate activity intensity.

![Interface Window](https://i.imgur.com/HoQc9kk.jpg)

-----------
-----------
# 3. Plugin Components

## Polygon Centerline
This component extracts the centerline and edges of polygons in order to evaluate sidewalk width. The centerline is based on a sampling of the input Brep defined by the 'DivdeDist' input.

<center>
<img src="https://i.imgur.com/p15j82q.jpg" alt="component3" width="200"/>
</center>

## Sidewalk Widths
This component extracts profile lines for the sidewalk width corresponding with each segment of the sidewalk centerline.

<center>
<img src="https://i.imgur.com/wFcZMH8.jpg" alt="component3" width="200"/>
</center>

## Visualize Analysis
This component extracts profile lines for the sidewalk width corresponding with each segment of the sidewalk centerline.

<center>
<img src="https://i.imgur.com/fdtzgUa.jpg" alt="component3" width="200"/>
</center>

-----------
-----------

# 4. Reference

## a) Data

### Planning NYC Gov
* [NYC Rhino Model](https://www1.nyc.gov/site/planning/data-maps/open-data/dwn-nyc-3d-model-download.page)**
* [NYC Parcel Data](https://www1.nyc.gov/site/planning/data-maps/open-data/dwn-pluto-mappluto.page)**

### Open Data NYC
* [NYC Planimetric Sidewalks](https://data.cityofnewyork.us/City-Government/Sidewalk/vfx9-tbb6)**
* [NYC Sidewalk Widths](https://opendata.cityofnewyork.us/projects/sidewalk-widths-nyc/) |  [Map](https://www.sidewalkwidths.nyc/#13/40.78568/-73.96364)  |  [github](https://github.com/meliharvey/sidewalkwidths-nyc)
* [Traffic Counts](https://data.cityofnewyork.us/Transportation/Traffic-Volume-Counts-2014-2019-/ertz-hr4r)

### GIS NY Gov
* [Streets](http://gis.ny.gov/gisdata/inventories/details.cfm?DSID=932)

### DOT NY Gov
* [Traffic Counts](https://www.dot.ny.gov/tdv)

### ArcGIS Online
* [Average Daily Traffic Count](https://www.arcgis.com/home/item.html?id=bd861b3b8fe44188920faca0749e0c00)

** denotes Use within workflow

## b) Tools & Precedent

### Grasshopper Plugins Used
* [Urbano](https://www.urbano.io/)
* [BearGIS](https://nicoazel.github.io/BearGIS/)
* [HumanUI](https://www.food4rhino.com/app/human-ui)
* [Elfront](https://www.food4rhino.com/app/elefront)
* [MetaHopper](https://www.food4rhino.com/app/metahopper).

### Code Examples
* [Polygon Center in Grasshopper](https://discourse.mcneel.com/t/extract-centreline-of-polylines/85133/22)


-----------
-----------

# 4. Reflection and Conclusions
Though developed as a proofs of concept, this workflow has the potent to provide usable insights with the addition of better informed weighting thresholds for covid-19 risk. All the risk weighting thresholds are currently placeholder for demonstration of the scripts functionality. In developing the components to the custom plugin, the Polygon Centerline and Visualize Analysis components have great potential for re-use in other applications. These are general use components that are particularly well suited for efficient sidewalk analysis but have the flexibility to work in any context that is either looking for a polygon centerline or to color a mesh based on spatial weighting. The later of which is quite common in spatial analysis. One significant value of this workflow is this ability to design the weighting system based on simple points, making it very adaptable and customizable.

As a larger workflow, the setup script currently looses efficacy by requiring the cropping of sidewalk data in gis prior to import. This cropping can be done in grasshopper but is time intensive. If sidewalk files were pre-processed to match the boundaries of the cities' rhino model divisions, this extra clipping step could be eliminated completely, making the workflow more accessible to none GIS users. Looking forward, if this work where to continue, I think next steps would be more robust integration of both land use and sidewalk width into a single index, and also incorporate urbano.io activity routing to influence sidewalk use intensity over time. If urbano where to be paired with Google Places API data or more robust datasets like StreetLight of SafeGraph, it would be possible to deliver a high resolution estimate of sidewalk use intensity.
