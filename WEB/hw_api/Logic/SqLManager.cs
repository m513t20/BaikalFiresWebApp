using System.Data;
using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Npgsql;
using SqlConfig;


namespace SqlManager;

public class SqlService{


    Request config=new Request();
    public SqlService(){
    }

    public Dictionary<string,string> GetMetricKeywords(int metric=1){
        using var dataSource = new NpgsqlDataSourceBuilder(config.connection).Build();
        using var connection = dataSource.OpenConnection();
        var sql=string.Format(config.sql_request,metric);
        using var command = new NpgsqlCommand(sql, connection);

        // Выполняем запрос 
        using  var adapter = new NpgsqlDataAdapter(command);
        var table = new DataTable();
        adapter.Fill(table);
        var res=new List<string>();
        var ret=new Dictionary<string,string>();
        foreach(var cur_col in table.Columns){
            res.Add(cur_col.ToString());
        }

        foreach (var cur_row in table.Rows){
            foreach (var col in res){
                ret.Add(col,table.Rows[0][col].ToString());
            }
        }
        if (ret["is_t1"]=="True"){
            ret.Add("t1_gr",ret["t2"]);
        }
        else{
            ret.Add("t1_gr",ret["t1"]);
        }

        return ret;




        // // using var connection=new Npgsql.NpgsqlConnection(config.connection);
        // // connection.Open();
        // // var command=new Npgsql.NpgsqlCommand(sql,connection);
        // string[] res=new string[8];
        // // using (NpgsqlDataReader reader=command.ExecuteReader()){
        // //     while (reader.Read()){
        // //         for (int i=0;i<8;i++){
        // //             res[i]=reader.GetString(i);
        // //         }
        // //     }
        // // }
        // // connection.Close();
        // return res;     

    }

    public List<Dictionary<string,string>> GetMetric(Dictionary<string,string> args){


        using var dataSource = new NpgsqlDataSourceBuilder(config.connection_study).Build();
        using var connection = dataSource.OpenConnection();
        
        var sql=config.format_request;


        foreach(var key in args.Keys){
            sql=sql.Replace($"{key}@",args[key]);
        }
        
        using var command = new NpgsqlCommand(sql, connection);

        // Выполняем запрос 
        using  var adapter = new NpgsqlDataAdapter(command);
        var table = new DataTable();
        adapter.Fill(table);
        var res=new List<Dictionary<string, string>>() ;

        for (int cur_row_ind=0;cur_row_ind<table.Rows.Count; cur_row_ind++){
            var key=table.Rows[cur_row_ind][table.Columns[0].ToString()].ToString();
            var amount=table.Rows[cur_row_ind][table.Columns[1].ToString()].ToString();
            res.Add(new Dictionary<string, string>());
            res[cur_row_ind].Add("Key",key);
            res[cur_row_ind].Add("Val",amount);
        }



        return res;
    }
}


   
