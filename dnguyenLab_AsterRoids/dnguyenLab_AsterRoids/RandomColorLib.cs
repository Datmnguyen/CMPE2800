/*********************************************************************************
* Course: CMPE2800
* Instructor: Prof. Shane Kelemen

* Program: class RandomColorLib
* Description: Build a library of all color available, good library for color
* Date: April 1, 2020
* Author: Dat Nguyen
**********************************************************************************/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dnguyenLab_AsterRoids
{
    public class RandomColorLib
    {
        //static variable of Random, share btw functions
        private static Random _rnd = new Random();

        /// <summary>
        /// Static method, build a library of visible color
        /// </summary>
        /// <returns>a library of visible color</returns>
        public static List<Color> ListRandColor()
        {
            //random color list
            System.Array _colorEnum = Enum.GetValues(typeof(KnownColor));
            List<Color> lstKnownColor = new List<Color>();

            //get random color
            //load know color to array
            foreach (KnownColor enumVal in _colorEnum)
            {
                Color someColor = Color.FromKnownColor(enumVal);

                //make sure the color is visible
                bool bVisible = someColor.R > 10 || someColor.G > 10 || someColor.B > 10;
                bVisible = bVisible && someColor.ToArgb() != Color.Transparent.ToArgb();
                
                if (bVisible) lstKnownColor.Add(someColor);
            }

            //return accepted list of color
            return lstKnownColor;
        }

        /// <summary>
        /// Pick a random color from the color library
        /// </summary>
        /// <returns>a random color</returns>
        public static Color RandColor()
        {
            //get the library
            List<Color> lstKnownColor = ListRandColor();

            //return the accepted color
            return lstKnownColor[_rnd.Next(lstKnownColor.Count)];
        }
    }
}
