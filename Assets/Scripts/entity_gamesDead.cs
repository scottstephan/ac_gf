using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;

namespace Assets.autoCompete.games
{
    [DynamoDBTable("games_dead")] //players / games_live / games_dead

    public class entity_gamesDead
    {
        [DynamoDBHashKey]
        public string gameID { get; set; }//Hash Key
        [DynamoDBRangeKey]
        public string winnerPlayer { get; set; }
        [DynamoDBProperty]
        public string player1_id { get; set; }
        [DynamoDBProperty]
        public string player1_name { get; set; }
        [DynamoDBProperty]
        public string player2_id { get; set; } 
        [DynamoDBProperty]
        public string player2_name;
        [DynamoDBProperty]
        public int p1_score { get; set; }
        [DynamoDBProperty]
        public int p2_score { get; set; }


        public void initGameDead(entity_games gameToCopy)
        {
            gameID = gameToCopy.gameID;
            player1_id = gameToCopy.player1_id;
            player1_name = gameToCopy.player1_name;
            player2_id = gameToCopy.player2_id;
            player2_name = gameToCopy.player2_name;
            p1_score = gameToCopy.p1_score;
            p2_score = gameToCopy.p2_score;

            if(p1_score > p2_score)
            {
                winnerPlayer = "p1";
            }
            else if( p1_score < p2_score)
            {
                winnerPlayer= "p2";
            }
            else if(p1_score == p2_score)
            {
                winnerPlayer = "tie";
            }
            
        }


    }


}
