using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MoreLinq;
using SiteParser.Implementation.Database;
using SiteParser.Implementation.Indexing;
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

		public async Task<ScanResult> ScanAsync(Uri pageUrl, int maxDepth, int maxLinksOnPageCount)
		{
			var indexedUrls = new ConcurrentDictionary<Uri, byte>();
			var urlsToIndex = new ConcurrentDictionary<Uri, int>();
			var urlsNotToIndex = new ConcurrentDictionary<Uri, byte>();
			urlsToIndex.TryAdd(pageUrl, 1);
			
			var semaphore = new SemaphoreSlim(8);
			
			var indexingTasks = new ConcurrentDictionary<Task, byte>();
			var completedIndexingTasks = new ConcurrentStack<Task>();
			while (!urlsToIndex.IsEmpty)
			{
				var currentUrl = urlsToIndex.Keys.First();
				urlsToIndex.TryRemove(currentUrl, out var currentDepth);
				if (currentDepth > maxDepth)
				{
					urlsNotToIndex.TryAdd(currentUrl, default);
					continue;
				}

				semaphore.Wait();
				indexingTasks.TryAdd(
				  ScanPageAsync(currentUrl, currentDepth, pageUrl, urlsNotToIndex, urlsToIndex, indexedUrls, maxLinksOnPageCount)
						.ContinueWith(task =>
						{
							 semaphore.Release();
							 indexingTasks.TryRemove(task, out _);
							 completedIndexingTasks.Push(task);
						}),
				  default
				);

				while (urlsToIndex.IsEmpty)
				{
				  var indexingTask = indexingTasks.Keys.FirstOrDefault();
				  if (indexingTask == null)
						break;
				  
				  indexingTasks.TryRemove(indexingTask, out _);
				  indexingTask.Wait();
				}
			}
			
			return new ScanResult(indexedUrls.Keys.ToArray());
		}

		private async Task ScanPageAsync(
			Uri currentUrl,
			int currentDepth,
			Uri baseUrl,
			ConcurrentDictionary<Uri, byte> urlsNotToIndex,
			ConcurrentDictionary<Uri, int> urlsToIndex,
			ConcurrentDictionary<Uri, byte> indexedUrls,
			int maxLinksOnPageCount)
		{
			
			var page = await _downloader.DownloadPageAsync(currentUrl);
			
			var parsedHtml = HtmlParser.ParseHtml(page, baseUrl);

			var filteredLinks = _urlFilter.Filter(parsedHtml.Links, baseUrl);
			filteredLinks
				.Where(url => !indexedUrls.ContainsKey(url))
				.Where(url => !urlsNotToIndex.ContainsKey(url))
				.Take(maxLinksOnPageCount)
				.ForEach(uri => urlsToIndex.TryAdd(uri, currentDepth + 1));
			
#pragma warning disable 4014
			_database.InsertAsync(new ScannedPage(currentUrl, parsedHtml.Text));
#pragma warning restore 4014
			indexedUrls.TryAdd(currentUrl, default);
		}
	}
}