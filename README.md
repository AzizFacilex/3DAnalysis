# 3DAnalysis
3DAnalysis is an algorithm for optimizing and comparing 3D models orientations.
Included in First release:
* Optimize overhangs
* Printablity check
* Check an orientation and give feedback

# Features
* Supports Ascii and Binary STL Files
* File rotation logging in XML Format

# To come:
* Add support for additional file types
* Check STL for failures before rotating
* ...

# How to use
* To rotate a file and save it use(-i and -o options) :
> dotnet 3DAnalyzer.dll -i PATH/TO/toRotate.stl -o PATH/TO/rotated.stl
* To Check an orientation use(-i and -c options) :
> dotnet 3DAnalyzer.dll -i PATH/TO/toCheck.stl -c
* To rotate all files present in a folder
> dotnet 3DAnalyzer.dll --input-folder PATH/TO/folder/
--output-folder PATH/TO/RotatedFolder


