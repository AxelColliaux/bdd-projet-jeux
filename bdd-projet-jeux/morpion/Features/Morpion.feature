Feature: Morpion

    Scenario: Début de partie avec grille vide
        Given une nouvelle partie de morpion
        Then toutes les cases sont vides
        
    Scenario: Le premier joueur est toujours X
        Given une nouvelle partie de morpion
        Then le joueur courant est X

    Scenario: Placer un symbole sur une case vide
        Given une nouvelle partie de morpion
        When le joueur X joue en 0,0
        Then la case 0,0 contient X
        
    Scenario: Partie en cours
        Given une nouvelle partie de morpion
        When le joueur X joue en 0,0
        Then la partie est en cours
        
    Scenario: Alternance des joueurs
        Given une nouvelle partie de morpion
        When le joueur X joue en 0,0
        Then le joueur courant est O
        When le joueur O joue en 0,1
        Then le joueur courant est X

    Scenario: Refuser un placement sur une case occupée
        Given une nouvelle partie de morpion
        And le joueur X joue en 0,0
        When le joueur O joue en 0,0
        Then le coup est refusé

    Scenario: Victoire par ligne horizontale
        Given une nouvelle partie de morpion
        And le joueur X joue en 0,0
        And le joueur O joue en 1,0
        And le joueur X joue en 0,1
        And le joueur O joue en 1,1
        And le joueur X joue en 0,2
        Then le joueur X a gagné

    Scenario: Victoire par ligne verticale
        Given une nouvelle partie de morpion
        And le joueur X joue en 0,0
        And le joueur O joue en 0,1
        And le joueur X joue en 1,0
        And le joueur O joue en 1,1
        And le joueur X joue en 2,0
        Then le joueur X a gagné

    Scenario: Victoire par diagonale
        Given une nouvelle partie de morpion
        And le joueur X joue en 0,0
        And le joueur O joue en 0,1
        And le joueur X joue en 1,1
        And le joueur O joue en 1,0
        And le joueur X joue en 2,2
        Then le joueur X a gagné

    Scenario: Match nul
        Given une nouvelle partie de morpion
        And le joueur X joue en 0,0
        And le joueur O joue en 0,1
        And le joueur X joue en 0,2
        And le joueur O joue en 1,1
        And le joueur X joue en 1,0
        And le joueur O joue en 1,2
        And le joueur X joue en 2,1
        And le joueur O joue en 2,0
        And le joueur X joue en 2,2
        Then la partie est nulle