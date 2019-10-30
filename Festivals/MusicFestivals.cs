using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

namespace Festivals
{
    public class Band
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("recordLabel")]
        public string RecordLabel { get; set; }
    }

    public class Festivals
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("bands", Required = Required.Always)]
        public Band[] Bands { get; set; }
    }

    class RequiredBand
    {
        [JsonProperty("name")]
        public string BandName { get; set; }

        [JsonProperty("name")]
        public List<string> FestivalNames { get; set; }
    }

    class RequiredFormat
    {
        public string RecordLabel { get; set; }

        public RequiredBand[] RequiredBands { get; set; }
    }

    class MyWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest wr = base.GetWebRequest(address);
            wr.Timeout = 5000; // timeout in milliseconds (ms)
            return wr;
        }
    }

    class MusicFestivals
    {
        private List<Festivals> musicFestivals = new List<Festivals>();
        private List<RequiredFormat> requiredFormat = new List<RequiredFormat>();
        private List<string> recordLabel = new List<string>();

        public bool GetApi()
        {
            try{
                using (MyWebClient wc = new MyWebClient()){
                    var json = wc.DownloadString("testFile.json");
                    musicFestivals = JsonConvert.DeserializeObject<List<Festivals>>(json);
                    if (musicFestivals == null || musicFestivals.Count == 0)
                        throw new Newtonsoft.Json.JsonException();
                }
            }
            catch (WebException we){
                Console.WriteLine(we.Message);
                return false;
            }
            catch (Newtonsoft.Json.JsonException e){
                Console.WriteLine(e.Message);
                return false;
            }
            recordLabel = GetRecordLabels(musicFestivals);
            requiredFormat = InstantiateRequiredFormat(musicFestivals, recordLabel);
            SortBandName(requiredFormat);
            PrintRequiredFormat(requiredFormat);

            return true;
        }

        private List<string> GetRecordLabels(List<Festivals> musicFestivals)
        {
            List<string> recordLabel = new List<string>();

            foreach (var musicFestival in musicFestivals){
                foreach (var value in musicFestival.Bands){
                    if (!recordLabel.Contains(value.RecordLabel))
                        recordLabel.Add(value.RecordLabel);
                }
            }
            recordLabel.Sort();

            return recordLabel;
        }

        //Funtion below creates the template required 
        private List<RequiredFormat> InstantiateRequiredFormat(List<Festivals> musicFestivals, List<string> recordLabels)
        {
            List<RequiredBand> requiredbands = new List<RequiredBand>();
            List<RequiredBand> bands = new List<RequiredBand>();
            RequiredBand band = new RequiredBand();
            RequiredFormat format = new RequiredFormat();
            List<string> festivals = new List<string>();
            List<string> names = new List<string>();

            foreach (var recordLabel in recordLabels){
                format = new RequiredFormat{
                    RecordLabel = recordLabel
                };
                names = new List<string>();
                foreach (var musicFestival in musicFestivals){
                    foreach (var value in musicFestival.Bands){
                        if (value.RecordLabel == recordLabel){
                            if (!names.Contains(value.Name)){
                                names.Add(value.Name);
                                festivals = new List<string>();
                                festivals.Add(musicFestival.Name);
                                band = new RequiredBand{
                                    BandName = value.Name,
                                    FestivalNames = festivals
                                };
                                bands.Add(band);
                            }
                            else{
                                for(int i = 0; i < bands.Count; i++){
                                    if(bands[i].BandName == value.Name){
                                        List<string> temp = new List<string>();
                                        temp = bands[i].FestivalNames;
                                        temp.Add(musicFestival.Name);
                                        temp.Sort();
                                        bands[i].FestivalNames = temp;
                                    }
                                }
                            }
                        }
                    }
                }
                format.RequiredBands = bands.ToArray();
                bands = new List<RequiredBand>();
                requiredFormat.Add(format);
            }
            return requiredFormat;
        }

        private List<RequiredFormat> SortBandName(List<RequiredFormat> requiredFormat)
        {
            foreach (var format in requiredFormat){
                for (int i = 0; i < format.RequiredBands.Length - 1; i++){
                    if (format.RequiredBands.Length < 2)
                        continue; // If only one band, continue. 
                    if (format.RequiredBands[i].BandName.CompareTo(format.RequiredBands[i + 1].BandName) > 0){
                        RequiredBand temp = new RequiredBand{
                            BandName = format.RequiredBands[i + 1].BandName,
                            FestivalNames = format.RequiredBands[i + 1].FestivalNames
                        };
                        format.RequiredBands[i + 1].BandName = format.RequiredBands[i].BandName;
                        format.RequiredBands[i + 1].FestivalNames = format.RequiredBands[i].FestivalNames;
                        format.RequiredBands[i].BandName = temp.BandName;
                        format.RequiredBands[i].FestivalNames = temp.FestivalNames;
                        i = -1;
                    }
                }
            }
            return requiredFormat;
        }

        private void PrintRequiredFormat(List<RequiredFormat> requiredFormat)
        {
            foreach (var format in requiredFormat){
                if (format.RecordLabel == "")
                    Console.WriteLine("Record Label:''");
                else
                    Console.WriteLine(format.RecordLabel != null ? "Record label: " + format.RecordLabel : "Record label:");
                foreach (var value in format.RequiredBands){
                    Console.WriteLine(value.BandName != null ? "     Band name: " + value.BandName : null);
                    foreach(var festivalName in value.FestivalNames)
                    if (festivalName == "")
                        Console.WriteLine("     Festival name:''");
                    else
                        Console.WriteLine(festivalName != null ? "     Festival name: " + festivalName : "     Festival name:");
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
        }
    }
}
