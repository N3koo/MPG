A. Interfata de nivel I (Supervisare/Configurari) trebuie sa contina urmatoarele tab-uri
1. Planificare fabricatie
2. Urmarire fabricatie
3. Raportare fabricatie
4. Administrare-Mentenanta
.................. Detaliere interfata nivel I...................
1. Planificare fabricatie
- interfata trebuie sa contina informatii privind urmarirea status-ul comenzilor de fabricatie -> [DONE]
- status-urile comenzilor de fabricatie sunt : -> [DONE]
	-> BLOC=Blocat (Comanda blocata); 
	-> ELB=Eliberat (nelansate in fabricatie-receptionate din MES/SAP);
	-> PRLS=Lansat (Lansat in productie); PRLI= Intrerupt (Fabricatie intrerupta/stopata); 
	-> PRLT= Finalizat (Fabricatie finalizata);
- sa fie afisat nr. comenzii clientului respectiv clientul pentru care s-a deschis aceasta comanda (acolo unde este cazul) -> [SE VA DEZVOLTA]

-> Posibilitatea de a schimba prioritatea (cand termin o comanda scad prioritatea pentru celelalte) -> [DONE]
	-> Prioritatea nu se schimba dupa ce a fost transmisa -> [DONE]
	-> Prioritate 0 pentru ce este ultra urgent -> [DONE] {Posibil sa fie necesar un mesaj de informare}
	-> Predare partiala pt ce este gata facut (PRLT)
	-> Descriere status prin buton de info -> [DONE]
	-> Eliminare 0 din fata comenzilor -> [DONE]
	-> Dezactivare interfata atunci cand se incarca comenzile -> [DONE]
	-> Verificare functionalitati
2. Urmarire fabricatie
21/23.05.22 -> S-a actulizat viata mea
- trebuie evidentiate notele de predare care s-au creat cu referinta la comenzi (nr. nota predare, cantitate predata, data document) -> [DONE]
- evidentiere bonuri de consum generate cu referinta la comenzi (nr. bon de consum, data document) -> [DONE]
- sa se poata urmari cate galeti s-au realizat dintr-o cda, cate mai sunt de realizat, cate sunt in celelalte zone de exemplu (in asteptare pentru predare) -> [Pt alta versiune]

		De adaugat:
	-> Afisare cantitate neta pentru fiecare galeata -> [DONE]
	-> Unitate de masura -> [DONE]
	-> Adaugare camp pentru cod produs -> [DONE]
	-> Kober Lot [PP ca e intern si va ramane asa] + numar de lot [Same but from exterior] -> [DONE]
	-> Volum -> [Done] [Pentru comanda va fi numarul total, iar pt galeata va fi cantitatea]
	-> Data predarii -> [DONE]
	-> Filtru pentru comanda si status -> [DONE]
	-> Filtrare dupa cod produs (V8810-B30 pentru prima comanda PRLT) -> [DONE]
	
3. Raportare fabricatie
- raport privind comenzile lansate/fabricate intr-o anumita perioada;
- urmarirea intarzierilor fata de data planificata;
- diferente dintre planificat versus realizat;
- statistici privind controlul de calitate

		Discutii:
	Rapoartele sa fie in forma tabelara si sa poata fi exportate pentru inchiderea de luna din contabilitate. (cantitatea coloranti, coloranti, QC)
	Propun un raport general la nivel de comanda si un raport detaliat la nivel de comanda si galeata.
	Sa poata fi realizate cu filtrul principal perioada. -> [DONE]
	raportl principal: 
		Comanda(fara 0 de inceput asa cum s-a discutat deja), -> [DONE]
		Lot Kober, -> [DONE]
		Cod produs, -> [DONE]
		Denumire produs (descriere material) -> [DONE]
		cantitatea comanda -> [DONE]
		si cantitatea predata in buc si in kg, -> [DONE]
		data ora de start si finalizare planificate -> [DONE]
		si realizate si statusul final al comenzii -> [DONE]
	raportul detaliat sa fie pentru fiecare galeata si in plus sa aiba precizate
	precizate galetile la care s-a efectuat control de calitate -> [DONE]
	numarul de corectii si daca se poate cat reprezinta suma corectiilor in kg fata de suma cantitatilor in kg initial dozate.
	filtrarea sa poata fi facuta dupa Lot si dupa cod produs -> [DONE]
	

4. Administrare-Mentenanta
- in acest modul trebuie sa se poata configura accesul utilizatorilor la optiunile aplicatiei;
- configurarea parametrilor/constantelor de lucru;
- definirea procedurilor de mentenanta si testare a componentelor sistemului de automatizare

B. Interfata nivel II - executie
- de detaliat de catre Alex si Andrei
................. Alte Observatii .....................
a) La nivelul aplicatiei sa existe un mecanism de arhivare automata a datelor pentru descongestionarea tabelelor curente
b) Definirea/eetarea parametrilor sa se poata face de la distanta, de la calculatorul inginerului tehnolog;
c) MPG-ul va avea propria lui gestiune;
d) Aplicatia MPG trebuie sa genereze transferuri automate din sectia VI catre sectia alocata MPG-ului;
e) MPG-ul trebuie sa faca rezervari (el este prioritar);
f) Prioritatile intre sectie si MPG se stabilesc de catre directorul de fabrica;
g) Blocarea unei comenzi se face numai daca nu s-au generat BP si BC

FLINK WINDRIVER DOCUMENTE DESCRIERE
3 imprimante(2 la fel si una la sfarsit)
Materiale -> 77k

!!!!!!!StringBuffer.append(Linie)
