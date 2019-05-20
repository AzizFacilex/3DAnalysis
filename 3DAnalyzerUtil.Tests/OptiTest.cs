using STLReadWrite;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
namespace _3DAnalyzerUtil.Tests
{
    public class OptiTest
    {
        Opti Optivar = new Opti();
        [Fact]
        public void NormalFromVerticesTest_ShouldGetNormal()
        {
            Normal Shouldbe = new Normal (-10,-3,7);
            Vertex Vertex0 = new Vertex(0,0,0);
            Vertex Vertex1 = new Vertex(1,-1,1);
            Vertex Vertex2 = new Vertex(4,3,7);
            IList<Vertex> VerticesList = new List<Vertex> { Vertex0, Vertex1, Vertex2 };
            Normal Normalvector = Optivar.NormalFromVertices(VerticesList);
            Assert.Equal(Normalvector, Shouldbe);
        }//Finished
        [Fact]
        public void FNormalFromVerticesTest_ShouldGetNormal()
        {
            
            Normal Shouldbe = new Normal((float) -0.7955572841757303, (float) -0.2386671852527191, (float)0.5568900989230112);
            Vertex Vertex0 = new Vertex(0, 0, 0);
            Vertex Vertex1 = new Vertex(1, -1, 1);
            Vertex Vertex2 = new Vertex(4, 3, 7);
            IList<Vertex> VerticesList = new List<Vertex> { Vertex0, Vertex1, Vertex2 };
            Normal Normalvector = Optivar.FNormalFromVertices(VerticesList);
            Shouldbe.X =(float) Math.Round(Shouldbe.X, 5, MidpointRounding.AwayFromZero);
            Shouldbe.Y = (float)Math.Round(Shouldbe.Y, 5, MidpointRounding.AwayFromZero);
            Shouldbe.Z = (float)Math.Round(Shouldbe.Z, 5, MidpointRounding.AwayFromZero);
            Normalvector.X = (float)Math.Round(Normalvector.X, 5, MidpointRounding.AwayFromZero);
            Normalvector.Y = (float)Math.Round(Normalvector.Y, 5, MidpointRounding.AwayFromZero);
            Normalvector.Z = (float)Math.Round(Normalvector.Z, 5, MidpointRounding.AwayFromZero);
            
            Assert.Equal(Normalvector, Shouldbe);
        }//No
        [Fact]
        public void NormalValueFromVertex_ShouldGetValue()
        {
            double Value = 5;
            Vertex Vertex0 = new Vertex(0, 0, 0);
            Vertex Vertex1 = new Vertex(1, -1, 1);
            Vertex Vertex2 = new Vertex(4, 3, 7);
            IList<Vertex> VerticesList = new List<Vertex> { Vertex0, Vertex1, Vertex2 };
            var result = Optivar.NormalValueFromVertex(VerticesList);
            Assert.Equal(result, Value);

        }
        public void SubstractArraysTest_Shouldsubstract()
        {

        }
        public void PreprocessTest_ShouldgivevalidMesh()
        {

        }
        public void RotateMeshTest_ShouldrotateMesh()
        {

        }
        public void MultiplyByMatrixTest_ShouldRotateByMatrix()
        {

        }
        public void AreaZinateTest_ShouldGiveValidORientations()
        {

        }
    }

}