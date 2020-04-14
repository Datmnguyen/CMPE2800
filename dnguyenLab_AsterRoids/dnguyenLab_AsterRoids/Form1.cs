/*********************************************************************************
* Course: CMPE2800
* Instructor: Prof. Shane Kelemen

* Program: dnguyenLab_AsterRoids
* Description: Using GDI+ knowledge to build a classic game
* Date: April 1, 2020
* Author: Dat Nguyen
**********************************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Input;

namespace dnguyenLab_AsterRoids
{
    public partial class Form1 : Form
    {
        //graphic field holder
        Graphics _gr;

        //sound thread to avoid flickering
        Thread m_SoundThread;

        /************************OBJECTS STATE*******************************/
        //all kind of Rock
        private List<ShapeRock> _lstRock = new List<ShapeRock>();
        private List<ShapeRock> _lstBrokenRock = new List<ShapeRock>();

        //bullet collection
        private List<ShapeBullet> _lstBullet = new List<ShapeBullet>();

        //ship holder
        ShapeShip _Airship;
        ShapeThruster _Thruster;

        //airship speed
        public enum eAirShipSpeed
        {
            Normal = 1,
            Fast = 5
        }

        //speed variable
        eAirShipSpeed _shipSpeed = eAirShipSpeed.Normal;

        //selected game level
        eGameLevel _gameLevel = eGameLevel.Easy;

        //explosion images
        public Bitmap _ShipImg;
        public Bitmap _ExplodeImg;
        public Bitmap _BonusImg;

        //award life bar
        public const int AWARD_LIFE = 10000;

        //user info
        UserInfo _UserInfo = new UserInfo();

        /************************GAME STATE*******************************/
        //sound public holder
        SoundPlayer _soundEffect;

        //sound folder and files variable
        string _SoundFolder;
        string _soundOpening, _soundLazer, _soundExplosion, _soundGotHit, _soundGameOver;


        //declare variables for random, stopwatchs
        private static Random _rnd = new Random();

        //stopwatch to timing
        private Stopwatch _sWatch = new Stopwatch();

        //object lock
        private object _oLock = new object();

        //key state information
        private bool _bUp = false, _bLeft = false, _bRight = false, _bDown = false;
        //private bool _bSpace = false;

        //level of game
        public enum eGameLevel
        {
            Easy = 5000,
            Medium = 2000,
            Hard = 500
        }

        //set point for game level
        private const int _MediumScore = 20000;
        private const int _HardScore = 100000;

        //START FLAG
        private bool _bGameStart = false;
        private bool _bWelcome = false;
        private bool _bGameOver = false;
        private bool _bGamePause = false;

        //list of the three highest scores
        List<long> _lstHighScores = new List<long>();

        //score file path
        string _ScoreFilePath;

        //helpbox holder
        frmInfos _HelpBox;

        /************************************************************************/
        /****************************MAIN CODE***********************************/
        /// <summary>
        /// Timer tick -> Calculate new value of each shape -> Move and render the canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerRender_Tick(object sender, EventArgs e)
        {
            //process turning angle of ship, +/- 5 degrees everytime
            if (_bLeft)
                _Airship._fRotation += -5;

            if (_bRight)
                _Airship._fRotation += 5;

            //move forward, consider speed acceleration?!?
            if (_bUp)
            {
                _Airship._fXSpeed += (float)(Math.Sin(_Airship._fRotation * Math.PI / 180) * (int)_shipSpeed);
                _Airship._fYSpeed += (float)(Math.Cos(_Airship._fRotation * Math.PI / 180) * (int)_shipSpeed * -1);
            }

            //move backward, same... but plus PI, make it change direction
            if (_bDown)
            {
                _Airship._fXSpeed += (float)(Math.Sin(_Airship._fRotation * Math.PI / 180 + Math.PI) * (int)_shipSpeed);
                _Airship._fYSpeed += (float)(Math.Cos(_Airship._fRotation * Math.PI / 180 + Math.PI) * (int)_shipSpeed * -1);
            }

            //update thruster values
            _Thruster.Location = _Airship.Location;
            _Thruster._fRotation = _Airship._fRotation;
            _Thruster._shipSpeed = _Airship._fSpeed;

            //now draw
            Render();
        }

        /// <summary>
        /// Render machine run everytime drawing timer tick
        /// </summary>
        private void Render()
        {
            //create brush
            SolidBrush brushRegInter = new SolidBrush(Color.Gray);

            //using double buffer for drawing
            //context first
            using (BufferedGraphicsContext bgc = new BufferedGraphicsContext())
            {
                //then buffer graphics
                using (BufferedGraphics bg = bgc.Allocate(CreateGraphics(), this.ClientRectangle))
                {
                    //if game is over show dead screen
                    if (_bGameOver && !_bWelcome && !_bGameStart)
                    {
                        GameOver();
                        GameOverScreen(bg.Graphics);
                    }

                    //if game not start show welcome screen
                    else if (!_bGameOver && _bWelcome && !_bGameStart)
                    {
                        // restart process
                        WelcomeScreen(bg.Graphics);
                    }

                    //else let's play
                    else if (!_bGameOver && !_bWelcome && _bGameStart)
                    {
                        //clear the back buffer, and fill with black
                        bg.Graphics.Clear(Color.Black);

                        ShowInforOnCanvas(bg.Graphics);

                        //if ship is dead -> restart game
                        if (_Airship.IsMarkedForDeath)
                        {
                            RestartGame();
                        }

                        //else keep showing ship
                        else
                        {
                            _Airship.Tick(ClientSize);
                            _Airship.Render(bg.Graphics);
                            _Thruster.Render(bg.Graphics);
                            _Airship.ShieldTick();

                            //if shield is on, mark a circle around the ship
                            if (_Airship.GotShield)
                                MarkCircle(_Airship.Location, bg.Graphics, _Airship.GetPath(), Color.MediumVioletRed, _Airship);

                            //check collision of ship-rocks, bullet-rocks
                            CheckCollision(_Airship, _lstRock, bg.Graphics);
                        }

                        //tick "move" each shape
                        _lstRock.ForEach(x => x.Tick(ClientSize));
                        _lstBullet.ForEach(x => x.Tick(ClientSize));

                        //extra bullet tick for distance removal
                        _lstBullet.ForEach(x => x.BulletTick());

                        //extra tick for rotation angle and color                            
                        _lstRock.ForEach(s => s.ExtraTick());

                        //render all shapes, lock bc render takes longer time
                        //than add and remove objects
                        lock (_oLock)
                        {
                            _lstRock.ForEach(x => x.Render(bg.Graphics));
                            _lstBullet.ForEach(x => x.Render(bg.Graphics));
                        }

                        //loop bc many bullet on screen not like ship
                        foreach (var bullet in _lstBullet)
                            CheckCollision(bullet, _lstRock, bg.Graphics);

                        //remove all the collided shapes
                        _lstBullet.RemoveAll(x => x.IsMarkedForDeath);
                        _lstRock.RemoveAll(x => x.IsMarkedForDeath);
                    }

                    //smooth display
                    bg.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                    //flip/move the back buffer to display
                    bg.Render();

                    //combine the new broken rock list with old collections
                    _lstRock = _lstRock.Union(_lstBrokenRock).ToList();
                }
            }
        }

        /// <summary>
        /// Function to mark colliding shapes with color circles
        /// </summary>
        /// <param name="pointF">center of the shape</param>
        /// <param name="gr">graphics canvas it is inside</param>
        /// <param name="gp">graphicpath of it</param>
        /// <param name="color">color of the circle</param>
        private void MarkCircle(PointF pointF, Graphics gr, GraphicsPath gp, Color color, ShapeBase s)
        {
            //clone the path
            GraphicsPath gpClone = (GraphicsPath)gp.Clone();

            //add ellipse to that clone path, around the og shape
            gpClone.AddEllipse(pointF.X - s.Radius, pointF.Y - s.Radius, s.Radius * 2, s.Radius * 2);

            //draw it on the board
            gr.DrawPath(new Pen(color), gpClone);
        }

        /// <summary>
        /// Show game infos on canvas
        /// </summary>
        /// <param name="gr">current canvas</param>
        private void ShowInforOnCanvas(Graphics gr)
        {
            //display life at the corner
            for (int i = 0; i < _UserInfo._Life; i++)
            {
                DisplayImg(gr, Properties.Resources.ship, new PointF(0 + i * Properties.Resources.ship.Width, 0));
            }

            //build a font
            Font font = new Font("Curlz MT", 10, GraphicsUnit.Millimeter);

            //display current point
            gr.DrawString(_UserInfo._CurrentScore.ToString("g5"), font, new SolidBrush(Color.Orange), this.Width - 300, 0);

            //display current weapon and game level
            font = new Font("Curlz MT", 6, GraphicsUnit.Millimeter);

            gr.DrawString(ShapeBullet.bulletType.ToString(), font, new SolidBrush(Color.Orange), 0, 20);
            gr.DrawString(_gameLevel.ToString(), font, new SolidBrush(Color.YellowGreen), 0, 40);

            //location of ship on form            
            this.Text = $"Ship Location: {_Airship.Location.ToString()} - Speed: " + _Airship._fSpeed.ToString("G3")
                + $" - Rock: {_lstRock.Count}";
        }

        /// <summary>
        /// Helper to check the collision of shapes on canvas
        /// </summary>
        /// <typeparam name="T">accepted any type</typeparam>
        /// <param name="s1">shape to check</param>
        /// <param name="listShapes">collection of ROCK to check with the main shape</param>
        /// <param name="gr">current canvas</param>
        /// <returns>flag of collision</returns>
        private bool CheckCollision<T>(ShapeBase s1, List<T> listShapes, Graphics gr)
        {
            //declare a flag
            bool bCollided = false;

            //loop through the collection to check collision
            foreach (var t2 in listShapes)
            {
                //convert t2 to rock type
                if (!(t2 is ShapeBase s2))
                    return false;

                //the dist btw 2 shapes is less than the sum of their radius -> removing both
                if (!ReferenceEquals(s1, s2) && ShapeBase.Dist(s1, s2) < (s1.Radius + s2.Radius))
                {
                    //mark the checking shapes removal
                    //extra check for ship, only remove ship if rock is fully appear
                    //and ship doesn't have the shield
                    if (s1 is ShapeShip)
                    {
                        if (s2.IsFading && !((ShapeShip)s1).GotShield)
                        {
                            s1.IsMarkedForDeath = true;
                        }
                    }

                    //if not, mark ship for removal
                    else
                    {
                        s1.IsMarkedForDeath = true;
                    }

                    //break down the shape in the rock list, if rock hit bullet ONLY
                    if (s1 is ShapeBullet)
                    {
                        //literally
                        PlaySound(_soundExplosion);

                        //mark rock for removal
                        s2.IsMarkedForDeath = true;

                        //update score based on rock size
                        UpdateScore(s2 as ShapeRock);

                        //break this rock based on its size to smaller rocks
                        BreakDownRock(s2 as ShapeRock);

                        //break 1 rock only
                        break;
                    }
                }
            }

            //return the accepted result
            return bCollided;
        }

        /// <summary>
        /// Helper to break down the input rock by size
        /// </summary>
        /// <param name="rockOG">input rock</param>
        private void BreakDownRock(ShapeRock rockOG)
        {
            //clear the current collection, make sure no repeatition
            _lstBrokenRock.Clear();

            //if this rock is big, break it to 3 smaller rocks
            if (rockOG._rockSize == ShapeRock.RockSize.Big)
            {
                //count
                for (int i = 0; i < 3; ++i)
                {
                    //make a new rock
                    ShapeRock rock = new ShapeRock(rockOG.Location, ShapeRock.RockSize.Medium);

                    //make all the broken rocks same direction, use QUADRANT
                    //use sign of original rock
                    float fX_OG = (float)Math.Ceiling(rockOG._fXSpeed);
                    float fY_OG = (float)Math.Ceiling(rockOG._fYSpeed);

                    //check sign of orignal rock, if make the smaller ones same sign
                    if (rockOG._fXSpeed >= 0)
                    {
                        rock._fXSpeed = (float)(fX_OG + _rnd.NextDouble() * fX_OG);
                    }
                    else
                    {
                        rock._fXSpeed = -(float)(fX_OG + _rnd.NextDouble() * fX_OG);
                    }

                    //check sign of orignal rock, if make the smaller ones same sign
                    if (rockOG._fYSpeed >= 0)
                    {
                        rock._fYSpeed = (float)(fY_OG + _rnd.NextDouble() * fY_OG);
                    }
                    else
                    {
                        rock._fYSpeed = -(float)(fY_OG + _rnd.NextDouble() * fY_OG);
                    }

                    //set the rock increment
                    rock._fRotationIncrement = (float)(_rnd.NextDouble() * 5.0 - 2.5);

                    //set this rock active
                    rock.IsMarkedForDeath = false;

                    //use lock?
                    lock (_oLock)
                    {
                        //add new rock to the sub list, combine to main list later
                        _lstBrokenRock.Add(rock);
                    }
                }
            }

            //if this rock is medium, break it to 2 smaller rocks
            else if (rockOG._rockSize == ShapeRock.RockSize.Medium)
            {
                //count
                for (int i = 0; i < 2; ++i)
                {
                    //make a new rock
                    ShapeRock rock = new ShapeRock(rockOG.Location, ShapeRock.RockSize.Small);

                    //make all the broken rocks same direction, use QUADRANT
                    //use sign of original rock
                    float fX_OG = (float)Math.Ceiling(rockOG._fXSpeed);
                    float fY_OG = (float)Math.Ceiling(rockOG._fYSpeed);

                    //check sign of orignal rock, if make the smaller ones same sign
                    if (rockOG._fXSpeed >= 0)
                    {
                        rock._fXSpeed = (float)(fX_OG + _rnd.NextDouble() * fX_OG);
                    }
                    else
                    {
                        rock._fXSpeed = -(float)(fX_OG + _rnd.NextDouble() * fX_OG);
                    }

                    //check sign of orignal rock, if make the smaller ones same sign
                    if (rockOG._fYSpeed >= 0)
                    {
                        rock._fYSpeed = (float)(fY_OG + _rnd.NextDouble() * fY_OG);
                    }
                    else
                    {
                        rock._fYSpeed = -(float)(fY_OG + _rnd.NextDouble() * fY_OG);
                    }

                    //set the rock increment
                    rock._fRotationIncrement = (float)(_rnd.NextDouble() * 5.0 - 2.5);

                    //set this rock active
                    rock.IsMarkedForDeath = false;

                    lock (_oLock)
                    {
                        //add new rock to the sub list, combine to main list later
                        _lstBrokenRock.Add(rock);
                    }
                }
            }
        }

        /// <summary>
        /// Helper to update score everytime bullet hit the rock
        /// </summary>
        /// <param name="rock">input rock</param>
        private void UpdateScore(ShapeRock rock)
        {
            //use rock instance property to update
            _UserInfo._CurrentScore += rock.RScore;

            //if current score mul by 10K -> award player a life
            if ((int)_UserInfo._CurrentScore / AWARD_LIFE > _UserInfo._AwardsTimes)
            {
                _UserInfo._AwardsTimes++;
                _UserInfo._Life++;
            }

            //increase level when user get 20k, and 100k
            if (_UserInfo._AwardsTimes == (int)_MediumScore / 10000)
                _gameLevel = eGameLevel.Medium;

            if (_UserInfo._AwardsTimes == (int)_HardScore / 10000)
                _gameLevel = eGameLevel.Hard;
        }

        /// <summary>
        /// Helper run everytime ship is resurrected
        /// </summary>
        private void RestartGame()
        {
            //bc ship is dead, reset the game
            _lstBullet.Clear();

            //reset flags
            _bGameOver = false;
            _bGameStart = true;
            _bWelcome = false;

            //red screen for 1 tick
            Graphics gr = CreateGraphics();
            gr.Clear(Color.OrangeRed);
            DisplayImg(gr, Properties.Resources.blast, new PointF(_Airship.Location.X - Properties.Resources.blast.Width / 2,
                _Airship.Location.Y - Properties.Resources.blast.Height / 2));

            //play the explosion sound
            PlaySound(_soundGotHit);

            //decrease life
            _UserInfo._Life--;

            //if no life -> GAME OVER
            if (_UserInfo._Life < 3)
            {
                _bGameOver = true;
                _bGameStart = false;
                _bWelcome = false;
            }

            //else make another ship
            else
            {
                //add main ship after a delay, async?
                MakeAShip(3000, ClientSize);
            }
        }

        /// <summary>
        /// Welcome screen at the beginning of the game
        /// </summary>
        /// <param name="gr">current canvas</param>
        private void WelcomeScreen(Graphics gr)
        {
            //display on canvas
            gr.Clear(Color.Black);

            //build a font and display the texts
            Font tmpfont = new Font("Curlz MT", 20, GraphicsUnit.Millimeter);
            gr.DrawString("ASTEROIDS RETRO", tmpfont, new SolidBrush(Color.Azure), new PointF(ClientSize.Width / 2 - 325, ClientSize.Height / 2 - 200));

            tmpfont = new Font("Showcard Gothic", 20, FontStyle.Underline);
            gr.DrawString("Champion Table", tmpfont, new SolidBrush(Color.OrangeRed), new PointF(ClientSize.Width / 2 - 110, ClientSize.Height / 2 - 76));

            tmpfont = new Font("Impact", 20, FontStyle.Underline);
            gr.DrawString("Press Space to Play", tmpfont, new SolidBrush(Color.GreenYellow), new PointF(ClientSize.Width / 2 - 120, ClientSize.Height / 2 + 75));

            tmpfont = new Font("Jing Jing", 12, FontStyle.Underline);
            gr.DrawString("Instructions", tmpfont, new SolidBrush(Color.White), new PointF(ClientSize.Width / 2 - 70, ClientSize.Height / 2 + 150));

            tmpfont = new Font("Jing Jing", 12);
            gr.DrawString("F1 to show Help!", tmpfont, new SolidBrush(Color.White), new PointF(ClientSize.Width / 2 - 85, ClientSize.Height / 2 + 175));

            //show score from saved file
            int i = 0;
            tmpfont = new Font("Showcard Gothic", 16);
            foreach (var score in _lstHighScores)
            {
                gr.DrawString(score.ToString(), tmpfont, new SolidBrush(Color.White), new PointF(ClientSize.Width / 2 - 50, ClientSize.Height / 2 - (25 * i) + 5));

                i++;
            }
        }

        /// <summary>
        /// Helper run when user runs out of lives
        /// </summary>
        private void GameOver()
        {
            //check if player made a new record or not
            if (_lstHighScores.Count < 3 || _UserInfo._CurrentScore > _lstHighScores[_lstHighScores.Count - 1])
            {
                //add new score to the end of the list
                _lstHighScores.Add(_UserInfo._CurrentScore);

                //sort the list
                _lstHighScores = _lstHighScores.OrderByDescending(x => x).ToList();

                //if the list is more than 3 record remove until fit
                if (_lstHighScores.Count > 3)
                {
                    //copy the top 3 scores
                    _lstHighScores = _lstHighScores.GetRange(0, 3);
                }

                //save record
                try
                {
                    FileStream fs = new FileStream(_ScoreFilePath, FileMode.Create, FileAccess.Write);
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, _lstHighScores);
                    fs.Close();
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            //reset parameters
            _bGameStart = false;
            _bGamePause = false;
            _bWelcome = false;
            _UserInfo = new UserInfo();
            _lstRock.Clear();
            _lstBrokenRock.Clear();
            _lstBullet.Clear();
            _lstHighScores.Clear();

            //ensure respawn timer does not carry over to next game
            timerRender.Dispose();
        }

        /// <summary>
        /// Death screen
        /// </summary>
        /// <param name="gr">current canvas</param>
        private void GameOverScreen(Graphics gr)
        {
            //display on canvas
            gr.Clear(Color.Black);

            //play Yuna song
            PlaySound(_soundGameOver);

            //form text
            Text = "Goodbye Warrior... See you again...";

            //build a font and displat the text
            Font tmpfont = new Font("Curlz MT", 20, GraphicsUnit.Millimeter);
            gr.DrawString("GAME OVER", tmpfont, new SolidBrush(Color.DarkRed), new PointF(ClientSize.Width / 2 - 210, ClientSize.Height / 2 - 100));
        }

        /// <summary>
        /// Occur when user press key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //set proper flags based on user key pressed
            if (e.KeyCode == System.Windows.Forms.Keys.Up)
                _bUp = true;

            if (e.KeyCode == System.Windows.Forms.Keys.Down)
                _bDown = true;

            if (e.KeyCode == System.Windows.Forms.Keys.Left)
                _bLeft = true;

            if (e.KeyCode == System.Windows.Forms.Keys.Right)
                _bRight = true;

            //set thurster while airship is alive
            if (e.KeyCode == System.Windows.Forms.Keys.Up && !_Airship.IsMarkedForDeath)
                _Thruster.IsThruster = true;

            //if user press SPACEBAR start game
            if (e.KeyCode == System.Windows.Forms.Keys.Space)
            {
                if (_bGameOver && !_bGameStart && !_bWelcome)
                {
                    _bGameStart = false;
                    _bWelcome = true;
                    _bGameOver = false;

                    InitGame();
                }


                else if (_bWelcome && !_bGameStart & !_bGameOver)
                {
                    _bGameOver = false;
                    _bWelcome = false;
                    _bGameStart = true;
                }

                timerRender.Start();

            }

            //ESC -> pause/un-pause game
            if (e.KeyCode == System.Windows.Forms.Keys.Escape)
            {
                if (_bGamePause)
                {
                    _bGamePause = false;
                    timerRender.Start();
                }
                else
                {
                    _bGamePause = true;
                    timerRender.Stop();
                }
            }

            //show help box
            if (e.KeyCode == System.Windows.Forms.Keys.F1)
                _HelpBox.Show();

            //set new game level
            if (e.KeyCode == System.Windows.Forms.Keys.D1)
            {
                _gameLevel = eGameLevel.Easy;

                //call make asteroid to update the change
                MakeAsteroid((int)_gameLevel, _gr.VisibleClipBounds.Size);
            }

            else if (e.KeyCode == System.Windows.Forms.Keys.D2)
            {
                _gameLevel = eGameLevel.Medium;

                //call make asteroid to update the change
                MakeAsteroid((int)_gameLevel, _gr.VisibleClipBounds.Size);
            }

            else if (e.KeyCode == System.Windows.Forms.Keys.D3)
            {
                _gameLevel = eGameLevel.Hard;

                //call make asteroid to update the change
                MakeAsteroid((int)_gameLevel, _gr.VisibleClipBounds.Size);
            }

            //if user press SPACEBAR, also add bullets in ALLOWED LIMIT
            if (e.KeyCode == System.Windows.Forms.Keys.Space && _lstBullet.Count < ShapeBullet.BULLET_ALLOW)
            {
                ShapeBullet newBullet = new ShapeBullet(_Airship.Location, _Airship);
                _lstBullet.Add(newBullet);
                PlaySound(_soundLazer);
            }
        }

        /// <summary>
        /// Occur when user release key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //un-set correspondent flags
            if (e.KeyCode == System.Windows.Forms.Keys.Up)
                _bUp = false;

            if (e.KeyCode == System.Windows.Forms.Keys.Up && !_Airship.IsMarkedForDeath)
                _Thruster.IsThruster = false;

            if (e.KeyCode == System.Windows.Forms.Keys.Down)
                _bDown = false;

            if (e.KeyCode == System.Windows.Forms.Keys.Left)
                _bLeft = false;

            if (e.KeyCode == System.Windows.Forms.Keys.Right)
                _bRight = false;

            //if (e.KeyCode == System.Windows.Forms.Keys.Space)
            //_bSpace = false;
        }

        /// <summary>
        /// FORM CTOR
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            //set initial color
            ShapeRock.BaseColor = Color.LightSlateGray;
            ShapeShip.BaseColor = Color.GreenYellow;

        }

        /// <summary>
        /// Occur when Form load, show Welcome screen
        /// set flags, ship, rock, music
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            //call init Game
            InitGame();
        }

        /// <summary>
        /// Helper setup start game state
        /// </summary>
        private void InitGame()
        {
            //show modeless box
            _HelpBox = new frmInfos();
            this.Location = new Point(10, 10);
            _HelpBox.Show();

            //set helpbox location
            _HelpBox.Location = new Point(this.Location.X + this.Width + 10, 10);

            //preset flag
            _bGameStart = false;
            _bGameOver = false;
            _bGamePause = false;
            _bWelcome = true;

            //reset state
            _UserInfo = new UserInfo();
            _gameLevel = eGameLevel.Easy;
            this.Focus();

            //add main ship
            MakeAShip(0, ClientSize);

            //add one rock
            Point center = new Point(_rnd.Next(ClientRectangle.Width), _rnd.Next(ClientRectangle.Height));
            ShapeRock newRock = new ShapeRock(center, (ShapeRock.RockSize)_rnd.Next(3));
            _lstRock.Add(newRock);

            //call make color function
            //make a random known color every second
            MakeColor(1000);

            //make rock based on selected level
            //add asteroids on canvas based on game level
            _gr = CreateGraphics();
            MakeAsteroid((int)_gameLevel, _gr.VisibleClipBounds.Size);

            //play opening sound
            _SoundFolder = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "Sound"));
            _soundOpening = _SoundFolder + "\\opening.wav";
            _soundExplosion = _SoundFolder + "\\explosion-012.wav";
            _soundGameOver = _SoundFolder + "\\Yunas-Theme.wav";
            _soundGotHit = _SoundFolder + "\\explosion.wav";
            _soundLazer = _SoundFolder + "\\lazer.wav";

            PlaySound(_soundOpening);

            //load images
            //LoadImages();

            //play hot sound
            PlaySound(_soundOpening);

            // Helper to load existing high score
            LoadFile();
        }

        /// <summary>
        /// Helper to load existing high score
        /// </summary>
        private void LoadFile()
        {
            //don't reload
            if(_lstHighScores.Count > 0)
                return;

            //set score file path
            _ScoreFilePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "Resources\\AsteroidScore.bin"));

            //if the high score file existed
            if (File.Exists(_ScoreFilePath))
            {
                //open record
                try
                {
                    FileStream fs = new FileStream(_ScoreFilePath, FileMode.Open, FileAccess.Read);
                    BinaryFormatter bf = new BinaryFormatter();

                    //load the data to the list
                    _lstHighScores = bf.Deserialize(fs) as List<long>;

                    fs.Close();
                }

                //catch any error
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        ///// <summary>
        ///// Helper to set link to image files
        ///// </summary>
        //private void LoadImages()
        //{
        //    string shipFolder = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "Images"));

        //    _ShipImg = new Bitmap(shipFolder + "\\ship.png");

        //    _ExplodeImg = new Bitmap(shipFolder + "\\blast.png");

        //    _BonusImg = new Bitmap(shipFolder + "\\bonus.png");
        //}

        /// <summary>
        /// Helper to display image
        /// </summary>
        /// <param name="gr">current canvas</param>
        /// <param name="image">image</param>
        /// <param name="pointF">center of image</param>
        private void DisplayImg(Graphics gr, Image image, PointF pointF)
        {
            gr.DrawImage(image, pointF);
        }

        /// <summary>
        /// Occur when form is resized, not used
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Resize(object sender, EventArgs e)
        {
            //CreateGraphics();
        }

        /// <summary>
        /// Mother function of async method that create random color in specific time
        /// </summary>
        /// <param name="time">input time</param>
        public async void MakeColor(int time)
        {
            //use await call
            await Task.Run(() => RandomColor(time));

            //run again after each success call
            MakeColor(time);
        }

        /// <summary>
        /// Child function of async method that create random color in specific time
        /// </summary>
        /// <param name="time">input time</param>
        private void RandomColor(int time)
        {
            //delay inside method, use stopwatch to get exact timing
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            while (sw.ElapsedMilliseconds < time)
                ;

            //random number -> random color
            ShapeRock.BaseColor = RandomColorLib.RandColor();
        }


        /// <summary>
        /// Mother function of async method that 
        /// call function to create a random rock in specific time
        /// </summary>
        /// <param name="time">input time</param>
        /// <param name="size">canvas size</param>
        public async void MakeAsteroid(int time, SizeF size)
        {
            //use await call
            await Task.Run(() => MakeARock(time, size));

            //time based on level
            //recall itself when successfully called
            MakeAsteroid(time, size);
        }


        /// <summary>
        /// Child function of async method that create a random rock in specific time
        /// </summary>
        /// <param name="time">input time</param>
        /// <param name="size"></param>
        private void MakeARock(int time, SizeF size)
        {
            //delay inside method, use stopwatch to get exact timing
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            while (sw.ElapsedMilliseconds < time)
                ;

            //random location
            PointF center = new PointF((float)(_rnd.NextDouble() * size.Width), (float)(_rnd.NextDouble() * size.Height));
            lock (_oLock)
            {
                ShapeRock newRock = new ShapeRock(center, (ShapeRock.RockSize)_rnd.Next(1, 4));
                _lstRock.Add(newRock);
            }
        }

        /// <summary>
        /// Child function of async method that create a ship in specific time
        /// </summary>
        /// <param name="time">input time</param>
        /// <param name="size"></param>
        private void MakeAShip(int time, SizeF size)
        {
            //delay inside method, use stopwatch to get exact timing
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            while (sw.ElapsedMilliseconds < time)
                ;

            //random location
            Point center = new Point(ClientRectangle.Width / 2, ClientRectangle.Height / 2);
            lock (_oLock)
            {
                _Airship = new ShapeShip(center);
                _Airship.IsMarkedForDeath = false;

                _Thruster = new ShapeThruster(_Airship.Location, _Airship);
            }
        }

        /// <summary>
        /// Helper to play sound in a new thread
        /// </summary>
        /// <param name="fullPath">file path</param>
        private void PlaySound(string fullPath)
        {
            //make a new thread
            m_SoundThread = new Thread(SoundThread);
            m_SoundThread.IsBackground = true;
            m_SoundThread.Start(fullPath);
        }

        /// <summary>
        /// Main sound function
        /// </summary>
        /// <param name="oFullPath">file path</param>
        private void SoundThread(object oFullPath)
        {
            //convert path from object
            string fullPath = oFullPath.ToString();

            //try to play the file
            try
            {
                //set location, load, then play
                //avoid flickering
                _soundEffect = new SoundPlayer(fullPath);
                _soundEffect.SoundLocation = fullPath;
                _soundEffect.Load();
                _soundEffect.Play();
            }

            //catch any err
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
