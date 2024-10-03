using System.Numerics;

namespace sql_comands;

public class SqlManager{
    private string connection_path=string.Empty;
    private Npgsql.NpgsqlConnection connect=new();
    private string command_template="select {t2}.{request}, {agrigation} from {t1} inner join {t2} on {t1}.{fk_column} = {t2}.{ik_column} group by {t2}.{request}";
    private string comand_entry="SELECT request_word,t1,t2,group_by_field FROM public.entry_table WHERE id={0}";

    public SqlManager(string path){
        connection_path=path;
        connect=new Npgsql.NpgsqlConnection(connection_path);
    }

    public string CreateRequest(int entry_id){
        connect.Open();
        var cmd=string.Format(comand_entry,entry_id);
        var command=new Npgsql.NpgsqlCommand(cmd,connect);
        var res=command.ExecuteReader();
        while(res.Read()){

        }        


        connect.Close();
        return "";
    }

}