# DemoArrimageFRW
Application de démonstration pour l'arrimage d'un système autorisé à l'API FRW.

# Qu'est ce que c'est?

L'application de démonstration permet d'offir <strong>une base pour les développeurs de système autorisés</strong> souhaitant s'arrimer à FRW.
<br/>
Dans un premier temps, nous vous invitons à poser des <em>breakpoints</em> dans le code, et de passer l'application en <strong>débug</strong> afin de comprendre comment les appels sont fait.
<br/>
<br/>
Nous avons ajouté des commentaires au maximum d'endroit afin que votre session de débug soit semblable à lire une documentation.
<br/>
<br/>
Ensuite, il vous suffit de copier coller le code nécessaire dans votre projet en l'adaptant selon vos besoin et le tour est joué!

# Comment lancer le projet?

## Sélection des projets de démarrage

Avant de lancer l'application, il est important de bien configurer ses projets de démarrage. La démo est constituée de 2 projets :

- DemoFRW.API
- DemoFRW.PR

<strong>DemoFRW.API</strong> doit <strong>toujours</strong> être lancé lors du lancement de la démo puisque c'est ce projet qui va s'occuper de faire les appels à FRW.
<br/>
C'est d'ailleurs ici que nous vous conseillons d'insérer vos <em>breakpoint</em> afin d'observer la structure nécessaire pour les appels à FRW.
<br/>
<br/>
<strong>DemoFRW.PR</strong> est optionnel. Sélectionner ce projet vous ouvrira une page HTML de démonstration sur laquelle vous pourrez tester les appels au cas par cas depuis une interface utilisateur utilisant les librairies VueFormulate et <strong>UTD</strong>, que vous nous recommandons pour vos applications ministérielles.
<br/><br/>
![image](https://github.com/FujuDev/DemoArrimageFRW/assets/31103239/7532a9f9-e6d5-48d0-8c61-ed4a924e48ec)

Dans la fenêtre qui s'ouvre suite aux instructions au dessus, choisissez "<strong>Démarrer</strong>" pour <strong>DemoFRW.API</strong> (puisque c'est ce que vous voulez debug) et ensuite vous pouvez choisir l'option que vous voulez pour <strong>DemoFRW.PR</strong>, selon ce que vous voulez tester.

![image](https://github.com/FujuDev/DemoArrimageFRW/assets/31103239/edd4f828-2aad-40bd-a13d-2a9e30b69878)

Cliquez ensuite sur "OK", puis démarrez l'application!

## Naviguer dans le code avec le débogueur

S'il y a une chose <strong>importante</strong> que vous devez savoir, c'est que par défaut, <strong>l'application de démonstration simule ses communications à FRW</strong>.
Le code pour communiquer avec FRW est donc présent, mais on ne passe pas par là! La raison est simple : il n'y a pas de clé d'API et de numéro publique de système autorisé de configuré!

Afin de passer par la communication avec FRW, voici les étapes à réaliser :

- Ouvrir le appsettings.json du projet DemoFRW.API
- Remplacer le champ "VotreCleApi" par votre clé d'API FRW (si vous n'en avez pas, vous ne pourrez pas utiliser cette fonctionnalité de l'application de démonstration)
- Remplacer le champ "VotreNoPublicSystemAutorise" par votre numéro publique de système autorisé lié à votre clé d'API
- Dans le constructeur de la classe ApiFRW, mettez le booléen demo à <strong>false</strong>

![image](https://github.com/FujuDev/DemoArrimageFRW/assets/31103239/5131178b-e398-43bf-a7a2-8b3b74612ca8)

![image](https://github.com/FujuDev/DemoArrimageFRW/assets/31103239/8085a3bf-bc08-4259-8dd9-a4660b44a4b0)


Et voilà! Vous passez désormais par les appels à FRW!

<strong>Veuillez noter que nous stockons ici la clé d'API et le numéro publique de système autorisé dans le fichier appsettings.json par soucis de simplicité du programme de démonstration, mais il est extrêmement déconseillé de les mettre ici dans une application étant déstinée à la production. Déplacez ces informations dans des endroits sécurisés, inaccessibles aux utilisateurs. Ces informations ne devrait pas être communiquées à n'importe qui.</strong>
