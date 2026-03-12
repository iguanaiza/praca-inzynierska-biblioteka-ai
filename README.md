Azure live: https://biblioteka-www-prod-ajbch6bpdte8ejem.polandcentral-01.azurewebsites.net/


EN
# Project Objectives and Assumptions
The goal of the project is to create a web-based system for managing a school library and browsing its resources online, designed for seamless future expansion. The system consists of two subsystems – a library catalog and a management panel, each with a dedicated user interface. Access to a given system and its functionalities depends on the user's role; the catalog itself is accessible without logging in, but with restricted functionalities (e.g., book reservation).

The application is adapted for screen readers and offers accessibility options compliant with WCAG 2.1, along with features facilitating site navigation, such as: 
1. Font size adjustment: small (12px), medium (16px; default), large (20px);
2. Theme selection: green (default), blue, brown, dark gray;
3. High contrast mode option;
4. Sitemap;
5. Catalog usage instructions.

The reader will be able to browse and search for books, add them to favorites, add them to the cart, and borrow them (if available). The catalog also offers a view of recommended items and separate subpages for mandatory reading lists and textbooks. The reader will also have the opportunity to get help from a chatbot supported by AI.

The management panel for the librarian allows viewing, adding, modifying, and deleting resources (books, textbooks, mandatory reading, copies) as well as user accounts. Access to the panel is granted to users with the librarian or administrator role, and switching between the panel and the catalog is done via a dedicated button in the navigation bar. 

## Project scope includes:
- Creation of a database and connecting it to the application via an API
- Creation of separate frontend and backend layers and merging them into a single application
- Creation of a user authorization and authentication system
- Creation of a catalog page where the user can browse and reserve books, save them to favorites, and view their borrowing history
- Creation of a management panel page where an authorized user can manage resources and users (view, add, modify, delete)
- Implementation of accessibility options compliant with WCAG 2.1 guidelines
- Implementation of a reader assistant in the form of an AI chat
- Implementation of a responsive, user-friendly, and readable user interface
- Application of modern design patterns
 
# Technologies Used and System Architecture
The foundation of the project is the .NET 9 platform. The system architecture was designed as a multi-tier and distributed solution, ensuring scalability and ease of maintenance. The structure of the solution is based on a strict division into independent logical layers (Fig. 1):
1. Presentation Layer (Frontend): Client application built using Blazor Web App technology.
2. API Layer (Backend): RESTful API responsible for handling requests and communicating with the database.
3. Business Logic Layer (Services): Module processing business rules and validation.
4. Data Access Layer (Repositories): Direct operations on the database.

To ensure consistency of data contracts between the API and the Frontend, a separate Shared project was extracted. It contains definitions of Data Transfer Objects (DTOs), enumerations (enums), and validation attributes. Thanks to this solution, any changes in the data structure are immediately detected at compile time in both layers of the system, eliminating integration errors.

The core of the system is an asynchronous API based on the ASP.NET Core 9 framework. The Services layer centralizes business logic, Dependency Injection (DI) ensures loose coupling of components, and AutoMapper standardizes the transformation of domain models to DTOs.

The foundation of data storage is the Microsoft SQL Server engine. The database schema was modeled using the Redgate Data Modeler tool, which ensured the correctness of relations at the planning stage. Data access is implemented by Entity Framework Core 9. Global Query Filters were also implemented to manage record availability, along with Split Queries to optimize the performance of queries retrieving related data.

The presentation layer was built using the Blazor Web App (.NET 9) model, utilizing a modern hybrid rendering model: Server-Side Rendering (SSR) and Client-Side (WebAssembly). The Blazor Server application acts as a Proxy, forwarding requests from the browser to the actual Backend API, which increases the security of the architecture.
 
The user interface (UI) was designed using mockups in the Figma tool. The Bootstrap library was used to implement a responsive web design (RWD), along with the Syncfusion Blazor Components suite. The Syncfusion package was used to build advanced data grids with server-side sorting, filtering, and pagination, as well as to handle forms, charts, and modal windows.

