/*********************************************************************************
* Course: CMPE2800
* Instructor: Prof. Shane Kelemen

* Program: class ShapeThruster
* Description: Derived shape from base shape, for thruster of ship in game
* Date: April 10, 2020
* Author: Dat Nguyen
**********************************************************************************/
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace dnguyenLab_AsterRoids
{
    class ShapeThruster : ShapeBase
    {
        //this shape graphicpath
        private GraphicsPath _gpThruster = new GraphicsPath();

        //flag to draw the thurster
        public bool IsThruster { get; set; } = false;

        //thruster size depends on ship speed
        public double _shipSpeed = 0;

        /// <summary>
        /// Main CTOR of this shape, link to base shape
        /// </summary>
        /// <param name="center">center of this shape</param>
        /// <param name="ship">current airship instance</param>
        public ShapeThruster(PointF center, ShapeShip ship) : base(center)
        {
            //make path
            _gpThruster = MakeThruster(Radius, 3);

            //use ship angle to calculate thruster rotation
            _fRotation = ship._fRotation + 90;
        }

        /// <summary>
        /// Override render method, draw shape when flag is true, else transparent it
        /// </summary>
        /// <param name="gr">current graphics canvas</param>
        public override void virtualRender(Graphics gr)
        {
            //rand color everytime render
            _itemColor = RandomColorLib.RandColor();

            //always draw but use transparent color if thruting is not ON
            if (IsThruster)
            {
                //make color like a flame
                _fOpacity = _rnd.Next(25, 200);
                gr.FillPath(new SolidBrush(_itemColor), GetPath());
            }

            //hide it by set the color to transparent
            else
            {
                gr.FillPath(new SolidBrush(Color.Transparent), GetPath());
            }

        }

        /// <summary>
        /// Make a graphicpath for this shape
        /// </summary>
        /// <param name="radius">input size</param>
        /// <param name="vertexCount">input sides</param>
        /// <returns>accpeted graphicpath</returns>
        private GraphicsPath MakeThruster(float radius, int vertexCount)
        {
            //build a path
            GraphicsPath gp = new GraphicsPath();

            //declare the array holder
            PointF[] arrayPts = new PointF[vertexCount + 1];

            //depend on how many faces/sides of shape
            //build a point array based on current ship characteristics
            float halfR = (float)(radius / 2.0);
            float fSin = (float)(Math.Sin(360 / vertexCount * Math.PI / 180) * radius);

            arrayPts[0] = new PointF(0, (float)(radius * _shipSpeed));
            arrayPts[1] = new PointF(-fSin, halfR);
            arrayPts[2] = new PointF(0, 0);
            arrayPts[3] = new PointF(fSin, halfR);

            //use the array to build a drawing path
            gp.StartFigure();
            gp.AddLines(arrayPts);
            gp.CloseFigure();

            //return accepted path
            return gp;
        }

        /// <summary>
        /// Get the original graphicpath, modify with new angle and location
        /// </summary>
        /// <returns>accpeted graphicpath</returns>
        public override GraphicsPath GetPath()
        {
            //clone Graphic Path
            GraphicsPath gpClone = (GraphicsPath)MakeThruster(Radius, 3).Clone();

            //make matrix for rotate or move
            Matrix matrix = new Matrix();

            ////rotate matrix first, if move, move after this
            matrix.Rotate(_fRotation);
            gpClone.Transform(matrix);

            //reuse the old matrix
            //move the origin to center of the shape!!!
            matrix.Reset();
            matrix.Translate(this.Location.X, this.Location.Y);
            gpClone.Transform(matrix);

            //return and exit
            return gpClone;
        }
    }
}
