/*********************************************************************************
* Course: CMPE2800
* Instructor: Prof. Shane Kelemen

* Program: class ShapeBullet
* Description: Derived shape from base shape, for bullet in game
* Date: April 1, 2020
* Author: Dat Nguyen
**********************************************************************************/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace dnguyenLab_AsterRoids
{
    class ShapeBullet : ShapeBase
    {
        //number of bullet on screen - 8 for now
        public const int BULLET_ALLOW = 8;

        //bullet will explode or disappeared at 30 frames
        private const int MAX_BULLET_FRAME = 30;

        //base values of bullet time
        public int _iBulletPunk = MAX_BULLET_FRAME;

        //2 kinds of bullet available idea
        //not enough time to develop
        public enum eBulletType
        {
            Laser = 10,
            Bomb = 2
        }

        //bullet type for play
        public static eBulletType bulletType
        { get; set; } = eBulletType.Laser;

        //graphicpath of the whole type
        public GraphicsPath _bulletPath;
        public GraphicsPath _bombPath;

        //collection of bullet
        private static List<ShapeBullet> _lstCurrentBullet = new List<ShapeBullet>(BULLET_ALLOW);

        /// <summary>
        /// Main CTOR of bullet shape
        /// </summary>
        /// <param name="center"></param>
        public ShapeBullet(PointF center, ShapeShip ship) : base(center)
        {
            //this is a bullet item
            _eItem = eItemType.Shot;

            //bullet characteristics: color, radius
            _itemColor = Color.White;
            _fOpacity = 255;
            Radius = 6;

            //calculate location of initial bullet based on 
            //the location of the center of the ship
            float radRotation = (float)((ship._fRotation - 90) * Math.PI / 180);
            float x = (float)(ship.Location.X + ship.Radius * Math.Cos(radRotation));
            float y = (float)(ship.Location.Y + ship.Radius * Math.Sin(radRotation));
            Location = new PointF(x, y);

            //manual parameters of velocity
            radRotation = (float)(ship._fRotation * Math.PI / 180);
            _fXSpeed = (float)(Math.Sin(radRotation) * (int)bulletType);
            _fYSpeed = (float)(Math.Cos(radRotation) * (int)bulletType * -1);

            //graphicpath
            _bulletPath = MakeBullet(Radius);
            _bombPath = MakeBullet(Radius * (int)eBulletType.Bomb);
        }

        /// <summary>
        /// Make a bullet shape
        /// </summary>
        /// <param name="radius">size of bullet</param>
        /// <returns>accpected shape graphicpath</returns>
        public GraphicsPath MakeBullet(float radius)
        {
            //declare the gp variable
            GraphicsPath gp = new GraphicsPath();
            float halfR = (float)(radius / 2.0);

            //normal bullet is a circle around the center
            gp.AddEllipse(-halfR, -halfR, radius, radius);

            //accpected shape graphicpath
            return gp;
        }

        /// <summary>
        /// override of abstract method in mother class
        /// return calculated graphicpath
        /// </summary>
        /// <returns>return calculated graphicpath</returns>
        public override GraphicsPath GetPath()
        {
            //clone Graphic Path
            GraphicsPath gpClone = (GraphicsPath)_bulletPath.Clone();

            //make matrix for rotate or move
            Matrix matrix = new Matrix();
            matrix.Translate(this.Location.X, this.Location.Y);
            gpClone.Transform(matrix);

            //return and exit
            return gpClone;
        }

        /// <summary>
        /// Check bullet timing to decide the removal
        /// </summary>
        /// <returns>flag to remove the bullet</returns>
        public bool BulletTick()
        {
            _iBulletPunk--;

            if (_iBulletPunk < 1)
                IsMarkedForDeath = true;

            return IsMarkedForDeath;
        }
    }
}
