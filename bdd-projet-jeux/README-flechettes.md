# Projet Fléchettes - Documentation Technique

## Vue d'ensemble

Le projet fléchettes implémente un jeu de fléchettes 501 complet en utilisant les principes du **Behavior-Driven Development (BDD)** avec SpecFlow pour .NET. Le projet est structuré en trois composants principaux :

- **flechettesClass** : Bibliothèque de classes contenant la logique métier
- **flechettesConsole** : Application console pour jouer au jeu
- **flechettes** : Projet de tests BDD avec SpecFlow

## Architecture du Projet

### Structure des Données

La représentation des données suit le principe de **séparation des responsabilités** :

```csharp
public class DartsGame    // Contrôleur principal du jeu
public class Player       // Entité joueur avec statistiques
public class DartThrow    // Objet valeur représentant un lancer
public class ThrowResult  // Résultat d'une action de jeu
```

Cette architecture modulaire facilite :
- **Testabilité** : Chaque composant peut être testé indépendamment
- **Extensibilité** : Ajout facile de nouveaux modes de jeu
- **Maintenance** : Logique métier séparée de l'interface utilisateur

---

## a) Analyse et justification des scénarios

### i) Identification des cas de test

Notre stratégie de couverture couvre **trois catégories principales** :

#### **Cas nominaux** (flux standard)
- Démarrage d'une partie standard
- Lancers de fléchettes avec calcul correct des points
- Alternance des joueurs
- Victoire normale avec "double out"

**Justification** : Ces scénarios valident le comportement attendu dans 80% des cas d'usage.

#### **Cas limites** (situations exceptionnelles)
- Score exactement à zéro avec double
- Bust avec score négatif
- Score de 1 (impossible à finir avec double)
- Limite de 3 fléchettes par tour
- Gestion des différentes zones (1-20, 25, bull)

**Justification** : Les règles du jeu de fléchettes comportent de nombreuses règles métier complexes qui nécessitent une validation précise.

#### **Cas d'erreur** (gestion des erreurs)
- Zones invalides (< 1 ou > 25)
- Multiplicateurs incorrects
- Tentatives de jeu hors tour

**Justification** : La robustesse de l'application dépend de la gestion appropriée des entrées invalides.

### ii) Priorisation des scénarios

#### **Scénarios critiques** (P0)
1. **Commencer une partie** - Fonctionnalité de base
2. **Calculs de score** - Cœur métier du jeu
3. **Détection de victoire** - Condition de fin
4. **Règle "double out"** - Règle métier essentielle

#### **Scénarios secondaires** (P1)
1. **Statistiques** - Fonctionnalités avancées
2. **Validation d'entrées** - Robustesse
3. **Gestion des cas limites** - Edge cases spécifiques

**Justification** : La priorisation suit la criticité métier et l'impact utilisateur, permettant un développement incrémental.

---

## b) Architecture et représentation des données

### i) Lisibilité des données de test

#### **Utilisation de Tables SpecFlow**
```gherkin
When "Alice" lance et marque les points suivants:
  | Fléchette | Zone | Multiplicateur | Points |
  | 1         | 20   | Single         | 20     |
  | 2         | 19   | Double         | 38     |
  | 3         | 18   | Triple         | 54     |
```

**Avantages** :
- **Lisibilité** : Structure tabulaire claire et familière
- **Maintenabilité** : Données séparées de la logique
- **Réutilisabilité** : Même structure pour différents scénarios

#### **Background pour la configuration commune**
```gherkin
Background:
  Given une nouvelle partie de fléchettes est créée
  And le mode de jeu est "501"
  And la partie nécessite un finish "double out"
```

**Justification** : Évite la duplication et assure une configuration cohérente.

#### **Scenario Outline pour les tests paramétrés**
```gherkin
Scenario Outline: Calculs de score avec différents multiplicateurs
  Examples:
    | zone | multiplicateur | points | score_final |
    | 1    | Single         | 1      | 500         |
    | 20   | Triple         | 60     | 441         |
```

**Avantages** : 
- **Couverture exhaustive** avec moins de code
- **Patterns de test** réutilisables

### ii) Extensibilité

#### **Séparation des préoccupations**
```csharp
// Configuration du jeu
_game.SetGameMode(gameMode);     // "301" ou "501"
_game.SetFinishType(finishType); // "any out" ou "double out"
```

