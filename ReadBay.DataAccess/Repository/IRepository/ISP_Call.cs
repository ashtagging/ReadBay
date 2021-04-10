using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadBay.DataAccess.Repository.IRepository
{
    // Interface Stored Procedure Calls Wrap multiple repositories inside a unit of work
    // Stored Procedure call that we can use tyo call stored procedures
    public interface ISP_Call : IDisposable
    {
        // Use Dapper to pass all the parameters
        T Single<T>(string procedureName, DynamicParameters param = null);

        // Executes something to the database without retrieving anything
        void Execute(string procedureName, DynamicParameters param = null);

        // Retrieves one complete record (row)
        T OneRecord<T>(string procedureName, DynamicParameters param = null);

        // Retrieves all the rows (records)
        IEnumerable<T> List<T>(string procedureName, DynamicParameters param = null);

        //Stored Procedure that returns two tables
        Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string procedureName, DynamicParameters param = null);
    }
}
