using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class Form1 : Form
    {
        List<Snake> snake = new List<Snake>();
        Snake bait = new Snake();
        
        //SET GAME BOARD DIMENSIONS 
        const int WIDTH = 24;
        const int HEIGHT = 24;
        const string SCORE = "points.txt";

        bool gameOver = false;
        int score = 0;
        int speed = 150; //MOVEMENT SPEED
        Direction direction;
           
        public Form1()
        {
            InitializeComponent();
            fps.Interval = speed;     
            fps.Tick += new EventHandler(ChangeDirections); //THINK OF IT AS FRAMES PER SECOND
            fps.Start();
            Start();
        }

        //CREATING THE GAME INTERFACE (GAME OVER, SCORE, AND SNAKE)
        private void GameInterface(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;
            Font font = this.Font;
            font = new Font("Arial", 24, FontStyle.Bold);

            if (gameOver)
            {
                string displayScore = "GAME OVER! YOU EARNED " + score.ToString() + " POINTS!";
                
                //CUSTOMIZING TEXT
                int centerText = Board.Width / 2;
                SizeF points = canvas.MeasureString(displayScore, font);
                PointF txtLocation = new PointF(centerText - points.Width / 2, 300);
                
                //Display the text
                canvas.DrawString(displayScore, font, Brushes.White, txtLocation);
                points = canvas.MeasureString(displayScore, font);
            }
            else
            {
                //DISPLAY SCORE
                string displayScore = "SCORE: " + score.ToString();
                font = new Font("Arial", 14, FontStyle.Bold);

                int centerText = Board.Width / 2;
                PointF txtLocation = new PointF(750, 15);
                canvas.DrawString(displayScore, font, Brushes.White, txtLocation);

                //SETTING BAIT COLOR
                canvas.FillRectangle(Brushes.White, new Rectangle(bait.X * WIDTH, bait.Y * HEIGHT, WIDTH, HEIGHT));

                //ADDS SQUARE TO SNAKE
                for (int i = 0; i < snake.Count; i++) 
                {
                    //SETTING SNAKE COLOR SCHEME
                    Brush colorScheme = i == 0 ? Brushes.Red : Brushes.White;
                    canvas.FillRectangle(colorScheme, new Rectangle(snake[i].X * WIDTH, snake[i].Y * HEIGHT, WIDTH, HEIGHT));
                }
            }
        }

        private void Start()
        {
            gameOver = false;
            score = 0;
            fps.Interval = speed;

            snake.Clear();
            //WHERE THE SNAKE BEGINS
            Snake head = new Snake(18, 8);
            snake.Add(head);

            //RANDOMLY PLACES BAIT
            bait.LoadBait(Board.Size.Width / WIDTH, Board.Size.Height / HEIGHT);
        }

        enum Direction
        {
            Down = 0,
            Up = 1,
            Left = 2,
            Right = 3,
        }

        private void LevelUp()
        {
            for (int i = snake.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    switch (direction)
                    {
                        case Direction.Down:
                            snake[i].Y++;
                            break;
                        case Direction.Up:
                            snake[i].Y--;
                            break;
                        case Direction.Left:
                            snake[i].X--;
                            break;
                        case Direction.Right:
                            snake[i].X++;
                            break;
                    }

                    int snakeWidth = Board.Width / WIDTH;
                    int snakeHeight = Board.Height / HEIGHT;

                    //SET BOUNDARIES - GAME OVER IF PLAYER HITS WALL
                    gameOver = snake[i].X >= snakeWidth || snake[i].Y >= snakeHeight || snake[i].X < 0 || snake[i].Y < 0;

                    if (snake[i].CompareTo(bait) == 0)
                    {
                        Snake length = new Snake(snake[snake.Count - 1].X, snake[snake.Count - 1].Y);
                        snake.Add(length);

                        //GENERATE NEW BAIT
                        bait.LoadBait(Board.Size.Width / WIDTH, Board.Size.Height / HEIGHT);
                        //MOVEMENT SPEED INCREASE AFTER EVERY BITE TAKEN
                        fps.Interval -= 5;
                        score++;
                    }
                }
                else
                {
                    //SQUARES WILL BE BEHIND EACH OTHER RATHER THAN STACKED ON TOP
                    snake[i].X = snake[i - 1].X;
                    snake[i].Y = snake[i - 1].Y;
                }
            }
        }

        private void ChangeDirections(object sender, EventArgs e)
        {
            if (gameOver)
            {
                if (SnakeGame.Controls.Pressed(Keys.Enter))
                    Start();
            }
            else
            {
                if (SnakeGame.Controls.Pressed(Keys.Right))
                {
                    if (snake.Count < 2 || snake[0].X == snake[1].X)
                        direction = Direction.Right;
                }
                else if (SnakeGame.Controls.Pressed(Keys.Left))
                {
                    if (snake.Count < 2 || snake[0].X == snake[1].X)
                        direction = Direction.Left;
                }
                else if (SnakeGame.Controls.Pressed(Keys.Up))
                {
                    if (snake.Count < 2 || snake[0].Y == snake[1].Y)
                        direction = Direction.Up;
                }
                else if (SnakeGame.Controls.Pressed(Keys.Down))
                {
                    if (snake.Count < 2 || snake[0].Y == snake[1].Y)
                        direction = Direction.Down;
                }
                LevelUp();
            }
            //REFRESH
            Board.Invalidate();
        }

        private void UpArrow(object sender, KeyEventArgs e)
        {
            SnakeGame.Controls.ChangeState(e.KeyCode, false);
        }
        private void DownArrow(object sender, KeyEventArgs e)
        {
            SnakeGame.Controls.ChangeState(e.KeyCode, true);
            fps.Stop();
            ChangeDirections(null, null);
            fps.Start();
        }
    }
}
