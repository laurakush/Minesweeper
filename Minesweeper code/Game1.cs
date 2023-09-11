using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Helper;
using Animation2D; 

namespace Minesweeper
{
    public class Game1 : Game
    {

        static Random rng = new Random();

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        static StreamWriter outFile;
        static StreamReader inFile;

        string filePath = "scoresAndLevel.txt";

        const int INSTRUCTIONS = 0;
        const int GAMEPLAY = 1;
        const int ENDGAME = 2;

        int gameState = INSTRUCTIONS;

        const int EASY = 0;
        const int MED = 1;
        const int HARD = 2;

        int level = EASY;

        const int NUM_LEVELS = 3; 

        const int NON_BOARD = -1;
        const int MINE = 9;

        const int HUD_HEIGHT = 60;

        const int EASY_WIDTH = 450;
        const int EASY_HEIGHT = 360;

        const int EASY_ROW = 8;
        const int EASY_COL = 10;
        const int EASY_MINES = 10;

        const int MED_WIDTH = 540;
        const int MED_HEIGHT = 420;

        const int MED_ROW = 14;
        const int MED_COL = 18;
        const int MED_MINES = 40;

        const int HARD_WIDTH = 600;
        const int HARD_HEIGHT = 500;

        const int HARD_ROW = 20;
        const int HARD_COL = 24;
        const int HARD_MINES = 99;

        const int EASY_PIX = 45;
        const int MED_PIX = 30;
        const int HARD_PIX = 25;

        const float TRANSPERENCY = 0.75f;

        int[] curRow = {EASY_ROW, MED_ROW, HARD_ROW};
        int[] curCol = {EASY_COL, MED_COL, HARD_COL};

        int[] curPix = {EASY_PIX, MED_PIX, HARD_PIX};

        int[] curNumMines = {EASY_MINES, MED_MINES, HARD_MINES};

        int[,] tiles;

        int [] curConsoleHeight = {EASY_HEIGHT + HUD_HEIGHT, MED_HEIGHT + HUD_HEIGHT, HARD_HEIGHT + HUD_HEIGHT};
        int [] curConsoleWidth = {EASY_WIDTH, MED_WIDTH, HARD_WIDTH};

        double targetInsTime = 3000;
        Timer instructionsTimer;

        double targetScoreTime = 999000;
        Timer scoreTimer;

        double [] highScores = {0, 0, 0};
        double curScore; 

        bool isIns1Shown = true;

        bool isDropDown;

        bool[,] isTilePressed;
        bool[,] isFlagPressed;

        bool[,] isWrongFlagPressed;

        bool isGameWon;

        bool isSoundOn = true;
        bool isLargeClear = false; 

        int tilePressedCounter = 0;

        int flagCounter;

        int instructionDimension;

        int exitImgCompress;

        int[,] mineColours;

        int tileColour = 1;

        Vector2[] flagLoc = new Vector2[NUM_LEVELS];
        Vector2 flagTextLoc;
        Vector2 watchLoc;
        Vector2 scoreTimeLoc;

        Vector2 noCurScoreLoc;
        Vector2 noHighScoreLoc;
        Vector2 curScoreLoc;
        Vector2 highScoreLoc;

        Vector2 [] checkLoc = new Vector2[NUM_LEVELS]; 

        MouseState prevMouse;
        MouseState mouse;

        SpriteFont HUDFont;

        Texture2D[] bgImg = new Texture2D[NUM_LEVELS];
        Rectangle[] bgRec = new Rectangle[NUM_LEVELS];

        Texture2D lightTileImg;
        Texture2D darkTileImg;
        Texture2D curTileImg;

        Texture2D HUDImg;
        Texture2D flagImg;
        Texture2D watchImg;

        Rectangle HUDRec;
        Rectangle flagHUDRec;
        Rectangle watchRec;

        Texture2D exitImg;
        Rectangle exitRec;

        Texture2D instructions1Img;
        Texture2D instructions2Img;
        Rectangle instructionsRec;

        Texture2D dropDownImg;
        Texture2D checkImg; 
        Texture2D[] levelImgs = new Texture2D[NUM_LEVELS];

        Rectangle dropDownRec;
        Rectangle checkRec; 
        Rectangle[] levelRecs = new Rectangle[NUM_LEVELS];
        Rectangle[] curLevelRec = new Rectangle[NUM_LEVELS];

        Rectangle[,] tilesRec;

        Texture2D[] numImgs = new Texture2D[8]; 

