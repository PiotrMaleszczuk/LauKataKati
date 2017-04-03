%%%%%%%%%%%%  SZTUCZNA INTELIGENCJA  %%%%%%%%%%%%

:- dynamic pozycja/3.

%Sprawdza, czy A jest różne od B i C jest różne od D.
czyRozne(A,B,_,_) :- dif(A,B), !.
czyRozne(_,_,C,D) :- dif(C,D), !.

%Zwraca wszystkie pozycje pionków gracza
wszystkiePozycje('B', X1, Y1, X2, Y2, X3, Y3, X4, Y4) :- pozycja(X1, Y1, 'B1'), pozycja(X2, Y2, 'B2'), pozycja(X3, Y3, 'B3'), pozycja(X4, Y4, 'B4'), !.
wszystkiePozycje('R', X1, Y1, X2, Y2, X3, Y3, X4, Y4) :- pozycja(X1, Y1, 'R1'), pozycja(X2, Y2, 'R2'), pozycja(X3, Y3, 'R3'), pozycja(X4, Y4, 'R4'), !.

%Sprawdza, czy wszystkie elementy od pierwszego elementu na liście są pomiędzy MIN a MAX.
czyPomiedzy([T|R], MIN, MAX) :- between(MIN, MAX, T), czyPomiedzy(R, MIN, MAX).
czyPomiedzy([], _, _).

%Zwraca wszystkie pionki zależnie od koloru.
zwrocPionki('B', ['B1', 'B2', 'B3', 'B4']).
zwrocPionki('R', ['R1', 'R2', 'R3', 'R4']).

%Zwraca nazwę przeciwnika.
zamienGracza('R', 'B').
zamienGracza('B', 'R').

%Zwrócenie początkowych elementów spośród wszystkich elementów pierwszego argumentu listy.
zwrocPierwsze([], []) :- !.
zwrocPierwsze([[T|_]|R2], [T|R3]) :- zwrocPierwsze(R2, R3).

%Zwraca min lub max listy w zależności od pierwszego argumentu.
minLubMax(1, L, V) :- max_list(L, V).
minLubMax(-1, L, V) :- min_list(L, V).

%Ostatni nieustawiony pionek.
ostatniPionek(Gracz, Pionek) :- zwrocPionki(Gracz, PioneksList), zwrocNieustawionyPionek(PioneksList, Pionek).

%Zwraca pierwszy nieustawiony pionek.
zwrocNieustawionyPionek([M|_], M) :- not(pozycja(_,_,M)), !.
zwrocNieustawionyPionek([T|R], M) :- pozycja(_,_,T), zwrocNieustawionyPionek(R, M).

%Sprawdza, czy gracz grający czarnymi pionkami wygrywa.
zwyciezca('B') :-
    pozycja(B1X, B1Y, 'B1'),
    pozycja(B2X, B2Y, 'B2'),
    pozycja(B3X, B3Y, 'B3'),
    pozycja(B4X, B4Y, 'B4'),
    zwyciezca(B1X, B1Y, B2X, B2Y, B3X, B3Y, B4X, B4Y), !.

%Sprawdza, czy gracz grający czerwonymi pionkami wygrywa.
zwyciezca('R') :-
    pozycja(R1X, R1Y, 'R1'),
    pozycja(R2X, R2Y, 'R2'),
    pozycja(R3X, R3Y, 'R3'),
    pozycja(R4X, R4Y, 'R4'),
    zwyciezca(R1X, R1Y, R2X, R2Y, R3X, R3Y, R4X, R4Y), !.

%Wykrywa ustawienie pionków dla wygranej w poziomie.
zwyciezca(M1X, M1Y, M2X, M2Y, M3X, M3Y, M4X, M4Y) :- 
    permutation([[X1, Y], [X2, Y], [X3, Y], [X4, Y]], [[M1X, M1Y], [M2X, M2Y], [M3X, M3Y], [M4X, M4Y]]),
    czyPomiedzy([X1, X2, X3, X4, Y], 1, 5),
    X2 is X1 + 1, X3 is X2 + 1, X4 is X3 + 1.

