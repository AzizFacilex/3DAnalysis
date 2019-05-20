using System;
using System.Collections.Generic;

using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using IxMilia.Stl;
namespace _3DAnalyzerUtil {
    public class FileHandler {
        public virtual void OnActionOccured (OptiActionEventsArgs myEvent) // Define Event Trigger Method
        {
            EventHandler<OptiActionEventsArgs> handler = ActionOccured;
            if (handler != null) {
                handler (this, myEvent);
            }
        }
        public event EventHandler<OptiActionEventsArgs> ActionOccured; // Add Event to FileHandler Class
        public OptiActionEventsArgs args = new OptiActionEventsArgs ();
        public List<StlTriangle> LoadFile (string path) {

            
            StlFile stlFile = null;

            using (FileStream fs = new FileStream (path, FileMode.Open)) {
                stlFile = StlFile.Load (fs);
            }

            return stlFile.Triangles.ToList ();
        }
        public void WriteFIle (IList<StlTriangle> facets, string path) {
            StlFile stlFile = new StlFile ();
            stlFile.SolidName = Path.GetFileNameWithoutExtension (path);
            stlFile.Triangles.AddRange (facets);
            using (FileStream fs = new FileStream (path, FileMode.Create)) {
                stlFile.Save (fs, false);
            }
        }
        
        static public List<string> getAllPathsInDirectory (string directoryPath) {
            var path = Path.GetDirectoryName (directoryPath);
            return Directory.GetFiles (path, "*.stl").ToList ();
        }

    }
}