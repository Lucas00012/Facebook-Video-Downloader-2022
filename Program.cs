using System;
using System.Threading.Tasks;
using FacebookVideosDownloader.Core.Entities;

namespace FacebookVideosDownloader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine(@"
███████╗ █████╗  ██████╗███████╗██████╗  ██████╗  ██████╗ ██╗  ██╗    ██╗   ██╗██╗██████╗ ███████╗ ██████╗ 
██╔════╝██╔══██╗██╔════╝██╔════╝██╔══██╗██╔═══██╗██╔═══██╗██║ ██╔╝    ██║   ██║██║██╔══██╗██╔════╝██╔═══██╗
█████╗  ███████║██║     █████╗  ██████╔╝██║   ██║██║   ██║█████╔╝     ██║   ██║██║██║  ██║█████╗  ██║   ██║
██╔══╝  ██╔══██║██║     ██╔══╝  ██╔══██╗██║   ██║██║   ██║██╔═██╗     ╚██╗ ██╔╝██║██║  ██║██╔══╝  ██║   ██║
██║     ██║  ██║╚██████╗███████╗██████╔╝╚██████╔╝╚██████╔╝██║  ██╗     ╚████╔╝ ██║██████╔╝███████╗╚██████╔╝
╚═╝     ╚═╝  ╚═╝ ╚═════╝╚══════╝╚═════╝  ╚═════╝  ╚═════╝ ╚═╝  ╚═╝      ╚═══╝  ╚═╝╚═════╝ ╚══════╝ ╚═════╝ 
                                                                                                           
██████╗  ██████╗ ██╗    ██╗███╗   ██╗██╗      ██████╗  █████╗ ██████╗ ███████╗██████╗                      
██╔══██╗██╔═══██╗██║    ██║████╗  ██║██║     ██╔═══██╗██╔══██╗██╔══██╗██╔════╝██╔══██╗                     
██║  ██║██║   ██║██║ █╗ ██║██╔██╗ ██║██║     ██║   ██║███████║██║  ██║█████╗  ██████╔╝                     
██║  ██║██║   ██║██║███╗██║██║╚██╗██║██║     ██║   ██║██╔══██║██║  ██║██╔══╝  ██╔══██╗                     
██████╔╝╚██████╔╝╚███╔███╔╝██║ ╚████║███████╗╚██████╔╝██║  ██║██████╔╝███████╗██║  ██║                     
╚═════╝  ╚═════╝  ╚══╝╚══╝ ╚═╝  ╚═══╝╚══════╝ ╚═════╝ ╚═╝  ╚═╝╚═════╝ ╚══════╝╚═╝  ╚═╝                                                                                                                                                            ");

            Console.WriteLine("Author: Lucas00012");
            Console.WriteLine("Github: https://github.com/Lucas00012");
            Console.WriteLine("Linkedin: https://www.linkedin.com/in/lucasormond");
            Console.WriteLine();

            Console.Write("Please insert the video url that you want: ");
            var facebookPostUrl = Console.ReadLine();

            Console.Write("Please insert the output directory that you want: ");
            var outputDirectory = Console.ReadLine();

            try
            {
                Console.Clear();

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