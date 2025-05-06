using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Game
{
    public partial class Form1 : Form
    {
        private GameManager _gameManager;
        private Button _addBirdButton;
        private const int BIRD_COUNT = 100; // Default number of birds

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            // Mark the original bird as template
            bird.Tag = "BirdTemplate";

            // Create a list of pipe pairs
            List<(Panel pipeBot, Panel pipeTop)> pipes = [(pipeBot1, pipeTop1), (pipeBot2, pipeTop2)];

            // Initialize game manager
            _gameManager = new GameManager(this, gameTimer, label1, label2);
            _gameManager.InitializeGame(bird, pipes, BIRD_COUNT); // Start with 10 birds

            // Create an "Add Bird" button
            CreateAddBirdButton();
        }

        private void CreateAddBirdButton()
        {
            _addBirdButton = new Button
            {
                Text = "Add Bird",
                Location = new Point(button1.Right + 10, button1.Top),
                Size = button1.Size,
                Enabled = false // Disabled until game starts
            };

            _addBirdButton.Click += AddBirdButton_Click;
            this.Controls.Add(_addBirdButton);
        }

        private void AddBirdButton_Click(object sender, EventArgs e)
        {
            _gameManager.AddBird(bird);
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            // Game logic is now handled by the GameManager
            // This method is left for compatibility
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            _gameManager.StartGame();
            button1.Enabled = false;
            _addBirdButton.Enabled = true; // Enable the Add Bird button when game starts
            this.Focus();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            _gameManager.HandleKeyPress(e.KeyCode);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Any additional initialization can go here
        }
    }
}