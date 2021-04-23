using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace FinalProjectV1
{
    public partial class Game : Form
    {
        int difficulty = 1;
        bool setupActive = true;
        int shipNumber = 1;
        int playersSunk = 0;
        int computersSunk = 0;
        string currentTarget = "none";
        string currentOrientation = "none";
        string startPoint = "none";
        List<Ship> playerShips = new List<Ship>();
        List<Ship> computerShips = new List<Ship>();
        List<string> randomGuesses = new List<string>();
        List<string> normalGuesses = new List<string>();
        List<string> impossibleGuesses = new List<string>();
        List<string> impossibleMisses = new List<string>();
        Ship patrolBoat = new Ship("Patrol Boat", 2);
        Ship submarine = new Ship("Submarine", 3);
        Ship destroyer = new Ship("Destroyer", 3);
        Ship battleship = new Ship("Battleship", 4);
        Ship carrier = new Ship("Carrier", 5);
        Ship patrolBoatC = new Ship("Patrol Boat", 2);
        Ship submarineC = new Ship("Submarine", 3);
        Ship destroyerC = new Ship("Destroyer", 3);
        Ship battleshipC = new Ship("Battleship", 4);
        Ship carrierC = new Ship("Carrier", 5);
        public Game(int dif)
        {
            InitializeComponent();
            difficulty = dif;
            playerShips.Add(patrolBoat);
            playerShips.Add(submarine);
            playerShips.Add(destroyer);
            playerShips.Add(battleship);
            playerShips.Add(carrier);
            computerShips.Add(patrolBoatC);
            computerShips.Add(submarineC);
            computerShips.Add(destroyerC);
            computerShips.Add(battleshipC);
            computerShips.Add(carrierC);
            if(difficulty == 0)
            {
                foreach(Control c in Controls)
                {
                    if (c is Button)
                    {
                        Button b = (Button)c;
                        if (b.Name[0] == 'p')
                            randomGuesses.Add(b.Name);
                    }
                }
            }
            if (difficulty == 1)
            {
                foreach (Control c in Controls)
                {
                    if (c is Button)
                    {
                        Button b = (Button)c;
                        if (b.Name[0] == 'p')
                        {
                            if ((int.Parse(b.Name[1].ToString()) % 2 != 0 & int.Parse(b.Name[2].ToString()) % 2 != 0) | (int.Parse(b.Name[1].ToString()) % 2 == 0 & int.Parse(b.Name[2].ToString()) % 2 == 0))
                                normalGuesses.Add(b.Name);
                        }
                    }
                }
            }
            PlaceEnemyShips();
            Setup();
        }
        public void Setup()
        {
            if(shipNumber == 1)
                lblCenter.Text = "Place Patrol Boat\r\nLength: 2";
            else if(shipNumber == 2)
                lblCenter.Text = "Place Submarine\r\nLength: 3";
            else if (shipNumber == 3)
                lblCenter.Text = "Place Destroyer\r\nLength: 3";
            else if (shipNumber == 4)
                lblCenter.Text = "Place Battleship\r\nLength: 4";
            else if (shipNumber == 5)
                lblCenter.Text = "Place Carrier\r\nLength: 5";
            else if (shipNumber == 6)
            {
                //textBox1.Text=(patrolBoat.locations[0] +"," +patrolBoat.locations[1] + ","+submarine.locations[0]+","+submarine.locations[1]+","+submarine.locations[2]+","+
                //    destroyer.locations[0] + "," + destroyer.locations[1] + "," + destroyer.locations[2] + "," +
                //    battleship.locations[0] + "," + battleship.locations[1] + "," + battleship.locations[2] + "," + battleship.locations[3] + "," +
                //    carrier.locations[0] + "," + carrier.locations[1] + "," + carrier.locations[2] + "," + carrier.locations[3] + "," + carrier.locations[4]).Replace('p', 'c');
                lblCenter.Text = "Start Game\r\n";
                btnRestart.Dispose();
                if(difficulty == 2)
                {
                    foreach (Control c in Controls)
                    {
                        if (c is Button)
                        {
                            Button b = (Button)c;
                            if (b.Name[0] == 'p')
                            {
                                foreach(Ship s in playerShips)
                                {
                                    if (s.locations.Contains(b.Name))
                                        impossibleGuesses.Add(b.Name);
                                }
                                if (!impossibleGuesses.Contains(b.Name))
                                    impossibleMisses.Add(b.Name);
                            }
                        }
                    }
                    ImpossibleGuess();
                }
                setupActive = false;
            }
        }
        private void btnClick(object sender, EventArgs e)
        {
            if (setupActive == true)
            {
                Button b = sender as Button;
                if (SetShip(b.Name) == true)
                {
                    b.BackColor = Color.Black;
                }
            }
        }
        private void GuessClick(object sender, EventArgs e)
        {
            if (setupActive == false)
            {
                Button b = sender as Button;
                if(b.BackColor != Color.Red)
                {
                    lblCenter.Text = "";
                    b.BackColor = Color.Red;
                    if (GuessShip(b.Name) == false)
                        lblCenter.Text += "You missed\r\n";
                    else
                    {
                        b.Text = "X";
                        if(computersSunk == 5)
                        {
                            if (difficulty == 2)
                                MessageBox.Show("Woah, that was supposed to be impossible");
                            else
                                MessageBox.Show("Player Wins!");
                            this.Close();
                            return;
                        }
                    }
                    if (difficulty == 0)
                        EasyGuess();
                    else if (difficulty == 1)
                    {
                        bool again = false;
                        while(again == false)
                            again = NormalGuess();
                    }
                    else
                        ImpossibleGuess();
                    if (playersSunk == 5)
                    {
                        MessageBox.Show("Computer Wins");
                        this.Close();
                        return;
                    }
                    lblRemain.Text = "Player ships remaining: " + (5 - playersSunk).ToString() + "\nComputer ships remaining: " + (5 - computersSunk).ToString();
                }
            }
        }
        private bool SetShip(string name)
        {
            bool add = false;
            if (shipNumber == 1)
            {
                if(patrolBoat.placed == 0)
                    add = true;
                else
                {
                    if (patrolBoat.locations[0][1] == name[1])
                    {
                        if (Math.Abs(patrolBoat.locations[0][2] - name[2]) == 1)
                            add = true;
                    }
                    else if (patrolBoat.locations[0][2] == name[2])
                    {
                        if (Math.Abs(patrolBoat.locations[0][1] - name[1]) == 1)
                            add = true;
                    }
                }
                if(add == true)
                {
                    patrolBoat.AddLocation(name);
                    patrolBoat.placed++;
                    if (patrolBoat.width == patrolBoat.placed)
                        shipNumber++;
                    Setup();
                    return true;
                }
            }
            else if (shipNumber == 2)
            {
                foreach (Ship s in playerShips)
                {
                    foreach(string st in s.locations)
                    {
                        if (st == name)
                            return false;
                    }
                }
                if (submarine.placed == 0)
                {
                    add = true;
                }
                else if (submarine.orientation == "none")
                {
                    if (submarine.locations[0][1] == name[1])
                    {
                        if (Math.Abs(submarine.locations[0][2] - name[2]) == 1)
                        {
                            add = true;
                            submarine.orientation = "horizontal";
                        } 
                    }
                    else if (submarine.locations[0][2] == name[2])
                    {
                        if (Math.Abs(submarine.locations[0][1] - name[1]) == 1)
                        {
                            add = true;
                            submarine.orientation = "vertical";
                        }  
                    }
                }
                else if (submarine.orientation == "horizontal")
                {
                    foreach(string s in submarine.locations)
                    {
                        if(s != "0")
                        {
                            if (Math.Abs(s[2] - name[2]) == 1 & Math.Abs(s[1] - name[1]) == 0)
                            {
                                add = true;
                            }
                        }
                    }
                }
                else if (submarine.orientation == "vertical")
                {
                    foreach (string s in submarine.locations)
                    {
                        if (s != "0")
                        {
                            if (Math.Abs(s[1] - name[1]) == 1 & Math.Abs(s[2] - name[2]) == 0)
                            {
                                add = true;
                            }
                        }
                    }   
                }
                if (add == true)
                {
                    submarine.AddLocation(name);
                    submarine.placed++;
                    if (submarine.width == submarine.placed)
                        shipNumber++;
                    Setup();
                    return true;
                }
            }
            else if (shipNumber == 3)
            {
                foreach (Ship s in playerShips)
                {
                    foreach (string st in s.locations)
                    {
                        if (st == name)
                            return false;
                    }
                }
                if (destroyer.placed == 0)
                {
                    add = true;
                }
                else if (destroyer.orientation == "none")
                {
                    if (destroyer.locations[0][1] == name[1])
                    {
                        if (Math.Abs(destroyer.locations[0][2] - name[2]) == 1)
                        {
                            add = true;
                            destroyer.orientation = "horizontal";
                        }
                    }
                    else if (destroyer.locations[0][2] == name[2])
                    {
                        if (Math.Abs(destroyer.locations[0][1] - name[1]) == 1)
                        {
                            add = true;
                            destroyer.orientation = "vertical";
                        }
                    }
                }
                else if (destroyer.orientation == "horizontal")
                {
                    foreach (string s in destroyer.locations)
                    {
                        if (s != "0")
                        {
                            if (Math.Abs(s[2] - name[2]) == 1 & Math.Abs(s[1] - name[1]) == 0)
                            {
                                add = true;
                            }
                        }
                    }
                }
                else if (destroyer.orientation == "vertical")
                {
                    foreach (string s in destroyer.locations)
                    {
                        if (s != "0")
                        {
                            if (Math.Abs(s[1] - name[1]) == 1 & Math.Abs(s[2] - name[2]) == 0)
                            {
                                add = true;
                            }
                        }
                    }
                }
                if (add == true)
                {
                    destroyer.AddLocation(name);
                    destroyer.placed++;
                    if (destroyer.width == destroyer.placed)
                        shipNumber++;
                    Setup();
                    return true;
                }
            }
            else if (shipNumber == 4)
            {
                foreach (Ship s in playerShips)
                {
                    foreach (string st in s.locations)
                    {
                        if (st == name)
                            return false;
                    }
                }
                if (battleship.placed == 0)
                {
                    add = true;
                }
                else if (battleship.orientation == "none")
                {
                    if (battleship.locations[0][1] == name[1])
                    {
                        if (Math.Abs(battleship.locations[0][2] - name[2]) == 1)
                        {
                            add = true;
                            battleship.orientation = "horizontal";
                        }
                    }
                    else if (battleship.locations[0][2] == name[2])
                    {
                        if (Math.Abs(battleship.locations[0][1] - name[1]) == 1)
                        {
                            add = true;
                            battleship.orientation = "vertical";
                        }
                    }
                }
                else if (battleship.orientation == "horizontal")
                {
                    foreach (string s in battleship.locations)
                    {
                        if (s != "0")
                        {
                            if (Math.Abs(s[2] - name[2]) == 1 & Math.Abs(s[1] - name[1]) == 0)
                            {
                                add = true;
                            }
                        }
                    }
                }
                else if (battleship.orientation == "vertical")
                {
                    foreach (string s in battleship.locations)
                    {
                        if (s != "0")
                        {
                            if (Math.Abs(s[1] - name[1]) == 1 & Math.Abs(s[2] - name[2]) == 0)
                            {
                                add = true;
                            }
                        }
                    }
                }
                if (add == true)
                {
                    battleship.AddLocation(name);
                    battleship.placed++;
                    if (battleship.width == battleship.placed)
                        shipNumber++;
                    Setup();
                    return true;
                }
            }
            else if (shipNumber == 5)
            {
                foreach (Ship s in playerShips)
                {
                    foreach (string st in s.locations)
                    {
                        if (st == name)
                            return false;
                    }
                }
                if (carrier.placed == 0)
                {
                    add = true;
                }
                else if (carrier.orientation == "none")
                {
                    if (carrier.locations[0][1] == name[1])
                    {
                        if (Math.Abs(carrier.locations[0][2] - name[2]) == 1)
                        {
                            add = true;
                            carrier.orientation = "horizontal";
                        }
                    }
                    else if (carrier.locations[0][2] == name[2])
                    {
                        if (Math.Abs(carrier.locations[0][1] - name[1]) == 1)
                        {
                            add = true;
                            carrier.orientation = "vertical";
                        }
                    }
                }
                else if (carrier.orientation == "horizontal")
                {
                    foreach (string s in carrier.locations)
                    {
                        if (s != "0")
                        {
                            if (Math.Abs(s[2] - name[2]) == 1 & Math.Abs(s[1] - name[1]) == 0)
                            {
                                add = true;
                            }
                        }
                    }
                }
                else if (carrier.orientation == "vertical")
                {
                    foreach (string s in carrier.locations)
                    {
                        if (s != "0")
                        {
                            if (Math.Abs(s[1] - name[1]) == 1 & Math.Abs(s[2] - name[2]) == 0)
                            {
                                add = true;
                            }
                        }
                    }
                }
                if (add == true)
                {
                    carrier.AddLocation(name);
                    carrier.placed++;
                    if (carrier.width == carrier.placed)
                        shipNumber++;
                    Setup();
                    return true;
                }
            }
            return false;
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            if(shipNumber == 1)
            {
                foreach(string s in patrolBoat.locations)
                {
                    foreach (Control c in Controls)
                    {
                        if(c is Button)
                        {
                            Button b = (Button)c;
                            if (b.Name.ToString() == s)
                            {
                                b.BackColor = Control.DefaultBackColor;
                                b.UseVisualStyleBackColor = true;
                            }
                        }
                    }
                }
                patrolBoat.locations.Clear();
                patrolBoat.placed = 0;
            }
            else if (shipNumber == 2)
            {
                foreach (string s in submarine.locations)
                {
                    foreach (Control c in Controls)
                    {
                        if (c is Button)
                        {
                            Button b = (Button)c;
                            if (b.Name.ToString() == s)
                            {
                                b.BackColor = Control.DefaultBackColor;
                                b.UseVisualStyleBackColor = true;
                            }
                        }
                    }
                }
                submarine.locations.Clear();
                submarine.placed = 0;
                submarine.orientation = "none";
            }
            else if (shipNumber == 3)
            {
                foreach (string s in destroyer.locations)
                {
                    foreach (Control c in Controls)
                    {
                        if (c is Button)
                        {
                            Button b = (Button)c;
                            if (b.Name.ToString() == s)
                            {
                                b.BackColor = Control.DefaultBackColor;
                                b.UseVisualStyleBackColor = true;
                            }
                        }
                    }
                }
                destroyer.locations.Clear();
                destroyer.placed = 0;
                destroyer.orientation = "none";
            }
            else if (shipNumber == 4)
            {
                foreach (string s in battleship.locations)
                {
                    foreach (Control c in Controls)
                    {
                        if (c is Button)
                        {
                            Button b = (Button)c;
                            if (b.Name.ToString() == s)
                            {
                                b.BackColor = Control.DefaultBackColor;
                                b.UseVisualStyleBackColor = true;
                            }
                        }
                    }
                }
                battleship.locations.Clear();
                battleship.placed = 0;
                battleship.orientation = "none";
            }
            else
            {
                foreach (string s in carrier.locations)
                {
                    foreach (Control c in Controls)
                    {
                        if (c is Button)
                        {
                            Button b = (Button)c;
                            if (b.Name.ToString() == s)
                            {
                                b.BackColor = Control.DefaultBackColor;
                                b.UseVisualStyleBackColor = true;
                            }
                        }
                    }
                }
                carrier.locations.Clear();
                carrier.placed = 0;
                carrier.orientation = "none";
            }
            return;
        }
        private void PlaceEnemyShips()
        {
            string[] placements = File.ReadAllLines("Ships.csv");
            Random random = new Random();
            int randomNumber = random.Next(0, placements.Length);
            string[] randomPlacements = placements[randomNumber].Split(',');
            patrolBoatC.locations = new List<string> { randomPlacements[0], randomPlacements[1] };
            submarineC.locations = new List<string> { randomPlacements[2], randomPlacements[3], randomPlacements[4] };
            destroyerC.locations = new List<string> { randomPlacements[5], randomPlacements[6], randomPlacements[7] };
            battleshipC.locations = new List<string> { randomPlacements[8], randomPlacements[9], randomPlacements[10], randomPlacements[11] };
            carrierC.locations = new List<string> { randomPlacements[12], randomPlacements[13], randomPlacements[14], randomPlacements[15], randomPlacements[16] };
            return;
        }
        private bool GuessShip(string name)
        {
            foreach(Ship s in computerShips)
            {
                if (s.locations.Contains(name))
                {
                    s.hits++;
                    lblCenter.Text += "You hit\r\n";
                    if (s.Sunk == true)
                    {
                        lblCenter.Text += "You sank " + s.name + "\n";
                        computersSunk++;
                    }
                    return true;
                }
            }
            return false;
        }
        private void EasyGuess()
        {
            Random random = new Random();
            int index = random.Next(0, randomGuesses.Count);
            string guess = randomGuesses[index];
            randomGuesses.Remove(guess);
            foreach (Control c in Controls)
            {
                if (c is Button)
                {
                    Button b = (Button)c;
                    if (b.Name.ToString() == guess)
                    {
                        b.BackColor = Color.Red;
                        foreach (Ship s in playerShips)
                        {
                            if (s.locations.Contains(guess))
                            {
                                b.Text = "X";
                                s.hits++;
                                lblCenter.Text += "Computer hit\r\n";
                                if (s.Sunk == true)
                                {
                                    lblCenter.Text += "Computer sank " + s.name + "\n";
                                    playersSunk++;
                                }
                                return;
                            }
                        }
                        lblCenter.Text += "Computer missed\r\n";
                        return;
                    }
                }
            }
        }
        private bool NormalGuess()
        {
            if (currentTarget == "none")
            {
                currentOrientation = "none";
                foreach (Ship s in playerShips)
                {
                    if (s.hits > 0 & s.Sunk == false)
                    {
                        currentTarget = s.name;
                        if (s.hits > 1 & s.Sunk == false)
                        {
                            if (s.locations[0][1] == s.locations[1][1])
                                currentOrientation = "horizontal";
                            else
                                currentOrientation = "verticle";
                        }
                    }
                }
            }
            if (currentTarget == "none")
            {
                Random random = new Random();
                int index = random.Next(0, normalGuesses.Count);
                string guess = normalGuesses[index];
                normalGuesses.Remove(guess);
                foreach (Control c in Controls)
                {
                    if (c is Button)
                    {
                        Button b = (Button)c;
                        if (b.Name.ToString() == guess)
                        {
                            b.BackColor = Color.Red;
                            foreach (Ship s in playerShips)
                            {
                                if (s.locations.Contains(guess))
                                {
                                    s.hits++;
                                    b.Text = "X";
                                    lblCenter.Text += "Computer hit\r\n";
                                    currentTarget = s.name;
                                    if (s.Sunk == true)
                                    {
                                        lblCenter.Text += "Computer sank " + s.name + "\n";
                                        playersSunk++;
                                        currentTarget = "none";
                                        currentOrientation = "none";
                                        startPoint = "none";
                                    }
                                    return true;
                                }
                            }
                            lblCenter.Text += "Computer missed\r\n";
                            return true;
                        }
                    }
                }
            }
            else
            {
                if(currentOrientation == "none")
                {
                    foreach(Ship s in playerShips)
                    {
                        if(s.name == currentTarget)
                        {
                            if(s.hits > 1 & s.Sunk == false)
                            {
                                if (s.locations[0][1] == s.locations[1][1])
                                    currentOrientation = "horizontal";
                                else
                                    currentOrientation = "verticle";
                            }
                        }
                    }
                }
                if(currentOrientation == "none")
                {
                    foreach (Ship s in playerShips)
                    {
                        if (s.name == currentTarget)
                        {
                            foreach (string st in s.locations)
                            {
                                foreach (Control c in Controls)
                                {
                                    if (c is Button)
                                    {
                                        Button b = (Button)c;
                                        if (b.Name == st)
                                        {
                                            if (b.Text == "X" & b.Name[0] == 'p')
                                            {
                                                startPoint = b.Name;
                                                bool repeat = true;
                                                while(repeat == true)
                                                {
                                                    Random random = new Random();
                                                    int direction = random.Next(1, 5);
                                                    if(direction == 1 & startPoint[1] != '0')
                                                    {
                                                        string guess = "p" + (int.Parse(startPoint[1].ToString()) - 1).ToString() + startPoint[2];
                                                        if(int.Parse(guess[1].ToString()) >= 0)
                                                        {
                                                            foreach(Control c2 in Controls)
                                                            {
                                                                if(c2 is Button)
                                                                {
                                                                    Button b2 = (Button)c2;
                                                                    if (b2.Name == guess)
                                                                    {
                                                                        if (b2.BackColor != Color.Red)
                                                                        {
                                                                            if (normalGuesses.Contains(b2.Name))
                                                                                normalGuesses.Remove(b2.Name);
                                                                            b2.BackColor = Color.Red;
                                                                            foreach (Ship sh in playerShips)
                                                                            {
                                                                                if (sh.locations.Contains(b2.Name))
                                                                                {
                                                                                    b2.Text = "X";
                                                                                    lblCenter.Text += "Computer hit\r\n";
                                                                                    sh.hits++;
                                                                                    currentOrientation = "verticle";
                                                                                    if (s.Sunk == true)
                                                                                    {
                                                                                        lblCenter.Text += "Computer sank " + s.name + "\n";
                                                                                        playersSunk++;
                                                                                        currentTarget = "none";
                                                                                        currentOrientation = "none";
                                                                                        startPoint = "none";
                                                                                    }
                                                                                    return true;
                                                                                }
                                                                            }
                                                                            lblCenter.Text += "Computer missed\r\n";
                                                                            return true;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else if (direction == 2 & startPoint[1] != '9')
                                                    {
                                                        string guess = "p" + (int.Parse(startPoint[1].ToString()) + 1).ToString() + startPoint[2];
                                                        if (int.Parse(guess[1].ToString()) <= 9)
                                                        {
                                                            foreach (Control c2 in Controls)
                                                            {
                                                                if (c2 is Button)
                                                                {
                                                                    Button b2 = (Button)c2;
                                                                    if (b2.Name == guess)
                                                                    {
                                                                        if (b2.BackColor != Color.Red)
                                                                        {
                                                                            if (normalGuesses.Contains(b2.Name))
                                                                                normalGuesses.Remove(b2.Name);
                                                                            b2.BackColor = Color.Red;
                                                                            foreach (Ship sh in playerShips)
                                                                            {
                                                                                if (sh.locations.Contains(b2.Name))
                                                                                {
                                                                                    b2.Text = "X";
                                                                                    lblCenter.Text += "Computer hit\r\n";
                                                                                    sh.hits++;
                                                                                    currentOrientation = "verticle";
                                                                                    if (s.Sunk == true)
                                                                                    {
                                                                                        lblCenter.Text += "Computer sank " + s.name + "\n";
                                                                                        playersSunk++;
                                                                                        currentTarget = "none";
                                                                                        currentOrientation = "none";
                                                                                        startPoint = "none";
                                                                                    }
                                                                                    return true;
                                                                                }
                                                                            }
                                                                            lblCenter.Text += "Computer missed\r\n";
                                                                            return true;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else if (direction == 3 & startPoint[2] != '0')
                                                    {
                                                        string guess = "p" + startPoint[1] + (int.Parse(startPoint[2].ToString()) - 1).ToString();
                                                        if (int.Parse(guess[2].ToString()) >= 0)
                                                        {
                                                            foreach (Control c2 in Controls)
                                                            {
                                                                if (c2 is Button)
                                                                {
                                                                    Button b2 = (Button)c2;
                                                                    if (b2.Name == guess)
                                                                    {
                                                                        if (b2.BackColor != Color.Red)
                                                                        {
                                                                            if (normalGuesses.Contains(b2.Name))
                                                                                normalGuesses.Remove(b2.Name);
                                                                            b2.BackColor = Color.Red;
                                                                            foreach (Ship sh in playerShips)
                                                                            {
                                                                                if (sh.locations.Contains(b2.Name))
                                                                                {
                                                                                    b2.Text = "X";
                                                                                    lblCenter.Text += "Computer hit\r\n";
                                                                                    sh.hits++;
                                                                                    currentOrientation = "horizontal";
                                                                                    if (s.Sunk == true)
                                                                                    {
                                                                                        lblCenter.Text += "Computer sank " + s.name + "\n";
                                                                                        playersSunk++;
                                                                                        currentTarget = "none";
                                                                                        currentOrientation = "none";
                                                                                        startPoint = "none";
                                                                                    }
                                                                                    return true;
                                                                                }
                                                                            }
                                                                            lblCenter.Text += "Computer missed\r\n";
                                                                            return true;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else if (direction == 4 & startPoint[2] != '9')
                                                    {
                                                        string guess = "p" + startPoint[1] + (int.Parse(startPoint[2].ToString()) + 1).ToString();
                                                        if (int.Parse(guess[2].ToString()) <= 9)
                                                        {
                                                            foreach (Control c2 in Controls)
                                                            {
                                                                if (c2 is Button)
                                                                {
                                                                    Button b2 = (Button)c2;
                                                                    if (b2.Name == guess)
                                                                    {
                                                                        if (b2.BackColor != Color.Red)
                                                                        {
                                                                            if (normalGuesses.Contains(b2.Name))
                                                                                normalGuesses.Remove(b2.Name);
                                                                            b2.BackColor = Color.Red;
                                                                            foreach (Ship sh in playerShips)
                                                                            {
                                                                                if (sh.locations.Contains(b2.Name))
                                                                                {
                                                                                    b2.Text = "X";
                                                                                    lblCenter.Text += "Computer hit\r\n";
                                                                                    sh.hits++;
                                                                                    currentOrientation = "horizontal";
                                                                                    if (s.Sunk == true)
                                                                                    {
                                                                                        lblCenter.Text += "Computer sank " + s.name + "\n";
                                                                                        playersSunk++;
                                                                                        currentTarget = "none";
                                                                                        currentOrientation = "none";
                                                                                        startPoint = "none";
                                                                                    }
                                                                                    return true;
                                                                                }
                                                                            }
                                                                            lblCenter.Text += "Computer missed\r\n";
                                                                            return true;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (currentOrientation == "verticle")
                {
                    foreach(Ship s in playerShips)
                    {
                        if(s.name == currentTarget)
                        {
                            foreach(string st in s.locations)
                            {
                                foreach(Control c in Controls)
                                {
                                    if(c is Button)
                                    {
                                        Button b = (Button)c;
                                        if(b.Name == st)
                                        {
                                            if(b.BackColor == Color.Red)
                                            {
                                                string up = "p" + (int.Parse(st[1].ToString()) - 1).ToString() + st[2];
                                                string down = "p" + (int.Parse(st[1].ToString()) + 1).ToString() + st[2];
                                                foreach (Control c2 in Controls)
                                                {
                                                    if(c2 is Button)
                                                    {
                                                        Button b2 = (Button)c2;
                                                        if(b2.Name == up & b2.BackColor != Color.Red)
                                                        {
                                                            if (normalGuesses.Contains(b2.Name))
                                                                normalGuesses.Remove(b2.Name);
                                                            b2.BackColor = Color.Red;
                                                            foreach (Ship sh in playerShips)
                                                            {
                                                                if (sh.locations.Contains(b2.Name))
                                                                {
                                                                    b2.Text = "X";
                                                                    lblCenter.Text += "Computer hit\r\n";
                                                                    sh.hits++;
                                                                    if (s.Sunk == true)
                                                                    {
                                                                        lblCenter.Text += "Computer sank " + s.name + "\n";
                                                                        playersSunk++;
                                                                        currentTarget = "none";
                                                                        currentOrientation = "none";
                                                                        startPoint = "none";
                                                                    }
                                                                    return true;
                                                                }
                                                            }
                                                            lblCenter.Text += "Computer missed\r\n";
                                                            return true;
                                                        }
                                                        else if (b2.Name == down & b2.BackColor != Color.Red)
                                                        {
                                                            if (normalGuesses.Contains(b2.Name))
                                                                normalGuesses.Remove(b2.Name);
                                                            b2.BackColor = Color.Red;
                                                            foreach (Ship sh in playerShips)
                                                            {
                                                                if (sh.locations.Contains(b2.Name))
                                                                {
                                                                    b2.Text = "X";
                                                                    lblCenter.Text += "Computer hit\r\n";
                                                                    sh.hits++;
                                                                    if (s.Sunk == true)
                                                                    {
                                                                        lblCenter.Text += "Computer sank " + s.name + "\n";
                                                                        playersSunk++;
                                                                        currentTarget = "none";
                                                                        currentOrientation = "none";
                                                                        startPoint = "none";
                                                                    }
                                                                    return true;
                                                                }
                                                            }
                                                            lblCenter.Text += "Computer missed\r\n";
                                                            return true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (currentOrientation == "horizontal")
                {
                    foreach (Ship s in playerShips)
                    {
                        if (s.name == currentTarget)
                        {
                            foreach (string st in s.locations)
                            {
                                foreach (Control c in Controls)
                                {
                                    if (c is Button)
                                    {
                                        Button b = (Button)c;
                                        if (b.Name == st)
                                        {
                                            if (b.BackColor == Color.Red)
                                            {
                                                string left = "p" + st[1] + (int.Parse(st[2].ToString()) - 1).ToString();
                                                string right = "p" + st[1] + (int.Parse(st[2].ToString()) + 1).ToString();
                                                foreach (Control c2 in Controls)
                                                {
                                                    if (c2 is Button)
                                                    {
                                                        Button b2 = (Button)c2;
                                                        if (b2.Name == left & b2.BackColor != Color.Red)
                                                        {
                                                            if (normalGuesses.Contains(b2.Name))
                                                                normalGuesses.Remove(b2.Name);
                                                            b2.BackColor = Color.Red;
                                                            foreach (Ship sh in playerShips)
                                                            {
                                                                if (sh.locations.Contains(b2.Name))
                                                                {
                                                                    b2.Text = "X";
                                                                    lblCenter.Text += "Computer hit\r\n";
                                                                    sh.hits++;
                                                                    if (s.Sunk == true)
                                                                    {
                                                                        lblCenter.Text += "Computer sank " + s.name + "\n";
                                                                        playersSunk++;
                                                                        currentTarget = "none";
                                                                        currentOrientation = "none";
                                                                        startPoint = "none";
                                                                    }
                                                                    return true;
                                                                }
                                                            }
                                                            lblCenter.Text += "Computer missed\r\n";
                                                            return true;
                                                        }
                                                        else if (b2.Name == right & b2.BackColor != Color.Red)
                                                        {
                                                            if (normalGuesses.Contains(b2.Name))
                                                                normalGuesses.Remove(b2.Name);
                                                            b2.BackColor = Color.Red;
                                                            foreach (Ship sh in playerShips)
                                                            {
                                                                if (sh.locations.Contains(b2.Name))
                                                                {
                                                                    b2.Text = "X";
                                                                    lblCenter.Text += "Computer hit\r\n";
                                                                    sh.hits++;
                                                                    if (s.Sunk == true)
                                                                    {
                                                                        lblCenter.Text += "Computer sank " + s.name + "\n";
                                                                        playersSunk++;
                                                                        currentTarget = "none";
                                                                        currentOrientation = "none";
                                                                        startPoint = "none";
                                                                    }
                                                                    return true;
                                                                }
                                                            }
                                                            lblCenter.Text += "Computer missed\r\n";
                                                            return true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            currentOrientation = "none";
            currentTarget = "none";
            return false;
        }
        private void ImpossibleGuess()
        {
            if(Ahead() == false)
            {
                Random random = new Random();
                int index = random.Next(0, impossibleGuesses.Count);
                string guess = impossibleGuesses[index];
                impossibleGuesses.Remove(guess);
                foreach (Control c in Controls)
                {
                    if (c is Button)
                    {
                        Button b = (Button)c;
                        if (b.Name.ToString() == guess)
                        {
                            b.BackColor = Color.Red;
                            b.Text = "X";
                            foreach (Ship s in playerShips)
                            {
                                if (s.locations.Contains(guess))
                                {
                                    s.hits++;
                                    lblCenter.Text += "Computer hit\r\n";
                                    if (s.Sunk == true)
                                    {
                                        lblCenter.Text += "Computer sank " + s.name + "\n";
                                        playersSunk++;
                                    }
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Random random = new Random();
                int index = random.Next(0, impossibleMisses.Count);
                string guess = impossibleMisses[index];
                impossibleMisses.Remove(guess);
                foreach (Control c in Controls)
                {
                    if (c is Button)
                    {
                        Button b = (Button)c;
                        if (b.Name.ToString() == guess)
                        {
                            b.BackColor = Color.Red;
                            lblCenter.Text += "Computer missed\r\n";
                            return;
                        }
                    }
                }
            }
        }
        private bool Ahead()
        {
            int computerHits = patrolBoat.hits + submarine.hits + destroyer.hits + battleship.hits + carrier.hits;
            int playerHits = patrolBoatC.hits + submarineC.hits + destroyerC.hits + battleshipC.hits + carrierC.hits;
            if (computerHits <= playerHits)
                return false;
            else
                return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HowToPlay howToPlay = new HowToPlay();
            howToPlay.Show();
        }
    }
}