using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
//pc
namespace MemoryGame
{
    class Game
    {
        //----Global Variable Decleration (Start)----//

        //List that houses all the "card" buttons
        private static List<Button> buttons = new List<Button>();
        //List that houses all the "card" images
        private static List<Image> icons = new List<Image>();
        //Hashtable that associates a button with an image
        private static Hashtable btn_icons = new Hashtable();
        //Hashset used for associating random images with buttons. Hashset does not allow duplicate values to be inserted
        private static HashSet<int> random_ints = new HashSet<int>();

        //Flipped down card image
        private static Image card_back = Image.FromFile("../../images/cardback.gif");
        private static Image ace_of_spades = Image.FromFile("../../images/ace_of_spades.png");
        private static Image ace_of_hearts = Image.FromFile("../../images/ace_of_hearts.png");
        private static Image ace_of_diamonds = Image.FromFile("../../images/ace_of_diamonds.png");
        private static Image ace_of_clubs = Image.FromFile("../../images/ace_of_clubs.png");
        private static Image king_of_hearts = Image.FromFile("../../images/king_of_hearts.png");
        private static Image queen_of_diamonds = Image.FromFile("../../images/queen_of_diamonds.png");
        private static Image jack_of_spades = Image.FromFile("../../images/jack_of_spades.png");
        private static Image ten_of_clubs = Image.FromFile("../../images/ten_of_clubs.png");
        private static Image ten_of_hearts = Image.FromFile("../../images/ten_of_hearts.png");
        private static Image joker = Image.FromFile("../../images/joker.png");

        //Sound effects
        private static System.Media.SoundPlayer kaching = new System.Media.SoundPlayer("../../images/ka-ching.wav");
        private static System.Media.SoundPlayer finish = new System.Media.SoundPlayer("../../images/finish.wav");

        //Timers
        private static Timer conceal_cards = new Timer();
        private static Timer flip_cards_down = new Timer();

        //Random object
        private static Random rnd = new Random();

        //Global variables
        private static int clickeCounter = 0;//helps us determine whether the first or the second card of a potential pair has been clicked
        private static int score = 0;//stores the player's score
        private static int streak = 0;//stores the player's winning streak
        private static int pairCounter = 0;//counts the correct pairs
        private static Button btn_compare1;//temporary var for storing the button first button that triggered the click event
        private static Button btn_compare2;//temporary var for storing the button second button that triggered the click event
        private static Button start;//reference for the start button 
        private static int gameCounter = 0;//rounds counter
        private static SuperMemory mainGame;//get reference of the Menu so that we can edit some labels

        //----Global Variable Decleration (End)----//
        
        //----Main Methods (Start)----//

        //method used to set the game up
        public static void setGame(SuperMemory menu)
        {
            mainGame = menu;//reference of the Menu form

            //sets the background image
            menu.BackgroundImage = Image.FromFile("../../images/bgimg.png");

            //gets all the "card" buttons and places them into a list
            foreach (Button btn in menu.Controls.OfType<Button>().Where(btn => btn.Name.StartsWith("card")))
            {
                //sets width, height, background image and place the into the list
                btn.Width = 75;
                btn.Height = 100;
                btn.BackgroundImage = card_back; //Environment.CurrentDirectory shows that I am in the bin/Debug folder, so we go up 2 folders
                buttons.Add(btn);
            }

            //adds all the necessary card images into a list, twice
            for (int i = 0; i < 2; i++)
            {
                icons.Add(ace_of_spades);
                icons.Add(ace_of_hearts);
                icons.Add(ace_of_diamonds);
                icons.Add(ace_of_clubs);
                icons.Add(king_of_hearts);
                icons.Add(queen_of_diamonds);
                icons.Add(jack_of_spades);
                icons.Add(ten_of_clubs);
                icons.Add(ten_of_hearts);
                icons.Add(joker);
            }
        }

        //starts the game
        public static void startGame(Button btn_start)
        {
            start = btn_start;//reference for the start button

            //shuffles the deck and randomly assigns cards to buttons
            shuffle();

            //reveal cards 
            foreach (Button btn in buttons)
            {
                btn.BackgroundImage = (Image)btn_icons[btn];
            }

            //flips the cards back after 6 seconds
            conceal_cards.Interval = 6000;
            conceal_cards.Tick += new EventHandler(conceal_cards_tick);//what happens after the specified amount of time
            conceal_cards.Enabled = true;
            conceal_cards.Start();

            //disables the button so that it cannot be pressed again
            btn_start.Enabled = false;
        }

        //----Main Methods (End)----//

        //----Support Methods (Start)----//

