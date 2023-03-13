# 3DAnalysis - Algorithm for Optimizing and Comparing 3D Models Orientations

## Introduction

3DAnalysis is a powerful algorithm designed to optimize and compare 3D models orientations. It can analyze various 3D files in Ascii and Binary STL format, providing extensive feedback on the printability of a 3D model, optimizing overhangs, and recommending the best orientation for printing. The algorithm saves rotation logs in XML format to assist with future analysis and modifications.

## Features

    Supports Ascii and Binary STL Files
    File rotation logging in XML Format
    Optimizes overhangs for better print quality
    Checks printability of a 3D model and recommends orientation
    Provides detailed feedback on model orientation and printing failures

## Future Improvements

Our team is working on adding support for additional file types and implementing a failure check to identify issues before rotating the STL file.
How to Use

Using 3DAnalysis is simple and straightforward. Follow the steps below to optimize your 3D models:

To rotate a file and save it, use (-i and -o options) :

    dotnet 3DAnalyzer.dll -i PATH/TO/toRotate.stl -o PATH/TO/rotated.stl

To check an orientation and receive feedback, use (-i and -c options) :

    dotnet 3DAnalyzer.dll -i PATH/TO/toCheck.stl -c

To rotate all files present in a folder, use (--input-folder and --output-folder options) :

    dotnet 3DAnalyzer.dll --input-folder PATH/TO/folder/ --output-folder PATH/TO/RotatedFolder

## Conclusion

3DAnalysis is a valuable tool for 3D modeling enthusiasts who seek to optimize and compare their models' orientations for better print quality. With its advanced algorithms and detailed feedback, it is an essential tool in the arsenal of any 3D printing enthusiast.

License

This project is licensed under the GNU General Public License v3.0 - see the LICENSE.md file for details.
