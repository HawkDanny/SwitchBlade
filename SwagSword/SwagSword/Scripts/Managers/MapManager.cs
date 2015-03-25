﻿#region Using statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
//using Microsoft.Xna.Framework.GamerServices;
#endregion

//Names: Ryan Bell

namespace SwagSword
{
    /// <summary>
    /// Will draw the map, and hold a reference to it to be used by other classes such as spawn manager
    /// </summary>
    public class MapManager : Manager
    {
        #region Fields
        Tile[,] map;                                                        //This is the base level map. It is a 2d array of Tile objects
                                                                            //  -It holds only the arrangment of pathways and not pahtways
        int tileSize;                                                       //The size in pixels of one tile (width and height)
        int mapWidth;                                                       //This is the map width in tiles. It should be odd if possible
        int mapHeight;                                                      //Map height in tiles. It should also be odd if possible
        int circleRadius;
        List<List<Tile>> branches;
        List<Tile> leftBranch;
        List<Tile> rightBranch;
        List<Tile> topBranch;
        List<Tile> lowerBranch;
        Tile leftCenter;
        Tile rightCenter;
        Tile topCenter;
        Tile lowerCenter;
        int strongholdWidth;
        int strongholdHeight;
        Texture2D canvas;
        #endregion

        public int TileSize { get { return tileSize; } }
        public int MapWidth { get { return mapWidth; } }
        public int MapHeight { get { return mapHeight; } }

        /// <summary>
        /// Default Constructor sets up reference to the main manager
        /// </summary>
        /// <param name="mainMan"></param>
        public MapManager(Game1 mainMan)
            : base(mainMan)
        {

        }

        /// <summary>
        /// Method that is called upon instantiation of this class.
        /// (occurs before content is loaded)
        /// assigns values to basic fields
        /// </summary>
        public override void Init()
        {
            tileSize = 64;
            mapWidth = 91;
            mapHeight = 59;
            circleRadius = 6;
            map = new Tile[mapWidth, mapHeight];
            leftBranch = new List<Tile>();
            rightBranch = new List<Tile>();
            topBranch = new List<Tile>();
            lowerBranch = new List<Tile>();
            branches = new List<List<Tile>>();
            branches.Add(leftBranch);
            branches.Add(rightBranch);
            branches.Add(topBranch);
            branches.Add(lowerBranch);
            strongholdWidth = tileSize * 3;
            strongholdHeight = tileSize * 3;
            canvas = new Texture2D(mainMan.GraphicsDevice, 64, 64);
        }


        /// <summary>
        /// This is called right after content is loaded in game1.cs
        /// It is used to carry out functions that should occur before anything is drawn
        /// It begins the process of the creation of the base map
        /// </summary>
        public void Startup()
        {
            PopulateMap();
            generateSimplePath();
            BoldenMap();
            BoldenMap();
            GenerateCircles();
        }

        /// <summary>
        /// Fills the map with Tiles that have notPathway textures and appropiatly calculated centers
        /// </summary>
        void PopulateMap()
        {
            for(int x = 0; x < mapWidth; x++)                                                   //Iterate through the grid in x
                for (int y = 0; y < mapHeight; y++)                                             //and y direction
                {
                    //at the current position in the map grid,
                    //assign a new tile with the 'notPathway' texture.
                    map[x, y] = new Tile(mainMan.DrawMan.NotPathwayTexture, new Point(x, y), this);
                }


            for (int x = 0; x < mapWidth; x++)                                                   //Iterate through the grid in x
                for (int y = 0; y < mapHeight; y++)                                             //and y direction
                {
                    if (x > 0)
                    {
                        map[x, y].Left = map[x - 1, y];
                        if(y > 0)
                        {
                            map[x, y].DUL = map[x - 1, y - 1];
                        }
                        if (y < mapHeight - 1)
                        {
                            map[x, y].DLL = map[x - 1, y + 1];
                        }
                    }
                    if (x < mapWidth - 1)
                    {
                        map[x, y].Right = map[x + 1, y];
                        if (y > 0)
                        {
                            map[x, y].DUR = map[x + 1, y - 1];
                        }
                        if (y < mapHeight - 1)
                        {
                            map[x, y].DLR = map[x + 1, y + 1];
                        }
                    }
                    if (y > 0)
                    {
                        map[x, y].Top = map[x, y - 1];
                    }
                    if (y < mapHeight - 1)
                    {
                        map[x, y].Lower = map[x, y + 1];
                    }
                    //Console.WriteLine(x + " , " + y +" , " + map[x, y].ToString());
                }
            foreach (Tile t in map)
            {
                t.FormGroups();
            }

        }

        /// <summary>
        /// this method provides some simple distance calculation 2 points
        /// used with the center points of 2 tiles
        /// </summary>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <returns></returns>
        protected double CalcDistance(Point a0, Point a1)
        {
            return Math.Sqrt(Math.Pow(a1.X - a0.X, 2) + Math.Pow(a1.Y - a0.Y, 2));              //the distance formula
        }


