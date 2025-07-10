class Program
{
    static Random r = new();

    static void Main()
    {
        int balance = 100;//peníze ve hře
    // ---Spustění programu---
        Console.WriteLine("Vítej v kasínu Las Vegas!");
        Console.WriteLine($"Máš {balance} dolarů.");
    // ---Výběr hry---
        while (true)
        {
            Console.WriteLine("\nDostupné hry: BlackJack, Kostky, Ruleta, Automaty");
            Console.Write("Kterou chceš hrát? (nebo napiš konec): ");
            string volba = Console.ReadLine()?.Trim().ToLower();

            if (volba == "konec") break;
        
            switch (volba)
            {
                case "blackjack":
                    balance = BlackJack(balance);
                    break;
                case "kostky":
                    balance = Kostky(balance);
                    break;
                case "ruleta":
                    balance = Ruleta(balance);
                    break;
                case "automaty":
                    balance = Automaty(balance);
                    break;
                default:
                    Console.WriteLine("Tuhle hru tu nemáme.");
                    break;
            }

            if (balance <= 0)
            {
                Console.WriteLine("Došly ti peníze. Konec hry.");
                break;
            }

            Console.WriteLine($"Aktuální zůstatek: {balance} dolarů.");
        }

        Console.WriteLine("Díky za návštěvu kasína!");
    }

    // --- BlackJack ---
    static int Draw() => r.Next(2, 12); //Losuje číslo od 2 do 11 jako hodnotu karty.

    static int Score(List<int> cards) //Sečte hodnoty karet a upravuje hodnoty es (11 → 1), pokud je součet nad 21.
    {
        int sum = 0, aces = 0; // Inicializujeme součet a počítadlo es.
        foreach (var c in cards) // Přičítáme je do sum, Pokud je karta 11 (eso), započteme si ji do es.
        {
            sum += c;
            if (c == 11) aces++;
        }
        while (sum > 21 && aces > 0) // Pokud má hráč více než 21 a má eso, snížíme hodnotu jednoho esa z 11 na 1 (odčteme 10).
        {
            sum -= 10;
            aces--;
        }
        return sum;
    }

    static void PrintCards(string who, List<int> cards) //Vypíše všechny karty jednoho hráče (hráče nebo dealera), barevně, a jejich součet.
    {
        Console.Write(who + ": "); // napíše komu karty patří
        for (int i = 0; i < cards.Count; i++)
        {
            Console.ForegroundColor = ConsoleColor.Yellow; //Vypíše všechny hodnoty karet oddělené čárkami a žlutě.
            Console.Write(cards[i]);
            Console.ResetColor();
            if (i < cards.Count - 1) Console.Write(", ");
        }
        Console.Write(" = "); //Vypíše součet: = XX. Vypočte skóre pomocí metody Score.
        int score = Score(cards);
        if (score <= 21) Console.ForegroundColor = ConsoleColor.Green; // Pokud skóre nepřesáhne 21, vypíše ho zeleně. Jinak červeně (přetažení).
        else Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(score);
        Console.ResetColor();
    }

    static int BlackJack(int balance)
    {
        Console.WriteLine("\n--- BlackJack ---");
        Console.WriteLine($"Máš {balance} dolarů. Kolik chceš vsadit?");
        int bet;
        while (true) //Čeká, dokud hráč nezadá platné číslo sázky.
        {
            if (int.TryParse(Console.ReadLine(), out bet) && bet > 0 && bet <= balance)
                break;
            Console.WriteLine("Neplatná sázka. Zadej číslo od 1 do " + balance);
        }
        balance -= bet; //Okamžitě odečte vsazenou částku ze zůstatku.

        var player = new List<int> { Draw(), Draw() }; //Hráč i dealer dostanou dvě náhodné karty.
        var dealer = new List<int> { Draw(), Draw() };

        PrintCards("Ty", player); //Zobrazí hráčovy karty a jen jednu dealerovu (druhá je skrytá).
        Console.WriteLine("Dealer: " + dealer[0]);

        while (Score(player) < 21) //Dokud hráč má méně než 21: Může se rozhodnout „vzít kartu“ nebo „zůstat“
        {
            Console.WriteLine("1 - vzít kartu"); //Pokud vybere 1, přidá se mu nová karta.
            Console.WriteLine("2 - stát"); //Pokud vybere 2, nebude si brát další kartu
            string odp = Console.ReadLine();//přečte odpověď
            if (odp != "1") break;
            player.Add(Draw());
            PrintCards("Ty", player);
        }

        while (Score(dealer) < 17) dealer.Add(Draw()); //Dealer tahá, dokud nemá 17 nebo víc. Potom se vypíšou jeho karty.
        PrintCards("Dealer", dealer);

        int ps = Score(player); //Získá bodový součet hráče (ps) 
        int ds = Score(dealer);// a dealera (ds).

        if (ps > 21) // Hráč přetáhl → prohra (červená).
            Console.ForegroundColor = ConsoleColor.Red;
        else if (ds > 21 || ps > ds) // Dealer přetáhl nebo hráč má víc → výhra (zelená).
        {
            Console.ForegroundColor = ConsoleColor.Green;
            balance += bet * 2;
        }
        else if (ps == ds) //žlutá a vrací se sázka.
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            balance += bet;
        }
        else //Jinak → prohra.
            Console.ForegroundColor = ConsoleColor.Red;

        if (ps > 21) //Výsledek se vypíše textově a vrací se aktualizovaný balance.
            Console.WriteLine("Prohra");
        else if (ds > 21 || ps > ds)
            Console.WriteLine("Výhra");
        else if (ps == ds)
            Console.WriteLine("Remíza");
        else
            Console.WriteLine("Prohra");

        Console.ResetColor();
        return balance;
    }

    // --- Automaty ---
    static int Automaty(int balance) //Definuje funkci pro hru „automaty“. Vrací aktualizovaný zůstatek (balance).
    {
        Console.WriteLine("\n--- Automaty ---");
        int cena = 10;
        if (balance < cena) //Pokud má hráč méně než 10 dolarů, hra ho nepustí točit a vrátí zůstatek.
        {
            Console.WriteLine("Nemáš dost peněz na zatočení.");
            return balance;
        }

        while (true) //Spustí smyčku, která umožní opakované točení, dokud hráč chce a má peníze.
        {
            Console.WriteLine($"Máš {balance} dolarů. Zatočit za {cena} dolarů? (ano/ne)"); //Zeptá se hráče, jestli chce zatočit. Pokud neodpoví „ano“, smyčka se ukončí.
            if (Console.ReadLine()?.ToLower() != "ano") break;

            balance -= cena;

            int c1 = r.Next(1, 7);
            int c2 = r.Next(1, 7);
            int c3 = r.Next(1, 7);

            Console.WriteLine($"Výsledek: {c1} {c2} {c3}"); //Zobrazí výsledek zatočení.

            int vyhra = 0; // Tři šestky = jackpot (50× výhra), Tři stejná čísla = 10×, Dvě stejná = 2×
            if (c1 == 6 && c2 == 6 && c3 == 6)
                vyhra = cena * 50;
            else if (c1 == c2 && c2 == c3)
                vyhra = cena * 10;
            else if (c1 == c2 || c2 == c3 || c1 == c3)
                vyhra = cena * 2;

            if (vyhra > 0) // Pokud je vyhra > 0, vypíše výhru, jinak oznámí prohru.
                Console.WriteLine($"Vyhrál jsi {vyhra} dolarů!");
            else
                Console.WriteLine("Nic."); 

            balance += vyhra;

            if (balance < cena) //Pokud už hráč nemá peníze na další kolo, smyčka se ukonč
            {
                Console.WriteLine("Nemáš dost peněz na další zatočení.");
                break;
            }
        }
        return balance;
    }

    // --- Ruleta ---
    static int Ruleta(int balance)
    {
        Console.WriteLine("\n--- Ruleta ---");
        var colorsRed = new int[] {1,3,5,7,9,12,14,16,18,19,21,23,25,27,30,32,34,36}; //Pole červených čísel podle standardní rulety.

        string GetColor(int n) //Funkce zjistí, jakou barvu má číslo na ruletě (zelená je jen nula).
        {
            if (n == 0) return "zelena"; 
            return Array.Exists(colorsRed, x => x == n) ? "cervena" : "cerna";
        }

        while (balance > 0)
        {
            Console.WriteLine($"Máš {balance} dolarů. Kolik chceš vsadit?");
            if (!int.TryParse(Console.ReadLine(), out int bet) || bet <= 0 || bet > balance) //Kontroluje platnost sázky.
            {
                Console.WriteLine("Neplatná sázka.");
                continue;
            }

            Console.WriteLine("Vyber typ sázky:");
            Console.WriteLine("1 - Číslo (výhra 36x)");
            Console.WriteLine("2 - Barva (cervena/cerna) (výhra 2x)");
            Console.WriteLine("3 - Sudé/Liché (výhra 2x)");
            Console.Write("Volba: ");
            string volba = Console.ReadLine();

            int cislo = r.Next(0, 37);
            string barva = GetColor(cislo);

            balance -= bet;

            switch (volba)
            {
                case "1":
                    Console.Write("Zadej číslo 0-36: ");
                    if (!int.TryParse(Console.ReadLine(), out int tip) || tip < 0 || tip > 36)
                    {
                        Console.WriteLine("Neplatné číslo.");
                        balance += bet;
                        continue;
                    }
                    Console.WriteLine($"Padlo číslo {cislo} ({barva})");
                    if (tip == cislo)
                    {
                        int vyhra = bet * 36;
                        balance += vyhra;
                        Console.WriteLine($"Vyhrál jsi {vyhra} dolarů!");
                    }
                    else
                        Console.WriteLine("Prohra.");
                    break;

                case "2":
                    Console.Write("Zvol barvu (cervena/cerna): ");
                    string tipBarva = Console.ReadLine().ToLower();
                    Console.WriteLine($"Padlo číslo {cislo} ({barva})");
                    if (tipBarva != "cervena" && tipBarva != "cerna")
                    {
                        Console.WriteLine("Neplatná barva.");
                        balance += bet;
                        continue;
                    }
                    if (tipBarva == barva)
                    {
                        balance += bet * 2;
                        Console.WriteLine($"Vyhrál jsi {bet * 2} dolarů!");
                    }
                    else Console.WriteLine("Prohra.");
                    break;

                case "3":
                    Console.Write("Zvol sude/liche: ");
                    string typ = Console.ReadLine().ToLower();
                    Console.WriteLine($"Padlo číslo {cislo} ({barva})");
                    if (typ != "sude" && typ != "liche")
                    {
                        Console.WriteLine("Neplatná volba.");
                        balance += bet;
                        continue;
                    }
                    if (cislo == 0)
                    {
                        Console.WriteLine("Padla nula, prohra.");
                        break;
                    }
                    if ((cislo % 2 == 0 && typ == "sude") || (cislo % 2 != 0 && typ == "liche"))
                    {
                        balance += bet * 2;
                        Console.WriteLine($"Vyhrál jsi {bet * 2} dolarů!");
                    }
                    else Console.WriteLine("Prohra.");
                    break;

                default:
                    Console.WriteLine("Neplatná volba sázky.");
                    balance += bet;
                    break;
            }

            if (balance <= 0)
            {
                Console.WriteLine("Došly ti peníze.");
                break;
            }

            Console.Write("Chceš hrát dál? (a/n): ");
            if (Console.ReadLine()?.ToLower() != "a")
                break;
        }
        return balance;
    }

    // --- Kostky ---
    static int Kostky(int balance)
    {
        Console.WriteLine("\n--- Kostky ---");
        int cena = 10;
        if (balance < cena)
        {
            Console.WriteLine("Nemáš dost peněz na sázku.");
            return balance;
        }

        while (true)
        {
            Console.WriteLine($"Máš {balance} dolarů. Chceš vsadit {cena} dolarů? (ano/ne)"); //Hráč je opakovaně tázán, zda chce pokračovat.
            if (Console.ReadLine()?.ToLower() != "ano") break;

            balance -= cena;

            int d1 = r.Next(1, 7);
            int d2 = r.Next(1, 7);
            int soucet = d1 + d2;

            Console.WriteLine($"Padly kostky: {d1} a {d2}, součet {soucet}");

            if (soucet >= 8) //Pokud je součet 8 nebo víc, hráč vyhrává dvojnásobek. Jinak prohra.
            {
                int vyhra = cena * 2;
                balance += vyhra;
                Console.WriteLine($"Vyhrál jsi {vyhra} dolarů!");
            }
            else
            {
                Console.WriteLine("Prohra.");
            }

            if (balance < cena)
            {
                Console.WriteLine("Nemáš dost peněz na další sázku.");
                break;
            }
        }

        return balance;
    }
}
