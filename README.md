# Projet C# Microservices avec Blazor

Ce projet vise à créer une application web basée sur une architecture microservices, utilisant Blazor pour le frontend, une API Gateway pour la gestion des requêtes et plusieurs microservices pour la gestion de la vente de livre. 

## Déploiement

Le projet est disponible en ligne à l'adresse suivante : [https://izzibooks-isima.me/](https://izzibooks-isima.me/).


## Architecture du Projet

Le projet est structuré en utilisant une architecture de microservices. Voici une vue d'ensemble de la structure du répertoire :

```
.
├── Micro_Services
│   ├── BookService
│   │   ├── appsettings.Development.json
│   │   ├── appsettings.json
│   │   ├── Book.db
│   │   ├── BookService.csproj
│   │   ├── BookService.http
│   │   ├── Controllers
│   │   │   └── BooksController.cs
│   │   ├── Data
│   │   │   └── BookServiceContext.cs
│   │   ├── Migrations
│   │   ├── Program.cs
│   │   └── Properties
│   │       └── launchSettings.json
│   ├── Entities
│   │   ├── Book.cs
│   │   ├── Entities.csproj
│   │   ├── StringDistance.cs
│   │   └── User.cs
│   ├── Front
│   │   ├── Components
│   │   │   ├── App.razor
│   │   │   ├── _Imports.razor
│   │   │   ├── Layout
│   │   │   │   └── MainLayout.razor
│   │   │   ├── Pages
│   │   │   │   ├── Admin.razor
│   │   │   │   ├── Admin.razor.cs
│   │   │   │   ├── Cart.razor
│   │   │   │   ├── Cart.razor.cs
│   │   │   │   ├── Checkout.razor
│   │   │   │   ├── Checkout.razor.cs
│   │   │   │   ├── Explore.razor
│   │   │   │   ├── Explore.razor.cs
│   │   │   │   ├── Index.razor
│   │   │   │   ├── Index.razor.cs
│   │   │   │   ├── Login.razor
│   │   │   │   ├── Login.razor.cs
│   │   │   │   ├── Profile.razor
│   │   │   │   ├── Profile.razor.cs
│   │   │   │   ├── Signup.razor
│   │   │   │   ├── Signup.razor.cs
│   │   │   │   ├── TermsOfServiceModal.razor
│   │   │   │   ├── TermsOfServiceModal.razor.cs
│   │   │   │   ├── ThankYou.razor
│   │   │   │   └── ThankYou.razor.cs
│   │   │   └── Routes.razor
│   │   ├── Front.csproj
│   │   ├── Program.cs
│   │   ├── Properties
│   │   │   └── launchSettings.json
│   │   ├── Services
│   │   │   ├── BookService.cs
│   │   │   ├── CartStateService.cs
│   │   │   ├── CheckoutService.cs
│   │   │   ├── CustomAuthenticationStateProvider.cs
│   │   │   └── LoginService.cs
│   │   └── wwwroot
│   │       ├── Images
│   │       └── js
│   │           ├── konamiCode.js
│   │           └── stripeintegration.js
│   ├── GatewayService
│   │   ├── appsettings.Development.json
│   │   ├── appsettings.json
│   │   ├── Controllers
│   │   │   ├── BookController.cs
│   │   │   └── UserController.cs
│   │   ├── GatewayService.csproj
│   │   ├── GatewayService.http
│   │   ├── .gitignore
│   │   ├── Program.cs
│   │   ├── Properties
│   │   │   └── launchSettings.json
│   │   └── Services
│   │       └── JwtTokenValidation.cs
│   ├── identifier.sqlite
│   ├── MicroService.sln
│   └── UserService
│       ├── appsettings.Development.json
│       ├── appsettings.json
│       ├── Controllers
│       │   └── UsersController.cs
│       ├── Data
│       │   └── UserServiceContext.cs
│       ├── .gitignore
│       ├── Migrations
│       ├── Program.cs
│       ├── UserService.csproj
│       ├── UserService.http
│       └── Utilisateur.db
└── README.md
```


## Instructions d'Installation


### Prérequis

- [.NET Core SDK 8](https://dotnet.microsoft.com/download)

### Étapes d'Installation

1. Clonez le repo git :

   ```bash
   git clone https://github.com/MalikSploit/Service_Web.git
   ```

2. Ouvrez le dossier du projet dans votre éditeur de code.

3. Exécutez les commandes suivantes pour restaurer les dépendances et lancer les microservices :

```bash
cd Micro_Services/GatewayService && dotnet run

cd Micro_Services/UserService && dotnet run

cd Micro_Services/BookService && dotnet run

cd Micro_Services/Front && dotnet run
```

4. Ouvrez votre navigateur et allez à l'adresse suivante : [http://localhost:8080](http://localhost:8080)


# Fonctionnalités

- [:white_check_mark:] Inscription
- 

# Fonctionnalités

-[:white_check_mark:] Inscription : La possibilité pour les utilisateurs de s'inscrire à l'application.
-[:white_check_mark:] Connexion : Le frontend offre une fonctionnalité de connexion permettant aux utilisateurs de se connecter à leurs comptes.
- [:white_check_mark:] Intégration Frontend et API Gateway : Le frontend interagit de manière transparente avec l'API Gateway pour toutes les opérations nécessaires.
- [:white_check_mark:] Token JWT : Une fois connecté, le frontend reçoit un token JWT qu'il utilise pour authentifier les requêtes envoyées à la Gateway.
- [:white_check_mark:] Microservice UserService : Un microservice dédié à la gestion des utilisateurs, permettant l'inscription, la connexion, et d'autres fonctionnalités associées.
- [:white_check_mark:] Microservice BookService : Un microservice 
- [:white_check_mark:] Microservice Entities : Un microservice dédié à la gestion des entités, permettant la gestion des livres, des utilisateurs, et d'autres fonctionnalités associées.

Bonus :
- [:white_check_mark:] Champ Rôle Utilisateur : Un champ de rôle est ajouté aux utilisateurs (basique, admin) avec une migration et une application en base de données.
- [:white_check_mark:] Page Additionnelle pour Admin : En fonction du rôle de l'utilisateur, le frontend affiche une page supplémentaire permettant aux administrateurs de consulter la liste de tous les utilisateurs inscrits.
- [:white_check_mark:] Gestion des Erreurs : Le frontend gère les erreurs de manière conviviale, affichant des messages appropriés en cas d'informations de connexion incorrectes ou d'erreurs renvoyées par la Gateway.
- [:white_check_mark:] Persistence des Données : Les données du microservice Todo sont persistées, que ce soit dans une base de données, un fichier, ou tout autre mécanisme de stockage.
- [:white_check_mark:] Bonne Qualité de Code : Le code est bien indenté, lisible, et suit les bonnes pratiques de développement.
- [:white_check_mark:] Konami Code : Une fonctionnalité bonus amusante avec l'intégration d'un Konami Code.


