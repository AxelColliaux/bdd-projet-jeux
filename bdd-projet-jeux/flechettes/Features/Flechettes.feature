Feature: Jeu de Fléchettes 501
  En tant que joueur de fléchettes
  Je veux pouvoir jouer une partie de fléchettes 501
  Afin de pouvoir défier mes amis et suivre mon score

  Background:
    Given une nouvelle partie de fléchettes est créée
    And le mode de jeu est "501"
    And la partie nécessite un finish "double out"

  Scenario: Commencer une partie avec deux joueurs
    Given les joueurs "Alice" et "Bob" rejoignent la partie
    When la partie commence
    Then le score de "Alice" devrait être 501
    And le score de "Bob" devrait être 501
    And c'est le tour de "Alice"
    And la partie devrait être "En cours"

  Scenario: Lancer des fléchettes et calculer le score
    Given les joueurs "Alice" et "Bob" rejoignent la partie
    And la partie commence
    When "Alice" lance et marque les points suivants:
      | Fléchette | Zone | Multiplicateur | Points |
      | 1         | 20   | Single         | 20     |
      | 2         | 19   | Double         | 38     |
      | 3         | 18   | Triple         | 54     |
    Then le score de "Alice" devrait être 389
    And c'est le tour de "Bob"

  Scenario: Calcul du score avec miss
    Given les joueurs "Alice" et "Bob" rejoignent la partie
    And la partie commence
    When "Alice" lance et marque les points suivants:
      | Fléchette | Zone | Multiplicateur | Points |
      | 1         | 20   | Single         | 20     |
      | 2         | 0    | Miss           | 0      |
      | 3         | 19   | Single         | 19     |
    Then le score de "Alice" devrait être 462
    And c'est le tour de "Bob"

  Scenario: Bust - dépassement du score (score négatif)
    Given les joueurs "Alice" et "Bob" rejoignent la partie
    And la partie commence
    And le score de "Alice" est 50
    When "Alice" lance et marque les points suivants:
      | Fléchette | Zone | Multiplicateur | Points |
      | 1         | 20   | Triple         | 60     |
    Then le score de "Alice" devrait être 50
    And le message devrait être "Bust! Score remis à 50"
    And c'est le tour de "Bob"

  Scenario: Bust - finir sur un score de 1 (impossible de finir avec double)
    Given les joueurs "Alice" et "Bob" rejoignent la partie
    And la partie commence
    And le score de "Alice" est 3
    When "Alice" lance et marque les points suivants:
      | Fléchette | Zone | Multiplicateur | Points |
      | 1         | 1    | Single         | 1      |
      | 2         | 1    | Single         | 1      |
    Then le score de "Alice" devrait être 3
    And le message devrait être "Bust! Score remis à 3"
    And c'est le tour de "Bob"

  Scenario: Victoire avec double out
    Given les joueurs "Alice" et "Bob" rejoignent la partie
    And la partie commence
    And le score de "Alice" est 32
    When "Alice" lance et marque les points suivants:
      | Fléchette | Zone | Multiplicateur | Points |
      | 1         | 16   | Double         | 32     |
    Then le score de "Alice" devrait être 0
    And "Alice" devrait être le gagnant
    And la partie devrait être "Terminée"
    And le message devrait être "Alice a gagné!"

  Scenario: Tentative de victoire sans double out
    Given les joueurs "Alice" et "Bob" rejoignent la partie
    And la partie commence
    And le score de "Alice" est 20
    When "Alice" lance et marque les points suivants:
      | Fléchette | Zone | Multiplicateur | Points |
      | 1         | 20   | Single         | 20     |
    Then le score de "Alice" devrait être 20
    And le message devrait être "Bust! Doit finir avec un double"
    And c'est le tour de "Bob"

  Scenario: Partie complète avec alternance de joueurs
    Given les joueurs "Alice" et "Bob" rejoignent la partie
    And la partie commence
    # Tour d'Alice
    When "Alice" lance et marque les points suivants:
      | Fléchette | Zone | Multiplicateur | Points |
      | 1         | 20   | Triple         | 60     |
      | 2         | 20   | Triple         | 60     |
      | 3         | 20   | Triple         | 60     |
    Then le score de "Alice" devrait être 321
    And c'est le tour de "Bob"
    # Tour de Bob
    When "Bob" lance et marque les points suivants:
      | Fléchette | Zone | Multiplicateur | Points |
      | 1         | 19   | Triple         | 57     |
      | 2         | 19   | Triple         | 57     |
      | 3         | 19   | Triple         | 57     |
    Then le score de "Bob" devrait être 330
    And c'est le tour de "Alice"

  Scenario: Gestion des différentes zones du plateau
    Given les joueurs "Alice" et "Bob" rejoignent la partie
    And la partie commence
    When "Alice" lance et marque les points suivants:
      | Fléchette | Zone | Multiplicateur | Points |
      | 1         | 25   | Single         | 25     |
      | 2         | 25   | Double         | 50     |
      | 3         | 25   | Bull           | 50     |
    Then le score de "Alice" devrait être 376

  Scenario Outline: Calculs de score avec différents multiplicateurs
    Given les joueurs "Alice" et "Bob" rejoignent la partie
    And la partie commence
    When "Alice" lance dans la zone <zone> avec multiplicateur <multiplicateur>
    Then les points marqués devraient être <points>
    And le score de "Alice" devrait être <score_final>

    Examples:
      | zone | multiplicateur | points | score_final |
      | 1    | Single         | 1      | 500         |
      | 1    | Double         | 2      | 499         |
      | 1    | Triple         | 3      | 498         |
      | 20   | Single         | 20     | 481         |
      | 20   | Double         | 40     | 461         |
      | 20   | Triple         | 60     | 441         |
      | 25   | Single         | 25     | 476         |
      | 25   | Double         | 50     | 451         |
      | 25   | Bull           | 50     | 451         |

  Scenario: Validation des lancers invalides
    Given les joueurs "Alice" et "Bob" rejoignent la partie
    And la partie commence
    When "Alice" tente de lancer dans une zone invalide "0"
    Then une erreur devrait être levée "Zone invalide"
    And le score de "Alice" devrait rester 501

  Scenario: Limite de 3 fléchettes par tour
    Given les joueurs "Alice" et "Bob" rejoignent la partie
    And la partie commence
    When "Alice" lance 3 fléchettes
    Then "Alice" ne peut plus lancer de fléchettes
    And c'est le tour de "Bob"

  Scenario: Statistiques de la partie
    Given les joueurs "Alice" et "Bob" rejoignent la partie
    And la partie commence
    When "Alice" lance et marque les points suivants:
      | Fléchette | Zone | Multiplicateur | Points |
      | 1         | 20   | Triple         | 60     |
      | 2         | 20   | Triple         | 60     |
      | 3         | 20   | Triple         | 60     |
    Then la moyenne de "Alice" devrait être 180
    And le nombre total de fléchettes de "Alice" devrait être 3
    And le meilleur score de "Alice" devrait être 180