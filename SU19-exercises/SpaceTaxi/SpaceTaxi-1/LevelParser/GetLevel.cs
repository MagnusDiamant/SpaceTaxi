using System.IO;
using System.Reflection;

namespace SpaceTaxi_1.LevelParser {
    public class GetLevel {
        public string path;

        public GetLevel(string filename) {
            path = GetLevelFilePath(filename);
        }
        
        /// <summary>
        ///     Added from absalon, given by the TA's. Finds base path, and searches for the folder
        ///     called "Levels". Furthermore it finds the file that matches the filename if it exists
        /// </summary>
        /// <param name="filename"> The file that the method searches for</param>
        /// <returns> Returns the path in which the file can be found</returns>
        /// <exception cref="FileNotFoundException">
        ///     Throws an exception if the file does not
        ///     exist
        /// </exception>
        public string GetLevelFilePath(string filename) {
            // Find base path.
            var dir = new DirectoryInfo(Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location));

            while (dir.Name != "bin") {
                dir = dir.Parent;
            }

            dir = dir.Parent;

            // Find level file.
            var path = Path.Combine(dir.FullName, "Levels", filename);

            if (!File.Exists(path)) {
                throw new FileNotFoundException($"Error: The file \"{path}\" does not exist.");
            }

            return path;
        }
    }
}