%Wykrywa ustawienie pionków dla wygranej w pionie.
zwyciezca(M1X, M1Y, M2X, M2Y, M3X, M3Y, M4X, M4Y) :-
    permutation([[X, Y1], [X, Y2], [X, Y3], [X, Y4]], [[M1X, M1Y], [M2X, M2Y], [M3X, M3Y], [M4X, M4Y]]),
    czyPomiedzy([X, Y1, Y2, Y3, Y4], 1, 5),
    Y2 is Y1 + 1, Y3 is Y2 + 1, Y4 is Y3 + 1.

%Wykrywa ustawienie pionków dla wygranej po przekątnej.
zwyciezca(M1X, M1Y, M2X, M2Y, M3X, M3Y, M4X, M4Y) :-
    permutation([[X1, Y1], [X2, Y2], [X3, Y3], [X4, Y4]], [[M1X, M1Y], [M2X, M2Y], [M3X, M3Y], [M4X, M4Y]]),
    czyPomiedzy([X1, X2, X3, X4, Y1, Y2, Y3, Y3], 1, 5),
    Y2 is Y1 + 1, X2 is X1 + 1, Y3 is Y2 + 1, X3 is X2 + 1, Y4 is Y3 + 1, X4 is X3 + 1.
    
%Wykrywa ustawienie pionków dla wygranej po przeciwnej przekątnej.
zwyciezca(M1X, M1Y, M2X, M2Y, M3X, M3Y, M4X, M4Y) :-
    permutation([[X1, Y1], [X2, Y2], [X3, Y3], [X4, Y4]], [[M1X, M1Y], [M2X, M2Y], [M3X, M3Y], [M4X, M4Y]]),
    czyPomiedzy([X1, X2, X3, X4, Y1, Y2, Y3, Y3], 1, 5),
    Y2 is Y1 + 1, X2 is X1 - 1, Y3 is Y2 + 1, X3 is X2 - 1, Y4 is Y3 + 1, X4 is X3 - 1.
    
%Wykrywa ustawienie pionków dla wygranej w kwadracie.
zwyciezca(M1X, M1Y, M2X, M2Y, M3X, M3Y, M4X, M4Y) :-
    permutation([[X1, Y1], [X2, Y1], [X2, Y2], [X1, Y2]], [[M1X, M1Y], [M2X, M2Y], [M3X, M3Y], [M4X, M4Y]]),
    czyPomiedzy([X1, X2, Y1, Y2], 1, 5),
    X2 is X1 + 1, Y2 is Y1 + 1.

%Obliczenie położenia pionka.
obliczPolozenie(X, Y, M) :-
    czyPomiedzy([X, Y], 1, 5),
    not(pozycja(X, Y, _)),
    not(pozycja(_, _, M)).

%Ustawienie pionka na planszy.
ustawPionek(X, Y, M) :- nonvar(X), nonvar(Y), nonvar(M), obliczPolozenie(X, Y, M), assert(pozycja(X, Y, M)), !.
ustawPionek(PosX, PosY, X, Y, M) :- nonvar(X), nonvar(Y), nonvar(M), obliczPolozenie(X, Y, M), assert(pozycja(X, Y, M)), PosX = X, PosY = Y, !.

%Usunięcie pionka z planszy.
usunPionek(M) :- nonvar(M), pozycja(X, Y, M), retract(pozycja(X, Y, M)), !.

%Obliczenie wszystkich możliwych pozycji do ustawienia dla pionka i sprawdzenie, czy pionek jest pierwszym, który może być ustawiony.
obliczWszystkiePozycje(C, M, X, Y) :-
    ostatniPionek(C, M),
    obliczPolozenie(X, Y, 'NE').

%Wykonanie ruchu pionka, przesunięcie go do innej pozycji z poprzedniej.
przesunPionek(TOX, TOY, M) :- pozycja(FROMX, FROMY, M), obliczRuch(TOX, TOY, M), assert(pozycja(TOX, TOY, M)), retract(pozycja(FROMX, FROMY, M)), !.
przesunPionek(PosX, PosY, PosXO, PosYO, TOX, TOY, M) :- pozycja(FROMX, FROMY, M), obliczRuch(TOX, TOY, M), assert(pozycja(TOX, TOY, M)), retract(pozycja(FROMX, FROMY, M)), PosX is TOX, PosY is TOY, PosXO is FROMX, PosYO is FROMY, !.

