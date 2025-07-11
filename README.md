# Documentation et justification des choix techniques
## Projet BDD - Jeux (Morpion et Fléchettes 501)

---

## a) Analyse et justification des scénarios

### i) Identification des cas de test

#### **Morpion - Stratégie de couverture**

**Cas nominaux :**
- *Début de partie avec grille vide* : Valide l'état initial du jeu
- *Le premier joueur est toujours X* : Vérifie les règles métier de base
- *Placer un symbole sur une case vide* : Teste le mécanisme de jeu principal
- *Alternance des joueurs* : Valide la logique de tour par tour

**Cas de victoire :**
- *Victoire par ligne horizontale/verticale/diagonale* : Couvre tous les patterns de victoire possibles
- Ces scénarios garantissent que l'algorithme de détection de victoire fonctionne dans tous les cas

**Cas limites :**
- *Match nul* : Teste le cas où la grille est pleine sans gagnant
- *Refuser un placement sur une case occupée* : Valide les règles de placement

**Justification :** Cette approche exhaustive garantit que toutes les règles du morpion sont respectées. L'ordre logique (état initial → actions de base → cas complexes) facilite le développement incrémental.

#### **Fléchettes 501 - Stratégie de couverture**

**Cas nominaux :**
- *Commencer une partie avec deux joueurs* : Initialisation et état de départ
- *Lancer des fléchettes et calculer le score* : Mécanisme de base du scoring
- *Calcul du score avec miss* : Gestion des échecs de tir

**Cas de victoire et échec (Bust) :**
- *Victoire avec double out* : Règle de fin officielle du 501
- *Bust - dépassement du score* : Protection contre les scores négatifs
- *Bust - finir sur un score de 1* : Cas spécifique du double out
- *Tentative de victoire sans double out* : Validation des règles de fin

**Cas avancés :**
- *Gestion des différentes zones du plateau* : Bull, zones spéciales
- *Calculs de score avec différents multiplicateurs* : Utilisation des Examples pour couvrir tous les cas
- *Statistiques de la partie* : Fonctionnalités métier avancées

**Cas d'erreur :**
- *Validation des lancers invalides* : Gestion des erreurs utilisateur
- *Limite de 3 fléchettes par tour* : Respect des règles officielles

**Justification :** La complexité du 501 nécessite une couverture extensive. L'utilisation des Scenario Outline avec Examples permet de tester efficacement tous les multiplicateurs sans duplication de code.

### ii) Priorisation des scénarios

#### **Scénarios critiques (développés en premier) :**

**Morpion :**
1. État initial et placement de base (fondamentaux)
2. Alternance des joueurs (logique de jeu)
3. Détection de victoire (règles métier critiques)

**Fléchettes :**
1. Initialisation et scoring de base
2. Règles de Bust (intégrité du jeu)
3. Double out (règle officielle critique)

#### **Scénarios secondaires :**
- Statistiques (fonctionnalités d'amélioration de l'expérience)
- Cas d'erreur spécifiques (robustesse)
- Gestion des zones spéciales (complétude)

**Justification :** Cette priorisation suit le principe "walking skeleton" du BDD, en implémentant d'abord les fonctionnalités core puis en ajoutant les raffinements.

---

## b) Architecture et représentation des données

### i) Lisibilité des données de test

#### **Utilisation des Tables**
```gherkin
When "Alice" lance et marque les points suivants:
  | Fléchette | Zone | Multiplicateur | Points |
  | 1         | 20   | Single         | 20     |
  | 2         | 19   | Double         | 38     |
  | 3         | 18   | Triple         | 54     |
```

**Justification :** Les tables offrent une lisibilité maximale pour les séquences complexes de lancers. Chaque colonne a une sémantique claire, facilitant la compréhension par les non-développeurs.

#### **Utilisation des Examples**
```gherkin
Examples:
  | zone | multiplicateur | points | score_final |
  | 1    | Single         | 1      | 500         |
  | 20   | Triple         | 60     | 441         |
```

**Justification :** Les Examples permettent de factoriser les tests de calcul de points, évitant la duplication tout en maintenant la lisibilité. Particulièrement efficace pour tester les variations de multiplicateurs.

#### **Background approprié**
```gherkin
Background:
  Given une nouvelle partie de fléchettes est créée
  And le mode de jeu est "501"
  And la partie nécessite un finish "double out"
```

**Justification :** Le Background évite la répétition de l'initialisation commune tout en gardant les scénarios focalisés sur leur objectif spécifique.

### ii) Extensibilité

#### **Architecture modulaire**

