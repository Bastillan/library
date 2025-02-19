# Narzędzia typu RAD
## Zadanie 2 - Biblioteka w ASP.MVC
- [X] Proszę napisać prosty system obsługi biblioteki
- [X] Istnieje jeden specjalny (istniejący) użytkownik 'librarian'
- [X] Zwykły użytkownik musi się zalogować do swojego konta, jak nie ma konta to może takie stworzyć w procesie rejestracji konta (bez potwierdzania mailowego)
- [X] Zalogowany użytkownik który nie ma wypożyczonych książek może skasować swoje konto (librarian nie może skasować konta)
- [X] Użytkownik może wyszukiwać książki.
- [X] Jeżeli książka nie jest wypożyczona lub zarezerwowana użytkownik może ją zarezerwować.
- [X] Rezerwacja jest ważna do końca dnia następnego
- [X] Użytkownik może wyświetlić swoje rezerwacje i niektóre (wskazane) kasować.
- [X] Lista książek w bibliotece jest zarządzana przez użytkownika 'librarian' Jeżeli książka była choć raz wypożyczona to jej skasowanie polega na zaznaczeniu że jest trwale niedostepna (i niewidoczna przy wyszukiwaniu przez zwykłych użytkowników).
- [X] Użytkownik librarian może wyświetlić listę wszystkich rezerwacji i wyszukaną rezerwację zmienić na wypożyczenie
- [X] Użytkownik librarian może wyświetlić listę wszystkich wypożyczeń i w wyszukanym wypożyczeniu zmienić informację, że książka jest dostępna
- [X] Dodanie poprawnej obsługi równoległości. Dodanie pól odpowiedzialnych za wykrywanie konfliktów.
- [X] Udokumentowanie procesu migracji bazy danych

## Dodatkowe funkcjonalności
- Użytkownik może wyszukiwać książki (a także swoje rezerwacje i wypożyczenia), wpisując tytuł, autora lub wybierając gatunek książki
- Bibliotekarz analogicznie do użytkownika może wyszukiwać książki, dodatkowo może filtrować rezerwacje i wyporzyczenia po nazwie użytkownika
- Bibliotekarz może dodawać nowe książki do biblioteki lub edytować istniejące egzemplarze
- Użytkownik i bibliotekarz mogą wyświetlić historię wypożyczeń (użytkownik wszystkie swoje historyczne i obecne wypożyczenia, bibliotekarz historię wszystkich wypożyczeń)
- Użytkownik anonimowy (bez tworzenia konta) może przeglądać katalog książek, ale nie może ich rezerwować, ani wypożyczać
- W modelu książki zamiast pola ceny jest pole gatunku
- Wyświetlany jest status wypożyczenia danej książki (Availabe, Reserved, Checked out, Permanently unavailable - wyświetlane tylko dla bibliotekarza)
- Bibliotekarz może ręcznie usunąć rezerwację książki
- Stronicowanie widoku listy książek

## Realizacja rozwiązania
- ASP.NET Core Identity - Uwierzytelnianie i autoryzacja, role-based authorization
    - Dostęp do konkretnych akcji jest blokowany dla użytkowników bez przypisanej odpowiedniej roli (Librarian, Reader)
    - Użytkownicy są przekierowywani do właściwych dla swojej roli akcji
- Entity Framework - zarządzanie bazą danych (SQL Server LocalDB), mapowanie relacji obiektów, migracje
- MVC
    - Models:
        - ApplicationUser - użytkownicy systemu
        - Book - książka
        - BookGenreViewModel - model umożliwiający wyszukiwanie książek po gatunku wybieranym z rozwijanej listy
        - Reservation - rezerwacje
        - ReservationBookViewModel - model rezerwacji poszerzony o podstawowe informacje o książce
        - Checkout - wypożyczenia
        - CheckoutBookViewModel - model wypożyczeń poszerzony o podstawowe informacje o książce
    - Controllers:
        - HomeController
        - BooksController
        - ReservationsController
        - CheckoutsController
    - Views:
        - Shared: - layaut, ...
        - Home:
            - Index
        - Books:
            - Index, IndexLibrarian, IndexReader
            - Details, DetailsLibrarian
            - Create
            - Delete
            - Edit
        - Reservations:
            - Index, IndexLibrarian, IndexReader
            - Reserve
            - Unreserve
        - Checkouts:
            - Index, IndexLibrarian, IndexReader
            - Checkout
            - Endcheckout

## Obsługa współbieżności
Zaimplementowano wariant optymistycznej współbieżności. Do modelu `Book` dodano pole `RowVersion` z atrybutem `[Timestamp]`, które jest tablicą bajtów. Wartość tego pola jest automatycznie zwiększana po każdej modyfikacji danego rekordu w bazie danych.

Podczas wykonywania modyfikacji danego rekordu (`Update`, `Delete`) kolumna `RowVersion` zostaje automatycznie uwzględniona w klauzuli `Where`. Jeżeli aktualizowany wiersz został zmieniony przez innego użytkownika, to wartości `RowVersion` w bazie danych i w poleceniu modyfikacji będą się różnić. W wyniku tego zostanie zgłoszony wyjątek `DbUpdateConcurrencyError`, który jest obsługiwany na poziomie kontrolerów.

## Migracje
Kolejne zastosowane migracje znajdują się w katalogu *Data/Migrations/*.