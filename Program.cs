using System;
using System.Threading.Tasks;
using FacebookVideosDownloader.Core.Entities;

namespace FacebookVideosDownloader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("FACEBOOK VIDEO DOWNLOADER BY: Lucas00012");

            Console.Write("Please insert the video url that you want: ");
            var facebookPostUrl = Console.ReadLine();

            Console.Write("Please insert the output directory that you want: ");
            var outputDirectory = Console.ReadLine();

            try
            {
                var facebookVideoDownloader = new FacebookVideoDownloader();
                await facebookVideoDownloader.Download(facebookPostUrl, outputDirectory);

                Console.Clear();
                Console.WriteLine("DOWNLOAD COMPLETED!");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}