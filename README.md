Din minnes hanterare!

Här i denna simpla console app kan du lägga till och hantera saker du ska komma ihåg med några enkla knapptryck!
----------------------------------------
Den är uppbyggd i C#, i grunden  skulle den spara ner listan/or i  .json.
Men detta blev ett problem för programmet behövde admin behörighet.
Försökte lösa detta problemet i app.manifest och ändra så den fick admin behörighet för att  "skriva".
Detta gick tyvärr fortfarande inte så hopppade till Plan B och ändrade så  den sparar de i .txt istället.
I .txt dokument så sparas de på detta  vis med kommatecken emellan: (titel, datum, klar/ej klar, projekt).
---------------------------------------
Applikationen är public så de är bara att clona!
