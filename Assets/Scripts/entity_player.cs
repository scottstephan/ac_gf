using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;

namespace Assets.autoCompete.players
{
    [DynamoDBTable("players")] //players / games_live / games_dead

    public class entity_players
    {
        [DynamoDBHashKey]
        public string playerID { get; set; }//Hash Key- This is assigned at runtime based on player's device and is stored in playerPrefs
        [DynamoDBRangeKey]
        public string searchName { get; set; }
        [DynamoDBProperty]
        public string autoCompeteUsableID; //This one COULD change over tine if a Guest authenticates via GameCenter or Facebook. By default, it's the same as the playerID
        [DynamoDBProperty]
        public string playerName { get; set; }
        [DynamoDBProperty]
        public string playerAuthSource { get; set; } //can i do enum?? guest,fb,google,unauth,auth

    }
}
