using System.Reflection;
using System.Text.Json;

namespace Server
{

    class Categories
    {
        private static List<string> CategoriesList = new List<string>();
        public Categories()
        {
            FetchFiles();
        }
        private void FetchFiles()
        {
            string? ExEPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (ExEPath == null)
            {
                Console.WriteLine("Path is null");
                return;
            }
            string path = Path.Combine(ExEPath, "Categories");
            try
            {
                foreach (string file in Directory.GetFiles(path))
                {
                    string[] File = Path.GetFileName(file).Split('.');
                    CategoriesList.Add(File[0]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Access Denied :{ex.Message}");
            }
        }
        public string CategoriesAsJson()
        {
            string CategoriesAsJson = JsonSerializer.Serialize(CategoriesList);
            return CategoriesAsJson;
        }
        public static string[] GetWordsByCategory(string category)
        {
            string? exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (exePath == null)
            {
                throw new Exception("Executable path is null.");
            }

            string filePath = Path.Combine(exePath, "Categories", category + ".txt");

            string[] words = File.ReadAllLines(filePath);

            if (words.Length == 0)
            {
                throw new Exception("Category file is empty.");
            }

            return words;

        }
    }
}