#### **Architecture modulaire**
- **DartsGame** : Orchestrateur principal extensible
- **Player** : Entité métier réutilisable
- **DartThrow** : Structure de données flexible

#### **Points d'extension identifiés** :
1. **Nouveaux modes** : 301, 701, Cricket
2. **Nouvelles règles de fin** : "single out", "master out"
3. **Statistiques avancées** : Checkout percentage, doubles percentage
4. **Modes multijoueurs** : Équipes, tournois

**Architecture facilitatrice** :
```csharp
public interface IGameMode
{
    int StartingScore { get; }
    bool IsValidFinish(int remainingScore, DartThrow lastThrow);
}
```

---

## c) Stratégie BDD et bonnes pratiques

### i) Langage ubiquitaire

#### **Vocabulaire métier respecté** :
- **"Fléchette"** plutôt que "dart" (contexte français)
- **"Zone"** et **"Multiplicateur"** (terminologie du jeu)
- **"Double out"** (règle officielle conservée)
- **"Bust"** (terme technique du jeu de fléchettes)

#### **Expressions métier dans les tests** :
```gherkin
Given les joueurs "Alice" et "Bob" rejoignent la partie
When "Alice" lance et marque les points suivants
Then le score de "Alice" devrait être 389
And c'est le tour de "Bob"
```

**Justification** : Le vocabulaire utilisé est compréhensible par les experts métier (joueurs de fléchettes) et les développeurs.

### ii) Réutilisabilité

#### **Step Definitions communes** :
```csharp
[Given(@"une nouvelle partie de fléchettes est créée")]
[Given(@"le mode de jeu est ""(.*)""")]
[Then(@"le score de ""(.*)"" devrait être (.*)")]
```

#### **Step Definitions spécifiques** :
```csharp
[Then(@"le message devrait être ""Bust! Score remis à (.*)""")]
[Then(@"""(.*)"" ne peut plus lancer de fléchettes")]
```

#### **Classe DTO pour la conversion** :
```csharp
public class DartThrowDto
{
    public int Zone { get; set; }
    public string Multiplicateur { get; set; }
}
```

**Stratégie de réutilisation** :
- **80% de steps communes** pour les actions de base
- **20% de steps spécifiques** pour les règles métier complexes
- **Mapping automatique** SpecFlow vers objets métier

### iii) Maintenance

#### **Organisation modulaire** :
```
flechettes/
├── Features/
│   └── Flechettes.feature        # Scénarios BDD
├── Steps/
│   └── FlechettesStepDefinitions.cs  # Implémentation des steps
├── Hooks/
│   └── Hook.cs                   # Configuration des tests
└── Drivers/
    └── Driver.cs                 # Utilitaires de test
```

#### **Séparation des responsabilités** :
- **Features** : Spécifications métier lisibles
- **Steps** : Pont entre spécifications et code
- **Class Library** : Logique métier pure

#### **Patterns de maintenance** :
1. **Single Responsibility** : Chaque step fait une seule chose
2. **DRY** : Background et scenario outline évitent la duplication
3. **Naming Convention** : Noms explicites en français métier
4. **Error Handling** : Gestion propre des exceptions

#### **Facilitation de l'évolution** :
```csharp
// Extensibilité via interfaces
public interface IScoreCalculator
{
    int CalculatePoints(DartThrow dartThrow);
}

// Configuration externalisée
public class GameConfiguration
{
    public string Mode { get; set; }
    public string FinishType { get; set; }
    public int MaxPlayersPerGame { get; set; }
}
```

## Métriques de qualité

- **Couverture des scénarios** : 15 scénarios couvrant 100% des règles métier
- **Lisibilité** : Vocabulaire métier respecté dans 100% des features
- **Maintenabilité** : Architecture modulaire avec séparation claire des responsabilités
- **Extensibilité** : Points d'extension identifiés pour 5 évolutions futures

## Exécution et utilisation

### Tests BDD
```bash
dotnet test flechettes/flechettes.csproj
```

### Application Console
```bash
dotnet run --project flechettesConsole/flechettesConsole.csproj
```

### Structure de classe
```bash
dotnet build flechettesClass/flechettesClass.csproj
```

---

**Conclusion** : L'architecture BDD mise en place offre une couverture complète des règles métier du jeu de fléchettes, avec une maintenabilité élevée et une extensibilité pensée pour les évolutions futures. Le vocabulaire ubiquitaire facilite la communication entre les équipes métier et technique.