A key component of the project is the intelligent assistant module, built on the Azure AI Foundry platform. This solution implements an advanced RAG (Retrieval-Augmented Generation) architecture in a hybrid model, integrating two diverse data sources to provide precise answers:
- Dynamic data: The assistant communicates with the system's API to retrieve real-time information about the catalog's state (e.g., copy availability).
- Static data: The Azure AI Search service was used to index a static knowledge base (a DOCX file) containing institutional information, procedures, and instructions.

The analytical phase of the project included system modeling in the Visual Paradigm tool, where use case diagrams and sequence diagrams were developed.

## Project documentation
**Partial project documentation** in polish is available in the file [Biblioteka_docs_PL.pdf](https://github.com/iguanaiza/praca-inzynierska-biblioteka-ai/blob/master/Biblioteka_docs_PL.pdf). 
Full version can be sent on request for job recruitment process.

PL
# Cele i założenia projektu
Celem projektu jest utworzenie systemu webowego do obsługi szkolnej biblioteki i możliwości przeglądania zasobów online, umożliwiającego bezproblemowe rozbudowanie go w przyszłości. System składa się z dwóch podsystemów – katalogu biblioteki oraz panelu zarządza-nia, każdy z dedykowanym interfejsem użytkownika. Dostęp do danego systemu i jego funkcjonal-ności zależy od roli użytkownika, sam katalog jest dostępny bez logowania, ale ograniczając pewne funkcjonalności (np. rezerwacja książki).
Aplikacja jest dostosowana pod czytniki ekranowe oraz oferuje opcje dostępności zgodne z WCAG 2.1  i opcje ułatwiające poruszanie się po stronie takie, jak: 
1.	Zmiana wielkości czcionki: mała (12px), średnia (16px; domyślnie), duża (20px);
2.	Zmiana motywu strony: zielony (domyślnie), niebieski, brązowy, ciemnoszary;
3.	Wybór trybu wysokiego kontrastu strony;
4.	Mapa strony;
5.	Instrukcja użytkowania katalogu;

Czytelnik będzie mógł przeglądać i wyszukiwać książki, dodawać je do ulubionych, doda-wać je do koszyka oraz je wypożyczać (jeśli są dostępne). Katalog oferuje także przegląd poleca-nych przez bibliotekę pozycji oraz osobne podstrony dla listy lektur i podręczników. Czytelnik zy-ska również możliwość uzyskania pomocy od chatbota, korzystającego ze wsparcia AI.
Panel zarządzania dla bibliotekarza umożliwia przegląd, dodawanie, modyfikowanie i usuwanie zasobów (książek, podręczników, lektury, egzemplarzy) oraz kont czytelników. Dostęp do panelu jest przyznawany użytkownikom z rolą bibliotekarza lub administratora, a przełączanie między panelem a katalogiem odbywa się przy pomocy specjalnego przycisku w pasku nawigacji. 

## Zakresem projektu jest:
-	Utworzenie bazy danych i połączenie jej z aplikacją za pomocą API
-	Utworzenie osobnej warstwy frontend i backend oraz scalenie ich w jedną aplikację
-	Utworzenie systemu autoryzacji i logowania użytkowników
-	Utworzenie strony katalogu, na której użytkownik może przeglądać i rezerwować książki oraz zapisywać je do ulubionych, a także przeglądać historię swoich wypożyczeń
- Utworzenie strony panelu zarządzania, na której autoryzowany użytkownik może za-rządzać zasobami oraz użytkownikami (przegląd, dodawanie, modyfikacja, usuwanie)
-	Implementacja opcji dostępności zgodnych z wytycznymi WCAG 2.1
-	Implementacja asystenta czytelnika w postaci czatu AI
-	Implementacja responsywnego, przyjaznego i czytelnego interfejsu użytkownika
-	Zastosowanie nowoczesnych wzorców projektowych
 
# Wykorzystane technologie i architektura systemu
Fundamentem projektu jest platforma .NET 9. Architektura systemu została zaprojektowana jako rozwiązanie wielowarstwowe i rozproszone, co zapewnia skalowalność oraz łatwość utrzyma-nia. Struktura rozwiązania opiera się na ścisłym podziale na niezależne warstwy logiczne (Zdj. 1):
1.	Warstwa Prezentacji (Frontend): Aplikacja kliencka zrealizowana w technologii Blazor Web App.
2.	Warstwa API (Backend): RESTful API odpowiedzialne za obsługę żądań i komunikację z bazą danych.
3.	Warstwa Logiki Biznesowej (Services): Moduł przetwarzający reguły biznesowe i walidację.
4.	Warstwa Dostępu do Danych (Repositories): Bezpośrednie operacje na bazie danych.

W celu zapewnienia spójności kontraktów danych pomiędzy API a Frontendem, wydzielono projekt Shared. Zawiera on definicje obiektów transferu danych (DTO ), typy wyliczeniowe (enum) oraz atrybuty walidacji. Dzięki temu rozwiązaniu, wszelkie zmiany w strukturze danych są natychmiast wykrywane w czasie kompilacji w obu warstwach systemu, co eliminuje błędy integra-cyjne.
Rdzeń systemu stanowi asynchroniczne API oparte na frameworku ASP.NET Core 9. War-stwa Serwisów centralizuje logikę biznesową, Dependency Injection (DI) zapewnia luźne powiąza-nia komponentów, a AutoMapper standaryzuje transformację modeli domenowych na DTO.
Fundamentem składowania danych jest silnik Microsoft SQL Server. Schemat bazy wymodelowano w narzędziu Redgate Data Modeler, co zapewniło poprawność relacji na etapie planowania. Dostęp do danych realizuje Entity Framework Core 9. Zaimplementowano również Global Query Filters do zarządzania dostępnością rekordów oraz Split Queries w celu optymalizacji wydajności zapytań pobierających powiązane dane.
Warstwa prezentacji została zrealizowana w modelu Blazor Web App (.NET 9), wykorzystującym nowoczesny model renderowania hybrydowego Server-Side Rendering (SSR) i Client-Side (WebAssembly). Aplikacja Blazor Server pełni rolę pośrednika (Proxy), przekazując żądania z przeglądarki do właściwego API Backendowego, co zwiększa bezpieczeństwo architektury.
 
Interfejs użytkownika (UI) zaprojektowano z wykorzystaniem makiet w narzędziu Figma. Do implementacji interfejsu użyto biblioteki Bootstrap dla stworzenia responsywnego układu (RWD), oraz pakietu Syncfusion Blazor Components. Pakiet Syncfusion posłużył do budowy zaawansowanych tabel z obsługą sortowania, filtrowania i paginacji po stronie serwera, a także do obsługi formularzy, wykresów oraz okien modalnych.
Kluczowym komponentem projektu jest moduł inteligentnego asystenta, zbudowany w oparciu o platformę Azure AI Foundry. Rozwiązanie to implementuje zaawansowaną architekturę RAG (Retrieval-Augmented Generation ) w modelu hybrydowym, integrując dwa zróżnicowane źródła danych w celu dostarczenia precyzyjnych odpowiedzi:
-	Dane dynamiczne: Asystent komunikuje się z API systemu w celu pobierania bieżących informacji o stanie katalogu (np. dostępność egzemplarzy).
-	Dane statyczne: Wykorzystano usługę Azure AI Search do indeksowania statycznej bazy wiedzy (plik DOCX), zawierającej informacje instytucjonalne, procedury i instrukcje.

Faza analityczna projektu obejmowała modelowanie systemu w narzędziu Visual Paradigm, w ramach którego opracowano diagramy przypadków użycia i diagramy sekwencji.

## Dokumentacja projektu
**Fragment dokumentacji** jest dostępny w pliku [Biblioteka_docs_PL.pdf](https://github.com/iguanaiza/praca-inzynierska-biblioteka-ai/blob/master/Biblioteka_docs_PL.pdf). 
Pełna wersja może zostać udostępniona podczas rekrutacji.
