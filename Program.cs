using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using static program;
using System.Threading.Channels;

internal class program
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
         *  8 RENDERS YOU COMPLETELY HOPELESS, -5 Steps (scales up), 
         *  9 Applies most of above :)
         */
        public int projRange = 3; // 3 by default
        public int speed = 1; //1 lowest, 3 highest (translates to steps)
        public Enemy(string name, bool stunned, bool stunprod, bool isLethal, bool shootsProjectiles, int projDebuff, int projRange, int speed, bool isNPC)
        {
            this.name = name;
            this.stunned = stunned;
            this.stunprot = stunprod;
            this.isLethal = isLethal;
            this.shootsProjectiles = shootsProjectiles;
            this.projDebuff = projDebuff;
            this.projRange = projRange;
            this.speed = speed;
            this.isNPC = isNPC;
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
        string displayDif = "Solstice";
        int currentFloor = 0;
        int currentDif = 1;

        //Names the difficulty accordingly
        if (currentDif == 1) { displayDif = "Easy"; }
        if (currentDif == 2) { displayDif = "Medium"; }
        if (currentDif == 3) { displayDif = "Hard"; }
        if (currentDif == 4) { displayDif = "Insane"; }
        if (currentDif == 5) { displayDif = "Unreal"; }
        if (currentDif == 6) { displayDif = "Impossible"; }
        if (currentDif == 7) { displayDif = "Nil"; }
        if (currentDif == 8) { displayDif = "Artificial I"; }
        if (currentDif == 9) { displayDif = "Artificial II"; }
        if (currentDif == 10) { displayDif = "Artificial III"; }
        if (currentDif == 11) { displayDif = "Artificial IV"; }
        if (currentDif == 12) { displayDif = "Artificial V"; }
        if (currentDif == 13) { displayDif = "Artificial VI"; }
        if (currentDif == 14) { displayDif = "Artificial VII"; }
        if (currentDif == 15) { displayDif = "Broken I"; }
        if (currentDif == 16) { displayDif = "Broken II"; }
        if (currentDif == 17) { displayDif = "Broken III"; }
        if (currentDif == 18) { displayDif = "Broken IV"; }
        if (currentDif == 19) { displayDif = "Broken V"; }
        if (currentDif >= 20) { displayDif = "Solstice"; }

        //Variables that define how long an floor is and how hard it currently is to do stuff
        int buttonGoal = 0;
        int stepGoal;

        int currentSteps = 0;
        int currentButtons = 0;

        int enemyRarityMin;
        int enemySpawnRarity = 100; //Percentage from 1 on X

        

        bool wrathOnly = false; //On Nil
        bool errorOnly = false; //On broken
        bool artificialErrorWrathCombo = false; //On artificial

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
         * (Better chart at the gameplay loop)
         * 
         * With the Relapse modifier, rapidly changes through the difficulties 1 till 5
         * With the Eclipse modifier, caps difficulty from 3 upwards and spawns more enemies (Tougher Mode)
         * With the Blackout modifier, hides most information from you, as well as when an enemy is spotting you. (Blind Mode)
         * With the B&D (blind and deaf) modifier, hides ALL information from you, exept for when an floor is completed. (DO NOT TRY THIS AT HOME)
         * With the Survivor modifier, disables any items at the start, disables Sin items, disables most stuff. Leaves you with unfiltered experience (Legacy Mode)
         * With the 1:1 modifier, pernamently sets difficulty to Easy, but unlocks all enemies, all items, at all. (Randomizer Mode)
         */

        //Enemy Database Management
        Enemy Error = new Enemy("Error", true, true, true, true, 6, 100, 10, false); //Spawns when error
        Enemy Rusanic = new Enemy("Rusanic", false, true, true, true, 7, 7, 4, false); //Spawns at Zodiac :troll:
        Enemy CENSORED = new Enemy("[CENSORED]", false, false, true, true, 8, 3, 3, false);  //Only spawns at certain (secret) conditions
        Enemy Walker = new Enemy("Walker", false, false, true, true, 1, 3, 2, false);
        Enemy Gogh = new Enemy("Gogh", false, false, true, false, 0, 0, 3, false); //Special Enemy that kills if you have a GUN
        Enemy Seer = new Enemy("Seer", false, false, true, false, 0, 0, 3, false); 
        Enemy Blight = new Enemy("Blight", false, true, true, true, 2, 1, 2, false);
        Enemy Clown = new Enemy("Clown", false, false, true, false, 0, 0, 4, false);
        Enemy WN = new Enemy("White Night", false, true, true, true, 9, 2, 1, false); //Spawns under special conditions
        Enemy Enphoso = new Enemy("Enphoso", false, true, true, true, 8, 6, 3, false); //Spawns under special conditions
        Enemy Roland = new Enemy("Roland", true, true, false, false, 0, 0, 0, true); //NPC, Sets Despair back to 0 if above 40
        Enemy Martin = new Enemy("Martin", true, true, false, false, 0, 0, 0, true); //NPC, offers compassion, or anything else, for a price...
        Enemy Binah = new Enemy("Binah", true, true, false, false, 0, 0, 0, true); //NPC, offers utillity items such as camera's
        Enemy Bew = new Enemy("Bew", true, true, false, false, 0, 0, 0, true); //NPC, offers food items on the way
        Enemy Sinister = new Enemy("Sinister", true, true, false, false, 0, 0, 0, true); //NPC, offers friendship and various quotes from the real sinister

        //Sin enemy Database
        Enemy Greed = new Enemy("Greed", false, false, true, true, 4, 5, 4, false);
        Enemy Gluttony = new Enemy("Gluttony", false, false, true, true, 4, 5, 4, false);
        Enemy Pride = new Enemy("Pride", false, true, true, false, 0, 0, 5, false);
        Enemy Sloth = new Enemy("Sloth", false, true, true, true, 1, 10, 1, false);
        Enemy Lust = new Enemy("Lust", false, true, true, true, 4, 2, 2, false);
        Enemy Wrath = new Enemy("Wrath", false, true, true, false, 0, 0, 5, false);
        Enemy Despair = new Enemy("Despair", false, true, true, false, 0, 0, 3, false);

        //Enemy rarity Management (SINCE THIS WAS APPARENTLY NEEDED)
        var CommonE = new List<Enemy>();
        CommonE.add(Walker);
        CommonE.add(Clown);
        CommonE.add(Blight);
        CommonE.add(Seer);

        var NPC = new List<Enemy>();
        UnusualE.add(Bew);
        UnusualE.add(Martin);
        UnusualE.add(Roland);
        UnusualE.add(Sinister);
        UnusualE.add(Binah);
        UnusualE.add(Gogh);

        var SpecialE = new List<Enemy>();
        SpecialE.add(Error);
        SpecialE.add(Rusanic);
        SpecialE.add(CENSORED);
        SpecialE.add(WN);
        SpecialE.add(Enphoso);

        var SinEnemy = new List<Enemy>();
        SinEnemy.add(Wrath);
        SinEnemy.add(Greed);
        SinEnemy.add(Pride);
        SinEnemy.add(Sloth);
        SinEnemy.add(Despair);
        SinEnemy.add(Lust);
        SinEnemy.add(Gluttony);

        //Item Database Management

        //1 Inflicts stun when nearby
        Item Camera = new Item("Camera", 1, 1, 3, 0, false);
        Item Flash = new Item("Flashbang", 1, 1, 3, 0, false);
        Item Debug = new Item("Debug", 6, 1, 1, 1, false); //TO BE USED WHILE TESTING CODE!!!
        //2 Deters enemies
        Item SmokeBomb = new Item("Smoke Bomb", 1, 2, 1, 0, false);
        Item Fart = new Item("Fart", 3, 2, 5, 0, false); //Farts last longer and are more effective

        //3 Grants extra steps
        Item Shoes = new Item("Shoes", 2, 3, 5, 0, false);
        Item Jetpack = new Item("Jetpack", 4, 3, 10, 0, false);
        Item Glognut = new Item("Glognut", 5, 3, 1000, 0, false); //Has a 25% chance to spawn enphoso
        Item HeartGlognut = new Item("Heart Glognut", 6, 3, 10000, 0, false); //mwa mwa mwa mwa mwa

        //4 Turns into item of choice
        Item EightBall = new Item("8-Ball", 5, 4, 1, 0, false);
        Item BlackBox = new Item("---------", 5, 4, 1, 0, false); //Has a 30% Chance to become a GUN

        //5 Turns into random items
        Item sGift = new Item("Small Gift", 2, 5, 1, 0, false); //Gives 1 random
        Item mGift = new Item("Medium Gift", 3, 5, 1, 0, false); //Gives 2 random
        Item lGift = new Item("Large Gift", 4, 5, 1, 0, false); //Gives 3 random

        //6 Kills yourself instantly
        Item Knife = new Item("Knife", 4, 6, 1, 0, false); //100% chance
        Item Gun = new Item("Russian", 3, 6, 1, 0, false); //50% chance

        //7 "Confesses" for your sin, removes guilt debuff
        Item OneSin = new Item("One Sin and 100 good deeds", 4, 7, 10, 0, false); //100% Chance
        Item JCross = new Item("Jesus Cross", 5, 7, 3, 0, false); //50-50, summons whitenight upon fail

        //8 Cures HOPELESS
        Item HOPE = new Item("HOPES AND DREAMS", 4, 8, 10, 0, false); //Cures "6" hopeless
        Item HELP = new Item("HELPING HAND", 4, 8, 10, 0, false); //Cures "8" hopeless

        //9 Summons sin of choice
        Item VileDeed = new Item("Vile Deed", 3, 9, 3, 0, false);
        Item Contract = new Item("Contract", 3, 9, 3, 0, false); //"Reskin"

        //10 Summons random sin
        Item ThornCrown = new Item("Thorn Crown", 2, 10, 5, 0, false);
        
        //11 Swaps up your entire inventory
        Item CSlots = new Item("Crazy Slots", 5, 11, 1, 0, false);
        
        //12 Gives you zodiac, idk why you'd want this
        Item DollarSign = new Item("Dollar Sign", 5, 12, 1, 0, false);
        Item Constellation = new Item("Constellation", 5, 12, 1, 0, false); //Reskin

        //Item arrays dependant on rarity
        //SERVES AS LOOTTABLE, MODIFY AT YOUR OWN RISK!
        
        //Common
        var Common = new List<Item>();
        Common.Add(Camera);
        Common.Add(Flash);
        Common.Add(SmokeBomb);

        //Uncommon
        var Uncommon = new List<Item>();
        Uncommon.Add(Shoes);
        Uncommon.Add(sGift);
        Uncommon.Add(ThornCrown);

        //Rare
        var Rare = new List<Item>();
        Rare.Add(Fart);
        Rare.Add(mGift);
        Rare.Add(Gun);
        Rare.Add(Contract);
        Rare.Add(VileDeed);

        //Epic
        var Epic = new List<Item>();
        Epic.Add(Jetpack);
        Epic.Add(lGift);
        Epic.Add(Knife);
        Epic.Add(OneSin);
        Epic.Add(HOPE);
        Epic.Add(HELP);


        //One of a Kind
        var Exotic = new List<Item>();  
        Exotic.Add(EightBall);
        Exotic.Add(BlackBox);
        Exotic.Add(JCross);
        Exotic.Add(CSlots);
        Exotic.Add(DollarSign);
        Exotic.Add(Constellation);
        Exotic.Add(Glognut);

        //R.A.M.
        var RAMitems = new List<Item>();
        RAMitems.Add(HeartGlognut);
        RAMitems.Add(Debug);

        //Variables that define Exotic Enemies Values
        int sinChooser = 0; //Read below
        int despairModifier = 0; //Goes up every time an monster is encountered can only be lowered while meeting the "Roland" entity, which will not kill you when despair is above 40
        int chaseLength = 25; //Sets a step goal for the enemy, will stop walking afterwards, will scale with difficulty, rarity n'd such
        int enemySteps = 3; //Sets step amount
        int enemyStepsBonus = 0
        int enemyStepsFinal = enemySteps + enemyStepsBonus;

        //Variables that define Items and littering
        int littering = 20; // is an 1/definable % chance to spawn an item on a step

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
        Random step = new Random(); //Player step
        Random eStep = new Random(); //Enemy step
        Random itemTrigger = new Random(); //50,50 for items like GUN
        Random projHitcheck = new Random(); //Checks if projectile HAS hit, becomes higher with range
        Random itemSpawn = new Random(); //Checks if an item spawns in step range
        Random pickRare = new Random(); //Select Rarity Category
        Random indexPick = new Random(); //Checks in rarity lists when lootPick() is called
        

        //Variables that declare player status, such as stunned
        bool dead = false; //Checks for gameover
        bool start = false;
        bool spotted = false;

        //Variables that declare the current debuffs and existing debuffs off the player
        List<string> DebuffLibrary = new List<string>(); //Defines debuff library
        List<string> PlayerDebuffs = new List<string>(); //Defines current player debuffs

        //Debuff Library
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
         *  8 RENDERS YOU COMPLETELY HOPELESS, -5 Steps (scales up), 
         *  9 Applies most of above :)
         */

        DebuffLibrary.Add("Slow");
        DebuffLibrary.Add("Sludge");
        DebuffLibrary.Add("Guilt");
        DebuffLibrary.Add("Enchanted");
        DebuffLibrary.Add("Loom");
        DebuffLibrary.Add("EmotionFilter"); //H.O.P.E.L.E.S.S.
        DebuffLibrary.Add("Zodiac");
        DebuffLibrary.Add("HOPELESS");

        int itemsUsed = 0;
        int stepsTaken = 0;
        int monstersEncountered = 0;

        String name; //Player name

        int stepspeed = 3; //Defines amount of steps taken, also checks how much times an item will be able to be found, an enemy will spot
        int mood = 0; //Mood lowers despair, and will give bonus rewards when interacting with martin. will also serve as a step multiplier
        int stepfinal = 0; //Sum of modifiers + Base

        //Inventory Placeholder
        List<Item> Inventory = new List<Item>();

        //Stepbonus Placeholder
        List<int> stepBonus = new List<int>();
        
        //Admin Client secret (USE AT YOUR OWN RISK)
        bool admintoggle = false; //To declare when the debug is activated
        string adminsecret = "DebugMe";

        /*Although admin is in the works, here's on how it *should* work
         * 
         * help - displays commands
         * 
         * GAMEPLAY
         * gib (itemname), gives an item
         * in (sin), commits sin and forces it next round
         * look, forces chase from ALL enemies on current floor
         * gmod (mod), forces said modifier
         * 
         * DATA
         * dip (value) (value), adds the first to the second
         * rip (value) (value), removes value the second
         * get (defined int), shows current value
         * 
         * PLAYER
         * ruina (ailmentInt), gives said ailment int
         * libraryofruina, shows ailmentInt
         *
         */
        
        //Loottable picker
        Item lootPick(int minRarity)
        {
            Item chosenitem = Debug;
            int rarityC = 0; //rarity confirmer and debugging tool
            int index = 0; //After the list is picked, checks for random item
            int chosenRarity = pickRare.Next(1, minRarity); //Chooses an number from minRarity to 100, follow rarity below
            /*
            65 and above - Common
            between 45 and 65 - Uncommon
            between 25 and 44 - Rare
            between 10 and 24 - Epic
            between 3 and 9 - One of a kind
            between 1 and 2 - RAM item
            */


            //Picks the actual rarity when calling the function
            if (chosenRarity > 65) //Common
            {
                rarityC = 1;
            } else if (chosenRarity < 65 && chosenRarity >= 45) //Uncommon
            {
                rarityC = 2;
            } else if (chosenRarity < 45 && chosenRarity >= 25) //Rare
            {
                rarityC = 3;
            } else if (chosenRarity < 25 && chosenRarity >= 10) //Epic
            {
                rarityC = 4;    
            } else if (chosenRarity < 10 && chosenRarity >= 3) //One of a kind
            {
                rarityC = 5;
            } else if (chosenRarity < 3) //RAM item
            {
                rarityC = 6;
            }


            //Checks out of given rarity inside an list
            if (rarityC == 1)
            {
                index = indexPick.Next(0, Common.Count);
                chosenitem = Common[index];
            } else if (rarityC == 2)
            {
                index = indexPick.Next(0, Uncommon.Count);
                chosenitem = Uncommon[index];
            } else if (rarityC == 3)
            {
                index = indexPick.Next(0, Rare.Count);
                chosenitem = Rare[index];
            } else if (rarityC == 4) 
            {
                index = indexPick.Next(0, Epic.Count);
                chosenitem = Epic[index];
            } else if (rarityC == 5)
            {
                index = indexPick.Next(0, Exotic.Count);
                chosenitem = Exotic[index];
            } else if (rarityC == 6)
            {
                index = indexPick.Next(0, RAMitems.Count);
                chosenitem = RAMitems[index];
            }

            //Console.WriteLine("DEBUG: RARITY {0}, ITEMNAME {1}, LUCK {2}", chosenitem.rarity, chosenitem.name, chosenRarity);
            Console.WriteLine("You found an {0}!", chosenitem.name);
            return chosenitem;
        }

        void ItemUseage(Item selectedItemString)
        {
            if (selectedItemString.wornout)
            {
                Console.WriteLine("This item is wornout! You threw it away.");
                Inventory.Remove(selectedItemString);

            } else
            {
                Console.WriteLine("You used {0}", selectedItemString.name);
                Inventory.Remove(selectedItemString);

                /*I will now post the database to make sure I know what effects to apply
                *1 = Inflicts stun when nearby
                * 2 = Deters Enemies
                * 3 = Grants Extra Steps
                *4 = Item of choice
                *5 = Random items
                *6 = Instantly Kills you
                *7 = "Confess", purges Guilt
                *8 = Purges HOPELESS
                * 9 = Summons An Sin of Choice
                *10 = Summons a Random Sin
                * 11 = Exchanges your loadout for new items
                * 12 = Changes this item in an item of choice
                */

                switch (selectedItemString.itemUse)
                {
                    case 1:
                        //Checks which item this is (if needed)

                        //Check for distance player from enemy in chase

                        //apply stun

                        //Apply usedint
                        break;
                    case 2:
                        //Checks which item this is (if needed)

                        //Set chance of spawning to X for X amount of turns

                        //apply used int

                        break;
                    case 3:
                        //Checks which item this is (if needed)

                        break;
                    case 4:
                        //Checks which item this is (if needed)

                        //Conversion

                        //apply used int
                        break;
                    case 5:
                        //Checks which item this is (if needed)

                        //Gives X amount of randomized items with an item picker system

                        //apply used int
                        break;
                    case 6:
                        //Checks which item this is (if needed)

                        //(Optional chance)

                        //Gameovering and removal of the item
                        break;
                    case 7:
                        //Checks which item this is (if needed)

                        //Purge guilt (and or adds extra effects)

                        //Apply wornint
                        break;
                    case 8:
                        //Checks which item this is (if needed)

                        //Checks for hopeless debuff

                        //Removal

                        //Apply wornint
                        break;
                    case 9:
                        //Checks which item this is (if needed)

                        //Gives menu

                        //Spawns sin

                        //Apply wornint
                        break;
                    case 10:
                        //Checks which item this is (if needed)

                        //Spawns random sin

                        //Apply wornint
                        break;
                    case 11:
                        //Checks which item this is (if needed)

                        //Foreach loop

                        //Deletes this item
                        break;
                    case 12:
                        //Checks which item this is (if needed

                        //Summon zodiac or smth

                        //Apply usedint
                        break;
                    default:
                        //Default should never be called, atleast, I have to make sure it doesn't
                        Console.WriteLine("You've encountered an error. The current ItemUse is being used, which is {0}.", selectedItemString.itemUse);
                        break;
                        if (selectedItemString.currentuse == selectedItemString.maxuse)
                        {
                            selectedItemString.wornout = true;
                        }
                }
            }
        }

        void Chase(List<Enemy> listChoose)
        {
            //Better off making smth like I did when I picked items
            int enemyIndexSpawn = 0;
            int maxS = listChoose.Count;
            indexPick.next(0, maxS+1);

            Enemy chosenenemy = listChoose[indexPick];
            if (chosenenemy != null)
            {
                Console.WriteLine("Suddenly... you look behind you... Something is off...");
                Console.WriteLine("IS THAT {0}? YOU BETTER RUN!", chosenenemy.name);
                Console.WriteLine("DEBUG: Chased by {0}, Lasts for {1}, enemy has {2}")
            } else
            {
                Console.WriteLine("You've encountered a bug at line 638.");
            }
        }

        //Menu (FINALLY)
        Console.WriteLine("Minus One: Descent To Madness.");
        Console.WriteLine("Press any key to start.");
        Console.ReadKey();

        Console.WriteLine("What is your name, Traveller?");
        name = Console.ReadLine();
        Console.WriteLine("Very well, {0}.", name);
        Console.WriteLine("Do you want to hear the tutorial? Y/N");
        
        string answer = Console.ReadLine();

        if (answer == "Y")
        {
            Console.WriteLine("The premise is easy.");
            Console.ReadKey();
            Console.WriteLine("Your job is to get as deep as possible");
            Console.ReadKey();
            Console.WriteLine("You are not alone. There are monsters out there. Learn to survive and you will be fine.");
            Console.ReadKey();
            Console.WriteLine("There is no end. Don't expect there to be one exept death.");
            Console.ReadKey();
            Console.WriteLine("The Game will become more difficult the longer you are in the run.");
            Console.WriteLine("===========================================================================");
            Console.ReadKey();
            Console.WriteLine("The Gameplay can be simplified to two actions; Items and Stepping.");
            Console.WriteLine("STEPPING: Stepping is used to advance forward and complete your button/steps assignment.");
            Console.WriteLine("ITEMS: You can find various ITEMS while STEPPING, which you can then use to advance your next STEP.");
            Console.WriteLine("CHASE: Entities can chase you, and will chase you eventually. Outrun them for long enough and you'll be fine.");
            Console.WriteLine("SIN: Beware; the gods are watching as you descend down this cursed inferno. Keep an eye on what you eat, on what you do, everything.");
            Console.ReadKey();
        }
        Console.WriteLine("Great. Let's move you in position. Before you begin your run, I'll give you with an item that might help you out.");
        Console.WriteLine("Here, have this.");
        Inventory.Add(lootPick(101));

        do
        {
            //PRE FLOOR LAYER

            Console.WriteLine("Now moving to... Floor {0}.", currentFloor);
            Console.WriteLine("Current Difficulty... {0}.", displayDif);
            Console.ReadKey();
            /*
             * Now comes the hard part
             * Making actual formulas to keep the game balanced, but also harder over time
             * 
             * I already implemented an button goal that starts from 3.
             * 
             * Here's the chart again
             * 
             * 1 - Easy - Refuses to spawn any rarity 4 or highers, exept for sin requirements.
             * 2 - Medium - Refuses to spawn the rarest of enemies, will spawn tougher enemeies, and will make greater rewards, more enemies, and more button goals.
             * 3 - Hard - Allows spawning of all sorts of enemies, with even more button spawns. Powers some of the sins up.
             * 4 - Insane - Caps spawning to rarity 3 or higher, making them more common. Like usual, more buttons, more rewards, more danger.
             * 5 - Unreal - Caps spawning to 5 or higher, making them more common. Spawns more buttons, which can cap. Pernamently toggles multiple sin spawning.
             * 6 - Impossible - Caps spawning sins only, turning the game into a nightmare. Is meant to stop the player from continuing.
             * 7 - Nil - The point where items are drained, and only spawns Wrath. Occasionally spawns Rusanic.
             * 8/14 - Artificial(I/VII) - The point where the game itsself starts to break and will spawn Error or Wrath. Occasionally spawns Rusanic.
             * 15/19 - Broken(I/V) - The point where the game is unplayable. If somehow this difficulty is achieved, it will spawn only Error.
             * 20+ - Solstice - If SOMEHOW you are to come here, the game will become unplayable. This is only used for development, and should in no way be continued. Upon continuing, will just freeze you in your steps and is only achieved with the debug menu.
             */

            stepGoal = 30 + (3 * currentDif);
            if (currentDif >= 3) //If the difficulty is "Hard" or above...
            {
                buttonGoal = 1 + (2 * currentDif); //Sets button requirement by 1 + (2 * i), which is as base "6".
            }
            else if (currentDif >= 4)
            {
                enemyRarityMin = 3;
            }
            if (currentDif >= 5)
            {
                enemyRarityMin = 5;
            }
            if (currentDif >= 6)
            {
                enemyRarityMin = 7;
            }
            if (currentDif >= 7)
            {
                wrathOnly = true;
            }
            if (currentDif >= 8 && currentDif <= 14)
            {
                artificialErrorWrathCombo = true;
            }
            if (currentDif >= 15 && currentDif <= 19)
            {
                errorOnly = true;
            }
            if (currentDif >= 20)
            {
                buttonGoal = 50 + (currentDif * 10);
                Console.WriteLine("IF YOU HAVE GOTTEN THIS FAR, PLEASE END YOUR RUN. THIS IS THE FINAL DIFFICULTY, AND NOTHING IS BEHIND THIS. THIS DIFFICULTY IS MEANT TO STOP YOU. CONGRATULATIONS ON COMING THIS FAR.");
            }

            //FLOOR LAYER
            do
            {
                Console.WriteLine("Type what you will do.");
                Console.WriteLine("Current Commands: Inventory, Step, Use, Status");
                string answerGet;
                answerGet = Console.ReadLine();
                switch (answerGet)
                {
                    case "Inventory":
                        Console.WriteLine("Your have {0} item(s).", Inventory.Count);
                        foreach (Item Item in Inventory)
                        {
                            Console.WriteLine(Item.name);
                        }
                        break;
                    case "Step":

                        currentSteps = currentSteps + stepfinal;
                        stepsTaken = stepsTaken + stepfinal;

                        Console.WriteLine("Walked {0} steps.", stepfinal);
                        break;
                    case "Use":
                        Console.WriteLine("Use which item? Type \"cancel\" to cancel.");
                        foreach (Item Item in Inventory)
                        {
                            Console.WriteLine(Item.name);
                        }

                        string useInput = "None";
                        useInput = Console.ReadLine();
                        if (Inventory.Any(item => item.name.Equals(useInput, StringComparison.OrdinalIgnoreCase)))
                        {
                            Console.WriteLine("DEBUG: Used {0}!", useInput);
                            
                        } else
                        {
                            Console.WriteLine("That's not an valid item!");

                        }
                        break;
                }
            } while (currentSteps < stepGoal && currentButtons < buttonGoal);
        } while (!dead);

        //Gameover sequence
        Console.WriteLine("You died!");
        Console.WriteLine("You survived: {0} floors and were killed by: {1}", (currentFloor - 1), killedBy);
    }
}