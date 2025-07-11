# Projet Fléchettes - Documentation Technique BDD

## Vue d'ensemble
Jeu de fléchettes 501 implémenté avec SpecFlow (.NET) suivant les principes BDD.

**Architecture** : `flechettesClass` (logique métier) + `flechettesConsole` (UI) + `flechettes` (tests BDD)

---

## a) Analyse et justification des scénarios

### i) Identification des cas de test
- **Cas nominaux (80%)** : Démarrage partie, calculs score, alternance joueurs, victoire
- **Cas limites (15%)** : Score=0 avec double, bust négatif, score=1, limite 3 fléchettes
- **Cas d'erreur (5%)** : Zones invalides, multiplicateurs incorrects

### ii) Priorisation des scénarios
- **P0 (Critiques)** : Commencer partie, calculs score, victoire, règle "double out"
- **P1 (Secondaires)** : Statistiques, validation entrées, edge cases

**Justification** : Priorisation par impact métier pour développement incrémental.

---

## b) Architecture et représentation des données

### i) Lisibilité des données de test
```gherkin
# Tables SpecFlow pour structure claire
When "Alice" lance et marque les points suivants:
  | Fléchette | Zone | Multiplicateur | Points |
  | 1         | 20   | Single         | 20     |

# Background pour éviter duplication
Background:
  Given une nouvelle partie de fléchettes est créée
  And le mode de jeu est "501"

# Scenario Outline pour tests paramétrés
Examples:
  | zone | multiplicateur | points | score_final |
  | 1    | Single         | 1      | 500         |
```

### ii) Extensibilité
**Architecture modulaire** : DartsGame (orchestrateur) + Player (entité) + DartThrow (VO)

**Points d'extension** : Nouveaux modes (301, Cricket), règles fin (master out), statistiques avancées

---

## c) Stratégie BDD et bonnes pratiques

### i) Langage ubiquitaire
Vocabulaire français métier : "Fléchette", "Zone", "Multiplicateur", "Double out", "Bust"

### ii) Réutilisabilité
- **80% steps communes** : Actions de base (`[Given] une nouvelle partie`, `[Then] le score devrait être`)
- **20% steps spécifiques** : Règles complexes (`[Then] Bust! Score remis à`, `ne peut plus lancer`)
- **DTO mapping** : `DartThrowDto` pour conversion SpecFlow → objets métier

### iii) Maintenance
```
flechettes/
├── Features/Flechettes.feature      # Spécifications
├── Steps/FlechettesStepDefinitions.cs # Implémentation
└── Hooks/Hook.cs                    # Configuration
```

**Patterns** : Single Responsibility, DRY, noms explicites, gestion erreurs

---

## Métriques
- **15 scénarios** couvrant 100% règles métier
- **Architecture modulaire** avec séparation responsabilités
- **Extensibilité** : 5 points d'évolution identifiés

## Exécution
```bash
dotnet test flechettes/                    # Tests BDD
dotnet run --project flechettesConsole/    # Interface console
```

**Conclusion** : BDD couvre exhaustivement les règles métier avec maintenabilité et extensibilité optimales.