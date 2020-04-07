using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using Csg.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Csg.Data.DapperTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    [TestCategory("Integration")]
    public class AdventureworksTests
    {
        private const string _connectionString = @"Data Source=(localdb)\ProjectsV13;Initial Catalog=AdventureWorks;Integrated Security=True;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        private static System.Data.SqlClient.SqlConnection CreateConnection()
        {
            var c = new System.Data.SqlClient.SqlConnection(_connectionString);
            c.Open();
            return c;
        }

        [TestMethod]
        public async Task Test_QueryAsync()
        {
            using (var conn = CreateConnection())
            {
                var people = await conn.QueryBuilder("Person.Person")
                    .Select("BusinessEntityID", "FirstName", "LastName")
                    .Where(x => x.FieldEquals("PersonType", "EM", System.Data.DbType.StringFixedLength, size: 2))
                    .OrderBy("BusinessEntityID")
                    .QueryAsync<Person>();

                Assert.IsNotNull(people);
                Assert.AreEqual(273, people.Count());
            }
        }

        [TestMethod]
        public void Test_QueryStreaming()
        {
            using (var conn = CreateConnection())
            {
                var people = conn.QueryBuilder("Person.Person")
                    .Select("BusinessEntityID", "FirstName", "LastName")
                    .Where(x => x.FieldEquals("PersonType", "EM", System.Data.DbType.StringFixedLength, size: 2))
                    .OrderBy("BusinessEntityID")
                    .Query<Person>(Dapper.CommandFlags.Pipelined);

                Assert.IsNotNull(people);
                Assert.AreEqual(273, people.Count());
            }
        }
    }
}



