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
        [DynamoDBProperty]
        public bool isMPGame { get; set;}

        public void initGame(string gID, string p1Id, string p2Id, bool p1fin, bool p2fin, int p1Score, int p2Score, bool mp)
        {
            gameID = gID;
            player1_id = p1Id;
            player2_id = p2Id;
            p1_Fin = p1fin;
            p2_Fin = p2fin;
            p1Score = p1_score;
            p2Score = p2_score;
            isMPGame = mp;
        }


    }

   
}
