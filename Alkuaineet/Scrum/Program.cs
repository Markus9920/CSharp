using System;
using Newtonsoft.Json;
using System.Text.Json;
namespace SCRUM
{
    //Käyttäjä voi testata taitotasoaan hetkellisesti ja lisäksi ohjelma laskee keskiarvoa kaikilta käyttökerroilta.
    //Tee ohjelmalle komentorivi käyttöliittymä eli tyyliin haluatko pelata (p) vai tarkastella tuloksia (t)
    //Jos käyttäjä pelaa niin ohjelma käyttäjältä viittä alkuainetta ensimmäisten 20 alkuaineen joukosta. 
    //Ohjelman pitää lukea oikeat vastaukset tiedostosta alkuaineet.txt. Lopuksi ohjelma kertoo käyttäjälle montako 
    //meni oikein ja montako väärin. 
    //Ohjelma ei saa hyväksyä samaa vastausta useammin kuin yhden kerran.
    //Tuloksen kertomisen jälkeen ohjelma tekee työhakemistoon hakemiston muotoa pvkkvvvv (mikäli ei vielä olemassa) ja 
    //sinne tiedoston tulokset.json (Mikäli ei vielä olemassa) ja lisää aina tähän tiedostoon uudelle riville saadun 
    //tuloksen (numero arvo joko 0-5 tai 0%-100%)

    //kannattaa nämä asentaa. kopioi teksti konsoliin ja paina enter.
    //dotnet add package System.Text.Json
    //dotnet add package Newtonsoft.Json 
    class Program
    {
        //no actual code written by me, only the base of "class program" and "Main" etc...
        public static void Main(string[] args)
        {
            ChemicalElementProgram chemicalElementProgram = new ChemicalElementProgram();
            Inquiry inquiry = new Inquiry();
            bool continueCheck = true;

            while (continueCheck)
            {
                chemicalElementProgram.BeforeStart();
                chemicalElementProgram.CheckIfContinue(chemicalElementProgram);
            }
        }
    }
}