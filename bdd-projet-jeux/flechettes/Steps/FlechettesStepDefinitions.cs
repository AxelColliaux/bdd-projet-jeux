using flechettesClass;
using TechTalk.SpecFlow.Assist;
using FluentAssertions;

namespace flechettes.Steps;

[Binding]
public class FlechettesStepDefinitions
{
    private DartsGame _game;
    private string _lastMessage;
    private Exception _lastException;

    [Given(@"une nouvelle partie de fléchettes est créée")]
    public void GivenUneNouvellePartieDeFlechettesEstCreee()
    {
        _game = new DartsGame();
    }

    [Given(@"le mode de jeu est ""(.*)""")]
    public void GivenLeModeDeJeuEst(string gameMode)
    {
        _game.SetGameMode(gameMode);
    }

    [Given(@"la partie nécessite un finish ""(.*)""")]
    public void GivenLaPartieNecessiteUnFinish(string finishType)
    {
        _game.SetFinishType(finishType);
    }

    [Given(@"les joueurs ""(.*)"" et ""(.*)"" rejoignent la partie")]
    public void GivenLesJoueursEtRejoignentLaPartie(string player1, string player2)
    {
        _game.AddPlayer(player1);
        _game.AddPlayer(player2);
    }

    [Given(@"la partie commence")]
    [When(@"la partie commence")]
    public void WhenLaPartieCommence()
    {
        _game.StartGame();
    }

    [Given(@"le score de ""(.*)"" est (.*)")]
    public void GivenLeScoreDeEst(string playerName, int score)
    {
        _game.SetPlayerScore(playerName, score);
    }

    [When(@"""(.*)"" lance et marque les points suivants:")]
    public void WhenLanceEtMarqueLesPointsSuivants(string playerName, Table table)
    {
        // Conversion des données du tableau vers le type Class.DartThrow
        var tableData = table.CreateSet<DartThrowDto>();
        var throws = tableData.Select(dto => new flechettesClass.DartThrow
        {
            Zone = dto.Zone,
            Multiplicateur = dto.Multiplicateur
        });
        
        var result = _game.ProcessThrows(playerName, throws);
        _lastMessage = result.Message;
    }

    [When(@"""(.*)"" lance dans la zone (.*) avec multiplicateur (.*)")]
    public void WhenLanceDansLaZoneAvecMultiplicateur(string playerName, int zone, string multiplier)
    {
        var dartThrow = new flechettesClass.DartThrow
        {
            Zone = zone,
            Multiplicateur = multiplier
        };
        var result = _game.ProcessSingleThrow(playerName, dartThrow);
        _lastMessage = result.Message;
    }

    [When(@"""(.*)"" tente de lancer dans une zone invalide ""(.*)""")]
    public void WhenTenteDeLancerDansUneZoneInvalide(string playerName, string invalidZone)
    {
        try
        {
            var dartThrow = new flechettesClass.DartThrow
            {
                Zone = int.Parse(invalidZone),
                Multiplicateur = "Single"
            };
            _game.ProcessSingleThrow(playerName, dartThrow);
        }
        catch (Exception ex)
        {
            _lastException = ex;
        }
    }

    [When(@"""(.*)"" lance (.*) fléchettes")]
    public void WhenLanceFlechettes(string playerName, int numberOfThrows)
    {
        for (int i = 0; i < numberOfThrows; i++)
        {
            var dartThrow = new flechettesClass.DartThrow
            {
                Zone = 1,
                Multiplicateur = "Single"
            };
            _game.ProcessSingleThrow(playerName, dartThrow);
        }
    }

    [Then(@"le score de ""(.*)"" devrait être (.*)")]
    public void ThenLeScoreDeDevraitEtre(string playerName, int expectedScore)
    {
        _game.GetPlayerScore(playerName).Should().Be(expectedScore);
    }

    [Then(@"c'est le tour de ""(.*)""")]
    public void ThenCestLeTourDe(string playerName)
    {
        _game.GetCurrentPlayer().Should().Be(playerName);
    }

    [Then(@"la partie devrait être ""(.*)""")]
    public void ThenLaPartieDevraitEtre(string gameStatus)
    {
        _game.GetGameStatus().Should().Be(gameStatus);
    }

    [Then(@"""(.*)"" devrait être le gagnant")]
    public void ThenDevraitEtreLe(string playerName)
    {
        _game.GetWinner().Should().Be(playerName);
    }

    [Then(@"le message devrait être ""(.*)""")]
    public void ThenLeMessageDevraitEtre(string expectedMessage)
    {
        _lastMessage.Should().Be(expectedMessage);
    }

    [Then(@"les points marqués devraient être (.*)")]
    public void ThenLesPointsMarquesDevraientEtre(int expectedPoints)
    {
        _game.GetLastThrowPoints().Should().Be(expectedPoints);
    }

    [Then(@"une erreur devrait être levée ""(.*)""")]
    public void ThenUneErreurDevraitEtreLevee(string expectedError)
    {
        _lastException.Should().NotBeNull();
        _lastException.Message.Should().Contain(expectedError);
    }

    [Then(@"le score de ""(.*)"" devrait rester (.*)")]
    public void ThenLeScoreDeDevraitRester(string playerName, int expectedScore)
    {
        _game.GetPlayerScore(playerName).Should().Be(expectedScore);
    }

    [Then(@"""(.*)"" ne peut plus lancer de fléchettes")]
    public void ThenNePeutPlusLancerDeFlechettes(string playerName)
    {
        _game.CanPlayerThrow(playerName).Should().BeFalse();
    }

    [Then(@"la moyenne de ""(.*)"" devrait être (.*)")]
    public void ThenLaMoyenneDeDevraitEtre(string playerName, double expectedAverage)
    {
        _game.GetPlayerAverage(playerName).Should().Be(expectedAverage);
    }

    [Then(@"le nombre total de fléchettes de ""(.*)"" devrait être (.*)")]
    public void ThenLeNombreTotalDeFlechettesDeDevraitEtre(string playerName, int expectedCount)
    {
        _game.GetPlayerTotalDarts(playerName).Should().Be(expectedCount);
    }

    [Then(@"le meilleur score de ""(.*)"" devrait être (.*)")]
    public void ThenLeMeilleurScoreDeDevraitEtre(string playerName, int expectedBestScore)
    {
        _game.GetPlayerBestScore(playerName).Should().Be(expectedBestScore);
    }
}

// Classe DTO pour mapper les données du tableau SpecFlow
public class DartThrowDto
{
    public int Zone { get; set; }
    public string Multiplicateur { get; set; }
}