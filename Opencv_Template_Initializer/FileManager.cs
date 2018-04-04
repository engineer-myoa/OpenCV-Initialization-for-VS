using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.IO.Compression;

namespace Opencv_Template_Initializer {
    class FileManager {


        public void compress(String src, String dest) {

            // "OpenCV_Template_VS.zip"
            if (isExistFile(dest)) {
                File.Delete(dest);
            }
            ZipFile.CreateFromDirectory(src, dest, CompressionLevel.Fastest, false);
        }

        public void decompress(String src, String dest) {
            // "templates.zip"

            ZipFile.ExtractToDirectory(src, dest);
        }

        public void cleanFolder(String path) {
            if (isExistFolder(path)) {
                Directory.Delete(path, true);
            }
        }

        public static bool isExistFolder(String path) {
            return Directory.Exists(path);

        }

        public static bool isExistFile(String path) {
            return File.Exists(path);
        }

        public static void deleteFile(String path) {
            if(FileManager.isExistFile(path)) {
                File.Delete(path);
            }
        }

        public bool copyFile(String src, String dest) {
            if(File.Exists(src)) {
                File.Copy(src, dest);
            }
            return false;
            
        }


    }
}