%Obliczenie wszystkich możliwych ruchów dla wszystkich pionków danego koloru.
obliczWszystkieMozliweRuchy(C, M, TOX, TOY) :-
    zwrocPionki(C, LM),
    member(M, LM),
    obliczRuch(TOX, TOY, M).

%Obliczenie ruchu dla pionka.
obliczRuch(TOX, TOY, M) :-
    pozycja(FROMX, FROMY, M),
    czyPomiedzy([TOX, TOY], 1, 5),
    MAXX is FROMX + 1, MINX is FROMX - 1, czyPomiedzy([TOX], MINX, MAXX),
    MAXY is FROMY + 1, MINY is FROMY - 1, czyPomiedzy([TOY], MINY, MAXY),
    czyRozne(TOX, FROMX, TOY, FROMY),
    not(pozycja(TOX, TOY, _)).

%Obliczenie zbioru wszystkich ruchów w zależności od fazy rozgrywki.
obliczWszystkieRuchy(C, M, X, Y) :- ostatniPionek(C, M), obliczWszystkiePozycje(C, M, X, Y).
obliczWszystkieRuchy(C, M, X, Y) :- not(ostatniPionek(C, M)), obliczWszystkieMozliweRuchy(C, M, X, Y).

%Predykat dla algorytmu sztucznej inteligencji.
najkorzystniejszyRuch(C, _, _, M,3,3):- obliczLiczbeRuchow(C,0, _), not(pozycja(3,3,_)), ostatniPionek(C, M),!.
najkorzystniejszyRuch(C, _, _, M,X,Y):- obliczLiczbeRuchow(C,0, _), ostatniPionek(C, M), random(RndX), random(RndY), X is 2 + 2 * round(RndX), Y is 2 + 2 * round(RndY),!.
najkorzystniejszyRuch(C, L, IAL, M, X, Y) :- najkorzystniejszyRuch(C, L, IAL, _, 1, -200, 200, [M, X, Y]).

%Zwraca najlepsze możliwe przesunięcie pionka dla sztucznej inteligencji.
szansa(3).
szansaAI(3) :- random(X), X < 0.8.
najkorzystniejszyRuch(C, 0, L, X, _, _, _, _) :- zamienGracza(C, C1), ocen(L, C1, X).
najkorzystniejszyRuch(C, _, L, X, MINMAX, _, _, _) :- zwyciezca(C), szansaAI(L), X is MINMAX*100.
najkorzystniejszyRuch(C, _, L, X, MINMAX, _, _, _) :- zamienGracza(C, C2), zwyciezca(C2), szansa(L), X is -1*MINMAX*90.
najkorzystniejszyRuch(C, V, IAL, RE, MINMAX, ALFA, BETA, ADD) :- dif(V, 0), findall([C, M, X, Y], obliczWszystkieRuchy(C, M, X, Y), L), VAL is -1 * MINMAX * 200, zmianaPolozenia(L, V, IAL, LR, MINMAX, ALFA, BETA, VAL), zwrocPierwsze(LR, LV), minLubMax(MINMAX, LV, RE), nth1(_, LR, [RE|ADD]), !.

%Główna część algorytmu zależna od fazy gry (rozstawienie pionków lub ich przemieszczanie po rozstawieniu).
zmianaPolozenia([[C, M, X, Y]|R], V, IAL, [TE|RE], MINMAX, ALFA, BETA, VAL) :-
    ostatniPionek(C, M),
    ustawPionek(X, Y, M), zamienGracza(C, C2), V1 is V - 1, INVMINMAX is -1 * MINMAX,
    najkorzystniejszyRuch(C2, V1, IAL, T, INVMINMAX, ALFA, BETA, _), TE = [T, M, X, Y], usunPionek(M),
    minLubMax(MINMAX, [VAL, T], NEWVAL), alfaBetaCiecie(MINMAX, NEWVAL, ALFA, BETA, IAL, R, V, RE).
zmianaPolozenia([[C, M, X, Y]|R], V, IAL, [TE|RE], MINMAX, ALFA, BETA, VAL) :-
    not(ostatniPionek(C, M)),
    pozycja(FROMX, FROMY, M), przesunPionek(X, Y, M), zamienGracza(C, C2), V1 is V - 1, INVMINMAX is -1 * MINMAX,
    najkorzystniejszyRuch(C2, V1, IAL, T, INVMINMAX, ALFA, BETA, _), TE = [T, M, X, Y], przesunPionek(FROMX, FROMY, M),
    minLubMax(MINMAX, [VAL, T], NEWVAL), alfaBetaCiecie(MINMAX, NEWVAL, ALFA, BETA, IAL, R, V, RE).
