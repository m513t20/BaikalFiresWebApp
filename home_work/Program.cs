// See https://aka.ms/new-console-template for more information
using System.Runtime.CompilerServices;
using System.Text.Json;

using GeoJsonConvertor.Models;
using GeoJsonConvertor.Filtration;
using GeoJsonConvertor.Recieve;
using HistoryDataString;

//object
internal class Program
{
    private static void Main(string[] args)
    {

   
        Console.WriteLine("Hello, Worlds!");
        if (args.Length<2) throw new ArgumentException("неверное количество параметров");
        var file_path = "fires_2011-2021.geojson";
        var firesInfo = File.ReadAllText(file_path);
        var data = JsonSerializer.Deserialize<GeoInformation>(firesInfo);

        var file_settings="settings.json";
        var settings_info = File.ReadAllText(file_settings);
        var settings_str = JsonSerializer.Deserialize<Settings>(settings_info);
        

        using var connect=new Npgsql.NpgsqlConnection(settings_str.ConnectionString);
        connect.Open();

        //конец загрузки
        Console.WriteLine("Load success");

        //количество строк
        int size_of = data.features.Count();

        // создаем список нашей модели
        StatisticString[] fire_data = new StatisticString[size_of];

        //переносим данные
        for (int index = 0; index < size_of; index++)
        {
            Properties cur_string = data.features[index].properties;
            StatisticString temporal = new StatisticString(cur_string.name_ru, cur_string.lat, cur_string.lon, cur_string.init_date.Year, cur_string.init_date.Month);
            fire_data[index] = temporal;
        }


        //получаем регионы
        var regions = data.features.Select(x=> x.properties.name_ru).Distinct().Select(
            x => new Region(Guid.NewGuid().ToString(), 
            x)).ToList();

        // var sqlTemplate = "insert into public.regions(guid, name) values('{0}', '{1}')";
        // foreach(var region in regions)
        // {  
        //     var insertSql = string.Format(sqlTemplate, region.RegionId, region.RegionName);
        //     var command = new Npgsql.NpgsqlCommand(insertSql, connect);
        //     command.ExecuteNonQuery();
        // }



        //данные о пожарах
        var fires=data.features.Select(x=>new {regionName=x.properties.name_ru,
                period=x.properties.init_date,
                fire_count=1});

        
        //перевод в History data
        var info=fires.Select(x=> new{
            regionId= regions.First(y=>y.RegionName==x.regionName).RegionId,
            year=x.period.Year,
            month=x.period.Month
        }).Select(x=> new HistoryData(Guid.NewGuid().ToString(), x.regionId,x.year,x.month));


        

        //фильтрация
        var tst=new FiltrationFactory(info.ToList());
        tst=tst.Filter(args[0],ArgsManager.GetArgs(args[1]));
        


        //перевод в словарь по регионам
        var result=regions.Select(x=> new 
        {Key=x, Value=tst.data.Where(y=> y.RegionId==x.RegionId).ToList()}).ToDictionary(x=>x.Key,x=>x.Value);

        



        //написание таблицы
        foreach(var cur_region in result.Keys){
            Console.WriteLine($"{cur_region.RegionName}:\t{result[cur_region].Count()}");
//             var sqlTemplateReg=@"insert into public.fire_history(guid, region_id, year_period, month_period)
//                             select '{0}', id as region_id, {1}, {2} from public.regions where guid = '{3}' limit 1"
// ;
        
//             var region=result[cur_region];

//             foreach(var row in region){
//                 var insert_sql=string.Format(sqlTemplateReg,row.RecordId,row.year,row.month,cur_region.RegionId);

//                 var command=new Npgsql.NpgsqlCommand(insert_sql,connect);
//                 command.ExecuteNonQuery();
            
//             }



        }


                                                                            

        Console.WriteLine(result);


    }
}



//урок
record Region(string RegionId, string RegionName );
record Settings(string ConnectionString,string ConnectionStringFeoktistov);

namespace HistoryDataString
{
    public record HistoryData(string  RecordId, string RegionId, int year, int month);

}
