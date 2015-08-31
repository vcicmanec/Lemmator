using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Lemmator
{
    class FileTraverser
    {
        public static string[] getFileList()
        {
            string[] result = processDirectory(Directory.GetCurrentDirectory() + "\\lemma-source");

            Console.WriteLine("Found {0} .txt files in the current path", result.Length);

            return result;
        }

        private static string[] processDirectory(string path)
        {
            string[] dirs = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path, "*.txt");

            for(int i = 0; i < dirs.Length; i++)
            {

                files = files.Concat(processDirectory(dirs[i])).ToArray();
            }


            return files;
        }
    }
}
