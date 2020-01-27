using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MoreLinq;
using SiteParser.Implementation.Database;
using SiteParser.Implementation.Scan;
using SiteParser.Models;

namespace SiteParser.Implementation
{
	public class Scanner
	{
		private readonly Downloader _downloader;
		private readonly ElasticSearchClient _database;
		private readonly UrlFilter _urlFilter;

		public Scanner(Downloader downloader, ElasticSearchClient database, UrlFilter urlFilter = null)
		{
			_downloader = downloader;
			_database = database;
			_urlFilter = urlFilter ?? new UrlFilter();
		}

		public Task<ScanResult> ScanAsync(Uri pageUrl, int maxDepth, int maxLinksOnPageCount)
		{
			var scannedUrls = new ConcurrentDictionary<Uri, byte>();
			var urlsToScan = new ConcurrentDictionary<Uri, int>();
			var urlsNotToScan = new ConcurrentDictionary<Uri, byte>();
			urlsToScan.TryAdd(pageUrl, 1);
			
			var semaphore = new SemaphoreSlim(32);
			
			var scanTasks = new ConcurrentDictionary<Task, byte>();
			var completedScanTasks = new ConcurrentStack<Task>();
			while (!urlsToScan.IsEmpty)
			{
				var currentUrl = urlsToScan.Keys.First();
				urlsToScan.TryRemove(currentUrl, out var currentDepth);
				if (currentDepth > maxDepth)
				{
					urlsNotToScan.TryAdd(currentUrl, default);
					continue;
				}

				semaphore.Wait();
				scanTasks.TryAdd(
				  ScanPageAsync(currentUrl, currentDepth, pageUrl, urlsNotToScan, urlsToScan, scannedUrls, maxLinksOnPageCount)
						.ContinueWith(task =>
						{
							 semaphore.Release();
							 scanTasks.TryRemove(task, out _);
							 completedScanTasks.Push(task);
						}),
				  default
				);

				while (urlsToScan.IsEmpty)
				{
				  var scanTask = scanTasks.Keys.FirstOrDefault();
				  if (scanTask == null)
						break;
				  
				  scanTasks.TryRemove(scanTask, out _);
				  scanTask.Wait();
				}
			}

			Task.WaitAll(scanTasks.Keys.ToArray());
			var result = new ScanResult(scannedUrls.Keys.ToArray());
			return Task.FromResult(result);
		}

		private async Task ScanPageAsync(
			Uri currentUrl,
			int currentDepth,
			Uri baseUrl,
			ConcurrentDictionary<Uri, byte> urlsNotToScan,
			ConcurrentDictionary<Uri, int> urlsToScan,
			ConcurrentDictionary<Uri, byte> scannedUrls,
			int maxLinksOnPageCount)
		{
			
			var page = await _downloader.DownloadPageAsync(currentUrl);
			
			var parsedHtml = HtmlParser.ParseHtml(page, baseUrl);

			var filteredLinks = _urlFilter.Filter(parsedHtml.Links, baseUrl);
			filteredLinks
				.Where(url => !scannedUrls.ContainsKey(url))
				.Where(url => !urlsNotToScan.ContainsKey(url))
				.Take(maxLinksOnPageCount)
				.ForEach(uri => urlsToScan.TryAdd(uri, currentDepth + 1));
			
#pragma warning disable 4014
			_database.InsertAsync(new ScannedPage(currentUrl, parsedHtml.Text));
#pragma warning restore 4014
			scannedUrls.TryAdd(currentUrl, default);
		}
	}
}