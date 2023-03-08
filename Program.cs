﻿internal class program
{
    public class Enemy
    {
        //Variables that declare enemy status, such as stunned
        public string name;
        public bool stunned;
        public bool stunprot;
        public bool isLethal;
        public bool shootsProjectiles;
        public int projDebuff; //Putting this here as it relates to shootsProjectiles
        /* Explaination:
         * 0 = None, by default
         * 1 = Slow, removes 1 or 2 steps from total, scales with amount of steps took
         * 2 = Sludge, Removes 4 or 6 steps from total, scales with amount of steps took
         * 3 = Guilt, is inflicted whenever an sin is "committed", changes with each sin specifically
         * 4 = Enchanted, Removes all steps, +2 steps for entity in question, only inflicted by greed, lust, eyebird, stare
         * 5 = Loom, Removes half of steps if spotted = true
         * 6 = H.O.P.E.L.E.S.S, High Orientation Paralysis Exclusively Leaked Encountering Secret Sepiroth, causes extreme modifiers to be applied.
         * 7 = Zodiac, I don’t give a fuck what you rappin you been a fake,
         *  I’m everything that they ain’t,
         *  And can’t be, And won’t be,
         *  Wanna see me fall,
         *  I can’t go, i won’t leave

         *  12 can’t really stop shit,
         *  so I’m still pushin like a mosh pit
         *  
         *  7 RENDERS YOU COMPLETELY HOPELESS, -5 Steps (scales up), 
         */
        public int projRange = 3; // 3 by default
        public int speed = 1; //1 lowest, 3 highest (translates to steps
        public Enemy(string name, bool stunned, bool stunprod, bool isLethal, bool shootsProjectiles, int projDebuff, int projRange, int speed)
        {
            this.name = name;
            this.stunned = stunned;
            this.stunprot = stunprod;
            this.isLethal = isLethal;
            this.shootsProjectiles = shootsProjectiles;
            this.projDebuff = projDebuff;
            this.projRange = projRange;
            this.speed = speed;
        }
    }
    public class Item
    {
        public string name;
        public int rarity = 1;
        /*
         * 1 = Common
         * 2 = Uncommon
         * 3 = Rare
         * 4 = Very Rare
         * 5 = One of A Kind
         * 6 = RAM; Run Altering Modifier
         */
        public int itemUse;
        public int maxuse;
        public int currentuse;
        /*Explaination
         * 1 = Inflicts stun when nearby
         * 2 = Deters Enemies
         * 3 = Grants Extra Steps
         * 4 = Steps to Stamina (Stamina is stored steps and can be added in sets of 3)
         * 5 = Stamina to Steps (1 Stamina -> 3 Steps)
         * 6 = Instantly Kills you
         * 7 = "Confess", purges Guilt
         * 8 = Purges HOPELESS
         * 9 = Summons An Sin of Choice
         * 10 = Summons a Random Sin
         * 11 = Exchanges your loadout for new items
         * 12 = Changes this item in an item of choice
         */
        public bool wornout;
        public Item(string name, int rarity, int itemUse, int maxuse, int currentuse, bool wornout)
        {
            this.name = name;
            this.rarity = rarity;
            this.itemUse = itemUse;
            this.maxuse = maxuse;
            this.currentuse = currentuse;
            this.wornout = wornout;
        }
    }
    private static void Main(string[] args)
    {
        //Variables that explain during, after and before round information
        string spottedBy = "Error";
        string killedBy = "Error";
        int currentFloor = 0;
        int currentDif = 1;

        //Variables that select Modifiers
        bool relapse = false;
        bool boolMode = false;
        bool blackOut = false;
        bool BD = false;
        bool legacy = false;
        bool oneone = false;

        /* Explaination
         * Diffulty is handled in integers, which also should up the spawn chances of rarer enemies
         * 1 - Easy - Refuses to spawn any rarity 4 or highers, exept for sin requirements.
         * 2 - Medium - Refuses to spawn the rarest of enemies, will spawn tougher enemeies, and will make greater rewards, more enemies, and more button goals.
         * 3 - Hard - Allows spawning of all sorts of enemies, with even more button spawns. Powers some of the sins up.
         * 4 - Insane - Caps spawning to rarity 3 or higher, making them more common. Like usual, more buttons, more rewards, more danger.
         * 5 - Unreal - Caps spawning to 5 or higher, making them more common. Spawns more buttons, which can cap. Pernamently toggles multiple sin spawning.
         * 6 - Impossible - Caps spawning sins only, turning the game into a nightmare. Forces atleast 50 buttons each round, and is meant to stop the player from continuing.
         * 7 - Nil - The point where items are drained, and only spawns Error or Wrath. Occasionally spawns Rusanic
         * 
         * With the Relapse modifier, rapidly changes through the difficulties 1 till 5
         * With the Eclipse modifier, caps it from 3 upwards and spawns more enemies (Tougher Mode)
         * With the Blackout modifier, hides most information from you, as well as when an enemy is spotting you. (Blind Mode)
         * With the B&D (blind and deaf) modifier, hides ALL information from you, exept for when an floor is completed. (DO NOT TRY THIS AT HOME)
         * With the Survivor modifier, disables any items at the start, disables Sin items, disables most stuff. Leaves you with unfiltered experience (Legacy Mode)
         * With the 1:1 modifier, pernamently sets difficulty to Easy, but unlocks all enemies, all items, at all. (Randomizer Mode)
         */

        //Variables that handle menu coding
        int MenuCase = 0;

        //Enemy Database Management
        Enemy Error = new Enemy("Error", true, true, true, true, 6, 100, 10);
        Enemy Rusanic = new Enemy("Rusanic", false, true, true, true, 7, 7, 4);
        Enemy CENSORED = new Enemy("[CENSORED]", false, false, true, true, 6, 3, 3); 
        Enemy Walker = new Enemy("Walker", false, false, true, true, 1, 3, 2);
        Enemy Seer = new Enemy("Seer", false, false, true, false, 0, 0, 3);
        Enemy Blight = new Enemy("Blight", false, true, true, true, 2, 1, 2);
        Enemy Clown = new Enemy("Clown", false, false, true, false, 0, 0, 4);
        Enemy Roland = new Enemy("Roland", true, true, false, false, 0, 0, 0); //NPC, Sets Despair back to 0 if above 40
        Enemy Martin = new Enemy("Martin", true, true, false, false, 0, 0, 0); //NPC, offers compassion, or anything else, for a price...
        Enemy Binah = new Enemy("Binah", true, true, false, false, 0, 0, 0); //NPC, offers utillity items such as camera's
        Enemy Bew = new Enemy("Bew", true, true, false, false, 0, 0, 0); //NPC, offers food items on the way

        //Sin enemy Database
        Enemy Greed = new Enemy("Greed", false, false, true, true, 4, 5, 4);
        Enemy Gluttony = new Enemy("Gluttony", false, false, true, true, 4, 5, 4);
        Enemy Pride = new Enemy("Pride", false, true, true, false, 0, 0, 5);
        Enemy Sloth = new Enemy("Sloth", false, true, true, true, 1, 10, 1);
        Enemy Lust = new Enemy("Lust", false, true, true, true, 4, 2, 2);
        Enemy Wrath = new Enemy("Wrath", false, true, true, false, 0, 0, 5);
        Enemy Despair = new Enemy("Despair", false, true, true, false, 0, 0, 3);

        //Item Database Management
        /*Explaination
         * 1 = Inflicts stun when nearby
         * 2 = Deters Enemies
         * 3 = Grants Extra Steps
         * 4 = Changes this item in an item of choice
         * 5 = Changes this item into a random item
         * 6 = Instantly Kills you
         * 7 = "Confess", purges Guilt
         * 8 = Purges HOPELESS
         * 9 = Summons An Sin of Choice
         * 10 = Summons a Random Sin
         * 11 = Exchanges your loadout for new items
         * 12 = (Grants) Curses you with zodiac
         */

        //1 Inflicts stun when nearby
        Item Camera = new Item("Camera", 1, 1, 3, 0, false);

        //2 Deters enemies
        Item SmokeBomb = new Item("Smoke Bomb", 1, 2, 1, 0, false);
        
        //3 Grants extra steps
        Item Shoes = new Item("Shoes", 2, 3, 5, 0, false);
        
        //4 Turns into item of choice
        Item EightBall = new Item("8-Ball", 5, 4, 1, 0, false);
        Item BlackBox = new Item("---------", 5, 4, 1, 0, false);

        //5 Turns into random items
        Item sGift = new Item("Small Gift", 2, 5, 1, 0, false); //Gives 1 random
        Item mGift = new Item("Medium Gift", 3, 5, 1, 0, false); //Gives 2 random
        Item lGift = new Item("Large Gift", 4, 5, 1, 0, false); //Gives 3 random

        //6 Kills yourself instantly
        Item Knife = new Item("Knife", 4, 6, 1, 0, false); //100% chance
        Item Gun = new Item("Russian", 3, 6, 1, 0, false); //50% chance

        //7 "Confesses" for your sin, removes guilt debuff
        Item OneSin = new Item("One Sin and 100 good deeds", 4, 7, 10, 0, false);
        
        //8 Cures HOPELESS
        Item HOPE = new Item("HOPES AND DREAMS", 4, 8, 10, 0, false);
        
        //9 Summons sin of choice
        Item VileDeed = new Item("Vile Deed", 3, 9, 3, 0, false);
        
        //10 Summons random sin
        Item ThornCrown = new Item("Thorn Crown", 2, 10, 5, 0, false);
        
        //11 Swaps up your entire inv
        Item CSlots = new Item("Crazy Slots", 5, 11, 1, 0, false);
        
        //12 Gives you zodiac, idk why you'd want this
        Item DollarSign = new Item("Dollar Sign", 5, 12, 1, 0, false);

        //Variables that handle items and enemy generation
        int itemSpawnChance = 4; //This will be 1/4, so 8 is 1/8 (12.5%)
        int itemRarity = 1; //From a chance of 1 to 5, where 1 is common and 5 is an one-of-a-kind
        int monsterSpawnChance = 8;
        int monsterRarity = 1; //1 being lowest, 7 being rarest
        bool sinModifier = false; //Automatically locks the value to 7 and assigns sinChooser to current floor
        int sinChooser = 0; //Read below
        int despairModifier = 0; //Goes up every time an monster is encountered can only be lowered while meeting the "Roland" entity, which will not kill you when despair is above 40
        
        /*Sinchooser explaination:
         * 0 = error (Activates when the player encounters an error, chosen by default)
         * 1 = gluttony (Activates when a player has used more than 10 items during the current floor)
         * 2 = lust (Activates when the player has encountered 5 or more monsters in generation)
         * 3 = greed (Activates when a player has more than 25 items in their inventory)
         * 4 = despair (Activates when the despairmodifier is raised above 40)
         * 5 = wrath (Activates when the current floor is higher than 30, and the despair modifier is above 40 (can activate alongside despair))
         * 6 = sloth (Activates when the player has needed over 30 steps to clear the current floor)
         * 7 = pride (Activates when the player has less than 5 items starting at floor 5)
         * 
         * PRIORITY:
         * Error, wrath, sloth, pride, despair, greed, gluttony, lust
         * Whenever 2 of the sin entities conflict, forces the list first
         */
        

        //Variables that define the element of luck while playing
        Random step = new Random();
        int stepfinal = 0; //Defines how far a player will walk, with the amount of chances to be greeted by an entity

        //Variables that declare player status, such as stunned
        bool dead = false;
        bool start = false;
        int itemsUsed = 0;
        int stepsTaken = 0;
        int monstersEncountered = 0;

        //Variables that describe what an player has for items
        List<string> inventory = new List<string>(); //Inventory
        
        //Admin Client secret (USE AT YOUR OWN RISK)
        bool admintoggle = false; //To declare when the debug is activated
        string adminsecret = "DebugMe";
    }
}