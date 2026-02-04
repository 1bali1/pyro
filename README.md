<p align="center">
  <a href="#">
    <img src="pyro/Assets/banner.png" alt="banner">
  </a>
</p>


# Pyro Backend (C#)
- Pyro Backend új C# változata, Python helyett

<hr>

## Futtatás
**1. Telepítés**
Töltsd le a dependecyket az alábbi paranccsal:
```bash
dotnet restore
```

**2. Beállítások**
- **Környezeti változók**: `Írd át a .env.example nevét .env-re, és add meg a szükséges kulcsaidat`
- **Config**: `A Config/config.json fájlban állítsd be a saját beállításaidat`
- **Itemshop**: `Az itemshop, és autoitem shop beállításait a Config/itemshop.json fájlban tudod elvégezni`

**3. Indítás**
Futtasd az alábbi parancsot a root mappába:
```bash
dotnet run --project pyro
```

<hr>

## Hasznos információk
- A hírekbe ajánlott használni a (kb) 2558px 1440px szélességet és magasságot, hogy biztosan jól jelenjen meg a kép
- (Nincs tesztelve több verzión) Az itemshopban valószínüleg mindig a régi UI van, és ott az alábbi címek máshogyan értendőek: 
  - **Featured**(Játékba) -> Heti tárgyak(Configba)
  - **Daily**(Játékba) -> Napi tárgyak(Configba)
  - **Special Offers**(Játékba) -> Featured tárgyak(Configba)