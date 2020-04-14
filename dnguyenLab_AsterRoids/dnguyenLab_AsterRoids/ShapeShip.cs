/*********************************************************************************
* Course: CMPE2800
* Instructor: Prof. Shane Kelemen

* Program: class ShapeShip
* Description: Derived shape from base shape, for main ship in game
* Date: April 1, 2020
* Author: Dat Nguyen
**********************************************************************************/
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace dnguyenLab_AsterRoids
{
    /// <summary>
    /// children class Ship
    /// </summary>
    internal class ShapeShip : ShapeBase
    {
        //ship lives
        const int SHIP_LIVES = 3;

        //life property
        public int CurrentLife
        {
            get;
            private set;
        } = SHIP_LIVES;

        //size property
        public static int ShipSize
        { get; set; } = 15;

        //state of Airship
        public bool GotShield
        {
            get; set;
        } = true;

        //shield time, default 2s
        public int _iShieldTime = 200;

        //rotation angle of ship, import from main
        public float ShipHeading
        {
            get; set;
        }

        /// <summary>
        /// static graphic path of this class
        /// </summary>
        private static readonly GraphicsPath _modelStaticPath;

        /// <summary>
        /// basecolor property
        /// </summary>
        new public static Color BaseColor
        {
            get; set;
        }

        /// <summary>
        /// static CTOR, build the gp for this class
        /// </summary>
        static ShapeShip()
        {
            _modelStaticPath = MakeShip(ShipSize, 3);
        }

        /// <summary>
        /// main CTOR linked to mother class, set center location
        /// other parameters like Velo, Rotation Incr are set with default 0
        /// </summary>
        /// <param name="pos">input center</param>
        public ShapeShip(PointF pos) : base(pos)
        {
            //ship type
            _eItem = eItemType.Ship;

            //import chosen color
            _fOpacity = 255;
            _itemColor = BaseColor;

            //set shield for restart game
            _iShieldTime = 200;
            GotShield = true;
        }

        /// <summary>
        /// override of abstract method in mother class
        /// return calculated graphicpath
        /// </summary>
        /// <returns>return calculated graphicpath</returns>
        public override GraphicsPath GetPath()
        {
            //clone Graphic Path
            GraphicsPath gpClone = (GraphicsPath)_modelStaticPath.Clone();

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

        /// <summary>
        /// Make a graphicpath based on user choice
        /// </summary>
        /// <param name="radius">how big</param>
        /// <param name="vertexCount">how many sides</param>
        /// <returns></returns>
        protected static GraphicsPath MakeShip(float radius, int vertexCount)
        {
            //build a path
            GraphicsPath gp = new GraphicsPath();

            //declare the array holder
            PointF[] arrayPts = new PointF[vertexCount + 1];

            //depend on how many faces/sides of shape
            //build a point array
            float halfR = (float)(radius / 2.0);
            float fSin = (float)(Math.Sin(360 / vertexCount * Math.PI / 180) * radius);

            arrayPts[0] = new PointF(0, -radius);
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
        /// Function to check if the shield time is expired or not
        /// </summary>
        /// <returns></returns>
        public bool ShieldTick()
        {
            //decrease 1 shield time every main timer tick (20ms)
            _iShieldTime--;

            //if shield time is expired, set shield to off
            if (_iShieldTime < 1)
                GotShield = false;

            //return accepted flag
            return GotShield;
        }
    }

}
