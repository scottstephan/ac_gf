using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;

namespace Assets.autoCompete.games
{
    [DynamoDBTable("games_live")] //players / games_live / games_dead

    public class entity_games
    {
        [DynamoDBHashKey]
        public string gameID { get; set; }//Hash Key
        [DynamoDBProperty]
        public string player1_id { get; set; }
        [DynamoDBProperty]
        public string player2_id { get; set; } //can i do enum?? guest,fb,google,unauth,auth
        [DynamoDBProperty]
        public bool p1_Fin { get; set; }
        [DynamoDBProperty]
        public bool p2_Fin { get; set; }
        [DynamoDBProperty]
        public int p1_score { get; set; }
        [DynamoDBProperty]
        public int p2_score { get; set; }
    }
}
