/*********************************************************************************
* Course: CMPE2800
* Instructor: Prof. Shane Kelemen

* Program: class UserInfo
* Description: Information class, keep game score
* Date: April 1, 2020
* Author: Dat Nguyen
**********************************************************************************/

namespace dnguyenLab_AsterRoids
{
    public class UserInfo
    {
        //current score vs last award score 10K per life
        public long _CurrentScore = 0;
        public long _CurrentAwardScore = 0;

        //number of times life has been given
        public int _AwardsTimes = 0;

        //current life
        public int _Life = 3;

        /// <summary>
        /// Main CTOR, set and reset all class variables
        /// </summary>
        public UserInfo()
        {
            //auto
        }

    }
}
