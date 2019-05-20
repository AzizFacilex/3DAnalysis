using System;
using System.Collections.Generic;
using System.Linq;
using IxMilia.Stl;
namespace _3DAnalyzerUtil {
    public class CustomFacet {
        public double[] NormalVec { get; set; }
        public double[] Vertex1 { get; set; }
        public double[] Vertex2 { get; set; }
        public double[] Vertex3 { get; set; }
        public double[] Appended1 { get; set; }
        public double[] Appended2 { get; set; }
    } //Mesh Class
    class ArrayComparer : IEqualityComparer<double[]> {
        public bool Equals (double[] x, double[] y) {
            for (int i = 0; i < 3; i++) {
                if (x[i] != y[i]) return false;
            }
            return true;
        }

        public int GetHashCode (double[] obj) {
            return string.Join (",", obj).GetHashCode ();
        }
    } //Compare Two arrays
    public class OptiActionEventsArgs {
        public string message { get; set; }
    }

    public class Opti : IDisposable {
        bool msg = false;
        public double FirstLayerHeight { get; set; } = 0.25;
        public double NegligeFaceSize { get; set; } = 0.1; //Neglige little facets
        public double AbsoluteValue { get; set; } = 100; //Unprintability param
        public double VectorTolerance { get; set; } = 0.0001;
        public double RelativeValue { get; set; } = 1; //Unprintability param
        public bool disposed = false; //To release unmanaged resources
        public double Alpha { get; set; } = 5; //The Minimum angle between orientations

        public event EventHandler<OptiActionEventsArgs> ActionOccured;
        public OptiActionEventsArgs args = new OptiActionEventsArgs ();
        public virtual void OnActionOccured (OptiActionEventsArgs myEvent) {
            EventHandler<OptiActionEventsArgs> handler = ActionOccured;
            if (handler != null) {
                handler (this, myEvent);
            }
        }
        public void Dispose () {
            Dispose (true);
            GC.SuppressFinalize (this);
        } //Dispose Method