zmianaPolozenia([], _, _, [], _, _, _, _).

%Koniec algorytmu dla ruchu sztucznej inteligencji, sprawdza, czy alfa-beta cięcia są możliwe.
alfaBetaCiecie(-1, VAL, ALFA, _, _, _, _, []) :- ALFA >= VAL.
alfaBetaCiecie(-1, VAL, ALFA, BETA, IAL, R, V, RE) :- min_list([BETA, VAL], BETA2), zmianaPolozenia(R, V, IAL, RE, -1, ALFA, BETA2, VAL).
alfaBetaCiecie(1, VAL, _, BETA, _, _, _, []) :- VAL >= BETA.
alfaBetaCiecie(1, VAL, ALFA, BETA, IAL, R, V, RE) :- max_list([ALFA, VAL], ALFA2), zmianaPolozenia(R, V, IAL, RE, 1, ALFA2, BETA, VAL).

%Funkcja oceniająca.
ocen(_,C,100):- zwyciezca(C),!.
ocen(D,C,-100):- D > 1, zamienGracza(C,C1), zwyciezca(C1),!.
ocen(3,C,N):- obliczLiczbeRuchow(C, NbMC, LMC), NbMC<4, zamienGracza(C,C1),obliczLiczbeRuchow(C1, NbMC1, LMC1), NbMC1<4, !, obliczZwyciestwo(C,NWS, NbMC, LMC), obliczZwyciestwo(C1,NWS1, NbMC1, LMC1), N is (NbMC*NWS - 1.5*NbMC1*NWS1).
ocen(3,C,N):- obliczLiczbeRuchow(C, 4, _), zamienGracza(C,C1),obliczLiczbeRuchow(C1, 4, _), !,obliczUstawienie(C,NA), obliczUstawienie(C1,NA2), N is (4*NA - 7*NA2).
ocen(3,C,N):- obliczUstawienie(C,NA), zamienGracza(C,C1), obliczUstawienie(C1,NA2), obliczLiczbeRuchow(C, NbMC, LMC),obliczLiczbeRuchow(C1, NbMC1, LMC1),obliczZwyciestwo(C,NWS, NbMC, LMC), obliczZwyciestwo(C1,NWS1, NbMC1, LMC1), N is (4*NA - 7*NA2 + NbMC*NWS - NbMC1*NWS1).

%Oblicza liczbę pionków ustawionych dla danego koloru (sprawdza, czy wszystkie pola wymagane do wygranej nie są zajęte przez pionki przeciwnika).
obliczUstawienie(C,N):- wszystkiePozycje(C, _, _, X2, Y2, X3, Y3, X4, Y4), zwyciezca(X, Y, X2, Y2, X3, Y3, X4, Y4), czyPomiedzy([X,Y], 1, 5), sprawdzPustePole(X,Y,EC),!, N is 3*EC.
obliczUstawienie(C,N):- wszystkiePozycje(C, X1, Y1, _, _, X3, Y3, X4, Y4), zwyciezca(X1, Y1, X, Y, X3, Y3, X4, Y4), czyPomiedzy([X,Y], 1, 5), sprawdzPustePole(X,Y,EC),!, N is 3*EC.
obliczUstawienie(C,N):- wszystkiePozycje(C, X1, Y1, X2, Y2, _, _, X4, Y4), zwyciezca(X1, Y1, X2, Y2, X, Y, X4, Y4), czyPomiedzy([X,Y], 1, 5), sprawdzPustePole(X,Y,EC),!, N is 3*EC.
obliczUstawienie(C,N):- wszystkiePozycje(C, X1, Y1, X2, Y2, X3, Y3, _, _), zwyciezca(X1, Y1, X2, Y2, X3, Y3, X, Y), czyPomiedzy([X,Y], 1, 5), sprawdzPustePole(X,Y,EC),!, N is 3*EC.
obliczUstawienie(_,0).

