using System;
using System.Xml.Serialization;
using System.IO;
namespace _3DAnalyzerUtil {
    public class Counter {
        public double[] Array { get; set; }
        public double Value { get; set; }
        public long Count { get { return this.Array.Length; } }
    }

    public class DetailedResult {
        public DetailedResult () { }
        public double[][] RotationMatrix { get; set; }
        public double Phi { get; set; }
        public double[] RotationAxis { get; set; }
        public Result AssociatedResult { get; set; }
        public override string ToString()
        {

            string toReturn=$"Alignment : [{Math.Round (this.AssociatedResult.Orientation[0],4)}, {Math.Round (this.AssociatedResult.Orientation[1],4)} ,{Math.Round (this.AssociatedResult.Orientation[2],4)}]\r\n";
            toReturn+= $"Rotation Axis : [{Math.Round (this.RotationAxis[0],4)}, {Math.Round (this.RotationAxis[1],4)} ,{Math.Round (this.RotationAxis[2],4)}]\r\n";
            toReturn+=$"Rotation Angle : {this.Phi} PI\r\n"+"\r\n";
            toReturn+=$"Unprintabilty : {Math.Round(this.AssociatedResult.Unprintablitiy,4)}\r\n";
            toReturn+=$"Overhang : {Math.Round(this.AssociatedResult.Overhang,4)}\r\n";
            toReturn+=$"Bottom : {Math.Round(this.AssociatedResult.Bottom,4)}\r\n";
            toReturn+=$"Contour : {Math.Round(this.AssociatedResult.Contour,4)}\r\n";
            toReturn+=$"Matrix : \r\n";
            var rowCount = this.RotationMatrix.GetLength (0);
            for (int row = 0; row < rowCount; row++) {
                for (int col = 0; col < 3; col++)
                toReturn+=String.Format ("{0}\t", Math.Round(this.RotationMatrix[row][col],4)) ;
                toReturn+="\r\n";
            }
            return toReturn;
        }
        public void Serialize(string fileName)
        {
            XmlSerializer serializer=new XmlSerializer(typeof(DetailedResult));
            var folder=Path.GetDirectoryName(fileName);
            var newFolder=Directory.CreateDirectory(folder+"/Log");
            var fName=Path.GetFileNameWithoutExtension(fileName);
            TextWriter tw = new StreamWriter(newFolder.FullName+"/"+fName+".xml");
            serializer.Serialize(tw,this);
        }

    } //More Detailed Results(rotation Matrix, rotation axis..)

    public class Result {
        public Result () { }
        public Result (double[] orientation, double bottom, double overhang, double contour, double unprintability) {
            this.Orientation = orientation;
            this.Bottom = bottom;
            this.Overhang = overhang;
            this.Contour = contour;
            this.Unprintablitiy = unprintability;
        }
        public Result (Result r, bool anti_orient) {
            this.Bottom = r.Bottom;
            this.Overhang = r.Overhang;
            this.Contour = r.Contour;
            this.Unprintablitiy = r.Unprintablitiy;
            this.Orientation = new double[] {-r.Orientation[0], -r.Orientation[1], -r.Orientation[2] };
        }


        public double[] Orientation { get;set; }
        public double Bottom { get; set;}
        public double Overhang { get; set;}
        public double Contour { get; set;}

        public double Unprintablitiy { get;set; }
    }
}