        protected virtual void Dispose (bool disposing) {

            if (!disposed) {

                if (disposing) {
                    GC.Collect ();
                }

                disposed = true;
            }
        }
        public StlNormal NormalFromVertices (IList<StlVertex> vertices) {
            double[] v0 = new double[] {
                vertices[1].X - vertices[0].X,
                vertices[1].Y - vertices[0].Y,
                vertices[1].Z - vertices[0].Z
            };
            double[] v1 = new double[] {
                vertices[2].X - vertices[0].X,
                vertices[2].Y - vertices[0].Y,
                vertices[2].Z - vertices[0].Z
            };
            double x = v0[1] * v1[2] - v1[1] * v0[2];
            double y = -(v0[0] * v1[2] - v0[2] * v1[0]);
            double z = v0[0] * v1[1] - v1[0] * v0[1];
            StlNormal crossVector = new StlNormal ((float) x, (float) y, (float) z);
            return crossVector;
        } //Calculate the Normal Vector from the vertices
        public StlNormal FNormalFromVertices (IList<StlVertex> vertices) {
            double[] v0 = new double[] {
                vertices[1].X - vertices[0].X,
                vertices[1].Y - vertices[0].Y,
                vertices[1].Z - vertices[0].Z
            };
            double[] v1 = new double[] {
                vertices[2].X - vertices[0].X,
                vertices[2].Y - vertices[0].Y,
                vertices[2].Z - vertices[0].Z
            };
            double x = v0[1] * v1[2] - v1[1] * v0[2];
            double y = -(v0[0] * v1[2] - v0[2] * v1[0]);
            double z = v0[0] * v1[1] - v1[0] * v0[1];
            double[] crossVector = new double[] { x, y, z };
            var value = Math.Sqrt (crossVector[0] * crossVector[0] + crossVector[1] * crossVector[1] + crossVector[2] * crossVector[2]);
            return new StlNormal ((float) (crossVector[0] / value), (float) (crossVector[1] / value), (float) (crossVector[2] / value));
        } //Calculate the Norm of the Normal Vertex
        public double NormalValueFromVertex (IList<StlVertex> vertices) {
            var x = NormalFromVertices (vertices);
            return Math.Sqrt (x.X * x.X + x.Y * x.Y + x.Z * x.Z) / 2;

        } //Calculate the Value of the Normal Vertex
        public double[] SubstractArrays (double[] A, double[] B) {
            return new double[] { A[0] - B[0], A[1] - B[1], A[2] - B[2] };
        } //Method to calculate the substraction of arrays
        //finished
        public double[] AdditionArrays (double[] A) {
            return new double[] { A[0] + A[1] + A[2] };
        }
        public CustomFacet[] PreProcess (List<StlTriangle> facets) {
            var counter = new Counter ();
            counter.Array = new double[] { 0, 0, 1 };
            counter.Value = 0;

            var mesh = facets.Select (x => new CustomFacet () {
                NormalVec = new double[] { x.Normal.X, x.Normal.Y, x.Normal.Z },
                    Vertex1 = new double[] { x.Vertex1.X, x.Vertex1.Y, x.Vertex1.Z },
                    Vertex2 = new double[] { x.Vertex2.X, x.Vertex2.Y, x.Vertex2.Z },
                    Vertex3 = new double[] { x.Vertex3.X, x.Vertex3.Y, x.Vertex3.Z },
                    Appended1 = new double[] { x.Vertex1.Z, x.Vertex2.Z, x.Vertex3.Z },
                    Appended2 = new double[] {
                        NormalValueFromVertex (new List<StlVertex>{x.Vertex1,x.Vertex2,x.Vertex3}),
                            (new double[] { x.Vertex1.Z, x.Vertex2.Z, x.Vertex3.Z }).Max (),
                            (new double[] { x.Vertex1.Z, x.Vertex2.Z, x.Vertex3.Z }).Average ()
                    }
            }).Where (x => x.Appended2[0] > NegligeFaceSize).ToArray ();
            args.message = "Preprocessing Mesh finished";
            OnActionOccured (args); // Trigger ActionOccured with message
            args.message = null;
            return mesh;
        } //Transform the Facets to the custom facet Mesh
        //finished
        public IList<StlTriangle> RotateMesh (IList<StlTriangle> mesh, double[][] rotationmatrix) {

            var newVertices = mesh.Select (x => new List<StlVertex> {
                MultiplyByMatrix (x.Vertex1, rotationmatrix),
                MultiplyByMatrix (x.Vertex2, rotationmatrix),
                MultiplyByMatrix (x.Vertex3, rotationmatrix)
            });
            var rotatedCon = newVertices.Select (x => new StlTriangle (FNormalFromVertices(x),x.ElementAt(0),x.ElementAt(1),x.ElementAt(2)));
            args.message = "Rotate the mesh using matrix found";
            OnActionOccured (args); // Trigger or Rise ActionOccured with message
            args.message = null;
            return rotatedCon.ToList ();
        } //Rotate the mesh with the rotation matrix
        public StlVertex MultiplyByMatrix (StlVertex vertex, double[][] rotation) {
            /*| a11 a12 a13 |    | b1 |    | a11*b1 + a12*b2 + a13*b3 |
              | a21 a22 a23 | x | b2 | = | a21*b1 + a22*b2 + a23*b3 |
               | a31 a32 a33 |    | b3 |    | a31*b1 + a32*b2 + a33*b3 |  */
            double x = rotation[0][0] * vertex.X + rotation[0][1] * vertex.Y + rotation[0][2] * vertex.Z;
            double y = rotation[1][0] * vertex.X + rotation[1][1] * vertex.Y + rotation[1][2] * vertex.Z;
            double z = rotation[2][0] * vertex.X + rotation[2][1] * vertex.Y + rotation[2][2] * vertex.Z;
            return new StlVertex ((float) x, (float) y, (float) z);
        } //Multiplicate Vector with Matrix
        public List<Counter> AreaZinate (CustomFacet[] mesh, int top_n = 20) {
            // var grouper = mesh.OrderByDescending (x => x.Appended2[0])
            //     .GroupBy (x => x.NormalVec, new ArrayComparer ())
            //     .Take (top_n)
            //     .ToDictionary (x => x.Key, x => x.Select (y => y.Appended2[0]).Take (top_n));
            // var orientations = grouper.FirstOrDefault ();
            var dict = new Dictionary<string, double> ();
            var ArbitraryOrient = new Counter ();
            var groupByNormalVec = mesh.Select (x => new Counter () {
                Array = x.NormalVec,
                    Value = x.Appended2[0]
            }).Distinct ();
            var su = mesh.Select (x => x).Where (x => x.NormalVec[0] == 0 && x.NormalVec[1] == 0 && x.NormalVec[2] == -1).Select (x => x.Appended2[0]).ToList ().Sum ();
            foreach (var x in groupByNormalVec) {
                var key = String.Join (":", x.Array);
                if (dict.ContainsKey (key)) {
                    dict[key] += x.Value;
                } else {
                    dict.Add (key, x.Value);
                }
            }
            var acc = dict.Select (x => new Counter () {
                Array = Array.ConvertAll (x.Key.Split (':'), Double.Parse),
                    Value = x.Value
            }).OrderByDescending (x => x.Value).Take (20).ToList ();

            var New_acc = RemoDup (acc);
            New_acc.AddRange (AddSup (New_acc));
            // New_acc=RemoDup(New_acc);
            args.message = "Best alignments found";
            OnActionOccured (args); // Trigger or Rise ActionOccured with message
            args.message = null;
            return New_acc;
        } //Find the best 20 Orientations by finding the 20 biggest Normal vectors with same direction
        //Class to put in the orientations and the values
        public Result CalculateOverhang (CustomFacet[] mesh, double[] orient) {
            var angle = Math.Cos (120 * Math.PI / 180);
            var anti_orient = new double[] {-orient[0], -orient[1], -orient[2] };
            var min = mesh.Select (x => x.Appended1.Min ()).Min ();
            var max = mesh.Select (x => x.Appended1.Max ()).Max ();
            var bottoms = mesh.Select (x => x).Where (x => x.Appended2[1] < min + FirstLayerHeight);

            double bottom;
            if (bottoms.Count () > 0) {
                bottom = bottoms.Select (x => x.Appended2[0]).Sum ();
            } else {
                bottom = 0;
            }
            var overhangs = mesh.Select (x => x).Where (x => (ArrayProduct (x.NormalVec, orient) < angle && (x.Appended2[1] > min)));
            // 
            var plafond = overhangs.Select (x => x).Where (x => x.NormalVec.SequenceEqual (anti_orient)).Select (x => x.Appended2[0]).Sum ();
            //to extend
            //Incorrect
            var Overhang = overhangs.Select ((x) =>
                (ArrayProduct (new double[] {
                    (x.Vertex1[0] + x.Vertex2[0] + x.Vertex3[0]) / 3,
                    (x.Vertex1[1] + x.Vertex2[1] + x.Vertex3[1]) / 3,
                    (x.Vertex1[2] + x.Vertex2[2] + x.Vertex3[2]) / 3
                }, orient) - min) //heights
                *
                Math.Pow ((new double[] { 0.5, -ArrayProduct (x.NormalVec, orient) }).Max () - 0.5, 2)
            ).Sum ();
            // Overhang-=((0.2)*plafond);
            var contour = 4 * Math.Sqrt (bottom);
            //need to calculate contour and fix overhang
            var contours = mesh.Select (x => x).Where (x => x.Appended2[2] < min + FirstLayerHeight);

            if (contours.Count () > 0) {
                int[] conlentation = Enumerable.Range (0, contours.Count ()).ToArray ();
                var sortsc0 = mesh.Select (x => x.Appended1);
                var sortsc1 = mesh.Select (x => x.Appended1);
                var sorted0 = sortsc0.Select (x => x.Select ((y, i) => new KeyValuePair<double, double> (y, i)).OrderBy (y => y.Key).ToList ()).ToList ();
                var B = sorted0.Select (x => x.Select (y => y.Key)).ToList ();
                var idx = sorted0.Select (x => x.Select (y => y.Value).ToList ()).ToList ();
                var finalidx = idx.Select (x => x[0]).ToList ();
                var sorted1 = sortsc1.Select (x => x.Select ((y, i) => new KeyValuePair<double, double> (y, i)).OrderBy (y => y.Key).ToList ()).ToList ();
                var B1 = sorted1.Select (x => x.Select (y => y.Key)).ToList ();
                var idx1 = sorted1.Select (x => x.Select (y => y.Value).ToList ()).ToList ();
                var finalidx1 = idx1.Select (x => x[1]).ToList ();
                var con = contours.Select ((x, i) => {
                    if (finalidx.ElementAt (i) == 0 && finalidx1.ElementAt (i) == 0) {
                        return SubstractArrays (x.Vertex1, x.Vertex1);
                    } else if ((finalidx.ElementAt (i) == 0 && finalidx1.ElementAt (i) == 1)) {
                        return SubstractArrays (x.Vertex1, x.Vertex2);
                    } else if ((finalidx.ElementAt (i) == 0 && finalidx1.ElementAt (i) == 2)) {
                        return SubstractArrays (x.Vertex1, x.Vertex3);
                    } else if ((finalidx.ElementAt (i) == 1 && finalidx1.ElementAt (i) == 0)) {
                        return SubstractArrays (x.Vertex2, x.Vertex1);
                    } else if ((finalidx.ElementAt (i) == 1 && finalidx1.ElementAt (i) == 1)) {
                        return SubstractArrays (x.Vertex2, x.Vertex2);
                    } else if ((finalidx.ElementAt (i) == 1 && finalidx1.ElementAt (i) == 2)) {
                        return SubstractArrays (x.Vertex2, x.Vertex3);
                    } else if ((finalidx.ElementAt (i) == 2 && finalidx1.ElementAt (i) == 0)) {
                        return SubstractArrays (x.Vertex3, x.Vertex1);
                    } else if ((finalidx.ElementAt (i) == 2 && finalidx1.ElementAt (i) == 1)) {
                        return SubstractArrays (x.Vertex3, x.Vertex2);
                    } else if ((finalidx.ElementAt (i) == 2 && finalidx1.ElementAt (i) == 2)) {
                        return SubstractArrays (x.Vertex3, x.Vertex3);
                    } else return null;
                }).ToList ();
                var confinal = con.Select (x => ArrayPower (x, 2)).ToList ();
                contour = confinal.Select (x => Math.Sqrt (AdditionArrays (x).Sum ())).Sum ();
            } else {
                contour = 0;
            }
            var Unprintablitiy = (Overhang / 5) / AbsoluteValue + (Overhang / 5) / (1 + 0.5 * contour + bottom) / RelativeValue;
            // (overhang + 1) / (1 + CONTOUR_F * contour + bottom) / RELATIVE_F)

            if (msg == false) {
                args.message = "Printability parameters calculation finished";
                OnActionOccured (args); // Trigger or Rise ActionOccured with message
                args.message = null;
                msg = true;
            }
            return new Result (orient, bottom, Overhang, contour, Unprintablitiy);
        } //Calculates the Bottom area, the Overhang(the support Volume), the contour and the unprintability
        //finished
        public CustomFacet[] ProjectVertices (CustomFacet[] mesh, double[] orientation) {

            var newMesh = mesh.Select (x => new CustomFacet {
                NormalVec = x.NormalVec,
                    Vertex1 = x.Vertex1,
                    Vertex2 = x.Vertex2,
                    Vertex3 = x.Vertex3,
                    Appended1 = new double[] {
                        ArrayProduct (x.Vertex1, orientation),
                            ArrayProduct (x.Vertex2, orientation),
                            ArrayProduct (x.Vertex3, orientation)
                    },
                    Appended2 = new double[] {
                        x.Appended2[0],
                            (new double[] {
                                ArrayProduct (x.Vertex1, orientation),
                                    ArrayProduct (x.Vertex2, orientation),
                                    ArrayProduct (x.Vertex3, orientation)
                            }).Max (),
                            (new double[] {
                                ArrayProduct (x.Vertex1, orientation),
                                    ArrayProduct (x.Vertex2, orientation),
                                    ArrayProduct (x.Vertex3, orientation)
                            }).Average ()
                    }
            });

            return newMesh.ToArray ();
        }
        public DetailedResult GetEulerParams (Result result) {
            bool condition = (result.Orientation[0] < VectorTolerance) &&
                (result.Orientation[1] < VectorTolerance) &&
                (result.Orientation[2] + 1 < VectorTolerance);
            var condition1 = (result.Orientation[0] > -VectorTolerance) &&
                (result.Orientation[1] > -VectorTolerance) &&
                (result.Orientation[2] + 1 > -VectorTolerance);
            bool condition2 = (Math.Abs (result.Orientation[0]) < VectorTolerance) &&
                (Math.Abs (result.Orientation[1]) < VectorTolerance) &&
                (Math.Abs (result.Orientation[2] - 1) < VectorTolerance);
            var condition3 = (result.Orientation[0] > -VectorTolerance) &&
                (result.Orientation[1] > -VectorTolerance) &&
                (result.Orientation[2] - 1 > -VectorTolerance);
            double[] rotationAxis;
            double phi;
            if (condition && condition1) {
                rotationAxis = new double[] { 1, 0, 0 };
                phi = Math.PI;
            } else if (condition2 && condition3) {
                rotationAxis = new double[] { 1, 0, 0 };
                phi = 0;
            } else {
                phi = Math.PI - Math.Acos (-result.Orientation[2]);
                rotationAxis = new double[] {
                    result.Orientation[1], -result.Orientation[0],
                    0
                };
                var temp = new double[3];
                for (int i = 0; i < rotationAxis.Length; i++) {
                    temp[i] = rotationAxis[i] / Math.Sqrt (ArrayPower (rotationAxis, 2).Sum ());

                }
                rotationAxis = temp;

            }
            double[] temp0 = new double[] {
                rotationAxis[0] * rotationAxis[0] * (1 - Math.Cos (phi)) + Math.Cos (phi),
                rotationAxis[0] * rotationAxis[1] * (1 - Math.Cos (phi)) - rotationAxis[2] * Math.Sin (phi),
                rotationAxis[0] * rotationAxis[2] * (1 - Math.Cos (phi)) + rotationAxis[1] * Math.Sin (phi)

            };
            double[] temp1 = new double[] {
                rotationAxis[1] * rotationAxis[0] * (1 - Math.Cos (phi)) + rotationAxis[2] * Math.Sin (phi),
                rotationAxis[1] * rotationAxis[1] * (1 - Math.Cos (phi)) + Math.Cos (phi),
                rotationAxis[1] * rotationAxis[2] * (1 - Math.Cos (phi)) - rotationAxis[0] * Math.Sin (phi)

            };
            double[] temp2 = new double[] {
                rotationAxis[2] * rotationAxis[0] * (1 - Math.Cos (phi)) - rotationAxis[1] * Math.Sin (phi),
                rotationAxis[2] * rotationAxis[1] * (1 - Math.Cos (phi)) + rotationAxis[0] * Math.Sin (phi),
                rotationAxis[2] * rotationAxis[0] * (1 - Math.Cos (phi)) + Math.Cos (phi)

            };
            args.message = "Rotation parameters calculation finished";
            OnActionOccured (args); // Trigger or Rise ActionOccured with message
            args.message = null;
            var tempA = new double[3][];
            tempA[0] = temp0;
            tempA[1] = temp1;
            tempA[2] = temp2;

            return new DetailedResult () {
                AssociatedResult = result,
                    RotationMatrix = tempA,
                    Phi = phi,
                    RotationAxis = rotationAxis
            };
        } //Get the rotation matrix