%Sprawdza, czy pole o koordynatach X, Y jest puste.
sprawdzPustePole(X,Y,1):- not(pozycja(X,Y,_)),!.
sprawdzPustePole(_,_,0.2).

%Oblicza liczbę wygrywających pozycji możliwych dla pionków w pozostałych dla nich miejscach.
obliczZwyciestwo(C,T, N, L):- N<4, zamienGracza(C1,C), obliczPozycjeN(4,L,[X1, Y1, X2, Y2, X3, Y3, X4, Y4]), setof([[X1, Y1], [X2, Y2], [X3, Y3], [X4, Y4]], (zwyciezca(X1, Y1, X2, Y2, X3, Y3, X4, Y4), czyPomiedzy([X1, Y1, X2, Y2, X3, Y3, X4, Y4], 1, 5), pionkiPrzeciwnika(C1,[X1, Y1, X2, Y2, X3, Y3, X4, Y4])), L2),usunDuplikat(L2,L3), length(L3, T),!.
obliczZwyciestwo(_,0,_,_).

%Oblicza liczbę postawień pionka danego koloru. Zwraca pozycje pionków sąsiadujących do niego.
obliczLiczbeRuchow(C,N, L2):- findall([X,Y], (zwrocPionki(C, L), member(P,L), pozycja(X,Y,P)),L2), length(L2, N).

%Tworzy listę zawierającą N pozycji wzbogaconą o koordynaty pionków przekazywanych jako parametr.
obliczPozycjeN(0, _, []):-!.
obliczPozycjeN(N, [], [_,_|R]):- N1 is N-1, obliczPozycjeN(N1, [], R),!.
obliczPozycjeN(N, [[X,Y]|R], [X,Y|R2]):-  N1 is N-1, obliczPozycjeN(N1, R, R2).

%Sprawdza, czy pozycje zawarte na liście nie są zajmowane przez pionki gracza.
pionkiPrzeciwnika(_,[]):- !.
pionkiPrzeciwnika(C,[X,Y|R]):- zwrocPionki(C,[C1,C2,C3,C4]),not(pozycja(X,Y,C1)),not(pozycja(X,Y,C2)),not(pozycja(X,Y,C3)),not(pozycja(X,Y,C4)), pionkiPrzeciwnika(C,R).

%Usuwanie duplikatów
usunDuplikat([],[]):-!.
usunDuplikat([T|R],R2):- member(T2,R), obliczIdentyczne(T,T2), usunDuplikat(R,R2),!.
usunDuplikat([T|R],[T|R2]):-usunDuplikat(R,R2).

%Sprwadza, czy dwie pozycje są identyczne.
obliczIdentyczne([],_):-!.
obliczIdentyczne([T|R],L):-member(T,L),obliczIdentyczne(R,L).

%%%%%%%%%%%%  ETAP 0 - WSTĘPNA KONFIGURACJA GRY  %%%%%%%%%%%%

%Definicje predykatów zmieniających się w trakcie działania programu.
:- dynamic sztucznaInteligencja/1.

%Początkowe przypisanie zmiennych.
start:-
        assert(sztucznaInteligencja(3)).

%%%%%%%%%%%%  ETAP 1 - ROZSTAWIENIE PIONKÓW  %%%%%%%%%%%%

rozstawienie(Gracz):-
        not(uruchomPrzemieszczanie),
        zamienGracza(Gracz, Przeciwnik), not(zwyciezca(Przeciwnik)), !.

%Po rozstawieniu wszystkich pionków sprawdza, czy nie ma zwycięzcy.
rozstawienieOstatni(Gracz):-
        zamienGracza(Gracz, Przeciwnik), zwyciezca(Przeciwnik), !.

%Rozstawienie pionka przez człowieka.
rozstawienieCzlowiek(Pos, Gracz):-
        ostatniPionek(Gracz, Pionek), wprowadzPozycje(Pos, Pionek), !.

%Rozstawienie pionka przez sztuczną inteligencję.
rozstawienieAI(PosX, PosY, Gracz):-
        sztucznaInteligencja(L), 
		najkorzystniejszyRuch(Gracz, 3, L, M, X, Y), 
		ustawPionek(PosX, PosY, X, Y, M), !.

uruchomPrzemieszczanie:-
        not(ostatniPionek('B', _)), not(ostatniPionek('R', _)), !,
        not(zwyciezca('B')), not(zwyciezca('R')).

