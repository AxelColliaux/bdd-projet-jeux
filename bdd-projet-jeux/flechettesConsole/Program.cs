using flechettesClass;
using System;

namespace flechettesConsole
{
    class Program
    {
        private static DartsGame _game = new DartsGame();
        private static bool _gameRunning = true;

        static void Main(string[] args)
        {
            Console.WriteLine("=== JEU DE FLECHETTES ===");
            Console.WriteLine("Bienvenue dans le jeu de fléchettes!");
            Console.WriteLine();

            SetupGame();
            
            while (_gameRunning && _game.GetGameStatus() != "Terminée")
            {
                ShowGameStatus();
                ProcessPlayerTurn();
            }

            if (_game.GetGameStatus() == "Terminée")
            {
                ShowFinalResults();
            }

            Console.WriteLine("\nAppuyez sur une touche pour quitter...");
            Console.ReadKey();
        }

        private static void SetupGame()
        {
            // Configuration du mode de jeu
            Console.WriteLine("Choisissez le mode de jeu:");
            Console.WriteLine("1. 301 points");
            Console.WriteLine("2. 501 points");
            Console.Write("Votre choix (1 ou 2): ");
            
            var modeChoice = Console.ReadLine();
            var gameMode = modeChoice == "1" ? "301" : "501";
            _game.SetGameMode(gameMode);
            
            Console.WriteLine($"Mode de jeu sélectionné: {gameMode} points");

            // Configuration du type de fin
            Console.WriteLine("\nChoisissez le type de fin:");
            Console.WriteLine("1. Normale (any out)");
            Console.WriteLine("2. Double out (doit finir avec un double)");
            Console.Write("Votre choix (1 ou 2): ");
            
            var finishChoice = Console.ReadLine();
            var finishType = finishChoice == "2" ? "double out" : "any out";
            _game.SetFinishType(finishType);
            
            Console.WriteLine($"Type de fin sélectionné: {finishType}");

            // Ajout des joueurs
            Console.WriteLine("\n=== AJOUT DES JOUEURS ===");
            Console.Write("Nombre de joueurs (2-4): ");
            
            if (int.TryParse(Console.ReadLine(), out int playerCount) && playerCount >= 2 && playerCount <= 4)
            {
                for (int i = 1; i <= playerCount; i++)
                {
                    Console.Write($"Nom du joueur {i}: ");
                    var playerName = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(playerName))
                    {
                        _game.AddPlayer(playerName.Trim());
                        Console.WriteLine($"Joueur '{playerName}' ajouté!");
                    }
                }
            }
            else
            {
                // Joueurs par défaut
                _game.AddPlayer("Joueur 1");
                _game.AddPlayer("Joueur 2");
                Console.WriteLine("Joueurs par défaut ajoutés: Joueur 1 et Joueur 2");
            }

            _game.StartGame();
            Console.WriteLine("\n=== DEBUT DE LA PARTIE ===");
        }

        private static void ShowGameStatus()
        {
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine($"STATUT: {_game.GetGameStatus()}");
            Console.WriteLine($"JOUEUR ACTUEL: {_game.GetCurrentPlayer()}");
            Console.WriteLine(new string('=', 50));
            
            // Afficher les scores de tous les joueurs
            // Note: La classe DartsGame ne fournit pas de méthode pour lister tous les joueurs
            // Dans un vrai scénario, on ajouterait cette fonctionnalité
            Console.WriteLine($"Score actuel de {_game.GetCurrentPlayer()}: {_game.GetPlayerScore(_game.GetCurrentPlayer())} points");
        }

        private static void ProcessPlayerTurn()
        {
            var currentPlayer = _game.GetCurrentPlayer();
            Console.WriteLine($"\nTour de {currentPlayer}");
            Console.WriteLine("Choisissez une option:");
            Console.WriteLine("1. Lancer une fléchette");
            Console.WriteLine("2. Lancer 3 fléchettes");
            Console.WriteLine("3. Voir les statistiques");
            Console.WriteLine("4. Quitter");
            Console.Write("Votre choix: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ProcessSingleThrow(currentPlayer);
                    break;
                case "2":
                    ProcessMultipleThrows(currentPlayer);
                    break;
                case "3":
                    ShowPlayerStats(currentPlayer);
                    break;
                case "4":
                    _gameRunning = false;
                    break;
                default:
                    Console.WriteLine("Choix invalide!");
                    break;
            }
        }

