using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API
{
    public class Db
    {

        public static void Save()
        {

            var jsonD = JsonConvert.SerializeObject(DirectorService.GetAll());
            var jsonM = JsonConvert.SerializeObject(MovieService.GetAll());
            var jsonA = JsonConvert.SerializeObject(ActorService.GetAll());

            var filePathD = "./DB/directors.json";
            var filePathM = "./DB/movies.json";
            var filePathA = "./DB/actors.json";

            File.WriteAllText(filePathD, jsonD);
            File.WriteAllText(filePathM, jsonM);
            File.WriteAllText(filePathA, jsonA);
        }

        public static void Read()
        {
            MovieService.Read();
            DirectorService.Read();
            ActorService.Read();
        }


        //jesli usunie sie film to musi sie zmienic ocena directora 
        // mozna to dac do innej klasy zeby wiecej bylo ale nwm jaka nazwa XD
        

        // po update to samo jak zmienimy ocene _/

        //jak dodamy direcotra co ma film ktory juz istnieje to trzeba w filmie go tez dodac

        //jak dodam ocene filmu od razu a nie z rate to tez trzeba to uwzglednic _/


    }
}
