namespace flechettes.Class;

using System;
using System.Collections.Generic;
using System.Linq;

public class DartsGame
{
    private Dictionary<string, Player> _players;
    private string _currentPlayer;
    private string _gameMode;
    private string _finishType;
    private string _gameStatus;
    private string _winner;
    private int _currentPlayerThrows;
    private int _lastThrowPoints;
    private const int MAX_THROWS_PER_TURN = 3;

    public DartsGame()
    {
        _players = new Dictionary<string, Player>();
        _gameStatus = "En attente";
        _currentPlayerThrows = 0;
    }

    public void SetGameMode(string gameMode)
    {
        _gameMode = gameMode;
    }

    public void SetFinishType(string finishType)
    {
        _finishType = finishType;
    }

    public void AddPlayer(string playerName)
    {
        if (!_players.ContainsKey(playerName))
        {
            var startingScore = _gameMode == "501" ? 501 : 301;
            _players[playerName] = new Player(playerName, startingScore);
        }
    }

    public void StartGame()
    {
        if (_players.Count >= 2)
        {
            _gameStatus = "En cours";
            _currentPlayer = _players.Keys.First();
            _currentPlayerThrows = 0;
        }
    }

    public ThrowResult ProcessThrows(string playerName, IEnumerable<DartThrow> throws)
    {
        if (_gameStatus != "En cours" || _currentPlayer != playerName)
        {
            return new ThrowResult { Message = "Ce n'est pas votre tour" };
        }

        var player = _players[playerName];
        var originalScore = player.Score;
        var totalPoints = 0;
        var throwCount = 0;

        foreach (var dartThrow in throws)
        {
            if (_currentPlayerThrows >= MAX_THROWS_PER_TURN)
                break;

            var points = CalculatePoints(dartThrow);
            totalPoints += points;
            throwCount++;
            _currentPlayerThrows++;
            _lastThrowPoints = points;

            // Vérifier si le joueur peut finir avec ce lancer
            if (player.Score - totalPoints == 0)
            {
                if (_finishType == "double out" && dartThrow.Multiplicateur != "Double")
                {
                    // Bust - doit finir avec un double
                    player.Score = originalScore;
                    player.AddThrow(throwCount, 0);
                    NextPlayer();
                    return new ThrowResult { Message = "Bust! Doit finir avec un double" };
                }
                else
                {
                    // Victoire
                    player.Score = 0;
                    player.AddThrow(throwCount, totalPoints);
                    _winner = playerName;
                    _gameStatus = "Terminée";
                    return new ThrowResult { Message = $"{playerName} a gagné!" };
                }
            }
            else if (player.Score - totalPoints < 0 || player.Score - totalPoints == 1)
            {
                // Bust - score négatif ou score de 1 (impossible de finir avec double)
                player.Score = originalScore;
                player.AddThrow(throwCount, 0);
                NextPlayer();
                return new ThrowResult { Message = $"Bust! Score remis à {originalScore}" };
            }
        }

        // Appliquer les points si pas de bust
        player.Score -= totalPoints;
        player.AddThrow(throwCount, totalPoints);

        if (_currentPlayerThrows >= MAX_THROWS_PER_TURN)
        {
            NextPlayer();
        }

        return new ThrowResult { Message = "Tour terminé" };
    }

    public ThrowResult ProcessSingleThrow(string playerName, DartThrow dartThrow)
    {
        ValidateThrow(dartThrow);
        return ProcessThrows(playerName, new[] { dartThrow });
    }

    private void ValidateThrow(DartThrow dartThrow)
    {
        if (dartThrow.Zone < 1 || dartThrow.Zone > 25)
        {
            throw new ArgumentException("Zone invalide");
        }
    }

    private int CalculatePoints(DartThrow dartThrow)
    {
        if (dartThrow.Zone == 0) return 0; // Miss

        var basePoints = dartThrow.Zone;
        
        switch (dartThrow.Multiplicateur.ToLower())
        {
            case "single":
                return basePoints;
            case "double":
                return basePoints * 2;
            case "triple":
                return basePoints * 3;
            case "bull":
                return 50; // Bull's eye
            case "miss":
                return 0;
            default:
                return basePoints;
        }
    }

    private void NextPlayer()
    {
        var playerNames = _players.Keys.ToList();
        var currentIndex = playerNames.IndexOf(_currentPlayer);
        var nextIndex = (currentIndex + 1) % playerNames.Count;
        _currentPlayer = playerNames[nextIndex];
        _currentPlayerThrows = 0;
    }

    public void SetPlayerScore(string playerName, int score)
    {
        if (_players.ContainsKey(playerName))
        {
            _players[playerName].Score = score;
        }
    }

    public int GetPlayerScore(string playerName)
    {
        return _players.ContainsKey(playerName) ? _players[playerName].Score : 0;
    }

    public string GetCurrentPlayer()
    {
        return _currentPlayer;
    }

    public string GetGameStatus()
    {
        return _gameStatus;
    }

    public string GetWinner()
    {
        return _winner;
    }

    public int GetLastThrowPoints()
    {
        return _lastThrowPoints;
    }

    public bool CanPlayerThrow(string playerName)
    {
        return _currentPlayer == playerName && _currentPlayerThrows < MAX_THROWS_PER_TURN;
    }

    public double GetPlayerAverage(string playerName)
    {
        if (_players.ContainsKey(playerName))
        {
            return _players[playerName].GetAverage();
        }
        return 0;
    }

    public int GetPlayerTotalDarts(string playerName)
    {
        if (_players.ContainsKey(playerName))
        {
            return _players[playerName].TotalDarts;
        }
        return 0;
    }

    public int GetPlayerBestScore(string playerName)
    {
        if (_players.ContainsKey(playerName))
        {
            return _players[playerName].BestScore;
        }
        return 0;
    }
}

public class Player
{
    public string Name { get; }
    public int Score { get; set; }
    public int TotalDarts { get; private set; }
    public int TotalPoints { get; private set; }
    public int BestScore { get; private set; }

    public Player(string name, int startingScore)
    {
        Name = name;
        Score = startingScore;
        TotalDarts = 0;
        TotalPoints = 0;
        BestScore = 0;
    }

    public void AddThrow(int darts, int points)
    {
        TotalDarts += darts;
        TotalPoints += points;
        if (points > BestScore)
        {
            BestScore = points;
        }
    }

    public double GetAverage()
    {
        return TotalDarts > 0 ? (double)TotalPoints / TotalDarts * 3 : 0;
    }
}

public class ThrowResult
{
    public string Message { get; set; }
    public bool IsValid { get; set; } = true;
}

public class DartThrow
{
    public int Zone { get; set; }
    public string Multiplicateur { get; set; }
}
