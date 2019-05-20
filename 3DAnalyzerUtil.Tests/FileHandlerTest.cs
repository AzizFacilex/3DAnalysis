using System;
using System.Collections.Generic;
using System.Text;
using STLReadWrite;
using System.IO;
using Xunit;
namespace _3DAnalyzerUtil.Tests
{
    public class FileHandlerTest

    {
        FileHandler test = new FileHandler();
        IList<Facet> facettest = new List<Facet>();
        [Theory]
        [InlineData(@"C:\Users\aziz\Desktop\3D Analysis\3DAnalyzerUtil.Tests\3DModels\box_min_support_and_zheight.stl")]
        [InlineData(@"C:\Users\aziz\Desktop\3D Analysis\3DAnalyzerUtil.Tests\3DModels\box_tested.stl")]
        [InlineData(@"C:\Users\aziz\Desktop\3D Analysis\3DAnalyzerUtil.Tests\3DModels\crank_wheel_min_support.stl")]
        [InlineData(@"C:\Users\aziz\Desktop\3D Analysis\3DAnalyzerUtil.Tests\3DModels\crank_wheel_min_zheight.stl")]
        [InlineData(@"C:\Users\aziz\Desktop\3D Analysis\3DAnalyzerUtil.Tests\3DModels\crank_wheel_tested.stl")]
        [InlineData(@"C:\Users\aziz\Desktop\3D Analysis\3DAnalyzerUtil.Tests\3DModels\fan_min_support_and_zheight.stl")]
        [InlineData(@"C:\Users\aziz\Desktop\3D Analysis\3DAnalyzerUtil.Tests\3DModels\fan_tested.stl")]
        [InlineData(@"C:\Users\aziz\Desktop\3D Analysis\3DAnalyzerUtil.Tests\3DModels\yoda_min_support.stl")]
        [InlineData(@"C:\Users\aziz\Desktop\3D Analysis\3DAnalyzerUtil.Tests\3DModels\yoda_tested.stl")]
        public void FilehandlerLoadFile_FileshouldLoad(string x)
        {
            facettest = test.LoadFile(x);
            Assert.NotEmpty(facettest);
        }
        [Theory]
        [InlineData(@"C:\Users\aziz\Desktop\3D Analysis\3DAnalyzerUtil.Tests\3DModels\box_min_support_and_zheight.stl")]
        [InlineData(@"C:\Users\aziz\Desktop\3D Analysis\3DAnalyzerUtil.Tests\3DModels\box_tested.stl")]
        [InlineData(@"C:\Users\aziz\Desktop\3D Analysis\3DAnalyzerUtil.Tests\3DModels\crank_wheel_min_support.stl")]
        [InlineData(@"C:\Users\aziz\Desktop\3D Analysis\3DAnalyzerUtil.Tests\3DModels\crank_wheel_min_zheight.stl")]
        [InlineData(@"C:\Users\aziz\Desktop\3D Analysis\3DAnalyzerUtil.Tests\3DModels\crank_wheel_tested.stl")]
        [InlineData(@"C:\Users\aziz\Desktop\3D Analysis\3DAnalyzerUtil.Tests\3DModels\fan_min_support_and_zheight.stl")]
        [InlineData(@"C:\Users\aziz\Desktop\3D Analysis\3DAnalyzerUtil.Tests\3DModels\fan_tested.stl")]
        [InlineData(@"C:\Users\aziz\Desktop\3D Analysis\3DAnalyzerUtil.Tests\3DModels\yoda_min_support.stl")]
        [InlineData(@"C:\Users\aziz\Desktop\3D Analysis\3DAnalyzerUtil.Tests\3DModels\yoda_tested.stl")]
        public bool FleHandlerWritebinary_Fileshouldcreate(string x)
        {
            facettest = test.LoadFile(x);
            test.WriteFIle(facettest, @"C:\Users\aziz\Desktop\x.stl");
            bool Exists = File.Exists(@"C:\Users\aziz\Desktop\x.stl");
            if (Exists == true)
            {
                File.Delete(@"C:\Users\aziz\Desktop\x.stl");
            }
            return Exists;
        }
    }//Finished
}