        Texture2D[] mineImgs = new Texture2D[8];

        Texture2D gameShadowImg;
        Rectangle gameShadowRec;

        Texture2D gameOverWinImg;
        Texture2D gameOverLoseImg;
        Texture2D winReplayImg;
        Texture2D loseReplayImg;

        Rectangle gameOverRec;
        Rectangle replayRec;

        Texture2D noTimeImg;
        Rectangle noTimeRec1;
        Rectangle noTimeRec2;

        Texture2D xImg;

        Texture2D soundOffImg;
        Texture2D soundOnImg;
        Rectangle soundRec; 

        SoundEffect placeFlagSnd; 
        SoundEffect clearFlagSnd;
        SoundEffect smallClearSnd;
        SoundEffect largeClearSnd;
        SoundEffect mineSnd;

        Song winMusic;
        Song loseMusic;

        Texture2D explosionImg;
        Animation [,] explosionAnim;
        Vector2 [,] explosionLoc;

        bool [,] isAnimationStarted; 

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }


        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }


        protected override void LoadContent()
        {

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            instructionsTimer = new Timer(targetInsTime, true);
            scoreTimer = new Timer(targetScoreTime, false);

            flagLoc[EASY] = new Vector2(135, 6);
            flagLoc[MED] = new Vector2(175, flagLoc[EASY].Y);
            flagLoc[HARD] = new Vector2(205, flagLoc[EASY].Y);

            HUDFont = Content.Load<SpriteFont>("Fonts/HUDFont");

            lightTileImg = Content.Load<Texture2D>("Images/Sprites/Clear_Light");
            darkTileImg = Content.Load<Texture2D>("Images/Sprites/Clear_Dark");

            exitImg = Content.Load<Texture2D>("Images/Sprites/Exit");

            HUDImg = Content.Load<Texture2D>("Images/Backgrounds/HUDBar");
            flagImg = Content.Load<Texture2D>("Images/Sprites/flag");
            watchImg = Content.Load<Texture2D>("Images/Sprites/Watch");

            bgImg[EASY] = Content.Load<Texture2D>("Images/Backgrounds/board_easy");
            bgImg[MED] = Content.Load<Texture2D>("Images/Backgrounds/board_med");
            bgImg[HARD] = Content.Load<Texture2D>("Images/Backgrounds/board_hard");

            bgRec[EASY] = new Rectangle(0, HUD_HEIGHT, bgImg[EASY].Width, bgImg[EASY].Height);
            bgRec[MED] = new Rectangle(0, HUD_HEIGHT, bgImg[MED].Width, bgImg[MED].Height);
            bgRec[HARD] = new Rectangle(0, HUD_HEIGHT, bgImg[HARD].Width, bgImg[HARD].Height);

            instructions1Img = Content.Load<Texture2D>("Images/Sprites/Instructions1");
            instructions2Img = Content.Load<Texture2D>("Images/Sprites/Instructions2");

            exitImgCompress = (int)(exitImg.Height * 0.25);
            instructionDimension = (int)(instructions1Img.Height * 0.03);

            numImgs[0] = Content.Load<Texture2D>("Images/Sprites/1");
            numImgs[1] = Content.Load<Texture2D>("Images/Sprites/2");
            numImgs[2] = Content.Load<Texture2D>("Images/Sprites/3");
            numImgs[3] = Content.Load<Texture2D>("Images/Sprites/4");
            numImgs[4] = Content.Load<Texture2D>("Images/Sprites/5");
            numImgs[5] = Content.Load<Texture2D>("Images/Sprites/6");
            numImgs[6] = Content.Load<Texture2D>("Images/Sprites/7");
            numImgs[7] = Content.Load<Texture2D>("Images/Sprites/8");

            mineImgs[0] = Content.Load<Texture2D>("Images/Sprites/Mine1");
            mineImgs[1] = Content.Load<Texture2D>("Images/Sprites/Mine2");
            mineImgs[2] = Content.Load<Texture2D>("Images/Sprites/Mine3");
            mineImgs[3] = Content.Load<Texture2D>("Images/Sprites/Mine4");
            mineImgs[4] = Content.Load<Texture2D>("Images/Sprites/Mine5");
            mineImgs[5] = Content.Load<Texture2D>("Images/Sprites/Mine6");
            mineImgs[6] = Content.Load<Texture2D>("Images/Sprites/Mine7");
            mineImgs[7] = Content.Load<Texture2D>("Images/Sprites/Mine8");

            gameShadowImg = Content.Load<Texture2D>("Images/Backgrounds/GameOverBoardShadow");

            gameOverWinImg = Content.Load<Texture2D>("Images/Sprites/GameOver_WinResults");
            gameOverLoseImg = Content.Load<Texture2D>("Images/Sprites/GameOver_Results");
            winReplayImg = Content.Load<Texture2D>("Images/Sprites/GameOver_PlayAgain");
            loseReplayImg = Content.Load<Texture2D>("Images/Sprites/GameOver_TryAgain");

            noTimeImg = Content.Load<Texture2D>("Images/Sprites/GameOver_NoTime");

            dropDownImg = Content.Load<Texture2D>("Images/Sprites/DropDown");
            checkImg = Content.Load<Texture2D>("Images/Sprites/Check");
            levelImgs[EASY] = Content.Load<Texture2D>("Images/Sprites/EasyButton");
            levelImgs[MED] = Content.Load<Texture2D>("Images/Sprites/MedButton");
            levelImgs[HARD] = Content.Load<Texture2D>("Images/Sprites/HardButton");

            xImg = Content.Load<Texture2D>("Images/Sprites/X");

            soundOnImg = Content.Load<Texture2D>("Images/Sprites/SoundOn");
            soundOffImg = Content.Load<Texture2D>("Images/Sprites/SoundOff");

            placeFlagSnd = Content.Load<SoundEffect>("Audio/Sounds/PlaceFlag");
            clearFlagSnd = Content.Load<SoundEffect>("Audio/Sounds/ClearFlag");
            smallClearSnd = Content.Load<SoundEffect>("Audio/Sounds/SmallClear");
            largeClearSnd = Content.Load<SoundEffect>("Audio/Sounds/LargeClear");
            mineSnd = Content.Load<SoundEffect>("Audio/Sounds/Mine");

            SoundEffect.MasterVolume = 1f;

            winMusic = Content.Load<Song>("Audio/Music/Win");
            loseMusic = Content.Load<Song>("Audio/Music/Lose");

            MediaPlayer.Volume = 1f;
            MediaPlayer.IsRepeating = true;

            explosionImg = Content.Load<Texture2D>("Images/Sprites/Explode2");

            ReadFile();
            LevelSpecifcLoad();

            AssignNonBoard(curRow[level], curCol[level], tiles);
            RandomizeMines(curRow[level], curCol[level], curNumMines[level], tiles);
            CountMinesAdjacent(curRow[level], curCol[level], tiles);

        }


        private void LevelSpecifcLoad()
        {

            _graphics.PreferredBackBufferWidth = curConsoleWidth[level];
            _graphics.PreferredBackBufferHeight = curConsoleHeight[level];
            _graphics.ApplyChanges();

            exitRec = new Rectangle(curConsoleWidth[level] - exitImgCompress - 20, HUD_HEIGHT / 2 - exitImgCompress / 2 - 5, exitImgCompress, exitImgCompress);
            soundRec = new Rectangle(exitRec.X - exitImgCompress - 20, exitRec.Y, exitRec.Width, exitRec.Height); 

            flagTextLoc = new Vector2(flagLoc[level].X + EASY_PIX - 3, flagLoc[level].Y);
            watchLoc = new Vector2(flagTextLoc.X + EASY_PIX + 5, flagTextLoc.Y);
            scoreTimeLoc = new Vector2(watchLoc.X + EASY_PIX, flagTextLoc.Y);

            HUDRec = new Rectangle(0, 0, curConsoleWidth[level], HUD_HEIGHT);
            flagHUDRec = new Rectangle((int)flagLoc[level].X, (int)flagLoc[level].Y, (int)(flagImg.Width * 0.38), (int)(flagImg.Height * 0.38));
            watchRec = new Rectangle((int)watchLoc.X, (int)watchLoc.Y, (int)(watchImg.Width * 1 / 3), (int)(watchImg.Height * 1 / 3));

            instructionsRec = new Rectangle(curConsoleWidth[level] / 2 - instructionDimension / 2, curConsoleHeight[level] / 2 - instructionDimension / 2,
            instructionDimension, instructionDimension);

            tiles = new int[curRow[level] + 2, curCol[level] + 2];
            tilesRec = new Rectangle[curRow[level] + 2, curCol[level] + 2];
            mineColours = new int[curRow[level] + 2, curCol[level] + 2];

            isTilePressed = new bool[curRow[level] + 2, curCol[level] + 2];
            isFlagPressed = new bool[curRow[level] + 2, curCol[level] + 2];

            isWrongFlagPressed = new bool[curRow[level] + 2, curCol[level] + 2];

            isAnimationStarted = new bool[curRow[level] + 2, curCol[level] + 2];

            flagCounter = curNumMines[level];

            explosionLoc = new Vector2[curRow[level] + 2, curCol[level] + 2];
            explosionAnim = new Animation[curRow[level] + 2, curCol[level] + 2];

            for (int row = 1; row < curRow[level] + 1; row++)
            {
                for (int col = 1; col < curCol[level] + 1; col++)
                {
                    tilesRec[row, col] = new Rectangle(curPix[level] * (col - 1), curPix[level] * (row - 1) + HUD_HEIGHT, curPix[level], curPix[level]);

                    switch(level)
                    {
                        case EASY:
                            explosionLoc[row, col] = new Vector2(tilesRec[row, col].X - EASY_PIX / 2, tilesRec[row, col].Y - EASY_PIX / 2);
                            break;
                        default:
                            explosionLoc[row, col] = new Vector2(tilesRec[row, col].X - EASY_PIX + 10, tilesRec[row, col].Y - EASY_PIX + 10);
                            break;
                    }
                    explosionAnim[row, col] = new Animation(explosionImg, 5, 4, 20, 0, Animation.NO_IDLE, Animation.ANIMATE_ONCE, 2, explosionLoc[row, col], 0.5f, false);
                }
            }


            gameShadowRec = new Rectangle(0, 0, curConsoleWidth[level], curConsoleHeight[level]);

            gameOverRec = new Rectangle(curConsoleWidth[level] / 2 - gameOverWinImg.Width / 2, curConsoleHeight[level] / 2 -
                (gameOverWinImg.Height + winReplayImg.Height + 10) / 2, gameOverWinImg.Width, gameOverWinImg.Height);
            replayRec = new Rectangle(gameOverRec.X, gameOverRec.Y + gameOverWinImg.Height + 10, winReplayImg.Width, winReplayImg.Height);

            noCurScoreLoc = new Vector2(gameOverRec.X + 56, gameOverRec.Y + gameOverWinImg.Height / 2);
            noHighScoreLoc = new Vector2(noCurScoreLoc.X + 133, noCurScoreLoc.Y);
            curScoreLoc = new Vector2(noCurScoreLoc.X, noCurScoreLoc.Y - 20);
            highScoreLoc = new Vector2(noHighScoreLoc.X, curScoreLoc.Y);

            noTimeRec1 = new Rectangle((int)noCurScoreLoc.X, (int)noCurScoreLoc.Y, noTimeImg.Width, noTimeImg.Height);
            noTimeRec2 = new Rectangle((int)noHighScoreLoc.X, (int)noHighScoreLoc.Y, noTimeImg.Width, noTimeImg.Height);

            for (int i = 0; i < curLevelRec.Length; i++)
            {
                curLevelRec[i] = new Rectangle(7, HUD_HEIGHT / 2 - levelImgs[i].Height / 2, levelImgs[i].Width, levelImgs[i].Height);
            }

            dropDownRec = new Rectangle(curLevelRec[level].X, curLevelRec[level].Y + levelImgs[level].Height, dropDownImg.Width, dropDownImg.Height);

            for (int i = 0; i < levelRecs.Length; i++)
            {
                levelRecs[i] = new Rectangle(dropDownRec.X, (int)(dropDownRec.Y + i * (1 / 3.0) * (dropDownRec.Height)), dropDownImg.Width, (int)(dropDownImg.Height * 1 / 3.0));
            }

            checkLoc[EASY] = new Vector2(levelRecs[level].X + 10, levelRecs[level].Y + (levelRecs[level].Height / 2 - checkImg.Height/2));
            checkLoc[MED] = new Vector2(checkLoc[EASY].X, checkLoc[EASY].Y - 8);
            checkLoc[HARD] = new Vector2(checkLoc[EASY].X, checkLoc[MED].Y - 2);

            checkRec = new Rectangle((int)checkLoc[level].X, (int)checkLoc[level].Y, checkImg.Width, checkImg.Height);

        }


        protected override void Update(GameTime gameTime)
        {

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);

            GeneralUpdate(gameTime); 

            switch (gameState)
            {
                case INSTRUCTIONS:
                    UpdateInstructions(gameTime); 
                    break;
                case GAMEPLAY:
                    UpdateGamePlay(gameTime); 
                    break;
                case ENDGAME:
                    UpdateEndGame(); 
                    break; 
            }

        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
            _spriteBatch.Begin();

            switch (gameState)
            {
                case INSTRUCTIONS:
                    DrawInstructions(); 
                    break;
                case GAMEPLAY:
                    DrawGamePlay();
                    break;
                case ENDGAME:
                    DrawEndGame();
                    break;
            }
            GeneralDraw(); 

            _spriteBatch.End();

        }


        private void GeneralUpdate(GameTime gameTime)
        {

            prevMouse = mouse;
            mouse = Mouse.GetState();

            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed && exitRec.Contains(mouse.Position))
            {
                WriteFile();
                Exit();
            }

            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed && soundRec.Contains(mouse.Position))
            {
                isSoundOn = !isSoundOn;
            }

            for (int row = 1; row < curRow[level] + 1; row++)
            {
                for (int col = 1; col < curCol[level] + 1; col++)
                {
                    explosionAnim[row, col].Update(gameTime);
                }
            }

        }


        private void UpdateInstructions(GameTime gameTime)
        {

            instructionsTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            if(instructionsTimer.IsFinished()) 
            {
                isIns1Shown = !isIns1Shown;
                instructionsTimer.ResetTimer(true); 
            }

            CheckLeftClick();
            CheckRightClick();
            CheckLevelChange();

        }


        private void UpdateGamePlay(GameTime gameTime)
        {

            scoreTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            CheckLeftClick();
            CheckExplosionCompletion();

            CheckRightClick();

            CheckLevelChange();


            if (scoreTimer.IsFinished())
            {
                isGameWon = false;
                gameState = ENDGAME;
            }

            if (tilePressedCounter == (curRow[level] * curCol[level]) - curNumMines[level])
            {
                isGameWon = true;
                curScore = (int)(scoreTimer.GetTimePassed() / 1000);

                if (curScore < highScores[level] || highScores[level] == 0)
                {
                    highScores[level] = curScore;

                }

                WriteFile();

                gameState = ENDGAME;
            }

        }


        private void UpdateEndGame()
        {

            if (isSoundOn)
            {
                if (isGameWon)
                {
                    if (MediaPlayer.State != MediaState.Playing)
                    {
                        MediaPlayer.Play(winMusic);
                    }
                }
                else
                {
                    if (MediaPlayer.State != MediaState.Playing)
                    {
                        MediaPlayer.Play(loseMusic);
                    }
                }
            }
            else
            {
                MediaPlayer.Stop(); 
            }

            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                if (replayRec.Contains(mouse.Position))
                {
                    ResetGame();
                    MediaPlayer.Stop();
                    gameState = GAMEPLAY;
                }
            }

        }


        private void GeneralDraw()
        {

            _spriteBatch.Draw(exitImg, exitRec, Color.White);
            if (isSoundOn == true)
            {
                _spriteBatch.Draw(soundOnImg, soundRec, Color.White);
            }
            else
            {
                _spriteBatch.Draw(soundOffImg, soundRec, Color.White);
            }

        }


        private void DrawInstructions()
        {
            
            DrawBG();

            if (isIns1Shown == true)
            {
                _spriteBatch.Draw(instructions1Img, instructionsRec, Color.White * TRANSPERENCY);
            }
            else
            {
                _spriteBatch.Draw(instructions2Img, instructionsRec, Color.White * TRANSPERENCY);
            }

            DrawTiles();
            DrawFlags();
            DrawHUDDetails();
        }


        private void DrawGamePlay()
        {
            DrawBG();
            DrawTiles();
            DrawFlags();
            DrawHUDDetails();

            for (int row = 1; row < curRow[level] + 1; row++)
            {
                for (int col = 1; col < curCol[level] + 1; col++)
                {
                    explosionAnim[row, col].Draw(_spriteBatch, Color.White, Animation.FLIP_NONE);
                }
            }

            
        }


        private void DrawEndGame()
        {

            DrawBG();
            DrawTiles();
            DrawFlags();
            DrawHUDDetails();
            _spriteBatch.Draw(gameShadowImg, gameShadowRec, Color.White * TRANSPERENCY);

            if (isGameWon)
            {
                _spriteBatch.Draw(gameOverWinImg, gameOverRec, Color.White);
                _spriteBatch.Draw(winReplayImg, replayRec, Color.White);
                _spriteBatch.DrawString(HUDFont, ("" + curScore).PadLeft(3, '0'), curScoreLoc, Color.White);
            }
            else
            {
                _spriteBatch.Draw(gameOverLoseImg, gameOverRec, Color.White);
                _spriteBatch.Draw(loseReplayImg, replayRec, Color.White);
                _spriteBatch.Draw(noTimeImg, noTimeRec1, Color.White);
            }

            if (highScores[level] == 0)
            {
                _spriteBatch.Draw(noTimeImg, noTimeRec2, Color.White);
            }
            else
            {
                _spriteBatch.DrawString(HUDFont, ("" + highScores[level]).PadLeft(3, '0'), highScoreLoc, Color.White);
            }

        }


        private void DrawBG()
        {

            _spriteBatch.Draw(HUDImg, HUDRec, Color.White);
            _spriteBatch.Draw(bgImg[level], bgRec[level], Color.White);

        }


        private void DrawTiles()
        {

            for (int row = 1; row < curRow[level] + 1; row++)
            {
                for (int col = 1; col < curCol[level] + 1; col++)
                {
                    if (tileColour > 0)
                    {
                        curTileImg = lightTileImg;
                    }
                    else
                    {
                        curTileImg = darkTileImg;
                    }

                    if (isTilePressed[row, col] == true)
                    {
                        _spriteBatch.Draw(curTileImg, tilesRec[row, col], Color.White);

                        if (tiles[row, col] > 0 && tiles[row, col] < MINE)
                        {
                            _spriteBatch.Draw(numImgs[tiles[row, col] - 1], tilesRec[row, col], Color.White);
                        }
                        else if (tiles[row, col] == MINE)
                        {
                            _spriteBatch.Draw(mineImgs[mineColours[row, col]], tilesRec[row, col], Color.White);
                        }
   
                    }

                    tileColour *= -1;
                }
                tileColour *= -1;
            }

        }


        private void DrawFlags()
        {

            for (int row = 1; row < curRow[level] + 1; row++)
            {
                for (int col = 1; col < curCol[level] + 1; col++)
                {
                    if (isFlagPressed[row, col] == true)
                    {
                        _spriteBatch.Draw(flagImg, tilesRec[row, col], Color.White);
                    }

                    if(isWrongFlagPressed[row, col ]== true && isFlagPressed[row, col] == false)
                    {
                        _spriteBatch.Draw(xImg, tilesRec[row, col], Color.White);
                    }
                }
            }

        }


        private void DrawHUDDetails()
        {

            _spriteBatch.Draw(flagImg, flagHUDRec, Color.White);
            _spriteBatch.DrawString(HUDFont, ""+flagCounter, flagTextLoc, Color.White);
            _spriteBatch.Draw(watchImg, watchRec, Color.White);
            _spriteBatch.DrawString(HUDFont, (""+(int)(scoreTimer.GetTimePassed()/1000)).PadLeft(3, '0'), scoreTimeLoc, Color.White);
            _spriteBatch.Draw(levelImgs[level], curLevelRec[level], Color.White);

            if (isDropDown == true)
            {
                _spriteBatch.Draw(dropDownImg, dropDownRec, Color.White);
                _spriteBatch.Draw(checkImg, checkRec, Color.White); 
            }

        }


        private void AssignNonBoard(int rowNum, int colNum, int[,] tiles)
        {

            for(int i = 0; i < colNum+2; i++)
            {
                tiles[0, i] = NON_BOARD;
                tiles[rowNum + 1, i] = NON_BOARD;
            }

            for(int i = 0; i < rowNum+2; i++)
            {
                tiles[i, 0] = NON_BOARD;
                tiles[i, colNum + 1] = NON_BOARD; 
            }
            
        }


        private void RandomizeMines(int rowNum, int colNum, int numMines, int[,] tiles)
        {

            for(int i = 0; i < numMines; i++)
            {
                int row = rng.Next(1, rowNum + 1);
                int col = rng.Next(1, colNum + 1);

                if(tiles[row, col] == MINE)
                {
                    while(tiles[row, col] == MINE)
                    {

                        row = rng.Next(1, rowNum + 1);
                        col = rng.Next(1, colNum + 1);

                    }
                }

                tiles[row, col] = MINE;
                mineColours[row, col] = rng.Next(0, mineImgs.Length); 
            }

        }


        private void CountMinesAdjacent(int rowNum, int colNum, int[,] tiles)
        {

            for(int row = 1; row < rowNum+1; row++)
            {
                for(int col = 1; col < colNum+1; col++)
                {
                    int mineCounter = 0;

                    if(tiles[row,col] != MINE)
                    {
                        if(tiles[row-1, col-1] == MINE)
                        {
                            mineCounter++; 
                        }

                        if (tiles[row - 1, col] == MINE)
                        {
                            mineCounter++;
                        }

                        if (tiles[row - 1, col + 1] == MINE)
                        {
                            mineCounter++;
                        }

                        if (tiles[row, col + 1] == MINE)
                        {
                            mineCounter++;
                        }

                        if (tiles[row + 1, col + 1] == MINE)
                        {
                            mineCounter++;
                        }

                        if (tiles[row + 1, col] == MINE)
                        {
                            mineCounter++;
                        }

                        if (tiles[row + 1, col - 1] == MINE)
                        {
                            mineCounter++;
                        }

                        if (tiles[row, col - 1] == MINE)
                        {
                            mineCounter++;
                        }

                        tiles[row, col] = mineCounter; 
                    }
                }
            }

        }


        private void CheckLeftClick()
        {

            for (int row = 1; row < curRow[level] + 1; row++)
            {
                for (int col = 1; col < curCol[level] + 1; col++)
                {
                    if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed && tilesRec[row, col].Contains(mouse.Position) && isDropDown != true && isFlagPressed[row, col] != true)
                    {
                        if (tiles[row, col] != MINE)
                        {
                            ZeroTileRecurse(row, col, curRow[level], curCol[level]);

                            if (isSoundOn)
                            {
                                if (isLargeClear)
                                {
                                    largeClearSnd.CreateInstance().Play();
                                }
                                else
                                {
                                    smallClearSnd.CreateInstance().Play();
                                }

                                isLargeClear = false; 
                            }
                        }
                        else
                        {
                            if(isSoundOn)
                            {
                                mineSnd.CreateInstance().Play();

                            }

                            ActivateAllMines();

                            isGameWon = false;

                        }

                        if (gameState == INSTRUCTIONS)
                        {
                            scoreTimer.Activate(); 
                            gameState = GAMEPLAY; 
                        }
                    }
                }
            }

        }


        private void CheckRightClick()
        {

            for (int row = 1; row < curRow[level] + 1; row++)
            {
                for (int col = 1; col < curCol[level] + 1; col++)
                {
                    if (mouse.RightButton == ButtonState.Pressed && prevMouse.RightButton != ButtonState.Pressed)  
                    {
                        if (tilesRec[row, col].Contains(mouse.Position) && isTilePressed[row, col] != true)
                        {
                            if(isFlagPressed[row,col] == true)
                            {
                                if(isWrongFlagPressed[row, col] == true)
                                {
                                    isWrongFlagPressed[row, col] = false; 
                                }

                                isFlagPressed[row, col] = false;
                                flagCounter++;

                                if (isSoundOn)
                                {
                                    clearFlagSnd.CreateInstance().Play();
                                }
                            }
                            else
                            {
                                if(tiles[row, col] != MINE)
                                {
                                    isWrongFlagPressed[row, col] = true; 
                                }

                                isFlagPressed[row, col] = true;
                                flagCounter--;

                                if (isSoundOn)
                                {
                                    placeFlagSnd.CreateInstance().Play();
                                }
                            }
                        }
                    }
                }
            }

        }


        private void CheckLevelChange()
        {

            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                if (curLevelRec[level].Contains(mouse.Position))
                {
                    isDropDown = !isDropDown; 
                }

                if(isDropDown == true)
                {
                    for (int i = 0; i < levelRecs.Length; i++)
                    {
                        if (levelRecs[i].Contains(mouse.Position))
                        {
                            isDropDown = false;

                            if (i != level)
                            {
                                level = i;
                                LevelSpecifcLoad();
                                ResetGame();

                            }
                            break;
                        }

                    }
                }

            }

         }


        private void ZeroTileRecurse(int row, int col, int maxRow, int maxCol)
        {

            if (row < 1 || row > maxRow || col < 1 || col > maxCol)
            {

            }
            else if (tiles[row, col] != 0)
            {

                if(isTilePressed[row, col] != true && isFlagPressed[row, col] != true)
                {
                    tilePressedCounter++;
                    isTilePressed[row, col] = true;
                }

            }
            else
            {

                isLargeClear = true;

                if (isTilePressed[row, col] != true && isFlagPressed[row, col] != true)
                {
                    tilePressedCounter++;
                    isTilePressed[row, col] = true;
                }

                for (int r = -1; r < 2; r++)
                {
                    for (int c = -1; c < 2; c++)
                    {
                        if (!(r == 0 && c == 0) && isTilePressed[row + r, col + c] == false)
                        {
                            ZeroTileRecurse(row + r, col + c, maxRow, maxCol);
                        }

                    }
                }

            }

        }


        private void ActivateAllMines()
        {

            for (int row = 1; row < curRow[level] + 1; row++)
            {
                for (int col = 1; col < curCol[level] + 1; col++)
                {
                    if (tiles[row, col] == MINE && isTilePressed[row, col] == false && isFlagPressed[row, col] == false)
                    {
                        isTilePressed[row, col] = true;

                        if (explosionAnim[row, col].isAnimating == false && isAnimationStarted[row, col] != true)
                        {
                            isAnimationStarted[row, col] = true; 
                            explosionAnim[row, col].isAnimating = true; 
                        }

                    }

                    if(isWrongFlagPressed[row, col] == true)
                    {
                        isFlagPressed[row, col] = false; 
                    }
                }
            }

        }


        private void CheckExplosionCompletion()
        {

            for (int row = 1; row < curRow[level] + 1; row++)
            {
                for (int col = 1; col < curCol[level] + 1; col++)
                {
                    if(isAnimationStarted[row, col] == true)
                    {
                        if(explosionAnim[row, col].isAnimating == false)
                        {
                            gameState = ENDGAME;
                        }

                    }
                }
            }

        }


        private void ResetGame()
        {

            flagCounter = curNumMines[level]; 
            tilePressedCounter = 0;

            ResetBool2DArray(isTilePressed);
            ResetBool2DArray(isFlagPressed);
            ResetBool2DArray(isWrongFlagPressed);
            ResetBool2DArray(isAnimationStarted);

            ResetInt2DArray(mineColours);
            ResetInt2DArray(tiles);

            AssignNonBoard(curRow[level], curCol[level], tiles);
            RandomizeMines(curRow[level], curCol[level], curNumMines[level], tiles);
            CountMinesAdjacent(curRow[level], curCol[level], tiles);

            scoreTimer.ResetTimer(true);

            for (int row = 1; row < curRow[level] + 1; row++)
            {
                for (int col = 1; col < curCol[level] + 1; col++)
                {
                    explosionAnim[row,col].isAnimating = false;
                }
            }

        }


        private void ResetBool2DArray(bool [,] array)
        {

            for(int row = 0; row < array.GetLength(0); row++)
            {
                for(int col = 0; col < array.GetLength(1); col++)
                {
                    array[row, col] = false; 
                }
            }

        }


        private void ResetInt2DArray(int [,] array)
        {

            for (int row = 0; row < array.GetLength(0); row++)
            {
                for (int col = 0; col < array.GetLength(1); col++)
                {
                    array[row, col] = -2;
                }
            }

        }


        private void ReadFile()
        {
            try
            {

                if(File.Exists(filePath))
                {

                    inFile = File.OpenText(filePath);

                    for(int i = 0; i < NUM_LEVELS; i++)
                    {
                        Double.TryParse(inFile.ReadLine(), out highScores[i]);
                    }

                    Int32.TryParse(inFile.ReadLine(), out level);

                }
                else
                {

                    level = EASY;
                    
                }

            }
            catch(FileNotFoundException fnf)
            {
                Console.WriteLine("Error: " + fnf.Message); 
            }
            catch (FormatException fe)
            {
                Console.WriteLine("Error: " + fe.Message);
            }
            catch (IndexOutOfRangeException ie)
            {
                Console.WriteLine("Error: " + ie.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            finally
            {
                if(inFile != null)
                {
                    inFile.Close(); 
                }
            }

        }


        private void WriteFile()
        {

            try
            {

                outFile = File.CreateText(filePath);
                for (int i = 0; i < highScores.Length; i++)
                {
                    outFile.WriteLine(highScores[i]); 
                }

                outFile.WriteLine(level);

            }
            catch (FileNotFoundException fnf)
            {
                Console.WriteLine("Error: " + fnf.Message);
            }
            catch (IndexOutOfRangeException ie)
            {
                Console.WriteLine("Error: " + ie.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            finally
            {
                if(outFile != null)
                {
                    outFile.Close(); 
                }
            }

        }

    }
}