        public double[] ArrayPower (double[] v, double expo) {

            return v.Select (x => Math.Pow (x, expo)).ToArray ();
        } //Calculate the power of each value from an array
        public double ArrayProduct (double[] v0, double[] v1) {
            return v0[0] * v1[0] + v0[1] * v1[1] + v0[2] * v1[2];
        } //Calculate the product of two arrays
    
        List<Counter> AddSup (List<Counter> olds) {

            List<double[]> toReturn = new List<double[]> ();
            olds.Select (x => {
                toReturn.Add (new double[] {-x.Array[0], x.Array[1], -x.Array[2] });
                return false;
            });
            toReturn.Add (new double[] { 0, 0, -1 });
            toReturn.Add (new double[] { 0.70710678, 0, -0.70710678 });
            toReturn.Add (new double[] { 0, 0.70710678, -0.70710678 });
            toReturn.Add (new double[] {-0.70710678, 0, -0.70710678 });
            toReturn.Add (new double[] { 0, -0.70710678, -0.70710678 });
            toReturn.Add (new double[] { 1, 0, 0 });
            toReturn.Add (new double[] { 0.70710678, 0.70710678, 0 });
            toReturn.Add (new double[] { 0, 1, 0 });
            toReturn.Add (new double[] {-0.70710678, 0.70710678, 0 });
            toReturn.Add (new double[] {-1, 0, 0 });
            toReturn.Add (new double[] {-0.70710678, -0.70710678, 0 });
            toReturn.Add (new double[] { 0, -1, 0 });
            toReturn.Add (new double[] { 0.70710678, -0.70710678, 0 });
            toReturn.Add (new double[] { 0.70710678, 0, 0.70710678 });
            toReturn.Add (new double[] { 0, 0.70710678, 0.70710678 });
            toReturn.Add (new double[] {-0.70710678, 0, 0.70710678 });
            toReturn.Add (new double[] { 0, -0.70710678, 0.70710678 });
            toReturn.Add (new double[] { 0, 0, 1 });

            return toReturn.Select (x => new Counter () {
                Array = x,
                    Value = 1
            }).ToList ();
        } // Adds basic orientations
        List<Counter> RemoDup (List<Counter> orientations) {
            // Degrees to Radian
            double Rad_angle = Math.Sin (Alpha * Math.PI / 180);
            List<Counter> New_orientations = new List<Counter> ();
            for (int i = 0; i <= orientations.Count - 1; i++) {
                bool dup = false;
                double difference;
                double difference2;
                double difference3;
                for (int j = 0; j <= New_orientations.Count - 1; j++) {
                    difference = Math.Abs (-orientations[i].Array[0] - New_orientations[j].Array[0]);
                    difference2 = Math.Abs (-orientations[i].Array[1] - New_orientations[j].Array[1]);
                    difference3 = Math.Abs (-orientations[i].Array[2] - New_orientations[j].Array[2]);
                    if ((difference > 0.087) && (difference2 > 0.087) && (difference3 > 0.087)) {
                        dup = true;
                        break;
                    }
                }
                if (dup == false) {
                    New_orientations.Add (orientations[i]);
                }
            }
            return New_orientations;
        } //Remove the identical and almost equal orentations
    }
    //Class to put in the results(Orientation,Bottom,Overhang,Contour,Unprintability)
}