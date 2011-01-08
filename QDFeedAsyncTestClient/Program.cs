using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using QDFeedParser;

namespace QDFeedAsyncTestClient
{
    class Program
    {
        private const string ValidTestFileSysPath = @"..\..\GoogleNews-Atom.xml";
        private const string ValidTestHttpPath = "http://www.aaronstannard.com/syndication.axd";
        private static bool UseFileSys = true;

        static void Main(string[] args)
        {
            try
            {
                if(args.Length > 0 && args[0].ToLower().Equals("--help"))
                {
                    Console.WriteLine("Arguments: [http | filesys] [uri]");
                    Console.WriteLine(@"e.g. filesys C:\RSS\rss01.xml");
                    Console.WriteLine(@"e.g. http http://www.aaronstannard.com/syndication.axd");
                    Console.WriteLine("The results will be the parsed content of all of the items in the feed.");
                }
                else
                {
                    UseFileSys = args.Length == 0 ? true : !args[0].ToLower().Equals("http");
                    var strFeedUri = args.Length <= 1 ? ValidTestFileSysPath : args[1];

                    Uri feeduri;
                    IFeedFactory factory;
                    if (UseFileSys)
                    {
                        feeduri = new Uri(Path.GetFullPath(strFeedUri));
                        factory = new FileSystemFeedFactory();
                    }
                    else
                    {
                        feeduri = new Uri(strFeedUri);
                        factory = new HttpFeedFactory();
                    }

                    factory.BeginCreateFeed(feeduri, async =>
                    {
                        var feed = factory.EndCreateFeed(async);
                        PrintFeed(feed);
                    });


                    //Just to prevent the window from instantly bailing out.
                    Console.ReadKey();
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex);
            }
        }

        private static void PrintFeed(IFeed feed)
        {
            foreach (var feedItem in feed.Items)
            {
                Console.WriteLine("{0} {1}", feedItem.DatePublished, feedItem.Title);
                Console.WriteLine(feedItem.Link);
            }
        }
    }
}
