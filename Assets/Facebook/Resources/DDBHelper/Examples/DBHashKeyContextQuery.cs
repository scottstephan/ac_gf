using UnityEngine;
using System.Collections;
using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;
using System;

namespace DDBHelper
{
    /// <summary>
    /// Setup:
    /// 1.  Must have table named DDBHelper - or change the DynamoDBTable name
    /// 2.  Hash key must be named Name - or change the DynamoDBHashKey variable name
    /// 3.  Range key must be named Type - or change the DynamoDBRangeKey variable name
    /// 4.  If you want actual data back, you must have some items in your table with the name 'banana' - or change what you're querying
    /// 5.  If you're using cognito, this is an unauthenticated user call, and it must have permission to query the table
    /// 6.  If using basic credentials, must have table access to query
    /// </summary>
    [DynamoDBTable("DDBHelper")]
    public class DBHashKeyContextQuery
    {
        [DynamoDBHashKey]
        public string Name { get; set; }

        [DynamoDBRangeKey]
        public string Type { get; set; }

        /// <summary>
        /// Basic example, showing how to retrieve a list of items of type T using only the hash key
        /// In this example, the hash key is a string named 'banana'  -- yes yes, weird example name
        /// </summary>
        public static void TestHashKeyQueryForItem()
        {
            DBWorker.Instance.QueryHashKeyObject<DBHashKeyContextQuery>("banana", new List<string>(),
                delegate (List<DBHashKeyContextQuery> list, Exception e)
                {
                    if (e != null)
                    {
                        Debug.Log("QueryHashKeyObject msg=" + e.Message);
                    }
                    else
                    {
                        foreach (var item in list)
                            Debug.Log("Found Test Item  Hash=" + item.Name + "  Range=" + item.Type);
                    }
                });
        }
    }
}
