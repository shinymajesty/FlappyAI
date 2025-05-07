using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Game
{
    public partial class Form1 : Form
    {
        private GameManager _gameManager;
        private int birdCount = 100; // Default number of birds
        private List<GenomeEntry> population = [];

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            // Mark the original bird as template
            bird.Tag = "BirdTemplate";


            // Initialize game manager
            _gameManager = new GameManager(this, gameTimer, label1, label2);


        }



        private void GameTimer_Tick(object sender, EventArgs e)
        {
            // Game logic is now handled by the GameManager
            // This method is left for compatibility
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            start_button.Enabled = false;
            numericUpDown1.Enabled = false;
            this.birdCount = (int)numericUpDown1.Value;
            List<(Panel pipeBot, Panel pipeTop)> pipes = [(pipeBot1, pipeTop1), (pipeBot2, pipeTop2)];
            _gameManager.InitializeGame(bird, pipes, birdCount); // Start with 10 birds
            if (population != null && population!.Count > 0)
            {
                _gameManager.Initialize(population);
            }

            _gameManager.StartGame();
            this.Focus();
        }



        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void import_button_Click(object sender, EventArgs e)
        {
            var x = openFileDialog1.ShowDialog();
            if (x == DialogResult.Cancel)
                return;
            else if (openFileDialog1.FileName == "")
                return;
            else if (x == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                FileDAO<List<GenomeEntry>> fileDAO = new(filePath);
                var population = fileDAO.Load("population.json");
                if (population == null)
                {
                    MessageBox.Show("File not found or invalid format.");
                    return;
                }
                else
                    this.population = population;
            }
            else throw new Exception("Unknown error");


        }

        private void export_button_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            else if (saveFileDialog1.FileName == "")
            {
                MessageBox.Show("Please select a file to save.");
                return;
            }
            else
            {
                //Save the file 
                var x = _gameManager.SerializablePopulation;
                FileDAO<List<GenomeEntry>> fileDAO = new(saveFileDialog1.FileName);
                fileDAO.Save("population.flappycp", x);
            }


        }

    }
}