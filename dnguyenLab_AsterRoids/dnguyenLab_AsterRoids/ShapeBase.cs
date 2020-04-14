/*********************************************************************************
* Course: CMPE2800
* Instructor: Prof. Shane Kelemen

* Program: class ShapeBase
* Description: main class stored all shapes
* Date: March 17, 2020
* Author: Dat Nguyen
* 
* Revised: 
*   Rev01 - April 01, 2020 - modified Tick function
*   Rev02 - April 07, 2020 - modified Tick function, add class variables
**********************************************************************************/
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace dnguyenLab_AsterRoids
{
    /// <summary>
    /// mother of all children shapes
    /// </summary>
    internal abstract class ShapeBase
    {
        //static random shared btw classes
        protected static Random _rnd = new Random();

        //intial values of shape
        //radius
        protected const float _cfRadius = 15;

        //speed limit of any shape, used for ship speed
        protected const float _cMaxSpeed = 5;

        //rotation angle and its increment
        public float _fRotation;
        public float _fRotationIncrement;

        //speed vector
        public float _fXSpeed;
        public float _fYSpeed;

        //return current angle
        public float CurrentAngle
        {
            get
            {
                return _fRotation;
            }
        }

        //item type declaration
        public enum eItemType
        {
            Ship,
            Rock,
            Shot,
            BonusFire,
            BonusLife
        }

        //item type variable
        public eItemType _eItem { get; set; }

        /// <summary>
        /// static property of class Radius
        /// </summary>
        public float Radius
        {
            get; set;
        } = _cfRadius;

        /// <summary>
        /// property of shape removal flag
        /// </summary>
        public bool IsMarkedForDeath
        {
            get; set;
        } = false;

        /// <summary>
        /// property of shape center
        /// </summary>
        public PointF Location
        {
            get; set;
        }

        /// <summary>
        /// Color of each shape instance
        /// Rock shape use this to determine the collision with ship
        /// </summary>
        public Color _itemColor
        {
            get; set;
        } = RandomColorLib.RandColor();

        /// <summary>
        /// Fading flag
        /// Rock shape use this to determine the collision with ship
        /// </summary>
        public bool IsFading
        {
            get; set;
        } = false;

        /// <summary>
        /// basecolor property for the whole class
        /// </summary>
        public static Color BaseColor
        {
            get; set;
        } = RandomColorLib.RandColor();

        //current canvas size
        public Size _size;

        //start opacity
        public float _fOpacity { get; set; } = 0;

        //speed vector variable
        public double _fSpeed = 0;

        /// <summary>
        /// main CTOR of shapes
        /// </summary>
        /// <param name="pos">center</param>
        public ShapeBase(PointF pos)
        {
            //set shape center
            Location = pos;

            //import chosen color
            _itemColor = BaseColor;

            //starting rotation angle
            _fRotation = 0;

            //randomize the rotation increment
            _fRotationIncrement = 0;

            //randomize speed vectors
            _fXSpeed = 0;
            _fYSpeed = 0;
        }

        /// <summary>
        /// Render outlet to main program
        /// </summary>
        /// <param name="gr">canvas input</param>
        /// <param name="color">desired color</param>
        public void Render(Graphics gr)
        {
            virtualRender(gr);
        }

        /// <summary>
        /// Virtual method between main & derived classes
        /// </summary>
        /// <param name="gr">current canvas</param>
        public virtual void virtualRender(Graphics gr)
        {
            //if shape is buller, technicolor it for fun
            if (_eItem == eItemType.Shot) _itemColor = RandomColorLib.RandColor();

            //fill the supply graphic board with supplied path and color
            gr.FillPath(new SolidBrush(_itemColor), GetPath());

            //draw the outline of shape, for testing purpose
            //gr.DrawPath(new Pen(Color.White), GetPath());
        }

        /// <summary>
        /// Calculate the distance btw 2 shapes
        /// </summary>
        /// <param name="s1">shape 1</param>
        /// <param name="s2">shape 2</param>
        /// <returns></returns>
        public static float Dist(ShapeBase s1, ShapeBase s2)
        {
            //the distance between 2 center
            float distance = 0;

            //formular sqrt[(x1-x2)**2 + (y1-y2)**2]
            distance = (float)Math.Sqrt(Math.Pow((s1.Location.X - s2.Location.X), 2)
                + Math.Pow((s1.Location.Y - s2.Location.Y), 2));

            //return accepted value
            return distance;
        }

        /// <summary>
        /// move the shape and bouncing inside the canvas
        /// </summary>
        /// <param name="canvas">input border size</param>
        public void Tick(Size canvas)
        {
            //calculate item speed
            _fSpeed = Math.Sqrt(Math.Pow(_fXSpeed, 2) + Math.Pow(_fYSpeed, 2));

            //speed limit only for SHIP
            //based on the pre-set limit above
            if (_eItem == eItemType.Ship)
            {
                if (_fXSpeed > _cMaxSpeed) _fXSpeed = _cMaxSpeed;
                if (_fXSpeed < -_cMaxSpeed) _fXSpeed = -_cMaxSpeed;

                if (_fYSpeed > _cMaxSpeed) _fYSpeed = _cMaxSpeed;
                if (_fYSpeed < -_cMaxSpeed) _fYSpeed = -_cMaxSpeed;
            }

            //update current canvas size
            _size = canvas;

            //move the shape and bouncing inside the size
            float x = Location.X + _fXSpeed;
            float y = Location.Y + _fYSpeed;

            //half length of the shape
            float halfLen = (float)(Radius / 2.0);

            //check border, if go outside, flip the velo as usual
            //shape center goes out of right side
            if (x - halfLen > canvas.Width)
            {
                x = -halfLen;
            }

            //shape center goes out of left side
            if (x + halfLen < 0)
            {
                x = canvas.Width + halfLen;
            }

            //shape center goes out of bottom side
            if (y - halfLen > canvas.Height)
            {
                y = -halfLen;
            }

            //shape center goes out of top side
            if (y + halfLen < 0)
            {
                y = canvas.Height + halfLen;
            }

            //move the shape to new location
            Location = new PointF(x, y);
        }

        /// <summary>
        /// Make a graphicpath based on user choice
        /// </summary>
        /// <param name="radius">how big</param>
        /// <param name="vertexCount">how many sides</param>
        /// <param name="variance">how scale</param>
        /// <returns></returns>
        protected static GraphicsPath MakePolyPath(float radius, int vertexCount, float variance)
        {
            //build a path
            GraphicsPath gp = new GraphicsPath();

            //declare the array holder
            PointF[] arrayPts = new PointF[vertexCount];

            //depend on how many faces/sides of shape
            //build a point array
            for (int i = 0; i < vertexCount; i++)
            {
                //angle btw points
                double angle = (Math.PI * 2.0 / vertexCount) * i;

                //calculate x and y based on the radius and how many sides
                float x = (float)(Math.Cos(angle) * radius - _rnd.NextDouble() * radius * variance);

                float y = (float)(Math.Sin(angle) * radius - _rnd.NextDouble() * radius * variance);

                //add to the array
                arrayPts[i] = new PointF(x, y);
            }

            //use the array to build a drawing path
            gp.StartFigure();
            gp.AddLines(arrayPts);
            gp.CloseFigure();

            //return accepted path
            return gp;
        }

        /// <summary>
        /// abstract method for all children classes
        /// return calculated graphicpath
        /// </summary>
        /// <returns>accepted graphicpath</returns>
        public abstract GraphicsPath GetPath();
    }
}