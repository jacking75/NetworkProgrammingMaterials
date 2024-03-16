using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Dapper;
using MySql.Data.MySqlClient;

namespace WebApplication2
{
    public class DBMysql
    {
        static private MySqlConnection ConnectionFactory()
        {

            string ConnString = "Server=localhost;Database=gamedb;Uid=root;Pwd=qwer1234";
            var db = new MySql.Data.MySqlClient.MySqlConnection(ConnString);
            return db;

        }

        static public async Task<int> CreateUser(string userID, string userPW)
        {
            try
            {
                using (var connection = ConnectionFactory())
                {
                    var val = await connection.ExecuteAsync("insert users(id, pw, reg_dt) values(@id, @pw, date_format(now(), '%Y/%c/%e'));", new { id = userID, pw = userPW });
                    return val;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        static public async Task<bool> LoginUser(string userID, string userPW)
        {
            try
            {
                using (var connection = ConnectionFactory())
                {
                    var val = await connection.QuerySingleOrDefaultAsync<DBUser>("select * from users where id=@id", new { id = userID });

                    if (val == null || val.pw != userPW)
                    {
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    
        class DBUser
        {
            public string id = "";
            public string pw = "";
        }
    }
}
