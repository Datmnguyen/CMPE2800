/*********************************************************************************
* Course: CMPE2800
* Instructor: Prof. Shane Kelemen

* Program: class ShapeRock
* Description: Derived shape from base shape, for asteroids in the game
* Date: April 1, 2020
* Author: Dat Nguyen
**********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace dnguyenLab_AsterRoids
{

    /// <summary>
    /// children class Rock
    /// </summary>
    internal class ShapeRock : ShapeBase
    {
        /// <summary>
        /// graphic path of this class
        /// </summary>
        private readonly GraphicsPath _modelGraphicsPath;

        /// <summary>
        /// basecolor property
        /// </summary>
        new public static Color BaseColor
        {
            get; set;
        }

        /// <summary>
        /// Enum type for rock size, with correspondent integer value
        /// </summary>
        public enum RockSize
        {
            Big = 3,
            Medium = 2,
            Small = 1
        }

        //class variable of rock size
        public RockSize _rockSize;

        //return the score if this rock is destroyed
        public int RScore { get; private set; }

        /// <summary>
        /// main CTOR linked to mother class, set center location
        /// </summary>
        /// <param name="pos">input center</param>
        public ShapeRock(PointF pos, RockSize size) : base(pos)
        {
            //this is a rock/asteroids
            _eItem = eItemType.Rock;

            //set the size of this rock
            int iScale = (int)size;
            Radius = iScale * Radius;
            _rockSize = size;

            //score as requirements based on this rock size
            if (size == RockSize.Big) RScore = 100;
            if (size == RockSize.Medium) RScore = 200;
            else RScore = 300;

            //make a separate color for this type
            _itemColor = BaseColor;

            //randomize the rotation increment
            _fRotationIncrement = (float)(_rnd.NextDouble() * 6.0 - 3.0);

            //randomize speed vectors
            _fXSpeed = (float)(_rnd.NextDouble() * 5.0 - 2.5);
            _fYSpeed = (float)(_rnd.NextDouble() * 5.0 - 2.5);

            //build gp for this instance shape
            _modelGraphicsPath = MakePolyPath(Radius, _rnd.Next(6, 13), 0.5f);
        }

        /// <summary>
        /// override of abstract method in mother class
        /// return calculated graphicpath
        /// </summary>
        /// <returns>return calculated graphicpath</returns>
        public override GraphicsPath GetPath()
        {
            //clone Graphic Path
            GraphicsPath gpClone = (GraphicsPath)_modelGraphicsPath.Clone();

            //make matrix for rotate or move
            Matrix matrix = new Matrix();

            //move the origin to center of the shape!!!
            matrix.Translate(this.Location.X, this.Location.Y);
            
            //rotate matrix first, if move, move after this
            matrix.Rotate(this._fRotation);

            //modify the graphics path then return
            gpClone.Transform(matrix);
            return gpClone;
        }

        /// <summary>
        /// Extra function for Rock shape
        /// control color fading and rotation
        /// </summary>
        public void ExtraTick()
        {
            //update rotation
            _fRotation += _fRotationIncrement;

            //make fade in color, opacity from 0-255
            if (_fOpacity < 255)
            {
                _fOpacity += (float)0.5;
            }

            //limit the alpha value to max 255
            if (_fOpacity >= 255) _fOpacity = 255;

            //set IsFading flag, allow Rock hit Ship
            if (_fOpacity == 255) IsFading = true;

            //update color transparent
            _itemColor = Color.FromArgb((int)_fOpacity, _itemColor);
        }
    }
}

