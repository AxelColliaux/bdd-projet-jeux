using Morpions.Models;
using FluentAssertions;

namespace Morpions.Steps;

[Binding]
public class MorpionStepsDefinitions
{
    private Morpion jeu;
    private bool lastMoveResult;

    [Given("une nouvelle partie de morpion")]
    public void GivenUneNouvellePartie()
    {
        jeu = new Morpion();
    }

    [StepDefinition(@"le joueur (X|O) joue en (\d),(\d)")]
    public void LeJoueurJoueEn(string joueur, int ligne, int colonne)
    {
        if ((joueur == "X" && jeu.JoueurCourant != Morpion.Symbole.X) ||
            (joueur == "O" && jeu.JoueurCourant != Morpion.Symbole.O))
            throw new Exception("Mauvais joueur");

        lastMoveResult = jeu.Jouer(ligne, colonne);
    }

    [Then(@"la case (\d),(\d) contient (X|O)")]
    public void ThenLaCaseContient(int ligne, int colonne, string symbole)
    {
        var attendu = symbole == "X" ? Morpion.Symbole.X : Morpion.Symbole.O;
        jeu.Grille[ligne, colonne].Should().Be(attendu);
    }

    [Then("toutes les cases sont vides")]
    public void ThenToutesLesCasesSontVides()
    {
        for (int i = 0; i < 3; i++)
        for (int j = 0; j < 3; j++)
            jeu.Grille[i, j].Should().Be(Morpion.Symbole.Vide);
    }

    [Then("le coup est refusé")]
    public void ThenLeCoupEstRefuse()
    {
        lastMoveResult.Should().BeFalse();
    }

    [Then(@"le joueur (X|O) a gagné")]
    public void ThenLeJoueurAGagne(string joueur)
    {
        var attendu = joueur == "X" ? Morpion.Symbole.X : Morpion.Symbole.O;
        jeu.EstTerminee.Should().BeTrue();
        jeu.Gagnant.Should().Be(attendu);
    }

    [Then("la partie est nulle")]
    public void ThenLaPartieEstNulle()
    {
        jeu.EstTerminee.Should().BeTrue();
        jeu.Gagnant.Should().Be(Morpion.Symbole.Vide);
    }
    
    [Then(@"le joueur courant est (X|O)")]
    public void ThenLeJoueurCourantEst(string joueur)
    {
        var attendu = joueur == "X" ? Morpion.Symbole.X : Morpion.Symbole.O;
        jeu.JoueurCourant.Should().Be(attendu);
    }
    
    [Then("la partie est en cours")]
    public void ThenLaPartieEstEnCours()
    {
        jeu.EstTerminee.Should().BeFalse();
    }
}