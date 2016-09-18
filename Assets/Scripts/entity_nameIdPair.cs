using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;

namespace Assets.autoCompete.nameIDPair
{
    [DynamoDBTable("players_NameID")]
    public class nameIdPair
    {
        [DynamoDBHashKey]
        public string name;
        [DynamoDBProperty]
        public string id;
    }
}