**Séparation des responsabilités :**
- `MorpionStepDefinitions.cs` : Logique spécifique au morpion
- `FlechettesStepDefinitions.cs` : Logique spécifique aux fléchettes
- Classes métier séparées (`Morpion.cs`, `DartsGame.cs`)

**Justification :** Cette séparation facilite l'ajout de nouveaux jeux sans impact sur l'existant. Chaque jeu a ses propres step definitions et classes métier.

#### **Extensibilité pour nouveaux jeux :**
1. Créer un nouveau projet de test (ex: `echecs`)
2. Implémenter les step definitions spécifiques
3. Référencer une classe métier dédiée
4. Ajouter au solution globale

#### **Modification des règles existantes :**
- Les règles métier sont centralisées dans les classes du domaine
- Les step definitions agissent comme adaptateurs
- Modification d'une règle = changement dans une seule classe

---

## c) Stratégie BDD et bonnes pratiques

### i) Langage ubiquitaire

#### **Morpion - Vocabulaire métier**
- "une nouvelle partie de morpion" (initialisation)
- "le joueur X joue en 0,0" (action métier)
- "la case 0,0 contient X" (état résultant)
- "le joueur X a gagné" (condition de victoire)

#### **Fléchettes - Vocabulaire métier**
- "une nouvelle partie de fléchettes est créée"
- "le mode de jeu est '501'"
- "finish 'double out'"
- "Bust! Score remis à..."
- "lance et marque les points suivants"

**Justification :** Le vocabulaire utilisé correspond exactement à celui employé par les joueurs et experts métier. Les termes techniques (coordonnées, multiplicateurs) sont exprimés de manière naturelle.

### ii) Réutilisabilité

#### **Step definitions communes**
```csharp
[StepDefinition(@"le joueur (X|O) joue en (\d),(\d)")]
```
Utilise des expressions régulières pour gérer les variations de joueurs et positions.

#### **Step definitions spécifiques**
```csharp
[Given(@"le mode de jeu est ""(.*)""")]
[Given(@"la partie nécessite un finish ""(.*)""")]
```
Spécifiques aux fléchettes car ces concepts n'existent pas au morpion.

**Justification :**
- **Communes** : Actions génériques réutilisables (vérifications d'état, actions de base)
- **Spécifiques** : Concepts métier uniques à chaque jeu
- Cette approche évite la sur-ingénierie tout en maximisant la réutilisation

#### **Classe DTO pour mapping**
```csharp
public class DartThrowDto
{
    public int Zone { get; set; }
    public string Multiplicateur { get; set; }
}
```
Facilite la conversion des données Gherkin vers les objets métier.

### iii) Maintenance

#### **Organisation du code**

**Structure claire :**
```
Solution/
├── morpion/               # Tests morpion
│   ├── Features/
│   ├── Steps/
│   └── Models/
├── flechettes/            # Tests fléchettes  
│   ├── Features/
│   ├── Steps/
│   └── référence vers flechettesClass
└── flechettesClass/       # Logique métier fléchettes
```

#### **Facilitation de la maintenance**

1. **Isolation des changements** : Chaque jeu dans son propre projet
2. **Step definitions explicites** : Noms de méthodes clairs et auto-documentés
3. **Séparation logique métier/tests** : Classes métier indépendantes
4. **Utilisation de FluentAssertions** : Messages d'erreur explicites

#### **Évolution du code**

**Ajout de fonctionnalités :**
- Nouvelles step definitions sans impact sur l'existant
- Extension des classes métier selon les principes SOLID

**Modification de règles :**
- Changement centralisé dans les classes métier
- Tests existants valident que les modifications ne cassent pas l'existant

**Refactoring :**
- Step definitions agissent comme interface stable
- Logique métier peut être refactorisée sans changer les tests

#### **Documentation vivante**

Les fichiers .feature servent de documentation exécutable :
- Compréhensible par les non-développeurs
- Toujours à jour (tests échouent si désynchronisée)
- Peut générer de la documentation HTML via SpecFlow LivingDoc

---

## Conclusion

Cette architecture BDD respecte les principes fondamentaux :
- **Outside-In** : Les scénarios guident l'implémentation
- **Collaboration** : Langage ubiquitaire facilite la communication
- **Documentation vivante** : Tests = spécification exécutable
- **Feedback rapide** : Détection précoce des régressions

L'organisation modulaire permet une maintenance aisée et une extension naturelle vers de nouveaux jeux, tout en conservant la lisibilité et la compréhension métier au cœur du processus de développement.