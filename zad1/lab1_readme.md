# Narzędzia typu RAD
## Zadanie 1 - Biblioteka w ASP.MVC
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

## Dodatkowe funkcjonalności
- Użytkownik może wyszukiwać książki (a także swoje rezerwacje i wypożyczenia), wpisując tytuł, autora lub wybierając gatunek książki
- Bibliotekarz może dodawać nowe książki do biblioteki
- Użytkownik i bibliotekarz mogą wyświetlić historię wypożyczeń
- Użytkownik anonimowy (bez tworzenia konta) może przeglądać katalog książek, ale nie może ich rezerwować, ani wypożyczać
- Zamiast pola ceny jest pole gatunku
- Wyświetlany status wypożyczenia danej książki

### Realizacja rozwiązania
- ASP.NET Core Identity - Uwierzytelnianie i autoryzacja, role-based authorization
    - Dostęp do konkretnych akcji jest blokowany dla użytkowników bez przypisanej odpowiedniej roli (Librarian, Reader)
- Entity Framework - zarządzanie bazą danych (SQL Server LocalDB)
- MVC
    - Models
        - ApplicationUser - użytkownicy systemu
        - Book - książka
        - BookGenreViewModel - model umożliwiający wyszukiwanie książek po gatunku wybieranym z rozwijanej listy
        - Reservation - rezerwacje
        - ReservationBookViewModel - model rezerwacji poszerzony o podstawowe informacje o książce
        - Checkout - wypożyczenia
        - CheckoutBookViewModel - model wypożyczeń poszerzony o podstawowe informacje o książce
    - Controllers
        - HomeController
        - BooksController
        - ReservationsController
        - CheckoutsController
    - Views
        - Shared
        - Home
            - Index
        - Books
            - Index, IndexLibrarian, IndexReader
            - Details, DetailsLibrarian
            - Create
            - Delete
            - Edit
        - Reservations
            - Index, IndexLibrarian, IndexReader
            - Reserve
            - Unreserve
        - Checkouts
            - Index, IndexLibrarian, IndexReader
            - Checkout
            - Endcheckout