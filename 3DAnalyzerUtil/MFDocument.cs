using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using IxMilia.Stl;

namespace _3DAnalyzerUtil {

    public class MFDocument {
        public static List<List<StlTriangle>> test (string filePath) {
            var content = get_model (filePath);
            if (content == null) return null;
            XmlDocument document = new XmlDocument ();
            document.LoadXml (content);
            List<List<StlTriangle>> allMeshes = new List<List<StlTriangle>> ();

            var model = document.DocumentElement;
            if (model != null && model.Name == "model") {
                var ressources = getChild (model, "resources");
                if (ressources != null) {
                    var objects = getChilds (ressources, "object");
                    foreach (var obj in objects) {
                        var mesh = getChild (obj, "mesh");
                        if (mesh != null) {
                            List<StlVertex> ver = new List<StlVertex> ();
                            var vertices = getChild (mesh, "vertices");
                            if (vertices != null) {
                                var vertexs = getChilds (vertices, "vertex");
                                foreach (var verchild in vertexs) {
                                    var newVertex = new StlVertex ();
                                    newVertex.X = float.Parse (verchild.Attributes["x"].Value, CultureInfo.InvariantCulture);
                                    newVertex.Y = float.Parse (verchild.Attributes["y"].Value, CultureInfo.InvariantCulture);
                                    newVertex.Z = float.Parse (verchild.Attributes["z"].Value, CultureInfo.InvariantCulture);
                                    ver.Add (newVertex);
                                }
                            }
                            var triangles = getChild (mesh, "triangles");
                            var Opti=new Opti();
                            if (triangles != null) {
                                var tris = getChilds (triangles, "triangle");
                                var faces = tris.Select (x => {
                                    var Face = new StlTriangle (new StlNormal(),new StlVertex(),new StlVertex(),new StlVertex());
                                    Face.Vertex1=ver.ElementAt (int.Parse (x.Attributes["v1"].Value));
                                    Face.Vertex2=ver.ElementAt (int.Parse (x.Attributes["v2"].Value));
                                    Face.Vertex3=ver.ElementAt (int.Parse (x.Attributes["v3"].Value));
                                    Face.Normal=Opti.FNormalFromVertices(new List<StlVertex>{Face.Vertex1,Face.Vertex2,Face.Vertex3});
                                    return Face;

                                }).ToList ();
                                allMeshes.Add (faces);

                            }
                        }
                    }
                }
            }
            return allMeshes;
        }
        static XmlNode getChild (XmlNode node, string name) {
            foreach (XmlNode child in node.ChildNodes) {
                if (child.Name == name) {
                    return child;
                }
            }
            return null;
        }

        static List<XmlNode> getChilds (XmlNode node, string name) {
            var list = new List<XmlNode> ();
            foreach (XmlNode child in node.ChildNodes) {
                if (child.Name == name) {
                    list.Add (child);
                }
            }
            return list;
        }
        static string get_model (string filename) {
            string extractPath = "tmp";
            if (!Directory.Exists (extractPath)) Directory.CreateDirectory (extractPath);
            else Directory.Delete (extractPath, true);
            ZipFile.ExtractToDirectory (filename, extractPath);
            string modelDir = extractPath + "/3D";
            string modelFile = null;
            string contents = null;
            if (Directory.Exists (modelDir)) {
                var files = Directory.GetFiles (modelDir);
                foreach (var file in files) {
                    if (file.EndsWith (".model")) {
                        modelFile = file;
                        break;
                    }
                }
                if (modelFile != null) {
                    contents = File.ReadAllText (modelFile);
                }

                Directory.Delete (extractPath, true);
            }

            return contents;
        }

    }
}