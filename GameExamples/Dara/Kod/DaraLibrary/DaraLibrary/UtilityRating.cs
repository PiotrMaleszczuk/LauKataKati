using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaraLibrary
{
    public class UtilityRating
    {
        public static int MAX_RATING = Int32.MaxValue - 1;
        public static int MIN_RATING = (-Int32.MaxValue) + 1;
        internal static double rating = 0;


        public static double getRating(Board board)
        {
           // double rating = board.getMaxSidePawnCount() - board.getMinSidePawnCount();

            rating = 0;
            board.findAndRateTwos();
            board.findAndRateCloseToTwos();
            board.findAndRateCloseToThrees();
            board.findAndRateThrees();
            board.findAndRateCloseMoves();
        

            

            return rating;
        }
    }
}
