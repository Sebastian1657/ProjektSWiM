# 🚗 Autonomiczny Pojazd z STM32

Projekt semestralny z przedmiotu **Systemy Wbudowane i Mikrokontrolery**  
Autorzy: _[Sebastian Miler, Mateusz Brodzik, Bartłomiej Maczkowski]_  
Numer indeksu: _[21277, 21219, 21262]_  
Data rozpoczęcia: _[10.04.2025]_  
Repozytorium zawiera kod, dokumentację oraz materiały projektowe.

---

## 📌 Opis projektu

Celem projektu jest opracowanie modelu autonomicznego pojazdu sterowanego za pomocą mikrokontrolera STM32. Pojazd porusza się w trybie półautomatycznym lub automatycznym, omija przeszkody i/lub śledzi linię. Komunikacja z użytkownikiem odbywa się przez UART (Bluetooth).

---

## 🛠️ Zastosowane technologie i narzędzia

- **Mikrokontroler:** STM32 (model np. STM32FE303)
- **IDE:** STM32CubeIDE
- **Programowanie:** C (HAL / LL)
- **Sensory:**
  - Sensory optyczne (IR)
  - Sensory ultradźwiękowe
- **Zasilanie:** Akumulator NiMH 9.6V
- **Sterownik silników:** np. L298N
- **Komunikacja:** UART Bluetooth HM-10

---

## ⚙️ Funkcjonalności

- ✅ Napęd sterowany przez PWM z użyciem Timerów
- ✅ Obsługa sensorów ultradźwiękowych (pomiar odległości)
- ✅ Odczyt wartości z sensorów IR (linia / przeszkody) przy użyciu ADC
- ✅ Detekcja kolizji i unikanie przeszkód przy użyciu sensorów ultradźwiękowych
- ✅ Sterowanie ruchem przez UART (sterowanie zdalne, poprzez polecenia oraz niezależna jazda)
- ✅ Zasilanie bateryjne – pełna autonomia
- ✅ Regularne wersjonowanie kodu (min. 1 commit/tydzień)

---

## 📁 Struktura repozytorium


---

## 🔌 Komendy UART

| Komenda | Opis                    |
|--------:|-------------------------|
| `START` | Uruchamia pojazd       |
| `STOP`  | Zatrzymuje pojazd      |
| `LEFT`  | Skręt w lewo           |
| `RIGHT` | Skręt w prawo          |
| `DIST?` | Zwraca odczyt z HC-SR04 |

---

## 🧪 Scenariusze testowe

- [x] Detekcja przeszkody z przodu (sensor HC-SR04)
- [x] Reakcja na białą/czarną linię (IR)
- [x] Komunikacja przez Bluetooth
- [x] Test zasilania bateryjnego
- [x] Sterowanie ruchem w czasie rzeczywistym

---

## 📸 Demo i zdjęcia

- Zdjęcia pojazdu: [`/media/photos/`](./media/photos/)

---

## 📄 Dokumentacja

Pełna dokumentacja projektu znajduje się w folderze [`docs/`](./docs/), w tym:
- Raport końcowy (PDF)
- Schematy układów
- Lista komponentów
- Raporty z milestonów

---

## 📅 Harmonogram pracy

- Tydzień 1–2: Koncepcja i lista komponentów  
- Tydzień 3–5: Budowa pojazdu i montaż elektroniki  
- Tydzień 6–9: Programowanie sensorów i napędu  
- Tydzień 10–12: Komunikacja UART + testy  
- Tydzień 13–14: Finalizacja, dokumentacja, raport  

---

## 🧠 Wnioski

Roboty są super!

---

## 📬 Kontakt

W razie pytań:
- GitHub: https://github.com/Sebastian1657
- GitHub: https://github.com/maczkowski-bartlomiej
---

**Licencja:** MIT  
