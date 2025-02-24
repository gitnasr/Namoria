using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client
{
    internal class Updater
    {
        private string TagVersion = "v1.0.0";
        private readonly string GithubRepo = "https://api.github.com/repos/gitnasr/Namoria/releases";
        private  readonly HttpClient client = new HttpClient();

        public async void CheckForUpdate()
        {
            try
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("request");

                HttpResponseMessage response = await client.GetAsync(GithubRepo);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(responseBody);

                string tagName = doc.RootElement[0].GetProperty("tag_name").GetString();
                string latestRelease = doc.RootElement[0].GetProperty("name").GetString();

                string latestReleaseURL = doc.RootElement[0].GetProperty("html_url").GetString();

                if (tagName != TagVersion)
                {
                    DialogResult dialogResult = MessageBox.Show($"New version available: {latestRelease}\nDo you want to update?", "Update Available", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (dialogResult == DialogResult.OK)
                    {
                       
                        Process.Start(new ProcessStartInfo(latestReleaseURL) { UseShellExecute = true });
                        Application.Exit();

                    }else
                    {
                        Application.Exit();
                    }
                  
                }
              
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();

            }

        }
        public Updater() { 
        
            
        
        }
    }
}
