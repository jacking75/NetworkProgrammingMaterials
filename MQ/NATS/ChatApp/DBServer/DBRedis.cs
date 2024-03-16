using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudStructures;
using CloudStructures.Structures;

/*
 * 이 코드 내용을  CloudStructures로 바꾸기
 * https://github.com/neuecc/CloudStructures
 * 
 * https://github.com/microsoftarchive/redis/releases
 * MS에서 만든 redis
 * 
 */

namespace WebApplication2
{
    public class DBRedis {

        public static RedisConnection Connection { get; set; }
        
        static public void Init()
         {
            var config = new RedisConfig("localhost", "127.0.0.1:6379");
            Connection = new RedisConnection(config);
        }


        
        static public async Task<RedisResult<TReturn>> GetValue<TReturn>( string key )  {
            var defaultExpiry = TimeSpan.FromSeconds(60);
            var redis = new RedisString<TReturn>(DBRedis.Connection, key, defaultExpiry);
            var cachedObject = await redis.GetAsync();
            return cachedObject;
        }


        static public async Task<bool> SetValue<T>( string key, T value ) where T : class  {
            var defaultExpiry = TimeSpan.FromSeconds(60);
            var redis = new RedisString<T>(DBRedis.Connection, key, defaultExpiry);

            var result = await redis.SetAsync(value);
            return result;
        }


        /*static public async Task<TReturn> GetValue<TReturn>(string key)
             {
                 var cachedObject = await Sut.GetAsync<TReturn>(key);
                 return cachedObject;
             }

             static public async Task<bool> SetValue<T>(string key, T value) where T : class
             {
                 var result = await Sut.SetAddAsync<T>(key, value);
                 return result;
             }
              */

    }
}