        //shuffles the deck
        private static void shuffle()
        {
            //if it's not the first game, reset everything
            if (gameCounter > 0)
            {               
                random_ints.Clear();         
                btn_icons.Clear();
                //resetVariables();//resets variables such as score, streak etc
            }

            //Hashset does not allow duplicate values. We produce random numbers from 0-19 and add them to the Hashset.
            //This way it is guaranteed that we'll have every number from 0-19 only once
            do
            {
                random_ints.Add(rnd.Next(20));
            } while (random_ints.Count < 20);//we repeat untill the every number from 0-19 is in

            //we match a button from the buttons' list with an image from the icons' list which position is determined by the Hashset
            //Since Hashet has numbers from 0-19 in no particular order, we can mix the images with the buttons
            for (int i = 0; i < buttons.Count; i++)
            {
                btn_icons.Add(buttons.ElementAt<Button>(i), icons.ElementAt<Image>(random_ints.ElementAt<int>(i)));
            }
            Console.WriteLine(random_ints.Count + "  " + btn_icons.Count);
        }

        /*
            //resets variables for game restart
            private static void resetVariables()
            {
                clickeCounter = 0;//helps us determine whether the first or the second card of a potential pair has been clicked
                score = 0;//stores the player's score
                streak = 0;//stores the player's winning streak
                pairCounter = 0;//counts the correct pairs
                btn_compare1 = null;//temporary var for storing the button first button that triggered the click event
                btn_compare2 = null;//temporary var for storing the button second button that triggered the click event
                mainGame.setScore(score);//sets the score
                mainGame.setResult("");//sets the result
            }

        
            //removes all click events from a button
            private static void RemoveClickEvent(Button b)
            {
                FieldInfo f1 = typeof(Control).GetField("EventClick", BindingFlags.Static | BindingFlags.NonPublic);
                object obj = f1.GetValue(b);
                PropertyInfo pi = b.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
                EventHandlerList list = (EventHandlerList)pi.GetValue(b, null);
                list.RemoveHandler(obj, list[obj]);
            }
        */

        //----Support Methods (End)----//

        //----Card Buttons Click Event Handler (Start)----//

        //handler for the buttons' click event
        private static void card_button_click(object sender, EventArgs e)//click event handler
        {
            if (clickeCounter < 2)
            {
                //means that only one "card" has been clicked
                if (clickeCounter == 0)
                {
                    btn_compare1 = (Button)sender;//get the first button that triggered the event    
                    btn_compare1.BackgroundImage = (Image)btn_icons[btn_compare1];//flip the "card"
                    clickeCounter++;//increase counter
                }
                else if (clickeCounter == 1)//means that both "cards" have been clicked
                {
                    clickeCounter++;//increase counter
                    btn_compare2 = (Button)sender;//get the second button that triggered the event
                    btn_compare2.BackgroundImage = (Image)btn_icons[btn_compare2];//flip the "card"

                    //check if the cards are the same
                    if ((btn_icons[btn_compare1] == btn_icons[btn_compare2]) && (btn_compare1 != btn_compare2))//if the cards match
                    {
                        //play the kaching! sound
                        kaching.Play();
                        streak++;//increase the winning streak
                        if (streak >= 3)
                        {
                            score += 20;
                        }
                        else
                        {
                            score += 10;
                        }

                        mainGame.setScore(score); //update score                               
                        pairCounter++;//increase the pair counter
                        if (pairCounter == 10)//if all pairs have been matched, game is over
                        {
                            if (score >= 80)
                            {
                                mainGame.setResult("You won!");
                            }
                            else
                            {
                                mainGame.setResult("You lost!");
                            }
                            gameCounter++;

                            //play the finish sound
                            finish.Play();
                            //start.Enabled = true;//re-enable the start button
                        }
                        btn_compare1.Click -= card_button_click;//remove the event handler
                        btn_compare2.Click -= card_button_click;//remove the event handler

                        clickeCounter = 0;
                    }
                    else//if the cards do not match
                    {
                        //reset streak and update score
                        streak = 0;
                        score -= 5;
                        mainGame.setScore(score);

                        //timer for flipping cards back once wrongly paired
                        flip_cards_down.Interval = 2000;
                        flip_cards_down.Tick += new EventHandler(flip_cards_down_tick);//what happens after the specified amount of time
                        flip_cards_down.Enabled = true;
                        flip_cards_down.Start();
                    }
                }
            }
        }

        //----Card Buttons Click Event Handler (End)----//

        //----Timers (Start)----//

        //method executed after "conceal_cards" timer ends
        private static void conceal_cards_tick(object sender, EventArgs e)
        {
            //flips all cards back and assigns a Click event handler to each button
            foreach (Button btn in buttons)
            {
                btn.BackgroundImage = card_back;//flip card
                btn.Click += new EventHandler(card_button_click);//set event handler
            }
            conceal_cards.Stop();
        }

        //method executed after "card comparison" timer ends
        private static void flip_cards_down_tick(object sender, EventArgs e)
        {
            //flip cards
            btn_compare1.BackgroundImage = card_back;
            btn_compare2.BackgroundImage = card_back;

            clickeCounter = 0;//reset the click counter
            flip_cards_down.Stop();
        }

        //----Timers (End)----//
    }
}