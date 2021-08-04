# CountryChatbotExercise

**Note**: Strasznie trudno było mi wyłuskać best practices z tych milionów tutoriali, odpowiedzi na stackoverflow itd. Na przykład rozbawiło mnie to, że jedni mówią że HttpContext.Session to Cud nad Wisłą, inni mówią że jest przestarzały i zamiast niego powinno się używać ViewBag/ViewData, a kolejni - że to wszystko to jeden wielki code smell. Trochę mnie to denerwuje, że nie jestem w stanie się upewnić, czy to co ja wymyśliłem i stworzyłem jest zgodne z konwencją i ma sens w kontekście kultury WebDevu.

![obraz](https://user-images.githubusercontent.com/10108473/115216490-d75bf080-a104-11eb-9aa4-a70886023b61.png)

Backend mógłby być pewnie dużo bardziej SOLIDny, ale nie miałem niestety tak dużo czasu w weekend jak oczekiwałem, więc ściąłem parę zakrętów (powiedzmy, że w myśl YAGNI).

Projet Web to właściwie tylko frontend: Index zawiera historię wypowiedzi oraz zwykły HTMLowy input, z którego przechwytujemy surowym jsem wiadomości, renderujemy je, POSTujemy do ChatControllera (który je z kolei przesyła chatbotowi) i renderujemy odpowiedzi z backendu.

W projekcie Core mamy przede wszystkim przede wszystkim model konwersacji oraz wiadomosci (kazda konwersacja zawiera liste wiadomosci) oraz CountryChatbotService, który zajmuje się zarządzaniem konwersacjami. Jak dostaje nową wiadomość bez id, to nadaje jej id, zaczyna trackowac i jeszcze MessageProcessor rozpoczyna pierwsza wiadomość od wesołego 'Hello!'. Wszystkie wiadomości zapisuje do odpowiednich konwersacji (i trzyma je na ten moment w pamięci, dlatego jest Singletonem, co mi się nie podoba - powinien mieć jakiś obiekt repozytorium bazy (sql, sqlite, nosql, nieważne) służący do insertowania i usuwania wiadomości, wtedy ChatbotService nie musialby byc singletonem i system w calosci bylby ciut bardziej modularny i latwiejszy do testow jednostkowych.

MessageProcessor zajmuje sie przemieleniem wiadomosci kilkuetapowo:
* za pomoca Levenshteina znajduje nazwe panstwa o ktora chodzi userowi (w razie literowek)
* prosi RestCountryProvider o podanie obiektu panstwa o tej nazwie (a RestCountryProvider ma w sobie cache tych panstw, wiec jezeli pojawil sie w przeszlosci taki request, to Provider wtedy nie odpyta API restcountries tylko od razu zwroci obiekt ktory ma zapisany)
* generuje zdanie "Did you know that {country.Name}'s {property1} is {value1}, {property2} is {value2} and {property3} is {value3}?"
* jezeli ma oznaczona flage ze to nowa konwersacja to do odpowiedzi dodaje na poczatku "Hello!"
* zwraca sklejone zdanie chatbotowi

Dużym błędem z mojej strony było to, że MessageProcessor zajmuje się procesowaiem Message.Body zamiast całego DTO Message, przez co chatbot dodatkowo zajmuje się rozwiązywaniem wiadomości pożegnalnych (żeby zakończyć konwersację), co już w ogóle morduje SRP. Gdybym miał więcej czasu, to bym to przerobił.
