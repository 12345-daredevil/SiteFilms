using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SiteFilms.Data;
using System.Collections;
using System.Reflection;

namespace SiteFilms.Models
{
    public class MyСacheModel
	{
        static int time10 = 10;
        static int time30 = 30;
        //static int time90 = 90;
        static int time180 = 180;

        #region Country
        public static async Task<Country[]> GetCacheCountry(ApplicationDbContext db, IMemoryCache cache)
        {
            string key = "Country";

            if (cache.TryGetValue(key, out Country[] ? value))
                if (value != null)
                    return value;

            value = await db.Countrys.AsNoTracking().ToArrayAsync();

			cache.Set(key, value, new MemoryCacheEntryOptions
			{
				SlidingExpiration = TimeSpan.FromMinutes(time30)
			});

			return value;
		}
        #endregion

        #region Genre
        public static async Task<Genre[]> GetCacheGenre(ApplicationDbContext db, IMemoryCache cache)
        {
            string key = "Genre";

            if (cache.TryGetValue(key, out Genre[] ? value))
                if (value != null)
                    return value;

            value = await db.Genres.AsNoTracking().ToArrayAsync();

            cache.Set(key, value, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(time30)
            });

            return value;
        }
        #endregion

        #region Message
        public static async Task<Message> GetCacheMessage(ApplicationDbContext db, IMemoryCache cache)
        {
            string key = "Message";

            if (cache.TryGetValue(key, out Message? value))
                if (value != null)
                    return value;

            value = await db.Messages.AsNoTracking().FirstAsync();

            cache.Set(key, value, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(time180)
            });

            return value;
        }
        #endregion

        #region Person
        public static async Task<Person> GetCachePerson(ApplicationDbContext db, IMemoryCache cache, string userId)
        {
            string key = "Person" + userId;

            if (cache.TryGetValue(key, out Person? value))
                if (value != null)
                    return value;

            value = await db.Persons.AsNoTracking().FirstAsync(x => x.Id == userId);

            cache.Set(key, value, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(time10)
            });

            return value;
        }
        #endregion

        #region Add/Delete Block payment
        public static void AddCacheBlock(IMemoryCache cache, uint person, byte minute)
        {
            cache.Set("Block" + person, person, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minute)
            });
        }

        public static bool GetCacheBlock(IMemoryCache cache, uint person)
            => cache.TryGetValue("Block" + person, out _);
        #endregion

        #region Change list Person
        public static void ChangeCachePersonList(IMemoryCache cache, List<Person> list)
        {
            foreach (var value in list)
            {
                cache.Set("Person" + value.Id, value, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(time10)
                });
            }
        }
        #endregion

        #region Delete cache
        public static void DeleteCache(IMemoryCache cache, string name)
        {
            if (cache.TryGetValue(name, out _)) cache.Remove(name);
        }
        #endregion

        #region Get list cashe
        public static List<string> CacheListName(IMemoryCache cache, string name)
        {
            List<string> items = new();
            var coherentState = typeof(MemoryCache).GetField("_coherentState", BindingFlags.NonPublic | BindingFlags.Instance);
            var coherentStateValue = coherentState != null ? coherentState.GetValue(cache) : null;
            var field = coherentStateValue != null ? coherentStateValue.GetType().GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance) : null;

            if (field != null)
            {
                var collection = field.GetValue(coherentStateValue) as ICollection;
                if (collection != null)
                    foreach (var item in collection)
                    {
                        var methodInfo = item.GetType().GetProperty("Key");
                        if (methodInfo != null)
                        {
                            var val = methodInfo.GetValue(item);
                            if (val is not null && val is string && val.ToString()!.StartsWith(name)) items.Add((string)val);
                        }
                    }
            }
            return items;
        }
        #endregion

        #region If i wont delete part string and get number
        //static List<uint> SubstringList(List<string> list)
        //{
        //    var newList = new List<uint>();

        //    foreach (var item in list)
        //        newList.Add(uint.Parse(item.Substring(7)));

        //    return newList;
        //}
        #endregion
    }
}
