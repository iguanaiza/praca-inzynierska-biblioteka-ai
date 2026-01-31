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
