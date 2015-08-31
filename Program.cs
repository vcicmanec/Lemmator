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
                Console.WriteLine("!!! Error !!! - Missing argument");
            }
            else if(args.Length == 3)
            {
                processFile(args[1], lemmatizer, args[2]);
            }
        }

        private static void processFile(string path, ILemmatizer lemmatizer, string targetPath = null)
        {
            Console.WriteLine("Processing file {0}", new FileInfo(path).Name);

            string[] wordList = prepareFileContent(readFile(path));

            List<string> resultList = new List<string>();

            foreach(string word in wordList)
            {
                resultList.Add(processWord(word, lemmatizer));
            }

            writeOutputFile(resultList.ToArray(), path, targetPath);
        }

        private static string processWord(string word, ILemmatizer lemmatizer)
        {
            return lemmatizer.Lemmatize(word.ToLower());
        }

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
                Console.WriteLine("File {0} could not be read.", path);
                return "";
            }
        }

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

                Console.WriteLine("Written output file: {0}", resultPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

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