%Zapisanie pionka gracza dla wprowadzonej pozycji.
wprowadzPozycje(Pos, Pionek):-
        ostatniPionek('B', Pionek),
        name(Pos, ListaKoordynatow), uzyskajKoordynaty(Pionek, ListaKoordynatow, X, Y),
        not(pozycja(X, Y, _)), !, ustawPionek(X, Y, Pionek).

%Uzyskanie koordynatów pionka postawionego przez gracza.
uzyskajKoordynaty(_, [], _, _).
uzyskajKoordynaty(_, [X, Y], X1, Y1):-
        X1 is X - 48, Y1 is Y - 48,
        czyPomiedzy([X1, Y1], 1, 5), !.

%%%%%%%%%%%%  ETAP 2 - PRZESTAWIANIE PIONKÓW  %%%%%%%%%%%%

wykonajRuch(Gracz):-
        zamienGracza(Gracz, Przeciwnik), not(zwyciezca(Przeciwnik)), !.

%Przesunięcie wybranego pionka na wybraną pozycję przez gracza.		
wykonajRuchCzlowiek(Gracz, Move):-
        name(Move, ListaPrzemieszczenia), sprawdzPoprawnoscRuchu(Gracz, ListaPrzemieszczenia), !.

%Wykonanie przemieszczenia przez sztuczną inteligencję.
wykonajRuchAI(PosX, PosY, PosXO, PosYO):-
        sztucznaInteligencja(L), najkorzystniejszyRuch('R', 3, L, Pionek, X, Y), przesunPionek(PosX, PosY, PosXO, PosYO, X, Y, Pionek), !.

%Sprawdzanie poprawności komendy przemieszczenia pionka w liniach prostych.
sprawdzPoprawnoscRuchu(Gracz, [_, Wartosc, ProstyKierunekWartosc]):-
        NumerPionka is Wartosc - 48,
        between(1, 4, NumerPionka), !,
        concat(Gracz, NumerPionka, Pionek),
        name(ProstyKierunek, [ProstyKierunekWartosc]),
        ruchProsty(Pionek, ProstyKierunek).

%Sprawdzanie poprawności komendy przemieszczenia pionka po skosie.
sprawdzPoprawnoscRuchu(Gracz, [_, Wartosc, PierwszyKierunekWartosc, DrugiKierunekWartosc]):-
        NumerPionka is Wartosc - 48,
        between(1, 4, NumerPionka), !,
        concat(Gracz, NumerPionka, Pionek),
        name(FirstMove, [PierwszyKierunekWartosc]),
        name(SecondMove, [DrugiKierunekWartosc]),
        ruchZlozony(Pionek, FirstMove, SecondMove).
		
%Przemieszczenie pionka do zadanej pozycji przez gracza w liniach prostych.
ruchProsty(Pionek, ProstyKierunek):-
        pozycja(X, Y, Pionek),
        mozliweRuchy(X, Y, ProstyKierunek, NewX, NewY), not(pozycja(NewX, NewY, _)), !,
        przesunPionek(NewX, NewY, Pionek).

%Przemieszczenie pionka do zadanej pozycji przez gracza po skosie.
ruchZlozony(Pionek, FirstMove, SecondMove):-
        pozycja(X, Y, Pionek),
        mozliweRuchy(X, Y, FirstMove, NewX1, NewY1),
        mozliweRuchy(NewX1, NewY1, SecondMove, NewX2, NewY2),
        not(pozycja(NewX2, NewY2, _)), !,
        przesunPionek(NewX2, NewY2, Pionek).

%Możliwe ruchy proste dla pionka.
mozliweRuchy(X, Y, 'g', X, NewY):- NewY is Y - 1, czyPomiedzy([NewY], 1, 5), !.
mozliweRuchy(X, Y, 'd', X, NewY):- NewY is Y + 1, czyPomiedzy([NewY], 1, 5), !.
mozliweRuchy(X, Y, 'p', NewX, Y):- NewX is X + 1, czyPomiedzy([NewX], 1, 5), !.
mozliweRuchy(X, Y, 'l', NewX, Y):- NewX is X - 1, czyPomiedzy([NewX], 1, 5), !.

%Uruchomiane podczas wczytania pliku.
:- start.