        private static void ProcessSingleThrow(string playerName)
        {
            try
            {
                var dartThrow = GetDartThrowInput();
                var result = _game.ProcessSingleThrow(playerName, dartThrow);
                
                Console.WriteLine($"Résultat: {result.Message}");
                Console.WriteLine($"Points gagnés: {_game.GetLastThrowPoints()}");
                Console.WriteLine($"Score restant: {_game.GetPlayerScore(playerName)}");

                if (_game.GetGameStatus() == "Terminée")
                {
                    Console.WriteLine($"\n🎉 FELICITATIONS! {_game.GetWinner()} a gagné! 🎉");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
            }
        }

        private static void ProcessMultipleThrows(string playerName)
        {
            try
            {
                var throws = new List<DartThrow>();
                
                for (int i = 1; i <= 3; i++)
                {
                    if (!_game.CanPlayerThrow(playerName))
                        break;
                        
                    Console.WriteLine($"\nFléchette {i}/3:");
                    var dartThrow = GetDartThrowInput();
                    throws.Add(dartThrow);
                    
                    // Traiter le lancer individuellement pour voir le résultat immédiatement
                    var result = _game.ProcessSingleThrow(playerName, dartThrow);
                    Console.WriteLine($"Points: {_game.GetLastThrowPoints()} | {result.Message}");
                    Console.WriteLine($"Score restant: {_game.GetPlayerScore(playerName)}");
                    
                    if (_game.GetGameStatus() == "Terminée")
                    {
                        Console.WriteLine($"\n🎉 FELICITATIONS! {_game.GetWinner()} a gagné! 🎉");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
            }
        }

        private static DartThrow GetDartThrowInput()
        {
            Console.WriteLine("\nEntrez votre lancer:");
            Console.WriteLine("Zones: 1-20 (segments normaux), 25 (bull)");
            Console.WriteLine("Multiplicateurs: single, double, triple, bull, miss");
            
            Console.Write("Zone (1-25): ");
            if (!int.TryParse(Console.ReadLine(), out int zone) || zone < 0 || zone > 25)
            {
                Console.WriteLine("Zone invalide, utilisation de 0 (miss)");
                zone = 0;
            }

            Console.Write("Multiplicateur (single/double/triple/bull/miss): ");
            var multiplicateur = Console.ReadLine()?.ToLower() ?? "miss";
            
            if (!IsValidMultiplicateur(multiplicateur))
            {
                Console.WriteLine("Multiplicateur invalide, utilisation de 'miss'");
                multiplicateur = "miss";
            }

            return new DartThrow
            {
                Zone = zone,
                Multiplicateur = multiplicateur
            };
        }

        private static bool IsValidMultiplicateur(string multiplicateur)
        {
            return multiplicateur == "single" || multiplicateur == "double" || 
                   multiplicateur == "triple" || multiplicateur == "bull" || 
                   multiplicateur == "miss";
        }

        private static void ShowPlayerStats(string playerName)
        {
            Console.WriteLine($"\n=== STATISTIQUES DE {playerName.ToUpper()} ===");
            Console.WriteLine($"Score actuel: {_game.GetPlayerScore(playerName)}");
            Console.WriteLine($"Moyenne: {_game.GetPlayerAverage(playerName):F2}");
            Console.WriteLine($"Total de fléchettes: {_game.GetPlayerTotalDarts(playerName)}");
            Console.WriteLine($"Meilleur score (par tour): {_game.GetPlayerBestScore(playerName)}");
        }

        private static void ShowFinalResults()
        {
            Console.WriteLine("\n" + new string('*', 60));
            Console.WriteLine("                    PARTIE TERMINEE");
            Console.WriteLine(new string('*', 60));
            Console.WriteLine($"🏆 VAINQUEUR: {_game.GetWinner()} 🏆");
            Console.WriteLine(new string('*', 60));
        }
    }
}