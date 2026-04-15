# Plan de migration : Mise à niveau vers .NET 10.0 (All-At-Once)

## Table des matières

- [Résumé exécutif](#résumé-exécutif)
- [Stratégie de migration](#stratégie-de-migration)
- [Analyse des dépendances détaillée](#analyse-des-dépendances-détaillée)
- [Plans par projet](#plans-par-projet)
  - [DownloadComics\DownloadComics.csproj](#downloadcomicsdownloadcomicscsproj)
  - [SearchComics\SearchComics.csproj](#searchcomicssearchcomicscsproj)
- [Référence de mise à jour des packages NuGet](#référence-de-mise-à-jour-des-packages-nuget)
- [Catalogue des ruptures attendues (Breaking Changes)](#catalogue-des-ruptures-attendues-breaking-changes)
- [Stratégie de tests et validation](#stratégie-de-tests-et-validation)
- [Gestion des risques et atténuation](#gestion-des-risques-et-atténuation)
- [Stratégie de contrôle de source](#stratégie-de-contrôle-de-source)
- [Critères de réussite](#critères-de-réussite)

---

## Résumé exécutif

Objectif : Mettre à niveau la solution `Comics.slnx` pour cibler `.NET 10.0 (net10.0)` en appliquant une stratégie All-At-Once (mise à jour atomique de tous les projets simultanément).

High-level metrics (extrait de l'évaluation) :
- Projets impactés : 3 (analyse détaillée fournie; principaux projets listés ci-dessous)
- Projets analysés en détail dans ce plan : `DownloadComics` (WPF, SDK-style) et `SearchComics` (Classic .NET Framework)
- Total fichiers source : 23 ; Lignes de code totales : ~2365
- Packages NuGet recensés : 5 (compatibles selon l'analyse)
- Problèmes majeurs détectés : conversion de `SearchComics` vers SDK-style + mise à jour des frameworks cibles ; `DownloadComics` contient incompatibilités API importantes liées à WPF

Risque global : Moyennement élevé dû à la présence d'un projet WPF avec de nombreuses incompatibilités API (binary incompatible) et à la nécessité de convertir un projet classique (non SDK) en SDK-style avant mise à jour.

Recommandation de stratégie : All-At-Once (atomic upgrade) — justifiée par le nombre réduit de projets (<5) et la dépendance directe entre `DownloadComics` et `SearchComics` (mise à niveau simultanée facilite la résolution des incompatibilités).

---

## Stratégie de migration

Sélection : All-At-Once Strategy — toute la solution est mise à niveau en une opération atomique.

Raisons :
- Petite solution (≤5 projets) ; facilité pour appliquer changements cohérents aux packages et cibles
- `SearchComics` doit être converti en SDK-style — effectuer cette conversion en même temps que la mise à jour d'ensemble réduit les états intermédiaires instables
- Tests/validation seront exécutés après l'opération atomique

Principes d'exécution (pour l'exécuteur) — NOTER : plan uniquement, pas d'exécution ici
- TASK-000 (Prérequis) : SDK .NET 10 installé et sauvegarde/contrôle de source prêt (aucun dépôt Git détecté dans l'analyse)
- TASK-001 (Atomic upgrade) :
  - Convertir tous les projets non-SDK en SDK-style (`SearchComics`) et mettre à jour `TargetFramework` vers `net10.0` (ou `net10.0-windows` pour projets WPF)
  - Mettre à jour les références de package indiquées dans la section "Référence de mise à jour des packages NuGet"
  - Restaurer et effectuer une construction de la solution
  - Corriger erreurs de compilation causées par l'upgrade (API/behavior changes)
  - Rebuild et verification (solution build avec 0 erreurs)
- TASK-002 (Validation) : exécuter tests unitaires / validations fonctionnelles automatisées

Remarque sur la séquence : bien que l'approche soit atomique, respecter l'ordre logique (convertir projet non-SDK avant définir TargetFramework) dans la même opération.

---

## Analyse des dépendances détaillée

Synthèse :
- `DownloadComics` (SDK-style, WPF) dépend de `SearchComics` (Classic net48)
- `SearchComics` doit être converti en SDK-style et mis à jour vers `net10.0` afin que `DownloadComics` puisse consommer la version cible sans incompatibilités binaires

Graphe des dépendances (texte) :
- DownloadComics -> SearchComics

Ordre conceptuel à respecter (même si opération atomique) :
1. Convertir `SearchComics` en SDK-style et définir TargetFramework = `net10.0` (bibliothèque/outil)
2. Mettre à jour `DownloadComics` TargetFramework = `net10.0-windows` et activer support Windows Desktop si nécessaire (`<UseWindowsDesktop>true</UseWindowsDesktop>`)
3. Mettre à jour packages et restaurer

Points d'attention :
- Fichiers MSBuild partagés (ex. Directory.Build.props) s'il en existe — s'assurer qu'ils sont pris en compte dans la mise à jour atomique
- Conditions MSBuild ou logique conditionnelle basée sur TargetFramework : vérifier et adapter

---

## Plans par projet

### DownloadComics\DownloadComics.csproj

Contexte : WPF, SDK-style, actuel `net8.0-windows7.0`, proposition cible `net10.0-windows`.

Etat courant : SDK-style = True
Proposé : TargetFramework → `net10.0-windows` (ou `net10.0-windows` + `<UseWindowsDesktop>true</UseWindowsDesktop>`) et vérifier compatibilité d'API WPF

Étapes de migration (décrites pour l'exécutant — plan only) :
1. Prérequis : s'assurer que le SDK .NET 10 est installé sur la machine de travail.
2. Mettre à jour TargetFramework dans le projet à `net10.0-windows`.
3. Vérifier la propriété `<UseWindowsDesktop>` (ajouter `<UseWindowsDesktop>true</UseWindowsDesktop>` si absente).
4. Mettre à jour les package references si nécessaire (voir section Packages)
5. Restaurer et builder la solution
6. Corriger les erreurs de compilation liées aux API WPF (voir catalogue des ruptures)
7. Rebuild et vérifier absence d'erreurs

Risques spécifiques :
- Beaucoup de ruptures binary-incompatible liées à contrôles WPF et comportements (liste non exhaustive dans le catalogue des ruptures)
- Peut nécessiter changements XAML ou adaptation d'appel à API

Complexité : High (binary incompatible détectées : ~872 occurrences sur l'ensemble, et 569 issues liées au WPF selon l'analyse)

Validation projet :
- Build réussi
- Tests unitaires/automatisés (si présents) passent
- Scénarios UI critiques testés manuellement par l'équipe (non inclus dans tâches automatisées sauf si tests existent)

---

### SearchComics\SearchComics.csproj

Contexte : Projet Classic .NET Framework (net48). Proposé : conversion en SDK-style et montée vers `net10.0`.

Etat courant : net48, SDK-style = False
Proposé : TargetFramework → `net10.0` ; Convertir fichier projet vers SDK-style

Étapes de migration :
1. Convertir le fichier `.csproj` en SDK-style (remplacer l'ancien format par le format SDK, conserver les références de code et ressources)
2. Définir `<TargetFramework>net10.0</TargetFramework>` dans le nouveau projet SDK
3. Conserver ou migrer la logique des références `PackageReference` — convertir `packages.config` si présent
4. Mettre à jour les packages NuGet (ex : `FuzzySharp` vérifié comme compatible)
5. Restaurer et builder la solution
6. Corriger les erreurs de compilation (cas d'APIs .NET Framework non disponibles ou obsolètes)

Risques spécifiques :
- Si le code utilise APIs non disponibles en .NET 10 ou dépendances propriétaires non portées, des remplacements seront nécessaires. L'analyse n'a pas signalé de problèmes API pour ce projet (0 binary incompatible détecté)

Complexité : Low → Medium (conversion de format + vérification)

Validation projet :
- Le projet se compile en `net10.0`
- Les comportements CLI/console reproduisent l'existant

---

## Référence de mise à jour des packages NuGet

(Extrait de l'évaluation — tous les packages listés comme compatibles)

- `FuzzySharp` — version actuelle : `2.0.2` — projet : `SearchComics`
- `FuzzierSharp` — version actuelle : `3.0.1` — projet : `DownloadComics`
- `HtmlAgilityPack` — version actuelle : `1.12.4` — projet : `DownloadComics`
- `Microsoft.Web.WebView2` — version actuelle : `1.0.3650.58` — projet : `DownloadComics`
- `Newtonsoft.Json` — version actuelle : `13.0.4` — projet : `DownloadComics`

Règle de plan : inclure toutes les mises à jour recommandées par l'évaluation — si une version cible est indiquée dans l'évaluation, l'appliquer. Ici l'analyse indique compatibilité ; l'exécuteur doit :
- vérifier si des versions plus récentes et compatibles existent spécifiquement pour .NET 10, et appliquer les versions testées/validées
- adresser immédiatement tout paquet listé comme vulnérable (aucune vulnérabilité signalée dans l'analyse actuelle)

---

## Catalogue des ruptures attendues (Breaking Changes)

Synthèse des catégories principales (d'après l'analyse) :

1. WPF API changes / Binary Incompatible (haute priorité)
   - Contrôles UI et événements (TextBox, Button, ListView, ComboBox, etc.) peuvent nécessiter adaptations
   - Problèmes avec `RoutedEventHandler`, `RoutedEventArgs`, `Dispatcher` et méthodes associées
   - Evènements et propriétés (ex : `IsEnabled`, `ItemsSource`, `SelectedItem`) peuvent se comporter différemment ou exposer signatures modifiées

2. Comportements modifiés (Behavioral changes)
   - `System.Uri` et constructeurs associés peuvent afficher des comportements légèrement modifiés — tester parsing/normalisation d'URI

3. Configuration et app.config
   - Si des projets utilisent le système de configuration legacy, envisager `System.Configuration.ConfigurationManager` comme pont ou migrer vers `Microsoft.Extensions.Configuration`

Pour chaque rupture détectée lors du build, documenter la cause et la correction dans le journal d'upgrade (ex : remplacer appel X par Y, adapter XAML, ajouter using, etc.).

---

## Stratégie de tests et validation

Niveaux de test :
- Validation de compilation (obligatoire après TASK-001)
- Tests unitaires automatisés (si présents) — exécuter après build
- Tests d'intégration / end-to-end (si disponibles)
- Vérifications spécifiques WPF (scénarios UI critiques) — si tests UI automatisés présents, exécuter ; sinon inspection manuelle requise par l'équipe

Checklist de validation pour la mise à niveau atomique :
- [ ] Solution se compile sans erreurs
- [ ] Aucune dépendance manquante
- [ ] Tous les tests unitaires passent
- [ ] Scénarios critiques WPF validés
- [ ] Aucun avertissement de sécurité restant pour packages NuGet

---

## Gestion des risques et atténuation

Risques majeurs :
- Ruptures WPF massives provoquant besoin de modifications manuelles étendues
- Conversion de `SearchComics` mal configurée entraînant échecs de build
- Absence de dépôt Git / historique : perte de traçabilité

Atténuations recommandées :
- Effectuer une copie de sauvegarde de la branche/source actuelle avant l'upgrade (sauvegarde hors contrôle de source si aucun dépôt)
- Créer un commit unique/merge request qui contient l'upgrade atomique (si contrôle source disponible)
- Préparer un plan de retour arrière : restauration de la sauvegarde en cas d'échec critique
- Prioriser la résolution des erreurs de compilation listées après build initial et documenter chaque correction

---

## Stratégie de contrôle de source

Etat détecté : Aucun dépôt Git trouvé par l'analyse.

Recommandations :
- Si vous utilisez un contrôle de source local (Git ou autre), initialiser un dépôt avant l'opération et créer une branche dédiée `upgrade/dotnet-10.0` pour contenir le changement atomique.
- Commit unique recommandé pour l'upgrade atomique (TASK-001), suivi de commits de correction si nécessaire mais préférer regrouper dans la même branche pour revue.
- Si aucun contrôle de source n'est disponible, créer une archive complète du dépôt actuel avant de procéder.

---

## Critères de réussite

La migration sera considérée comme réussie lorsque :
1. Tous les projets listés ciblent leur framework proposé (`net10.0` ou `net10.0-windows` pour WPF)
2. Toutes les mises à jour de packages listées dans l'évaluation sont appliquées ou correctement justifiées
3. La solution construit sans erreurs
4. Tous les tests automatisés existants passent
5. Aucune vulnérabilité NuGet critique n'est détectée

---

## Annexes / Actions recommandées avant exécution

- Valider l'installation du SDK .NET 10 sur l'environnement d'exécution
- Créer sauvegarde complète ou initialiser dépôt Git et créer branche `upgrade/dotnet-10.0`
- Documenter les attentes pour les modifications manuelles sur WPF

---

_Fin du plan — prêt pour revue. Si vous souhaitez que j'ajuste la granularité (par ex. ajouter listes de modifications de code précises basées sur les erreurs de compilation attendues), demandez une itération de détail additionnelle._
