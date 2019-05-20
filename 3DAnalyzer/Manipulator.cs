using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using _3DAnalyzerUtil;
using CommandLine;
namespace _3DAnalyzer {
    public class Options {
        //needs to change to required (Release)
        [Option ('i', "input", Required = false, HelpText = "Set the absolute file Path or file name within the current folder (ex. toRotate.stl)")]
        public string Input { get; set; } = "3DModels/models/ps_repaired.3mf";

        [Option ('o', "output", Required = false, HelpText = "Set the absolute file Path or file name within the current folder (ex. ./rotated.stl)")]
        public string Output { get; set; } = "3DModels/rotated/ps_repaired.stl";
        [Option ('v', "verbose", Required = false, HelpText = "Running task in verbose mode, default=true")]

        public bool Verbose { get; set; } = true;
        [Option ('c', "compare", Required = false, HelpText = "Comparing the file with the perfect one")]
        public bool Compare { get; set; } = false;
        [Option ("input-folder", Required = false, HelpText = "Run the Rotation on every Stl file within the folder Provided")]
        public string IntputFolder { get; set; } = "3DModels/models/"; //output
        [Option ("output-folder", Required = false, HelpText = "Every rotated Stl file will be saved in folder Provided")]
        public string OutputFolder { get; set; } = "3DModels/rotated/"; //output

    }
    public static class Manipulator {
        private static bool verboseMode = false;
        private static string inputPath = "";
        private static string outputPath = "";
        private static string inputFolder = "";
        private static string outputFolder = "";
        private static bool compare = false;
        public static void Init (string[] args) {

            Parser.Default.ParseArguments<Options> (args)
                .WithParsed<Options> (o => {
                    inputPath = o.Input;
                    outputPath = o.Output;
                    inputFolder = o.IntputFolder;
                    outputFolder = o.OutputFolder;
                    verboseMode = o.Verbose;
                    compare = o.Compare;
                    if (inputFolder != "" && outputFolder != "") {
                        rotateFolderFiles (inputFolder, outputFolder);
                    } else if ((inputPath != "") && (compare == true)) {
                        comparefunction (inputPath);
                    } else if (inputPath != "" && outputPath != "") {
                        rotate (inputPath, outputPath);
                    } else {
                        Console.WriteLine ("ERROR: Please provide available options or check the help");
                    }
                });
        }
        static void comparefunction (string inputPath) {
            Console.WriteLine ("Analysing the actual Position of the Object " + Path.GetFileName (inputPath));
            var fileHandler1 = new FileHandler ();
            Stopwatch Total = new Stopwatch ();
            Total.Start ();
            var facets1 = fileHandler1.LoadFile (inputPath);
            fileHandler1.ActionOccured += actionOccured;
            CustomFacet[] preproccessed1;
            using (var opti1 = new Opti ()) {
                opti1.ActionOccured += actionOccured;
                preproccessed1 = opti1.PreProcess (facets1);
                Result results1;
                double[] orient = { 0, 0, 1 };
                results1 = opti1.CalculateOverhang (preproccessed1, orient);
                Console.WriteLine ("Analysis Complete");
                ////////
                Console.WriteLine ("Looking for the Perfect Position of the file " + Path.GetFileName (inputPath));
                var fileHandler = new FileHandler ();

                var facets = fileHandler.LoadFile (inputPath);

                CustomFacet[] preproccessed;
                using (var opti = new Opti ()) {
                    opti.ActionOccured += actionOccured;
                    preproccessed = opti.PreProcess (facets);
                }

                List<Counter> accumulated;
                using (var opti = new Opti ()) {
                    opti.ActionOccured += actionOccured;
                    accumulated = opti.AreaZinate (preproccessed);
                }

                Result result;
                IEnumerable<Result> results;
                DetailedResult detailedResult;
                using (var opti = new Opti ()) {
                    opti.ActionOccured += actionOccured;
                    // results = accumulated.Select (x => opti.CalculateOverhang (opti.ProjectVertices (preproccessed, x.Array), x.Array)).OrderBy (x => ((x.Unprintablitiy + 1) * (x.Overhang + 1)) / ((x.Bottom + 1) * (x.Contour + 1))).ToList();
                    results = accumulated.Select (x => opti.CalculateOverhang (opti.ProjectVertices (preproccessed, x.Array), x.Array)).OrderBy (x => x.Unprintablitiy).ToList ();
                    result = results.FirstOrDefault ();

                }

                using (var opti = new Opti ()) {
                    opti.ActionOccured += actionOccured;
                    detailedResult = opti.GetEulerParams (result);
                }
                Console.WriteLine ("Use the following parameters to optimize the Print");
                Console.WriteLine (detailedResult);
                Console.WriteLine ($"Perfect position found in {Total.Elapsed.TotalSeconds} Seconds");
                Console.WriteLine (results1.Unprintablitiy);
                Console.WriteLine (result.Unprintablitiy);
                var Compared = (result.Unprintablitiy / results1.Unprintablitiy) * 100;
                Console.WriteLine ("The object Orientation is positioned {0} % to the perfect position ", Compared);
                GC.Collect ();
                Total.Stop ();
            }
        }
        static void rotateFolderFiles (string inputFolder, string outputFolder) {
            List<string> filePaths = FileHandler.getAllPathsInDirectory (inputFolder);
            Stopwatch s = new Stopwatch ();
            s.Start ();

            foreach (var path in filePaths) {
                var fileName = Path.GetFileName (path);
                rotate (path, Path.GetDirectoryName (outputFolder) + "/" + fileName);
                Console.WriteLine ("******************************************************");
            }
            Console.WriteLine ($"All Files rotated in {s.Elapsed.TotalSeconds} Seconds");
            return;
        }
        static void rotate (string inputPath, string outputPath) {
            // var test= MFDocument.test(inputPath);
            Console.WriteLine ("Working with file " + Path.GetFileName (inputPath));
            var fileHandler = new FileHandler ();
            Stopwatch Total = new Stopwatch ();
            Total.Start ();
            var facets = fileHandler.LoadFile (inputPath);
            // var facets = MFDocument.test(inputPath).FirstOrDefault();
            fileHandler.ActionOccured += actionOccured;
            CustomFacet[] preproccessed;
            using (var opti = new Opti ()) {
                opti.ActionOccured += actionOccured;
                preproccessed = opti.PreProcess (facets);
            }

            List<Counter> accumulated;
            using (var opti = new Opti ()) {
                opti.ActionOccured += actionOccured;
                accumulated = opti.AreaZinate (preproccessed);
            }

            Result result;
            IEnumerable<Result> results;
            DetailedResult detailedResult;
            using (var opti = new Opti ()) {
                opti.ActionOccured += actionOccured;
                // results = accumulated.Select (x => opti.CalculateOverhang (opti.ProjectVertices (preproccessed, x.Array), x.Array)).OrderBy (x => ((x.Unprintablitiy + 1) * (x.Overhang + 1)) / ((x.Bottom + 1) * (x.Contour + 1))).ToList();
                results = accumulated.Select (x => opti.CalculateOverhang (opti.ProjectVertices (preproccessed, x.Array), x.Array)).OrderBy (x => x.Unprintablitiy).ToList ();
                result = results.FirstOrDefault() ;

            }

            using (var opti = new Opti ()) {
                opti.ActionOccured += actionOccured;
                detailedResult = opti.GetEulerParams (result);
            }
            detailedResult.Serialize (outputPath); //Create log xml file
            Console.WriteLine ("-----------------------------------------------------");
            Console.WriteLine (detailedResult);

            IxMilia.Stl.StlTriangle[] rotated;
            using (var opti = new Opti ()) {
                opti.ActionOccured += actionOccured;
                rotated = opti.RotateMesh (facets, detailedResult.RotationMatrix).ToArray ();
            }
            fileHandler = new FileHandler ();
            fileHandler.WriteFIle (rotated, outputPath);
            fileHandler.ActionOccured += actionOccured;
            Console.WriteLine ($"Mesh rotated and writen successfully  in {Total.Elapsed.TotalSeconds} Seconds");
            GC.Collect ();
            Total.Stop ();
        }
        static void actionOccured (object sender, OptiActionEventsArgs myEvent) // ActionOccured EventHandler
        {
            if (verboseMode) {
                Console.WriteLine ("message : {0}", myEvent.message);
            }
            // Environment.Exit(0);
        }
    }

}