using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LooneyTunesVac
{


    class Program
    {
        static void Main(string[] args)
        {

            if (args[0] == null) throw new Exception("No valid arguments.");
            if (!args[0].Substring(args[0].LastIndexOf('.') + 1).Equals("TLB")) throw new Exception("Not a vali TLB file!");


            using (StreamReader tlb = new StreamReader(new FileStream(args[0], FileMode.Open)))
            {
                tlb.ReadLine(); //~ToneLibrary
                tlb.ReadLine(); //{

                int tlbVersion = int.Parse(tlb.ReadLine().Trim().Split(' ')[1]);
                int tones = int.Parse(tlb.ReadLine().Trim().Split(' ')[1]);
                string dataFile = tlb.ReadLine().Trim();
                dataFile = dataFile.Substring(dataFile.IndexOf("\"") + 1).Replace("\"", "").ToUpper();
                if (!File.Exists(dataFile)) Console.Out.WriteLine("No corresponding VAC existent!");
                else
                {
                    //Populate files from TonesLibrary
                    Tone[] tonesList = new Tone[tones];

                    for (int i = 0; i < tones; i++)
                    {
                        tlb.ReadLine(); //~Tone
                        tlb.ReadLine(); //{

                        int version = int.Parse(tlb.ReadLine().Trim().Split(' ')[1]);
                        string name = tlb.ReadLine().Trim();
                        name = name.Substring(name.IndexOf("\"") + 1).Replace("\"", "") + ".pingusamp";
                        int offset = int.Parse(tlb.ReadLine().Trim().Split(' ')[1]);
                        int frequency = int.Parse(tlb.ReadLine().Trim().Split(' ')[1]);
                        bool loopFlag = Boolean.Parse(tlb.ReadLine().Trim().Split(' ')[1]);
                        tlb.ReadLine(); //empty line
                        tlb.ReadLine(); // } // Tone
                        
                        tonesList[i] = new Tone(name, offset, frequency, loopFlag);

                    }
                    int channels = 1;
                    //Grab info, write tones





                    using (BinaryReader vac = new BinaryReader(new FileStream(dataFile, FileMode.Open)))
                    {
                        for (int i = 0; i < tones; i++)
                        {
                            using (BinaryWriter outSmp = new BinaryWriter(new FileStream(tonesList[i].name, FileMode.Create)))
                            {
                           
                                int size = 0;
                                //Last file check
                                if (i == (tones - 1)) size = (int)vac.BaseStream.Length - tonesList[i].offset;
                                else size = tonesList[i + 1].offset - tonesList[i].offset;
                                //Console.Out.WriteLine("Name: " + tonesList[i].name + " Size: " + size);
                                outSmp.Write(channels);
                                outSmp.Write(tonesList[i].frequency);
                                byte[] data = vac.ReadBytes(size);
                                outSmp.Write(data);

                            }

                        }



                    }


                    

                }
            }


            //Console.ReadKey();
        }


    }
}

public class Tone
{
    public string name { get; set; }
    public int offset { get; set; }
    public int frequency { get; set; }
    public bool loopFlag { get; set; }

    public Tone(string name, int offset, int frequency, bool loopFlag)
    {
        this.name = name;
        this.offset = offset;
        this.frequency = frequency;
        this.loopFlag = loopFlag;
    }

}
