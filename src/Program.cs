using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LemmaSharp;

namespace Lemmator
{
    class Program
    {
        static void Main(string[] args)
        {
            LanguagePrebuilt language = args != null && args.Length > 0 ? getLanguage(args[0]) : getLanguage("default");

            ILemmatizer lemmatizer = new LemmatizerPrebuiltCompact(language);

            if (args.Length == 0 || args.Length == 1)
            {
                Console.WriteLine("Batch-processing all files contained in the subfolder 'lemma-source' into 'lemma-output'");

                string[] fileList = FileTraverser.getFileList();

                foreach (string file in fileList)
                {
                    processFile(file, lemmatizer);
                }
            }
            else if(args.Length == 2)
            {
                Logger.logError("Missing argument");
            }
            else if(args.Length == 3)
            {
                processFile(args[1], lemmatizer, args[2]);
            }
            else
            {
                Logger.logError("Argument count mismatch, expected max 3 received {0}", args.Length.ToString());
            }
        }

        /// <summary>
        /// Processes single file specified in the path parameter. If targetPath is specified, the result will be written in that location.
        /// If targetPath is not specified, result path will be built by replacing 'lemma-source' by 'lemma-output' in the original file path.
        /// </summary>
        /// <param name="path">file location</param>
        /// <param name="lemmatizer">Lemmatizer from lemmagen that should process the file</param>
        /// <param name="targetPath">target file location and name</param>
        private static void processFile(string path, ILemmatizer lemmatizer, string targetPath = null)
        {
            Console.WriteLine("Processing file {0}", new FileInfo(path).Name);

            string fileContent = readFile(path);

            if (fileContent == null)
                return;

            string[] wordList = prepareFileContent(fileContent);

            List<string> resultList = new List<string>();

            foreach(string word in wordList)
            {
                resultList.Add(processWord(word, lemmatizer));
            }

            writeOutputFile(resultList.ToArray(), path, targetPath);
        }

        /// <summary>
        /// Converts single word to lowercase and lemmatizes
        /// </summary>
        /// <param name="word"></param>
        /// <param name="lemmatizer"></param>
        /// <returns></returns>
        private static string processWord(string word, ILemmatizer lemmatizer)
        {
            return lemmatizer.Lemmatize(word.ToLower());
        }

        /// <summary>
        /// Opens a file and returns its contents
        /// </summary>
        /// <param name="path">File location</param>
        /// <returns></returns>
        private static string readFile(string path)
        {
           try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string text = reader.ReadToEnd();
                    return text;
                }
            }
            catch (Exception e)
            {
                Logger.logError("File {0} could not be read.", path);
                return null;
            }
        }

        /// <summary>
        /// Prepares the file content by removing spaces and punctuation and splitting it into a string array
        /// </summary>
        /// <param name="content">The content of the loaded file (or string to be lemmatized)</param>
        /// <returns></returns>
        private static string[] prepareFileContent(string content)
        {
            string[] result = content.Split(
                new char[] {' ', ' ', ',', '.', ')', '(', '!', '?', ';', '-', '–','"', '{', '}', '/', '\\'},
                StringSplitOptions.RemoveEmptyEntries
                );

            return result;
        }

        private static void writeOutputFile(string[] lemmatizedText, string sourcePath, string targetPath = null)
        {
            string resultPath = targetPath != null ? targetPath : sourcePath.Replace("lemma-source", "lemma-output");

            try
            {
                new FileInfo(resultPath).Directory.Create();
                System.IO.File.WriteAllLines(resultPath, lemmatizedText);

               Logger.logInfo("Written output file: {0}", resultPath);
            }
            catch (Exception e)
            {
               Logger.logError(e.Message);
            }
        }

        /// <summary>
        /// Retrieves the language parameter required for lemmatizer initialization based on command line parameter. 
        /// If no argument is passed, it defaults to Slovak
        /// </summary>
        /// <param name="languageParam"></param>
        /// <returns></returns>
        private static LanguagePrebuilt getLanguage(string languageParam)
        {
            LanguagePrebuilt language = LemmaSharp.LanguagePrebuilt.Slovak;

            string languageNameLog = "Slovak";

            switch (languageParam)
            {
                case "cs":
                case "cz":
                    language = LemmaSharp.LanguagePrebuilt.Czech;
                    languageNameLog = "Czech";
                    break;
            }

            Console.WriteLine("Lemmatisation language set to: {0}", languageNameLog);

            return language;
        }
    }
}
