using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    
    public class Łyba
    {
        
        public string Nazwa { get; set; }
        public int Zycie { get; set; }
        public int Atak { get; set; }
        public int Wartosc { get; set; } 

        
        public Łyba(string nazwa, int zycie, int atak, int wartosc)
        {
            Nazwa = nazwa;
            Zycie = zycie;
            Atak = atak;
            Wartosc = wartosc;
        }

        
        public virtual void Plywaj()
        {
            Console.WriteLine($"{Nazwa} pływa spokojnie...");
        }

        public virtual void WydajDzwiek()
        {
            Console.WriteLine($"{Nazwa} wydaje cichy dźwięk pod wodą.");
        }

        public virtual void Atakuj()
        {
            Console.WriteLine($"{Nazwa} atakuje z siłą {Atak}!");
        }
    }

    public class ZlotaŁyba : Łyba
    {
        public ZlotaŁyba() : base("Złota Rybka", 10, 0, 50) 
        {
        }

        public override void Plywaj()
        {
            Console.WriteLine($"{Nazwa} pływa");
        }

        public override void WydajDzwiek()
        {
            Console.WriteLine($"{Nazwa} nie wydaje dźwięków.");
        }
    }

    public class Łekin : Łyba
    {
        public Łekin() : base("Łekin", 100, 20, 200) 
        {
        }

        public override void Plywaj()
        {
            Console.WriteLine($"{Nazwa} pływa");
        }

        public override void WydajDzwiek()
        {
            Console.WriteLine($"{Nazwa} wydaje  dźwięki ");
        }

        public override void Atakuj()
        {
            Console.WriteLine($"{Nazwa} atakuje  {Atak}!");
        }
    }

    public class Swiatecznykarp : Łyba
    {
        public Swiatecznykarp() : base("Światecznykarp", 20, 5, 75) 
        {
        }

        public override void Plywaj()
        {
            Console.WriteLine($"{Nazwa} pływa");
        }

        public override void WydajDzwiek()
        {
            Console.WriteLine($"{Nazwa} wydaje dzwiek");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            
            Łyba zwyklaŁyba = new Łyba("Zwykła Łyb", 15, 2, 20);
            Łyba zlotaŁyba = new ZlotaŁyba();
            Łyba Łekin = new Łekin();
            Łyba Swiatecznykarp = new Swiatecznykarp();

            
            TestujRybe(zwyklaŁyba);
            TestujRybe(zlotaŁyba);
            TestujRybe(Łekin);
            TestujRybe(Swiatecznykarp);
        }

        static void TestujRybe(Łyba Łyba)
        {
            Console.WriteLine($"--- {Łyba.Nazwa} ---");
            Łyba.Plywaj();
            Łyba.WydajDzwiek();
            Łyba.Atakuj();
            Console.WriteLine($"Życie: {Łyba.Zycie}, Atak: {Łyba.Atak}, Wartość: {Łyba.Wartosc}");
            Console.WriteLine();
        }
    }
}