        void generateSimplePath()
        {
            Random rand = mainMan.Rnd;
            Tile origin = map[mapWidth / 2, mapHeight / 2];
            origin.Texture = mainMan.DrawMan.PathwayTexture;
            Tile subject = map[mapWidth / 2, mapHeight / 2 - 1];
            bool atEdge = false;
            //top branch
            for (int i = 0; !atEdge; i++)
            {
                subject.Texture = mainMan.DrawMan.PathwayTexture;
                int choice = 0;
                if (subject.GroupTop.Count > 1)
                    choice = rand.Next(subject.GroupTop.Count - 1);
                if (subject.GroupTop.Count > 0)
                    subject = subject.GroupTop.ElementAt(choice);
                topBranch.Add(subject);
                if (subject.Center.Y <= circleRadius * tileSize)
                    atEdge = true;
            }

            topCenter = subject;
            atEdge = false;
            subject = origin;
            //lower branch
            for (int i = 0; !atEdge; i++)
            {
                subject.Texture = mainMan.DrawMan.PathwayTexture;
                int choice = 0;
                if (subject.GroupLower.Count > 1)
                    choice = rand.Next(subject.GroupLower.Count - 1);
                if (subject.GroupLower.Count > 0)
                    subject = subject.GroupLower.ElementAt(choice);
                lowerBranch.Add(subject);
                if (subject.Center.Y >= (mapHeight - circleRadius) * tileSize)
                    atEdge = true;

            }
            lowerCenter = subject;
            atEdge = false;
            subject = origin;
            //left Branch
            for (int i = 0; !atEdge; i++)
            {
                subject.Texture = mainMan.DrawMan.PathwayTexture;
                int choice = 0;
                if (subject.GroupLeft.Count > 1)
                    choice = rand.Next(subject.GroupLeft.Count - 1);
                if (subject.GroupLeft.Count > 0)
                    subject = subject.GroupLeft.ElementAt(choice);
                leftBranch.Add(subject);
                if (subject.Center.X <= circleRadius * tileSize)
                    atEdge = true;

            }
            leftCenter = subject;
            atEdge = false;
            subject = origin;
            //right branch
            for (int i = 0; !atEdge; i++)
            {
                subject.Texture = mainMan.DrawMan.PathwayTexture;
                int choice = 0;
                if (subject.GroupRight.Count > 1)
                    choice = rand.Next(subject.GroupRight.Count - 1);
                if (subject.GroupRight.Count > 0)
                    subject = subject.GroupRight.ElementAt(choice);
                rightBranch.Add(subject);
                if (subject.Center.X >= (mapWidth - circleRadius) * tileSize)
                    atEdge = true;
            }
            rightCenter = subject;
        }

        protected void GenerateCircles()
        {
            foreach (Tile t in map)
            {
                if(CalcDistance(t.Center, leftCenter.Center) <= circleRadius * tileSize)
                {
                    t.Texture = mainMan.DrawMan.PathwayTexture;
                }
                if (CalcDistance(t.Center, rightCenter.Center) <= circleRadius * tileSize)
                {
                    t.Texture = mainMan.DrawMan.PathwayTexture;
                }
                if (CalcDistance(t.Center, topCenter.Center) <= circleRadius * tileSize)
                {
                    t.Texture = mainMan.DrawMan.PathwayTexture;
                }
                if (CalcDistance(t.Center, lowerCenter.Center) <= circleRadius * tileSize)
                {
                    t.Texture = mainMan.DrawMan.PathwayTexture;
                }

            }
        }



        /// <summary>
        /// This method runs through the map and widens the paths.
        /// It adds tiles down and to the right to work with all 
        /// four branches and the differing directions
        /// </summary>
        protected void BoldenMap()
        {
            for (int x = mapWidth - 2; x >= 0; x--)                                                  //start at the lower right corner and iterate
            {                                                                                       //  up to the upper left corner 
                for (int y = mapHeight - 1; y >= 0; y--)                                            //...
                {
                    if (map[x, y].Texture == mainMan.DrawMan.PathwayTexture)                         //check that the current tile is a pathway
                    {
                        if (x < mapWidth + 2)                                                       //check bounds to avoid null exception
                        {
                            map[x + 1, y].Texture = mainMan.DrawMan.PathwayTexture;                 //make right tile a path as well
                            if (y < mapHeight - 2)                                                  //check bounds to avoid null exception
                            {
                                map[x + 1, y + 1].Texture = mainMan.DrawMan.PathwayTexture;         //make lower right tile a path as well
                            }
                        }
                        if (y < mapHeight - 2)                                                      //check bounds to avoid null exception
                        {
                            map[x, y + 1].Texture = mainMan.DrawMan.PathwayTexture;                 //make the lower tile a path as well
                        }

                    }
                }
            }
        }

        //**************************************************************************************************************************

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Tile t in map)
            {
                Rectangle r = new Rectangle(t.Center.X - tileSize / 2, t.Center.Y - tileSize / 2,  tileSize, tileSize);
                
                spriteBatch.Draw(t.Texture, r, Color.White);
            }
            spriteBatch.Draw(mainMan.DrawMan.Stronghold, new Rectangle(leftCenter.Center.X - strongholdWidth / 2, leftCenter.Center.Y - strongholdHeight / 2, strongholdWidth, strongholdHeight), Color.White);
            spriteBatch.Draw(mainMan.DrawMan.Stronghold, new Rectangle(rightCenter.Center.X - strongholdWidth / 2, rightCenter.Center.Y - strongholdHeight / 2, strongholdWidth, strongholdHeight), Color.White);
            spriteBatch.Draw(mainMan.DrawMan.Stronghold, new Rectangle(topCenter.Center.X - strongholdWidth / 2, topCenter.Center.Y - strongholdHeight / 2, strongholdWidth, strongholdHeight), Color.White);
            spriteBatch.Draw(mainMan.DrawMan.Stronghold, new Rectangle(lowerCenter.Center.X - strongholdWidth / 2, lowerCenter.Center.Y - strongholdHeight / 2, strongholdWidth, strongholdHeight), Color.White);
            spriteBatch.Draw(canvas, new Rectangle(64, 64, 64, 64), Color.White);
        }






    }
}