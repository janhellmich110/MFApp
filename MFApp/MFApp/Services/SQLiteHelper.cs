using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using SQLite;

using SQLitePCL;

namespace MFApp.Services
{
    class SQLiteHelper
    {
        public static bool TableExists(string tableName, SQLiteConnection connection)
        {
            SQLite.TableMapping map = new TableMapping(typeof(SqlDbType)); // Instead of mapping to a specific table just map the whole database type
            object[] ps = new object[0]; // An empty parameters object since I never worked out how to use it properly! (At least I'm honest)

            Int32 tableCount = connection.Query(map, "SELECT * FROM sqlite_master WHERE type = 'table' AND name = '" + tableName + "'", ps).Count; // Executes the query from which we can count the results
            if (tableCount == 0)
            {
                return false;
            }
            else if (tableCount == 1)
            {
                return true;
            }
            else
            {
                throw new Exception("More than one table by the name of " + tableName + " exists in the database.", null);
            }

        }
    }
}
