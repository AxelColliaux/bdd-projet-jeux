namespace Morpions.Models;

public class Morpion
{
    public enum Symbole { Vide, X, O }
    public Symbole[,] Grille { get; set; }
    public Symbole JoueurCourant { get; set; }
    public bool EstTerminee { get; set; }
    public Symbole Gagnant { get; set; }

    public Morpion()
    {
        Grille = new Symbole[3, 3];
        JoueurCourant = Symbole.X;
        EstTerminee = false;
        Gagnant = Symbole.Vide;
    }

    public bool Jouer(int ligne, int colonne)
    {
        if (EstTerminee) return false;
        if (Grille[ligne, colonne] != Symbole.Vide) return false;

        Grille[ligne, colonne] = JoueurCourant;
        if (VerifierVictoire(ligne, colonne))
        {
            EstTerminee = true;
            Gagnant = JoueurCourant;
        }
        else if (VerifierMatchNul())
        {
            EstTerminee = true;
            Gagnant = Symbole.Vide;
        }
        else
        {
            JoueurCourant = JoueurCourant == Symbole.X ? Symbole.O : Symbole.X;
        }
        return true;
    }

    private bool VerifierVictoire(int ligne, int colonne)
    {
        Symbole s = JoueurCourant;
        if (Grille[ligne, 0] == s && Grille[ligne, 1] == s && Grille[ligne, 2] == s) return true;
        if (Grille[0, colonne] == s && Grille[1, colonne] == s && Grille[2, colonne] == s) return true;
        if (ligne == colonne && Grille[0, 0] == s && Grille[1, 1] == s && Grille[2, 2] == s) return true;
        if (ligne + colonne == 2 && Grille[0, 2] == s && Grille[1, 1] == s && Grille[2, 0] == s) return true;
        return false;
    }

    private bool VerifierMatchNul()
    {
        foreach (Symbole caseGrille in Grille)
            if (caseGrille == Symbole.Vide) return false;
        return true;
    }
}